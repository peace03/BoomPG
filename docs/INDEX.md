# BoomPG 문서 인덱스

> 이 프로젝트의 진실 공급원은 대화가 아니라 이 폴더입니다.
> 새 문서를 만들면 여기에 등록합니다. 등록되지 않은 문서는 없는 문서로 취급합니다.
> 규칙 전문: [rules/documentation.md](rules/documentation.md)

## 목차

### 규칙 — 지켜야 할 것

| 문서 | 내용 |
|---|---|
| [rules/documentation.md](rules/documentation.md) | **문서 규칙.** 무엇을 어디에 적는가, 언제 갱신하는가, 애매하면 어떻게 하는가 |
| [rules/workflow.md](rules/workflow.md) | Claude(기획) · Codex(구현) · 사람(결정·커밋)의 역할과 한 사이클 |
| [rules/code-style.md](rules/code-style.md) | C# · Unity 네이밍, 폴더 구조, 성능·에러 처리·주석 규칙 |

### 설계 — 게임이 무엇인가

| 문서 | 내용 | 상태 |
|---|---|---|
| [design/gdd.md](design/gdd.md) | **게임 디자인 문서 — 게임 내용의 정본.** 수치·모드·맵·마일스톤 전부 | **v0.4** |
| [design/worldbuilding.md](design/worldbuilding.md) | **세계관 · 연출 톤 · 캐릭터 설정의 정본.** 스파크·로카·기즈모 | 반입됨 |
| [design/glossary.md](design/glossary.md) | 용어 사전 (한국어 ↔ 코드 식별자) | 갱신됨 |

> gdd 개정본이 나오면 `docs/design/gdd.md`에 **덮어쓰십시오.** 버전별 파일을 만들지 않습니다.
> gdd 23장은 아키텍트가 덧붙인 정합성 검토입니다 (기획자 작성분이 아님).

### 기술 — 어떻게 만드는가

| 문서 | 내용 | 상태 |
|---|---|---|
| [tech/architecture.md](tech/architecture.md) | 기술 스택, 레이어 원칙, 현재 에셋 현황, MVP 구현 순서 | 확정분 반영 |
| [tech/networking.md](tech/networking.md) | 호스트 권위 경계, 넉백 처리 규약 | 확정분 반영 |
| [pipeline/asset-pipeline.md](pipeline/asset-pipeline.md) | 3D 모델 · 오디오 · VFX 반입 절차, AI 생성 워크플로우 | 확정분 반영 |
| [pipeline/blender-parts-workflow.md](pipeline/blender-parts-workflow.md) | **파츠 반입·보정 공통 워크플로우.** Codex 가 모델 파일 하나만 받아 실행하는 절차서 | 신설 |
| [pipeline/image-prompt-guide.md](pipeline/image-prompt-guide.md) | **이미지 생성 프롬프트 규칙.** 길이 · 블록 순서 · 손과 얼굴 · 3D 생성용 제약 | 신설 |
| [pipeline/prompts/spark.md](pipeline/prompts/spark.md) | 스파크 **7 파츠** 프롬프트 · 설계 의도 · 1차 생성 결과 | v7 |
| [pipeline/prompts/rocca.md](pipeline/prompts/rocca.md) | 로카 프롬프트 전문 · 설계 의도 · 개정 이력 | v3 (**모듈러 미적용**) |
| [pipeline/prompts/gizmo.md](pipeline/prompts/gizmo.md) | 기즈모 **본체 + 건틀릿** 프롬프트 | v4 (**모듈러 미적용**) |

### 결정 기록 (ADR) — 왜 그렇게 정했는가

| 번호 | 결정 | 상태 |
|---|---|---|
| [ADR-0001](decisions/ADR-0001-온라인-멀티-pvp.md) | 플레이 형태를 온라인 멀티 PvP로 한다 | 채택 |
| [ADR-0002](decisions/ADR-0002-pc-단독-플랫폼.md) | 목표 플랫폼을 PC (Windows) 단독으로 한다 | 채택 |
| [ADR-0003](decisions/ADR-0003-낙사형-배틀로얄-승리조건.md) | 승리 조건을 낙사형 배틀로얄로 한다 | 채택 |
| [ADR-0004](decisions/ADR-0004-photon-fusion-2.md) | 네트워크를 Photon Fusion 2 (Host Mode)로 한다 | 채택 |
| [ADR-0005](decisions/ADR-0005-프로젝트-코드-구조.md) | 프로젝트 코드 구조 (폴더 · 어셈블리 · 로깅 · 테스트 · 데이터) | 채택 |
| [ADR-0006](decisions/ADR-0006-캐릭터-실루엣-불변.md) | 캐릭터 실루엣을 스킨 불변 요소로 고정한다 | 채택 |
| [ADR-0007](decisions/ADR-0007-캐릭터-에셋-반입-규격.md) | 캐릭터 에셋 반입 규격 (포맷 · 리깅 · 폴리곤 · 텍스처) | 채택 (**일부 항목 ADR-0008 이 개정**) |
| [ADR-0008](decisions/ADR-0008-모듈러-캐릭터-구조.md) | 캐릭터를 모듈러 구조로 만든다 (베이스 바디 + 의상 + 장비) | 채택 |
| [ADR-template](decisions/ADR-template.md) | 새 ADR을 쓸 때 복사하는 양식 | — |

