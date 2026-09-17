# 로카 (Rocca) — 생성 프롬프트

> 작성 규칙은 [image-prompt-guide.md](../image-prompt-guide.md).
> 캐릭터 설정의 정본은 [worldbuilding 3.2](../../design/worldbuilding.md).
> **프롬프트를 고치면 아래 개정 이력에 무엇을 왜 바꿨는지 한 줄 남기십시오.**

## 현재 프롬프트 (v3)

```
MALE bomber-pilot game character, burly build, very broad shoulders, bulky chest. Oversized bomber jacket with a thick collar, a heavy ammo belt crossing the chest diagonally. A SMALL compact launcher tube strapped ON TOP OF THE RIGHT FOREARM, running along the outer upper surface, about as long as the forearm and no wider than the wrist, muzzle pointing forward. Short cropped hair, one solid volume. COLOR: HOT ORANGE and deep RED jacket and trousers over large areas; dark brown straps, belt and boots for contrast; overall bright and warm in value. Clean defined facial features, both eyes clearly visible. Empty open hands, five clearly separated fingers. Full body front view, T-pose, both arms straight out horizontally at equal height. Cartoon 3D game art, clean readable silhouette, plain light gray background, no shadows, whole figure visible head to feet. No distorted hands, no malformed fingers, no blurred face.
```

## 설계 의도

| 구절 | 왜 있는가 |
|---|---|
| `very broad shoulders, bulky chest` | **로카의 실루엣 핵심.** 세 캐릭터 중 유일하게 부피로 읽힙니다 |
| `a heavy ammo belt crossing the chest diagonally` | ADR-0006 불변 요소. 대각선이라 정면 실루엣에서도 읽힙니다 |
| `ON TOP OF THE RIGHT FOREARM, running along the outer upper surface` | **장착 위치를 면으로 특정합니다** (2026-09-16 결정). 아래 참조 |
| `about as long as the forearm and no wider than the wrist` | **크기를 신체 부위로 재서 지정합니다.** 형용사보다 강합니다 |
| `Short cropped hair, one solid volume` | D-028 |
| `overall bright and warm in value` | 스파크의 `dark in value` 와 **반대로 못박습니다.** 색 계열이 가까우므로 명도로 갈라야 합니다 |

## 주의 — 런처 크기

1차 생성에서 팔목 모듈이 **팔뚝보다 굵고 길게** 나와 무기라기보다 장갑처럼 보였습니다.
기즈모의 건틀릿과 실루엣이 헷갈릴 위험이 있었습니다.

그래서 v3 에서 크기를 **신체 치수로 묶었습니다** — `about as long as the forearm and
no wider than the wrist`. "작게"라고 쓰면 모델마다 해석이 다르지만, 팔뚝 길이와 손목 너비는
같은 그림 안에 있는 기준이라 흔들리지 않습니다.

> 이것은 gdd 의 **예비탄(`SpareShot`)** 스킬을 형상화한 것입니다 — 메인 로켓이 빗나갔을 때
> 즉시 쏘는 보조 발사기이므로, 주무기 RPG 보다 작아야 설정과 맞습니다.

## 개정 이력

| 버전 | 날짜 | 변경 |
|---|---|---|
| v1 | 2026-09-16 | 최초. 산문형 서술 |
| v2 | 2026-09-16 | 성별 대문자, `COLOR SCHEME:` 블록 분리, 어깨 폭에 정량 비교 추가, 밝은 회색 배경 |
| v3 | 2026-09-16 | 길이 압축. 손·얼굴 지시와 배제 문구 추가. **런처를 전완 상면에 고정하고 크기를 신체 치수로 제한** |

## 다음에 시도할 것

- [ ] 런처가 전완 **위쪽**에 붙는지 확인 (아래나 옆에 붙으면 실패)
- [ ] 기즈모 건틀릿과 실루엣이 혼동되지 않는지 확인
