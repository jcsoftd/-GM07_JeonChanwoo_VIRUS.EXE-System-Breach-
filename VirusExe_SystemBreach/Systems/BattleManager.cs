using System;
using System.Collections.Generic;
using System.Threading;
using VirusExe.SystemBreach.Characters;
using VirusExe.SystemBreach.Core;
using VirusExe.SystemBreach.DataGrid;
using VirusExe.SystemBreach.Rendering;

namespace VirusExe.SystemBreach.Systems
{
    // 턴제 전투 처리
    // 플레이어 턴, 적 턴, 변이 스킬, 상태이상, 보상 흐름 연결
    public class BattleManager
    {
        private const int BattleLogHistoryRows = 9; // 처리 중 EXECUTION LOG 확장 표시용 최근 로그 수

        private readonly Random random = new Random(); // 데미지 계산 랜덤
        private readonly ConsoleRenderer renderer; // 화면 렌더러
        private readonly RewardManager rewardManager; // 보상 매니저
        private int activeBossPhase = 1; // 보스전 현재 페이즈
        private bool bossPhase3Recovered; // 3페이즈 진입 회복 처리 여부
        private bool debugForceBossEnding; // F10 엔딩 테스트 진입 여부

        public BattleManager(ConsoleRenderer renderer, RewardManager rewardManager)
        {
            this.renderer = renderer;
            this.rewardManager = rewardManager; // 보상 매니저 저장
        }

        public bool StartBattle(Player player, Enemy enemy, int systemInfection, NodeType nodeType, int column, out string resultMessage, out RewardResult rewardResult)
        {
            rewardResult = RewardResult.Empty(); // 기본 보상 결과
            List<string> logs = new List<string>(); // 전투 로그
            AddSystemLog(logs, enemy.Name + " 침투 프로세스가 접근했습니다."); // 첫 로그

            int turn = 1; // 전투 턴
            activeBossPhase = 1; // 보스 페이즈 초기화
            bossPhase3Recovered = false; // 3페이즈 회복 초기화
            debugForceBossEnding = false; // F10 엔딩 테스트 초기화

            while (player.IsAlive && enemy.IsAlive) // 생존 전투 체크
            {
                int phase = GetBossPhase(enemy, logs); // 보스 페이즈
                int command = ReadBattleCommandWithAnimation(player, enemy, logs, phase); // 명령 입력

                if (debugForceBossEnding) // F10 엔딩 테스트 체크
                {
                    activeBossPhase = 3; // 최종 페이즈 연출 고정
                    renderer.PlayEnemyDeathSequence(player, enemy, GetLastLogs(logs), activeBossPhase); // 보스 사망 연출
                    break; // 전투 종료
                }

                bool turnUsed = ExecutePlayerCommand(player, enemy, command, logs, phase); // 플레이어 행동

                if (!turnUsed) // 행동 실패 체크
                {
                    continue; // 턴 유지
                }

                phase = GetBossPhase(enemy, logs); // 행동 후 페이즈 갱신

                if (!enemy.IsAlive) // 적 사망 체크
                {
                    renderer.PlayEnemyDeathSequence(player, enemy, GetLastLogs(logs), GetBossPhase(enemy, logs)); // 최신 페이즈 제거 연출
                    break;
                }

                ApplyEnemyStatusDamage(player, enemy, logs, phase); // 상태이상 피해 처리

                if (!enemy.IsAlive) // 상태이상 사망 체크
                {
                    renderer.PlayEnemyDeathSequence(player, enemy, GetLastLogs(logs), GetBossPhase(enemy, logs)); // 최신 페이즈 제거 연출
                    break;
                }

                Thread.Sleep(650); // 결과 체크 딜레이

                ExecuteEnemyTurn(player, enemy, logs, turn, systemInfection); // 적 턴

                player.RecoverEnergy(4); // 턴 종료 ENERGY 회복

                turn++; // 턴 증가
            }

            if (player.IsAlive) // 플레이어 생존 체크
            {
                if (enemy.IsBoss) // 보스 전투 체크
                {
                    resultMessage = "KERNEL CORE 무력화 완료.";
                    rewardResult = RewardResult.Empty();
                    return true;
                }

                rewardResult = rewardManager.GiveBattleReward(player, enemy, nodeType, column); // 열 기준 보상 지급
                resultMessage = rewardResult.GetFieldMessage(); // 필드 복귀 보상 문구
                return true;
            }

            resultMessage = "VIRUS.EXE가 중지되었습니다.";
            rewardResult = RewardResult.Empty();
            return false;
        }