### 문서 밖 (참고)

| 경로 | 내용 |
|---|---|
| [CLAUDE.md](../CLAUDE.md) | Claude(아키텍트) 지시서 |
| [AGENTS.md](../AGENTS.md) | Codex(구현자) 지시서 |
| `TaskPlan/TASK_PLAN.md` | 현재 진행 중인 작업 지시서 |
| `scripts/` | 가드 훅 · 핸드오프 스크립트 |

---

## 확정 요약

| 항목 | 결정 |
|---|---|
| 장르 · 시점 | 3인칭 로켓 아레나 액션 (넉백 파이터) |
| 플레이 형태 | 온라인 멀티 PvP · Photon Fusion 2 Host Mode |
| 플랫폼 | PC (Windows) 단독 · 키보드+마우스 |
| 기본 모드 | 배틀로얄 솔로 8인 · 목숨 1개 · 최후 1인 생존 |
| 승리 수단 | **낙사 주력** (목표 비율 60~70%) + HP 100 · 직격 3방 보조 |
| 매치 길이 | 5분 15초 (축소 6페이즈) |
| 맵 | 공중 정거장 스카이야드 1종 (160 × 160 m) |
| MVP 범위 | gdd 20장 그대로. **클래스 3종은 후순위** |
| 코드 구조 | `Assets/_Project/` · 도메인별 asmdef · `GameLog` 래퍼 · EditMode 테스트 · ScriptableObject |

---

## 결정 대기 목록

정해지지 않은 것은 **추측으로 메우지 않고 사용자에게 묻습니다.**
결정되면 해당 문서 본문을 고치고 이 목록에서 지웁니다.

> **첫 구현 계획서를 막는 결정은 모두 해소되었습니다.** M1 그레이박스 계획서를 쓸 수 있습니다.

### ⓪ 아트 — 생성 진행 중

VARCO 3D 1·2차 시도가 세 캐릭터 모두 반입 불가 수준으로 나와 구조를 바꿨습니다.
원인 분석은 [asset-pipeline 2.2](pipeline/asset-pipeline.md), 채택한 구조는
[ADR-0008](decisions/ADR-0008-모듈러-캐릭터-구조.md)에 있습니다.

**확정된 것**

- **D-027 (2026-09-16)** — **모듈러 파츠 분할을 채택합니다.** 파츠마다 이미지와 3D 를
  따로 만들고 블렌더/Unity 에서 조립합니다. 베이스 바디(리깅 대상) + 의상 + 장비의
  3 레이어 구조이며, 근거는 [ADR-0008](decisions/ADR-0008-모듈러-캐릭터-구조.md)입니다.
- **D-028 (2026-09-16)** — 머리를 **헬멧으로 덮지 않습니다.** 머리카락은 유지하되
  흩날리는 결 대신 **덩어리로 묶인 형태**로 설계합니다.

**남은 것**

| ID | 내용 | 문서 |
|---|---|---|
| **D-029** | **건틀릿이 빠진 기즈모 본체가 스파크와 실루엣으로 구분되는가** — 둘 다 날씬한 여성형이 된다. 실루엣 테스트에서 확인 필요 | [prompts/gizmo.md](pipeline/prompts/gizmo.md) · [asset-pipeline 4.7](pipeline/asset-pipeline.md) |
| **D-030** | **8인 × 최대 7 파츠 = 56 드로우콜이 GTX 1050 에서 60fps 를 지키는가** — 모듈러 구조의 대가. M2 프레임 측정의 1순위 항목 | [ADR-0008](decisions/ADR-0008-모듈러-캐릭터-구조.md) |

### ① 밸런스 — **현재 기본값 유지, 아트 적용 후 재조정** (2026-09-16 결정)

