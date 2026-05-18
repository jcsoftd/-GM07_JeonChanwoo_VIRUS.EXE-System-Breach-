using System;
using VirusExe.SystemBreach.Characters;
using VirusExe.SystemBreach.DataGrid;
using VirusExe.SystemBreach.Rendering;

namespace VirusExe.SystemBreach.Systems
{
    // 랜덤 이벤트 실행
    // 이벤트 선택지, 리스크 보상, 미니게임 연결 처리
    public class RandomEventManager
    {
        private readonly Random random = new Random();
        private readonly ConsoleRenderer renderer; // 화면 출력
        private readonly MiniGameManager miniGameManager; // 미니게임
        private readonly RewardManager rewardManager; // 보상

        public RandomEventManager(ConsoleRenderer renderer, MiniGameManager miniGameManager, RewardManager rewardManager)
        {
            this.renderer = renderer; 
            this.miniGameManager = miniGameManager; 
            this.rewardManager = rewardManager; 
        }

        public void Run(Player player, int column)
        {
            int roll = random.Next(0, 7);

            if (roll == 0) // 백업 데이터 이벤트
            {
                DamagedBackupEvent(player, column); // 백업 이벤트 실행
                return;
            }

            if (roll == 1) // 신호 동기화 이벤트
            {
                SignalSyncEvent(player, column); // SIGNAL SYNC 실행
                return;
            }

            if (roll == 2) // 오버클럭 이벤트
            {
                OverclockEvent(player); // 오버클럭 이벤트 실행
                return;
            }

            if (roll == 3) // 패치 파일 이벤트
            {
                UnknownPatchEvent(player, column); // 의심스러운 패치 이벤트 실행
                return;
            }

            if (roll == 4) // 데이터 캐시 도박 이벤트
            {
                VolatileDataCacheEvent(player, column); // KB 도박 이벤트 실행
                return;
            }

            if (roll == 5) // 실행파일 도박 이벤트
            {
                BlackboxExecutionEvent(player, column); // 실행파일 도박 이벤트 실행
                return;
            }

            FilePurgeEvent(player, column); // 파일 파괴 미니게임 이벤트 실행
        }

        private void DamagedBackupEvent(Player player, int column)
        {
            int input = renderer.ShowSelectionModal("DAMAGED BACKUP DATA", new string[] // 이벤트 선택지 출력
            {
                "손상된 백업 데이터를 발견했습니다.",
                "1~5 보안코드 3자리를 해독하면 보상을 얻습니다."
            }, new string[]
            {
                "탈취 시도",
                "폐기"
            }, ConsoleColor.Cyan, 1); // Q는 폐기 처리

            if (input == 1) // 폐기 선택 체크
            {
                ShowEventResult("백업 폐기", new string[] { "손상된 데이터를 격리했습니다." }, ConsoleColor.Gray); // 폐기 메시지
                return; // 이벤트 종료
            }

            bool success = miniGameManager.RunBackupRecoveryGame(); // 보안코드 해독 미니게임 실행

            if (success) // 탈취 성공 체크
            {
                player.AddKb(55); // KB 보상 지급
                player.AddAccessLevel(1); // ACCESS LEVEL 증가
                RewardResult itemReward = rewardManager.TryGiveNodeItemReward(player, NodeType.Event, column, 70); // 열 기준 아이템 보상
                ShowEventResult("탈취 보상", BuildRewardLines("KB +55", "ACCESS LEVEL +1", itemReward), ConsoleColor.Green); // 보상 메시지
                return; // 이벤트 종료
            }

            ShowEventResult("탈취 실패", new string[] { "백업 데이터 복구에 실패했습니다.", "획득 보상 없음" }, ConsoleColor.Red); // 실패 메시지
        }

