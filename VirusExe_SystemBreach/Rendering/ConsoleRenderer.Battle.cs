using System;
using System.Collections.Generic;
using System.Threading;
using VirusExe.SystemBreach.Characters;
using VirusExe.SystemBreach.Core;
using VirusExe.SystemBreach.Systems;

namespace VirusExe.SystemBreach.Rendering
{
    // 전투 화면 출력
    // 플레이어/몬스터 상태, 전투 로그, 공격/피격/사망 연출 관리
    public partial class ConsoleRenderer
    {
        private const int BattleEnemyViewportRows = 15; // 전투 몬스터 고정 뷰포트 높이
        private const int BattleActionLogRows = 4; // 전투 ACTION LOG 고정 줄 수
        private const int BattleProcessingRows = 9; // 처리 중 EXECUTION LOG 표시 줄 수
        private bool renderEnemyDamagePopup; // 적 데미지 팝업 표시 여부
        private string renderEnemyDamagePopupText = string.Empty; // 적 데미지 팝업 문구
        private bool renderEnemyCriticalPopup; // 치명타 팝업 강화 여부
        private int renderEnemyImpactFrame; // 피격 파편 오버레이 프레임
        private int renderBossDeathFrame = -1; // 보스 3페이즈 사망 연출 프레임

        public void RenderBattle(Player player, Enemy enemy, string[] logs, int phase)
        {
            RenderBattle(player, enemy, logs, phase, 1); // 기본 선택 명령
        }

        public void RenderBattle(Player player, Enemy enemy, string[] logs, int phase, int selectedCommand)
        {
            RenderBattle(player, enemy, logs, phase, string.Empty, string.Empty, ConsoleColor.DarkGray, true, selectedCommand); // 선택 메뉴 전투 화면
        }

        public void RenderBattle(Player player, Enemy enemy, string[] logs, int phase, string actionText, string impactText, ConsoleColor actionColor, bool showMenu)
        {
            RenderBattle(player, enemy, logs, phase, actionText, impactText, actionColor, showMenu, 1); // 기본 선택값 사용
        }

        private void RenderBattle(Player player, Enemy enemy, string[] logs, int phase, string actionText, string impactText, ConsoleColor actionColor, bool showMenu, int selectedCommand)
        {
            ResetRenderCursor(); // 커서 초기화
            battleFrame++; // 전투 프레임 증가
            renderBossUiPhase = enemy.IsBoss ? phase : 0; // 보스 페이즈별 UI 오염 단계

            string title = enemy.IsBoss ? GetBossBattleTitle(phase) : "VIRUS PROCESS // SECURITY BATTLE"; // 전투 제목

            WriteHeader(title); // 헤더 출력
            WriteEnemyBattleStatus(enemy); // 적 상태 출력
            WriteEmptyLine(); // 몬스터 영역 상단 여백
            RenderEnemyViewport(enemy, phase, battleFrame); // 고정 몬스터 뷰포트 출력
            //WriteEmptyLine(); // 몬스터 영역 하단 여백
            WriteBattleActionPanel(actionText, impactText, actionColor); // 행동/결과 패널
            WriteSeparator(); // 플레이어 패널 구분선
            WritePlayerBattleStatus(player); // 플레이어 상태 패널
            WriteSeparator(); // 하단 영역 구분선

            if (showMenu) // 명령 선택 화면 체크
            {
                WriteBattleLogBlock(logs, BattleActionLogRows); // 기존 ACTION LOG 4줄 유지
                WriteSeparator(); // COMMAND STACK 구분선
                WriteBattleMenuLine(player, enemy.IsBoss && phase >= 3, selectedCommand); // 명령 선택창 출력
            }
            else
            {
                WriteBattleProcessingBlock(logs); // 처리 중 EXECUTION LOG 출력
            }

            WriteFooter(); // 하단 프레임 유지
            ClearRenderTail(); // 잔여 줄 제거
            HideCursor(); // 전투 커서 유배
            renderBossUiPhase = 0; // 전투 렌더 후 UI 오염 단계 해제
        }

        private string GetBossBattleTitle(int phase)
        {
            if (phase >= 3) return "KERNEL CORE // DARK FRAME BREACH"; // 3페이즈 다크 UI
            if (phase == 2) return "KERNEL CORE // UI DESYNC"; // 2페이즈 UI 지직임
            return "KERNEL CORE // BOSS BATTLE"; // 1페이즈 기본 전투
        }