M1 그레이박스 플레이 결과 현재 수치가 쓸 만한 것으로 확인되었습니다. 회색 큐브 위에서는
거리감과 속도감이 실제와 다르므로, **아트가 들어간 뒤에 한 번에 재조정**합니다.
설정은 `SO_*.asset` 으로 빠져 있어 플레이 중 조정이 가능합니다 (인스펙터 툴팁 참조).

| ID | 내용 | 현재 값 | 문서 |
|---|---|---|---|
| D-022 | 넉백 합성 모드 최종 선택 | **`AdditiveDamped`** + 상한 25 m/s | [gdd 23.2](design/gdd.md) · [networking](tech/networking.md) |
| D-013 | 직격 데미지 35 vs 40 | 35 | [gdd 22](design/gdd.md) |
| D-024 | 로켓 점프 도달 높이 실측 (문서값 6.5 m와 계산 불일치) | 미측정 | [gdd 23.2](design/gdd.md) |
| D-025 | 제트팩 복귀 여유 (수평 16 m vs 플랫폼 간격 15 m) | 미측정 | [gdd 23.2](design/gdd.md) |
| D-023 | MVP "축소 3페이즈" 스케줄 재산정 | 미착수 (M2) | [gdd 23.2](design/gdd.md) |
| D-020 | 관제탑(+18 m)이 하이리스크로 느껴지는지 | 미착수 (M3 아트) | [gdd 22](design/gdd.md) |

> **2026-09-16 플레이 중 조정된 값** (아직 확정 아님, 아트 적용 후 재검토):
> 폭발 반경 4.5 → **6 m**, 로켓 중력 스케일 0.35 → **0.5**,
> 넉백 최소 상향 0.35 → **0.25**, 자해 비율 0.5 → **0.234**,
> 넉백 합성 모드 `Additive` → **`AdditiveDamped`**.
> **폭발 반경 6 m 는 gdd 본문(4.5 m)과 어긋납니다** — 확정되면 gdd 를 고쳐야 합니다.

### ② M2 전 · M2 중

| ID | 내용 | 문서 |
|---|---|---|
| **D-021** | **Fusion 2 라이선스 비용 · 무료 티어 CCU 한도.** 비용 확인 전에는 M2를 시작하지 않는다 | [networking](tech/networking.md) |
| D-014 | 모드 B 인원 상한선 (Host Mode 실측) | [networking](tech/networking.md) |

### ③ M3 이후

| ID | 내용 | 문서 |
|---|---|---|
| D-015 | 모드 B 인원수별 초기 반경 스케일링 공식 | [networking](tech/networking.md) |
| D-016 | 캐릭터 **확장** 여부 — 현재 3종(클래스 1:1)은 확정, 늘릴지는 M3 이후 판단 | [worldbuilding 4.3](design/worldbuilding.md) |
| D-017 | 갈고리 조준 시 적/아군/다운 우선순위 규칙 | [gdd 22](design/gdd.md) |
| D-018 | 모드 C 관제탑 점령 점수 배분 및 목표 점수 | [gdd 22](design/gdd.md) |
| D-019 | 강화 칩 리스폰 주기 60초 적절성 | [gdd 22](design/gdd.md) |

### 해소됨

| ID | 결정 | 근거 |
|---|---|---|
| D-004 | 네트워크 솔루션 — Photon Fusion 2 | ADR-0004 |
| D-005 | 서버 형태 — Host Mode (호스트 권위) | ADR-0004 |
| D-006 | 매치 인원 — 모드 A 8인 | ADR-0003 |
| D-009 | 승리 조건 — 낙사형 배틀로얄 | ADR-0003 |
| D-010 | 다양성 범위 — RPG 1종 + 클래스 3종(스킬만 차이, MVP 후순위) | ADR-0003 |
| D-011 | 매치 길이 — 5분 15초 | ADR-0003 |
| D-001 | 폴더 구조 — `Assets/_Project/` 신설, 템플릿 폴더 이동·삭제 | ADR-0005 |
| D-002 | 어셈블리 — 도메인별 asmdef 분리 (참조 방향 강제) | ADR-0005 |
| D-003 | 로깅 — `GameLog` 래퍼 도입, `Debug.Log` 직접 호출 금지 | ADR-0005 |
| D-007 | 테스트 — EditMode만 도입, PlayMode 없음 | ADR-0005 |
| D-008 | 데이터 — ScriptableObject (`Assets/_Project/Data/`) | ADR-0005 |
| D-026 | 캐릭터 실루엣 — 스킨 불변 요소로 고정, 반입 전 실루엣 테스트 필수 | ADR-0006 |
| D-012 | 에셋 반입 규격 — FBX · Humanoid · 8,000 tris · 2048 텍스처 | ADR-0007 |