        private int ReadBattleCommandWithAnimation(Player player, Enemy enemy, List<string> logs, int phase)
        {
            int selectedCommand = 1; // 현재 선택 명령
            int commandCount = 5; // 명령 개수

            while (player.IsAlive && enemy.IsAlive) // 입력 대기 체크
            {
                renderer.RenderBattle(player, enemy, GetLastLogs(logs), phase, selectedCommand); // 전투 화면

                if (Console.KeyAvailable) // 키 입력 체크
                {
                    ConsoleKey key = Console.ReadKey(true).Key; // 키 읽기

                    if (key == ConsoleKey.W) // 위 이동 체크
                    {
                        selectedCommand--; // 이전 명령
                        if (selectedCommand < 1) selectedCommand = commandCount; // 순환
                    }
                    else if (key == ConsoleKey.S) // 아래 이동 체크
                    {
                        selectedCommand++; // 다음 명령
                        if (selectedCommand > commandCount) selectedCommand = 1; // 순환
                    }
                    else if (key == ConsoleKey.E) // 실행 체크
                    {
                        return selectedCommand; // 선택
                    }
                    else if (key == ConsoleKey.F9) // 보스 체력 테스트 체크
                    {
                        ApplyBossDebugHealthCut(player, enemy, logs); // 보스 HP 10% 감소
                        phase = GetBossPhase(enemy, logs); // 테스트 후 페이즈 갱신
                    }
                    else if (key == ConsoleKey.F10) // 보스 엔딩 테스트 체크
                    {
                        ActivateBossDebugEnding(player, enemy, logs); // 엔딩 테스트 진입
                        return 0; // 전투 루프 즉시 종료
                    }
                }

                Thread.Sleep(120); // 대기 렌더 속도
            }

            return 1; // 종료 예외값
        }

        private bool ExecutePlayerCommand(Player player, Enemy enemy, int command, List<string> logs, int phase)
        {
            if (command == 1) // 기본 공격 체크
            {
                return PlayerAttack(player, enemy, phase >= 3 && !player.HasMutation ? "RESIST" : "ATK", SkillBalanceData.DefaultAttackMultiplierPercent, 0, logs, phase, GetPlayerCriticalChance(player), GetPlayerCriticalMultiplierPercent(player));
            }
            else if (command == 2) // 2번 스킬 체크
            {
                return ExecuteSecondSkill(player, enemy, logs, phase); // 변이별 2번 스킬
            }
            else if (command == 3) // 3번 스킬 체크
            {
                return ExecuteThirdSkill(player, enemy, logs, phase); // 변이별 3번 스킬
            }
            else if (command == 4) // PATCH 체크
            {
                return UsePatch(player, enemy, logs, phase);
            }
            else if (command == 5) // ENERGY CELL 체크
            {
                return UseEnergyCell(player, enemy, logs, phase);
            }

            return false; // 알 수 없는 명령
        }

        private bool ExecuteSecondSkill(Player player, Enemy enemy, List<string> logs, int phase)
        {
            if (player.Mutation == VirusMutation.Ransomware) return UseRansomwareEncrypt(player, enemy, logs, phase); // 랜섬웨어 2번
            if (player.Mutation == VirusMutation.Trojan) return UseTrojanBackdoor(player, enemy, logs, phase); // 트로젠 2번
            if (player.Mutation == VirusMutation.Adware) return UseAdwarePopupFlood(player, enemy, logs, phase); // 애드웨어 2번

            return PlayerAttack(player, enemy, phase >= 3 ? "BREAK_ROOT" : "OVERCLOCK", SkillBalanceData.DefaultSkillMultiplierPercent, SkillBalanceData.DefaultSkillEnergyCost, logs, phase, SkillBalanceData.CommonCriticalChance, SkillBalanceData.CommonCriticalMultiplierPercent); // 기본 2번
        }

        private bool ExecuteThirdSkill(Player player, Enemy enemy, List<string> logs, int phase)
        {
            if (player.Mutation == VirusMutation.Ransomware) return UseRansomwareNote(player, enemy, logs, phase); // 랜섬웨어 3번
            if (player.Mutation == VirusMutation.Trojan) return UseTrojanSpoofAuth(player, enemy, logs, phase); // 트로젠 3번
            if (player.Mutation == VirusMutation.Adware) return UseAdNotification(player, enemy, logs, phase); // 애드웨어 3번

            return UseStealth(player, enemy, logs, phase); // 기본 3번
        }