        public void PlayEnemyDeathSequence(Player player, Enemy enemy, string[] logs, int phase)
        {
            renderDeadEnemy = true; // 죽은 적 아트 사용

            if (enemy != null && enemy.IsBoss && phase >= 3) // 3페이즈 보스 사망 체크
            {
                renderBossDeathFrame = 0; // 코어 균열
                RenderBattle(player, enemy, logs, phase, "ROOT_EYE 균열 감지", "KERNEL CORE 무결성 붕괴", ConsoleColor.Red, false);
                Thread.Sleep(360);

                renderBossDeathFrame = 1; // 눈깔 붕괴
                RenderBattle(player, enemy, logs, phase, "동공 신호 폭주", "PRIVILEGE DROP 실패", ConsoleColor.Red, false);
                Thread.Sleep(360);

                renderBossDeathFrame = 2; // 시스템 정지
                RenderBattle(player, enemy, logs, phase, "SYSTEM_HALT_0x00", "KERNEL NULL 상태 진입", ConsoleColor.Yellow, false);
                Thread.Sleep(520);

                renderBossDeathFrame = 3; // 신호 소실
                RenderBattle(player, enemy, logs, phase, "SIGNAL_LOST", "DATA VOID 확산", ConsoleColor.DarkGray, false);
                Thread.Sleep(420);

                renderBossDeathFrame = 4; // 완전 소멸
                RenderBattle(player, enemy, logs, phase, "0x00000000", "ROOT ACCESS END", ConsoleColor.DarkGray, false);
                Thread.Sleep(650);

                renderBossDeathFrame = -1; // 사망 프레임 초기화
                renderDeadEnemy = false; // 일반 아트 복구
                return;
            }

            RenderBattle(player, enemy, logs, phase, "프로세스 붕괴 감지", "대상 무결성 손상", ConsoleColor.Red, false);
            Thread.Sleep(500);

            RenderBattle(player, enemy, logs, phase, "감염 페이로드 확산", "보안 코어 오염 중", ConsoleColor.Yellow, false);
            Thread.Sleep(500);

            RenderBattle(player, enemy, logs, phase, "보안 프로세스 장악 완료", "대상 프로세스 침묵", ConsoleColor.Green, false);
            Thread.Sleep(700);

            renderBossDeathFrame = -1; // 사망 프레임 초기화
            renderDeadEnemy = false; // 일반 아트 복구
        }

        public void PlayEnemyHitSequence(Player player, Enemy enemy, string[] logs, int phase, int damage, bool critical)
        {
            renderEnemyHit = true; // 적 피격 아트 활성화
            renderEnemyHealthFlash = true; // 적 HEALTH 바 점멸 활성화
            renderEnemyDamagePopup = true; // 데미지 팝업 활성화
            renderEnemyDamagePopupText = "-" + damage; // 팝업 피해값 설정
            renderEnemyCriticalPopup = critical; // 치명타 오버레이 여부
            renderEnemyImpactFrame = 0; // 첫 파편 프레임

            RenderBattle(
                player,
                enemy,
                logs,
                phase,
                critical ? "CRITICAL MEMORY BREACH" : "MEMORY BREACH",
                "DATA CORRUPTION // -" + damage + " HEALTH",
                critical ? ConsoleColor.Red : ConsoleColor.Cyan,
                false);
            Thread.Sleep(220); // 데미지 박스 유지

            renderEnemyImpactFrame = 1; // 두 번째 파편 프레임

            RenderBattle(
                player,
                enemy,
                logs,
                phase,
                critical ? "CRITICAL PAYLOAD FRACTURE" : "PAYLOAD FRAGMENTATION",
                critical ? "### CORE SHATTER ###" : "*** PROCESS FRACTURE ***",
                critical ? ConsoleColor.Red : ConsoleColor.Magenta,
                false);
            Thread.Sleep(220); // 데미지 박스 유지

            renderEnemyHealthFlash = false; // 적 HEALTH 바 점멸 해제
            renderEnemyDamagePopup = false; // 데미지 팝업 해제
            renderEnemyCriticalPopup = false; // 치명타 오버레이 해제
            renderEnemyImpactFrame = 0; // 파편 프레임 초기화

            RenderBattle(
                player,
                enemy,
                logs,
                phase,
                "BREACH CONFIRMED",
                "TARGET HEALTH -" + damage,
                ConsoleColor.Cyan,
                false);
            Thread.Sleep(320); // 반격 전 짧은 여운

            renderEnemyHit = false; // 적 피격 아트 해제
        }

        public void PlayEnemyAttackWarningSequence(Player player, Enemy enemy, string[] logs, int phase)
        {
            renderEnemyAttackOffset = -4; // 공격 전 좌측 흔들림
            RenderBattle(player, enemy, logs, phase, "HOSTILE SIGNAL LOCKED", "INCOMING ATTACK", ConsoleColor.Red, false);
            Thread.Sleep(180);

            renderEnemyAttackOffset = 4; // 공격 전 우측 흔들림
            RenderBattle(player, enemy, logs, phase, "ATTACK VECTOR CONFIRMED", "IMPACT ROUTE OPEN", ConsoleColor.Red, false);
            Thread.Sleep(180);

            renderEnemyAttackOffset = 0; // 위치 복구
            RenderBattle(player, enemy, logs, phase, "SECURITY PROCESS STRIKING", "BRACE FOR IMPACT", ConsoleColor.Yellow, false);
            Thread.Sleep(260);
        }

        public void PlayPlayerHitSequence(Player player, Enemy enemy, string[] logs, int phase, int damage)
        {
            renderPlayerHit = true; // 플레이어 UI 파손 활성화
            renderPlayerHealthFlash = true; // 플레이어 HEALTH 바 점멸 활성화
            renderBattleGlitch = true; // 전투 화면 전체 글리치 활성화

            RenderBattle(
                player,
                enemy,
                logs,
                phase,
                "UI BREACH DETECTED",
                "HEALTH LOSS // -" + damage,
                ConsoleColor.Red,
                false);
            Thread.Sleep(90);

            RenderBattle(
                player,
                enemy,
                logs,
                phase,
                "FRAME BUFFER CORRUPTED",
                "FULL BATTLE FRAME DESYNC",
                ConsoleColor.DarkRed,
                false);
            Thread.Sleep(90);

            RenderBattle(
                player,
                enemy,
                logs,
                phase,
                "SYSTEM FRAME FRACTURE",
                "DISPLAY MATRIX UNSTABLE",
                ConsoleColor.Red,
                false);
            Thread.Sleep(90);

            renderBattleGlitch = false; // 전체 글리치 해제
            renderPlayerHealthFlash = false; // 플레이어 HEALTH 점멸 해제

            RenderBattle(
                player,
                enemy,
                logs,
                phase,
                "CONTROL RESTORED",
                "DAMAGE_IN :: -" + damage,
                ConsoleColor.Yellow,
                false);
            Thread.Sleep(220);

            renderPlayerHit = false; // 플레이어 UI 파손 해제
        }


