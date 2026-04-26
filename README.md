플레이 영상 : https://youtu.be/wnMKjhCofMY

## 📋 프로젝트 정보

| 항목 | 내용 |
|------|------|
| **장르** | 모바일 비주얼 노벨 (Visual Novel) 싱글 게임 |
| **엔진** | Unity 2019.4.21f1 (LTS) |
| **개발 기간** | 2021.03 ~ 2022.03 (약 1년) |
| **개발 인원** | 2인 (클라이언트 개발), 그 외 5인 |
| **핵심 목표** | XML 기반 스크립트 파싱 시스템 구축, 멀티 엔딩 분기 구현, 모바일 최적화 UI/UX 제공 |

---

## 🔧 핵심 기술 및 구현 내용

#### XML 기반 게임 스크립트 파싱 시스템

`GameManager.cs` (1,390줄)를 중심으로 커스텀 XML 스크립트 파싱 엔진을 직접 구현하였습니다. 게임 내 텍스트 출력, 분기 조건 처리, 이벤트 실행, 태그(Tg) 시스템을 모두 XML 파일로 정의하고, `IdCheck`, `TgCheck`, `RandCheck`, `CondCheck`, `EvCheck` 등의 메서드를 통해 단계별로 파싱 및 처리합니다. 스크립트 파일의 교체만으로 게임 내용 전체를 유연하게 수정할 수 있어 콘텐츠와 로직의 분리를 실현했습니다.

#### 멀티 엔딩 분기 시스템 (태그 & 조건 기반)

`SetTg0` ~ `SetTg5` 메서드를 통해 게임 진행 중 태그 값을 런타임에 변경하고, `CondCheck`로 각 분기 조건을 평가하여 엔딩을 결정합니다. `EndingRoll.cs`는 엔딩 태그(`ending`, `TrueEnding`)를 감지하여 스태프롤 연출 및 일러스트 갤러리 해금을 자동으로 처리합니다. 단순 플래그 방식이 아닌 구조화된 태그 조합으로 복잡한 멀티 엔딩 분기를 구현하였습니다.

#### 세이브 / 로드 시스템 (JSON 직렬화)

`DataManager.cs`와 `GameManager.cs`의 협력 구조로 설계된 세이브 시스템입니다. `ObjectToJson` / `JsonToObject`를 통해 `GameData` 객체를 JSON으로 직렬화·역직렬화하며, 다중 슬롯(`SlotData`, `SlotManager`) 기반 저장을 지원합니다. 빠른 저장(`QuikSave`)과 슬롯 선택 UI를 통한 일반 저장을 분리 제공하고, `InitSaveSlot`으로 슬롯 상태 초기화를 관리합니다.

#### 업적 시스템 (XML 영속화 + Observer 패턴)

`AchievementManager.cs`는 `OnNotify` 메서드를 통해 게임 이벤트를 수신(Observer)하고, `CompareCond`로 조건을 비교하여 업적 달성 여부를 판단합니다. `AchievementXmlManager.cs`는 업적 데이터를 XML 파일로 생성(`CreateAchieveXml`), 파싱(`xmlScriptParsing`), 저장(`xmlScriptSave`), 로드(`LoadAchieveXml`)하여 앱 재실행 후에도 업적 진행 상황이 유지되도록 영속화합니다.

#### 씬 전환 및 페이드 이펙트 (Coroutine 기반)

`LevelChanger.cs`는 `FadeInStart`, `FadeToLevel`, `OnFadeComplete` 등의 메서드로 씬 전환 시 페이드 인/아웃 연출을 Coroutine으로 처리합니다. `LoadSceneManager.cs`는 씬 이름 또는 인덱스를 오버로딩(`LoadScene`)하여 로딩 처리를 통일합니다.

#### 사운드 매니저 (Singleton + BGM FadeOut)

`SoundManager.cs`는 `DontDestroyOnLoad`를 활용한 Singleton 패턴으로 씬 전환 시에도 BGM이 유지됩니다. 다중 AudioSource 풀(`audioSourceEffects[]`)을 통해 효과음 동시 재생을 지원하며, `BGMFadeOut` Coroutine에서 `Mathf.Lerp`를 사용한 자연스러운 볼륨 감소를 구현했습니다. `SettingManager`와 연동하여 BGM/효과음 볼륨 슬라이더 값이 실시간 반영됩니다.