        private bool PlayerAttack(Player player, Enemy enemy, string skillName, int skillMultiplierPercent, int energyCost, List<string> logs, int phase, int criticalChance, int criticalMultiplierPercent)
        {
            if (!TryUseEnergy(player, enemy, energyCost, logs, phase, skillName)) // ENERGY 체크
            {
                return false; // 실행 실패
            }

            int damage = RollAttackDamage(player, skillMultiplierPercent); // ATK 퍼센트 피해
            bool critical = RollCritical(criticalChance); // 치명타 판정

            if (critical) // 치명타 체크
            {
                damage = ApplyCriticalDamage(damage, criticalMultiplierPercent); // 치명타 피해
                AddCriticalLog(logs, skillName, criticalChance, criticalMultiplierPercent); // 치명타 로그
            }

            renderer.RenderBattle(player, enemy, GetLastLogs(logs), phase, skillName + " 신호 충전 중", "ATK x" + FormatMultiplier(skillMultiplierPercent), ConsoleColor.Yellow, false); // 공격 준비
            Thread.Sleep(350); // 준비 딜레이

            enemy.TakeDamage(damage); // 피해 적용
            int updatedPhase = GetBossPhase(enemy, logs); // 피해 적용 후 페이즈 재계산

            AddVirusLog(logs, player.Name + "가 " + skillName + " 명령을 실행하여 " + enemy.Name + "에게 " + damage + " 피해를 입혔습니다."); // 공격/피해 로그

            renderer.PlayEnemyHitSequence(player, enemy, GetLastLogs(logs), updatedPhase, damage, critical); // 갱신 페이즈 피격 연출

            return true; // 턴 사용
        }

        private bool UseRansomwareEncrypt(Player player, Enemy enemy, List<string> logs, int phase)
        {
            int energyCost = SkillBalanceData.RansomwareEncryptEnergyCost; // ENERGY 소모
            int skillMultiplierPercent = SkillBalanceData.RansomwareEncryptMultiplierPercent; // ENCRYPT 피해 배율
            int attackReductionPercent = SkillBalanceData.RansomwareEncryptAttackReductionPercent; // 다음 적 공격 약화율

            if (!TryUseEnergy(player, enemy, energyCost, logs, phase, "ENCRYPT")) // ENERGY 체크
            {
                return false; // 실행 실패
            }

            int damage = RollAttackDamage(player, skillMultiplierPercent); // 제어형 피해
            bool critical = RollCritical(SkillBalanceData.CommonCriticalChance); // 공통 치명타 판정

            if (critical) // 치명타 체크
            {
                damage = ApplyCriticalDamage(damage, SkillBalanceData.CommonCriticalMultiplierPercent); // 치명타 피해
                AddCriticalLog(logs, "ENCRYPT", SkillBalanceData.CommonCriticalChance, SkillBalanceData.CommonCriticalMultiplierPercent); // 치명타 로그
            }

            renderer.RenderBattle(player, enemy, GetLastLogs(logs), phase, "ENCRYPT 페이로드 주입", "ATK x" + FormatMultiplier(SkillBalanceData.RansomwareEncryptMultiplierPercent) + " / 공격 약화 " + SkillBalanceData.RansomwareEncryptAttackReductionPercent + "%", ConsoleColor.Yellow, false); // 암호화 준비
            Thread.Sleep(350); // 연출 딜레이

            enemy.ApplyEncryption(30); // $ 오염 적용
            enemy.ApplyNextAttackDamageReduction(attackReductionPercent); // 다음 공격 약화
            enemy.TakeDamage(damage); // 피해 적용
            int updatedPhase = GetBossPhase(enemy, logs); // 피해 적용 후 페이즈 재계산

            AddVirusLog(logs, "ENCRYPT 명령으로 " + enemy.Name + "에게 " + damage + " 피해를 입히고 다음 공격을 " + attackReductionPercent + "% 감소시켰습니다."); // 암호화/피해 로그

            renderer.PlayEnemyHitSequence(player, enemy, GetLastLogs(logs), updatedPhase, damage, critical); // 갱신 페이즈 피격 연출
            return true; // 턴 사용
        }

