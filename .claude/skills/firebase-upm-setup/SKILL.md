---
name: Firebase UPM Setup
description: >-
  Unity 프로젝트의 Firebase 를 .unitypackage(Assets/) 설치에서 Git/LFS 에 올라가지 않는
  로컬 UPM 타르볼 방식으로 마이그레이션/설치한다. Firebase, UPM, tarball, .tgz,
  unitypackage 정리, ExternalDependencyManager, GooglePackages, Git 제외, manifest.json
  관련 작업이거나 "파이어베이스를 깃에 안 올리고 싶다" 류 요청일 때 사용.
disable-model-invocation: true
allowed-tools: Read Grep Glob Edit Write Bash(ls *) Bash(cat *) Bash(find *) Bash(curl *) Bash(git *) Bash(python *)
---

# Firebase UPM Setup (.unitypackage → 로컬 UPM 타르볼)

Unity 프로젝트의 Firebase 를, **용량 큰 SDK 바이너리를 Git/LFS 에 넣지 않는** 방식으로 전환한다.
manifest 는 `file:../GooglePackages/*.tgz` 상대경로(= 환경 독립)로 패키지를 참조하고, 실제 타르볼은
gitignore 한 `GooglePackages/` 폴더에 **Unity 에디터 메뉴로 1회 다운로드**한다.

이 스킬은 **수동 전용**(`/firebase-upm-setup`)이며, 파일을 **삭제**하는 단계가 있으니
각 파괴적 작업 전에 반드시 사용자 확인을 받는다.

## 배경 사실 (이미 검증됨)
- Firebase 는 공식 UPM 레지스트리가 없어 로컬 `.tgz` 만 가능.
- 개별 타르볼 다운로드 URL: `https://dl.google.com/games/registry/unity/{id}/{id}-{ver}.tgz`
  - 예: `com.google.firebase.auth-13.12.0.tgz`, `com.google.external-dependency-manager-1.2.186.tgz`
- 다운로드는 번들된 에디터 인스톨러(`Tools > Firebase` 메뉴)가 수행한다. **이 스킬이 CLI 로 받지 않는다.**
- 정리 대상/보존 대상의 상세 목록: [reference/cleanup-unitypackage.md](reference/cleanup-unitypackage.md)

---

## 절차

### 1. 탐지
대상 프로젝트(현재 작업 폴더)의 상태를 조사한다:
- `Packages/manifest.json` 위치와 내용. `scopedRegistries` 에 google/firebase/openupm/maven 관련 항목이 있는지.
- `.unitypackage` 설치 흔적: `Assets/Firebase/`, `Assets/ExternalDependencyManager/`,
  `Assets/PlayServicesResolver/`, `Assets/Plugins/Android` 와 `Assets/Plugins/iOS` 의 Firebase 산출물,
  `Assets/GeneratedLocalRepo/`. **비어 있는 폴더는 설치로 보지 않는다.**
- **어떤 모듈을 쓰는지** 파악한다(`Assets/Firebase/Plugins` 의 `FirebaseAuth.dll`,
  `FirebaseFirestore.dll`, `FirebaseDatabase.dll` 등 dll 이름 → auth/firestore/database).
  이미 UPM 으로 일부 전환돼 있으면 manifest 의 `com.google.firebase.*` 항목에서 모듈을 읽는다.
- **버전은 재해석하지 않는다.** 기본값은 최신(현재 `13.12.0`). 사용자가 특정 버전을 원하면 그때 반영.

탐지 결과를 요약해 사용자에게 보여준다.

### 2. 계획 제시 + 동의
다음을 명확히 보여주고 진행 동의를 받는다:
- **제거**될 폴더/파일 목록 (1단계에서 실제로 발견된 것만)
- **추가**될 UPM 의존성: `com.google.external-dependency-manager-1.2.186` + 탐지된 각 모듈
  (`com.google.firebase.app` 필수 포함), 버전 기본 `13.12.0`
- 새로 생길 파일: `GooglePackages/`, `Assets/FirebaseUpmInstaller/Editor/`, `.gitignore` 항목

