# TASK_PLAN — M1 그레이박스 프로토타입 (넉백 코어 검증)

> 작성: Claude Code (Architect) · 구현: Codex (Executor)
> 이 파일은 Codex의 유일한 지시서입니다. 모호한 문장이 남아 있으면 계획이 미완성입니다.

## 0. 시작 전 반드시 읽을 것

1. `docs/rules/code-style.md` — 네이밍, 폴더 구조, asmdef 참조 표, 로깅 규칙
2. `docs/design/glossary.md` — 한국어 용어 ↔ 코드 식별자 대응
3. `docs/design/gdd.md` 5~7장(수치), 17장(구현 설계), 23장(아키텍트 검토)

### Unity Editor 상태 — **확인하지 말고 바로 진행하십시오**

- 에디터를 닫아야 했던 이유는 **Step 1의 파일 이동** 때문이었고, 그 작업은 이미 끝났습니다.
- **Step 2~15는 파일을 새로 만드는 작업이라 에디터 실행 여부와 무관합니다.**
  에디터가 켜져 있는지 확인하려 하지 말고, 사용자에게 묻지도 말고 바로 구현하십시오.
- **`mcp__unity__*` 툴이 보이면 에디터가 열려 있는 상태입니다.** 그것이 정상이며,
  6장의 테스트를 그 툴로 실행하십시오. 배치 모드 명령을 쓰지 마십시오.
- `mcp__unity__*` 툴이 **없을 때만** 6장의 배치 모드 명령을 쓰며, 그때는 에디터가 닫혀 있어야
  합니다 (프로젝트 락 충돌).

> **배치 모드는 이 환경에서 동작하지 않는 것으로 확인되었습니다** (2026-09-15).
> `-batchmode -runTests` 실행 시 `No valid Unity Editor license found` (exit 198) 로 실패합니다.
> 샌드박스와 무관한 라이선스 문제이므로 **MCP 경로를 우선 사용하십시오.**

### 멈춰야 할 때와 계속해야 할 때

지금까지 사소한 조건 확인으로 구현이 여러 번 중단되었습니다. 기준을 명확히 합니다.

**멈추고 보고해야 하는 경우 — 이때만 멈추십시오**

- 계획서의 지시가 서로 모순될 때
- 계획서에 명시되지 않은 파일을 만들거나 고쳐야만 진행이 되는 때
- asmdef 참조 표를 늘려야 할 것 같을 때 (레이어 원칙 위반 신호)

**멈추지 말고 계속해야 하는 경우**

- 환경 상태 확인(에디터 실행 여부, 프로세스 목록 등) — 위에 답을 적어두었습니다
- 도구나 샌드박스가 특정 명령을 거부할 때 → **그 항목만 건너뛰고 나머지 스텝을 계속 진행한 뒤**,
  무엇이 거부되었는지 최종 보고에 적으십시오. 전체를 중단하지 마십시오
- 변수명·내부 헬퍼 분리처럼 계획서가 시그니처를 특정하지 않은 세부 — `docs/rules/code-style.md`
  범위 안에서 판단해 구현하십시오

## 0.1 진행 상황 — **이번에 할 일은 아래 3가지뿐입니다** (2026-09-15 갱신)

**Step 1 ~ Step 15 의 파일 생성은 모두 끝났고, Unity 에디터에서 컴파일도 통과했습니다.**
스크립트를 처음부터 다시 만들지 마십시오.

아키텍트가 검증 완료한 것:

- `Assets/_Project/` 이동 완료, `.meta` 짝 일치, 고아 `.meta` 없음 (GUID 참조 무사)
- `Mobile_*` URP 에셋 보존, `ProjectSettings/QualitySettings.asset` 무변경
- asmdef 6개 생성, 참조 목록이 3장 표와 일치, **`Gameplay` → `Network` 참조 없음**
- 레이어 8~13 등록 완료
- `PlayerMotor.ApplyKnockback` 이 `KnockbackCalculator.Blend` 를 경유 (D-022 대응 성립)
- `GameLog.cs` 외 `Debug.Log` 직접 호출 없음, public 필드 없음, `Rigidbody` 미사용
- EditMode 테스트 12케이스 작성 완료 (`[Test]` 8 + `[TestCase]` 4)
- `.inputactions` 에 `Reload` 액션 추가 완료

### 추가로 완료된 것 (MCP 라운드)

- `GameLog.cs` 의 `Conditional` 심볼을 `DEBUG` 로 교체 — `UAC0009` 경고 해소
- 셋업 메뉴 실행 완료 — `SO_*.asset` 3개, `P_Rocket.prefab`, `M1_Greybox.unity` 생성
- **EditMode 테스트 12개 전부 통과** (`KeepStronger` 포함 — D-022 대응 검증 완료)
- `Assets/TutorialInfo/` 와 `Assets/Readme.asset` 삭제 완료 (사람이 처리)

### 이번에 할 일 — **코드 수정 1건뿐입니다**

**`RocketProjectile.cs` 의 레이어 마스크 초기화 위치를 옮기십시오.**

현재 10~11행이 `static readonly` 필드 이니셜라이저에서 `LayerMask.GetMask` 와 `NameToLayer`
를 호출해 런타임 예외가 발생합니다.

```text
UnityException: NameToLayer is not allowed to be called from a MonoBehaviour constructor
(or instance field initializer), call it in Awake or Start instead.
Called from MonoBehaviour 'RocketProjectile' on game object 'P_Rocket'.
```

계획서가 "`static readonly` 로 캐싱하라"고 지시했던 것이 원인이므로 계획서를 고쳤습니다.
**Step 12 의 3번 항목에 적힌 `Awake` 초기화 방식으로 바꾸십시오.**

수정 후 MCP 로 컴파일을 확인하고, EditMode 테스트를 다시 돌려 12개가 여전히 통과하는지
확인한 뒤 보고하십시오.

### 손대지 말 것

- 위 1건 외의 어떤 스크립트도 수정하지 마십시오
- `ProjectSettings/EditorBuildSettings.asset` — 씬 이동에 따라 Unity 가 자동 갱신한 것입니다.
  정상적인 변경이므로 **되돌리지 마십시오**

## 1. 목표 (Goal)

BoomPG의 M1 그레이박스 프로토타입을 만든다. 목적은 단 하나 — **"로켓 폭발로 상대를 밀어
떨어뜨리는 것이 재미있는가"** 를 직접 플레이해서 판정하는 것이다. 이 질문의 답이 "아니오"면
프로젝트 방향을 바꿔야 하므로, 아트·UI·네트워크·클래스 스킬을 전부 배제하고 회색 큐브 위에서
이동·발사·폭발·넉백·제트팩·낙사만 돌아가게 만든다. 동시에 [ADR-0005](../docs/decisions/ADR-0005-프로젝트-코드-구조.md)가
정한 프로젝트 골격(`_Project` 폴더, 도메인별 asmdef, `GameLog`, ScriptableObject 설정,
EditMode 테스트)을 이번에 함께 세워 이후 모든 작업이 같은 구조 위에 쌓이게 한다.

**네트워크는 이번 범위가 아니다.** 전부 로컬 싱글플레이로 동작한다.

## 2. 현재 구조 분석 (Current State)

- **게임 소스 코드가 존재하지 않는다.** `Assets/` 는 Unity URP 템플릿 그대로다.
  - `Assets/Scenes/SampleScene.unity` — 템플릿 기본 씬
  - `Assets/Settings/` — URP 에셋. `PC_*` 와 `Mobile_*` 가 공존
  - `Assets/TutorialInfo/`, `Assets/Readme.asset` — URP 템플릿 샘플
  - `Assets/InputSystem_Actions.inputactions` — 템플릿 기본 액션 맵
- 설치된 패키지 (`Packages/manifest.json`): Input System 1.20.0, Test Framework 1.8.0,
  URP 17.6.0 이 **이미 있다.** Cinemachine 과 Photon Fusion 은 없다. **이번에 추가하지 않는다.**
- `ProjectSettings/TagManager.asset` 의 레이어는 Unity 기본값만 있다 (8번 이후 전부 비어 있음).

### 지켜야 할 컨벤션 (`docs/rules/code-style.md`)

