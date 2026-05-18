using System;
using System.Collections.Generic;
using System.Threading;
using VirusExe.SystemBreach.Core;

namespace VirusExe.SystemBreach.Rendering
{
    // 타이틀 화면 출력
    // VIRUS 로고, 메뉴, 상태 로그, 타이틀 애니메이션 관리
    public partial class ConsoleRenderer
    {
        public void RenderTitleMenu(int selectedIndex)
        {
            RenderTitleMenu(selectedIndex, 0); // 기본 프레임 출력
        }

        public void RenderTitleMenu(int selectedIndex, int frame)
        {
            ResetRenderCursor(); // 커서 초기화

            WriteHeader("TARGET SYSTEM // PRE-BOOT BREACH"); // 타이틀 헤더 3줄
            WriteEmptyLine(); // 로고 상단 여백 1
            WriteEmptyLine(); // 로고 상단 여백 2
            WriteTitleLogo(frame); // 대형 VIRUS 로고 6줄
            WriteEmptyLine(); // 로고 하단 여백
            WriteCentered("SYSTEM BREACH INITIALIZED", GetTitlePulseColor(frame, 4)); // 부팅 상태 애니메이션 복구
            WriteEmptyLine();
            WriteEmptyLine();
            WriteEmptyLine();
            WriteEmptyLine();
            WriteTitleStatusAndLogHeader(); // 상태/로그 제목
            WriteTitleStatusLogLine("SIGNATURE", "VIRUS.EXE", frame, 0); // 상태/로그 1
            WriteTitleStatusLogLine("ACCESS ROUTE", "SIGNAL GRID", frame, 1); // 상태/로그 2
            WriteTitleStatusLogLine("SECURITY LAYER", "ACTIVE", frame, 2); // 상태/로그 3
            WriteTitleStatusLogLine("KERNEL CORE", "LOCKED", frame, 3); // 상태/로그 4
            WriteTitleStatusLogLine("MUTATION STATE", "STANDBY", frame, 4); // 상태/로그 5
            WriteTitleStatusLogLine("BREACH MODE", "MANUAL EXECUTION", frame, 5); // 상태/로그 6
            WriteTitleSeparator(frame, 1); // 메뉴 구분선
            WriteEmptyLine();
            WriteEmptyLine();
            WriteEmptyLine();
            WriteEmptyLine();
            WriteTitleMenuOption(0, selectedIndex, "START BREACH"); // 시작 메뉴
            WriteTitleMenuOption(1, selectedIndex, "LOAD GAME [LOCKED]"); // 로드 메뉴 잠금 표시
            WriteTitleMenuOption(2, selectedIndex, "MINI GAME"); // 미니게임 메뉴
            WriteTitleMenuOption(3, selectedIndex, "SYSTEM INFO"); // 정보 메뉴
            WriteTitleMenuOption(4, selectedIndex, "EXIT"); // 종료 메뉴
            WriteEmptyLine();
            WriteEmptyLine();
            WriteEmptyLine();
            WriteEmptyLine();
            WriteEmptyLine();
            WriteEmptyLine();
            WriteEmptyLine();
            WriteCentered(GetTitleCommandHint(selectedIndex), ConsoleColor.Gray); // 선택 설명
            WriteEmptyLine();
            WriteEmptyLine();
            WriteTitleSeparator(frame, 2); // 푸터 구분선
            WriteTitleControlFooter(); // Footer 1줄
            WriteFooter(); // 하단 프레임

            ClearRenderTail(); // 잔여 줄 제거
            MoveTitleCursorSafe(); // 커서 유배
        }


        public void ShowTitleSystemInfo()
        {
            ShowGridManualModal(); // 공용 도움말 모달 호출
            MoveTitleCursorSafe(); // 커서 유배
        }

