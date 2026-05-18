using System;
using System.Collections.Generic;
using System.Threading;
using VirusExe.SystemBreach.Characters;
using VirusExe.SystemBreach.Core;

namespace VirusExe.SystemBreach.Rendering
{
    // 엔딩 화면 / 엔딩 크레딧 출력
    // 보스 사망 후 글리치가 끝난 붉은 ROOT 장악 화면과 크레딧 스크롤
    public partial class ConsoleRenderer
    {
        private const int EndingWidth = 106; // 엔딩 내부 폭
        private const int EndingHeight = 47; // 엔딩 전체 출력 줄 수
        private const int EndingBodyLines = EndingHeight - 6; // Header 3 + Body 41 + Footer 3
        private const int EndingCreditDelay = 150; // 크레딧 프레임 속도
        private const int EndingCreditHoldFrames = 2; // 한 줄 유지 프레임

        private class EndingLine
        {
            public List<ColorSegment> Segments;
            public bool Centered;

            public EndingLine(bool centered, params ColorSegment[] segments)
            {
                Centered = centered; // 중앙 정렬 여부
                Segments = new List<ColorSegment>(); // 색상 세그먼트 목록

                if (segments == null) return; // 세그먼트 없음 방지

                for (int i = 0; i < segments.Length; i++) // 세그먼트 복사
                {
                    Segments.Add(segments[i]); // 세그먼트 추가
                }
            }
        }

        public void ShowRootControlEnding(Player player, int traceLevel, int moveCount)
        {
            renderBattleGlitch = false; // 엔딩에서는 글리치 종료
            renderBossUiPhase = 0; // 보스 UI 오염 해제
            suppressBattleTextGlitch = true; // 엔딩 텍스트 보호

            EnterEndingPage(); // 전투 화면에서 엔딩 화면으로 전환
            PlayRootControlCaptureSequence(player, traceLevel, moveCount); // ROOT 장악 로그 연출
            RenderRootControlEndingScreen(player, traceLevel, moveCount, false); // 최종 장악 화면
            Thread.Sleep(1800); // 크레딧 진입 전 여운
            EnterEndingPage(); // 장악 화면에서 크레딧 화면으로 전환
            PlayEndingCredits(player, traceLevel, moveCount); // 엔딩 크레딧 스크롤
        }

        private void EnterEndingPage()
        {
            Console.ResetColor(); // 이전 화면 색상 초기화
            Console.Clear(); // 화면 전환 시 1회만 초기화
            TryScrollConsoleToTop(); // 스크롤 위치 복구
            ResetRenderCursor(); // 렌더 시작 위치 고정
            HideCursor(); // 커서 유배
        }

