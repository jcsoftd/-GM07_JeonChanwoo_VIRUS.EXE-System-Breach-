# VIRUS.EXE : System Breach

> 바이러스가 되어 시스템 내부를 침투하는 콘솔 해킹 로그라이크 RPG

---
🎬 플레이 영상
https://youtu.be/VfTsmrDXx24
---
## 📌 프로젝트 소개

**VIRUS.EXE : System Breach**는 C# 콘솔 환경에서 제작한 턴제 로그라이크 RPG입니다.

플레이어는 `VIRUS.EXE`라는 악성 실행 파일이 되어 보안망이 구축된 시스템 내부를 침투하고,  
보안 프로세스와 방화벽을 격파하며 최종적으로 시스템의 핵심인 `KERNEL_CORE`를 장악하는 것이 목표입니다.

이 프로젝트는 콘솔의 그래픽적 한계를 단점으로 보지 않고,  
**콘솔 화면 자체를 해킹 터미널처럼 활용**하는 방향으로 기획되었습니다.  
명령어, 로그, 경고 메시지, 글리치 효과, ASCII 아트 같은 요소들이 콘솔 환경과 자연스럽게 어울립니다.

---

## 🌐 세계관

플레이어는 대상 시스템에 침투한 `VIRUS.EXE`입니다.  
시스템 내부의 `SIGNAL GRID`를 따라 노드를 이동하며 보안 프로세스를 제거하고,  
최종적으로 `KERNEL_CORE`에 접근해 시스템을 장악합니다.

기존 RPG 요소는 콘셉트에 맞게 전부 재해석되었습니다.

| 일반 RPG | VIRUS.EXE |
|---------|-----------|
| HP | STABILITY |
| MP | ENERGY |
| 돈 | KB / MB |
| 맵 | SIGNAL GRID |
| 일반 몬스터 | SECURITY PROCESS |
| 엘리트 몬스터 | FIREWALL GATEWAY |
| 보스 | KERNEL_CORE |
| 전직 | PAYLOAD MUTATION |
| 상점 | EXPLOIT MARKET |
| 강화소 | PAYLOAD LAB |

---

## 🎮 핵심 시스템

### 🗺️ SIGNAL GRID (맵 시스템)

13열 × 3행 노드 구조의 진행 맵입니다.  
전체 맵을 한 번에 보여주지 않고, 현재 위치를 기준으로 4열씩 화면에 표시됩니다.  
플레이어가 오른쪽으로 진행할수록 화면도 함께 스크롤되어, 시스템 내부를 점점 깊게 침투하는 느낌을 줍니다.

```
1열       : /Root  시작 침투 지점
2~4열     : 1구역
5열       : /Fw    방화벽 게이트
6~8열     : 2구역
9열       : /Fw    방화벽 게이트
10~12열   : 3구역
13열      : Kernel 최종 보스 노드
```

노드 배치는 완전 랜덤이 아닌 **규칙 기반 랜덤**으로, 매번 배치는 달라지지만  
전투 / 상점 / 강화 / 이벤트 / 보상 노드는 반드시 등장합니다.

| 노드 | 설명 |
|------|------|
| `/Root` | 시작 침투 지점 |
| `/Sec` | 일반 보안 프로세스 전투 |
| `/Fw` | 방화벽 게이트 / 엘리트 전투 |
| `/Mkt` | EXPLOIT MARKET / 상점 |
| `/Lab` | PAYLOAD LAB / 강화 |
| `/Tmp` | 랜덤 이벤트 |
| `/Data` | 데이터 캐시 / 회복 및 보상 |
| `Kernel` | 최종 보스 |

### ⚔️ 전투 시스템

플레이어가 먼저 행동을 선택하고 몬스터가 반격하는 턴제 전투입니다.

전투 자원은 **STABILITY(체력)**, **ENERGY(스킬 자원)**, **KB(재화)** 세 가지입니다.

변이 전에는 기본 스킬을, 변이 후에는 변이 종류에 따라 전용 스킬을 사용합니다.

적 구성은 다음과 같습니다.
- 일반 몬스터 7종 (`SCAN_DAEMON`, `LOGIC_BOMB`, `NULL_POINTER_VOID` 등)
- 엘리트 몬스터 3종 (`PROXY_SINGULARITY`, `SYN_FLOOD_GATE`, `IC_CRYPTO_GATE`)
- 최종 보스 1종 (`KERNEL_CORE` / 3페이즈)

### 🧬 PAYLOAD MUTATION (변이 시스템)

일정 레벨에 도달하면 바이러스의 침투 방식 자체가 바뀌는 **전직 시스템**입니다.  
변이 후에는 플레이어의 이름, 능력치, 스킬셋, 전투 스타일이 달라집니다.

| 변이 | 스타일 | 주요 스킬 |
|------|--------|-----------|
| **RANSOMWARE** | 방어형 / 생존 | `ENCRYPT` (암호화 + 적 약화), `RANSOM_NOTE` (KB 강탈 + 회복) |
| **TROJAN** | 공격형 / 암살자 | `BACKDOOR` (고치명타 공격), `SPOOF_AUTH` (다음 공격 강화) |
| **ADWARE** | 디버프형 / 상태이상 | `POPUP_FLOOD` (팝업 오염 + 적 약화), `AD_NOTIFICATION` (지속 피해 중첩) |

