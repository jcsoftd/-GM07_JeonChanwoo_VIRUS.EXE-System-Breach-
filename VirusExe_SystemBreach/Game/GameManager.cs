using System;
using VirusExe.SystemBreach.Characters;
using VirusExe.SystemBreach.Core;
using VirusExe.SystemBreach.DataGrid;
using VirusExe.SystemBreach.Rendering;
using VirusExe.SystemBreach.Systems;

namespace VirusExe.SystemBreach.Game
{
    // 게임 전체 진행 관리자
    // 타이틀 이후 GRID, 전투, 상점, 이벤트, 보스까지 큰 흐름을 연결
    public class GameManager
    {
        private readonly ConsoleRenderer renderer; // 콘솔 화면 출력과 연출
        private readonly Player player; // 플레이어 정보
        private readonly SignalGrid grid; // SIGNAL GRID
        private readonly EnemyFactory enemyFactory; // 적 생성
        private readonly RewardManager rewardManager; // 보상
        private readonly BattleManager battleManager; // 전투
        private readonly ShopManager shopManager; // 상점
        private readonly UpgradeManager upgradeManager; // 강화
        private readonly MiniGameManager miniGameManager; // 미니게임
        private readonly RandomEventManager randomEventManager; // 랜덤 이벤트
        private readonly PayloadMutationManager payloadMutationManager; // 페이로드 변이

        private GameState currentState; // 현재 게임 상태 저장
        private int systemInfection; // TRACE LEVEL
        private int moveCount; // 이동 횟수
        private bool isRunning; // 게임 루프 상태
        private bool bossDefeated; // 보스 처치 여부

        public GameManager()
        {
            renderer = new ConsoleRenderer(); // 출력 렌더러
            player = new Player(); // 플레이어
            grid = new SignalGrid(); // SIGNAL GRID
            enemyFactory = new EnemyFactory(); // 적
            rewardManager = new RewardManager(); // 보상 매니저
            battleManager = new BattleManager(renderer, rewardManager); // 전투 매니저
            shopManager = new ShopManager(renderer); // 상점 매니저
            upgradeManager = new UpgradeManager(renderer); // 강화 매니저
            miniGameManager = new MiniGameManager(renderer); // 미니게임 매니저
            randomEventManager = new RandomEventManager(renderer, miniGameManager, rewardManager); // 랜덤 이벤트 매니저
            payloadMutationManager = new PayloadMutationManager(renderer); // 페이로드 변이 매니저

            currentState = GameState.Grid; // GRID 입력 상태로 시작
            systemInfection = 15; // 시작 TRACE LEVEL 설정
            moveCount = 0; // 이동 횟수 초기화
            isRunning = true; // 게임 루프 시작
            bossDefeated = false; // 보스 상태 초기화
        }

        public void Run()
        {
            TitleMenuManager titleMenuManager = new TitleMenuManager(renderer); // 타이틀 메뉴 생성
            bool startGame = titleMenuManager.Open(); // 시작 화면 실행

            if (!startGame) // 게임 시작 취소 체크
                return; // 프로그램 종료

            renderer.PlayFullScreenGlitchTransition(); // 침투 연출 사이 전체 화면 글리치
            renderer.ShowBootSequence(); // 페이로드 투입 연출 출력
            renderer.RenderGrid(grid, player, systemInfection); // 첫 GRID 배경 출력
            renderer.ShowGridEntryIntroFlow(); // 첫 진입 안내 모달 출력

            while (isRunning && player.IsAlive && !bossDefeated) // 게임 진행 조건 체크
            {
                if (currentState == GameState.Grid) // GRID 상태 입력 체크
                {
                    renderer.RenderGrid(grid, player, systemInfection); // 필드 화면 출력

                    ConsoleKey key = InputHelper.ReadKey(); // 키 입력 받기

                    HandleGridInput(key); // GRID 입력 처리
                }
                else // 비동기 상태 꼬임 방지
                {
                    ReturnToGrid(); // GRID 상태 복귀
                }
            }

            if (bossDefeated) // 보스 처치 종료 체크
            {
                currentState = GameState.Ending; // 엔딩 상태 설정
                ShowEnding();
            }
            else if (!player.IsAlive) // 플레이어 사망 종료 체크
            {
                currentState = GameState.GameOver; // 게임오버 상태 설정
                ShowGameOver();
            }
        }

