using System;
using System.Collections.Generic;
using VirusExe.SystemBreach.Characters;
using VirusExe.SystemBreach.Core;
using VirusExe.SystemBreach.Systems;

namespace VirusExe.SystemBreach.Rendering
{
    // 콘솔 출력 공통 기능
    // 색상 출력, 상태 패널, 공통 바, 모달 보조 등 렌더러 기반 기능
    public partial class ConsoleRenderer
    {
        private const int InnerWidth = 106;
        private const int StatusRightColumnStart = 52; // LEVEL / ACCESS / WEAPON / GEAR 라벨 시작 열
        private const int StatusTraceColumnStart = 74; // EXP / TRACE 라벨 시작 열
        private const int StatusRightPadding = 2; // 오른쪽 벽 여백
        private int battleFrame; // 전투 애니메이션 프레임
        private bool renderDeadEnemy; // 죽은 적 아트 여부
        private bool renderEnemyHit; // 적 피격 아트 여부
        private bool renderPlayerHit; // 플레이어 피격 UI 여부
        private bool renderEnemyHealthFlash; // 적 HEALTH 바 점멸 여부
        private bool renderPlayerHealthFlash; // 플레이어 HEALTH 바 점멸 여부
        private bool renderBattleGlitch; // 전투 화면 전체 글리치 여부
        private bool suppressBattleTextGlitch; // 보호된 내부 텍스트 글리치 제외 여부
        private int renderBossUiPhase; // 보스 페이즈 UI 오염 단계
        private readonly Random glitchRandom = new Random(); // 전투 글리치 랜덤
        private int renderEnemyAttackOffset; // 적 공격 돌진 위치 보정
        private int lastRenderedBottom; // 이전 렌더 마지막 줄

        private struct ColorSegment
        {
            public string Text;
            public ConsoleColor Foreground;
            public ConsoleColor? Background;

            public ColorSegment(string text, ConsoleColor foreground)
            {
                Text = text ?? string.Empty; // null 방지
                Foreground = foreground; // 글자색 저장
                Background = null; // 배경색 없음
            }

            public ColorSegment(string text, ConsoleColor foreground, ConsoleColor background)
            {
                Text = text ?? string.Empty; // null 방지
                Foreground = foreground; // 글자색 저장
                Background = background; // 배경색 저장
            }
        }

        public static void TrySetupConsole()
        {
            try // 콘솔 크기 조정 실패 가능
            {
                Console.CursorVisible = false; // 시작 커서 숨김

                int windowWidth = Math.Min(GameConfig.ConsoleWidth, Console.LargestWindowWidth); // 문자 가로 보정
                int windowHeight = Math.Min(GameConfig.ConsoleHeight, Console.LargestWindowHeight); // 문자 세로 보정

                if (Console.WindowWidth > windowWidth || Console.WindowHeight > windowHeight) // 현재 창이 더 큰지 체크
                {
                    int shrinkWidth = Math.Min(Console.WindowWidth, windowWidth); // 축소 가로값
                    int shrinkHeight = Math.Min(Console.WindowHeight, windowHeight); // 축소 세로값
                    Console.SetWindowSize(shrinkWidth, shrinkHeight); // 버퍼 축소 전 창 축소
                }

                int bufferWidth = Math.Max(Console.BufferWidth, windowWidth); // 필요 버퍼 가로
                int bufferHeight = Math.Max(Console.BufferHeight, windowHeight); // 필요 버퍼 세로
                Console.SetBufferSize(bufferWidth, bufferHeight); // 창 설정 전 버퍼 확보
                Console.SetWindowSize(windowWidth, windowHeight); // 문자 창 크기 설정
                Console.SetBufferSize(windowWidth, windowHeight); // 스크롤 버퍼 고정
                Console.SetWindowPosition(0, 0); // 스크롤 맨 위 이동
            }
            catch
            {
            }

            TryResizeConsoleWindowPixels(GameConfig.ConsolePixelWidth, GameConfig.ConsolePixelHeight); // 픽셀 창 크기 설정
            TryScrollConsoleToTop(); // 스크롤 맨 위 보정
        }

        public void SetEnemyHitEffect(bool value)
        {
            renderEnemyHit = value; // 적 피격 아트 상태
        }

        public void SetPlayerHitEffect(bool value)
        {
            renderPlayerHit = value; // 플레이어 피격 UI 상태
        }

        public void SetEnemyHealthFlashEffect(bool value)
        {
            renderEnemyHealthFlash = value; // 적 HEALTH 바 점멸 상태
        }

        public void SetPlayerHealthFlashEffect(bool value)
        {
            renderPlayerHealthFlash = value; // 플레이어 HEALTH 바 점멸 상태
        }

        public void SetBattleGlitchEffect(bool value)
        {
            renderBattleGlitch = value; // 전투 화면 전체 글리치 상태
        }