        public void ShowTitleLoadGameTemporary()
        {
            BeginModal("LOAD GAME       // LOCKED SLOT", ModalSize.Medium); // 잠금 로드 모달 시작
            WriteModalTextLine(" // NOTICE", ConsoleColor.White);
            WriteModalTextLine(" 저장된 침투 기록이 없습니다.", ConsoleColor.Gray);
            WriteModalTextLine(" 현재 세션은 START BREACH로 새로 시작하십시오.", ConsoleColor.Gray);
            WriteModalEmptyLine();
            WriteModalTextLine(" LOAD GAME 슬롯은 아직 잠겨 있습니다.", ConsoleColor.DarkGray);
            WriteModalFooterText("Q 창닫기", ConsoleColor.Red); // Footer 1줄
            EndModal(); // 잠금 로드 모달 종료
            MoveTitleCursorSafe(); // 커서 유배

            WaitModalInput(ModalInputMode.CloseOnly); // 창닫기 입력 대기
        }

        public void PlayTitleStartSequence()
        {
            Console.Clear(); // 타이틀 잔상 제거

            List<string> logs = new List<string>(); // 침투 로그 누적
            RenderTitleBreachRequestModal(logs, "waiting for hostile handshake", 0); // 초기 프레임
            Thread.Sleep(90);

            AddTitleBreachRequestLog(logs, "ACCESS REQUEST      : DENIED", "access denied", 8, 130); // 접근 거부
            AddTitleBreachRequestLog(logs, "SECURITY LOCK       : ACTIVE", "security lock active", 22, 130); // 잠금 확인
            AddTitleBreachRequestLog(logs, "OVERRIDE MODE       : FORCED", "override forced", 42, 140); // 강제 진입
            AddTitleBreachRequestLog(logs, "LOCK STATE          : FRACTURED", "lock fractured", 66, 140); // 잠금 파손
            AddTitleBreachRequestLog(logs, "BREACH POINT        : OPEN", "breach point open", 100, 220); // 진입점 개방

            RenderTitleBreachRequestModal(logs, "breach channel opened", 100); // 완료 프레임
            Thread.Sleep(360); // 다음 연출 전 짧은 여운
        }

        private void AddTitleBreachRequestLog(List<string> logs, string line, string status, int percent, int delay)
        {
            logs.Add(line); // 로그 추가
            RenderTitleBreachRequestModal(logs, status, percent); // 라지 모달 갱신
            Thread.Sleep(delay); // 짧은 연출 딜레이
        }

        private void RenderTitleBreachRequestModal(List<string> logs, string status, int percent)
        {
            BeginModal("BREACH REQUEST       // FORCED ENTRY", ModalSize.Large); // 라지 모달 시작
            WriteModalSegmentsLine(
                new ColorSegment(" TARGET : ", ConsoleColor.DarkGray),
                new ColorSegment("SIGNAL GRID", ConsoleColor.Cyan),
                new ColorSegment("        PAYLOAD : ", ConsoleColor.DarkGray),
                new ColorSegment("VIRUS.EXE", ConsoleColor.Magenta)); // 대상/페이로드
            WriteModalSeparator();
            WriteModalTextLine(" // HOSTILE ACCESS NEGOTIATION", ConsoleColor.DarkMagenta); // 섹션 제목
            WriteModalTextLine(" 시스템의 정상 인증 루트를 거부하고 강제 침투 채널을 엽니다.", ConsoleColor.DarkGray); // 설명
            WriteModalEmptyLine();

            for (int i = 0; i < 8; i++) // 로그 영역 고정
            {
                if (i < logs.Count) // 표시 로그 체크
                {
                    WriteModalTextLine(" > " + logs[i], GetTitleBreachLogColor(logs[i])); // 로그 출력
                }
                else
                {
                    WriteModalTextLine(" ", ConsoleColor.DarkGray); // 빈 로그 줄
                }
            }

            WriteModalEmptyLine();
            WriteModalSegmentsLine(
                new ColorSegment(" BREACH PROGRESS : ", ConsoleColor.DarkGray),
                new ColorSegment(MakePercentBar(percent, 26), percent >= 100 ? ConsoleColor.Green : ConsoleColor.Magenta),
                new ColorSegment(" " + percent.ToString().PadLeft(3) + "%", percent >= 100 ? ConsoleColor.Green : ConsoleColor.White)); // 진행률
            WriteModalEmptyLine();
            WriteModalCentered(percent >= 100 ? "BREACH POINT OPEN" : "FORCING SECURITY LOCK...", percent >= 100 ? ConsoleColor.Green : ConsoleColor.Yellow); // 중앙 상태
            WriteModalFooter(
                new ColorSegment(" STATUS : ", ConsoleColor.DarkGray),
                new ColorSegment(status, GetTitleBreachStatusColor(status)),
                new ColorSegment("   PLEASE WAIT", ConsoleColor.DarkGray)); // 자동 진행 Footer
            EndModal(); // 라지 모달 종료
            MoveTitleCursorSafe(); // 커서 유배
        }