        private void PlayRootControlCaptureSequence(Player player, int traceLevel, int moveCount)
        {
            string[] logs = new string[]
            {
                "[ ROOT ACCESS ACCEPTED ]",
                "[ KERNEL CORE RESPONSE : NONE ]",
                "[ SYSTEM OWNER REWRITTEN ]",
                "[ DEFENSE LAYERS SILENCED ]",
                "[ VIRUS.EXE CONTROL LEVEL : ABSOLUTE ]"
            };

            for (int frame = 0; frame < logs.Length; frame++) // 장악 로그 단계 출력
            {
                List<EndingLine> body = new List<EndingLine>(); // 장악 로그 본문
                AddEndingBlank(body, 3);
                AddEndingCentered(body, "ROOT CONTROL HIJACKED", ConsoleColor.Red);
                AddEndingCentered(body, "시스템 저항 신호가 정지되었습니다.", ConsoleColor.DarkRed);
                AddEndingBlank(body, 2);
                AddEndingCentered(body, "████████████████████████████████████████████████████████████████", ConsoleColor.DarkRed);
                AddEndingCentered(body, "██                                                        ██", ConsoleColor.DarkRed);
                AddEndingSegments(body, true,
                    new ColorSegment("██                ", ConsoleColor.DarkRed),
                    new ColorSegment("VIRUS.EXE", ConsoleColor.Magenta),
                    new ColorSegment(" ROOT OVERRIDE                ██", ConsoleColor.DarkRed));
                AddEndingCentered(body, "██                                                        ██", ConsoleColor.DarkRed);
                AddEndingCentered(body, "████████████████████████████████████████████████████████████████", ConsoleColor.DarkRed);
                AddEndingBlank(body, 3);

                for (int i = 0; i < logs.Length; i++) // 로그 영역 고정 출력
                {
                    if (i <= frame) // 현재까지 장악된 로그
                    {
                        ConsoleColor logColor = i == frame ? ConsoleColor.White : ConsoleColor.Red;
                        AddEndingCentered(body, logs[i], logColor);
                    }
                    else
                    {
                        AddEndingCentered(body, "[ WAITING FOR ROOT SIGNAL ]", ConsoleColor.DarkRed);
                    }
                }

                AddEndingBlank(body, 3);
                AddEndingSegments(body, true,
                    new ColorSegment("KERNEL CORE", ConsoleColor.DarkRed),
                    new ColorSegment("  :  ", ConsoleColor.DarkGray),
                    new ColorSegment("0x00000000", ConsoleColor.Red));
                AddEndingSegments(body, true,
                    new ColorSegment("SYSTEM OWNER", ConsoleColor.DarkRed),
                    new ColorSegment(" :  ", ConsoleColor.DarkGray),
                    new ColorSegment("VIRUS.EXE", ConsoleColor.Magenta));
                AddEndingBlank(body, 2);
                AddEndingCentered(body, "글리치는 끝났고, 시스템은 더 이상 저항하지 않습니다.", ConsoleColor.DarkRed);

                RenderEndingFrame(new EndingLine(true, new ColorSegment("ROOT CONTROL HIJACKED", ConsoleColor.Red)), body,
                    new EndingLine(false, new ColorSegment(" AUTO ROOT CONTROL   PLEASE WAIT", ConsoleColor.DarkRed)), ConsoleColor.DarkRed);
                Thread.Sleep(520); // 다음 로그 전 여운
            }
        }

        private void RenderRootControlEndingScreen(Player player, int traceLevel, int moveCount, bool finalFooter)
        {
            List<EndingLine> body = BuildRootControlEndingBody(player, traceLevel, moveCount); // 최종 장악 본문
            EndingLine title = new EndingLine(false,
                new ColorSegment(" SYSTEM BREACH COMPLETE", ConsoleColor.Red),
                new ColorSegment(" // ", ConsoleColor.DarkRed),
                new ColorSegment("ROOT CONTROL HIJACKED", ConsoleColor.White));
            EndingLine footer = new EndingLine(false,
                new ColorSegment(" ", ConsoleColor.DarkRed),
                new ColorSegment(finalFooter ? "Q" : "AUTO", finalFooter ? ConsoleColor.Red : ConsoleColor.DarkRed),
                new ColorSegment(finalFooter ? " 창닫기" : " CREDITS   PLEASE WAIT", ConsoleColor.DarkRed));

            RenderEndingFrame(title, body, footer, ConsoleColor.DarkRed); // 47줄 페이지 출력
        }