        private void HandleGridInput(ConsoleKey key)
        {
            if (currentState != GameState.Grid) // GRID 상태가 아닌 입력 차단
                return;

            if (key == ConsoleKey.F9) // 보스전 테스트 입력 체크
                StartDebugBossBattleFromGrid(); // 보스전 테스트 진입
            else if (key == ConsoleKey.W || key == ConsoleKey.UpArrow) // 위 이동 입력 체크
                MovePlayer(0, -1);
            else if (key == ConsoleKey.S || key == ConsoleKey.DownArrow) // 아래 이동 입력 체크
                MovePlayer(0, 1);
            else if (key == ConsoleKey.A || key == ConsoleKey.LeftArrow) // 왼쪽 이동 입력 체크
                MovePlayer(-1, 0);
            else if (key == ConsoleKey.D || key == ConsoleKey.RightArrow) // 오른쪽 이동 입력 체크
                MovePlayer(1, 0);
            else if (key == ConsoleKey.E) // 현재 노드 접속 입력 체크
                InteractCurrentNode();
            else if (key == ConsoleKey.I) // 인벤토리 입력 체크
            {
                currentState = GameState.Inventory; // 인벤토리 상태 진입
                renderer.ShowInventory(player);
                ReturnToGrid(); // GRID 복귀
            }
            else if (key == ConsoleKey.C) // 상태창 입력 체크
            {
                currentState = GameState.Status; // 상태창 상태 진입
                renderer.ShowStatus(player);
                ReturnToGrid(); // GRID 복귀
            }
            else if (key == ConsoleKey.X) // 스캔 입력 체크
                UseScanPulse();
            else if (key == ConsoleKey.H) // 도움말 입력 체크
            {
                renderer.ShowGridManualModal(); // 공용 도움말 모달 호출
                ReturnToGrid(); // GRID 복귀
            }
            else if (key == ConsoleKey.Q) // 종료 입력 체크
            {
                bool exitGame = renderer.ShowGridExitConfirmModal(); // 종료 체크 모달

                if (exitGame) // 종료 확정 체크
                {
                    currentState = GameState.Exit; // 종료 상태 설정
                    renderer.PlayTitleTerminateSequence(); // 기존 종료 연출 실행
                    isRunning = false; // 게임 루프 종료
                }
                else
                    currentState = GameState.Grid; // 취소 시 GRID 유지
            }
        }

        private void MovePlayer(int dx, int dy)
        {
            bool moved = grid.Move(dx, dy); // SIGNAL GRID 이동 처리

            if (moved)
                moveCount++; // 이동 횟수기록
        }

        private void InteractCurrentNode()
        {
            GridNode node = grid.CurrentNode(); // 현재 노드 가져오기

            if (node.IsCleared && !node.IsReusableNode()) // 이미 클리어한 일반 노드 체크
                return;

            if (node.Type == NodeType.Start) // 시작 노드 체크
                return;

            EnterNode(node); // 노드 진입 처리
        }

        private void EnterNode(GridNode node)
        {
            int traceIncrease = GameConfig.GetTraceIncreaseByNodeType(node.Type); // 노드 타입별 TRACE 증가량

            currentState = GameState.TerminalSequence; // 터미널 연출 
            renderer.PlayNodeHackSequence(node, traceIncrease); // 노드 해킹 연출

            if (node.Type == NodeType.Security) // 일반 몬스터
            {
                currentState = GameState.Combat; // 전투
                RunSecurityBattle(node, false);
            }
            else if (node.Type == NodeType.Firewall) // 방화벽
            {
                currentState = GameState.Combat; // 전투
                RunSecurityBattle(node, true);
            }
            else if (node.Type == NodeType.Shop) // 상점
            {
                currentState = GameState.Shop; // 상점 
                renderer.RenderGrid(grid, player, systemInfection); // 모달 배경 GRID 출력
                shopManager.Open(player, grid.PlayerX);
                CompleteCurrentNode(); // 노드 클리어 및 TRACE 증가
            }
            else if (node.Type == NodeType.Mutation) // 강화 노드
            {
                currentState = GameState.Mutation; // 강화 진입
                renderer.RenderGrid(grid, player, systemInfection); // 모달 배경 GRID 출력
                upgradeManager.Open(player);
                CompleteCurrentNode(); // 노드 클리어 및 TRACE 증가
            }
            else if (node.Type == NodeType.Event) // 이벤트 노드
            {
                currentState = GameState.Event; // 이벤트진입
                RunRandomEvent(node);
            }
            else if (node.Type == NodeType.DataCache) // 캐시 노드
            {
                currentState = GameState.DataCache; // 캐시 진입
                RunDataCacheNode(node);
            }
            else if (node.Type == NodeType.Boss) // 보스 노드
            {
                currentState = GameState.Boss; // 보스 진입
                RunBossNode();
            }

            if (isRunning && player.IsAlive && !bossDefeated) // 진행 가능 상태
            {
                ReturnToGrid(); // GRID 복귀
            }
        }

