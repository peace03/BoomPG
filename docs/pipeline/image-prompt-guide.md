# 이미지 생성 프롬프트 규칙

> **캐릭터 이미지를 생성할 때 지켜야 할 일반 규칙입니다.**
> 캐릭터별 실제 프롬프트는 [prompts/](prompts/) 아래에 따로 있습니다.
> 이 문서는 "어떻게 쓰는가", 그쪽은 "무엇을 썼는가"입니다.
>
> 대상은 VARCO 3D 워크플로우의 `GenerateImage` 노드(현재 `gpt-image-2-medium`)이며,
> **생성된 이미지는 그대로 3D 생성의 입력이 됩니다.** 이 문서의 규칙 상당수는
> "보기 좋은 그림"이 아니라 **"3D로 잘 변환되는 그림"**을 목표로 합니다.

## 1. 기본 원칙

| 원칙 | 내용 |
|---|---|
| **길이 40~80 단어** | 그보다 길면 모델이 뒷부분 지시를 흘립니다. 중복 형용사를 걷어내 예산을 확보하십시오 |
| **앞이 무겁다** | 앞에 둔 요소에 가중치가 실립니다. **성별 → 체형 → 실루엣 요소** 를 앞에, 배경·양식을 뒤에 둡니다 |
| **형용사보다 수치** | `clearly oversized` 는 해석의 여지가 있지만 `roughly twice the bulk of the bare left arm` 은 없습니다 |
| **버즈워드 금지** | `8k` · `masterpiece` · `highly detailed` · `trending on artstation` 은 **효과가 없습니다.** 구형 모델 시절의 관습이며 단어 예산만 낭비합니다 |
| **반대쪽을 지정한다** | 원하는 것을 쓰고, 이어서 **원하지 않는 것을 배제**합니다 — `not flat painted markings`, `no small greebles` |

## 2. 블록 순서

프롬프트는 항상 아래 6블록을 **이 순서로** 씁니다.

| # | 블록 | 예 |
|---|---|---|
| 1 | **정체** — 성별을 대문자로 | `FEMALE speed-runner game character` |
| 2 | **체형** | `lean athletic build, narrow waist, inverted-triangle torso` |
| 3 | **의상·장비** — 구조를 서술. 실루엣 요소는 별도 문장 | `segmented panel lines, a high stand-up collar, ribbed knee pads` |
| 4 | **머리** | `Hair in one compact short ponytail, a single solid volume, no loose strands` |
| 5 | **`COLOR:`** — 넓은 면적 → 하이라이트 → 전체 명도 방향 | `mostly BLACK ...; vivid RED and YELLOW only as thin stripes` |
| 6 | **촬영·양식·배제** | `Full body front view, T-pose ... No distorted hands` |

## 3. 손과 얼굴

이미지 생성의 고질적 취약부입니다. **품질 형용사를 더하지 말고 두 가지를 하십시오.**

1. **원하는 상태를 서술한다**
   - `empty open hands, five clearly separated fingers`
   - `clean defined facial features, both eyes clearly visible`
2. **실패 양상을 직접 배제한다**
   - `No distorted hands, no malformed fingers, no blurred face`

> 인게임 교전 거리는 20~40 m 라 손가락은 보이지 않습니다(gdd 16장). 그럼에도 잡는 이유는
> **이 이미지가 3D 생성의 입력이기 때문입니다.** 손가락이 뭉친 이미지는 뭉친 메시를 만듭니다.

> **압축 요령** — 손 비움 지시와 손 디테일 지시를 한 구절로 합칩니다.
> `holding nothing, no weapons, no props` (6단어, 한 가지 해결)
> → `empty open hands, five clearly separated fingers` (7단어, 두 가지 해결)

## 4. 3D 생성을 위한 규칙

단일 정면 이미지에서 3D를 만들기 때문에 생기는 제약입니다.

| 규칙 | 이유 |
|---|---|
| **T 포즈, 양팔 같은 높이** | `T-pose, both arms straight out horizontally at equal height`. 팔이 몸에 붙으면 겨드랑이가 물갈퀴처럼 메워집니다. `Generate3D` 의 `tPose=1` 과 짝입니다 |
| **손에 아무것도 들리지 않는다** | 무기를 들면 손과 무기 메시가 엉킵니다. 무기는 별도 생성해 본에 붙입니다 |
| **얇고 긴 돌출 금지** | 흩날리는 머리카락·늘어진 끈·얇은 날개는 복원되지 않고 뭉개집니다. 머리카락은 **하나의 덩어리**로 (D-028) |
| **표면 잡동사니 금지** | `no small greebles, no tiny pipes, no surface clutter`. 작은 요철은 3D 에서 노이즈가 됩니다. 큰 면 몇 장이 낫습니다 |
| **배경은 밝은 회색** | `plain light gray background, no shadows`. 순백은 흰 하이라이트를 먹고, 그림자는 배경 제거를 방해합니다 |
| **전신이 잘리지 않는다** | `centered, whole figure visible head to feet` |

## 5. 실루엣과 명도

[ADR-0006](../decisions/ADR-0006-캐릭터-실루엣-불변.md)이 실루엣을 불변 요소로 고정했으므로,
프롬프트에도 그것이 반영되어야 합니다.

- **실루엣 요소는 전용 문장을 받습니다.** 의상 설명에 섞으면 표면 장식으로 그려집니다.
- **크기는 배수로 지정합니다** — `roughly twice the chest width of a lean character`
- **캐릭터마다 명도 방향을 반대로 못박습니다.** 스파크 `dark in value`, 로카
  `bright and warm in value, clearly lighter than a dark character`.
  둘의 색 계열이 가까워 명도로 갈라야 합니다 ([worldbuilding 4.3](../design/worldbuilding.md)).

## 6. 제출 전 체크리스트

- [ ] 40~80 단어인가 (배경·양식 문구까지 포함해서)
- [ ] 성별이 대문자로 앞쪽에 있는가
- [ ] 실루엣 요소가 전용 문장을 받았고, 크기가 배수로 적혔는가
- [ ] 머리가 "하나의 덩어리"로 지정되었는가
- [ ] `COLOR:` 블록에 **넓은 면적의 색**과 **전체 명도 방향**이 있는가
- [ ] T 포즈 + `at equal height` 가 있는가
- [ ] 손·얼굴 서술과 배제 문구가 모두 있는가
- [ ] 버즈워드(`8k`, `masterpiece` 등)가 없는가

## 출처

외부 자료 조사 (2026-09-16):

- [GPT Image Generation Models Prompting Guide — OpenAI](https://developers.openai.com/cookbook/examples/multimodal/image-gen-models-prompting-guide)
- [GPT Image 1.5 Prompt Guide — fal](https://fal.ai/learn/devs/gpt-image-1-5-prompt-guide)
- [GPT Image 2 Prompt Guide (2026) — PixVerse](https://pixverse.ai/en/blog/gpt-image-2-review-and-prompt-guide)
- [AI Character Pose Prompts (2026) — Neolemon](https://www.neolemon.com/guides/ai-character-pose-prompts/)

## 변경 이력

| 날짜 | 내용 |
|---|---|
| 2026-09-16 | 최초 작성. 외부 자료 조사와 1차 생성 결과(12장)를 반영 |