### 3. 정리 (cleanup) — 각 삭제 전 사용자 확인
[reference/cleanup-unitypackage.md](reference/cleanup-unitypackage.md) 를 따른다.
- 제거 대상 폴더/파일과 짝 `.meta` 를 삭제.
- **`google-services.json` / `GoogleService-Info.plist` / `google-services-desktop.json` 는 보존**
  (설정 입력값이지 SDK 가 아님). 사용자 작성 코드도 손대지 않는다.
- `Assets/Plugins/Android`·`iOS` 는 통째로 지우지 말고 Firebase/Google 산출물만 골라 제거.

### 4. 프로비저닝 (UPM 전환)
1. 프로젝트 루트(= `Packages/` 의 sibling)에 `GooglePackages/` 폴더 생성.
2. 번들 템플릿을 대상 프로젝트로 복사:
   - `${CLAUDE_SKILL_DIR}/templates/FirebasePackageInstaller.cs` → `Assets/FirebaseUpmInstaller/Editor/FirebasePackageInstaller.cs`
   - `${CLAUDE_SKILL_DIR}/templates/Firebase.Installer.Editor.asmdef` → `Assets/FirebaseUpmInstaller/Editor/Firebase.Installer.Editor.asmdef`
   - 복사 후, 인스톨러 상단 `Modules` 배열을 **탐지된 모듈**에 맞춰 수정. (버전이 최신이 아니어야 하면 `FirebaseVersion` 도 수정.)
3. `Packages/manifest.json` 편집:
   - Firebase/EDM 관련 `scopedRegistries`(maven.google.com, openupm 의 com.google scope, Game Package Registry 등)를 **제거**.
     단, 그 레지스트리가 **Firebase 외 다른 패키지**도 서빙 중이면 지우지 말고 사용자에게 경고.
   - `dependencies` 에 아래를 추가(상대경로):
     ```json
     "com.google.external-dependency-manager": "file:../GooglePackages/com.google.external-dependency-manager-1.2.186.tgz",
     "com.google.firebase.app": "file:../GooglePackages/com.google.firebase.app-13.12.0.tgz",
     "com.google.firebase.<module>": "file:../GooglePackages/com.google.firebase.<module>-13.12.0.tgz"
     ```
     (탐지된 모듈마다 한 줄. 기존 절대경로/레지스트리 기반 Firebase 항목이 있으면 이 형태로 교체.)
4. `.gitignore` 에 타르볼 제외 추가(파일 없으면 생성):
   ```gitignore
   # Firebase UPM 타르볼 — Tools > Firebase 메뉴로 받음 (Git/LFS 제외)
   /GooglePackages/*.tgz
   ```

### 5. 알림 + 핸드오프
편집은 여기까지. 다운로드는 사용자가 Unity 에서 수행한다. 다음을 **그대로 안내**한다:

> 1. `Assets/FirebaseUpmInstaller/Editor/FirebasePackageInstaller.cs` 를 열어
>    상단의 **`FirebaseVersion`** 과 **`Modules`** 가 맞는지 확인/수정하세요. (기본값: 최신 13.12.0)
> 2. Unity 로 돌아가 **`Tools > Firebase > Install Packages`** 를 클릭하세요.
>    (클릭하면 받을 버전·모듈을 보여주는 확인창이 먼저 뜹니다. 버전이 다르면 취소하고 1번을 고치세요.)
> 3. 다운로드가 끝나면 UPM 이 자동 재해석 → Firebase 가 import 되고 재컴파일됩니다.

타르볼이 없는 동안은 Firebase 미해결로 컴파일 에러가 보이는 게 정상이며,
인스톨러는 독립 어셈블리라 그 상태에서도 메뉴가 표시된다는 점을 덧붙인다.

### 6. 검증
- `Packages/manifest.json` 이 유효한 JSON 인지 확인.
- (사용자가 메뉴 실행 후) Package Manager 에 Firebase 모듈 + EDM 이 에러 없이 표시되고,
  **`Multiple precompiled assemblies` / duplicate 에러가 없는지** 확인(= 정리가 완전했다는 신호).
- `git status` 로 `GooglePackages/*.tgz` 가 추적되지 않는지 확인.