        public void WaitKey(string message)
        {
            WaitModalKey(); // Q 창닫기 대기
        }

        public void ShowStatus(Player player)
        {
            BeginModal("VIRUS STATUS", ModalSize.Medium); // 상태창 모달 시작
            WriteModalSegmentsLine(new ColorSegment(" NAME          : ", ConsoleColor.DarkGray), new ColorSegment(player.Name, ConsoleColor.Green));
            WriteModalSegmentsLine(new ColorSegment(" LEVEL         : ", ConsoleColor.DarkGray), new ColorSegment(player.Level.ToString(), ConsoleColor.Yellow), new ColorSegment("    EXP : ", ConsoleColor.DarkGray), new ColorSegment(player.Exp.ToString(), ConsoleColor.Cyan), new ColorSegment(" / ", ConsoleColor.DarkGray), new ColorSegment(player.ExpToNext.ToString(), ConsoleColor.Cyan));
            WriteModalSegmentsLine(new ColorSegment(" HEALTH        : ", ConsoleColor.DarkGray), new ColorSegment(player.Stability.ToString(), ConsoleColor.Green), new ColorSegment(" / ", ConsoleColor.DarkGray), new ColorSegment(player.MaxStability.ToString(), ConsoleColor.Green));
            WriteModalSegmentsLine(new ColorSegment(" ENERGY        : ", ConsoleColor.DarkGray), new ColorSegment(player.Energy.ToString(), ConsoleColor.Cyan), new ColorSegment(" / ", ConsoleColor.DarkGray), new ColorSegment(player.MaxEnergy.ToString(), ConsoleColor.Cyan));
            WriteModalSegmentsLine(new ColorSegment(" ATK           : ", ConsoleColor.DarkGray), new ColorSegment(player.Attack.ToString(), ConsoleColor.Yellow));
            WriteModalSegmentsLine(new ColorSegment(" STORAGE       : ", ConsoleColor.DarkGray), new ColorSegment(player.Kb.ToString(), ConsoleColor.Yellow), new ColorSegment("KB", ConsoleColor.DarkGray));
            WriteModalSegmentsLine(new ColorSegment(" ACCESS LEVEL  : ", ConsoleColor.DarkGray), new ColorSegment(player.AccessLevel.ToString(), ConsoleColor.Cyan), new ColorSegment(" / ", ConsoleColor.DarkGray), new ColorSegment(GameConfig.BossRequiredAccess.ToString(), ConsoleColor.Cyan));
            WriteModalFooterText("Q 창닫기", ConsoleColor.Red); // Footer 1줄
            EndModal(); // 상태창 모달 종료
            WaitModalKey(); 
        }

        public void ShowMessageBox(string title, string[] lines, ConsoleColor color)
        {
            int lineCount = lines == null ? 0 : lines.Length; // 메시지 줄 수
            int bodyLines = Math.Max(6, lineCount + 2); // 메시지 높이 힌트

            BeginModal(title, 78, bodyLines); // 메시지 모달 시작

            if (lines != null) // 메시지 존재 체크
            {
                for (int i = 0; i < lines.Length; i++) // 메시지 줄 출력
                {
                    WriteModalTextLine(" " + lines[i], color); // 메시지 출력
                }
            }

            WriteModalFooterText("Q 창닫기", ConsoleColor.Red); // Footer 1줄
            EndModal(); // 메시지 모달 종료
        }

        public void ShowSmallMessageBox(string title, string message, ConsoleColor color)
        {
            BeginModal(title, ModalSize.Small); // 소형 메시지 모달 시작
            WriteModalCentered(message, color); // 중앙 메시지 출력
            WriteModalFooterText("Q 창닫기", ConsoleColor.Red); // Footer 1줄
            EndModal(); // 소형 메시지 모달 종료
        }

