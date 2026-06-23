using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Firebase.Installer.Editor
{
    /// <summary>
    /// Firebase UPM 타르볼은 용량(app ~95MB)이 커서 Git/LFS에 넣지 않는다.
    /// manifest.json 은 file:../GooglePackages/*.tgz 상대경로로 패키지를 참조하므로,
    /// 새 환경(클론/CI)에서는 이 메뉴로 타르볼을 Google CDN에서 GooglePackages/ 에 받아야 한다.
    ///
    /// 이 스크립트는 참조가 없는 독립 에디터 어셈블리(Firebase.Installer.Editor)에 들어있다.
    /// 타르볼이 없어 Firebase 패키지가 미해결이면 Assembly-CSharp 전체가 컴파일 실패하지만,
    /// 이 어셈블리는 그와 무관하게 컴파일되므로 그 상태에서도 메뉴가 항상 표시된다.
    /// </summary>
    internal static class FirebasePackageInstaller
    {
        // =====================================================================
        // ⚠️  메뉴를 누르기 전에 이 구역을 확인/수정하세요.
        //     - FirebaseVersion : 사용할 Firebase 버전 (기본값 = 최신)
        //     - Modules         : 프로젝트가 실제로 쓰는 Firebase 모듈만 남기기
        //     사용 가능한 모듈 예: app(필수), auth, database, firestore, storage,
        //                          analytics, messaging, functions, remoteconfig,
        //                          crashlytics, installations, appcheck, dynamiclinks ...
        //     EdmVersion 은 거의 바뀌지 않음 (firebase.app 의 의존 버전).
        // =====================================================================
        private const string FirebaseVersion = "13.12.0";
        private const string EdmVersion = "1.2.186";

        private static readonly string[] Modules =
        {
            "app",        // 필수 (Firebase Core)
            "auth",
            "database",
            // "firestore",
            // "storage",
            // "analytics",
            // "messaging",
        };
        // =====================================================================

        private const string CdnBase = "https://dl.google.com/games/registry/unity";

        // Application.dataPath = <project>/Assets → 부모가 프로젝트 루트(= Packages/ 의 sibling).
        private static string PackagesDir =>
            Path.Combine(Path.GetDirectoryName(Application.dataPath), "GooglePackages");

        /// <summary>(id, version) 패키지 목록을 설정값에서 빌드. app 은 항상 포함.</summary>
        private static List<(string id, string version)> BuildPackageList()
        {
            var list = new List<(string, string)>
            {
                ("com.google.external-dependency-manager", EdmVersion),
            };

            bool hasApp = false;
            foreach (var m in Modules)
            {
                if (m == "app") hasApp = true;
                list.Add(($"com.google.firebase.{m}", FirebaseVersion));
            }
            if (!hasApp)
                list.Insert(1, ($"com.google.firebase.app", FirebaseVersion));

            return list;
        }

        [MenuItem("Tools/Firebase/Install Packages")]
        private static void InstallMissing() => Run(forceReinstall: false);

        [MenuItem("Tools/Firebase/Reinstall Packages")]
        private static void Reinstall() => Run(forceReinstall: true);

        private static void Run(bool forceReinstall)
        {
            var packages = BuildPackageList();

            // 다운로드 전 버전/모듈 확인 알림.
            var sb = new StringBuilder();
            sb.AppendLine($"Firebase 버전: {FirebaseVersion}   (EDM {EdmVersion})");
            sb.AppendLine();
            sb.AppendLine("받을 패키지:");
            foreach (var (id, version) in packages)
                sb.AppendLine($"  • {id}-{version}");
            sb.AppendLine();
            sb.AppendLine("버전/모듈이 맞나요? 다르면 [취소] 후 이 스크립트 상단의");
            sb.AppendLine("FirebaseVersion / Modules 를 수정하고 다시 실행하세요.");

            if (!EditorUtility.DisplayDialog("Firebase Installer", sb.ToString(), "계속", "취소"))
                return;

            string dir = PackagesDir;
            Directory.CreateDirectory(dir);

            int downloaded = 0;
            try
            {
                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromMinutes(10);

                    for (int i = 0; i < packages.Count; i++)
                    {
                        var (id, version) = packages[i];
                        string fileName = $"{id}-{version}.tgz";
                        string finalPath = Path.Combine(dir, fileName);

                        if (!forceReinstall && File.Exists(finalPath) && new FileInfo(finalPath).Length > 0)
                            continue;

                        string url = $"{CdnBase}/{id}/{fileName}";
                        if (!DownloadToFile(http, url, finalPath, fileName, i, packages.Count))
                        {
                            // 사용자 취소 또는 실패 — 진행 중단.
                            EditorUtility.ClearProgressBar();
                            return;
                        }
                        downloaded++;
                    }
                }
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog(
                    "Firebase Installer",
                    $"다운로드 중 오류가 발생했습니다:\n{e.Message}\n\n" +
                    "버전이 존재하지 않으면 404 가 납니다. 상단 FirebaseVersion 을 확인하세요.",
                    "확인");
                Debug.LogException(e);
                return;
            }
            EditorUtility.ClearProgressBar();

            if (downloaded == 0)
            {
                EditorUtility.DisplayDialog(
                    "Firebase Installer",
                    "모든 패키지가 이미 GooglePackages/ 에 있습니다. (Reinstall 로 강제 재다운로드)",
                    "확인");
                return;
            }

            // 타르볼이 새로 생겼으니 UPM 재해석 → Firebase import → 자동 재컴파일.
            Client.Resolve();
            Debug.Log($"[Firebase Installer] {downloaded}개 패키지를 {dir} 에 받았습니다. UPM 재해석 중...");
        }

        /// <summary>임시 파일로 스트리밍 다운로드 후 성공 시 교체. 취소/실패 시 false.</summary>
        private static bool DownloadToFile(HttpClient http, string url, string finalPath,
            string label, int index, int count)
        {
            string tmpPath = finalPath + ".tmp";
            if (File.Exists(tmpPath)) File.Delete(tmpPath);

            try
            {
                using (var resp = http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result)
                {
                    resp.EnsureSuccessStatusCode();
                    long total = resp.Content.Headers.ContentLength ?? -1L;

                    using (var src = resp.Content.ReadAsStreamAsync().Result)
                    using (var dst = File.Create(tmpPath))
                    {
                        byte[] buffer = new byte[1 << 16]; // 64KB
                        long read = 0;
                        int n;
                        while ((n = src.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            dst.Write(buffer, 0, n);
                            read += n;

                            float fileFrac = total > 0 ? (float)read / total : 0f;
                            float overall = (index + fileFrac) / count;
                            string info = total > 0
                                ? $"{label}  ({read / (1024f * 1024f):F1} / {total / (1024f * 1024f):F1} MB)"
                                : $"{label}  ({read / (1024f * 1024f):F1} MB)";

                            if (EditorUtility.DisplayCancelableProgressBar(
                                    $"Firebase 패키지 다운로드 ({index + 1}/{count})", info, overall))
                            {
                                dst.Dispose();
                                if (File.Exists(tmpPath)) File.Delete(tmpPath);
                                return false;
                            }
                        }
                    }
                }

                if (File.Exists(finalPath)) File.Delete(finalPath);
                File.Move(tmpPath, finalPath);
                return true;
            }
            catch
            {
                if (File.Exists(tmpPath)) File.Delete(tmpPath);
                throw;
            }
        }
    }
}