        private bool UseRansomwareNote(Player player, Enemy enemy, List<string> logs, int phase)
        {
            int energyCost = SkillBalanceData.RansomwareNoteEnergyCost; // ENERGY 소모
            int skillMultiplierPercent = SkillBalanceData.RansomwareNoteMultiplierPercent; // RANSOM_NOTE 피해 배율
            int healPercent = SkillBalanceData.RansomwareNoteHealPercent; // 피해 흡수율
            int kbGain = SkillBalanceData.RansomwareNoteKbGain; // KB 강탈량

            if (!enemy.IsEncrypted) // 암호화 상태 체크
            {
                AddSystemLog(logs, "RANSOM_NOTE는 암호화된 대상에게만 사용할 수 있습니다."); // 실패 로그
                renderer.RenderBattle(player, enemy, GetLastLogs(logs), phase, "명령 실행 실패", "대상 암호화 필요", ConsoleColor.Red, false); // 실패 출력
                Thread.Sleep(650); // 체크 딜레이
                return false; // 턴 미사용
            }

            if (!TryUseEnergy(player, enemy, energyCost, logs, phase, "RANSOM_NOTE")) // ENERGY 체크
            {
                return false; // 실행 실패
            }

            int damage = RollAttackDamage(player, skillMultiplierPercent); // 연계 마무리 피해
            bool critical = RollCritical(SkillBalanceData.CommonCriticalChance); // 공통 치명타 판정

            if (critical) // 치명타 체크
            {
                damage = ApplyCriticalDamage(damage, SkillBalanceData.CommonCriticalMultiplierPercent); // 치명타 피해
                AddCriticalLog(logs, "RANSOM_NOTE", SkillBalanceData.CommonCriticalChance, SkillBalanceData.CommonCriticalMultiplierPercent); // 치명타 로그
            }

            int healAmount = CalculatePercentDamage(damage, healPercent); // 피해 비례 회복

            renderer.RenderBattle(player, enemy, GetLastLogs(logs), phase, "RANSOM_NOTE 전송", "ATK x" + FormatMultiplier(SkillBalanceData.RansomwareNoteMultiplierPercent) + " / HEALTH 흡수", ConsoleColor.Yellow, false); // 강탈 준비
            Thread.Sleep(350); // 연출 딜레이

            player.AddKb(kbGain); // KB 강탈
            player.Heal(healAmount); // 피해량 기반 회복
            enemy.ClearEncryption(); // $ 오염 복구
            enemy.TakeDamage(damage); // 피해 적용
            int updatedPhase = GetBossPhase(enemy, logs); // 피해 적용 후 페이즈 재계산

            AddVirusLog(logs, "RANSOM_NOTE 명령으로 " + enemy.Name + "에게 " + damage + " 피해를 입히고 " + healAmount + " HEALTH / " + kbGain + "KB를 흡수했습니다."); // 흡수/피해 로그

            renderer.PlayEnemyHitSequence(player, enemy, GetLastLogs(logs), updatedPhase, damage, critical); // 갱신 페이즈 피격 연출
            return true; // 턴 사용
        }

        private bool UseTrojanBackdoor(Player player, Enemy enemy, List<string> logs, int phase)
        {
            int energyCost = SkillBalanceData.TrojanBackdoorEnergyCost; // ENERGY 소모
            int baseMultiplierPercent = SkillBalanceData.TrojanBackdoorMultiplierPercent; // 기본 BACKDOOR 배율

            if (!TryUseEnergy(player, enemy, energyCost, logs, phase, "BACKDOOR")) // ENERGY 체크
            {
                return false; // 실행 실패
            }

            bool spoofed = player.ConsumeTrojanSpoofAuth(); // 인증 위장 사용
            int skillMultiplierPercent = spoofed ? SkillBalanceData.TrojanSpoofBackdoorMultiplierPercent : baseMultiplierPercent; // 강화 배율
            int criticalChance = SkillBalanceData.TrojanCriticalChance + (spoofed ? SkillBalanceData.TrojanSpoofCriticalBonusPercent : 0); // 치명타 확률
            int damage = RollAttackDamage(player, skillMultiplierPercent); // 치명타형 피해
            bool critical = RollCritical(criticalChance); // 치명타 판정

            if (critical) // 치명타 체크
            {
                damage = ApplyCriticalDamage(damage, SkillBalanceData.TrojanCriticalMultiplierPercent); // 치명타 피해
                AddCriticalLog(logs, "BACKDOOR", criticalChance, SkillBalanceData.TrojanCriticalMultiplierPercent); // 치명타 로그
            }

            renderer.RenderBattle(player, enemy, GetLastLogs(logs), phase, "BACKDOOR 경로 개방", "ATK x" + FormatMultiplier(skillMultiplierPercent) + " / CRIT " + criticalChance + "%", ConsoleColor.Magenta, false); // 백도어 준비
            Thread.Sleep(350); // 연출 딜레이

            enemy.TakeDamage(damage); // 피해 적용
            int updatedPhase = GetBossPhase(enemy, logs); // 피해 적용 후 페이즈 재계산

            AddVirusLog(logs, player.Name + "가 BACKDOOR 명령을 실행하여 " + enemy.Name + "에게 " + damage + " 피해를 입혔습니다."); // 피해 로그
            renderer.PlayEnemyHitSequence(player, enemy, GetLastLogs(logs), updatedPhase, damage, critical); // 갱신 페이즈 피격 연출

            return true; // 턴 사용
        }

        private bool UseTrojanSpoofAuth(Player player, Enemy enemy, List<string> logs, int phase)
        {
            int energyCost = SkillBalanceData.TrojanSpoofAuthEnergyCost; // ENERGY 소모

            if (!TryUseEnergy(player, enemy, energyCost, logs, phase, "SPOOF_AUTH")) // ENERGY 체크
            {
                return false; // 실행 실패
            }

            player.ActivateTrojanSpoofAuth(); // 다음 BACKDOOR 강화

            AddVirusLog(logs, "SPOOF_AUTH 명령으로 다음 BACKDOOR를 ATK x" + FormatMultiplier(SkillBalanceData.TrojanSpoofBackdoorMultiplierPercent) + " / CRIT +" + SkillBalanceData.TrojanSpoofCriticalBonusPercent + "% 상태로 위장했습니다."); // 인증/강화 로그

            renderer.RenderBattle(player, enemy, GetLastLogs(logs), phase, "SPOOF_AUTH 활성화", "다음 BACKDOOR x" + FormatMultiplier(SkillBalanceData.TrojanSpoofBackdoorMultiplierPercent) + " / CRIT +" + SkillBalanceData.TrojanSpoofCriticalBonusPercent + "%", ConsoleColor.DarkCyan, false); // 결과 출력
            Thread.Sleep(650); // 체크 딜레이

            return true; // 턴 사용
        }

