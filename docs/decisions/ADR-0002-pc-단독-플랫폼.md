# ADR-0002: 목표 플랫폼을 PC (Windows) 단독으로 한다

- 상태: 채택
- 날짜: 2026-09-14
- 결정자: YunPyeongHwa

## 맥락

현재 `Assets/Settings/` 에는 Unity URP 템플릿이 만든 PC용(`PC_RPAsset`, `PC_Renderer`)과
모바일용(`Mobile_RPAsset`, `Mobile_Renderer`) 렌더 파이프라인 에셋이 **둘 다** 들어 있다.
어느 쪽을 유지할지 정하지 않으면 품질 설정이 두 벌로 갈라지고, 입력 계층도 키보드/마우스와
터치 양쪽을 추상화해야 한다.

## 검토한 선택지

| 선택지 | 장점 | 대가 |
|---|---|---|
| PC (Windows) 단독 | 입력·렌더 설정 단일화. 3인칭 조준에 마우스가 최적. 성능 예산 여유 | 모바일 사용자층 포기 |
| PC + 모바일 | 도달 범위 최대 | 입력 추상화 계층 필요. 렌더 품질 2단계 유지. 터치로 3인칭 조준 UX 설계 부담. 온라인 PvP에서 입력 장치 차이로 인한 형평성 문제 |
| 모바일 단독 | 접근성 | 가상 스틱 조준 정밀도 한계. 로켓 조준 게임에 부적합 |

## 결정

**PC (Windows) 단독으로 한다. 조작은 키보드 + 마우스를 기준으로 설계한다.**

## 이유

로켓 조준과 넉백 회피는 정밀한 에임과 즉각적인 이동 입력을 요구한다. 터치 입력은 이 장르의
조작 요구를 감당하기 어렵고, 온라인 PvP에서 마우스 사용자와 터치 사용자가 섞이면 공정성이 깨진다.
단일 플랫폼으로 좁히면 남는 예산을 렌더 품질과 게임 완성도에 쓸 수 있다.

## 결과

- **열리는 것**: 성능 예산 여유, 단일 입력 경로, 렌더 품질 상향.
- **닫히는 것**: 모바일·콘솔 빌드.
- **새로 생기는 정리 작업**:
  - `Assets/Settings/Mobile_RPAsset.asset`, `Assets/Settings/Mobile_Renderer.asset` 제거
  - `ProjectSettings/QualitySettings.asset`, `GraphicsSettings.asset` 에서 모바일 품질 레벨 참조 정리
  - `Assets/InputSystem_Actions.inputactions` 에서 Touch 스킴 정리
  - 위 작업은 별도 계획서로 진행한다. 이 ADR은 결정만 기록한다.
- **영향받는 문서**: `design/gdd.md`, `tech/architecture.md`, `rules/code-style.md`
- **되돌리는 비용**: 중간. 모바일을 나중에 추가하면 입력 추상화 도입과 렌더 설정 복구가 필요하지만,
  게임 규칙 자체는 영향받지 않는다.