        public void ShowBattleClearResult(Player player, Enemy enemy, RewardResult rewardResult, int accessGain, int traceGain)
        {
            battleFrame++; // 잔류 프레임 갱신

            int rewardLineCount = GetBattleResultRewardLineCount(rewardResult); // 보상 줄 수
            BeginModal("NODE CORRUPTION COMPLETE", 76, rewardLineCount + 8); // 전투 결과 모달 시작
            WriteModalSegmentsLine(new ColorSegment(" TARGET PROCESS : ", ConsoleColor.DarkGray), new ColorSegment(enemy.Name, enemy.IsBoss ? ConsoleColor.Red : ConsoleColor.Cyan));
            WriteModalSegmentsLine(new ColorSegment(" NODE STATUS    : ", ConsoleColor.DarkGray), new ColorSegment("~INFECTED~", ConsoleColor.Magenta));
            WriteModalSegmentsLine(new ColorSegment(" RESULT         : ", ConsoleColor.DarkGray), new ColorSegment("SECURITY PROCESS HIJACKED", ConsoleColor.Green));
            WriteModalSeparator();

            WriteBattleResultRewardLines(rewardResult); // 보상 출력
            WriteModalSegmentsLine(new ColorSegment(" ACCESS GAIN    : ", ConsoleColor.DarkGray), new ColorSegment("+" + accessGain, ConsoleColor.Cyan));
            WriteModalSegmentsLine(new ColorSegment(" TRACE EXPOSED  : ", ConsoleColor.DarkGray), new ColorSegment("+" + traceGain + "%", ConsoleColor.Yellow));

            WriteModalFooter(
                new ColorSegment(" Q", ConsoleColor.Red),
                new ColorSegment(" 창닫기", ConsoleColor.DarkGray)); // Footer 1줄
            EndModal(); // 전투 결과 모달 종료
            HideCursor(); // 커서 유배

            WaitBattleResultReturnKey(); // Q 창닫기 대기
            PlayRandomReturnToGridSequence(enemy); // 랜덤 복귀 연출
        }
        private void WaitBattleResultReturnKey()
        {
            while (true) // Q 창닫기 대기
            {
                ConsoleKey key = Console.ReadKey(true).Key; // 키 입력 읽기

                if (key == ConsoleKey.Q) // 창닫기 체크
                {
                    return;
                }
            }
        }
        private int GetBattleResultRewardLineCount(RewardResult rewardResult)
        {
            if (rewardResult == null || !rewardResult.HasAnyReward) return 2; // 기본 보상 줄
            return rewardResult.HasLevelUp ? 3 : 2; // 레벨업 줄 포함 여부
        }

        private void WriteBattleResultRewardLines(RewardResult rewardResult)
        {
            if (rewardResult == null || !rewardResult.HasAnyReward) // 보상 없음 체크
            {
                WriteModalSegmentsLine(new ColorSegment(" DATA EXTRACTED : ", ConsoleColor.DarkGray), new ColorSegment("-", ConsoleColor.DarkGray));
                WriteModalSegmentsLine(new ColorSegment(" DROP DETECTED  : ", ConsoleColor.DarkGray), new ColorSegment("-", ConsoleColor.DarkGray));
                return;
            }

            WriteModalSegmentsLine(new ColorSegment(" DATA EXTRACTED : ", ConsoleColor.DarkGray), new ColorSegment(rewardResult.GetBattleDataText(), ConsoleColor.Yellow));

            if (rewardResult.HasItem) // 아이템 드랍 체크
            {
                WriteModalSegmentsLine(new ColorSegment(" DROP DETECTED  : ", ConsoleColor.DarkGray), new ColorSegment(rewardResult.GetItemRewardText(), ConsoleColor.Green));
            }
            else
            {
                WriteModalSegmentsLine(new ColorSegment(" DROP DETECTED  : ", ConsoleColor.DarkGray), new ColorSegment("-", ConsoleColor.DarkGray));
            }

            if (rewardResult.HasLevelUp) // 레벨업 메시지 체크
            {
                WriteModalSegmentsLine(new ColorSegment(" LEVEL SIGNAL   : ", ConsoleColor.DarkGray), new ColorSegment(rewardResult.LevelUpMessage.Trim(), ConsoleColor.Cyan));
            }
        }

