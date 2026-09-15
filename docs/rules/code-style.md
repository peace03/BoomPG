# 코드 컨벤션 (Code Style)

대상: Unity 6000.6.0f1 · C# · URP · Input System.
Codex는 이 문서의 규칙을 따릅니다. 계획서(`TaskPlan/TASK_PLAN.md`)에 다른 지시가 있으면 계획서가 우선합니다.

## 1. 네이밍

| 대상 | 규칙 | 예 |
|---|---|---|
| 클래스 · 구조체 · 열거형 | PascalCase | `RocketLauncher` |
| 인터페이스 | `I` + PascalCase | `IKnockbackReceiver` |
| 메서드 · 프로퍼티 · 이벤트 | PascalCase | `ApplyKnockback` |
| public 필드 | 쓰지 않는다. 프로퍼티를 쓴다 | — |
| private · protected 필드 | `_camelCase` | `_currentHealth` |
| `[SerializeField] private` 필드 | `_camelCase` (인스펙터 표시명은 Unity가 자동 정리) | `_explosionRadius` |
| 지역 변수 · 매개변수 | camelCase | `impactPoint` |
| `const` · `static readonly` | PascalCase | `MaxPlayers` |
| 열거형 멤버 | PascalCase | `MatchState.Warmup` |
| 비동기 메서드 | 접미사 `Async` | `LoadArenaAsync` |
| 파일명 | 파일에 담긴 주 타입명과 동일 | `RocketLauncher.cs` |

- 축약어는 두 글자까지만 대문자 유지: `UIManager`(O), `HTTPClient`(X → `HttpClient`).
- 한국어 기획 용어와 코드 식별자의 대응은 [design/glossary.md](../design/glossary.md)에 기록합니다.
  새 용어가 생기면 그 자리에서 등록합니다.

## 2. 네임스페이스와 폴더

네임스페이스는 `BoomPG.<도메인>` 형태로, 폴더 경로와 1:1 대응시킵니다.

```
Assets/_Project/Scripts/Gameplay/Combat/RocketLauncher.cs
  → namespace BoomPG.Gameplay.Combat
```

### 2.1 폴더 구조 (확정 — [ADR-0005](../decisions/ADR-0005-프로젝트-코드-구조.md))

**내가 만든 파일은 전부 `Assets/_Project/` 아래에만 둡니다.** `Assets/` 바로 아래는
외부 패키지·에셋 스토어 반입물의 자리로 비워 둡니다.

```
Assets/_Project/
  Scripts/
    Core/        공통 유틸, 로깅, 상태 머신, 오브젝트 풀    → BoomPG.Core.asmdef
    Gameplay/    캐릭터 이동, 전투, 넉백, 피격 판정         → BoomPG.Gameplay.asmdef
    Network/     동기화, 권한, 매치 라이프사이클            → BoomPG.Network.asmdef
    UI/          HUD, 메뉴                                → BoomPG.UI.asmdef
    Editor/      에디터 전용                              → BoomPG.Editor.asmdef
    Tests/
      EditMode/  순수 로직 테스트                         → BoomPG.Tests.EditMode.asmdef
  Prefabs/
  Scenes/        ← 템플릿의 Assets/Scenes/ 를 여기로 이동
  Settings/      ← 템플릿의 Assets/Settings/ 를 여기로 이동
  Art/           Models / Materials / VFX
  Audio/
  Data/          ScriptableObject 에셋
```

템플릿 정리:

- `Assets/Scenes/`, `Assets/Settings/` → `Assets/_Project/` 아래로 이동한다.
  **`.meta` 파일을 반드시 함께 옮긴다.** Unity는 경로가 아니라 `.meta`의 GUID로 참조를 추적하므로,
  `.meta`가 따라가면 URP 에셋 참조와 빌드 씬 목록은 끊기지 않는다.
- `Assets/TutorialInfo/` 와 `Assets/Readme.asset` 은 URP 템플릿 샘플이므로 `.meta`와 함께 **삭제**한다.
- `Assets/Settings/Mobile_RPAsset.asset`, `Mobile_Renderer.asset` 은 이동이 아니라 **삭제**한다
  ([ADR-0002](../decisions/ADR-0002-pc-단독-플랫폼.md)).

### 2.2 어셈블리 정의 (확정 — [ADR-0005](../decisions/ADR-0005-프로젝트-코드-구조.md))

도메인별로 `.asmdef`를 나눕니다. **참조는 아래 방향으로만 허용합니다.**

| 어셈블리 | 참조 가능 대상 |
|---|---|
| `BoomPG.Core` | (없음) |
| `BoomPG.Gameplay` | `BoomPG.Core` |
| `BoomPG.Network` | `BoomPG.Core`, `BoomPG.Gameplay`, Fusion 2 |
| `BoomPG.UI` | `BoomPG.Core`, `BoomPG.Gameplay` |
| `BoomPG.Editor` | 전부 (에디터 전용, 빌드에서 제외) |
| `BoomPG.Tests.EditMode` | `BoomPG.Core`, `BoomPG.Gameplay` |