변이 시에는 각 바이러스 종류에 맞는 코드 컴파일 연출이 출력됩니다.  
(Ransomware → PowerShell 느낌 / Trojan → C/C++ 느낌 / Adware → JavaScript 느낌)

### 📊 TRACE / ACCESS 시스템

| 수치 | 설명 |
|------|------|
| **TRACE LEVEL** | 시스템 추적도. 노드 클리어 시 증가하며, 높을수록 적이 강해집니다. |
| **ACCESS LEVEL** | 보스 노드 진입 권한. `/Sec` +1, `/Fw` +2. 10 이상이어야 Kernel 진입 가능합니다. |

### 🏪 EXPLOIT MARKET (상점)

KB를 사용해 아이템을 구매하거나 보유 아이템을 판매할 수 있습니다.  
진행 구간과 변이 상태에 따라 상점 품목이 달라지며, 변이 후에는 전용 장비도 등장합니다.

### 🔧 MUTATION LAB (강화)

전용 재료 아이템을 사용해 능력치를 영구 강화할 수 있습니다.

| 강화 항목 | 재료 | 효과 |
|-----------|------|------|
| ATK 강화 | `MEMORY_SHARD` × 3 | ATK +2 |
| HEALTH 강화 | `CORE_FRAGMENT` × 3 | Max HEALTH +20 |
| ENERGY 강화 | `ENERGY_CORE` × 2 | Max ENERGY +10 |

### 🎲 랜덤 이벤트 / 미니게임

`/Tmp` 노드에서 7종의 랜덤 이벤트가 발생합니다.  
이벤트에 따라 선택지와 리스크가 다르며, 일부는 미니게임과 연결됩니다.

**구현된 미니게임 3종:**
- `SECURITY CODE BREACH` — 숫자야구 방식의 보안 코드 해독
- `SIGNAL SYNC` — 움직이는 마커를 타이밍에 맞춰 입력하는 신호 동기화
- `SUSPICIOUS CACHE PURGE` — 떨어지는 파일을 삭제빔으로 처리하는 콘솔 슈팅

---

## 🗂️ 프로젝트 구조

```
VirusExe.SystemBreach/
├── Program.cs
├── Core/
│   ├── GameConfig.cs           # 전역 설정 상수
│   ├── InputHelper.cs          # 키 입력 처리
│   └── TextUtil.cs             # 문자열 폭 계산 / 정렬 보정
├── Characters/
│   ├── CombatEntity.cs         # 전투 대상 추상 클래스
│   ├── Player.cs               # 플레이어 상태 및 행동
│   ├── Enemy.cs                # 적 데이터 및 상태이상
│   └── VirusMutation.cs        # 변이 종류 enum
├── Game/
│   ├── GameManager.cs          # 게임 전체 흐름 제어
│   └── GameState.cs            # 게임 상태 enum
├── DataGrid/
│   ├── SignalGrid.cs           # 맵 생성 및 이동 처리
│   ├── GridNode.cs             # 노드 데이터
│   └── NodeType.cs             # 노드 타입 enum
├── Systems/
│   ├── BattleManager.cs        # 전투 진행 로직
│   ├── EnemyFactory.cs         # 적 생성 팩토리 (셔플백 방식)
│   ├── MiniGameManager.cs      # 미니게임 3종
│   ├── RandomEventManager.cs   # 랜덤 이벤트 7종
│   ├── ShopManager.cs          # 상점 로직
│   ├── UpgradeManager.cs       # 강화 로직
│   ├── RewardManager.cs        # 보상 처리
│   ├── PayloadMutationManager.cs # 변이 선택 처리
│   ├── SkillBalanceData.cs     # 스킬 수치 관리
│   └── Items/                  # 아이템 데이터 / 풀 관리
└── Rendering/
    ├── ConsoleRenderer.cs
    ├── ConsoleRenderer.Battle.cs
    ├── ConsoleRenderer.Grid.cs
    ├── ConsoleRenderer.Inventory.cs
    ├── ConsoleRenderer.Shop.cs
    ├── ConsoleRenderer.Mutation.cs
    ├── ConsoleRenderer.EnemyArt.Boss.cs
    ├── ConsoleRenderer.Cutscene.cs
    ├── ConsoleRenderer.Ending.cs
    └── ...
```

---

## 🛠️ 개발 환경

| 항목 | 내용 |
|------|------|
| 언어 | C# (.NET 8.0) |
| 실행 환경 | Windows 콘솔 (Console Application) |
| IDE | Visual Studio 2022 |
| 콘솔 크기 | 110 × 47 문자 / 920 × 830 px |

---


## 💡 플레이 팁

- **TRACE LEVEL**이 높을수록 적이 강해집니다. 불필요한 전투는 신중히 선택하세요.
- **ACCESS LEVEL 10** 이상이어야 `Kernel` 노드에 진입할 수 있습니다.
- `/Data` 노드는 TRACE가 오르지 않는 안전한 회복 지점입니다. 루트 계획이 중요합니다.
- 변이(PAYLOAD MUTATION)는 전투 스타일을 완전히 바꿉니다. 플레이 스타일에 맞게 선택하세요.
- `SCAN_PULSE` 아이템으로 전방 2열의 노드를 미리 확인할 수 있습니다.

---

## 👤 개발자

 이름  전찬우
 기수  GM07