        private void PlayRandomReturnToGridSequence(Enemy enemy)
        {
            string nodeLabel = GetShortEnemyNodeLabel(enemy); // 노드 라벨 계산
            int maxStep = 24; // 이동 프레임 수

            for (int step = 0; step <= maxStep; step++) // PAYLOAD 이동
            {
                RenderNodeRewriteMotionFrame(nodeLabel, step, maxStep, false); // 이동 프레임 출력
                Thread.Sleep(45); // 이동 속도
            }

            for (int i = 0; i < 2; i++) // 감염 완료 점멸
            {
                RenderNodeRewriteMotionFrame(nodeLabel, maxStep, maxStep, true); // 감염 강조
                Thread.Sleep(90);
                RenderNodeRewriteMotionFrame(nodeLabel, maxStep, maxStep, false); // 일반 표시
                Thread.Sleep(90);
            }

            RenderNodeRewriteCompleteFrame(nodeLabel); // 최종 복귀 프레임
            Thread.Sleep(260);
        }
        private void RenderNodeRewriteMotionFrame(string nodeLabel, int step, int maxStep, bool flash)
        {
            string route = BuildMovingPayloadRoute(step, maxStep); // 이동 라인 생성
            string percent = GetMotionPercentText(step, maxStep); // 진행률 텍스트
            string status = step >= maxStep ? "NODE STATUS   : INFECTED" : "NODE STATUS   : REWRITING"; // 상태 텍스트
            string marker = flash ? "[~" + nodeLabel + "~]" : "[" + nodeLabel + "]"; // 점멸용 노드 표시

            BeginModal("SIGNAL GRID       // LIVE NODE REWRITE", ModalSize.Medium); // GRID 복귀 모달 시작
            WriteModalTextLine(" SECURITY PROCESS TERMINATED", ConsoleColor.Red);
            WriteModalTextLine(" PAYLOAD STREAM LOCKED", ConsoleColor.Yellow);
            WriteModalEmptyLine();
            WriteModalSegmentsLine(new ColorSegment(" TARGET NODE   : ", ConsoleColor.DarkGray), new ColorSegment("[" + nodeLabel + "]", ConsoleColor.Cyan));
            WriteModalSegmentsLine(new ColorSegment(" ROUTE SIGNAL  : ", ConsoleColor.DarkGray), new ColorSegment(percent, ConsoleColor.Green));
            WriteModalTextLine(" " + status, step >= maxStep ? ConsoleColor.Magenta : ConsoleColor.Gray);
            WriteModalEmptyLine();
            WriteModalTextLine(" [" + nodeLabel + "]" + route + marker, flash ? ConsoleColor.Magenta : ConsoleColor.Green);
            WriteModalEmptyLine();
            WriteModalTextLine(" ACCESS ROUTE  : scanning adjacent nodes...", ConsoleColor.Cyan);
            WriteModalTextLine(" TRACE SYNC    : pending", ConsoleColor.Yellow);
            WriteModalFooterText("AUTO REWRITE   PLEASE WAIT", ConsoleColor.DarkGray); // 자동 처리 Footer
            EndModal(); // GRID 복귀 모달 종료
            HideCursor(); // 커서 유배
        }
        private string BuildMovingPayloadRoute(int step, int maxStep)
        {
            int length = 34; // 이동 라인 길이
            int position = step * length / Math.Max(1, maxStep); // 현재 위치 계산

            if (position < 0) position = 0; // 최소 위치 보정
            if (position > length) position = length; // 최대 위치 보정

            char[] route = new char[length + 1]; // 라인 문자 배열

            for (int i = 0; i < route.Length; i++) // 기본 라인 생성
            {
                route[i] = '-'; // 비활성 경로
            }

            for (int i = 0; i < position; i++) // 지나간 경로 표시
            {
                route[i] = '='; // 감염 진행 경로
            }

            if (position < route.Length) // 이동 마커 위치 체크
            {
                route[position] = '>'; // PAYLOAD 이동 마커
            }

            return " " + new string(route) + " ";
        }
        private string GetMotionPercentText(int step, int maxStep)
        {
            int percent = step * 100 / Math.Max(1, maxStep); // 진행률 계산

            if (percent < 0) percent = 0; // 최소값 보정
            if (percent > 100) percent = 100; // 최대값 보정

            return percent.ToString("000") + "%";
        }
        private void RenderNodeRewriteCompleteFrame(string nodeLabel)
        {
            BeginModal("SIGNAL GRID       // LINK RESTORED", ModalSize.Medium); // GRID 복귀 완료 모달 시작
            WriteModalTextLine(" NODE STATUS   : INFECTED", ConsoleColor.Magenta);
            WriteModalTextLine(" ROUTE SIGNAL  : STABLE", ConsoleColor.Cyan);
            WriteModalTextLine(" GRID LINK     : ONLINE", ConsoleColor.Green);
            WriteModalEmptyLine();
            WriteModalTextLine("        [???]", ConsoleColor.DarkGray);
            WriteModalTextLine("          |", ConsoleColor.DarkGray);
            WriteModalTextLine(" [???]--[~" + nodeLabel + "~]--[???]", ConsoleColor.Magenta);
            WriteModalTextLine("          |", ConsoleColor.DarkGray);
            WriteModalTextLine("        [???]", ConsoleColor.DarkGray);
            WriteModalFooterText("AUTO REWRITE   PLEASE WAIT", ConsoleColor.DarkGray); // 자동 처리 Footer
            EndModal(); // GRID 복귀 완료 모달 종료
            HideCursor(); // 커서 유배
        }
        private string GetShortEnemyNodeLabel(Enemy enemy)
        {
            if (enemy == null) return "SEC"; // 기본 라벨
            if (enemy.IsBoss) return "BOS"; // 보스 라벨
            if (enemy.IsElite) return "FW"; // 엘리트 라벨

            return "SEC"; // 일반 전투 라벨
        }
        private void WritePlayerBattleStatus(Player player)
        {
            WritePlayerHudStatus(player, true); // GRID와 동일한 플레이어 HUD 재사용
        }