        private bool UseAdwarePopupFlood(Player player, Enemy enemy, List<string> logs, int phase)
        {
            int energyCost = SkillBalanceData.AdwarePopupFloodEnergyCost; // ENERGY 소모
            int skillMultiplierPercent = SkillBalanceData.AdwarePopupFloodMultiplierPercent; // POPUP_FLOOD 피해 배율
            int attackReductionPercent = SkillBalanceData.AdwarePopupFloodAttackReductionPercent; // 다음 적 공격 약화율

            if (!TryUseEnergy(player, enemy, energyCost, logs, phase, "POPUP_FLOOD")) // ENERGY 체크
            {
                return false; // 실행 실패
            }

            int damage = RollAttackDamage(player, skillMultiplierPercent); // 방해형 피해
            bool critical = RollCritical(SkillBalanceData.CommonCriticalChance); // 공통 치명타 판정

            if (critical) // 치명타 체크
            {
                damage = ApplyCriticalDamage(damage, SkillBalanceData.CommonCriticalMultiplierPercent); // 치명타 피해
                AddCriticalLog(logs, "POPUP_FLOOD", SkillBalanceData.CommonCriticalChance, SkillBalanceData.CommonCriticalMultiplierPercent); // 치명타 로그
            }

            enemy.ActivatePopupOverlay(); // 팝업 시각 효과
            enemy.ApplyNextAttackDamageReduction(attackReductionPercent); // 다음 공격 약화

            renderer.RenderBattle(player, enemy, GetLastLogs(logs), phase, "POPUP_FLOOD 살포", "ATK x" + FormatMultiplier(SkillBalanceData.AdwarePopupFloodMultiplierPercent) + " / 공격 약화 " + SkillBalanceData.AdwarePopupFloodAttackReductionPercent + "%", ConsoleColor.Green, false); // 팝업 연출
            Thread.Sleep(350); // 연출 딜레이

            enemy.TakeDamage(damage); // 피해 적용
            int updatedPhase = GetBossPhase(enemy, logs); // 피해 적용 후 페이즈 재계산

            AddVirusLog(logs, "POPUP_FLOOD 명령으로 " + enemy.Name + "에게 " + damage + " 피해를 입히고 다음 공격을 " + attackReductionPercent + "% 감소시켰습니다."); // 팝업/피해 로그

            renderer.PlayEnemyHitSequence(player, enemy, GetLastLogs(logs), updatedPhase, damage, critical); // 갱신 페이즈 피격 연출
            return true; // 턴 사용
        }

        private bool UseAdNotification(Player player, Enemy enemy, List<string> logs, int phase)
        {
            int energyCost = SkillBalanceData.AdNotificationEnergyCost; // ENERGY 소모
            int skillMultiplierPercent = SkillBalanceData.AdNotificationMultiplierPercent; // 즉시 피해 배율

            if (!TryUseEnergy(player, enemy, energyCost, logs, phase, "AD_NOTIFICATION")) // ENERGY 체크
            {
                return false; // 실행 실패
            }

            int beforeStacks = enemy.AdNotificationStacks; // 적용 전 중첩
            int damage = CalculatePercentDamage(player.Attack, skillMultiplierPercent); // 낮은 즉시 피해

            enemy.AddAdNotificationStack(); // 알림 중첩 증가

            renderer.RenderBattle(player, enemy, GetLastLogs(logs), phase, "AD_NOTIFICATION 전송", "ATK x" + FormatMultiplier(SkillBalanceData.AdNotificationMultiplierPercent) + " / TICK x" + FormatMultiplier(SkillBalanceData.AdNotificationTickPercent), ConsoleColor.Green, false); // 알림 준비
            Thread.Sleep(350); // 연출 딜레이

            enemy.TakeDamage(damage); // 즉시 피해 적용
            int updatedPhase = GetBossPhase(enemy, logs); // 피해 적용 후 페이즈 재계산

            if (enemy.AdNotificationStacks > beforeStacks) // 중첩 증가 체크
            {
                AddVirusLog(logs, "AD_NOTIFICATION 명령으로 " + enemy.Name + "에게 " + damage + " 피해를 입히고 알림 중첩 " + enemy.AdNotificationStacks + "개를 적용했습니다."); // 알림/피해 로그
            }
            else
            {
                AddVirusLog(logs, "AD_NOTIFICATION 명령으로 " + enemy.Name + "에게 " + damage + " 피해를 입혔지만 알림 중첩은 최대치입니다."); // 최대 중첩 로그
            }

            renderer.PlayEnemyHitSequence(player, enemy, GetLastLogs(logs), updatedPhase, damage, false); // 갱신 페이즈 피격 연출
            return true; // 턴 사용
        }