- 네이밍: 클래스·메서드·프로퍼티 PascalCase / private 필드 `_camelCase` / 지역·매개변수 camelCase
- public 필드를 쓰지 않는다. 프로퍼티를 쓴다
- 네임스페이스는 `BoomPG.<도메인>` 으로 폴더와 1:1 대응
- `Update`/`FixedUpdate` 안에서 `GetComponent`, `Find`, `Camera.main` 호출 금지 — `Awake`에서 캐싱
- 예외를 던지지 않는다. 검증 실패는 로그 + 조기 반환
- 인스펙터 참조 누락은 `Awake`에서 한 번 검사 → 에러 로그 → 컴포넌트 비활성화
- **`Debug.Log` 직접 호출 금지.** `GameLog` 를 쓴다 (Step 3에서 만든다)
- 주석은 한국어. **무엇이 아니라 왜**를 적는다. public API에는 `///` 와 단위 명시
- C# 파일 UTF-8(BOM 없음), 들여쓰기 공백 4칸

## 3. 영향 범위 (Files & Architecture)

### 이동 · 삭제

| 경로 | 처리 |
|---|---|
| `Assets/Scenes/` → `Assets/_Project/Scenes/` | 이동 (`.meta` 동반) |
| `Assets/Settings/` → `Assets/_Project/Settings/` | 이동 (`.meta` 동반) |
| `Assets/InputSystem_Actions.inputactions` → `Assets/_Project/Settings/` | 이동 (`.meta` 동반) |
| `Assets/TutorialInfo/` | 삭제 (`.meta` 포함) |
| `Assets/Readme.asset` | 삭제 (`.meta` 포함) |
| `Mobile_RPAsset.asset` · `Mobile_Renderer.asset` | **이동만 한다. 삭제하지 않는다** (아래 참조) |

> **Mobile URP 에셋을 삭제하지 않는 이유**
> `ProjectSettings/QualitySettings.asset` 의 `Mobile` 품질 레벨(배열 인덱스 0)이
> `Mobile_RPAsset.asset` 을 GUID `5e6cbd92db86f4b18aec3ed561671858` 로 참조하고 있다.
> 삭제하면 끊어진 참조가 남는데, 그것을 고치려면 품질 레벨 배열에서 항목을 빼야 하고
> 그러면 `m_PerPlatformDefaultQuality` 의 인덱스 매핑까지 전부 다시 맞춰야 한다.
> M1 의 목적(넉백 검증)과 무관한 위험이므로 이번 범위에서 제외한다.
> **이동은 안전하다** — Unity 는 경로가 아니라 GUID 로 추적하므로 `.meta` 를 동반해 옮기면
> 참조가 유지된다. 현재 활성 품질은 `m_CurrentQuality: 1` (PC) 이라 모바일 레벨이 남아 있어도
> PC 빌드에 영향이 없다.

### 신규 — 어셈블리 정의

| 파일 | 참조 |
|---|---|
| `Assets/_Project/Scripts/Core/BoomPG.Core.asmdef` | (없음) |
| `Assets/_Project/Scripts/Gameplay/BoomPG.Gameplay.asmdef` | `BoomPG.Core`, Unity.InputSystem |
| `Assets/_Project/Scripts/Network/BoomPG.Network.asmdef` | `BoomPG.Core`, `BoomPG.Gameplay` |
| `Assets/_Project/Scripts/UI/BoomPG.UI.asmdef` | `BoomPG.Core`, `BoomPG.Gameplay` |
| `Assets/_Project/Scripts/Editor/BoomPG.Editor.asmdef` | 위 전부. `includePlatforms: ["Editor"]` |
| `Assets/_Project/Scripts/Tests/EditMode/BoomPG.Tests.EditMode.asmdef` | `BoomPG.Core`, `BoomPG.Gameplay`, `UnityEngine.TestRunner`, `UnityEditor.TestRunner` |

### 신규 — 스크립트

| 파일 | 역할 |
|---|---|
| `Scripts/Core/Logging/GameLog.cs` | 로깅 래퍼 |
| `Scripts/Gameplay/Config/MovementConfig.cs` | 이동 수치 SO |
| `Scripts/Gameplay/Config/CombatConfig.cs` | 전투·넉백 수치 SO |
| `Scripts/Gameplay/Config/JetpackConfig.cs` | 제트팩 수치 SO |
| `Scripts/Gameplay/Combat/KnockbackCalculator.cs` | **넉백 순수 계산 (네트워크·엔진 비의존)** |
| `Scripts/Gameplay/Combat/ExplosionResolver.cs` | 폭발 판정 및 적용 |
| `Scripts/Gameplay/Combat/RocketProjectile.cs` | 로켓 비행·충돌 |
| `Scripts/Gameplay/Combat/RpgLauncher.cs` | 발사·재장전 |
| `Scripts/Gameplay/Player/PlayerMotor.cs` | 이동·점프·외부 속도 통합 |
| `Scripts/Gameplay/Player/JetpackController.cs` | 연료 소모·회복·잠금 |
| `Scripts/Gameplay/Player/PlayerInputRelay.cs` | Input System → 각 컴포넌트 |
| `Scripts/Gameplay/Player/HealthComponent.cs` | HP·사망 |
| `Scripts/Gameplay/World/KillZoneTrigger.cs` | 낙사 판정 |
| `Scripts/Gameplay/Camera/ThirdPersonCamera.cs` | 3인칭 카메라 |
| `Scripts/Tests/EditMode/KnockbackCalculatorTests.cs` | 넉백 계산 테스트 |
| `Scripts/Editor/M1SetupMenu.cs` | 설정 에셋·로켓 프리팹·그레이박스 씬 생성 메뉴 |

### 신규 — 에셋 (Step 15의 에디터 메뉴가 코드로 생성한다)

| 경로 | 내용 |
|---|---|
| `Assets/_Project/Data/SO_MovementConfig.asset` | 이동 수치 |
| `Assets/_Project/Data/SO_CombatConfig.asset` | 전투·넉백 수치 |
| `Assets/_Project/Data/SO_JetpackConfig.asset` | 제트팩 수치 |
| `Assets/_Project/Prefabs/P_Rocket.prefab` | 로켓 프리팹 |
| `Assets/_Project/Scenes/M1_Greybox.unity` | 그레이박스 씬 |

### 수정

| 파일 | 변경 |
|---|---|
| `ProjectSettings/TagManager.asset` | 레이어 6종 등록 |

### 의존 방향

```
Core (GameLog)
  ▲
Gameplay (Config → KnockbackCalculator → ExplosionResolver → 컴포넌트들)
  ▲
Editor / Tests
```

**`BoomPG.Gameplay` 는 `BoomPG.Network` 를 참조하지 않는다.** 역방향만 허용된다.

## 4. 구현 스텝 (Step-by-Step)

### Step 1 — 폴더 구조 정리 — **완료됨. 건너뛰십시오** (0.1 참조)

- 대상: `Assets/` 전체
- 변경:
  1. `Assets/_Project/` 를 만들고 그 아래에 `Scripts/Core`, `Scripts/Gameplay`, `Scripts/Network`,
     `Scripts/UI`, `Scripts/Editor`, `Scripts/Tests/EditMode`, `Prefabs`, `Art`, `Audio`, `Data` 를 만든다.
  2. `Assets/Scenes/` 를 `Assets/_Project/Scenes/` 로 옮긴다. **`Assets/Scenes.meta` 도 함께 옮긴다.**
  3. `Assets/Settings/` 를 `Assets/_Project/Settings/` 로 옮긴다. **`Assets/Settings.meta` 도 함께.**
  4. `Assets/InputSystem_Actions.inputactions` 와 그 `.meta` 를 `Assets/_Project/Settings/` 로 옮긴다.
  5. ~~`Assets/TutorialInfo/`, `Assets/Readme.asset` 삭제~~ — **범위 밖으로 옮겼다.**
     샌드박스 정책이 삭제를 거부한다. 사람이 Unity 에디터에서 처리한다.
     (`Readme.asset` 의 GUID `8105016687592461f977c054a80ce2f2` 를 참조하는 곳이 없음을 확인했으므로
     삭제 자체는 안전하다)
  6. **`Mobile_RPAsset.asset` 과 `Mobile_Renderer.asset` 은 삭제하지 않는다.**
     `Settings/` 이동에 딸려 `Assets/_Project/Settings/` 로 옮겨지기만 하면 된다.
     이유는 3장의 인용 블록을 참조하라.