#### 텍스트 타이핑 이펙트 (Coroutine 기반 연출)

`TextEffect.cs`의 `TypingEffectMode0`와 `DialogueManager.cs`의 `TypeSentence`는 각 문자를 한 글자씩 순서대로 출력하는 타이핑 애니메이션을 Coroutine으로 구현합니다. `Queue<string>` 자료구조로 대사 순서를 관리하며, `StopAllCoroutines`를 통해 빠른 진행(스킵) 시 이전 타이핑이 즉시 중단됩니다.

#### 모바일 터치 입력 처리 (멀티터치 & 핀치줌)

`ScreenTouchManager.cs`는 모바일 터치 이벤트를 처리하며 `FindTouchedObject`로 터치된 UI 오브젝트를 탐지합니다. `ZoomPanelManager.cs`는 일러스트 확대/축소 기능(`ZoomPanelOpen/Close`)을 제공하고, `Mode2ContentControl.cs`는 드래그 스크롤과 터치 충돌을 해결한 모드2(이미지 뷰 모드) 콘텐츠 제어를 담당합니다. `TouchParticle.cs`는 터치 위치에 파티클 이펙트를 생성하여 시각적 피드백을 제공합니다.

#### UI 시스템 (다중 선택지 패널 & 이미지 갤러리)

선택지 수(1~5개)에 따라 `OneChoicePanel` ~ `FiveChoicePanel`을 분리하여 관리합니다. `GalleryUI.cs`와 `gallery` 폴더의 스크립트로 클리어 조건에 따라 잠금 해제되는 일러스트 갤러리 UI를 구현하며, `ImgBtnChild.cs`로 개별 이미지 버튼의 상태를 제어합니다. `ScrollViewManager.cs`는 게임 내 텍스트 스크롤 뷰를 관리하고, `MobileTalkManager.cs`는 모바일 메신저 UI 연출을 담당합니다.

#### 2프레임 이미지 애니메이션 (`Anim2Frame`)

`Anim2Frame.cs`의 `ChangeImageEffect`는 두 개 이상의 이미지 프레임을 Coroutine으로 교체하여 저비용 스프라이트 애니메이션을 구현합니다. 비주얼 노벨의 캐릭터 표정 변화나 간단한 오브젝트 연출에 활용됩니다.

---

## ✅ 주요 구현 포인트

- XML 파싱 엔진을 직접 설계·구현하여 ID/Tag/Condition/Event 태그를 단계별 처리하는 커스텀 스크립트 시스템 구축
- `SetTg0~5` 런타임 태그 변수와 `CondCheck` 조건 평가로 복잡한 멀티 엔딩 분기 로직을 데이터 주도 방식으로 구현
- JSON 직렬화 기반 다중 슬롯 세이브/로드 시스템과 빠른 저장(QuikSave) 기능을 분리 설계
- Observer 패턴 적용 업적 시스템에 XML 영속화를 결합하여 앱 재시작 후에도 업적 진행 상황 유지
- `DontDestroyOnLoad` Singleton SoundManager로 씬 전환 간 BGM 연속 재생 및 Lerp 기반 페이드아웃 구현
- `Queue<string>` + Coroutine 조합으로 대화 타이핑 이펙트와 즉시 스킵 기능을 충돌 없이 동시 구현
- 모바일 터치/드래그/핀치 입력 처리와 터치 파티클 피드백으로 모바일 UX 완성도 향상
- 선택지 수(1~5)별 전용 패널 클래스 분리 및 이미지 갤러리 잠금 해제 시스템 설계
- `LevelChanger` Coroutine 페이드 씬 전환 + `LoadSceneManager` 오버로딩으로 씬 전환 로직 일원화
- `Anim2Frame` 경량 2프레임 이미지 스왑 애니메이션으로 추가 애니메이터 없이 캐릭터 연출 구현