        private void WriteEnemyBattleStatus(Enemy enemy)
        {
            ConsoleColor nameColor = enemy.IsBoss ? ConsoleColor.Red : enemy.IsElite ? ConsoleColor.Magenta : ConsoleColor.Cyan; // 적 타입 색상
            ConsoleColor barColor = renderEnemyHealthFlash ? ConsoleColor.Red : enemy.IsBoss ? ConsoleColor.Red : ConsoleColor.Magenta; // 피격 시 HEALTH 점멸
            string enemyStatusText = BuildEnemyBattleStatusText(enemy); // 상태이상 태그 구성
            ConsoleColor enemyStatusColor = enemyStatusText == "[CLEAR]" ? ConsoleColor.DarkGray : ConsoleColor.Yellow; // 상태 존재 시 강조
            string attackText = enemy.AttackMin + " - " + enemy.AttackMax; // ATK 범위 문구
            int statusWidth = Math.Max(1, InnerWidth - StatusRightPadding - StatusTraceColumnStart - TextUtil.GetDisplayWidth("STATUS : ")); // 상태칸 폭
            List<ColorSegment> segments = new List<ColorSegment>(); // 고정 열 출력 세그먼트

            segments.Add(new ColorSegment(" TARGET   : ", ConsoleColor.DarkGray));
            segments.Add(new ColorSegment(enemy.Name, nameColor));
            PadSegmentsToColumn(segments, StatusRightColumnStart); // ATK 열 정렬
            segments.Add(new ColorSegment("ATK : ", ConsoleColor.DarkGray));
            segments.Add(new ColorSegment(TextUtil.Fit(attackText, 15), ConsoleColor.Yellow));
            PadSegmentsToColumn(segments, StatusTraceColumnStart); // STATUS 열 정렬
            segments.Add(new ColorSegment("STATUS : ", ConsoleColor.DarkGray));
            segments.Add(new ColorSegment(TextUtil.Fit(enemyStatusText, statusWidth), enemyStatusColor));
            WriteSegmentsLine(segments); // 적 이름/ATK/상태 출력

            WriteSegmentsLine(
                new ColorSegment(" HEALTH ", ConsoleColor.DarkGray),
                new ColorSegment(MakeDynamicBar(enemy.Hp, enemy.MaxHp), barColor),
                new ColorSegment(" " + enemy.Hp.ToString("000") + " / " + enemy.MaxHp.ToString("000"), barColor));
        }

        private string BuildEnemyBattleStatusText(Enemy enemy)
        {
            List<string> tags = new List<string>(); // 상태 태그 목록

            if (enemy.NextAttackDamageReductionPercent > 0) // 다음 공격 약화 체크
            {
                tags.Add("[ATK↓" + enemy.NextAttackDamageReductionPercent + "%]"); // 공격력 감소 표시
            }

            if (enemy.IsEncrypted) // 암호화 상태 체크
            {
                tags.Add("[ENCRYPT]"); // 랜섬웨어 상태 표시
            }

            if (enemy.PopupOverlayActive) // 팝업 오염 체크
            {
                tags.Add("[POPUP]"); // 애드웨어 팝업 표시
            }

            if (enemy.AdNotificationStacks > 0) // 고정 피해 상태 체크
            {
                tags.Add("[ADx" + enemy.AdNotificationStacks + "]"); // 알림 중첩 표시
            }

            if (tags.Count == 0) return "[CLEAR]"; // 상태 없음
            return string.Join(" ", tags.ToArray()); // 상태 태그 결합
        }

        private void WriteBattleActionPanel(string actionText, string impactText, ConsoleColor actionColor)
        {
            if (string.IsNullOrEmpty(actionText) && string.IsNullOrEmpty(impactText)) // 액션 없음 체크
            {
                WriteSegmentsLine(new ColorSegment(" 행동 : ", ConsoleColor.DarkGray), new ColorSegment("명령 대기 중", ConsoleColor.DarkGray));
                WriteSegmentsLine(new ColorSegment(" 결과 : ", ConsoleColor.DarkGray), new ColorSegment("-", ConsoleColor.DarkGray));
                return;
            }

            WriteSegmentsLine(new ColorSegment(" 행동 : ", ConsoleColor.DarkGray), new ColorSegment(actionText, actionColor));
            WriteSegmentsLine(new ColorSegment(" 결과 : ", ConsoleColor.DarkGray), new ColorSegment(impactText, actionColor));
        }

        private void RenderEnemyViewport(Enemy enemy, int phase, int frame)
        {
            RenderEnemyArt(enemy, phase, frame); // EnemyArt 내부 고정 뷰포트로 출력
        }

        private void WriteBattleLogBlock(string[] logs, int visibleRows)
        {
            if (logs == null) logs = new string[0]; // null 방지

            int start = Math.Max(0, logs.Length - visibleRows); // 최신 로그 기준 시작

            for (int i = 0; i < visibleRows; i++) // 고정 로그 줄 출력
            {
                int index = start + i; // 실제 로그 인덱스

                if (index < logs.Length) WriteLogLine(logs[index]); // 로그 출력
                else WriteLogLine(string.Empty); // 빈 로그 줄 유지
            }
        }