        private bool UseStealth(Player player, Enemy enemy, List<string> logs, int phase)
        {
            player.StealthActive = true; // 방어 상태 활성화
            AddVirusLog(logs, "STEALTH 프로토콜을 활성화했습니다."); // 로그 추가
            renderer.RenderBattle(player, enemy, GetLastLogs(logs), phase, "은신 프로토콜 활성화", "받는 피해 감소", ConsoleColor.DarkCyan, false); // 결과 출력
            Thread.Sleep(550); // 체크 딜레이
            return true; // 턴 사용
        }

        private bool UsePatch(Player player, Enemy enemy, List<string> logs, int phase)
        {
            if (player.Inventory.Remove(ItemNames.Patch, 1)) // PATCH 보유 체크
            {
                player.Heal(35); // HEALTH 회복
                AddItemLog(logs, "PATCH_32KB를 사용하여 HEALTH를 35 회복했습니다."); // 회복 로그
                renderer.RenderBattle(player, enemy, GetLastLogs(logs), phase, "PATCH_32KB 실행", "HEALTH +35 회복", ConsoleColor.Green, false); // 회복 출력
                Thread.Sleep(550); // 체크 딜레이
                return true; // 턴 사용
            }

            AddSystemLog(logs, "PATCH_32KB가 없습니다."); // 실패 로그
            renderer.RenderBattle(player, enemy, GetLastLogs(logs), phase, "명령 실패", "PATCH 없음", ConsoleColor.Red, false); // 실패 출력
            Thread.Sleep(650); // 체크 딜레이
            return false; // 턴 미사용
        }

        private bool UseEnergyCell(Player player, Enemy enemy, List<string> logs, int phase)
        {
            if (player.Inventory.Remove(ItemNames.EnergyCell, 1)) // ENERGY CELL 보유 체크
            {
                player.RecoverEnergy(25); // ENERGY 회복
                AddItemLog(logs, "ENERGY_CELL_24KB를 사용하여 ENERGY를 25 회복했습니다."); // 회복 로그
                renderer.RenderBattle(player, enemy, GetLastLogs(logs), phase, "ENERGY_CELL_24KB 실행", "ENERGY +25 회복", ConsoleColor.Cyan, false); // 회복 출력
                Thread.Sleep(550); // 체크 딜레이
                return true; // 턴 사용
            }

            AddSystemLog(logs, "ENERGY_CELL_24KB가 없습니다."); // 실패 로그
            renderer.RenderBattle(player, enemy, GetLastLogs(logs), phase, "명령 실패", "ENERGY_CELL 없음", ConsoleColor.Red, false); // 실패 출력
            Thread.Sleep(650); // 체크 딜레이
            return false; // 턴 미사용
        }

        private int RollAttackDamage(Player player, int skillMultiplierPercent)
        {
            int rollPercent = random.Next(SkillBalanceData.DamageRollMinPercent, SkillBalanceData.DamageRollMaxPercent + 1); // ATK 변동률
            int raw = player.Attack * rollPercent * skillMultiplierPercent; // ATK x 랜덤 x 스킬배율
            int damage = (raw + 5000) / 10000; // 반올림 정수화

            if (damage < 1) damage = 1; // 최소 피해 보정
            return damage; // 최종 피해
        }

        private int CalculatePercentDamage(int value, int percent)
        {
            int damage = (value * percent + 50) / 100; // 퍼센트 반올림
            if (damage < 1) damage = 1; // 최소값 보정
            return damage; // 계산 결과
        }

        private bool RollCritical(int chancePercent)
        {
            if (chancePercent <= 0) return false; // 치명타 없음
            return random.Next(0, 100) < chancePercent; // 확률 판정
        }

        private int ApplyCriticalDamage(int damage, int multiplierPercent)
        {
            return CalculatePercentDamage(damage, multiplierPercent); // 치명타 배율 적용
        }

        private int GetPlayerCriticalChance(Player player)
        {
            if (player != null && player.Mutation == VirusMutation.Trojan) return SkillBalanceData.TrojanCriticalChance; // 트로젠 특화
            return SkillBalanceData.CommonCriticalChance; // 공통 치명타
        }

        private int GetPlayerCriticalMultiplierPercent(Player player)
        {
            if (player != null && player.Mutation == VirusMutation.Trojan) return SkillBalanceData.TrojanCriticalMultiplierPercent; // 트로젠 치명타 피해
            return SkillBalanceData.CommonCriticalMultiplierPercent; // 공통 치명타 피해
        }

