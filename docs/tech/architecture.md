# 시스템 아키텍처 (Architecture)

게임 내용·수치의 정본은 [design/gdd.md](../design/gdd.md)입니다.
이 문서는 **구조와 경계**만 다룹니다. 스크립트별 역할표는 gdd 17.3장에 있습니다.

## 1. 기술 스택 (확정)

| 영역 | 선택 | 근거 |
|---|---|---|
| 엔진 | Unity 6000.6.0f1 + URP | 프로젝트 현황 (gdd 17.1은 "Unity 6 LTS"로 표기) |
| 네트워크 | Photon Fusion 2 (Host Mode) | [ADR-0004](../decisions/ADR-0004-photon-fusion-2.md) |
| 입력 | Input System | gdd 17.1 |
| 카메라 | Cinemachine 3 (**M2부터**) | 3인칭 + 견착 FOV 전환 + 관전 추적. M1은 자체 `ThirdPersonCamera` 스크립트로 진행한다 — 패키지가 아직 설치돼 있지 않고, 그레이박스 검증에는 단순 카메라가 디버깅에 유리하다 |
| 캐릭터 이동 | `CharacterController` + 수동 속도 관리 | **`Rigidbody` 미사용** — ADR-0004 |
| 목표 사양 | GTX 1050 / 1080p 60fps, 시뮬레이션 60 Hz | gdd 17.1 |

## 2. 현재 상태

게임 소스 코드는 아직 한 줄도 없습니다. `Assets/` 는 Unity URP 템플릿 그대로입니다.

| 경로 | 내용 | 처리 |
|---|---|---|
| `Assets/Scenes/SampleScene.unity` | URP 템플릿 기본 씬 | 그레이박스 씬으로 대체 예정 |
| `Assets/Settings/PC_*.asset` | PC용 URP 에셋 | 유지 |
| `Assets/Settings/Mobile_*.asset` | 모바일용 URP 에셋 | 제거 대상이나 **단독 삭제 불가** (아래) |
| `Assets/TutorialInfo/` | URP 템플릿 샘플 | 제거 대상 |
| `Assets/InputSystem_Actions.inputactions` | 기본 액션 맵 | gdd 4장 조작표로 재작성 |

정리 작업은 별도 계획서로 진행합니다.

### 2.1 Mobile URP 에셋이 단독으로 지워지지 않는 이유

[ADR-0002](../decisions/ADR-0002-pc-단독-플랫폼.md)는 모바일 렌더 에셋을 정리 대상으로 정했지만,
`ProjectSettings/QualitySettings.asset` 의 `Mobile` 품질 레벨(배열 인덱스 0)이
`Mobile_RPAsset.asset` 을 GUID `5e6cbd92db86f4b18aec3ed561671858` 로 참조하고 있습니다.

에셋만 지우면 끊어진 참조가 남고, 그것을 없애려면 품질 레벨 배열에서 항목을 빼야 하는데
그러면 `m_PerPlatformDefaultQuality` 의 인덱스 매핑을 전부 다시 맞춰야 합니다.
YAML 을 직접 손대면 위험하므로 **Unity 에디터의 Quality 설정 UI에서 처리하는 것이 안전합니다.**

현재 활성 품질은 `m_CurrentQuality: 1` (PC) 이므로 모바일 레벨이 남아 있어도 PC 빌드에 영향은
없습니다. 파일 이동은 GUID 기반이라 참조가 유지되므로 안전합니다.

> 후속 작업으로 남깁니다. 처리 방식은 세 가지입니다 — (a) 품질 레벨 통째로 제거 + 인덱스 재매핑,
> (b) Mobile 레벨의 `customRenderPipeline` 을 PC 에셋으로 교체한 뒤 모바일 에셋 삭제,
> (c) 그대로 둔다. 빌드 설정을 손볼 시점에 정합니다.
> `GraphicsSettings.asset` 에는 모바일 참조가 없음을 확인했습니다.

## 3. 레이어 원칙

의존 방향은 **항상 한 방향**입니다. 아래 레이어가 위 레이어를 참조하지 않습니다.

```
UI          ──┐
Network     ──┼──▶ Gameplay ──▶ Core
```

- `Core` — 다른 레이어를 모릅니다. 수학 유틸, 로깅, 상태 머신, 오브젝트 풀.
- `Gameplay` — 게임 규칙. **네트워크 코드를 직접 알지 않습니다.**
- `Network` — 권한, 동기화, 매치 라이프사이클.
- `UI` — 표시 전용. 게임 상태를 읽기만 하고 규칙을 갖지 않습니다.

### 왜 이 방향을 고집하는가

[ADR-0004](../decisions/ADR-0004-photon-fusion-2.md)는 되돌리는 비용이 가장 큰 결정이고,
D-014(모드 B 인원 상한) 실측 결과에 따라 전용 서버로 뒤집힐 가능성이 남아 있습니다.
넉백 계산을 **입력이 같으면 출력이 같은 순수 함수**로 유지하면, 네트워크 솔루션이 바뀌어도
게임 규칙은 살아남습니다.