---

## 확인이 필요한 외부 사실

| 내용 | 상태 |
|---|---|
| **Rocket Arena (EA, 2020)** 선례 — gdd 1장이 유사 선례로 언급하면서 "검색 없이 작성했으므로 직접 확인하라"고 명시. 차별점 정리와 포트폴리오 발표 대비에 필요 | 미확인 |
| gdd 17.1의 "Unity 6 LTS" 표기 vs 실제 프로젝트 버전 `6000.6.0f1` | 미확인 |

---

## 계획서 이력

완료된 계획서는 `TaskPlan/`에 날짜를 붙여 보관합니다.

| 날짜 | 작업 | 파일 | 상태 |
|---|---|---|---|
| 2026-09-14 | 가드 훅 스크립트 보정 (인코딩 · 오탐 · 자기유지) | `TaskPlan/2026-09-14-가드-훅-보정.md` | 완료 |
| 2026-09-15 | M1 그레이박스 프로토타입 (넉백 코어 검증) | `TaskPlan/2026-09-15-M1-그레이박스-프로토타입.md` | 구현 완료 · **플레이 검증 대기** |
| 2026-09-16 | 설정 에셋 인스펙터 가독성 개선 (Header · Tooltip · Range) | `TaskPlan/2026-09-16-설정에셋-인스펙터-개선.md` | 완료 |
| 2026-09-16 | 마우스 커서 잠금 (테스트 편의) | `TaskPlan/2026-09-16-마우스-커서-잠금.md` | 완료 |
| 2026-09-16 | 넉백 중 좌우 공중 제어 | `TaskPlan/TASK_PLAN.md` | **진행 중** |

## 변경 이력

| 날짜 | 내용 |
|---|---|
| 2026-09-14 | 최초 작성. 문서 체계 수립, ADR-0001 · ADR-0002 등록, 미결정 D-001 ~ D-012 등록 |
| 2026-09-14 | gdd v0.4 반입. ADR-0003 · ADR-0004 등록, D-004~D-006 · D-009~D-011 해소, D-013 ~ D-025 신규 등록 |
| 2026-09-14 | ADR-0005 등록. D-001 · D-002 · D-003 · D-007 · D-008 해소 — 첫 구현 계획서 착수 가능 |
| 2026-09-15 | D-022 부분 확정. M1 그레이박스 계획서 작성, 가드 훅 계획서 보관 처리 |
| 2026-09-16 | M1 플레이 결과 반영 — 밸런스 수치는 현재값 유지, 재조정을 아트 적용 후로 미룸 |
| 2026-09-16 | 세계관·캐릭터 기획서 반입. D-016 기준선 확정(캐릭터 3종), D-026 신규 등록 |
| 2026-09-16 | ADR-0006 등록. D-026 해소 — 실루엣 불변 + 실루엣 테스트를 반입 절차에 편입 |
| 2026-09-16 | ADR-0007 등록. D-012 해소 — 캐릭터 에셋 반입 규격 확정, 아트 착수 가능 |
| 2026-09-16 | VARCO 2차 실패 분석을 asset-pipeline 2.2 에 기록. D-027(파츠 분할) · D-028(머리 커버링) 신규 등록 |
| 2026-09-16 | 프롬프트 문서 4종 신설(가이드 1 + 캐릭터 3). D-027 부분 채택(기즈모 건틀릿 분리), D-029 신규 등록 |
| 2026-09-16 | **ADR-0008 등록 — 모듈러 캐릭터 구조 채택.** 스킨 시스템 대비. ADR-0007 의 머티리얼·리깅 항목 개정, D-030 신규 등록 |
| 2026-09-17 | 3D 생성 도구 이름을 `BARCO AI` → **`VARCO 3D`** 로 정정. ADR-0006 · ADR-0007 · worldbuilding · asset-pipeline · rules 2종 · README 총 17곳 |
| 2026-09-17 | `pipeline/blender-parts-workflow.md` 신설 — VARCO 3D 파츠를 Blender 에서 보정·정리하는 Codex 실행용 절차서 |
| 2026-09-17 | 스파크 부스터를 어깨·무릎 → **허리춤·종아리**로 변경. worldbuilding · asset-pipeline · prompts/spark(v8) · VARCO 노드 3개 반영 |
| 2026-09-17 | **제트팩을 캐릭터 외형에 편입** (worldbuilding 3.0 신설) — gdd 기본 장비인데 외형 묘사에 없던 공백. ADR-0008 에 공용 파츠 절 추가, 허리춤 부스터 폐기, prompts/spark v9 |