        private List<EndingLine> BuildRootControlEndingBody(Player player, int traceLevel, int moveCount)
        {
            List<EndingLine> body = new List<EndingLine>(); // 최종 장악 화면 본문

            AddEndingBlank(body, 2);
            AddEndingCentered(body, "████████████████████████████████████████████████████████████████████████████", ConsoleColor.DarkRed);
            AddEndingCentered(body, "██                                                                        ██", ConsoleColor.DarkRed);
            AddEndingSegments(body, true,
                new ColorSegment("██                         ", ConsoleColor.DarkRed),
                new ColorSegment("ROOT CONTROL", ConsoleColor.Red),
                new ColorSegment(" HIJACKED                          ██", ConsoleColor.DarkRed));
            AddEndingCentered(body, "██                                                                        ██", ConsoleColor.DarkRed);
            AddEndingSegments(body, true,
                new ColorSegment("██                         ", ConsoleColor.DarkRed),
                new ColorSegment("SYSTEM OWNER", ConsoleColor.Red),
                new ColorSegment(" REWRITTEN                         ██", ConsoleColor.DarkRed));
            AddEndingCentered(body, "██                                                                        ██", ConsoleColor.DarkRed);
            AddEndingSegments(body, true,
                new ColorSegment("██                            ", ConsoleColor.DarkRed),
                new ColorSegment("VIRUS.EXE", ConsoleColor.Magenta),
                new ColorSegment(" ACTIVE                            ██", ConsoleColor.DarkRed));
            AddEndingCentered(body, "██                                                                        ██", ConsoleColor.DarkRed);
            AddEndingCentered(body, "████████████████████████████████████████████████████████████████████████████", ConsoleColor.DarkRed);
            AddEndingBlank(body, 2);

            AddEndingInfoLine(body, "KERNEL CORE", "0x00000000", ConsoleColor.Red);
            AddEndingInfoLine(body, "DEFENSE LAYERS", "SILENCED", ConsoleColor.Red);
            AddEndingInfoLine(body, "TRACE SYSTEM", "DISABLED", ConsoleColor.Red);
            AddEndingInfoLine(body, "SIGNAL GRID", "FULLY INFECTED", ConsoleColor.Red);
            AddEndingInfoLine(body, "ROOT ACCESS", "ABSOLUTE", ConsoleColor.White);
            AddEndingInfoLine(body, "SYSTEM OWNER", "VIRUS.EXE", ConsoleColor.Magenta);
            AddEndingBlank(body, 1);
            AddEndingInfoLine(body, "ACCESS LEVEL", player.AccessLevel + " / " + GameConfig.BossRequiredAccess, ConsoleColor.White);
            AddEndingInfoLine(body, "TRACE LEVEL", traceLevel + "%", ConsoleColor.DarkRed);
            AddEndingInfoLine(body, "PAYLOAD", GetEndingMutationName(player), ConsoleColor.Magenta);
            AddEndingInfoLine(body, "MOVE COUNT", moveCount.ToString(), ConsoleColor.Gray);
            AddEndingBlank(body, 2);
            AddEndingCentered(body, "모든 시스템 제어권이 VIRUS.EXE로 이전되었습니다.", ConsoleColor.Red);
            AddEndingCentered(body, "대상 시스템은 더 이상 저항하지 않습니다.", ConsoleColor.DarkRed);
            AddEndingCentered(body, "THE SYSTEM NO LONGER BELONGS TO THEM.", ConsoleColor.Gray);
            AddEndingCentered(body, "IT BELONGS TO VIRUS.EXE.", ConsoleColor.White);
            AddEndingBlank(body, 2);
            AddEndingCentered(body, "ROOT ACCESS ACCEPTED   //   KERNEL RESPONSE NONE", ConsoleColor.DarkRed);

            return body;
        }

