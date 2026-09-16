# TASK_PLAN — 마우스 커서 잠금 (테스트 편의)

> 작성: Claude Code (Architect) · 구현: Codex (Executor)
> 이 파일은 Codex의 유일한 지시서입니다. 모호한 문장이 남아 있으면 계획이 미완성입니다.

## 0. 시작 전 반드시 읽을 것

`docs/rules/code-style.md` — 네이밍, 로깅(`GameLog`), 인스펙터 필드 규칙(6.1)

### 작업 성격

신규 파일 1개 + 기존 파일 1개의 소폭 수정. Unity 에디터가 열려 있어도 무방합니다.
**씬 파일(`M1_Greybox.unity`)과 프리팹을 수정하지 마십시오.** 아래 설계는 씬을 건드리지 않고
동작하도록 만들었습니다.

### 멈춰야 할 때와 계속해야 할 때

- **멈춰야 하는 경우**: 이 계획서에 없는 파일을 고쳐야 할 때, 지시가 서로 모순될 때
- **멈추지 말 것**: 환경 상태 확인, 도구가 특정 명령을 거부할 때(그 항목만 건너뛰고 계속)

## 1. 목표 (Goal)

M1 그레이박스를 플레이로 검증하는 중인데, 마우스 커서가 게임 화면 위에 그대로 떠 있어
시점을 돌리다 보면 커서가 화면 밖으로 나가 에디터의 다른 창을 클릭하게 된다. 3인칭 슈터를
조작하는 것 자체가 어려워 테스트가 방해받는다. **게임 화면을 클릭하면 커서가 사라져 화면 중앙에
잠기고, `Esc` 를 누르면 커서가 돌아오게** 만든다. 커서가 풀린 동안에는 시점과 이동 입력이
들어가지 않아야 한다 — 그렇지 않으면 커서를 꺼내 인스펙터를 만지는 동안 카메라가 제멋대로 돈다.

이것은 게임 기능이 아니라 **테스트 편의 장치**다. 최소한으로 만들고 씬 구성을 건드리지 않는다.

## 2. 현재 구조 분석 (Current State)

- `Assets/_Project/Scripts/Gameplay/Player/PlayerInputRelay.cs` 가 Input System 액션을
  `Update` 에서 폴링해 `PlayerMotor` · `JetpackController` · `ThirdPersonCamera` · `RpgLauncher`
  에 전달한다. 커서 관련 처리는 없다.
- 씬 `M1_Greybox.unity` 는 `M1SetupMenu` 가 코드로 생성한 것이며, 플레이어 오브젝트에
  위 컴포넌트들이 붙어 있다.
- `BoomPG.Gameplay` asmdef 는 `Unity.InputSystem` 을 참조한다. 새 파일도 같은 어셈블리에 둔다.

### 지켜야 할 컨벤션

- 네이밍: 클래스 PascalCase / private 필드 `_camelCase`
- 네임스페이스는 폴더와 1:1 (`Gameplay/Player/` → `BoomPG.Gameplay.Player`)
- `Debug.Log` 직접 호출 금지. `GameLog` 를 쓴다
- `Update` 안에서 `GetComponent`·`Find`·`Camera.main` 호출 금지
- 인스펙터 노출 필드에는 `[Tooltip]` 한국어 설명 (code-style 6.1)
- 주석은 한국어, **무엇이 아니라 왜**를 적는다
- C# 파일 UTF-8(BOM 없음), 들여쓰기 공백 4칸

## 3. 영향 범위 (Files & Architecture)

| 파일 | 변경 유형 | 역할 |
|---|---|---|
| `Assets/_Project/Scripts/Gameplay/Player/CursorLockController.cs` | **신규** | 커서 잠금·해제 |
| `Assets/_Project/Scripts/Gameplay/Player/PlayerInputRelay.cs` | 수정 | 커서가 풀린 동안 입력 중계 중단 |

**씬·프리팹·설정 에셋은 수정하지 않는다.**

## 4. 구현 스텝 (Step-by-Step)

### Step 1 — `CursorLockController` 신규 작성