- 주의: `.meta` 파일을 반드시 동반해 옮긴다. Unity는 경로가 아니라 `.meta` 안의 GUID로 참조를
  추적하므로, `.meta`가 따라가면 URP 에셋 참조와 빌드 씬 목록이 끊기지 않는다.
- 완료 기준: `Assets/` 바로 아래에 `_Project` 폴더와 그 `.meta` 만 남는다.
  `Assets/TutorialInfo`, `Assets/Readme.asset` 이 존재하지 않는다.
  `Assets/_Project/Settings/` 안에 `PC_*` 와 `Mobile_*` 에셋이 **모두** 있다.
  `ProjectSettings/QualitySettings.asset` 은 변경되지 않는다.

### Step 2 — 레이어 등록

- 대상: `ProjectSettings/TagManager.asset`
- 변경: `layers:` 배열의 8번 인덱스부터 아래 순서대로 문자열을 채운다. 기존 0~7번은 건드리지 않는다.

| 인덱스 | 이름 |
|---|---|
| 8 | `Player` |
| 9 | `Platform` |
| 10 | `Rocket` |
| 11 | `Pickup` |
| 12 | `KillZone` |
| 13 | `Grapple` |

- 완료 기준: `TagManager.asset` 의 8~13번 인덱스가 위 이름으로 채워져 있다.

### Step 3 — asmdef 6개 생성

- 대상: 3장 "신규 — 어셈블리 정의" 표의 6개 파일
- 변경: 각 폴더에 asmdef를 만든다. `name` 은 파일명과 동일하게 한다.
  - `BoomPG.Editor.asmdef` 는 `"includePlatforms": ["Editor"]` 를 넣는다.
  - `BoomPG.Tests.EditMode.asmdef` 는 아래 형태여야 한다. `precompiledReferences` 의
    `nunit.framework.dll` 과 `overrideReferences: true` 가 **둘 다 있어야** NUnit 을 찾는다.
    하나라도 빠지면 `Assert` 에서 컴파일 에러가 난다.

    ```json
    {
        "name": "BoomPG.Tests.EditMode",
        "references": ["BoomPG.Core", "BoomPG.Gameplay", "UnityEngine.TestRunner", "UnityEditor.TestRunner"],
        "includePlatforms": ["Editor"],
        "overrideReferences": true,
        "precompiledReferences": ["nunit.framework.dll"],
        "defineConstraints": ["UNITY_INCLUDE_TESTS"]
    }
    ```
  - `BoomPG.Gameplay.asmdef` 의 참조에 `Unity.InputSystem` 을 넣는다.
- **참조 표를 임의로 늘리지 말 것.** 구현 중 "참조가 안 된다"는 이유로 `BoomPG.Gameplay` 에
  다른 어셈블리를 추가해야 할 것 같으면, 그것은 레이어 원칙을 어기려는 신호다.
  추가하지 말고 **멈추고 보고**하라.
- 완료 기준: 6개 asmdef가 존재하고 참조 목록이 3장 표와 정확히 일치한다.

### Step 4 — `GameLog` 구현

- 대상: `Assets/_Project/Scripts/Core/Logging/GameLog.cs`
- 네임스페이스: `BoomPG.Core.Logging`
- 변경: 아래 공개 API를 갖는 `static class GameLog` 를 만든다.

```csharp
public enum LogRole { Unknown, Host, Client }

public static void SetRole(LogRole role);

[Conditional("UNITY_EDITOR"), Conditional("DEBUG")]
public static void Core(string message);

[Conditional("UNITY_EDITOR"), Conditional("DEBUG")]
public static void Combat(string message);

[Conditional("UNITY_EDITOR"), Conditional("DEBUG")]
public static void Net(string message);

[Conditional("UNITY_EDITOR"), Conditional("DEBUG")]
public static void UI(string message);

public static void Warn(string category, string message);
public static void Error(string category, string message);
```

- 출력 형식: `[<역할>][<카테고리>] <메시지>` — 역할은 `Unknown`→`?`, `Host`→`H`, `Client`→`C`.
  예: `[?][Combat] 넉백 18.0 m/s 적용`
- `Warn`/`Error` 에는 `Conditional` 을 **붙이지 않는다.** 릴리즈 빌드에도 남아야 한다.
- 내부 구현은 `UnityEngine.Debug.Log` / `LogWarning` / `LogError` 를 호출한다.
  **이 파일이 `Debug.Log` 를 직접 호출하는 유일한 파일이다.**
- `System.Diagnostics.Conditional` 과 `UnityEngine.Debug` 의 이름 충돌에 주의한다
  (`using Debug = UnityEngine.Debug;` 를 쓰거나 완전 수식명을 쓴다).
- 완료 기준: 컴파일이 통과하고, `GameLog.Combat("테스트")` 호출 시 콘솔에 `[?][Combat] 테스트` 가 찍힌다.

### Step 5 — 설정 ScriptableObject 3종

- 대상: `Assets/_Project/Scripts/Gameplay/Config/` 아래 3개 파일
- 네임스페이스: `BoomPG.Gameplay.Config`
- 모든 필드는 `[SerializeField] private` + 읽기 전용 프로퍼티로 노출한다. public 필드 금지.
- 각 클래스에 `[CreateAssetMenu(menuName = "BoomPG/<이름>")]` 을 붙인다.

**`MovementConfig.cs`** — 값은 gdd 5.1

| 프로퍼티 | 타입 | 기본값 | 단위 |
|---|---|---|---|
| `MoveSpeed` | float | 6.0 | m/s |
| `JumpHeight` | float | 1.8 | m |
| `AirControl` | float | 0.4 | 비율 |
| `Gravity` | float | -9.81 | m/s² |
| `GroundDrag` | float | 8.0 | m/s per s |
| `AirDrag` | float | 2.5 | m/s per s |
| `CapsuleRadius` | float | 0.4 | m |
| `CapsuleHeight` | float | 1.8 | m |

**`JetpackConfig.cs`** — 값은 gdd 7장

| 프로퍼티 | 타입 | 기본값 | 단위 |
|---|---|---|---|
| `MaxFuel` | float | 100 | — |
| `ConsumePerSecond` | float | 25 | /초 |
| `AscendSpeed` | float | 6.5 | m/s |
| `HorizontalSpeed` | float | 4.0 | m/s |
| `RefillDelay` | float | 1.5 | 초 |
| `RefillPerSecond` | float | 20 | /초 |
| `HitLockSeconds` | float | 1.0 | 초 |

**`CombatConfig.cs`** — 값은 gdd 6.1 · 6.2 · 6.3, 합성 규칙은 gdd 23.2

먼저 아래 보조 타입을 같은 파일에 선언한다.

```csharp
[Serializable]
public struct SplashBand
{
    [SerializeField] private float _maxDistance;      // 이 거리 이하일 때 적용 (m)
    [SerializeField] private float _damage;
    [SerializeField] private float _knockbackSpeed;   // m/s
    public float MaxDistance { get; }
    public float Damage { get; }
    public float KnockbackSpeed { get; }
}

public enum KnockbackBlendMode { Additive, KeepStronger, AdditiveDamped }
```

| 프로퍼티 | 타입 | 기본값 | 단위 |
|---|---|---|---|
| `RocketSpeed` | float | 28 | m/s |
| `RocketGravityScale` | float | 0.35 | 배율 |
| `ReloadSeconds` | float | 2.2 | 초 |
| `ExplosionRadius` | float | 4.5 | m |
| `DirectHitDamage` | float | 35 | — |
| `DirectHitKnockback` | float | 14 | m/s |
| `SplashBands` | `SplashBand[]` | 아래 4개 | — |
| `KnockbackUpMin` | float | 0.35 | 방향 y 하한 |
| `SelfDamageRatio` | float | 0.5 | 비율 |
| `SelfKnockbackRatio` | float | 1.0 | 비율 |
| `BlendMode` | `KnockbackBlendMode` | `Additive` | — |
| `MaxKnockbackSpeed` | float | 25 | m/s |