        public bool ShowGridExitConfirmModal()
        {
            int selectedIndex = 1; // 기본 선택은 취소
            string[] options = new string[] { "게임종료", "취소" }; // 종료 체크 선택지

            while (true) // 종료 체크 모달 루프
            {
                BeginModal("SYSTEM TERMINATE", ModalSize.Small); // Small 모달 시작
                WriteModalTextLine(" 게임을 종료하시겠습니까?", ConsoleColor.Yellow); // 종료 체크 문구
                WriteModalEmptyLine(); // 선택지 간격

                for (int i = 0; i < options.Length; i++) // 선택지 출력
                {
                    bool selected = i == selectedIndex; // 선택 위치 체크
                    WriteModalSegmentsLine(
                        new ColorSegment(selected ? " >> " : "    ", selected ? ConsoleColor.Magenta : ConsoleColor.DarkGray),
                        new ColorSegment(options[i], selected ? ConsoleColor.White : ConsoleColor.Gray));
                }

                WriteModalFooter(
                    new ColorSegment(" W/S", ConsoleColor.Cyan),
                    new ColorSegment(" 이동   ", ConsoleColor.DarkGray),
                    new ColorSegment("E", ConsoleColor.Green),
                    new ColorSegment(" 선택   ", ConsoleColor.DarkGray),
                    new ColorSegment("Q", ConsoleColor.Red),
                    new ColorSegment(" 창닫기", ConsoleColor.DarkGray));
                EndModal(); // 체크 모달 종료
                HideCursor(); // 커서 유배

                ConsoleKey key = InputHelper.ReadKey(); // 키 입력

                if (key == ConsoleKey.W || key == ConsoleKey.UpArrow) // 위 이동
                {
                    selectedIndex--; // 위 선택
                    if (selectedIndex < 0) selectedIndex = options.Length - 1; // 순환
                }
                else if (key == ConsoleKey.S || key == ConsoleKey.DownArrow) // 아래 이동
                {
                    selectedIndex++; // 아래 선택
                    if (selectedIndex >= options.Length) selectedIndex = 0; // 순환
                }
                else if (key == ConsoleKey.E || key == ConsoleKey.Enter) // 선택 확정
                {
                    return selectedIndex == 0; // 게임종료 선택 여부
                }
                else if (key == ConsoleKey.Q) // 창닫기
                {
                    return false; // 취소 처리
                }
            }
        }

        public int ShowSelectionModal(string title, string[] lines, string[] options, ConsoleColor color, int cancelIndex)
        {
            int selectedIndex = 0; // 선택 위치
            int optionCount = options == null ? 0 : options.Length; // 선택지 수
            int lineCount = lines == null ? 0 : lines.Length; // 설명 줄 수
            int bodyHint = lineCount + optionCount + 3; // 모달 크기 힌트

            if (optionCount <= 0) return cancelIndex; // 선택지 없음 방지

            while (true) // 선택 모달 루프
            {
                BeginModal(title, bodyHint >= 10 ? ModalSize.Large : ModalSize.Medium); // 선택 모달 시작

                if (lines != null) // 설명 출력
                {
                    for (int i = 0; i < lines.Length; i++) // 설명 줄 순회
                    {
                        WriteModalTextLine(" " + lines[i], color); // 설명 줄
                    }
                }

                if (lineCount > 0) WriteModalEmptyLine(); // 설명/선택 간격

                for (int i = 0; i < optionCount; i++) // 선택지 출력
                {
                    bool selected = i == selectedIndex; // 선택 체크
                    WriteModalSegmentsLine(
                        new ColorSegment(selected ? " >> " : "    ", selected ? ConsoleColor.Magenta : ConsoleColor.DarkGray),
                        new ColorSegment(options[i], selected ? ConsoleColor.White : ConsoleColor.Gray));
                }

                WriteModalFooter(
                    new ColorSegment(" W/S", ConsoleColor.Cyan),
                    new ColorSegment(" 이동   ", ConsoleColor.DarkGray),
                    new ColorSegment("E", ConsoleColor.Green),
                    new ColorSegment(" 선택   ", ConsoleColor.DarkGray),
                    new ColorSegment("Q", ConsoleColor.Red),
                    new ColorSegment(" 창닫기", ConsoleColor.DarkGray));
                EndModal(); // 선택 모달 종료
                HideCursor(); // 커서 유배

                ConsoleKey key = InputHelper.ReadKey(); // 키 입력

                if (key == ConsoleKey.W || key == ConsoleKey.UpArrow) // 위 이동
                {
                    selectedIndex--; // 위 선택
                    if (selectedIndex < 0) selectedIndex = optionCount - 1; // 순환
                }
                else if (key == ConsoleKey.S || key == ConsoleKey.DownArrow) // 아래 이동
                {
                    selectedIndex++; // 아래 선택
                    if (selectedIndex >= optionCount) selectedIndex = 0; // 순환
                }
                else if (key == ConsoleKey.E || key == ConsoleKey.Enter) // 선택 확정
                {
                    return selectedIndex; // 선택
                }
                else if (key == ConsoleKey.Q) // 창닫기
                {
                    return cancelIndex; // 취소 선택
                }
            }
        }

        private string MakeBar(int current, int max, int length)
        {
            if (max <= 0) return "[ERROR]"; // 최대값 오류 체크
            if (current < 0) current = 0; // 최소값 보정
            if (current > max) current = max; // 최대값 보정

            int filled = current * length / max;
            return "[" + new string('█', filled) + new string('░', length - filled) + "]";
        }

        private string MakeDynamicBar(int current, int max)
        {
            if (max <= 0) return "[ERROR]"; // 최대값 오류 체크
            if (current < 0) current = 0; // 최소값 보정
            if (current > max) current = max; // 최대값 보정

            int length = Math.Max(1, max / 10); // 최대 수치 10당 1칸
            int filled = current * length / max; // 현재 비율 칸 계산

            return "[" + new string('█', filled) + new string('░', length - filled) + "]";
        }