- 대상: `Assets/_Project/Scripts/Gameplay/Player/CursorLockController.cs`
- 네임스페이스: `BoomPG.Gameplay.Player`

**씬에 배치하지 않고 자동으로 생성되게 만든다.** 이미 만들어진 `M1_Greybox.unity` 를 고치지
않아도 동작해야 하고, 앞으로 어떤 씬에서 Play 해도 같이 따라와야 하기 때문이다.

```csharp
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
private static void Bootstrap()
{
    // 씬에 배치하지 않아도 플레이 시 자동으로 붙는다. 테스트 편의 장치이므로
    // 씬 구성을 바꾸지 않는 쪽을 택했다.
    var host = new GameObject("[CursorLock]");
    host.AddComponent<CursorLockController>();
    Object.DontDestroyOnLoad(host);
}
```

**공개 API**

```csharp
/// <summary>커서가 화면 중앙에 잠겨 게임 입력을 받는 상태인가.</summary>
public static bool IsLocked => Cursor.lockState == CursorLockMode.Locked;
```

**동작 규칙**

1. **플레이 시작 시에는 잠그지 않는다.** 커서가 보이는 상태로 시작한다.
   에디터에서 Play 를 누른 직후 인스펙터를 만질 수 있어야 하기 때문이다.
2. 커서가 **풀린 상태**에서 마우스 **왼쪽 버튼을 누르면 잠근다.**
   ```csharp
   Cursor.lockState = CursorLockMode.Locked;
   Cursor.visible = false;
   ```
3. `Esc` 키를 누르면 **푼다.**
   ```csharp
   Cursor.lockState = CursorLockMode.None;
   Cursor.visible = true;
   ```
4. 입력은 Input System 으로 읽는다. `Keyboard.current.escapeKey.wasPressedThisFrame` 와
   `Mouse.current.leftButton.wasPressedThisFrame` 를 쓴다.
   `Keyboard.current` · `Mouse.current` 가 `null` 일 수 있으므로 반드시 검사한다.
   - `.inputactions` 에 액션을 추가하지 않는다. 이것은 게임플레이 액션이 아니라 창 제어이고,
     `PlayerInputRelay` 가 멈춘 동안에도 동작해야 하므로 액션 맵에 의존하면 안 된다.
5. `Esc` 검사를 클릭 검사보다 **먼저** 하고, 같은 프레임에 둘 다 처리되지 않게 한다
   (`else if`). 그렇지 않으면 `Esc` 로 풀자마자 같은 프레임의 클릭으로 다시 잠길 수 있다.
6. `OnApplicationFocus(bool hasFocus)` 에서 `hasFocus == false` 이면 커서를 푼다.
   Alt+Tab 으로 다른 창에 갔다가 돌아왔을 때 커서가 잡힌 채로 남지 않게 한다.
7. `OnDestroy` 에서 커서를 푼다. 플레이를 멈췄을 때 에디터에 커서가 없으면 곤란하다.
8. 잠금·해제 시 `GameLog.Core($"커서 잠금: {IsLocked}")` 같은 로그는 **남기지 않는다.**
   매 전환마다 콘솔이 지저분해지고, 지금 콘솔은 전투 로그를 읽는 용도다.

- 완료 기준: 컴파일 통과. Play 후 게임 화면을 클릭하면 커서가 사라지고, `Esc` 로 돌아온다.

### Step 2 — `PlayerInputRelay` 가 잠금 상태를 존중하게 한다

- 대상: `Assets/_Project/Scripts/Gameplay/Player/PlayerInputRelay.cs`
- 변경: `Update()` 의 **맨 처음**에 아래 조기 반환을 넣는다.

```csharp
private void Update()
{
    // 커서가 풀린 동안에는 입력을 중계하지 않는다.
    // 그렇지 않으면 인스펙터를 만지는 사이에 시점이 돌아간다.
    if (!CursorLockController.IsLocked)
    {
        _motor.SetMoveInput(Vector3.zero);
        _jetpack.SetThrustInput(false, Vector3.zero);
        return;
    }

    // ... 기존 코드 ...
}
```