        private void SignalSyncEvent(Player player, int column)
        {
            int input = renderer.ShowSelectionModal("UNSTABLE SIGNAL", new string[] // 이벤트 선택지 출력
            {
                "불안정한 관리자 신호가 근처 노드로 새고 있습니다.",
                "SAFE RANGE 안에서 신호를 고정하면 데이터를 회수할 수 있습니다."
            }, new string[]
            {
                "SIGNAL SYNC 실행: KB +35 / ENERGY +20 / 아이템 확률",
                "무시한다"
            }, ConsoleColor.Green, 1); // Q는 무시 처리

            if (input == 1) // 무시 선택 체크
            {
                ShowEventResult("SIGNAL BYPASS", new string[] { "불안정한 신호를 우회했습니다." }, ConsoleColor.Gray); // 무시 메시지
                return; // 이벤트 종료
            }

            bool success = miniGameManager.RunSignalSyncGame(); // SIGNAL SYNC 실행

            if (success) // 동기화 성공 체크
            {
                player.AddKb(35); // KB 보상 지급
                player.RecoverEnergy(20); // ENERGY 회복
                RewardResult itemReward = rewardManager.TryGiveNodeItemReward(player, NodeType.Event, column, 55); // 아이템 보상
                ShowEventResult("SIGNAL SYNC 보상", BuildRewardLines("KB +35", "ENERGY +20", itemReward), ConsoleColor.Green); // 성공 보상
                return; // 이벤트 종료
            }

            ShowEventResult("SIGNAL DRIFT", new string[] { "동기화에 실패했습니다.", "획득 보상 없음" }, ConsoleColor.Red); // 실패 메시지
        }

        private void OverclockEvent(Player player)
        {
            int input = renderer.ShowSelectionModal("UNSTABLE OVERCLOCK", new string[] // 선택지 출력
            {
                "불안정한 연산 클러스터를 점유했습니다.",
                "안전하게 출력을 올리거나, 위험하게 한계값을 깨뜨릴 수 있습니다."
            }, new string[]
            {
                "안정 오버클럭: ATK +1 / ENERGY -10",
                "위험 오버클럭: 65% ATK +3 / 실패 시 HEALTH -22, ENERGY -12",
                "취소한다"
            }, ConsoleColor.Yellow, 2); // Q는 취소 처리

            if (input == 0) // 안정 오버클럭 선택 체크
            {
                player.IncreaseAttack(1); // ATK 증가
                player.UseEnergy(10); // ENERGY 소모
                ShowEventResult("안정 오버클럭", new string[] { "출력 제한을 안전 범위에서 재작성했습니다.", "ATK +1", "ENERGY -10" }, ConsoleColor.Green); // 결과 출력
                return; // 이벤트 종료
            }

            if (input == 1) // 위험 오버클럭 선택 체크
            {
                if (random.Next(0, 100) < 65) // 성공 확률 체크
                {
                    player.IncreaseAttack(3); // ATK 대폭 증가
                    player.UseEnergy(16); // 성공 에너지 소모
                    ShowEventResult("한계 돌파", new string[] { "출력 제한을 강제로 찢어냈습니다.", "ATK +3", "ENERGY -16" }, ConsoleColor.Green); // 성공 출력
                    return; // 이벤트 종료
                }

                player.TakeDamage(22); // 실패 피해
                player.UseEnergy(12); // 실패 에너지 손실
                ShowEventResult("오버클럭 역류", new string[] { "불안정한 전류가 VIRUS.EXE 코어를 태웠습니다.", "HEALTH -22", "ENERGY -12" }, ConsoleColor.Red); // 실패 출력
                return; // 이벤트 종료
            }

            ShowEventResult("오버클럭 취소", new string[] { "연산 클러스터 점유를 해제했습니다." }, ConsoleColor.Gray); // 취소 출력
        }