`SplashBands` 기본값 (거리 오름차순):

| MaxDistance | Damage | KnockbackSpeed |
|---|---|---|
| 1.0 | 20 | 18 |
| 2.5 | 12 | 13 |
| 4.0 | 6 | 8 |
| 4.5 | 3 | 4 |

**위 기본값은 전부 `[SerializeField]` 필드 이니셜라이저로 코드에 박는다.**
`SplashBands` 배열도 마찬가지로 4개 원소를 이니셜라이저로 채운다.
이렇게 해야 `ScriptableObject.CreateInstance<CombatConfig>()` 만으로 기본값이 채워진 인스턴스를
얻을 수 있고, Step 7의 테스트가 별도 설정 없이 돌아간다.

- 완료 기준: 3개 SO 클래스가 컴파일되고, Unity 메뉴 `Assets > Create > BoomPG` 에 3개 항목이 보인다.
  `ScriptableObject.CreateInstance<CombatConfig>().SplashBands.Length == 4` 이다.

### Step 6 — `KnockbackCalculator` (넉백 순수 계산)

- 대상: `Assets/_Project/Scripts/Gameplay/Combat/KnockbackCalculator.cs`
- 네임스페이스: `BoomPG.Gameplay.Combat`
- **이 클래스는 `MonoBehaviour` 가 아니며 `static` 이다. Unity 씬·네트워크에 의존하지 않는다.**
  입력이 같으면 출력이 같아야 한다. 이 성질 때문에 EditMode 테스트가 가능하고, 네트워크
  솔루션이 바뀌어도 이 파일은 살아남는다.

```csharp
public static class KnockbackCalculator
{
    /// <summary>폭심 거리에 해당하는 스플래시 밴드를 찾는다. 반경 밖이면 false.</summary>
    /// <param name="distance">폭심으로부터의 거리 (m)</param>
    public static bool TryGetSplash(CombatConfig config, float distance,
                                    out float damage, out float knockbackSpeed);

    /// <summary>넉백 방향. y 성분에 하한을 적용해 항상 살짝 띄운다.</summary>
    public static Vector3 GetDirection(Vector3 explosionPos, Vector3 targetPos, float minUpward);

    /// <summary>기존 외부 속도와 새 넉백을 합성한다.</summary>
    public static Vector3 Blend(Vector3 current, Vector3 incoming,
                                KnockbackBlendMode mode, float maxSpeed);
}
```

**`TryGetSplash` 규칙**: `SplashBands` 를 앞에서부터 순회하며 `distance <= MaxDistance` 인
**첫 밴드**를 쓴다. 어느 밴드에도 속하지 않으면 `false` 를 반환하고 out 값은 0으로 둔다.

**`GetDirection` 규칙** (gdd 6.2 코드 블록과 동일):

```csharp
Vector3 dir = (targetPos - explosionPos).normalized;
dir.y = Mathf.Max(dir.y, minUpward);
return dir.normalized;
```

단, `targetPos == explosionPos` 로 방향이 0벡터가 되는 경우 `Vector3.up` 을 반환한다
(0벡터를 정규화하면 0이 되어 넉백이 사라진다).

**`Blend` 규칙** — gdd 23.2

| 모드 | 계산 |
|---|---|
| `Additive` | `Vector3.ClampMagnitude(current + incoming, maxSpeed)` |
| `KeepStronger` | `incoming.sqrMagnitude > current.sqrMagnitude ? incoming : current` |
| `AdditiveDamped` | `Vector3.ClampMagnitude(current * 0.5f + incoming, maxSpeed)` |

- 완료 기준: 컴파일 통과. Step 7의 테스트가 전부 통과한다.

### Step 7 — EditMode 테스트

- 대상: `Assets/_Project/Scripts/Tests/EditMode/KnockbackCalculatorTests.cs`
- 네임스페이스: `BoomPG.Tests.EditMode`
- `CombatConfig` 인스턴스는 `ScriptableObject.CreateInstance<CombatConfig>()` 로 만든다.
  Step 5에서 기본값을 필드 이니셜라이저로 넣었으므로 **추가 설정 없이 그대로 쓸 수 있다.**
  테스트 전용 팩토리나 세터를 만들지 말고, 리플렉션도 쓰지 않는다.
  `[SetUp]` 에서 만들고 `[TearDown]` 에서 `ScriptableObject.DestroyImmediate` 로 정리한다.
- 아래 케이스를 모두 작성한다.

| # | 대상 | 입력 | 기대 |
|---|---|---|---|
| 1 | `TryGetSplash` | distance 0.0 | true, damage 20, knockback 18 |
| 2 | `TryGetSplash` | distance 1.0 | true, damage 20, knockback 18 (경계 포함) |
| 3 | `TryGetSplash` | distance 1.01 | true, damage 12, knockback 13 |
| 4 | `TryGetSplash` | distance 4.5 | true, damage 3, knockback 4 |
| 5 | `TryGetSplash` | distance 4.51 | **false** |
| 6 | `GetDirection` | 폭심 (0,0,0), 대상 (1,0,0), minUpward 0.35 | y 성분이 0보다 크고, 결과 크기가 1 |
| 7 | `GetDirection` | 폭심과 대상이 같은 좌표 | `Vector3.up` |
| 8 | `Blend` Additive | current (0,0,18), incoming (0,0,4), max 25 | (0,0,22) |
| 9 | `Blend` Additive 상한 | current (0,0,18), incoming (0,0,18), max 25 | 크기 25 |
| 10 | `Blend` Additive 상쇄 | current (0,0,18), incoming (0,0,-18), max 25 | 크기 0 |
| 11 | `Blend` KeepStronger | current (0,0,18), incoming (0,0,4) | (0,0,18) — **약한 폭발이 강한 넉백을 지우지 않는다** |
| 12 | `Blend` AdditiveDamped | current (0,0,18), incoming (0,0,4), max 25 | (0,0,13) |

- 부동소수 비교는 `Assert.AreEqual(expected, actual, 0.001f)` 를 쓴다.
- 완료 기준: 12개 테스트가 모두 통과한다 (6장 테스트 계획 참조).

### Step 8 — 입력 액션 정리

- 대상: `Assets/_Project/Settings/InputSystem_Actions.inputactions`
- **기존 파일의 구조를 최대한 그대로 둔다.** 이 파일은 Unity 템플릿이 만든 JSON이고,
  액션·바인딩·컨트롤 스킴을 손으로 지우면 참조가 깨지기 쉽다. M1 목적(넉백 검증)과도 무관하다.

현재 `Player` 맵에 이미 있는 액션 (확인 완료):
`Move`, `Look`, `Attack`, `Interact`, `Crouch`, `Jump`, `Previous`, `Next`, `Sprint`

- 변경 내용은 **딱 하나다**: `Player` 맵에 `Reload` 액션(Button)을 추가하고 키보드 `R` 에 바인딩한다.
- **`Fire` 라는 새 액션을 만들지 않는다.** 이미 있는 `Attack` 을 발사에 그대로 쓴다.
  `Attack` 의 바인딩에 마우스 좌클릭(`<Mouse>/leftButton`)이 있는지 확인하고, 없을 때만 추가한다.
- `Interact`·`Crouch`·`Previous`·`Next`·`Sprint` 는 M1에서 쓰지 않지만 **지우지 않는다.**
  Touch·Gamepad·XR 컨트롤 스킴과 바인딩도 **그대로 둔다** — PC에서 해가 없다.
  정리는 별도 작업으로 미룬다 (7장 범위 밖).

- 완료 기준: `Player` 맵에 `Reload` 액션이 존재하고 `R` 키에 바인딩돼 있다.
  기존 액션이 하나도 사라지지 않았다. Unity 에디터에서 파일을 열었을 때 파싱 에러가 없다.

### Step 9 — `PlayerMotor`

- 대상: `Assets/_Project/Scripts/Gameplay/Player/PlayerMotor.cs`
- 네임스페이스: `BoomPG.Gameplay.Player`
- `[RequireComponent(typeof(CharacterController))]`