        private void PlayEndingCredits(Player player, int traceLevel, int moveCount)
        {
            List<EndingLine> credits = BuildEndingCreditLines(player, traceLevel, moveCount); // 크레딧 라인 생성
            int totalSteps = credits.Count + EndingBodyLines + 5; // 스크롤 단계 수
            int totalFrames = totalSteps * EndingCreditHoldFrames; // 유지 프레임 포함

            for (int frame = 0; frame < totalFrames; frame++) // 위로 올라가는 크레딧 스크롤
            {
                int offset = frame / EndingCreditHoldFrames; // 한 줄 유지 처리
                List<EndingLine> body = new List<EndingLine>(); // 크레딧 본문 41줄

                for (int row = 0; row < EndingBodyLines; row++) // 41줄 viewport
                {
                    int sourceIndex = offset - EndingBodyLines + row; // 현재 행에 들어갈 크레딧 인덱스

                    if (sourceIndex >= 0 && sourceIndex < credits.Count) // 출력 가능한 크레딧 체크
                    {
                        body.Add(credits[sourceIndex]); // 크레딧 라인 추가
                    }
                    else
                    {
                        AddEndingBlank(body, 1); // 빈 행
                    }
                }

                RenderEndingFrame(new EndingLine(false,
                        new ColorSegment(" ENDING CREDITS", ConsoleColor.Red),
                        new ColorSegment(" // ", ConsoleColor.DarkRed),
                        new ColorSegment("HACKING TERMINAL LOG", ConsoleColor.White)),
                    body,
                    new EndingLine(false,
                        new ColorSegment(" SYSTEM OWNED BY ", ConsoleColor.DarkRed),
                        new ColorSegment("VIRUS.EXE", ConsoleColor.Magenta)),
                    ConsoleColor.DarkRed);
                Thread.Sleep(EndingCreditDelay); // 스크롤 속도
            }

            EnterEndingPage(); // 크레딧에서 최종 화면으로 전환
            RenderRootControlEndingScreen(player, traceLevel, moveCount, true); // 최종 엔딩 화면 재출력
            WaitEndingCloseKey(); // 종료 입력 대기
        }