        private void RunSecurityBattle(GridNode node, bool elite)
        {
            Enemy enemy = elite ? enemyFactory.CreateFirewall(systemInfection) : enemyFactory.CreateSecurityProcess(systemInfection); // 전투 대상 생성

            string resultMessage; // 전투 결과 메시지
            RewardResult rewardResult; // 전투 보상 결과

            bool win = battleManager.StartBattle(player, enemy, systemInfection, node.Type, grid.PlayerX, out resultMessage, out rewardResult); // 전투 실행

            if (win) // 승리
            {
                int accessGain = elite ? 2 : 1; // ACCESS 증가량
                int traceGain = CompleteCurrentNode(); // 노드 클리어 및 TRACE 증가

                player.AddAccessLevel(accessGain); // ACCESS LEVEL 지급

                renderer.ShowBattleClearResult(player, enemy, rewardResult, accessGain, traceGain); // 전투 종료 결과 화면
            }
        }

        private void RunRandomEvent(GridNode node)
        {
            renderer.RenderGrid(grid, player, systemInfection); // 모달 배경 GRID 출력
            randomEventManager.Run(player, grid.PlayerX); // 열 기준 이벤트 실행

            CompleteCurrentNode(); // 노드 클리어 및 TRACE 증가
        }

        private void RunDataCacheNode(GridNode node)
        {
            player.Heal(45); // HP 회복

            player.RecoverEnergy(35); // ENERGY 회복

            RewardResult itemReward = rewardManager.TryGiveNodeItemReward(player, NodeType.DataCache, grid.PlayerX, 70); // DAT 보상 굴림

            CompleteCurrentNode(); // 노드 클리어 및 TRACE 증가

            renderer.RenderGrid(grid, player, systemInfection); // 모달 배경 GRID 출력
            renderer.ShowMessageBox("DATA CACHE", BuildDataCacheResultLines(itemReward), ConsoleColor.Green); // 데이터 캐시 결과 모달
            renderer.WaitKey("Q 창닫기"); 
        }

        private string[] BuildDataCacheResultLines(RewardResult itemReward)
        {
            if (itemReward != null && itemReward.HasItem) // 아이템 보상 체크
                return new string[] { "HEALTH +45", "ENERGY +35", itemReward.GetItemRewardText() };

            return new string[] { "HEALTH +45", "ENERGY +35", "DROP DETECTED : -" };
        }

        private void StartDebugBossBattleFromGrid()
        {
            currentState = GameState.Boss; // 보스전 테스트 상태 진입

            Enemy boss = enemyFactory.CreateBoss(systemInfection); // 테스트용 보스 생성

            string resultMessage; // 보스전 결과 메시지
            RewardResult rewardResult; // 보스전 보상 결과

            bool win = battleManager.StartBattle(player, boss, systemInfection, NodeType.Boss, grid.PlayerX, out resultMessage, out rewardResult); // 보스전 테스트 실행

            if (win && player.IsAlive) // 테스트 보스 처치 체크
            {
                bossDefeated = true; // 엔딩 진입 플래그
                currentState = GameState.Ending; // 엔딩 상태 설정
            }
            else if (player.IsAlive) // 테스트 후 생존 체크
                currentState = GameState.Grid; // GRID 복귀
            else
                currentState = GameState.GameOver; // 사망 시 게임오버 흐름 유지
        }