```csharp
public bool IsGrounded { get; }
public Vector3 ExternalVelocity { get; }

/// <summary>이동 입력. 카메라 yaw 로 이미 변환된 <b>월드 XZ 단위 방향</b>을 받는다.</summary>
public void SetMoveInput(Vector3 worldDirection);
public void RequestJump();
public void ApplyKnockback(Vector3 velocity, bool isPull = false);
public void SetVerticalVelocity(float value);      // 제트팩이 상승 속도를 덮어쓸 때 (m/s)
public void SetHorizontalAssist(Vector3 velocity); // 제트팩 공중 수평 이동 (m/s)

/// <summary>누적 속도를 전부 0으로. 리스폰 시 호출한다 (Step 11).</summary>
public void ResetVelocity();
```

`SetMoveInput` 이 **Vector2 가 아니라 월드 방향 Vector3** 인 이유: 카메라 기준 변환을
`PlayerInputRelay`(Step 14)가 담당하므로, `PlayerMotor` 는 카메라를 알 필요가 없다.
더미 캐릭터처럼 입력이 없는 개체도 같은 컴포넌트를 그대로 쓴다.

**내부 동작**

1. 속도를 세 성분으로 나눠 관리한다: `_inputVelocity`, `_externalVelocity`, `_verticalVelocity`.
2. `ApplyKnockback` 은 **대입이 아니라 합성**이다.
   `_externalVelocity = KnockbackCalculator.Blend(_externalVelocity, velocity, config.BlendMode, config.MaxKnockbackSpeed);`
   — 이것이 D-022 대응의 핵심이다. 단순 대입으로 구현하면 안 된다.
3. `isPull == false` 일 때만 `JetpackController.Lock(JetpackConfig.HitLockSeconds)` 를 호출한다
   (견인에는 제트팩 잠금이 걸리지 않는다).
4. `Update` 에서 `_externalVelocity` 를 `Vector3.MoveTowards(_externalVelocity, Vector3.zero, drag * Time.deltaTime)`
   로 감쇠시킨다. `drag` 는 접지 시 `MovementConfig.GroundDrag`, 공중은 `AirDrag`.
5. 공중에서는 `SetMoveInput` 의 영향력에 `MovementConfig.AirControl` 을 곱한다.
6. 점프 초기 속도는 `Mathf.Sqrt(2f * -Gravity * JumpHeight)` 로 계산한다.
   `RequestJump` 는 `IsGrounded` 일 때만 받는다.
7. 최종 이동: `_controller.Move((_inputVelocity + _externalVelocity + Vector3.up * _verticalVelocity) * Time.deltaTime);`
8. `JetpackController` 참조는 `Awake` 에서 `GetComponent` 로 한 번 캐싱한다. 없으면 `null` 을 허용한다.
9. `CombatConfig`·`MovementConfig` 는 `[SerializeField]` 로 주입받는다. `Awake` 에서 null 검사 후
   `GameLog.Error("Player", ...)` 를 남기고 `enabled = false`.

- 완료 기준: 컴파일 통과. 씬에서 WASD 이동과 Space 점프가 동작하고, 인스펙터에서
  `ApplyKnockback` 을 호출할 수단 없이도 Step 13의 폭발로 캐릭터가 밀려난다.

### Step 10 — `JetpackController`

- 대상: `Assets/_Project/Scripts/Gameplay/Player/JetpackController.cs`
- 네임스페이스: `BoomPG.Gameplay.Player`

```csharp
public float FuelRatio { get; }      // 0~1
public bool IsLocked { get; }

/// <summary>추진 입력. worldDirection 은 PlayerMotor 와 같은 월드 XZ 단위 방향이다.</summary>
public void SetThrustInput(bool held, Vector3 worldDirection);
public void Lock(float seconds);
```

`SetThrustInput` 이 방향을 함께 받는 이유: 규칙 2의 수평 보조 이동에 이동 입력이 필요한데,
`JetpackController` 는 입력 시스템을 직접 알지 않기 때문이다. 둘 다 가진 `PlayerInputRelay`
(Step 14)가 한 번에 넘긴다.

**규칙** (gdd 7장)

1. `SetThrustInput(true, ...)` 이고 `IsLocked == false` 이고 연료 > 0 이고
   `PlayerMotor.IsGrounded == false` 일 때만 추진한다.
2. 추진 중: 연료를 `ConsumePerSecond * Time.deltaTime` 만큼 줄이고,
   `PlayerMotor.SetVerticalVelocity(AscendSpeed)` 와
   `PlayerMotor.SetHorizontalAssist(worldDirection * HorizontalSpeed)` 를 호출한다.
3. 회복: 접지 후 `RefillDelay` 초가 지나면 `RefillPerSecond` 속도로 회복한다.
   공중에서는 회복하지 않는다.
4. `Lock(seconds)` 는 남은 잠금 시간과 비교해 **더 긴 쪽**으로 갱신한다.
- 완료 기준: 공중에서 Space 홀드 시 상승하고, 연료가 4초 만에 소진된다.
  접지 1.5초 후부터 회복이 시작된다.

### Step 11 — `HealthComponent` 와 `KillZoneTrigger`

- 대상:
  - `Assets/_Project/Scripts/Gameplay/Player/HealthComponent.cs`
  - `Assets/_Project/Scripts/Gameplay/World/KillZoneTrigger.cs`

```csharp
// BoomPG.Gameplay.Player
public enum DeathCause { Damage, RingOut }

public float Current { get; }
public bool IsDead { get; }
public event Action<HealthComponent, DeathCause, GameObject> Died;  // 마지막 인자는 가해자 (없으면 null)
public void ApplyDamage(float amount, GameObject source);
public void KillByRingOut(GameObject credit);
```

- 최대 HP는 상수 100f 를 `HealthComponent` 에 `[SerializeField] private float _maxHealth = 100f;`
  로 둔다. HP는 gdd 5.1의 공통 스펙이며 `MovementConfig` 소관이 아니다.
- 사망 시 `Died` 를 한 번만 발생시키고, 오브젝트를 파괴하지 않는다.
  **M1에서는 사망 처리 = 초기 스폰 지점으로 되돌리고 HP를 최대로 복구한다.**
  (리스폰 규칙은 M2 소관이지만, 검증 중 더미가 사라지면 반복 실험이 불가능하다)
- 스폰 지점은 `Awake` 에서 `transform.position` 을 `_spawnPoint` 에 저장해 쓴다.
- **되돌릴 때 주의 두 가지:**
  1. `CharacterController` 가 붙어 있으면 `transform.position` 대입이 무시될 수 있다.
     반드시 아래 순서로 한다.
     ```csharp
     _controller.enabled = false;
     transform.position = _spawnPoint;
     _controller.enabled = true;
     ```
  2. `PlayerMotor` 의 누적 속도를 함께 초기화한다. 하지 않으면 밀려나던 속도가 남아
     리스폰 직후 그대로 다시 날아간다. `PlayerMotor.ResetVelocity()` 를 호출한다.
- `GameLog.Combat($"{name} 사망 ({cause})")` 를 남긴다.

```csharp
// BoomPG.Gameplay.World — KillZone 레이어 오브젝트에 붙인다
private void OnTriggerEnter(Collider other);  // HealthComponent 를 찾아 KillByRingOut(null) 호출
```

- 완료 기준: 캐릭터가 Y = -20 아래로 떨어지면 콘솔에 사망 로그가 찍히고 스폰 지점으로 돌아온다.

### Step 12 — `RocketProjectile` 과 `RpgLauncher`

- 대상:
  - `Assets/_Project/Scripts/Gameplay/Combat/RocketProjectile.cs`
  - `Assets/_Project/Scripts/Gameplay/Combat/RpgLauncher.cs`

**`RocketProjectile`**

```csharp
public void Launch(Vector3 origin, Vector3 direction, GameObject owner, CombatConfig config);
```

1. `Rigidbody` 를 쓰지 않는다. 속도를 직접 관리한다
   ([ADR-0004](../docs/decisions/ADR-0004-photon-fusion-2.md)).