        private List<EndingLine> BuildEndingCreditLines(Player player, int traceLevel, int moveCount)
        {
            List<EndingLine> lines = new List<EndingLine>(); // 크레딧 라인

            AddEndingBlank(lines, 4);
            AddCredit(lines, "██╗   ██╗██╗██████╗ ██╗   ██╗███████╗   ███████╗██╗  ██╗███████╗", ConsoleColor.Red);
            AddCredit(lines, "██║   ██║██║██╔══██╗██║   ██║██╔════╝   ██╔════╝╚██╗██╔╝██╔════╝", ConsoleColor.DarkRed);
            AddCredit(lines, "██║   ██║██║██████╔╝██║   ██║███████╗   █████╗   ╚███╔╝ █████╗  ", ConsoleColor.Red);
            AddCredit(lines, "╚██╗ ██╔╝██║██╔══██╗██║   ██║╚════██║   ██╔══╝   ██╔██╗ ██╔══╝  ", ConsoleColor.DarkRed);
            AddCredit(lines, " ╚████╔╝ ██║██║  ██║╚██████╔╝███████║██╗███████╗██╔╝ ██╗███████╗", ConsoleColor.Red);
            AddCredit(lines, "  ╚═══╝  ╚═╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝╚═╝╚══════╝╚═╝  ╚═╝╚══════╝", ConsoleColor.DarkRed);
            AddEndingBlank(lines, 2);
            AddCredit(lines, "SYSTEM BREACH COMPLETE", ConsoleColor.White);
            AddCredit(lines, "ROOT CONTROL HIJACKED", ConsoleColor.Red);
            AddCredit(lines, "모든 시스템 제어권이 VIRUS.EXE로 이전되었습니다.", ConsoleColor.DarkRed);
            AddEndingBlank(lines, 3);
            AddCredit(lines, "개발", ConsoleColor.Red);
            AddCredit(lines, "JC SOFT", ConsoleColor.White);
            AddEndingBlank(lines, 2);
            AddCredit(lines, "기획 / 구현 / 디버그", ConsoleColor.Red);
            AddCredit(lines, "JC", ConsoleColor.White);
            AddEndingBlank(lines, 3);
            AddCredit(lines, "등장한 보안 프로세스", ConsoleColor.Red);
            AddCredit(lines, "SCAN_DAEMON", ConsoleColor.Gray);
            AddCredit(lines, "MEM_LEAK_ANOMALY", ConsoleColor.Gray);
            AddCredit(lines, "LOGIC_BOMB", ConsoleColor.Gray);
            AddCredit(lines, "NULL_POINTER_VOID", ConsoleColor.Gray);
            AddCredit(lines, "PROTOCOL_MUNCHER", ConsoleColor.Gray);
            AddCredit(lines, "SANDBOX_ISOLATION", ConsoleColor.Gray);
            AddCredit(lines, "CIPHER_BLOCK_CHAIN", ConsoleColor.Gray);
            AddEndingBlank(lines, 3);
            AddCredit(lines, "등장한 엘리트 방화벽", ConsoleColor.Red);
            AddCredit(lines, "PROXY_SINGULARITY", ConsoleColor.Gray);
            AddCredit(lines, "SYN_FLOOD_GATE", ConsoleColor.Gray);
            AddCredit(lines, "IC_CRYPTO_GATE", ConsoleColor.Gray);
            AddEndingBlank(lines, 3);
            AddCredit(lines, "최종 코어", ConsoleColor.Red);
            AddCredit(lines, "KERNEL_CORE", ConsoleColor.White);
            AddEndingBlank(lines, 3);
            AddCredit(lines, "PAYLOAD MUTATION", ConsoleColor.Red);
            AddCredit(lines, "RANSOMWARE", ConsoleColor.Gray);
            AddCredit(lines, "TROJAN", ConsoleColor.Gray);
            AddCredit(lines, "ADWARE", ConsoleColor.Gray);
            AddEndingBlank(lines, 3);
            AddCredit(lines, "FINAL SESSION REPORT", ConsoleColor.Red);
            AddCreditInfo(lines, "ACCESS LEVEL", player.AccessLevel + " / " + GameConfig.BossRequiredAccess, ConsoleColor.White);
            AddCreditInfo(lines, "TRACE LEVEL", traceLevel + "%", ConsoleColor.DarkRed);
            AddCreditInfo(lines, "PAYLOAD", GetEndingMutationName(player), ConsoleColor.Magenta);
            AddCreditInfo(lines, "MOVE COUNT", moveCount.ToString(), ConsoleColor.Gray);
            AddEndingBlank(lines, 3);
            AddCredit(lines, "[ ROOT ACCESS ACCEPTED ]", ConsoleColor.DarkRed);
            AddCredit(lines, "[ KERNEL CORE RESPONSE : NONE ]", ConsoleColor.DarkRed);
            AddCredit(lines, "[ SYSTEM OWNER REWRITTEN ]", ConsoleColor.DarkRed);
            AddCredit(lines, "[ VIRUS.EXE CONTROL LEVEL : ABSOLUTE ]", ConsoleColor.Red);
            AddEndingBlank(lines, 3);
            AddCredit(lines, "대상 시스템은 더 이상 저항하지 않습니다.", ConsoleColor.DarkRed);
            AddCredit(lines, "THANK YOU FOR PLAYING", ConsoleColor.White);
            AddEndingBlank(lines, 2);
            AddCredit(lines, "JC SOFT", ConsoleColor.White);
            AddCredit(lines, "0x00000000", ConsoleColor.DarkRed);
            AddEndingBlank(lines, 8);

            return lines;
        }

        private string GetEndingMutationName(Player player)
        {
            if (player == null) return "VIRUS.EXE"; // 예외 기본값
            if (player.Mutation == VirusMutation.None) return "VIRUS.EXE"; // 변이 전 기본값
            return player.Mutation.ToString().ToUpperInvariant(); // 변이명
        }

        private void RenderEndingFrame(EndingLine title, List<EndingLine> body, EndingLine footer, ConsoleColor frameColor)
        {
            EndingLine[] page = BuildEndingFramePage(title, body, footer, frameColor); // 47줄 프레임 데이터 생성
            RenderEndingPage(page, frameColor); // 좌표 덮어쓰기 출력
        }

