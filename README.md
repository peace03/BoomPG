# BoomPG
3인칭 로켓 아레나 액션 (넉백 파이터) 게임<br><br>
Claude Code(기획), Codex(구현), BARCO AI(3D 모델 생성) 워크플로우를 만들어 생산성 및 효율성을 높인 프로젝트이다.<br>
>워크플로우 구현 방법: https://astonishing-breeze-0eb.notion.site/Claude-Code-Codex-WorkFlow-3d7b577343d880868ce6e9ec95777f3a?source=copy_link

## 문서

이 프로젝트의 규칙·설계·결정은 전부 [docs/INDEX.md](docs/INDEX.md)에 정리되어 있다.
문서에 적히지 않은 결정은 존재하지 않는 결정으로 취급한다.

| | |
|---|---|
| [문서 규칙](docs/rules/documentation.md) | 무엇을 어디에 적는가, 언제 갱신하는가 |
| [작업 흐름](docs/rules/workflow.md) | Claude · Codex · 사람의 역할 분담 |
| [코드 컨벤션](docs/rules/code-style.md) | C# · Unity 네이밍과 폴더 구조 |
| [게임 디자인](docs/design/gdd.md) | 게임이 무엇인가 |
| [결정 기록 (ADR)](docs/INDEX.md#결정-기록-adr--왜-그렇게-정했는가) | 왜 그렇게 정했는가 |

### 확정된 축

- 플레이 형태: 온라인 멀티 PvP ([ADR-0001](docs/decisions/ADR-0001-온라인-멀티-pvp.md))
- 플랫폼: PC (Windows) 단독 ([ADR-0002](docs/decisions/ADR-0002-pc-단독-플랫폼.md))
- 승리 조건: 낙사형 배틀로얄 — 솔로 8인, 최후 1인 생존 ([ADR-0003](docs/decisions/ADR-0003-낙사형-배틀로얄-승리조건.md))
- 네트워크: Photon Fusion 2 Host Mode ([ADR-0004](docs/decisions/ADR-0004-photon-fusion-2.md))
- 엔진: Unity 6000.6.0f1 · URP