        private ConsoleColor GetPercentColor(int value, bool lowerIsBad)
        {
            if (lowerIsBad) // 낮을수록 위험한 수치 체크
            {
                if (value <= 25) return ConsoleColor.Red;
                if (value <= 50) return ConsoleColor.Yellow;
                return ConsoleColor.Green;
            }

            if (value >= 75) return ConsoleColor.Red;
            if (value >= 45) return ConsoleColor.Yellow;
            return ConsoleColor.Green;
        }

        private void WritePlayerHudStatus(Player player, bool battleMode)
        {
            bool previousSuppressBattleTextGlitch = suppressBattleTextGlitch; // 기존 글리치 보호 상태 보관
            suppressBattleTextGlitch = battleMode && renderBossUiPhase >= 2; // 보스 2/3페이즈 VIRUS.EXE 내부 정보 보호

            try
            {
                string youLabel = renderPlayerHit ? " Y0U : " : " YOU : "; // 피격 시 라벨 노이즈
                string payloadLabel = renderPlayerHit ? " PAYL?AD : " : " PAYLOAD : "; // 피격 시 PAYLOAD 라벨
                string healthLabel = renderPlayerHit ? " HEA#TH  " : " HEALTH  "; // 피격 시 HEALTH 라벨
                string energyLabel = renderPlayerHit ? " EN?RGY  " : " ENERGY  "; // 피격 시 ENERGY 라벨
                ConsoleColor nameColor = renderPlayerHit ? ConsoleColor.White : ConsoleColor.Green; // 이름 색상
                ConsoleColor healthColor = renderPlayerHealthFlash ? ConsoleColor.Red : ConsoleColor.Green; // HEALTH 색상
                ConsoleColor energyColor = renderPlayerHit ? ConsoleColor.DarkCyan : ConsoleColor.Cyan; // ENERGY 색상
                string weaponName = GetEquippedItemDisplayName(player.EquippedWeapon); // 무기명
                string gearName = GetEquippedItemDisplayName(player.EquippedGear); // 장비명

                if (battleMode && renderPlayerHit) // 전투 피격 HUD 파손 표시
                {
                    WriteSegmentsLine(
                        new ColorSegment(" ", ConsoleColor.DarkGray),
                        new ColorSegment(">>> PLAYER HUD DESYNC ", ConsoleColor.White, ConsoleColor.DarkRed),
                        new ColorSegment("// FRAME BUFFER CORRUPTED <<<", ConsoleColor.Red));
                }

                WritePlayerTitleLine(youLabel, player, nameColor); // YOU / LEVEL / EXP 출력
                WriteStatusThinSeparator(); // 상태 패널 내부 구분선
                WriteSegmentsLine(
                    new ColorSegment(payloadLabel, renderPlayerHit ? ConsoleColor.Red : ConsoleColor.DarkGray),
                    new ColorSegment(GetPayloadDisplayName(player), renderPlayerHit ? ConsoleColor.White : ConsoleColor.Magenta));
                WriteSegmentsLine(
                    new ColorSegment(healthLabel, renderPlayerHit ? ConsoleColor.Red : ConsoleColor.DarkGray),
                    new ColorSegment(MakeDynamicBar(player.Stability, player.MaxStability), healthColor, renderPlayerHealthFlash ? ConsoleColor.DarkRed : ConsoleColor.Black),
                    new ColorSegment("    " + player.Stability + " / " + player.MaxStability, healthColor));
                WriteSegmentsLine(
                    new ColorSegment(energyLabel, renderPlayerHit ? ConsoleColor.Red : ConsoleColor.DarkGray),
                    new ColorSegment(MakeDynamicBar(player.Energy, player.MaxEnergy), energyColor),
                    new ColorSegment("    " + player.Energy + " / " + player.MaxEnergy, energyColor));
                WritePlayerStatusPairLine("ATK", player.Attack.ToString(), ConsoleColor.Yellow, "WEAPON", weaponName, ConsoleColor.White); // WEAPON 열 정렬
                WritePlayerStatusPairLine("STORAGE", player.Kb + "KB", ConsoleColor.Yellow, "GEAR", gearName, ConsoleColor.White); // GEAR 열 정렬
            }
            finally
            {
                suppressBattleTextGlitch = previousSuppressBattleTextGlitch; // 글리치 보호 상태 복구
            }
        }