        private void AddCriticalLog(List<string> logs, string skillName, int criticalChance, int criticalMultiplierPercent)
        {
            AddSystemLog(logs, skillName + " 치명타 발생. CRIT " + criticalChance + "% / x" + FormatMultiplier(criticalMultiplierPercent)); // 치명타 로그
        }

        private string FormatMultiplier(int multiplierPercent)
        {
            return SkillBalanceData.FormatMultiplier(multiplierPercent); // 공통 배율 표기
        }

        private bool TryUseEnergy(Player player, Enemy enemy, int energyCost, List<string> logs, int phase, string skillName)
        {
            if (energyCost <= 0) // ENERGY 소모 없음 체크
            {
                return true; // 바로 성공
            }

            if (!player.HasEnergy(energyCost)) // ENERGY 부족 체크
            {
                AddSystemLog(logs, "ENERGY가 부족하여 " + skillName + " 실행에 실패했습니다."); // 실패 로그
                renderer.RenderBattle(player, enemy, GetLastLogs(logs), phase, "명령 실행 실패", "ENERGY 부족", ConsoleColor.Red, false); // 실패 출력
                Thread.Sleep(650); // 체크 딜레이
                return false; // 실패
            }

            player.UseEnergy(energyCost); // ENERGY 소모
            return true; // 성공
        }

        private void ApplyEnemyStatusDamage(Player player, Enemy enemy, List<string> logs, int phase)
        {
            if (enemy.AdNotificationStacks <= 0) // 알림 상태 없음 체크
            {
                return; // 처리 없음
            }

            int damage = CalculatePercentDamage(player.Attack, SkillBalanceData.AdNotificationTickPercent * enemy.AdNotificationStacks); // 중첩 지속 피해
            enemy.TakeDamage(damage); // 지속 피해 적용
            int updatedPhase = GetBossPhase(enemy, logs); // 피해 적용 후 페이즈 재계산

            AddSystemLog(logs, "AD_NOTIFICATION " + enemy.AdNotificationStacks + "중첩이 " + damage + " 지속 피해를 발생시켰습니다."); // 상태 피해 로그
            renderer.RenderBattle(player, enemy, GetLastLogs(logs), updatedPhase, "AD_NOTIFICATION TICK", "ATK x" + FormatMultiplier(SkillBalanceData.AdNotificationTickPercent * enemy.AdNotificationStacks), ConsoleColor.Green, false); // 갱신 페이즈 상태 피해 출력
            Thread.Sleep(420); // 체크 딜레이
        }

        private void ExecuteEnemyTurn(Player player, Enemy enemy, List<string> logs, int turn, int systemInfection)
        {
            int phase = GetBossPhase(enemy, logs); // 현재 페이즈

            renderer.PlayEnemyAttackWarningSequence(player, enemy, GetLastLogs(logs), phase); // 공격 예고

            int damage = random.Next(enemy.AttackMin, enemy.AttackMax + 1); // 적 피해량

            if (enemy.IsBoss && turn % 3 == 0) // 보스 특수 패턴 체크
            {
                damage += 8; // 보스 추가 피해
                AddEnemyLog(logs, "KERNEL CORE가 UI 오염 패턴을 실행했습니다."); // 보스 로그
            }

            int enemyReduction = enemy.ConsumeNextAttackDamageReductionPercent(); // 적 공격 약화 소비
            if (enemyReduction > 0) // 공격 약화 체크
            {
                damage = damage * (100 - enemyReduction) / 100; // 적 공격 약화 적용
                AddSystemLog(logs, "대상 공격 루트가 오염되어 피해가 " + enemyReduction + "% 감소했습니다."); // 약화 로그
            }

            int playerReduction = player.ConsumeNextDamageReductionPercent(); // 플레이어 방어 소비
            if (player.StealthActive) // STEALTH 체크
            {
                if (playerReduction < 50) playerReduction = 50; // 기본 은신 감소율
                player.StealthActive = false; // 은신 해제
            }

            if (playerReduction > 0) // 플레이어 방어 체크
            {
                damage = damage * (100 - playerReduction) / 100; // 플레이어 방어 적용
                AddSystemLog(logs, "방어 프로토콜이 피해를 " + playerReduction + "% 감소시켰습니다."); // 방어 로그
            }

            if (damage < 0) damage = 0; // 피해 하한 보정

            player.TakeDamage(damage); // 플레이어 피해 적용

            AddEnemyLog(logs, enemy.Name + "가 침투 공격을 실행하여 " + player.Name + "의 HEALTH를 " + damage + " 감소시켰습니다."); // 공격/피해 로그

            renderer.PlayPlayerHitSequence(player, enemy, GetLastLogs(logs), phase, damage); // 피격 연출
        }