2. `FixedUpdate` 에서 `_velocity += Vector3.up * (Physics.gravity.y * config.RocketGravityScale * Time.fixedDeltaTime);`
   로 포물선을 만들고 위치를 갱신한다.
3. 충돌 검사는 이전 위치 → 새 위치 구간의 `Physics.Linecast` 로 한다.
   매 프레임 위치만 갱신하면 28 m/s 에서 얇은 벽을 통과한다.
   충돌 마스크는 `Player | Platform` 이다. 매 프레임 `LayerMask.GetMask` 를 호출하지 않되,
   **`static readonly` 필드 이니셜라이저로 만들지 마십시오.**

   ```csharp
   // 이렇게 하면 안 된다 — MonoBehaviour 인스턴스 생성 시점에 예외가 난다
   private static readonly int CollisionMask = LayerMask.GetMask("Player", "Platform");
   ```

   `LayerMask.GetMask` 와 `NameToLayer` 는 `MonoBehaviour` 의 생성자·필드 이니셜라이저에서
   호출할 수 없다 (`UnityException: NameToLayer is not allowed to be called from a
   MonoBehaviour constructor`). **`Awake` 에서 초기화한다.**

   ```csharp
   private static int _collisionMask = -1;
   private static int _playerLayer = -1;

   private void Awake()
   {
       if (_collisionMask < 0)
       {
           _collisionMask = LayerMask.GetMask("Player", "Platform");
           _playerLayer = LayerMask.NameToLayer("Player");
       }
   }
   ```

   `static` 을 유지하므로 로켓이 여러 개 생겨도 계산은 한 번뿐이다.
   Step 13의 `ExplosionResolver` 는 `MonoBehaviour` 가 아닌 정적 클래스라 이 제약을 받지 않는다.
4. 충돌 시 `ExplosionResolver.Resolve(...)` 를 호출하고 자신을 `Destroy` 한다.
   충돌한 콜라이더가 `Player` 레이어면 그 `GameObject` 를 `directHitTarget` 으로 넘기고,
   아니면 `null` 을 넘긴다.
5. 발사 후 5초가 지나면 폭발 없이 `Destroy` 한다 (무한 비행 방지).
6. **자기 자신과 발사자를 무시한다.** 로켓은 발사자 몸 안에서 출발하므로, `Linecast` 결과가
   `owner` 이면 충돌로 치지 않고 통과시킨다. 그렇지 않으면 발사 즉시 자폭한다.
   (로켓 점프는 지면에 맞고 터진 폭발의 스플래시로 발생하는 것이지 자폭이 아니다)

**`RpgLauncher`**

```csharp
public bool IsReloading { get; }
public float ReloadProgress { get; }   // 0~1
public void TryFire(Vector3 origin, Vector3 direction);
public void RequestReload();
```

1. 장탄 1발. 발사하면 즉시 빈 상태가 되고 `ReloadSeconds`(2.2초) 후 자동 장전된다.
2. `IsReloading` 중 `TryFire` 는 무시한다 (로그 없이 조용히 반환).
3. 로켓 프리팹은 `[SerializeField] private GameObject _rocketPrefab;` 로 주입받는다.
   **프리팹 자체와 주입은 Step 15의 에디터 메뉴가 만든다.** 여기서는 필드만 선언한다.
   `CombatConfig` 도 같은 방식으로 `[SerializeField]` 주입이다.
4. **오브젝트 풀을 쓰지 않는다.** `Instantiate`/`Destroy` 로 충분하다 (7장 범위 밖 참조).

- 완료 기준: 마우스 좌클릭으로 로켓이 발사되어 포물선을 그리며 날아가고, 지형에 맞으면 사라진다.
  2.2초 동안 재발사가 되지 않는다.

### Step 13 — `ExplosionResolver`

- 대상: `Assets/_Project/Scripts/Gameplay/Combat/ExplosionResolver.cs`
- 네임스페이스: `BoomPG.Gameplay.Combat`

```csharp
public static class ExplosionResolver
{
    public static void Resolve(Vector3 center, GameObject owner, GameObject directHitTarget,
                               CombatConfig config);
}
```

레이어 마스크는 매개변수로 받지 않는다. Step 2에서 레이어 이름이 확정되었으므로 클래스 안에
**`static readonly` 로 한 번만 캐싱한다.** 호출부가 마스크를 잘못 넘길 여지를 없애기 위해서다.

```csharp
private static readonly int TargetMask = LayerMask.GetMask("Player");
private static readonly int BlockMask  = LayerMask.GetMask("Platform");
```

**처리 순서**

1. `Physics.OverlapSphere(center, config.ExplosionRadius, TargetMask)` 로 대상을 모은다.
2. 각 대상에 대해 `Physics.Linecast(center, 대상 중심, BlockMask)` 로 벽 관통을 차단한다.
   막혀 있으면 그 대상은 건너뛴다.
3. **직격 판정**: 대상이 `directHitTarget` 과 같으면 `DirectHitDamage`(35)와
   `DirectHitKnockback`(14 m/s)를 쓴다. 아니면 `KnockbackCalculator.TryGetSplash` 로 밴드를 찾는다.
   밴드에 속하지 않으면 건너뛴다.
4. **자가 판정**: 대상이 `owner` 와 같으면 데미지에 `SelfDamageRatio`(0.5)를,
   넉백에 `SelfKnockbackRatio`(1.0)를 곱한다.
5. 방향은 `KnockbackCalculator.GetDirection(center, 대상 위치, config.KnockbackUpMin)`.
6. `HealthComponent.ApplyDamage(damage, owner)` 와
   `PlayerMotor.ApplyKnockback(direction * knockbackSpeed)` 를 호출한다.
7. 각 적용에 대해 `GameLog.Combat($"{대상 이름} 데미지 {damage} 넉백 {speed:F1} m/s")` 를 남긴다.

- `PlayerMotor`·`HealthComponent` 는 `GetComponent` 로 찾되, 없으면 조용히 건너뛴다
  (지형 오브젝트가 대상에 섞일 수 있다).
- 완료 기준: 더미 근처에서 로켓을 터뜨리면 더미가 밀려나고 HP가 줄어든다.
  자기 발밑에 쏘면 자신이 떠오르고 HP가 10 줄어든다.

### Step 14 — `ThirdPersonCamera` 와 `PlayerInputRelay`

- 대상:
  - `Assets/_Project/Scripts/Gameplay/Camera/ThirdPersonCamera.cs`
  - `Assets/_Project/Scripts/Gameplay/Player/PlayerInputRelay.cs`

**`ThirdPersonCamera`** — Cinemachine을 쓰지 않는다 (M2에 도입).

```csharp
public void AddLookInput(Vector2 delta);   // 마우스 델타 누적
```

1. `[SerializeField] private Transform _target;` 를 기준으로 구면 좌표 추적.
2. 기본 거리 5 m, 높이 오프셋 1.6 m, 마우스 감도는 `[SerializeField]` 로 노출.
3. 상하 회전 각도는 -40° ~ 70° 로 제한한다.
4. `LateUpdate` 에서 위치를 갱신한다. **충돌 회피(벽 뚫림 방지)는 구현하지 않는다** — M1 범위 밖.

**`PlayerInputRelay`** — 입력을 읽어 각 컴포넌트에 전달만 한다. 게임 규칙을 갖지 않는다.

주입받는 참조 (전부 `[SerializeField] private`):

| 필드 | 타입 |
|---|---|
| `_actions` | `UnityEngine.InputSystem.InputActionAsset` |
| `_camera` | `ThirdPersonCamera` |
| `_motor` · `_jetpack` · `_launcher` | 같은 오브젝트의 컴포넌트 (`Awake` 에서 `GetComponent` 로 캐싱해도 된다) |

`_actions` 에는 Step 8에서 정리한 `InputSystem_Actions.inputactions` 를 넣는다 (Step 15에서 주입).
`Awake` 에서 `_actions.FindActionMap("Player", true)` 로 맵을 얻고 각 액션을 필드에 캐싱한다.
`OnEnable` 에서 맵을 `Enable()`, `OnDisable` 에서 `Disable()` 한다.