        private void WritePlayerTitleLine(string youLabel, Player player, ConsoleColor nameColor)
        {
            string nameText = "VIRUS.EXE"; // 플레이어 표시명
            string levelPrefix = "LEVEL : "; // LEVEL 라벨
            string expPrefix = "EXP : "; // EXP 라벨
            string expValue = player.Exp + " / " + player.ExpToNext; // EXP 값
            int nameWidth = Math.Max(1, StatusRightColumnStart - TextUtil.GetDisplayWidth(youLabel) - 1); // LEVEL 전 이름 영역
            int levelValueWidth = Math.Max(1, StatusTraceColumnStart - StatusRightColumnStart - TextUtil.GetDisplayWidth(levelPrefix) - 1); // LEVEL 값 폭
            int expValueWidth = Math.Max(1, InnerWidth - StatusRightPadding - StatusTraceColumnStart - TextUtil.GetDisplayWidth(expPrefix)); // EXP 값 폭
            List<ColorSegment> segments = new List<ColorSegment>(); // 고정 열 세그먼트

            segments.Add(new ColorSegment(youLabel, renderPlayerHit ? ConsoleColor.Red : ConsoleColor.DarkGray));
            segments.Add(new ColorSegment(TextUtil.Fit(nameText, nameWidth), nameColor, renderPlayerHit ? ConsoleColor.DarkRed : ConsoleColor.Black));
            PadSegmentsToColumn(segments, StatusRightColumnStart); // LEVEL / ACCESS 열 정렬
            segments.Add(new ColorSegment(levelPrefix, ConsoleColor.DarkGray));
            segments.Add(new ColorSegment(TextUtil.Fit(player.Level.ToString(), levelValueWidth), ConsoleColor.Yellow));
            PadSegmentsToColumn(segments, StatusTraceColumnStart); // EXP / TRACE 열 정렬
            segments.Add(new ColorSegment(expPrefix, ConsoleColor.DarkGray));
            segments.Add(new ColorSegment(TextUtil.Fit(expValue, expValueWidth), ConsoleColor.Cyan));

            WriteSegmentsLine(segments);
        }

        private void WritePlayerStatusPairLine(string leftLabel, string leftValue, ConsoleColor leftColor, string rightLabel, string rightValue, ConsoleColor rightColor)
        {
            string leftPrefix = " " + TextUtil.Fit(leftLabel, 7) + " : "; // 좌측 라벨
            string rightPrefix = TextUtil.Fit(rightLabel, 7) + " : "; // 우측 라벨
            int leftValueWidth = Math.Max(1, StatusRightColumnStart - TextUtil.GetDisplayWidth(leftPrefix) - 1); // 우측 열 전까지 값 폭
            int rightValueWidth = Math.Max(1, InnerWidth - StatusRightPadding - StatusRightColumnStart - TextUtil.GetDisplayWidth(rightPrefix)); // 우측 값 폭
            List<ColorSegment> segments = new List<ColorSegment>(); // 고정 열 세그먼트

            segments.Add(new ColorSegment(leftPrefix, ConsoleColor.DarkGray));
            segments.Add(new ColorSegment(TextUtil.Fit(leftValue, leftValueWidth), leftColor));
            PadSegmentsToColumn(segments, StatusRightColumnStart); // WEAPON / GEAR 열 정렬
            segments.Add(new ColorSegment(rightPrefix, ConsoleColor.DarkGray));
            segments.Add(new ColorSegment(TextUtil.Fit(rightValue, rightValueWidth), rightColor));

            WriteSegmentsLine(segments);
        }

        private void WriteStatusThinSeparator()
        {
            SetColor(ConsoleColor.Cyan);
            Console.Write(ApplyBattleGlitch("╟" + new string('─', InnerWidth) + "╢"));
            ClearPhysicalLineRemainder(); // 오른쪽 잔상 제거
            Console.WriteLine();
            Console.ResetColor();
        }

        private void PadSegmentsToColumn(List<ColorSegment> segments, int targetColumn)
        {
            int currentWidth = GetSegmentsWidth(segments); // 현재 출력 폭
            int padding = targetColumn - currentWidth; // 목표 열까지 필요한 공백

            if (padding > 0) // 목표 열 도달 전 체크
            {
                segments.Add(new ColorSegment(new string(' ', padding), ConsoleColor.DarkGray)); // 고정 열 공백
            }
        }


        private void WriteControlLine()
        {
            WriteSegmentsLine(
                new ColorSegment(" ", ConsoleColor.Gray),
                new ColorSegment("W/A/S/D", ConsoleColor.Cyan), new ColorSegment(" 이동   ", ConsoleColor.DarkGray),
                new ColorSegment("E", ConsoleColor.Green), new ColorSegment(" 접속   ", ConsoleColor.DarkGray),
                new ColorSegment("I", ConsoleColor.Yellow), new ColorSegment(" 인벤토리   ", ConsoleColor.DarkGray),
                new ColorSegment("C", ConsoleColor.Magenta), new ColorSegment(" 상태창   ", ConsoleColor.DarkGray),
                new ColorSegment("X", ConsoleColor.Cyan), new ColorSegment(" 스캔   ", ConsoleColor.DarkGray),
                new ColorSegment("H", ConsoleColor.Cyan), new ColorSegment(" 도움말   ", ConsoleColor.DarkGray),
                new ColorSegment("Q", ConsoleColor.Red), new ColorSegment(" 게임종료", ConsoleColor.DarkGray));
        }