        private void RunBossNode()
        {
            if (player.AccessLevel < GameConfig.BossRequiredAccess) // 보스 접근 권한 체크
            {
                renderer.RenderGrid(grid, player, systemInfection); // 모달 배경 GRID 출력
                renderer.ShowMessageBox("KERNEL GATE", new string[] { "현재 ACCESS LEVEL: " + player.AccessLevel, "필요 ACCESS LEVEL: " + GameConfig.BossRequiredAccess, "접근 거부. ACCESS LEVEL " + GameConfig.BossRequiredAccess + "이 필요합니다.", "/Sec 장악 시 +1, /Fw 돌파 시 +2를 확보합니다." }, ConsoleColor.Red);
                renderer.WaitKey("Q 창닫기");
                return;
            }

            renderer.PlayGlitchCutscene("KERNEL CORE가 터미널 제어권을 탈취했습니다"); // 보스전 진입 연출

            Enemy boss = enemyFactory.CreateBoss(systemInfection); // 보스 생성

            string resultMessage; // 보스전 결과 메시지
            RewardResult rewardResult; // 보스전 보상 결과

            bool win = battleManager.StartBattle(player, boss, systemInfection, NodeType.Boss, grid.PlayerX, out resultMessage, out rewardResult); // 보스전 실행

            if (win) // 보스전 승리 체크
            {
                CompleteCurrentNode(); // Kernel 클리어 및 TRACE 증가
                bossDefeated = true; // 엔딩 진입 플래그
            }
        }

        private int CompleteCurrentNode()
        {
            GridNode node = grid.CurrentNode(); // 현재 노드 체크
            int traceIncrease = GameConfig.GetTraceIncreaseByNodeType(node.Type); // 노드별 TRACE 증가량

            grid.ResolveCurrentNode(); // 현재 노드 클리어
            IncreaseSystemInfection(traceIncrease); // 노드별 TRACE 증가

            return traceIncrease; // 실제 증가량
        }



        private void UseScanPulse()
        {
            if (player.Inventory.Remove(ItemNames.ScanPulse, 1)) // SCAN_PULSE 보유 체크
            {
                grid.RevealForwardFromClearedColumn(GameConfig.ScanRevealRadius); // 최전방 기준 정보 공개
                renderer.ShowSmallMessageBox("SCAN_PULSE", "전방 구역이 스캔되었습니다.", ConsoleColor.Cyan); // 성공 모달
                renderer.WaitKey("Q 창닫기"); 
            }
            else
            {
                renderer.ShowSmallMessageBox("SCAN_PULSE", "아이템이 부족합니다.", ConsoleColor.Red); // 실패 모달
                renderer.WaitKey("Q 창닫기"); 
            }
        }

        private void IncreaseSystemInfection(int amount)
        {
            systemInfection += amount; // TRACE 변화 적용

            if (systemInfection < 0)
                systemInfection = 0;

            if (systemInfection > GameConfig.MaxSystemInfection) // 최대 TRACE 체크
                systemInfection = GameConfig.MaxSystemInfection;
        }

        private void ReturnToGrid()
        {
            currentState = GameState.Grid; // GRID 입력 상태 복귀

            if (player.PendingMutation && !player.HasMutation && player.IsAlive) // 변이 이벤트 체크
            {
                renderer.RenderGrid(grid, player, systemInfection); // GRID 복귀 직전 화면

                payloadMutationManager.TryRun(player); // 변이 선택 실행

                currentState = GameState.Grid; // GRID 상태 유지
            }
        }

        private void ShowEnding()
        {
            renderer.ShowRootControlEnding(player, systemInfection, moveCount); // ROOT 장악 엔딩 / 크레딧 출력
        }

        private void ShowGameOver()
        {
            renderer.PlayGlitchCutscene("VIRUS.EXE 실행 종료"); // 게임오버 연출 출력

            renderer.ShowMessageBox("SYSTEM FAILURE", new string[]
            {
                "VIRUS.EXE가 응답하지 않습니다.",
                "KERNEL CORE는 여전히 시스템을 장악하고 있습니다."
            }, ConsoleColor.Red);

            renderer.WaitKey("Q 창닫기");
        }
    }
}