구체적으로: 거리→데미지·넉백 변환(gdd 6.2 표)은 `Gameplay` 안의 순수 계산으로 두고,
`ExplosionResolver`가 **누구에게 언제 적용할지**만 호스트 권한으로 판단합니다.
이렇게 나누면 D-007(테스트)이 도입될 때 넉백 규칙을 엔진 없이 검증할 수 있습니다.

## 4. 오브젝트 풀 대상 (확정)

후반 난전에서 초당 수십 개가 생성되므로 아래는 반드시 풀링합니다 (gdd 17.4).

- 로켓 투사체 (호스트 실제 + 클라이언트 연출용 가짜)
- 폭발 VFX
- 축소 연출의 모듈 분리 파편

## 5. Layer / Physics (확정)

- Layer: `Player`, `Platform`, `Rocket`, `Pickup`, `KillZone`, `Grapple`
- `KillZone`: `Y = -20` 에 거대한 BoxCollider(IsTrigger) **1개**.
  매 프레임 전 플레이어의 Y 좌표를 비교하는 것보다 저렴합니다.
- 폭발: `OverlapSphere(pos, 4.5f, playerMask)` → 대상별 `Linecast`로 벽 관통 차단

## 6. 구현 순서 (MVP)

MVP 범위는 gdd 20장을 그대로 채택합니다. **클래스 3종은 후순위**입니다 —
M1의 검증 질문("밀어서 떨어뜨리는 게 재밌는가")에 클래스는 변수만 더합니다.

```
M1 그레이박스 → PlayerMotor · RpgLauncher · RocketProjectile · ExplosionResolver
                · JetpackController · KillZone
                ↑ D-022(넉백 합성 규칙) 검증이 M1의 최우선 과제
M2 온라인      → Fusion 2 8인 · CollapseManager · KillCreditTracker · 결과 화면
```

## 7. 테스트 (확정 — [ADR-0005](../decisions/ADR-0005-프로젝트-코드-구조.md))

**EditMode 테스트만 둡니다. PlayMode 통합 테스트는 만들지 않습니다.**

검증 대상은 3장에서 순수 함수로 분리한 로직입니다.

| 대상 | 검증 내용 |
|---|---|
| 거리 → 데미지·넉백 변환 | gdd 6.2 표의 경계값 (0.0 / 1.0 / 2.5 / 4.0 / 4.5 m) |
| **다중 폭발 합성 규칙** | D-022의 결론을 테스트로 고정한다 |
| 넉백 방향 계산 | `dir.y` 하한 0.35 적용 여부 |
| 킬 크레딧 5초 룰 | 경계 시각에서의 크레딧 귀속 |
| 축소 페이즈 스케줄 | 페이즈별 반경·시각 |

게임의 **재미는 테스트로 알 수 없습니다.** "밀어서 떨어뜨리는 게 재밌는가"(M1 검증 질문)는
플레이로만 판정합니다. 테스트는 규칙이 의도대로 계산되는지만 봅니다.

## 8. 게임 데이터 (확정 — [ADR-0005](../decisions/ADR-0005-프로젝트-코드-구조.md))

**밸런스 수치는 `ScriptableObject`로 둡니다.** 위치는 `Assets/_Project/Data/`.

- 대상: gdd 6.2 넉백·데미지 테이블, 6.1 RPG 스펙, 7장 제트팩 수치, 9.1 축소 스케줄,
  12.1 RPG 등급, 5.2 클래스 스킬 수치
- 코드에 `const`로 박지 않습니다. 기획서가 "모든 수치는 플레이테스트로 조정 전제"라고
  명시했으므로 **플레이 중 인스펙터에서 조정 가능해야** 합니다.
- 에셋 명명은 `SO_<대상>` ([pipeline/asset-pipeline.md](../pipeline/asset-pipeline.md) 3장).

## 9. 남은 결정

첫 구현 계획서를 막는 결정은 모두 해소되었습니다.
남은 항목은 [docs/INDEX.md](../INDEX.md)의 결정 대기 목록을 참조하십시오.
이 문서와 관련된 것은 M1에서 검증할 D-022 · D-023 · D-024 · D-025입니다.

## 변경 이력

| 날짜 | 내용 |
|---|---|
| 2026-09-14 | 최초 작성(뼈대) |
| 2026-09-14 | gdd v0.4 반입. 기술 스택 확정, 레이어 원칙 근거 보강, 풀링·Layer 확정, MVP 구현 순서 추가 |
| 2026-09-14 | ADR-0005 채택. D-007(EditMode 테스트) · D-008(ScriptableObject) 해소 |
| 2026-09-15 | Cinemachine 도입 시점을 M2로 명시. M1은 자체 `ThirdPersonCamera` 로 진행 |
| 2026-09-15 | 2.1 추가 — Mobile URP 에셋이 QualitySettings 참조 때문에 단독 삭제 불가함을 기록 |