        private string GetNodeLabel(VirusExe.SystemBreach.DataGrid.NodeType type)
        {
            if (type == VirusExe.SystemBreach.DataGrid.NodeType.Start) return "STR";
            if (type == VirusExe.SystemBreach.DataGrid.NodeType.Security) return "SEC";
            if (type == VirusExe.SystemBreach.DataGrid.NodeType.Firewall) return "FW";
            if (type == VirusExe.SystemBreach.DataGrid.NodeType.Shop) return "SHP";
            if (type == VirusExe.SystemBreach.DataGrid.NodeType.Mutation) return "MUT";
            if (type == VirusExe.SystemBreach.DataGrid.NodeType.Event) return "EVT";
            if (type == VirusExe.SystemBreach.DataGrid.NodeType.DataCache) return "DAT";
            if (type == VirusExe.SystemBreach.DataGrid.NodeType.Boss) return "BOS";
            return "EMP";
        }

        private void WriteHeader(string title)
        {
            SetColor(GetBattleFrameColor());
            Console.Write(ApplyBattleGlitch("╔" + new string('═', InnerWidth) + "╗"));
            ClearPhysicalLineRemainder(); // 오른쪽 잔상 제거
            Console.WriteLine();

            Console.Write(ApplyBattleGlitch("║"));
            WriteColored(TextUtil.Fit(" " + title, InnerWidth), GetBattleHeaderTextColor());
            SetColor(GetBattleFrameColor());
            Console.Write(ApplyBattleGlitch("║"));
            ClearPhysicalLineRemainder(); // 오른쪽 잔상 제거
            Console.WriteLine();

            Console.Write(ApplyBattleGlitch("╠" + new string('═', InnerWidth) + "╣"));
            ClearPhysicalLineRemainder(); // 오른쪽 잔상 제거
            Console.WriteLine();
            Console.ResetColor();
        }

        private void WriteSeparator()
        {
            if (modalRedirectActive) // 모달 출력 중인지 체크
            {
                WriteModalSeparator(); // 모달 구분선 출력
                return;
            }

            SetColor(GetBattleFrameColor());
            Console.Write(ApplyBattleGlitch("╠" + new string('═', InnerWidth) + "╣"));
            ClearPhysicalLineRemainder(); // 오른쪽 잔상 제거
            Console.WriteLine();
            Console.ResetColor();
        }

        private void WriteFooter()
        {
            if (modalRedirectActive) // 모달 출력 중인지 체크
            {
                EndModalRedirect(); // 모달 종료
                return;
            }

            SetColor(GetBattleFrameColor());
            Console.Write(ApplyBattleGlitch("╚" + new string('═', InnerWidth) + "╝"));
            ClearPhysicalLineRemainder(); // 오른쪽 잔상 제거
            Console.WriteLine();
            Console.ResetColor();
        }

        private void WriteEmptyLine()
        {
            if (modalRedirectActive) // 모달 출력 중인지 체크
            {
                WriteModalEmptyLine(); // 모달 빈 줄
                return;
            }

            SetColor(GetBattleFrameColor());
            Console.Write(ApplyBattleGlitch("║"));
            Console.ResetColor();
            Console.Write(TextUtil.Fit(string.Empty, InnerWidth));
            SetColor(GetBattleFrameColor());
            Console.Write(ApplyBattleGlitch("║"));
            Console.ResetColor();
            ClearPhysicalLineRemainder(); // 오른쪽 잔상 제거
            Console.WriteLine();
        }

        private void WriteLine(string text, ConsoleColor color)
        {
            if (modalRedirectActive) // 모달 출력 중인지 체크
            {
                WriteModalTextLine(text, color); // 모달 줄 출력
                return;
            }

            WriteSegmentsLine(new ColorSegment(TextUtil.Fit(text, InnerWidth), color));
        }

        private void WriteSegmentsLine(params ColorSegment[] segments)
        {
            List<ColorSegment> list = new List<ColorSegment>();
            for (int i = 0; i < segments.Length; i++) // 배열을 리스트로 변환
            {
                list.Add(segments[i]);
            }

            WriteSegmentsLine(list);
        }

        private void WriteSegmentsLine(List<ColorSegment> segments)
        {
            if (modalRedirectActive) // 모달 출력 중인지 체크
            {
                WriteModalSegmentsLine(segments); // 모달 세그먼트 출력
                return;
            }

            SetColor(GetBattleFrameColor());
            Console.Write(ApplyBattleGlitch("║"));
            Console.ResetColor();

            int width = WriteSegments(segments, InnerWidth); // 출력 폭 계산
            int remain = InnerWidth - width; // 남은 폭 계산

            if (remain > 0) // 오른쪽 여백 체크
            {
                Console.Write(new string(' ', remain));
            }

            SetColor(GetBattleFrameColor());
            Console.Write(ApplyBattleGlitch("║"));
            Console.ResetColor();
            ClearPhysicalLineRemainder(); // 오른쪽 잔상 제거
            Console.WriteLine();
        }

