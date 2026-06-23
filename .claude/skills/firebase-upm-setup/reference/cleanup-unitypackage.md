# `.unitypackage` Firebase 설치물 정리 체크리스트

Firebase 를 `.unitypackage` 로 Assets 에 임포트하면 아래 항목들이 `Assets/` 안에 생성된다.
UPM(타르볼) 방식과 **동시에 존재하면 어셈블리/네이티브 라이브러리가 중복**되어
`Multiple precompiled assemblies with the same name` 류의 컴파일 에러가 난다.
따라서 UPM 으로 전환할 때 아래를 제거해야 한다.

> 폴더/파일을 지울 때는 짝이 되는 `.meta` 파일도 함께 지운다.
> (예: `Assets/Firebase` 와 `Assets/Firebase.meta`)

## 🔴 제거 대상

| 경로 | 설명 |
|---|---|
| `Assets/Firebase/` | Firebase SDK 본체 (Plugins/Editor/Sample dll·네이티브) |
| `Assets/ExternalDependencyManager/` | EDM4U (UPM 쪽 EDM 타르볼과 중복) |
| `Assets/PlayServicesResolver/` | 구버전 EDM4U 폴더명 (있으면 제거) |
| `Assets/Plugins/Android/` 내 Firebase 관련 | `*.aar`, `GeneratedLocalRepo/`, `mainTemplate.gradle` 의 Firebase 항목 등 — Firebase 가 넣은 것만 |
| `Assets/Plugins/iOS/Firebase/` | iOS 네이티브 (있으면) |
| `Assets/GeneratedLocalRepo/` | EDM 이 해석해 만든 Android 로컬 저장소 |
| Firebase 가 만든 `link.xml` | 보통 `Assets/Firebase/link.xml` 등. 사용자가 직접 만든 link.xml 과 혼동 주의 |

> 위 폴더들은 환경마다 일부만 존재할 수 있다. **실제로 존재하는 것만** 제거.
> `Assets/Plugins/Android/` 와 `Assets/Plugins/iOS/` 는 Firebase 외 다른 플러그인도
> 들어있을 수 있으니 **Firebase/Google 관련 산출물만** 골라 지운다(통째로 지우지 말 것).

## 🟢 반드시 보존

| 경로 | 이유 |
|---|---|
| `Assets/google-services.json` (Android) | Firebase 프로젝트 **설정 입력값** — SDK 가 아님 |
| `GoogleService-Info.plist` (iOS) | 동상 |
| `Assets/StreamingAssets/google-services-desktop.json` | 데스크톱용 설정 |
| 사용자 작성 코드 (`FirebaseManager` 등) | 우리 코드 — 손대지 않음 |

## 정리 후 확인

- UPM 전환 후 EDM 은 **UPM 쪽 하나만** 존재해야 한다. `Assets/ExternalDependencyManager`
  가 남아 있으면 UPM EDM 타르볼과 충돌한다.
- Unity 콘솔에 `duplicate` / `Multiple precompiled assemblies` 에러가 없으면 정리 완료.
- 남은 `.meta` 고아 파일(대응 폴더 없는 meta)은 Unity 가 자동 정리하거나 경고만 낸다.