- `SetMoveInput(Vector3.zero)` 와 `SetThrustInput(false, ...)` 를 호출하는 이유: 잠금이 풀리기
  직전의 입력이 남아 캐릭터가 계속 움직이는 것을 막기 위해서다. 단순히 `return` 만 하면
  마지막 입력 값이 유지된다.
- `_jetpack` 이 `null` 일 수 있으면 `?.` 로 호출한다. 기존 코드의 null 처리 방식을 따른다.
- **이 외에 `PlayerInputRelay` 의 다른 부분을 수정하지 마십시오.**

- 완료 기준: 컴파일 통과. `Esc` 로 커서를 푼 상태에서 마우스를 움직여도 시점이 돌지 않고,
  WASD 를 눌러도 캐릭터가 움직이지 않는다.

### Step 3 — 컴파일 확인

- `mcp__unity__*` 툴이 보이면 에셋 새로고침 후 **컴파일 에러가 없는지 확인**하고 실제 출력을 보고한다.
- 툴이 없으면 "컴파일 확인 불가"라고 보고에 적는다. 배치 모드를 시도하지 않는다
  (이 환경에서는 라이선스 오류로 실패한다).
- EditMode 테스트는 이번 변경과 무관하므로 다시 돌리지 않아도 된다.

## 5. 인수 조건 (Acceptance Criteria)

- [ ] `CursorLockController.cs` 가 `Gameplay/Player/` 에 있고 네임스페이스가 `BoomPG.Gameplay.Player` 다
- [ ] `[RuntimeInitializeOnLoadMethod]` 로 자동 생성되며, **씬 파일이 수정되지 않았다**
- [ ] `public static bool IsLocked` 가 있다
- [ ] 플레이 시작 시 커서가 보이는 상태다 (자동으로 잠기지 않는다)
- [ ] 게임 화면 좌클릭 → 커서가 사라지고 중앙에 잠긴다
- [ ] `Esc` → 커서가 다시 보인다
- [ ] 같은 프레임에 `Esc` 와 클릭이 함께 처리되지 않는다 (`else if`)
- [ ] 애플리케이션 포커스를 잃으면 커서가 풀린다
- [ ] `OnDestroy` 에서 커서가 풀린다
- [ ] 커서가 풀린 동안 마우스를 움직여도 시점이 돌지 않고, WASD 로 이동하지 않는다
- [ ] 커서 전환 로그를 남기지 않는다
- [ ] `.inputactions` 에 액션을 추가하지 않았다
- [ ] 컴파일 에러가 없다
- [ ] `git status --short` 에 위 2개 파일 외의 변경이 없다
      (`Assets/_Project/Data/SO_CombatConfig.asset` 은 사람이 조정 중이므로 예외)

## 6. 테스트 계획 (Test Plan)

| 확인 | 기대 결과 |
|---|---|
| Unity MCP 로 에셋 새로고침 + 컴파일 확인 | 에러 0건 |
| `git diff --stat` | `CursorLockController.cs`(신규), `PlayerInputRelay.cs` 만 변경 |
| `git status --short` | `M1_Greybox.unity`, `P_Rocket.prefab` 이 목록에 없다 |

플레이 동작 확인은 사람이 한다. Codex 는 컴파일까지만 검증하고 보고한다.

## 7. 범위 밖 (Out of Scope)

- **씬·프리팹 수정** — `M1SetupMenu.cs` 도 고치지 않는다. 자동 생성 방식이라 필요 없다
- `.inputactions` 수정 — 커서 제어는 게임플레이 액션이 아니다
- 일시정지 메뉴·UI — 커서만 다룬다. `Time.timeScale` 을 건드리지 마라
- 설정 에셋(`SO_*.asset`)의 값 — 사람이 플레이하며 조정 중이다
- 다른 컴포넌트의 입력 처리 — `ThirdPersonCamera` 등은 `PlayerInputRelay` 를 통해서만
  입력을 받으므로 Step 2 만으로 충분하다
- `docs/`, `TaskPlan/`, `scripts/`, `.claude/` 아래 전부
- git 커밋 · 브랜치 조작 — 커밋은 사람이 한다