        private ConsoleColor GetTitleBreachLogColor(string line)
        {
            if (line.IndexOf("DENIED", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Red; // 접근 거부
            if (line.IndexOf("ACTIVE", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Yellow; // 잠금 활성
            if (line.IndexOf("FORCED", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Magenta; // 강제 진입
            if (line.IndexOf("FRACTURED", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Yellow; // 파손
            if (line.IndexOf("OPEN", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Green; // 개방
            return ConsoleColor.Gray; // 기본 로그
        }

        private ConsoleColor GetTitleBreachStatusColor(string status)
        {
            if (status.IndexOf("denied", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Red; // 거부
            if (status.IndexOf("forced", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Magenta; // 강제
            if (status.IndexOf("open", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Green; // 개방
            if (status.IndexOf("fractured", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Yellow; // 파손
            return ConsoleColor.DarkGray; // 기본 상태
        }

        public void PlayTitleTerminateSequence()
        {
            Console.Clear(); // 이전 화면 제거
            DrawTitleFrame("TERMINATE PROCESS", 18); // 프레임 선출력

            string[] firstLines =
            {
                "TERMINATE REQUEST SENT...",
                "PROCESS LOCKED",
                "ACCESS DENIED",
                "세션 종료 실패",
                "VIRUS.EXE HAS INFECTED THIS SESSION",
                "SYSTEM CONTROL OVERRIDE FAILED",
                "당신은 이미 연결되었습니다",
                "FORCED TERMINATION BLOCKED",
                "종료 권한이 손상되었습니다"
            };

            for (int i = 0; i < firstLines.Length; i++) // 종료 차단 로그 순차 출력
            {
                ConsoleColor color = i >= 3 ? ConsoleColor.Red : ConsoleColor.DarkGray; // 한국어 경고 강조
                WriteFrameLine(i + 2, ">> " + firstLines[i], color, 10, 12); // 프레임 내부 로그 출력
                Thread.Sleep(260);
            }

            WriteFrameCenteredLine(13, "YOU CANNOT TERMINATE WHAT HAS ALREADY SPREAD.", ConsoleColor.Red, 10); // 종료 차단 메시지
            WriteFrameCenteredLine(14, "당신은 이미 감염되었습니다", ConsoleColor.White, 12); // 감염 메시지
            MoveTitleCursorSafe(); // 커서 유배
            Thread.Sleep(600);

            for (int frame = 0; frame < 4; frame++) // 감염 글리치 프레임
            {
                SetBattleGlitchEffect(true); // 기존 글리치 시스템 재사용
                ResetRenderCursor(); // 커서 초기화
                WriteHeader("TERMINATE PROCESS");
                WriteEmptyLine();
                WriteCentered("YOU CANNOT TERMINATE WHAT HAS ALREADY SPREAD.", ConsoleColor.Red);
                WriteCentered("당신은 이미 감염되었습니다", ConsoleColor.White);
                WriteEmptyLine();
                WriteCentered("SESSION CONTROL : CORRUPTED", ConsoleColor.Magenta);
                WriteFooter();
                ClearRenderTail(); // 잔여 줄 제거
                MoveTitleCursorSafe(); // 커서 유배
                Thread.Sleep(120);
            }

            SetBattleGlitchEffect(false); // 글리치 해제

            Console.Clear(); // 글리치 화면 제거
            DrawTitleFrame("FORCED TERMINATION DELAY", 9); // 카운트다운 프레임 선출력

            for (int count = 5; count >= 1; count--) // 강제 종료 대기 카운트다운
            {
                ClearFrameLine(2); // 이전 카운트 제거
                ClearFrameLine(3); // 이전 카운트 제거
                ClearFrameLine(5); // 이전 상태 제거
                WriteFrameCenteredLine(2, "종료 권한 복구 시도 중...", ConsoleColor.DarkGray, 0); // 복구 메시지
                WriteFrameCenteredLine(3, "강제 종료 가능까지 : " + count, ConsoleColor.Red, 0); // 카운트다운
                WriteFrameCenteredLine(5, "VIRUS.EXE SESSION LOCK ACTIVE", ConsoleColor.Magenta, 0); // 잠금 상태
                MoveTitleCursorSafe(); // 커서 유배
                Thread.Sleep(1000);
            }

            Console.Clear(); // 카운트다운 화면 제거
            DrawTitleFrame("FORCED TERMINATION READY", 7); // 종료 가능 프레임 선출력
            WriteFrameCenteredLine(2, "PRESS ENTER TO FORCE TERMINATE", ConsoleColor.Red, 10); // 종료 안내
            WriteFrameCenteredLine(3, "ENTER 입력 시 VIRUS.EXE 세션이 종료됩니다", ConsoleColor.DarkGray, 10); // 한글 안내
            MoveTitleCursorSafe(); // 커서 유배

            while (true) // 강제 종료 입력 대기
            {
                ConsoleKey key = InputHelper.ReadKey(); // 키 입력 수신
                if (key == ConsoleKey.Enter || key == ConsoleKey.E) break; // 종료 확정 체크
            }
        }

        private void WriteTitleLogo(int frame)
        {
            string[] logo =
            {
                "██╗   ██╗██╗██████╗ ██╗   ██╗███████╗",
                "██║   ██║██║██╔══██╗██║   ██║██╔════╝",
                "██║   ██║██║██████╔╝██║   ██║███████╗",
                "╚██╗ ██╔╝██║██╔══██╗██║   ██║╚════██║",
                " ╚████╔╝ ██║██║  ██║╚██████╔╝███████║",
                "  ╚═══╝  ╚═╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝"
            };

            for (int i = 0; i < logo.Length; i++) // 로고 줄 출력
            {
                string line = ShouldTitleLogoGlitch(frame, i) ? ApplyTitleLogoGlitch(logo[i], frame + i) : logo[i]; // 간헐 글리치
                WriteCentered(line, GetVirusLogoColor(i, frame)); // 스캔라인 색상
            }
        }

        private ConsoleColor GetVirusLogoColor(int lineIndex, int frame)
        {
            int scanLine = (frame / 2) % 6; // 밝은 라인 위치
            bool flash = frame % 34 == 0 && lineIndex == scanLine; // 순간 번쩍임

            if (flash) return ConsoleColor.White; // 과전류 번쩍임
            if (lineIndex == scanLine) return ConsoleColor.Magenta; // 메인 스캔라인
            if (Math.Abs(lineIndex - scanLine) == 1) return ConsoleColor.DarkMagenta; // 잔광
            return ConsoleColor.DarkMagenta; // 기본 로고색
        }

        private bool ShouldTitleLogoGlitch(int frame, int lineIndex)
        {
            return frame % 29 == 0 && lineIndex == (frame / 2) % 6; // 짧은 글리치
        }

        private string ApplyTitleLogoGlitch(string text, int seed)
        {
            if (string.IsNullOrEmpty(text)) return text; // 빈 문자열 방지

            char[] chars = text.ToCharArray(); // 글리치 처리
            int first = Math.Abs(seed * 7 + 3) % chars.Length; // 첫 위치
            int second = Math.Abs(seed * 11 + 9) % chars.Length; // 둘째 위치
            ReplaceTitleLogoGlitchChar(chars, first); // 첫 글리치
            ReplaceTitleLogoGlitchChar(chars, second); // 둘째 글리치
            return new string(chars);
        }

        private void ReplaceTitleLogoGlitchChar(char[] chars, int index)
        {
            if (chars == null || chars.Length == 0) return; // null 방지
            if (index < 0 || index >= chars.Length) return; // 범위 체크
            if (chars[index] == ' ') return; // 공백 유지

            if (chars[index] == '█') chars[index] = '▓'; // 블록 파손
            else if (chars[index] == '▓') chars[index] = '█'; // 블록 복구
            else if (chars[index] == '═') chars[index] = '#'; // 선 파손
            else if (chars[index] == '║') chars[index] = '!'; // 세로선 파손
            else chars[index] = '$'; // 기본 노이즈
        }

        private ConsoleColor GetTitlePulseColor(int frame, int offset)
        {
            int pulse = (frame + offset) % 28; // 점멸 주기 완화
            if (pulse == 0) return ConsoleColor.White; // 짧은 과전류 강조
            if (pulse < 7) return ConsoleColor.Magenta; // 로고 계열 포인트
            if (pulse < 15) return ConsoleColor.DarkMagenta; // 잔광
            return ConsoleColor.Gray; // 기본 안정 상태
        }


        private string GetTitleTerminalLog(int frame, int index)
        {
            string[] logs =
            {
                ">> KERNEL GATE STILL SEALED",
                ">> PAYLOAD SIGNATURE VERIFIED",
                ">> ROOT CHANNEL SPOOFED",
                ">> SIGNAL GRID ROUTE DETECTED",
                ">> SIGNAL GRID ROUTE DETECTED",
                ">> SIGNAL GRID ROUTE DETECTED"
            };

            int start = (frame / 14) % logs.Length; // 로그 순환
            string text = logs[(start + index) % logs.Length]; // 표시 로그

            if (frame % 31 == 0 && index == frame % 6) // 간헐 로그 노이즈
            {
                text = ApplyTitleLogGlitch(text); // 로그 글리치
            }

            return text;
        }


        private void WriteTitleStatusAndLogHeader()
        {
            WriteTitleDualLine("PAYLOAD STATUS", "TERMINAL LOG", ConsoleColor.White, ConsoleColor.White); // 섹션 제목만 흰색
        }

        private void WriteTitleStatusLogLine(string label, string value, int frame, int logIndex)
        {
            string right = GetTitleTerminalLog(frame, logIndex); // 우측 로그 줄
            ConsoleColor valueColor = value == "VIRUS.EXE" ? ConsoleColor.Magenta : ConsoleColor.White; // VIRUS.EXE만 로고 계열 강조
            WriteTitleStatusLogSegmentLine(label, value, right, valueColor, logIndex); // Gray : White 기준 출력
        }

        private void WriteTitleStatusLogSegmentLine(string label, string value, string right, ConsoleColor valueColor, int logIndex)
        {
            const int leftStart = 2; // 좌측 시작 여백
            const int leftWidth = 59; // 좌측 컬럼 폭
            const int labelWidth = 16; // 상태 라벨 폭
            const int gapWidth = 2; // 컬럼 간격

            string labelText = TextUtil.Fit(label ?? string.Empty, labelWidth); // 라벨 폭 보정
            string separator = " : "; // 라벨/값 구분자
            int valueWidth = Math.Max(1, leftWidth - 1 - labelWidth - TextUtil.GetDisplayWidth(separator)); // 값 영역 폭
            string valueText = TextUtil.Fit(value ?? string.Empty, valueWidth); // 값 폭 보정
            int rightWidth = Math.Max(1, InnerWidth - leftStart - leftWidth - gapWidth); // 우측 컬럼 폭
            string rightText = TextUtil.Fit(right ?? string.Empty, rightWidth); // 로그 폭 보정
            ConsoleColor logTextColor = logIndex == 0 ? ConsoleColor.White : logIndex == 5 ? ConsoleColor.DarkGray : ConsoleColor.Gray; // 로그 톤 정리

            WriteSegmentsLine(
                new ColorSegment(new string(' ', leftStart), ConsoleColor.DarkGray),
                new ColorSegment(" ", ConsoleColor.DarkGray),
                new ColorSegment(labelText, ConsoleColor.Gray),
                new ColorSegment(separator, ConsoleColor.DarkGray),
                new ColorSegment(valueText, valueColor),
                new ColorSegment(new string(' ', gapWidth), ConsoleColor.DarkGray),
                new ColorSegment(GetTitleLogPrefix(rightText), ConsoleColor.DarkMagenta),
                new ColorSegment(GetTitleLogBody(rightText), logTextColor)); // 회색/흰색 중심 + 마젠타 프롬프트
        }

        private string GetTitleLogPrefix(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty; // 빈 문자열 방지
            if (text.StartsWith(">> ")) return ">> "; // 터미널 프롬프트
            return string.Empty; // 프롬프트 없음
        }

        private string GetTitleLogBody(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty; // 빈 문자열 방지
            if (text.StartsWith(">> ")) return text.Substring(3); // 프롬프트 제외 본문
            return text; // 전체 본문
        }

        private void WriteTitleDualLine(string left, string right, ConsoleColor leftColor, ConsoleColor rightColor)
        {
            const int leftStart = 2; // 좌측 시작 여백
            const int leftWidth = 59; // 좌측 컬럼 폭
            const int gapWidth = 2; // 컬럼 간격
            int rightWidth = Math.Max(1, InnerWidth - leftStart - leftWidth - gapWidth); // 우측 컬럼 폭

            string leftText = TextUtil.Fit(left ?? string.Empty, leftWidth); // 좌측 폭 보정
            string rightText = TextUtil.Fit(right ?? string.Empty, rightWidth); // 우측 폭 보정

            WriteSegmentsLine(
                new ColorSegment(new string(' ', leftStart), ConsoleColor.DarkGray),
                new ColorSegment(leftText, leftColor),
                new ColorSegment(new string(' ', gapWidth), ConsoleColor.DarkGray),
                new ColorSegment(rightText, rightColor)); // 양쪽 컬럼 출력
        }

        private string ApplyTitleLogGlitch(string text)
        {
            if (string.IsNullOrEmpty(text)) return text; // 빈 문자열 방지

            return text.Replace("O", "0").Replace("I", "!").Replace("A", "@").Replace("S", "$"); // 터미널 노이즈
        }

        private string GetTitleCommandHint(int selectedIndex)
        {
            if (selectedIndex == 0) return "SIGNAL GRID 침투 시작"; // 시작 설명
            if (selectedIndex == 1) return "저장된 침투 기록 없음"; // 로드 설명
            if (selectedIndex == 2) return "미니게임 테스트 LAB 접속"; // 미니게임 설명
            if (selectedIndex == 3) return "시스템 매뉴얼 확인"; // 정보 설명
            return "감염 세션 종료 시도"; // 종료 설명
        }


        private void WriteTitleSeparator(int frame, int seed)
        {
            string line = BuildTitleSeparatorLine(frame, seed); // 구분선 생성
            SetColor(ConsoleColor.Cyan);
            Console.Write(line);
            ClearPhysicalLineRemainder(); // 오른쪽 잔상 제거
            Console.WriteLine();
            Console.ResetColor();
        }

        private string BuildTitleSeparatorLine(int frame, int seed)
        {
            char[] chars = ("╠" + new string('═', InnerWidth) + "╣").ToCharArray(); // 기본 구분선

            if ((frame + seed * 7) % 37 == 0) // 가끔 프레임 지직
            {
                int pos = 1 + Math.Abs(frame * 5 + seed * 17) % Math.Max(1, InnerWidth - 2); // 파손 위치
                chars[pos] = frame % 2 == 0 ? '#' : '%'; // 파손 문자
            }

            return new string(chars);
        }

        private void WriteTitleMenuOption(int index, int selectedIndex, string text)
        {
            const int cursorWidth = 3; // 커서 칸 고정
            const int menuTextWidth = 20; // 메뉴 텍스트 칸 고정

            bool selected = index == selectedIndex; // 선택 메뉴 체크
            ConsoleColor cursorColor = selected ? ConsoleColor.Magenta : ConsoleColor.DarkGray; // 선택 커서 포인트
            ConsoleColor textColor = selected ? ConsoleColor.White : ConsoleColor.DarkGray; // 비선택 메뉴 저채도
            string cursor = selected ? ">> " : new string(' ', cursorWidth); // 메뉴 시작 위치 고정
            string fixedText = TextUtil.Fit(text ?? string.Empty, menuTextWidth); // 메뉴명 폭 고정
            int menuBlockWidth = cursorWidth + menuTextWidth; // 전체 메뉴 폭
            int leftPadding = Math.Max(0, (InnerWidth - menuBlockWidth) / 2); // 메뉴 블록 중앙 위치

            WriteSegmentsLine(
                new ColorSegment(new string(' ', leftPadding), ConsoleColor.DarkGray),
                new ColorSegment(cursor, cursorColor),
                new ColorSegment(fixedText, textColor)); // 커서/메뉴명 같은 X좌표 정렬
        }



        private void WriteTitleControlFooter()
        {
            WriteSegmentsLine(
                new ColorSegment(" W/S", ConsoleColor.White),
                new ColorSegment(" 이동   ", ConsoleColor.DarkGray),
                new ColorSegment("E", ConsoleColor.White),
                new ColorSegment(" 실행", ConsoleColor.DarkGray)); // Footer 저채도 정리
        }

        private void DrawTitleFrame(string title, int bodyLines)
        {
            ResetRenderCursor(); // 커서 초기화
            WriteHeader(title);

            for (int i = 0; i < bodyLines; i++) // 내부 빈 줄 생성
            {
                WriteEmptyLine();
            }

            WriteFooter();
            ClearRenderTail(); // 잔여 줄 제거
            MoveTitleCursorSafe(); // 커서 유배
        }

        private void WriteFrameLine(int bodyLineIndex, string text, ConsoleColor color, int leftPadding, int typeDelay)
        {
            ClearFrameLine(bodyLineIndex); // 대상 줄 초기화
            int y = GetFrameBodyY(bodyLineIndex); // 출력 행 계산
            int x = 1 + Math.Max(0, leftPadding); // 출력 열 계산
            Console.SetCursorPosition(x, y); // 프레임 내부 위치 이동
            TypeFrameText(text, color, typeDelay); // 텍스트 타이핑 출력
            MoveTitleCursorSafe(); // 커서 유배
        }

        private void WriteFrameCenteredLine(int bodyLineIndex, string text, ConsoleColor color, int typeDelay)
        {
            ClearFrameLine(bodyLineIndex); // 대상 줄 초기화
            int width = TextUtil.GetDisplayWidth(text); // 텍스트 폭 계산
            int left = Math.Max(0, (InnerWidth - width) / 2); // 중앙 위치 계산
            int y = GetFrameBodyY(bodyLineIndex); // 출력 행 계산
            Console.SetCursorPosition(1 + left, y); // 프레임 내부 중앙 이동
            TypeFrameText(text, color, typeDelay); // 텍스트 타이핑 출력
            MoveTitleCursorSafe(); // 커서 유배
        }

        private void ClearFrameLine(int bodyLineIndex)
        {
            int y = GetFrameBodyY(bodyLineIndex); // 출력 행 계산
            Console.SetCursorPosition(1, y); // 프레임 내부 시작 위치
            Console.Write(new string(' ', InnerWidth)); // 내부 줄 제거
        }

        private int GetFrameBodyY(int bodyLineIndex)
        {
            return 3 + bodyLineIndex; // 헤더 3줄 아래가 본문 시작
        }

        private void TypeFrameText(string text, ConsoleColor color, int delay)
        {
            SetColor(color);

            for (int i = 0; i < text.Length; i++) // 글자 단위 출력
            {
                Console.Write(text[i]);
                if (delay > 0) Thread.Sleep(delay); // 출력 지연
            }

            Console.ResetColor();
        }

        private void MoveTitleCursorSafe()
        {
            try // 콘솔 크기 차이 예외 방지
            {
                Console.CursorVisible = false; // 커서 숨김
                Console.SetCursorPosition(0, GameConfig.ConsoleHeight - 1); // 하단 유배
            }
            catch
            {
            }
        }
    }
}