        private void WriteBattleProcessingBlock(string[] logs)
        {
            if (logs == null) logs = new string[0]; // null 방지

            WriteSegmentsLine(new ColorSegment(" EXECUTION LOG", ConsoleColor.Cyan)); // 처리 로그 제목
            WriteThinSeparator(); // 내부 얇은 구분선

            int start = Math.Max(0, logs.Length - BattleProcessingRows); // 확장 로그 시작

            for (int i = 0; i < BattleProcessingRows; i++) // 확장된 처리 로그 출력
            {
                int index = start + i; // 로그 인덱스

                if (index < logs.Length) WriteLogLine(logs[index]); // 로그 출력
                else WriteLogLine(string.Empty); // 빈 줄 유지
            }

            WriteSeparator(); // 하단 문구 구분선
            WriteSegmentsLine(new ColorSegment(" > ", ConsoleColor.DarkGray), new ColorSegment("신호 처리 중", ConsoleColor.Yellow), new ColorSegment(" ...", ConsoleColor.DarkGray)); // 처리 중 문구
        }
        private void WriteThinSeparator()
        {
            SetColor(ConsoleColor.Cyan);
            Console.Write(ApplyBattleGlitch("╟" + new string('─', InnerWidth) + "╢")); // 얇은 구분선 출력
            ClearPhysicalLineRemainder(); // 오른쪽 잔상 제거
            Console.WriteLine();
            Console.ResetColor();
        }

        private void WriteLogLine(string log)
        {
            ConsoleColor color = GetTaggedBattleLogColor(log); // 태그 기준 색상
            WriteLine(" " + log, color); // 로그 출력
        }

        private ConsoleColor GetTaggedBattleLogColor(string log)
        {
            if (string.IsNullOrEmpty(log)) return ConsoleColor.White; // 빈 로그 기본색
            if (log.StartsWith("[ENEMY]", StringComparison.Ordinal)) return ConsoleColor.Red; // 적 로그
            if (log.StartsWith("[SYSTEM]", StringComparison.Ordinal)) return ConsoleColor.Cyan; // 시스템 로그
            if (log.StartsWith("[VIRUS]", StringComparison.Ordinal)) return ConsoleColor.White; // 플레이어 로그
            if (log.StartsWith("[ITEM]", StringComparison.Ordinal)) return ConsoleColor.White; // 아이템 로그
            return ConsoleColor.White; // 기본 로그
        }

        private void WriteBattleMenuLine(Player player, bool corrupted, int selectedCommand)
        {
            selectedCommand = NormalizeBattleCommandIndex(selectedCommand); // 선택값 보정

            string selectedName = GetBattleCommandCategoryTag(selectedCommand) + " " + GetBattleCommandName(player, selectedCommand, corrupted); // 선택 명령 이름
            string[] infoLines = GetBattleCommandDescriptionLines(player, selectedCommand, corrupted); // 선택 설명

            WriteBattleDualLine(" COMMAND STACK", ConsoleColor.Cyan, " ACTION PROFILE :: " + selectedName, ConsoleColor.Cyan);

            for (int i = 1; i <= 5; i++) // 전투 명령 출력
            {
                bool selected = i == selectedCommand; // 현재 선택 체크
                string cursor = selected ? " >> " : "    "; // 선택 커서
                string commandName = GetBattleCommandCategoryTag(i) + " " + GetBattleCommandName(player, i, corrupted); // 폴더식 명령 이름
                string leftText = cursor + commandName; // 좌측 메뉴 텍스트
                string rightText = i - 1 < infoLines.Length ? infoLines[i - 1] : string.Empty; // 우측 설명 텍스트

                ConsoleColor leftColor = selected ? GetBattleCommandColor(player, i, corrupted) : ConsoleColor.DarkGray; // 메뉴 색상
                ConsoleColor rightColor = ConsoleColor.Gray; // 설명 색상

                WriteBattleDualLine(leftText, leftColor, rightText, rightColor);
            }

            WriteBattleDualLine(new string('-', 31), ConsoleColor.DarkGray, new string('-', 72), ConsoleColor.DarkGray); // 조작부 구분선

            string controlText = " W/S 이동   E 실행"; // 좌측 조작 설명
            string infoText = " 선택한 명령 실행"; // 우측 조작 설명

            int leftWidth = 31; // 좌측 메뉴 폭
            int rightWidth = InnerWidth - leftWidth - 3; // 우측 설명 폭

            int controlWidth = TextUtil.GetDisplayWidth(controlText); // 좌측 실제 폭
            int infoWidth = TextUtil.GetDisplayWidth(infoText); // 우측 실제 폭

            WriteSegmentsLine(
                new ColorSegment(" ", ConsoleColor.DarkGray),
                new ColorSegment("W/S", ConsoleColor.Cyan),
                new ColorSegment(" 이동   ", ConsoleColor.DarkGray),
                new ColorSegment("E", ConsoleColor.Green),
                new ColorSegment(" 실행", ConsoleColor.DarkGray),
                new ColorSegment(new string(' ', Math.Max(0, leftWidth - controlWidth)), ConsoleColor.DarkGray),
                new ColorSegment(" │ ", ConsoleColor.DarkGray),
                new ColorSegment(infoText, ConsoleColor.DarkGray),
                new ColorSegment(new string(' ', Math.Max(0, rightWidth - infoWidth)), ConsoleColor.DarkGray));
        }