        private void WriteCentered(string text, ConsoleColor color)
        {
            if (modalRedirectActive) // 모달 출력 중인지 체크
            {
                WriteModalCentered(text, color); // 모달 중앙 출력
                return;
            }

            int width = TextUtil.GetDisplayWidth(text); // 텍스트 폭 계산
            int left = Math.Max(0, (InnerWidth - width) / 2); // 중앙 위치 계산
            WriteLine(TextUtil.Fit(new string(' ', left) + text, InnerWidth), color);
        }

        private void WriteCenteredSegments(List<ColorSegment> segments)
        {
            if (modalRedirectActive) // 모달 출력 중인지 체크
            {
                int modalWidth = GetSegmentsWidth(segments); // 모달 세그먼트 폭
                int modalLeftPadding = Math.Max(0, (modalInnerWidth - modalWidth) / 2); // 중앙 여백
                List<ColorSegment> modalCentered = new List<ColorSegment>();
                if (modalLeftPadding > 0) modalCentered.Add(new ColorSegment(new string(' ', modalLeftPadding), ConsoleColor.DarkGray)); // 좌측 여백
                modalCentered.AddRange(segments);
                WriteModalSegmentsLine(modalCentered); // 모달 중앙 출력
                return;
            }

            int width = GetSegmentsWidth(segments); // 세그먼트 폭 계산
            int left = Math.Max(0, (InnerWidth - width) / 2); // 중앙 위치 계산
            List<ColorSegment> centered = new List<ColorSegment>();

            if (left > 0) centered.Add(new ColorSegment(new string(' ', left), ConsoleColor.DarkGray)); // 좌측 여백
            centered.AddRange(segments);
            WriteSegmentsLine(centered);
        }

        private int WriteSegments(List<ColorSegment> segments, int maxWidth)
        {
            int width = 0;

            for (int i = 0; i < segments.Count; i++) // 세그먼트 출력
            {
                ColorSegment segment = segments[i];
                if (string.IsNullOrEmpty(segment.Text)) continue; // 빈 텍스트 제외

                string text = segment.Text;
                int textWidth = TextUtil.GetDisplayWidth(text);

                if (width + textWidth > maxWidth) // 박스 폭 초과 체크
                {
                    text = TextUtil.Fit(text, maxWidth - width);
                    textWidth = TextUtil.GetDisplayWidth(text);
                }

                WriteColored(text, segment.Foreground, segment.Background);
                width += textWidth;

                if (width >= maxWidth) break; // 남은 공간 없음
            }

            Console.ResetColor();
            return width;
        }

        private int GetSegmentsWidth(List<ColorSegment> segments)
        {
            int width = 0;

            for (int i = 0; i < segments.Count; i++) // 폭 합산
            {
                width += TextUtil.GetDisplayWidth(segments[i].Text);
            }

            return width;
        }

        private void WriteColored(string text, ConsoleColor foreground)
        {
            WriteColored(text, foreground, null);
        }

        private void WriteColored(string text, ConsoleColor foreground, ConsoleColor? background)
        {
            SetColor(foreground);
            if (background.HasValue) Console.BackgroundColor = background.Value; // 배경색 적용
            Console.Write(suppressBattleTextGlitch ? text : ApplyBattleGlitch(text)); // 보호 텍스트는 글리치 제외
            Console.ResetColor();
        }


        private ConsoleColor GetBattleFrameColor()
        {
            if (renderBossUiPhase >= 3) // 3페이즈 다크 UI 프레임
            {
                return battleFrame % 4 == 0 ? ConsoleColor.DarkRed : ConsoleColor.DarkGray;
            }

            if (renderBossUiPhase == 2) // 2페이즈 UI 지직임
            {
                return battleFrame % 5 == 0 ? ConsoleColor.Gray : ConsoleColor.Cyan;
            }

            return ConsoleColor.Cyan; // 기본 UI 프레임
        }

        private ConsoleColor GetBattleHeaderTextColor()
        {
            if (renderBossUiPhase >= 3) return ConsoleColor.Gray; // 3페이즈 제목 암전
            if (renderBossUiPhase == 2 && battleFrame % 4 == 0) return ConsoleColor.White; // 2페이즈 제목 점멸
            return ConsoleColor.Cyan; // 기본 제목 색상
        }

        private int GetBattleGlitchChance()
        {
            int chance = renderBattleGlitch ? 18 : 0; // 기존 피격 글리치

            if (renderBossUiPhase == 2) chance = Math.Max(chance, 7); // 2페이즈 상시 약한 글리치
            if (renderBossUiPhase >= 3) chance = Math.Max(chance, 18); // 3페이즈 상시 강한 글리치
            if (renderBossUiPhase >= 3 && renderBattleGlitch) chance = 32; // 3페이즈 피격 중 극대화

            return chance;
        }