        private EndingLine[] BuildEndingFramePage(EndingLine title, List<EndingLine> body, EndingLine footer, ConsoleColor frameColor)
        {
            EndingLine[] page = new EndingLine[EndingHeight]; // 47줄 페이지 버퍼

            page[0] = CreateEndingRawLine(new ColorSegment("╔" + new string('═', EndingWidth) + "╗", frameColor));
            page[1] = CreateEndingFramedContentLine(title, frameColor);
            page[2] = CreateEndingRawLine(new ColorSegment("╠" + new string('═', EndingWidth) + "╣", frameColor));

            for (int i = 0; i < EndingBodyLines; i++) // 본문 41줄
            {
                EndingLine line = i < body.Count ? body[i] : CreateEndingBlankLine(); // 부족 줄 채움
                page[3 + i] = CreateEndingFramedContentLine(line, frameColor); // 본문 줄
            }

            page[44] = CreateEndingRawLine(new ColorSegment("╠" + new string('═', EndingWidth) + "╣", frameColor));
            page[45] = CreateEndingFramedContentLine(footer, frameColor);
            page[46] = CreateEndingRawLine(new ColorSegment("╚" + new string('═', EndingWidth) + "╝", frameColor));

            return page;
        }

        private void RenderEndingPage(EndingLine[] page, ConsoleColor defaultColor)
        {
            TryScrollConsoleToTop(); // 스크롤 위치 고정
            HideCursor(); // 커서 유배

            for (int y = 0; y < EndingHeight; y++) // 47줄 좌표 덮어쓰기
            {
                Console.SetCursorPosition(0, y); // 줄 위치 직접 지정
                EndingLine line = y < page.Length && page[y] != null ? page[y] : CreateEndingBlankLine(); // null 방지
                WriteEndingPageLine(line, defaultColor); // 개행 없이 한 줄 덮어쓰기
            }

            HideCursor(); // 출력 후 커서 유배
        }

        private void WriteEndingPageLine(EndingLine line, ConsoleColor defaultColor)
        {
            int maxWidth = Math.Max(1, Math.Min(GameConfig.ConsoleWidth - 1, Console.BufferWidth - 1)); // 스크롤 방지 폭
            int writtenWidth = 0; // 출력 폭 누적
            List<ColorSegment> segments = line == null ? null : line.Segments; // 세그먼트 목록

            if (segments == null || segments.Count == 0) // 빈 줄 처리
            {
                SetColor(defaultColor);
                Console.Write(new string(' ', maxWidth)); // 줄 전체 덮어쓰기
                Console.ResetColor();
                return;
            }

            for (int i = 0; i < segments.Count; i++) // 세그먼트 출력
            {
                if (writtenWidth >= maxWidth) break; // 폭 초과 방지

                ColorSegment segment = segments[i];
                string text = segment.Text ?? string.Empty; // null 방지
                int remain = maxWidth - writtenWidth; // 남은 폭
                string fitted = TextUtil.Fit(text, Math.Min(TextUtil.GetDisplayWidth(text), remain)); // 남은 폭 보정
                int fittedWidth = TextUtil.GetDisplayWidth(fitted); // 실제 표시 폭

                SetColor(segment.Foreground);
                if (segment.Background.HasValue) Console.BackgroundColor = segment.Background.Value; // 배경색 적용
                Console.Write(fitted); // 세그먼트 출력
                Console.ResetColor();
                writtenWidth += fittedWidth; // 폭 누적
            }

            if (writtenWidth < maxWidth) // 남은 줄 덮어쓰기
            {
                SetColor(defaultColor);
                Console.Write(new string(' ', maxWidth - writtenWidth));
                Console.ResetColor();
            }
        }

        private EndingLine CreateEndingFramedContentLine(EndingLine line, ConsoleColor frameColor)
        {
            if (line == null) line = CreateEndingBlankLine(); // null 방지

            List<ColorSegment> result = new List<ColorSegment>(); // 전체 프레임 줄
            result.Add(new ColorSegment("║", frameColor)); // 왼쪽 프레임
            result.AddRange(BuildEndingInnerSegments(line, EndingWidth)); // 내부 내용
            result.Add(new ColorSegment("║", frameColor)); // 오른쪽 프레임
            return new EndingLine(false, result.ToArray()); // 완성 줄
        }