        private void UnknownPatchEvent(Player player, int column)
        {
            int input = renderer.ShowSelectionModal("UNKNOWN PATCH FILE", new string[] // 선택지 출력
            {
                "서명되지 않은 패치 파일을 발견했습니다.",
                "회복 코드일 수도 있고, 보안 함정일 수도 있습니다."
            }, new string[]
            {
                "검증 설치: 70% HEALTH +25, ENERGY +15 / 실패 시 HEALTH -12",
                "강제 설치: 45% HEALTH +55, ENERGY +30, 아이템 확률 / 실패 시 큰 피해",
                "격리한다: KB +25 / 아이템 낮은 확률"
            }, ConsoleColor.Cyan, 2); // Q는 격리 처리

            if (input == 2) // 격리 선택 체크
            {
                player.AddKb(25); // 격리 보상 KB 지급
                RewardResult itemReward = rewardManager.TryGiveNodeItemReward(player, NodeType.Event, column, 35); // 낮은 확률 아이템 보상
                ShowEventResult("격리 완료", BuildRewardLines("의심스러운 파일을 격리했습니다.", "KB +25", itemReward), ConsoleColor.Green); // 격리 메시지
                return; // 이벤트 종료
            }

            if (input == 0) // 검증 설치 선택 체크
            {
                if (random.Next(0, 100) < 70) // 검증 설치 성공 체크
                {
                    player.Heal(25); // HEALTH 회복
                    player.RecoverEnergy(15); // ENERGY 회복
                    ShowEventResult("패치 적용", new string[] { "검증된 코드 조각만 추출했습니다.", "HEALTH +25", "ENERGY +15" }, ConsoleColor.Green); // 성공 메시지
                    return; // 이벤트 종료
                }

                player.TakeDamage(12); // 실패 피해
                ShowEventResult("검증 실패", new string[] { "검증 중 숨겨진 손상 코드가 실행되었습니다.", "HEALTH -12" }, ConsoleColor.Red); // 실패 메시지
                return; // 이벤트 종료
            }

            if (random.Next(0, 100) < 45) // 강제 설치 성공 체크
            {
                player.Heal(55); // HEALTH 대량 회복
                player.RecoverEnergy(30); // ENERGY 대량 회복
                RewardResult itemReward = rewardManager.TryGiveNodeItemReward(player, NodeType.Event, column, 45); // 아이템 보상
                ShowEventResult("강제 패치 성공", BuildRewardLines(new string[] { "패치 파일의 회복 루틴을 통째로 탈취했습니다.", "HEALTH +55", "ENERGY +30" }, itemReward), ConsoleColor.Green); // 성공 메시지
                return; // 이벤트 종료
            }

            player.TakeDamage(28); // 강제 설치 실패 피해
            player.UseEnergy(18); // ENERGY 손실
            ShowEventResult("강제 패치 실패", new string[] { "패치 파일 내부의 역추적 루틴이 폭주했습니다.", "HEALTH -28", "ENERGY -18" }, ConsoleColor.Red); // 실패 메시지
        }