        private void WriteBattleDualLine(string leftText, ConsoleColor leftColor, string rightText, ConsoleColor rightColor)
        {
            const int leftWidth = 31; // 좌측 메뉴 폭
            const int separatorWidth = 3; // 구분선 폭
            int rightWidth = InnerWidth - leftWidth - separatorWidth; // 우측 설명 폭

            WriteSegmentsLine(
                new ColorSegment(TextUtil.Fit(leftText, leftWidth), leftColor),
                new ColorSegment(" │ ", ConsoleColor.DarkGray),
                new ColorSegment(TextUtil.Fit(rightText, rightWidth), rightColor));
        }
        private string GetBattleCommandCategoryTag(int command)
        {
            if (command == 1) return "[ATK]"; // 일반공격 묶음
            if (command == 4 || command == 5) return "[ITEM]"; // 아이템사용 묶음
            return "[SKILL]"; // 스킬공격 묶음
        }
        private int NormalizeBattleCommandIndex(int selectedCommand)
        {
            if (selectedCommand < 1) return 1; // 최소값 보정
            if (selectedCommand > 5) return 5; // 최대값 보정
            return selectedCommand; // 정상값
        }
        private string GetBattleCommandName(Player player, int command, bool corrupted)
        {
            if (player != null && player.Mutation == VirusMutation.Ransomware) // 랜섬웨어 변이 체크
            {
                if (command == 1) return "ATK";
                if (command == 2) return "ENCRYPT";
                if (command == 3) return "RANSOM_NOTE";
                if (command == 4) return "PATCH";
                if (command == 5) return "ENERGY_CELL";
            }

            if (player != null && player.Mutation == VirusMutation.Trojan) // 트로젠 변이 체크
            {
                if (command == 1) return "ATK";
                if (command == 2) return "BACKDOOR";
                if (command == 3) return "SPOOF_AUTH";
                if (command == 4) return "PATCH";
                if (command == 5) return "ENERGY_CELL";
            }

            if (player != null && player.Mutation == VirusMutation.Adware) // 애드웨어 변이 체크
            {
                if (command == 1) return "ATK";
                if (command == 2) return "POPUP_FLOOD";
                if (command == 3) return "AD_NOTIFICATION";
                if (command == 4) return "PATCH";
                if (command == 5) return "ENERGY_CELL";
            }

            if (corrupted) // 보스 오염 메뉴 체크
            {
                if (command == 1) return "RESIST";
                if (command == 2) return "BREAK_ROOT";
                if (command == 3) return "STEALTH";
                if (command == 4) return "PATCH";
                if (command == 5) return "ENERGY_CELL";
            }

            if (command == 1) return "ATK";
            if (command == 2) return "OVERCLOCK";
            if (command == 3) return "STEALTH";
            if (command == 4) return "PATCH";
            if (command == 5) return "ENERGY_CELL";

            return "UNKNOWN";
        }

        private ConsoleColor GetBattleCommandColor(Player player, int command, bool corrupted)
        {
            if (player != null && player.Mutation == VirusMutation.Ransomware) // 랜섬웨어 색상
            {
                if (command == 2) return ConsoleColor.Yellow;
                if (command == 3) return ConsoleColor.Yellow;
            }

            if (player != null && player.Mutation == VirusMutation.Trojan) // 트로젠 색상
            {
                if (command == 2) return ConsoleColor.Magenta;
                if (command == 3) return ConsoleColor.DarkCyan;
            }

            if (player != null && player.Mutation == VirusMutation.Adware) // 애드웨어 색상
            {
                if (command == 2) return ConsoleColor.Green;
                if (command == 3) return ConsoleColor.Green;
            }

            if (corrupted) // 보스 오염 메뉴 체크
            {
                if (command == 1) return ConsoleColor.Red;
                if (command == 2) return ConsoleColor.Red;
                if (command == 3) return ConsoleColor.DarkRed;
                if (command == 4) return ConsoleColor.DarkRed;
                if (command == 5) return ConsoleColor.DarkRed;
            }

            if (command == 1) return ConsoleColor.Red;
            if (command == 2) return ConsoleColor.Yellow;
            if (command == 3) return ConsoleColor.DarkCyan;
            if (command == 4) return ConsoleColor.Green;
            if (command == 5) return ConsoleColor.Cyan;

            return ConsoleColor.White;
        }