**`BoomPG.Gameplay`는 Fusion 2를 참조하지 않습니다.** 게임 규칙이 네트워크 솔루션을 모르게
유지하기 위한 것이며, 이 표를 어기면 컴파일 에러로 막힙니다
([tech/architecture.md](../tech/architecture.md) 3장 참조).

## 3. MonoBehaviour 작성 순서

한 클래스 안에서 아래 순서를 지킵니다.

1. `const` / `static readonly`
2. `[SerializeField]` 필드
3. private 필드
4. 프로퍼티
5. 이벤트
6. Unity 콜백 — `Awake` → `OnEnable` → `Start` → `Update` → `FixedUpdate` → `LateUpdate` → `OnDisable` → `OnDestroy`
7. public 메서드
8. private 메서드

## 4. 성능 규칙 (PC 단독 타깃이어도 지킨다)

- `Update`/`FixedUpdate` 안에서 `GetComponent`, `Find`, `FindObjectOfType`, `Camera.main` 호출 금지.
  `Awake`에서 캐싱합니다.
- 매 프레임 실행되는 경로에서 문자열 연결·LINQ·박싱을 만들지 않습니다.
- 물리 연산은 `FixedUpdate`에서, 입력 수집은 `Update`에서 합니다.
- 자주 생성·파괴되는 오브젝트(로켓, 폭발 VFX)는 오브젝트 풀을 씁니다.
- 코루틴보다 `UniTask`/`Awaitable`을 선호하되, 도입 전까지는 코루틴을 씁니다.

## 5. 에러 처리

- 게임플레이 런타임 경로에서는 예외를 던지지 않습니다. 검증 실패는 **로그 + 조기 반환**입니다.
- 인스펙터 참조 누락 등 "개발자 실수"는 `Awake`에서 한 번 검사하고 에러 로그를 남긴 뒤
  해당 컴포넌트를 비활성화합니다. 매 프레임 에러를 뿜지 않습니다.
- `try/catch`는 외부 I/O(파일, 네트워크 전송 계층)에만 씁니다. 빈 catch 블록 금지.

## 6. 로깅 (확정 — [ADR-0005](../decisions/ADR-0005-프로젝트-코드-구조.md))

**`Debug.Log`를 직접 호출하지 않습니다.** `BoomPG.Core.Logging.GameLog` 정적 클래스를 거칩니다.

- 카테고리별 진입점: `GameLog.Core`, `GameLog.Combat`, `GameLog.Net`, `GameLog.UI`
- 각 카테고리 메서드에 `[Conditional("UNITY_EDITOR")]` 와 `[Conditional("DEVELOPMENT_BUILD")]` 를
  붙여 **릴리즈 빌드에서 호출 자체가 사라지게** 합니다. 인자 문자열 조립 비용까지 제거됩니다.
- 경고·에러(`GameLog.Warn`, `GameLog.Error`)는 `Conditional`을 **붙이지 않습니다.**
  릴리즈에서도 남아야 합니다.
- 출력 형식: `[역할][카테고리] 메시지` — 예: `[H][Combat] 넉백 18.0 m/s 적용`
- 역할 접두(`H` 호스트 / `C` 클라이언트)는 네트워크 세션 시작 시 `GameLog.SetRole()` 로 한 번
  설정합니다. 호스트 권위 구조에서 **어느 쪽 로그인지 구분되지 않으면 넉백 디버깅이 불가능합니다**
  ([tech/networking.md](../tech/networking.md) 3장).

정확한 시그니처는 첫 구현 계획서에서 확정합니다.

## 7. 주석

- 주석은 한국어로 씁니다.
- **무엇을 하는지**가 아니라 **왜 그렇게 했는지**를 적습니다. 코드가 이미 말하는 것은 반복하지 않습니다.
- public API에는 `///` XML 주석을 답니다. 매개변수 단위를 반드시 명시합니다. (`초`, `m/s`)

## 8. 파일 형식

- C# 파일: UTF-8 (BOM 없음), 들여쓰기 공백 4칸.
- PowerShell 스크립트(`scripts/*.ps1`): UTF-8 **BOM 포함** + CRLF.
  BOM이 없으면 Windows PowerShell 5.1이 CP949로 읽어 한글 주석에서 파싱 에러가 납니다.
- 마크다운: UTF-8 (BOM 없음), LF.
- `.meta` 파일은 직접 만들지 않습니다. Unity가 생성합니다.

## 변경 이력

| 날짜 | 내용 |
|---|---|
| 2026-09-14 | 최초 작성. 미결정 D-001 ~ D-003 등록 |
| 2026-09-14 | ADR-0005 채택. D-001(폴더 구조) · D-002(asmdef 분리) · D-003(로깅 래퍼) 해소 |