**액션은 콜백 등록이 아니라 `Update` 에서 폴링한다.** 실행 순서가 명확하고 그레이박스에 충분하다.

| 액션 | 읽는 방법 | 전달 |
|---|---|---|
| `Move` | `ReadValue<Vector2>()` | 카메라 yaw 로 월드 방향 변환 후 `PlayerMotor.SetMoveInput(worldDir)` |
| `Look` | `ReadValue<Vector2>()` | `ThirdPersonCamera.AddLookInput(delta)` |
| `Jump` | `WasPressedThisFrame()` / `IsPressed()` | 눌린 프레임 → `PlayerMotor.RequestJump()`, 홀드 → `JetpackController.SetThrustInput(IsPressed(), worldDir)` |
| `Attack` | `WasPressedThisFrame()` | `RpgLauncher.TryFire(카메라 위치, 카메라 forward)` |
| `Reload` | `WasPressedThisFrame()` | `RpgLauncher.RequestReload()` |

카메라 기준 월드 방향 변환:

```csharp
Vector3 f = _camera.transform.forward; f.y = 0f; f.Normalize();
Vector3 r = _camera.transform.right;   r.y = 0f; r.Normalize();
Vector3 worldDir = (f * move.y + r * move.x).normalized;
```

- 완료 기준: 마우스로 시점이 돌고, 이동 방향이 카메라 기준으로 맞고, 좌클릭으로 조준 방향에 로켓이 나간다.

### Step 15 — 에디터 셋업 메뉴

- 대상: `Assets/_Project/Scripts/Editor/M1SetupMenu.cs`
- 네임스페이스: `BoomPG.Editor`
- **씬과 에셋을 손으로 조립하는 대신 코드로 생성한다.** 재현 가능하고 검토할 수 있다.

```csharp
[MenuItem("BoomPG/Setup/1. 기본 에셋 생성")]
private static void CreateBaseAssets();

[MenuItem("BoomPG/Setup/2. M1 그레이박스 씬 생성")]
private static void CreateGreyboxScene();
```

**`CreateBaseAssets`** — 설정 에셋 3종과 **로켓 프리팹**을 만든다.
이미 있으면 덮어쓰지 않고 `GameLog.Warn` 을 남기고 건너뛴다.

`Assets/_Project/Data/` 에:

- `SO_MovementConfig.asset`
- `SO_CombatConfig.asset`
- `SO_JetpackConfig.asset`

`Assets/_Project/Prefabs/` 에:

- `P_Rocket.prefab` — 구성은 아래와 같다.

| 항목 | 값 |
|---|---|
| 루트 | `GameObject.CreatePrimitive(PrimitiveType.Sphere)` |
| 이름 | `P_Rocket` |
| 스케일 | (0.3, 0.3, 0.3) |
| 레이어 | `Rocket` |
| `SphereCollider` | **제거한다** (`Object.DestroyImmediate`) — 충돌 판정은 Step 12의 `Linecast` 가 하므로 콜라이더는 불필요하고, 남겨두면 `CharacterController` 와 간섭한다 |
| `MeshRenderer` | 유지 (그레이박스에서 로켓이 보여야 한다) |
| 컴포넌트 | `RocketProjectile` 추가 |

`PrefabUtility.SaveAsPrefabAsset` 으로 저장한 뒤 임시로 만든 씬 오브젝트는 `DestroyImmediate` 한다.
`RocketProjectile` 은 `CombatConfig` 를 `Launch()` 인자로 받으므로 프리팹에 설정 에셋을 주입하지 않는다.

**`CreateGreyboxScene`** — `Assets/_Project/Scenes/M1_Greybox.unity` 를 새로 만들고 아래를 배치한다.

| 오브젝트 | 구성 |
|---|---|
| 메인 발판 | Cube, 스케일 (40, 1, 40), 위치 (0, 0, 0), 레이어 `Platform` |
| 부속 발판 ×3 | Cube, 스케일 (8, 1, 8), 위치 (26, 0, 0) / (-26, 0, 0) / (0, 0, 26), 레이어 `Platform` |
| 벽 ×1 | Cube, 스케일 (6, 4, 1), 위치 (0, 2, 10), 레이어 `Platform` — 폭발 차폐 확인용 |
| 킬존 | 빈 오브젝트 + BoxCollider(IsTrigger), 스케일 (400, 1, 400), 위치 (0, -20, 0), 레이어 `KillZone`, `KillZoneTrigger` |
| 플레이어 | Capsule, 위치 (0, 1, -8), 레이어 `Player`, `CharacterController` + `PlayerMotor` + `JetpackController` + `HealthComponent` + `RpgLauncher` + `PlayerInputRelay` |
| 더미 ×4 | Capsule, 위치 (±4, 1, 4) / (±8, 1, 8), 레이어 `Player`, `CharacterController` + `PlayerMotor` + `HealthComponent` |
| 카메라 | Main Camera + `ThirdPersonCamera`(target = 플레이어) |
| 조명 | Directional Light |

**주입 목록** — 에셋은 `AssetDatabase.LoadAssetAtPath` 로 찾는다.
하나라도 못 찾으면 `GameLog.Error` 를 남기고 **씬 생성을 중단한다** (반쯤 만들어진 씬을 남기지 않는다).

| 대상 | 필드 | 넣을 값 |
|---|---|---|
| `PlayerMotor` (플레이어 + 더미 4개 **전부**) | `_movementConfig` | `SO_MovementConfig.asset` |
| `PlayerMotor` (전부) | `_combatConfig` | `SO_CombatConfig.asset` |
| `JetpackController` | `_jetpackConfig` | `SO_JetpackConfig.asset` |
| `RpgLauncher` | `_combatConfig` | `SO_CombatConfig.asset` |
| `RpgLauncher` | `_rocketPrefab` | `P_Rocket.prefab` |
| `PlayerInputRelay` | `_actions` | `Assets/_Project/Settings/InputSystem_Actions.inputactions` |
| `PlayerInputRelay` | `_camera` | Main Camera 의 `ThirdPersonCamera` |
| `ThirdPersonCamera` | `_target` | 플레이어 `Transform` |

주입 대상이 전부 `private` 필드이므로 **`SerializedObject` 로 써야 한다.**

```csharp
var so = new SerializedObject(component);
so.FindProperty("_rocketPrefab").objectReferenceValue = prefab;
so.ApplyModifiedProperties();
```

필드 이름이 바뀌면 여기도 같이 바꿔야 한다. 문자열이라 컴파일러가 잡아주지 않으므로,
주입 후 `FindProperty` 결과가 `null` 인지 확인하고 `null` 이면 `GameLog.Error` 를 남긴다.

- 생성 후 `EditorSceneManager.SaveScene` 으로 저장한다.
- 완료 기준: 메뉴 두 개를 순서대로 실행하면 **에셋 3개 + 프리팹 1개 + 씬 1개**가 만들어지고,
  씬을 열어 Play 하면 조작이 가능하며, 좌클릭 시 로켓이 발사된다
  (`_rocketPrefab` 이 비어 있으면 발사가 안 되므로 이것이 주입 성공의 확인이 된다).

## 5. 인수 조건 (Acceptance Criteria)

**구조**

- [x] `Assets/_Project/` 로 이동 완료, `.meta` 짝 일치 (Step 1 — 검증 완료)
- [x] `Assets/_Project/Settings/` 에 `PC_*` 와 `Mobile_*` URP 에셋이 모두 남아 있다
- [x] `ProjectSettings/QualitySettings.asset` 이 변경되지 않았다
- [x] `Assets/TutorialInfo`, `Assets/Readme.asset` 삭제 (사람이 Unity에서 처리 — 완료)
- [x] `Assets/` 바로 아래에 `_Project` 와 `_Project.meta` 만 남음
- [ ] asmdef 6개가 존재하고 참조 목록이 3장 표와 일치한다
- [ ] `BoomPG.Gameplay.asmdef` 의 참조에 `BoomPG.Network` 가 **없다**
- [ ] `TagManager.asset` 8~13번에 `Player`, `Platform`, `Rocket`, `Pickup`, `KillZone`, `Grapple` 이 등록돼 있다
- [ ] 메뉴 1 실행 후 `Assets/_Project/Data/` 에 SO 3개, `Assets/_Project/Prefabs/P_Rocket.prefab` 이 생성된다
- [ ] `P_Rocket.prefab` 에 `RocketProjectile` 이 붙어 있고 `SphereCollider` 가 없다
- [ ] 메뉴 2 실행 후 씬의 `RpgLauncher._rocketPrefab` 이 비어 있지 않다