        private void AddVirusLog(List<string> logs, string message)
        {
            logs.Add("[VIRUS] " + message); // 플레이어 로그
        }

        private void AddEnemyLog(List<string> logs, string message)
        {
            logs.Add("[ENEMY] " + message); // 적 로그
        }

        private void AddSystemLog(List<string> logs, string message)
        {
            logs.Add("[SYSTEM] " + message); // 시스템 로그
        }

        private void AddItemLog(List<string> logs, string message)
        {
            logs.Add("[ITEM] " + message); // 아이템 로그
        }

        private void ApplyBossDebugHealthCut(Player player, Enemy enemy, List<string> logs)
        {
            if (enemy == null || !enemy.IsBoss) // 보스전 여부 체크
            {
                return; // 일반 전투에서는 무시
            }

            int damage = enemy.MaxHp / 10; // 최대 HP 10% 피해
            if (damage < 1) damage = 1; // 최소 감소량 보정

            if (enemy.Hp <= 1) // 최소 HP 체크
            {
                AddSystemLog(logs, "F9 TEST: KERNEL CORE HP가 이미 최소값입니다."); // 테스트 로그
                renderer.RenderBattle(player, enemy, GetLastLogs(logs), GetBossPhase(enemy, logs), "F9 TEST BLOCKED", "BOSS HEALTH MINIMUM", ConsoleColor.DarkGray, false); // 테스트 결과
                Thread.Sleep(260); // 확인 딜레이
                return; // 추가 감소 없음
            }

            if (enemy.Hp - damage < 1) // HP 1 미만 방지 체크
            {
                damage = enemy.Hp - 1; // 최소 HP 1 유지
            }

            enemy.TakeDamage(damage); // 테스트 피해 적용
            AddSystemLog(logs, "F9 TEST: KERNEL CORE HP를 " + damage + " 감소시켰습니다."); // 테스트 로그

            int phase = GetBossPhase(enemy, logs); // 피해 후 페이즈 체크
            renderer.RenderBattle(player, enemy, GetLastLogs(logs), phase, "F9 TEST DAMAGE", "BOSS HEALTH -" + damage, ConsoleColor.Yellow, false); // 테스트 결과
            Thread.Sleep(260); // 확인 딜레이
        }


        private void ActivateBossDebugEnding(Player player, Enemy enemy, List<string> logs)
        {
            if (enemy == null || !enemy.IsBoss) // 보스전 여부 체크
            {
                return; // 일반 전투에서는 무시
            }

            activeBossPhase = 3; // 최종 페이즈 연출 강제
            debugForceBossEnding = true; // 전투 종료 후 엔딩 진입
            AddSystemLog(logs, "F10 TEST: KERNEL CORE 엔딩 테스트를 실행합니다."); // 테스트 로그
            renderer.RenderBattle(player, enemy, GetLastLogs(logs), activeBossPhase, "F10 ENDING TEST", "FORCE ROOT CONTROL", ConsoleColor.Red, false); // 테스트 결과
            Thread.Sleep(260); // 확인 딜레이
        }

        private int GetBossPhase(Enemy enemy)
        {
            return GetBossPhase(enemy, null); // 로그 없이 페이즈 확인
        }

        private int GetBossPhase(Enemy enemy, List<string> logs)
        {
            if (enemy == null || !enemy.IsBoss) // 보스 아님 체크
            {
                return 1; // 일반 페이즈
            }

            if (activeBossPhase < 2 && enemy.Hp <= enemy.MaxHp * 7 / 10) // 70% 이하 체크
            {
                activeBossPhase = 2; // 2페이즈 고정
                if (logs != null) AddSystemLog(logs, "KERNEL CORE가 2페이즈로 전환되었습니다."); // 전환 로그
            }

            if (activeBossPhase < 3 && enemy.Hp <= enemy.MaxHp * 3 / 10) // 30% 이하 체크
            {
                activeBossPhase = 3; // 3페이즈 고정

                if (!bossPhase3Recovered) // 3페이즈 회복 미처리 체크
                {
                    enemy.RestoreHealthFull(); // 3페이즈 진입 시 보스 HP 전체 회복
                    bossPhase3Recovered = true; // 회복 1회 처리
                    if (logs != null) AddSystemLog(logs, "KERNEL CORE가 모든 체력을 복구하고 3페이즈로 진입했습니다."); // 전환 로그
                }
            }

            return activeBossPhase; // 현재 페이즈 반환
        }

        private string[] GetLastLogs(List<string> logs)
        {
            int count = Math.Min(BattleLogHistoryRows, logs.Count); // 처리 중 확장 로그까지 고려한 최근 로그 수
            string[] result = new string[count]; // 결과 배열
            for (int i = 0; i < count; i++) // 로그 복사
            {
                result[i] = logs[logs.Count - count + i]; // 최근 로그
            }
            return result; // 로그
        }
    }
}
