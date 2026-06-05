using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class EditorScreenshot : EditorWindow
{
    // 유니티 상단 메뉴에 'Tools > Capture Game View' 메뉴 추가
    [MenuItem("Tools/Capture Game View")]
    public static void CaptureGameView()
    {
        // Game 뷰가 열려있는지 확인하고 포커스 지정
        System.Type gameViewType = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        EditorWindow gameView = EditorWindow.GetWindow(gameViewType);

        if (gameView != null)
        {
            gameView.Focus();

            // 파일명 및 경로 설정 (프로젝트 루트 폴더/Screenshots/)
            string folderPath = Path.Combine(Application.dataPath, "../Screenshots");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = $"Editor_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
            string fullPath = Path.Combine(folderPath, fileName);

            // ScreenCapture API로 에디터 상태의 Game 뷰 캡처
            ScreenCapture.CaptureScreenshot(fullPath);

            // 프로젝트 에셋 새로고침 및 로그 출력
            AssetDatabase.Refresh();
            Debug.Log($"[캡처 완료] 저장 경로: {fullPath}");
        }
        else
        {
            Debug.LogError("Game 뷰를 찾을 수 없습니다. Game 창을 열어주세요.");
        }
    }
}