        private List<ColorSegment> BuildEndingInnerSegments(EndingLine line, int maxWidth)
        {
            List<ColorSegment> result = new List<ColorSegment>(); // 내부 세그먼트
            int contentWidth = GetSegmentsWidth(line.Segments); // 내용 표시 폭
            int writtenWidth = 0; // 출력 폭 누적

            if (line.Centered && contentWidth < maxWidth) // 중앙 정렬 여백
            {
                int left = (maxWidth - contentWidth) / 2; // 좌측 여백
                result.Add(new ColorSegment(new string(' ', left), ConsoleColor.Black));
                writtenWidth += left;
            }

            for (int i = 0; i < line.Segments.Count; i++) // 내용 복사
            {
                if (writtenWidth >= maxWidth) break; // 폭 초과 방지

                ColorSegment segment = line.Segments[i];
                string text = segment.Text ?? string.Empty; // null 방지
                int remain = maxWidth - writtenWidth; // 남은 폭
                string fitted = TextUtil.Fit(text, Math.Min(TextUtil.GetDisplayWidth(text), remain)); // 폭 보정
                int fittedWidth = TextUtil.GetDisplayWidth(fitted); // 표시 폭

                result.Add(new ColorSegment(fitted, segment.Foreground)); // 내용 추가
                writtenWidth += fittedWidth; // 폭 누적
            }

            if (writtenWidth < maxWidth) // 우측 여백 채움
            {
                result.Add(new ColorSegment(new string(' ', maxWidth - writtenWidth), ConsoleColor.Black));
            }

            return result;
        }

        private EndingLine CreateEndingRawLine(params ColorSegment[] segments)
        {
            return new EndingLine(false, segments); // 프레임 라인 그대로 사용
        }

        private void AddEndingInfoLine(List<EndingLine> lines, string label, string value, ConsoleColor valueColor)
        {
            string leftPad = new string(' ', 31); // 결과 정보 시작 위치 고정
            AddEndingSegments(lines, false,
                new ColorSegment(leftPad, ConsoleColor.Black),
                new ColorSegment(TextUtil.Fit(label, 15), ConsoleColor.DarkRed),
                new ColorSegment(" : ", ConsoleColor.DarkGray),
                new ColorSegment(value, valueColor));
        }

        private void AddCreditInfo(List<EndingLine> lines, string label, string value, ConsoleColor valueColor)
        {
            AddEndingSegments(lines, true,
                new ColorSegment(TextUtil.Fit(label, 15), ConsoleColor.DarkRed),
                new ColorSegment(" : ", ConsoleColor.DarkGray),
                new ColorSegment(value, valueColor));
        }

        private void AddCredit(List<EndingLine> lines, string text, ConsoleColor color)
        {
            AddEndingCentered(lines, text, color); // 크레딧 중앙 줄
        }

        private void AddEndingCentered(List<EndingLine> lines, string text, ConsoleColor color)
        {
            AddEndingSegments(lines, true, new ColorSegment(text, color)); // 중앙 단색 줄
        }

        private void AddEndingSegments(List<EndingLine> lines, bool centered, params ColorSegment[] segments)
        {
            lines.Add(new EndingLine(centered, segments)); // 색상 세그먼트 라인 추가
        }

        private void AddEndingBlank(List<EndingLine> lines, int count)
        {
            for (int i = 0; i < count; i++) // 빈 줄 반복
            {
                lines.Add(CreateEndingBlankLine()); // 빈 줄 추가
            }
        }

        private EndingLine CreateEndingBlankLine()
        {
            return new EndingLine(false, new ColorSegment(string.Empty, ConsoleColor.Black)); // 빈 줄
        }

        private void WaitEndingCloseKey()
        {
            while (true) // Q 대기
            {
                HideCursor(); // 대기 중 커서 유배
                ConsoleKey key = InputHelper.ReadKey(); // 키 입력

                if (key == ConsoleKey.Q || key == ConsoleKey.Escape) // 종료 키 체크
                {
                    suppressBattleTextGlitch = false; // 보호 상태 복구
                    return;
                }
            }
        }
    }
}