        private string ApplyBattleGlitch(string text)
        {
            int chance = GetBattleGlitchChance(); // 현재 전투 글리치 강도

            if (chance <= 0 || string.IsNullOrEmpty(text)) // 글리치 필요 여부 체크
            {
                return text;
            }

            char[] chars = text.ToCharArray(); // 글리치 처리용 문자 배열

            for (int i = 0; i < chars.Length; i++) // 문자 단위 랜덤 파손
            {
                if (chars[i] == ' ') continue; // 공백 폭 유지
                if (glitchRandom.Next(0, 100) >= chance) continue; // 페이즈별 파손 강도

                chars[i] = GetBattleGlitchChar(chars[i]); // 같은 폭 문자로 치환
            }

            return new string(chars);
        }

        private char GetBattleGlitchChar(char value)
        {
            if (value == '═') return glitchRandom.Next(0, 2) == 0 ? '▓' : '╬'; // 가로 프레임 파손
            if (value == '║') return glitchRandom.Next(0, 2) == 0 ? '▓' : '┃'; // 세로 프레임 파손
            if (value == '╔' || value == '╗' || value == '╚' || value == '╝' || value == '╠' || value == '╣') return '╬'; // 모서리 파손
            if (value == '█') return '▓'; // HEALTH 바 파손
            if (value == '░') return '▒'; // 빈 게이지 파손
            if (value == 'A') return '@';
            if (value == 'E') return '3';
            if (value == 'I') return '!';
            if (value == 'O') return '0';
            if (value == 'S') return '$';
            if (value == 'T') return '+';
            if (value == 'H') return '#';
            if (value == 'R') return 'Я';
            if (value == 'a') return '@';
            if (value == 'e') return '3';
            if (value == 'i') return '!';
            if (value == 'o') return '0';
            if (value == 's') return '$';

            return glitchRandom.Next(0, 3) == 0 ? '#' : glitchRandom.Next(0, 2) == 0 ? '?' : '!'; // 기본 파손 문자
        }

        private void SetColor(ConsoleColor foreground)
        {
            Console.ForegroundColor = foreground;
        }


        private void HideCursor()
        {
            try // 커서 제어 실패 가능
            {
                Console.CursorVisible = false; // 커서 숨김

                int x = Console.WindowLeft + Console.WindowWidth - 2; // 우측 끝 보정
                int y = Console.WindowTop + Console.WindowHeight - 1; // 하단 끝 보정

                x = Math.Max(0, Math.Min(x, Console.BufferWidth - 1)); // X 범위 제한
                y = Math.Max(0, Math.Min(y, Console.BufferHeight - 1)); // Y 범위 제한

                Console.SetCursorPosition(x, y); // 커서 하단 유배
            }
            catch
            {
            }
        }

        private void ClearPhysicalLineRemainder()
        {
            int remain = Console.BufferWidth - Console.CursorLeft - 1; // 현재 줄 잔여폭 계산
            if (remain > 0) Console.Write(new string(' ', remain)); // 이전 프레임 잔상 제거
        }

        private void ResetRenderCursor()
        {
            Console.SetCursorPosition(0, 0); // 첫 위치 이동
        }

        private void ClearRenderTail()
        {
            try // 콘솔 위치 제어 실패 가능
            {
                int renderBottom = Console.CursorTop; // 이번 렌더 종료 줄
                int clearStart = renderBottom; // 잔상 제거 시작 줄
                int clearEnd = Math.Min(lastRenderedBottom, Console.BufferHeight - 1); // 이전 렌더 끝 줄

                if (clearEnd > clearStart) // 이전 화면 잔상 존재 체크
                {
                    int restoreLeft = Console.CursorLeft; // 현재 X 저장
                    int restoreTop = Console.CursorTop; // 현재 Y 저장

                    for (int y = clearStart; y <= clearEnd; y++) // 남은 잔상 줄 제거
                    {
                        Console.SetCursorPosition(0, y); // 제거할 줄 이동
                        Console.Write(new string(' ', Math.Max(0, Console.BufferWidth - 1))); // 한 줄 덮어쓰기
                    }

                    restoreLeft = Math.Min(restoreLeft, Console.BufferWidth - 1); // X 범위 보정
                    restoreTop = Math.Min(restoreTop, Console.BufferHeight - 1); // Y 범위 보정

                    Console.SetCursorPosition(restoreLeft, restoreTop); // 커서 복구
                }

                lastRenderedBottom = renderBottom; // 이번 렌더 끝 위치 저장
            }
            catch
            {
                // 콘솔 환경에서 커서 제어 실패 시 무시
            }
        }
    }
}