        private void VolatileDataCacheEvent(Player player, int column)
        {
            int input = renderer.ShowSelectionModal("VOLATILE DATA CACHE", new string[] // 선택지 출력
            {
                "압축되지 않은 데이터 캐시가 노출되었습니다.",
                "깊게 긁을수록 보상은 커지지만 캐시 폭발 위험도 증가합니다."
            }, new string[]
            {
                "안전 회수: KB +25 확정",
                "깊게 추출: 60% KB +70, 아이템 확률 / 실패 시 ENERGY -15",
                "과부하 추출: 35% KB +120, 아이템 확률 / 실패 시 큰 피해",
                "무시한다"
            }, ConsoleColor.DarkYellow, 3); // Q는 무시 처리

            if (input == 0) // 안전 회수 선택 체크
            {
                player.AddKb(25); // KB 확정 지급
                ShowEventResult("CACHE EXTRACTED", new string[] { "불안정한 캐시 일부만 안전하게 회수했습니다.", "KB +25" }, ConsoleColor.Green); // 결과 출력
                return; // 이벤트 종료
            }

            if (input == 1) // 깊게 추출 선택 체크
            {
                if (random.Next(0, 100) < 60) // 깊게 추출 성공 체크
                {
                    player.AddKb(70); // KB 보상 지급
                    RewardResult itemReward = rewardManager.TryGiveNodeItemReward(player, NodeType.Event, column, 35); // 아이템 보상
                    ShowEventResult("DEEP CACHE HIT", BuildRewardLines("캐시 내부의 압축 데이터까지 회수했습니다.", "KB +70", itemReward), ConsoleColor.Green); // 성공 출력
                    return; // 이벤트 종료
                }

                player.UseEnergy(15); // ENERGY 손실
                ShowEventResult("CACHE DRIFT", new string[] { "캐시 블록이 중간에 붕괴했습니다.", "ENERGY -15" }, ConsoleColor.Red); // 실패 출력
                return; // 이벤트 종료
            }

            if (input == 2) // 과부하 추출 선택 체크
            {
                if (random.Next(0, 100) < 35) // 과부하 추출 성공 체크
                {
                    player.AddKb(120); // KB 대량 지급
                    RewardResult itemReward = rewardManager.TryGiveNodeItemReward(player, NodeType.Event, column, 65); // 높은 확률 아이템 보상
                    ShowEventResult("CACHE JACKPOT", BuildRewardLines("캐시 블록을 통째로 뜯어냈습니다.", "KB +120", itemReward), ConsoleColor.Green); // 성공 출력
                    return; // 이벤트 종료
                }

                player.TakeDamage(25); // 실패 피해
                player.UseEnergy(20); // 실패 에너지 손실
                ShowEventResult("CACHE EXPLOSION", new string[] { "압축 캐시가 역류하며 폭발했습니다.", "HEALTH -25", "ENERGY -20" }, ConsoleColor.Red); // 실패 출력
                return; // 이벤트 종료
            }

            ShowEventResult("CACHE IGNORED", new string[] { "불안정한 캐시를 건드리지 않았습니다." }, ConsoleColor.Gray); // 무시 출력
        }

        private void BlackboxExecutionEvent(Player player, int column)
        {
            int input = renderer.ShowSelectionModal("BLACKBOX EXECUTION", new string[] // 선택지 출력
            {
                "서명되지 않은 실행 파일이 자동 실행 대기 중입니다.",
                "좋은 페이로드일 수도 있고, 보안 함정일 수도 있습니다."
            }, new string[]
            {
                "즉시 실행: 45% ATK +1, KB +50, 아이템 확률 / 실패 시 피해",
                "샌드박스 실행: ENERGY -8 / 70% KB +35 / 실패 시 보상 없음",
                "삭제한다: KB +10"
            }, ConsoleColor.Magenta, 2); // Q는 삭제 처리

            if (input == 2) // 삭제 선택 체크
            {
                player.AddKb(10); // 삭제 보상
                ShowEventResult("FILE PURGED", new string[] { "실행 파일을 삭제하고 잔여 데이터를 회수했습니다.", "KB +10" }, ConsoleColor.Gray); // 삭제 출력
                return; // 이벤트 종료
            }

            if (input == 1) // 샌드박스 실행 선택 체크
            {
                player.UseEnergy(8); // 샌드박스 비용

                if (random.Next(0, 100) < 70) // 샌드박스 성공 체크
                {
                    player.AddKb(35); // KB 보상
                    ShowEventResult("SANDBOX RESULT", new string[] { "샌드박스 안에서 안전한 데이터 루틴을 분리했습니다.", "ENERGY -8", "KB +35" }, ConsoleColor.Green); // 성공 출력
                    return; // 이벤트 종료
                }

                ShowEventResult("SANDBOX EMPTY", new string[] { "실행 파일은 더미였습니다.", "ENERGY -8", "획득 보상 없음" }, ConsoleColor.Yellow); // 실패 출력
                return; // 이벤트 종료
            }

            if (random.Next(0, 100) < 45) // 즉시 실행 성공 체크
            {
                player.IncreaseAttack(1); // ATK 증가
                player.AddKb(50); // KB 보상
                RewardResult itemReward = rewardManager.TryGiveNodeItemReward(player, NodeType.Event, column, 45); // 아이템 보상
                ShowEventResult("BLACKBOX PAYLOAD", BuildRewardLines(new string[] { "실행 파일 내부의 페이로드를 장악했습니다.", "ATK +1", "KB +50" }, itemReward), ConsoleColor.Green); // 성공 출력
                return; // 이벤트 종료
            }

            player.TakeDamage(24); // 실패 피해
            player.UseEnergy(15); // 실패 에너지 손실
            ShowEventResult("BLACKBOX TRAP", new string[] { "파일 내부의 보안 루틴이 역실행되었습니다.", "HEALTH -24", "ENERGY -15" }, ConsoleColor.Red); // 실패 출력
        }