**코드 규약**

- [ ] `GameLog.cs` 를 제외한 어떤 파일도 `Debug.Log` / `Debug.LogWarning` / `Debug.LogError` 를 직접 호출하지 않는다
- [ ] public 필드가 없다 (`[SerializeField] private` + 프로퍼티)
- [ ] `Update`/`FixedUpdate` 안에 `GetComponent`, `Find`, `FindObjectOfType`, `Camera.main` 호출이 없다
- [ ] `PlayerMotor.ApplyKnockback` 이 `_externalVelocity` 에 **대입하지 않고** `KnockbackCalculator.Blend` 를 거친다
- [ ] `RocketProjectile` 이 `Rigidbody` 를 쓰지 않는다

**테스트**

- [ ] EditMode 테스트 12개가 모두 통과한다
- [ ] 테스트 11번(`KeepStronger` 에서 약한 넉백이 강한 넉백을 지우지 않음)이 통과한다

**플레이 검증** (사람이 Unity 에디터에서 확인)

- [ ] WASD 이동 · Space 점프 · 마우스 시점 회전이 동작한다
- [ ] 좌클릭으로 로켓이 포물선을 그리며 날아가고 2.2초 재장전이 걸린다
- [ ] 더미 발밑에 로켓을 터뜨리면 더미가 밀려나고 HP가 20 줄어든다
- [ ] 더미를 직격하면 HP가 35 줄어든다
- [ ] 벽 뒤의 더미는 폭발 영향을 받지 않는다
- [ ] 자기 발밑에 쏘면 떠오르고 HP가 10 줄어든다
- [ ] 공중에서 Space 홀드로 상승하며 연료가 약 4초에 소진된다
- [ ] 발판 밖으로 떨어져 Y = -20 을 넘으면 사망 로그가 뜨고 스폰 지점으로 돌아온다

## 6. 테스트 계획 (Test Plan)

### 6.1 기본 경로 — Unity MCP (`mcp__unity__*` 툴이 보일 때)

**이 경로를 우선 사용하십시오.** 배치 모드는 이 환경에서 라이선스 오류로 실패합니다(0장 참조).

1. 에셋 새로고침 / 컴파일 관련 툴을 실행해 **컴파일 에러 유무를 먼저 확인**한다.
   에러가 있으면 그 내용을 그대로 보고하고 멈춘다.
2. 메뉴 `BoomPG/Setup/1. 기본 에셋 생성` 과 `BoomPG/Setup/2. M1 그레이박스 씬 생성` 을
   실행할 수 있는 툴이 있으면 실행한다. 없으면 보고에 "메뉴 실행 불가"라고 적고 넘어간다.
3. `mcp__unity__run_tests` 로 EditMode 테스트를 실행하고 `mcp__unity__test_status` 로 결과를 받는다.
4. **툴이 돌려준 실제 출력을 보고에 그대로 넣는다.** 추측으로 "통과"라고 쓰지 않는다.

기대 결과: 테스트 **12개 전부 통과**. 특히
`Blend_KeepStronger_PreservesStrongerCurrentVelocity` 가 통과해야 D-022 대응이 성립한다.

MCP 쓰기 툴은 샌드박스 제한을 받지 않습니다. **계획서 범위 밖 파일을 MCP 로 건드리지 마십시오.**
씬·프리팹·프로젝트 설정을 바꾸는 `set_*` 계열은 이 계획서가 명시한 것 외에는 쓰지 마십시오.

### 6.2 대체 경로 — 배치 모드 (`mcp__unity__*` 툴이 없을 때만)

에디터를 닫은 상태에서만 동작하며, **현재 환경에서는 라이선스 오류로 실패하는 것이 확인되었습니다.**
그래도 시도해야 한다면 아래를 쓰고, 실패하면 실패한 출력을 그대로 보고하십시오.

| 명령어 | 기대 결과 |
|---|---|
| `"C:\Program Files\Unity\Hub\Editor\6000.6.0f1\Editor\Unity.exe" -batchmode -runTests -projectPath "C:\개인 폴더\BoomPG" -testPlatform EditMode -testResults "C:\개인 폴더\BoomPG\Temp\m1-tests.xml" -logFile -` | 종료 코드 0. XML 의 `<test-run>` 에 `total="12" passed="12" failed="0"` |

### 6.3 공통

| 명령어 | 기대 결과 |
|---|---|
| `git status --short` | `Assets/_Project/`, `ProjectSettings/TagManager.asset` 외의 변경이 없다 |

컴파일 에러가 나면 그 지점에서 멈추고 보고하십시오. 다음 스텝으로 넘어가지 마십시오.

## 7. 범위 밖 (Out of Scope)

**이번에 만들지 않는 것** — 필요해 보여도 손대지 마십시오.

- 네트워크 일체 — Photon Fusion 2 설치, 권한 처리, RPC, 동기화 (M2)
- Cinemachine 패키지 추가 (M2)
- 오브젝트 풀링 — M1은 `Instantiate`/`Destroy` 로 간다 (M2)
- 맵 축소(`CollapseManager`), 킬 크레딧 5초 룰(`KillCreditTracker`) (M2)
- 클래스 스킬 3종(대시·예비탄·갈고리), 특수 탄두, 강화 칩, RPG 등급 (M3)
- 다운 시스템, 치료제 총, 모드 B / 모드 C (M3)
- HUD·UI 일체 — 연료 게이지, 크로스헤어, 킬 로그, 결과 화면
- 아트·사운드·VFX — 회색 기본 머티리얼로 충분하다
- 카메라 벽 충돌 회피
- AI 봇 — 더미는 움직이지 않는 캡슐이다
- **Mobile URP 에셋 삭제와 모바일 품질 레벨 정리** — `QualitySettings.asset` 의 참조 때문에
  단독으로 지울 수 없다. 3장 인용 블록 참조. 별도 작업으로 분리한다
- **입력 액션 정리** — 안 쓰는 액션(`Interact`·`Crouch`·`Sprint` 등)과 Touch·Gamepad·XR
  컨트롤 스킴 삭제. PC에서 해가 없고 JSON 을 손대는 위험만 크다. 별도 작업으로 분리한다
- **`Assets/TutorialInfo/` 와 `Assets/Readme.asset` 삭제** — 샌드박스 정책이 파일 삭제를
  거부한다. 사람이 Unity 에디터의 Project 창에서 지운다 (에디터가 `.meta` 까지 함께 정리한다).
  **삭제를 다시 시도하지 마십시오.** 남아 있어도 이후 스텝에 영향이 없다
- **`ProjectSettings/ProjectSettings.asset` 의 `activeInputHandler`** — 현재 `2`(Both) 라서
  "Input Manager is marked for deprecation" 경고가 뜬다. Input System 전용(`1`)으로 바꾸려면
  에디터 재시작이 필요하고 M1 목적과 무관하다. **건드리지 마십시오.** 별도 작업으로 분리한다

**건드리면 안 되는 것**

- `Assets/_Project/Settings/` 의 URP 에셋 내용 (이동·모바일 에셋 삭제 외)
- `Packages/manifest.json` — 새 패키지를 추가하지 마십시오
- `ProjectSettings/` 의 `TagManager.asset` 을 제외한 모든 파일
- `docs/` 아래 전부 — **읽기만 하십시오.** 문서는 아키텍트만 씁니다
- `TaskPlan/` 아래 전부 (이 계획서 포함)
- `scripts/`, `.claude/` — 기획 워크플로우 키트
- git 커밋 · 브랜치 조작 — 커밋은 사람이 합니다

**판단이 필요해지면 멈추고 보고하십시오.** 특히 asmdef 참조를 늘려야 할 것 같은 상황은
레이어 원칙을 어기려는 신호입니다. 임의로 추가하지 마십시오.