        private string[] GetBattleCommandDescriptionLines(Player player, int command, bool corrupted)
        {
            if (player != null && player.Mutation == VirusMutation.Ransomware) // 랜섬웨어 설명
            {
                if (command == 1) return GetDefaultCommandDescription(command, corrupted);
                if (command == 2) return new string[] { BuildDamageDescription(SkillBalanceData.RansomwareEncryptMultiplierPercent), "  소모 : ENERGY " + SkillBalanceData.RansomwareEncryptEnergyCost, BuildCriticalDescription(SkillBalanceData.CommonCriticalChance, SkillBalanceData.CommonCriticalMultiplierPercent), "  상태 : 다음 적 공격 " + SkillBalanceData.RansomwareEncryptAttackReductionPercent + "% 감소", "  연계 : RANSOM_NOTE 사용 가능" };
                if (command == 3) return new string[] { BuildDamageDescription(SkillBalanceData.RansomwareNoteMultiplierPercent), "  소모 : ENERGY " + SkillBalanceData.RansomwareNoteEnergyCost, BuildCriticalDescription(SkillBalanceData.CommonCriticalChance, SkillBalanceData.CommonCriticalMultiplierPercent), "  흡수 : 피해 " + SkillBalanceData.RansomwareNoteHealPercent + "% 회복 / KB +" + SkillBalanceData.RansomwareNoteKbGain, "  조건 : ENCRYPT 상태 필요" };
            }

            if (player != null && player.Mutation == VirusMutation.Trojan) // 트로젠 설명
            {
                if (command == 1) return new string[] { BuildDamageDescription(SkillBalanceData.DefaultAttackMultiplierPercent), "  소모 : 없음", BuildCriticalDescription(SkillBalanceData.TrojanCriticalChance, SkillBalanceData.TrojanCriticalMultiplierPercent), "  특징 : 디버프 없는 순수 공격", "  위험 : 실행 후 적 반격 발생" };
                if (command == 2) return new string[] { BuildDamageDescription(SkillBalanceData.TrojanBackdoorMultiplierPercent), "  소모 : ENERGY " + SkillBalanceData.TrojanBackdoorEnergyCost, BuildCriticalDescription(SkillBalanceData.TrojanCriticalChance, SkillBalanceData.TrojanCriticalMultiplierPercent), "  역할 : 주력 공격기", "  강화 : SPOOF_AUTH 후 x" + SkillBalanceData.FormatMultiplier(SkillBalanceData.TrojanSpoofBackdoorMultiplierPercent) + " / 치명 +" + SkillBalanceData.TrojanSpoofCriticalBonusPercent + "%" };
                if (command == 3) return new string[] { "  설명 : 다음 BACKDOOR 자기 강화", "  소모 : ENERGY " + SkillBalanceData.TrojanSpoofAuthEnergyCost, "  피해 : 없음", "  강화 : 다음 BACKDOOR ATK x" + SkillBalanceData.FormatMultiplier(SkillBalanceData.TrojanSpoofBackdoorMultiplierPercent), "  치명 : 다음 BACKDOOR 치명 +" + SkillBalanceData.TrojanSpoofCriticalBonusPercent + "%" };
            }

            if (player != null && player.Mutation == VirusMutation.Adware) // 애드웨어 설명
            {
                if (command == 1) return GetDefaultCommandDescription(command, corrupted);
                if (command == 2) return new string[] { BuildDamageDescription(SkillBalanceData.AdwarePopupFloodMultiplierPercent), "  소모 : ENERGY " + SkillBalanceData.AdwarePopupFloodEnergyCost, BuildCriticalDescription(SkillBalanceData.CommonCriticalChance, SkillBalanceData.CommonCriticalMultiplierPercent), "  상태 : POPUP 부여", "  약화 : 다음 적 공격 " + SkillBalanceData.AdwarePopupFloodAttackReductionPercent + "% 감소" };
                if (command == 3) return new string[] { BuildDamageDescription(SkillBalanceData.AdNotificationMultiplierPercent), "  소모 : ENERGY " + SkillBalanceData.AdNotificationEnergyCost, "  치명 : 없음", "  지속 : 매턴 ATK x" + SkillBalanceData.FormatMultiplier(SkillBalanceData.AdNotificationTickPercent), "  중첩 : 최대 " + SkillBalanceData.AdNotificationMaxStacks + "중첩" };
            }

            return GetDefaultCommandDescription(command, corrupted); // 기본 설명
        }



        private string BuildDamageDescription(int multiplierPercent)
        {
            return "  피해 : ATK x" + SkillBalanceData.FormatMultiplier(multiplierPercent); // 피해 배율 설명
        }

        private string BuildCriticalDescription(int chancePercent, int multiplierPercent)
        {
            return "  치명 : " + chancePercent + "% / x" + SkillBalanceData.FormatMultiplier(multiplierPercent); // 치명타 설명
        }

        private string[] GetDefaultCommandDescription(int command, bool corrupted)
        {
            if (command == 1) // 기본 공격 설명
            {
                return new string[]
                {
                    BuildDamageDescription(SkillBalanceData.DefaultAttackMultiplierPercent),
                    "  소모 : 없음",
                    BuildCriticalDescription(SkillBalanceData.CommonCriticalChance, SkillBalanceData.CommonCriticalMultiplierPercent),
                    corrupted ? "  상태 : ROOT 오염 저항 명령" : "  상태 : 일반 침투 명령",
                    "  위험 : 실행 후 적 반격 발생"
                };
            }

            if (command == 2) // 강공격 설명
            {
                return new string[]
                {
                    BuildDamageDescription(SkillBalanceData.DefaultSkillMultiplierPercent),
                    "  소모 : ENERGY " + SkillBalanceData.DefaultSkillEnergyCost,
                    BuildCriticalDescription(SkillBalanceData.CommonCriticalChance, SkillBalanceData.CommonCriticalMultiplierPercent),
                    corrupted ? "  상태 : ROOT 연결 강제 파괴" : "  상태 : 과부하 공격 패킷",
                    "  위험 : 실행 후 적 반격 발생"
                };
            }

            if (command == 3) // 방어 설명
            {
                return new string[]
                {
                    "  설명 : STEALTH 프로토콜 활성화",
                    "  소모 : 없음",
                    "  효과 : 다음 피격 피해 감소",
                    "  상태 : 1회 방어 후 자동 해제",
                    "  위험 : 공격 없이 턴 소모"
                };
            }

            if (command == 4) // 회복 설명
            {
                return new string[]
                {
                    "  설명 : PATCH_32KB 실행",
                    "  소모 : PATCH 1개",
                    "  효과 : HEALTH +35",
                    "  실패 : PATCH 없으면 실행 불가",
                    "  위험 : 사용 후 적 반격 발생"
                };
            }

            if (command == 5) // ENERGY 회복 설명
            {
                return new string[]
                {
                    "  설명 : ENERGY_CELL_24KB 실행",
                    "  소모 : ENERGY_CELL 1개",
                    "  효과 : ENERGY +25",
                    "  실패 : ENERGY_CELL 없으면 실행 불가",
                    "  위험 : 사용 후 적 반격 발생"
                };
            }

            return new string[]
            {
                "  효과 : 알 수 없는 명령",
                "  소모 : -",
                "  결과 : -",
                "  상태 : -",
                "  위험 : -"
            };
        }

    }
}
