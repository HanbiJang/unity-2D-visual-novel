# Dream Catcher - 모바일 비주얼 노벨

**XML 기반 스크립트 파싱 엔진을 직접 구현하고 멀티 엔딩 분기 시스템을 설계한 모바일 비주얼 노벨 게임 - Google Play 출시작**

[![Unity](https://img.shields.io/badge/Unity-000000?style=flat&logo=unity&logoColor=white)](https://unity.com/)
[![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=csharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)

[![게임플레이 영상](https://img.youtube.com/vi/wnMKjhCofMY/0.jpg)](https://youtu.be/wnMKjhCofMY?si=e8oNtJKVfQcT3Hr3)

> 이미지를 클릭하면 게임플레이 영상을 볼 수 있습니다.
[![YouTube](https://img.shields.io/badge/YouTube-FF0000?style=flat&logo=youtube&logoColor=white)](https://youtu.be/wnMKjhCofMY?si=e8oNtJKVfQcT3Hr3)

---

## 프로젝트 정보

| 항목 | 내용 |
|---|---|
| 장르 | 모바일 비주얼 노벨 (Visual Novel) |
| 개발 기간 | 2021.03 ~ 2022.03 (약 1년) |
| 팀 구성 | 총 7인 (클라이언트 개발 2인, 기획/아트 5인) |
| 사용 기술 | Unity 2019.4.21f1 (LTS), C#, XML, JSON |
| 출시 | Google Play 출시 |

**본인 담당 파트:** XML 파싱 시스템, 주요 게임 로직, 업적 시스템, 갤러리 시스템, 세이브/로드 시스템

---

## 프로젝트 개요

게임 개발 동아리 활동 중 팀 프로젝트로 제작하여 Google Play에 출시한 모바일 비주얼 노벨 게임입니다.

서드파티 대화 시스템 없이 XML 스크립트 파싱 엔진을 직접 설계하여, 스크립트 파일 교체만으로 게임 전체 내용을 수정할 수 있는 데이터 주도 구조를 구현했습니다. 태그 기반 조건 분기로 멀티 엔딩을 처리하고, XML 영속화 업적 시스템과 JSON 다중 슬롯 세이브/로드를 직접 구축했습니다.

---

## 코드 상세

코드 파일은 레포에 포함되어 있지 않아 파일 링크 대신 클래스별 역할로 정리합니다.

### 게임 로직 핵심 (`GameControl/`)

| 클래스 | 역할 |
|---|---|
| `GameManager` | 게임 전체 진행 총괄. XML 스크립트 파싱 엔진 핵심. `IdCheck`, `TgCheck`, `RandCheck`, `CondCheck`, `EvCheck` 메서드로 ID/태그/조건/이벤트를 단계별 처리. `SetTg0~5`로 런타임 태그 변수 관리 |
| `StatusManager` | 플레이어 상태 수치 관리. 멘탈 게이지 등 스탯 추적 |
| `GameButtonControl` | 게임 패널 on/off 및 UI 버튼 입력 처리 |
| `ScrollViewManager` | 텍스트 스크롤 뷰 관리. XML 스크립트 파싱(`xmlScriptParsing`) 및 콘텐츠 동적 생성 |
| `IllustManager` | 인게임 일러스트 표시 및 전환 처리 |
| `LevelChanger` | 씬 전환 시 페이드 인/아웃 연출. `FadeInStart`, `FadeToLevel`, `OnFadeComplete`를 Coroutine으로 처리 |
| `SettingManager` | BGM/효과음 볼륨, 텍스트 속도 등 설정 관리. `SoundManager`와 실시간 연동 |
| `MentalGaugeGra` | 멘탈 게이지 UI 그래픽 처리 |
| `OneChoicePanel` ~ `FiveChoicePanel` | 선택지 수(1~5개)별 전용 패널 클래스. 각 선택지에 따른 분기 처리 |
| `TouchSystem` | 인게임 터치 입력 처리 |

### 업적 시스템 (`Achievement/`)

| 클래스 | 역할 |
|---|---|
| `AchievementManager` | Observer 패턴 기반 업적 시스템. `OnNotify`로 게임 이벤트 수신, `CompareCond`로 달성 조건 판별 |
| `AchievementXmlManager` | 업적 데이터 XML 영속화. `CreateAchieveXml`(최초 생성), `xmlScriptParsing`(파싱), `xmlScriptSave`(달성 시 저장), `LoadAchieveXml`(로드) |
| `AchievementData` | 업적 데이터 구조체 |
| `AchievementSetting` | 업적 UI 설정 및 표시 처리 |

### 데이터 / 세이브 (`Data/`)

| 클래스 | 역할 |
|---|---|
| `DataManager` | JSON 직렬화 기반 세이브/로드. `ObjectToJson` / `JsonToObject`로 `GameData` 객체 직렬화. 다중 슬롯(`saveID`) 저장 지원 |
| `GameData` | 세이브 데이터 구조체. `scriptPath`, `mode`, `isChoice` 등 게임 진행 상태 포함 |
| `SlotManager` | 세이브 슬롯 목록 관리 및 상태 초기화(`InitSaveSlot`) |
| `SlotData` | 슬롯별 저장 데이터 |
| `SaveID` | 슬롯 ID 정의 |
| `ClearOrUsingSaveData` | 세이브 슬롯 사용 여부 및 클리어 상태 관리 |

### 갤러리 (`gallery/`)

| 클래스 | 역할 |
|---|---|
| `GalleryManager` | 일러스트 갤러리 관리. 클리어 조건에 따른 잠금 해제 처리. `ShowGalleryIllusts`로 해금 여부에 따라 이미지 동적 표시 |
| `GalleryData` | 갤러리 일러스트 데이터 및 해금 상태 JSON 영속화 |
| `GalleryUI` | 갤러리 패널 UI. 이미지 확대(줌) 기능(`ZoomPanelOpen`) 포함 |

### 씬/UI 시스템

| 클래스 | 역할 |
|---|---|
| `EndingRoll` | 엔딩 태그(`ending`, `TrueEnding`) 감지 시 스태프롤 연출 및 갤러리 자동 해금 |
| `SoundManager` | `DontDestroyOnLoad` Singleton BGM/SFX 관리. 다중 AudioSource 풀로 효과음 동시 재생. `BGMFadeOut` Coroutine(`Mathf.Lerp`) 기반 자연스러운 볼륨 감소 |
| `LoadSceneManager` | 씬 이름/인덱스 오버로딩으로 씬 전환 로직 일원화 |
| `LobbySceneManager` | 로비 씬 진입 처리 |
| `TextEffect` | 타이핑 애니메이션. 문자 한 글자씩 순차 출력, `StopAllCoroutines`로 즉시 스킵 지원 |
| `ScreenTouchManager` | 모바일 터치 이벤트 처리. `FindTouchedObject`로 터치된 UI 오브젝트 탐지 |
| `ZoomPanelManager` | 일러스트 핀치 줌 기능(`ZoomPanelOpen/Close`) |
| `Mode2ContentControl` | 드래그 스크롤과 터치 충돌을 해결한 이미지 뷰 모드 제어 |
| `TouchParticle` | 터치 위치 파티클 이펙트 생성 |
| `MobileTalkManager` | 모바일 메신저 UI 연출 |
| `Anim2Frame` | 2프레임 이미지 교체 경량 애니메이션. 추가 Animator 없이 캐릭터 표정 변화 구현 |
| `DialogueManager` | 대화 흐름 관리. `Queue<string>`으로 대사 순서 관리, `TypeSentence` Coroutine으로 타이핑 출력 |

---

## 핵심 구현

### XML 기반 스크립트 파싱 시스템

서드파티 대화 시스템 없이 `GameManager`에 커스텀 XML 파싱 엔진을 직접 구현했습니다. ID, 태그(Tg), 조건(Cond), 이벤트(Ev), 랜덤(Rand) 태그를 단계별 메서드로 파싱하여 처리합니다. 스크립트 파일 교체만으로 게임 전체 내용을 수정할 수 있어 콘텐츠와 로직이 분리된 구조입니다.

### 멀티 엔딩 분기 시스템

`SetTg0~5`로 런타임 태그 변수를 관리하고, `CondCheck`로 분기 조건을 평가해 엔딩을 결정합니다. `EndingRoll`이 엔딩 태그를 감지하면 스태프롤 연출과 갤러리 해금을 자동 처리합니다. 단순 플래그 방식이 아닌 구조화된 태그 조합으로 복잡한 멀티 엔딩 분기를 데이터 주도 방식으로 구현했습니다.

### 업적 시스템 - Observer 패턴 + XML 영속화

`AchievementManager`가 `OnNotify`로 게임 이벤트를 수신하고 달성 조건을 판별합니다. `AchievementXmlManager`가 업적 진행 상황을 XML로 영속화하여 앱 재실행 후에도 상태를 유지합니다.

### 세이브/로드 - JSON 다중 슬롯

`DataManager`가 `GameData` 객체를 JSON으로 직렬화/역직렬화하며, 빠른 저장(`QuikSave`)과 슬롯 선택 UI를 통한 일반 저장을 분리 제공합니다.

### 사운드 매니저

`DontDestroyOnLoad` Singleton으로 씬 전환 시에도 BGM을 유지합니다. 다중 AudioSource 풀로 효과음 동시 재생을 지원하며, `Mathf.Lerp` 기반 `BGMFadeOut` Coroutine으로 자연스러운 볼륨 감소를 구현했습니다.

---

## 폴더 구조

> 팀원 저작권 문제로 코드 파일은 레포에 포함되어 있지 않습니다.

- `Assets/GameControl/` : 게임 로직 핵심 (GameManager, 선택지 패널, 씬 전환 등)
- `Assets/Achievement/` : Observer 패턴 업적 시스템, XML 영속화
- `Assets/gallery/` : 일러스트 갤러리, 잠금 해제 시스템
- `Assets/TextEffect/` : 타이핑 애니메이션
- `Assets/DialogueScripts/` : 대화 흐름 관리
- `Assets/Anim2Frame/` : 경량 2프레임 스프라이트 애니메이션
- `Assets/EndingRoll.cs` : 엔딩 연출 및 갤러리 자동 해금
- `Assets/GalleryUI.cs` : 갤러리 패널 UI, 핀치 줌
- `Assets/LoadSceneManager.cs` : 씬 전환 로직 일원화
- `Assets/LobbySceneManager.cs` : 로비 씬 진입 처리
- `Assets/LogoSceneManager.cs` : 로고 씬 처리
- `Assets/Mode2ContentControl.cs` : 드래그/터치 충돌 해결 이미지 뷰 모드 제어
- `Assets/ScreenTouchManager.cs` : 모바일 터치 이벤트 처리
- `Assets/SoundManager.cs` : Singleton BGM/SFX 관리
- `Assets/TouchParticle.cs` : 터치 위치 파티클 이펙트
- `Assets/ZoomPanelManager.cs` : 일러스트 핀치 줌