        private void FilePurgeEvent(Player player, int column)
        {
            int input = renderer.ShowSelectionModal("SUSPICIOUS CACHE PURGE", new string[] // 선택지 출력
            {
                "숨겨진 폴더에서 수상한 파일 캐시가 낙하 대기 중입니다.",
                "DELETE CANNON으로 파일을 삭제하면 잔여 데이터를 회수할 수 있습니다."
            }, new string[]
            {
                "삭제 요청 수락: 성공 시 KB +50 / ENERGY +15 / 아이템 확률",
                "안전 격리: KB +15",
                "무시한다"
            }, ConsoleColor.DarkYellow, 2); // Q는 무시 처리

            if (input == 2) // 무시 선택 체크
            {
                ShowEventResult("CACHE PURGE SKIPPED", new string[] { "수상한 캐시 묶음을 우회했습니다." }, ConsoleColor.Gray); // 무시 출력
                return; // 이벤트 종료
            }

            if (input == 1) // 격리 선택 체크
            {
                player.AddKb(15); // 안전 보상 지급
                ShowEventResult("CACHE QUARANTINED", new string[] { "수상한 파일 캐시를 안전하게 격리했습니다.", "KB +15" }, ConsoleColor.Green); // 격리 결과
                return; // 이벤트 종료
            }

            bool success = miniGameManager.RunFilePurgeGame(); // 파일 파괴 미니게임 실행

            if (success) // 파일 파괴 성공 체크
            {
                player.AddKb(50); // KB 보상 지급
                player.RecoverEnergy(15); // ENERGY 회복
                RewardResult itemReward = rewardManager.TryGiveNodeItemReward(player, NodeType.Event, column, 60); // 아이템 보상
                ShowEventResult("SUSPICIOUS CACHE 보상", BuildRewardLines(new string[] { "수상한 파일 캐시를 삭제했습니다.", "KB +50", "ENERGY +15" }, itemReward), ConsoleColor.Green); // 성공 보상
                return; // 이벤트 종료
            }

            player.UseEnergy(8); // 실패 에너지 손실
            ShowEventResult("SUSPICIOUS CACHE 실패", new string[] { "일부 수상한 파일이 버퍼 밖으로 유출되었습니다.", "ENERGY -8" }, ConsoleColor.Red);
        }

        private string[] BuildRewardLines(string first, string second, RewardResult itemReward)
        {
            if (itemReward == null || !itemReward.HasItem) // 아이템 보상 없음 체크
            {
                return new string[] { first, second }; // 기본 보상만
            }

            return new string[] { first, second, itemReward.GetItemRewardText() }; // 아이템 보상 포함
        }

        private string[] BuildRewardLines(string[] baseLines, RewardResult itemReward)
        {
            if (baseLines == null) // null 방지
            {
                baseLines = new string[0]; // 빈 배열 보정
            }

            if (itemReward == null || !itemReward.HasItem) // 아이템 보상 없음 체크
            {
                return baseLines; // 기본 라인만
            }

            string[] result = new string[baseLines.Length + 1]; // 아이템 라인 포함 배열

            for (int i = 0; i < baseLines.Length; i++) // 기본 라인 복사
            {
                result[i] = baseLines[i]; // 라인 복사
            }

            result[result.Length - 1] = itemReward.GetItemRewardText(); // 아이템 보상 추가
            return result;
        }
        private void ShowEventResult(string title, string[] lines, ConsoleColor color)
        {
            renderer.ShowMessageBox(title, lines, color); // 이벤트 결과 모달 출력
            renderer.WaitKey("Q 창닫기"); 
        }

    }
}
