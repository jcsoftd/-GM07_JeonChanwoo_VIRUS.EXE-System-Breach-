using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using VirusExe.SystemBreach.Core;
using VirusExe.SystemBreach.DataGrid;

namespace VirusExe.SystemBreach.Rendering
{
    // 컷신/글리치 연출
    // 노드 진입, 보스 진입, 변이 감지 같은 분위기 연출
    public partial class ConsoleRenderer
    {
        public void ShowBootSequence()
        {
            Console.Clear(); // 이전 침투 모달 잔상 제거

            List<string> logs = new List<string>(); // 페이로드 투입 로그
            RenderPayloadDropModal(logs, "payload route initializing", 0); // 초기 프레임
            Thread.Sleep(90);

            AddPayloadDropLog(logs, "SIGNATURE        : VIRUS.EXE", "signature verified", 18, 110); // 시그니처 확인
            AddPayloadDropLog(logs, "PAYLOAD          : INJECTED", "payload injected", 38, 120); // 페이로드 투입
            AddPayloadDropLog(logs, "TRACE MASK       : ACTIVE", "trace mask active", 58, 120); // 추적 마스크
            AddPayloadDropLog(logs, "KERNEL CORE      : DETECTED", "kernel core detected", 78, 130); // 코어 감지
            AddPayloadDropLog(logs, "SIGNAL GRID      : ONLINE", "signal grid online", 100, 220); // 그리드 활성화

            RenderPayloadDropModal(logs, "system entry confirmed", 100); // 완료 프레임
            Thread.Sleep(360); // GRID 진입 전 여운
        }

        public void ShowGridEntryIntroFlow()
        {
            ShowGridEntryNoticeModal(); // 첫 침투 안내
            ShowGridManualModal(); // 게임 설명창
        }

        private void AddPayloadDropLog(List<string> logs, string line, string status, int percent, int delay)
        {
            logs.Add(line); // 로그 추가
            RenderPayloadDropModal(logs, status, percent); // 라지 모달 갱신
            Thread.Sleep(delay); // 짧은 연출 딜레이
        }

        private void RenderPayloadDropModal(List<string> logs, string status, int percent)
        {
            BeginModal("VIRUS.EXE PAYLOAD DROP       // SYSTEM ENTRY", ModalSize.Large); // 라지 모달 시작
            WriteModalSegmentsLine(
                new ColorSegment(" ENTRY CHANNEL : ", ConsoleColor.DarkGray),
                new ColorSegment("BREACH POINT", ConsoleColor.Green),
                new ColorSegment("        ROUTE : ", ConsoleColor.DarkGray),
                new ColorSegment("SIGNAL GRID", ConsoleColor.Cyan)); // 진입 채널
            WriteModalSeparator();
            WriteModalTextLine(" // PAYLOAD INJECTION", ConsoleColor.Magenta); // 섹션 제목
            WriteModalTextLine(" 열린 침투 지점으로 VIRUS.EXE 페이로드를 투입합니다.", ConsoleColor.DarkGray); // 설명
            WriteModalEmptyLine();

            for (int i = 0; i < 8; i++) // 로그 영역 고정
            {
                if (i < logs.Count) // 표시 로그 체크
                {
                    WriteModalTextLine(" > " + logs[i], GetPayloadDropLogColor(logs[i])); // 로그 출력
                }
                else
                {
                    WriteModalTextLine(" ", ConsoleColor.DarkGray); // 빈 로그 줄
                }
            }

            WriteModalEmptyLine();
            WriteModalSegmentsLine(
                new ColorSegment(" INJECTION RATE : ", ConsoleColor.DarkGray),
                new ColorSegment(MakePercentBar(percent, 26), percent >= 100 ? ConsoleColor.Green : ConsoleColor.Magenta),
                new ColorSegment(" " + percent.ToString().PadLeft(3) + "%", percent >= 100 ? ConsoleColor.Green : ConsoleColor.White)); // 진행률
            WriteModalEmptyLine();
            WriteModalCentered(percent >= 100 ? "SYSTEM ENTRY CONFIRMED" : "DROPPING PAYLOAD INTO MEMORY...", percent >= 100 ? ConsoleColor.Green : ConsoleColor.Yellow); // 중앙 상태
            WriteModalFooter(
                new ColorSegment(" STATUS : ", ConsoleColor.DarkGray),
                new ColorSegment(status, GetPayloadDropStatusColor(status)),
                new ColorSegment("   PLEASE WAIT", ConsoleColor.DarkGray)); // 자동 진행 Footer
            EndModal(); // 라지 모달 종료
            HideCursor(); // 커서 유배
        }

        private void ShowGridEntryNoticeModal()
        {
            BeginModal("SYSTEM BREACH COMPLETE", ModalSize.Medium); // 중형 안내 모달
            WriteModalCentered("시스템에 침투 하였습니다.", ConsoleColor.Green); // 침투 완료
            WriteModalEmptyLine();
            WriteModalTextLine(" 모든 GRID를 감염시키고", ConsoleColor.Gray); // 목표 1
            WriteModalTextLine(" KERNEL CORE를 파괴하십시오.", ConsoleColor.Gray); // 목표 2
            WriteModalFooter(
                new ColorSegment(" E", ConsoleColor.Green),
                new ColorSegment(" 다음", ConsoleColor.DarkGray)); // 다음 안내
            EndModal(); // 모달 종료
            WaitModalNextKey(); // 다음 입력 대기
        }

        public void ShowGridManualModal()
        {
            BeginModal("SIGNAL GRID MANUAL       // FIRST ACCESS", ModalSize.Large); // 라지 설명 모달
            WriteModalTextLine(" // OBJECTIVE", ConsoleColor.Magenta);
            WriteModalTextLine(" ACCESS LEVEL " + GameConfig.BossRequiredAccess + " 확보 후 KERNEL CORE를 파괴하십시오.", ConsoleColor.Gray);
            WriteModalEmptyLine();
            WriteModalTextLine(" // CONTROL", ConsoleColor.Cyan);
            WriteModalTextLine(" W / A / S / D : SIGNAL GRID 이동", ConsoleColor.Gray);
            WriteModalTextLine(" E             : 노드 접속", ConsoleColor.Gray);
            WriteModalTextLine(" I             : DATA STORAGE    C : VIRUS STATUS", ConsoleColor.Gray);
            WriteModalTextLine(" X             : SCAN_PULSE      H : SYSTEM INFO", ConsoleColor.Gray);
            WriteModalEmptyLine();
            WriteModalTextLine(" // NODE TYPE", ConsoleColor.Yellow);
            WriteModalTextLine(" /Sec    : 일반 보안 프로세스 전투 / ACCESS +1", ConsoleColor.Gray);
            WriteModalTextLine(" /Fw     : 방화벽 엘리트 전투 / ACCESS +2", ConsoleColor.Gray);
            WriteModalTextLine(" /Mkt    : EXPLOIT MARKET 상점", ConsoleColor.Gray);
            WriteModalTextLine(" /Lab    : PAYLOAD 강화 노드", ConsoleColor.Gray);
            WriteModalTextLine(" /Tmp    : 랜덤 시스템 이벤트", ConsoleColor.Gray);
            WriteModalTextLine(" /Data   : 회복 / 데이터 캐시", ConsoleColor.Gray);
            WriteModalTextLine(" Kernel  : 최종 KERNEL CORE", ConsoleColor.Gray);
            WriteModalEmptyLine();
            WriteModalTextLine(" // WARNING", ConsoleColor.Red);
            WriteModalTextLine(" 노드를 장악할 때마다 TRACE LEVEL이 상승합니다.", ConsoleColor.DarkGray);
            WriteModalTextLine(" TRACE LEVEL이 상승하면 보안 프로세스가 강화됩니다.", ConsoleColor.DarkGray);
            WriteModalTextLine(" 전투 전에는 HEALTH / ENERGY / ITEM 상태를 확인하십시오.", ConsoleColor.DarkGray);
            WriteModalFooterText("Q 창닫기", ConsoleColor.Red); // 도움말 Footer
            EndModal(); // 설명 모달 종료
            WaitModalCloseKey(); // 창닫기 입력 대기
        }

        private void WaitModalNextKey()
        {
            WaitModalInput(ModalInputMode.NextOnly); // 다음 입력 대기
        }

        private void WaitModalCloseKey()
        {
            WaitModalInput(ModalInputMode.CloseOnly); // 창닫기 입력 대기
        }

        private ConsoleColor GetPayloadDropLogColor(string line)
        {
            if (line.IndexOf("VIRUS.EXE", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Magenta; // 페이로드
            if (line.IndexOf("INJECTED", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Green; // 투입 완료
            if (line.IndexOf("ACTIVE", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Green; // 활성화
            if (line.IndexOf("DETECTED", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Yellow; // 감지
            if (line.IndexOf("ONLINE", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Cyan; // 온라인
            return ConsoleColor.Gray; // 기본 로그
        }

        private ConsoleColor GetPayloadDropStatusColor(string status)
        {
            if (status.IndexOf("confirmed", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Green; // 완료
            if (status.IndexOf("online", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Cyan; // 온라인
            if (status.IndexOf("detected", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Yellow; // 감지
            if (status.IndexOf("active", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Green; // 활성화
            if (status.IndexOf("injected", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Green; // 투입
            return ConsoleColor.DarkGray; // 기본 상태
        }

        public void PlayFullScreenGlitchTransition()
        {
            Random random = new Random(); // 글리치 난수
            string chars = "01#@%░▒▓VIRUSKERNELTRACE"; // 전체 화면 노이즈 문자
            ConsoleColor[] colors = new ConsoleColor[]
            {
                ConsoleColor.DarkGray,
                ConsoleColor.DarkRed,
                ConsoleColor.DarkMagenta,
                ConsoleColor.Red,
                ConsoleColor.Magenta
            }; // 글리치 색상 풀

            for (int frame = 0; frame < 10; frame++) // 약 500ms 글리치
            {
                Console.SetCursorPosition(0, 0); // 화면 상단 고정

                for (int y = 0; y < GameConfig.ConsoleHeight - 1; y++) // 전체 화면 라인
                {
                    Console.ForegroundColor = colors[random.Next(colors.Length)]; // 프레임 색상
                    Console.Write(BuildGlitchNoiseLine(random, chars, GameConfig.ConsoleWidth)); // 노이즈 출력
                }

                Console.ResetColor(); // 색상 복구
                HideCursor(); // 커서 유배
                Thread.Sleep(50); // 프레임 지연
            }
        }

        public void PlayGlitchCutscene(string message)
        {
            Random random = new Random(); // 글리치 난수
            string chars = "01#@%░▒▓KERNELVIRUS"; // 글리치 문자 풀

            for (int frame = 0; frame < 10; frame++) // 글리치 프레임
            {
                BeginModal("SYSTEM GLITCH       // FRAME BUFFER DESYNC", ModalSize.Large); // 글리치 모달 시작

                for (int y = 0; y < 9; y++) // 노이즈 행 출력
                {
                    WriteModalTextLine(" " + BuildGlitchNoiseLine(random, chars, 72), ConsoleColor.DarkRed); // 노이즈 출력
                }

                WriteModalCentered(CorruptText(message, random), ConsoleColor.Red); // 메시지 출력
                WriteModalFooterText("SYSTEM GLITCH   PLEASE WAIT", ConsoleColor.DarkGray); // 자동 처리 Footer
                EndModal(); // 글리치 모달 종료
                HideCursor(); // 커서 유배
                Thread.Sleep(60); // 프레임 지연
            }

            Console.ResetColor(); // 색상 복구
        }

        private string BuildGlitchNoiseLine(Random random, string chars, int length)
        {
            StringBuilder builder = new StringBuilder(); // 노이즈 문자열

            for (int i = 0; i < length; i++) // 길이만큼 문자 생성
            {
                builder.Append(chars[random.Next(chars.Length)]); // 랜덤 문자 추가
            }

            return builder.ToString(); // 노이즈
        }

        public void PlayNodeHackSequence(GridNode node, int traceIncrease)
        {
            ConsoleColor nodeColor = GetHackNodeColor(node.Type); // 노드 색상
            string nodeCode = GetNodeLabel(node.Type); // 노드 코드
            List<string> logs = new List<string>(); // 모달 로그 목록

            RenderNodeHackModal(node, traceIncrease, logs, "connecting...", nodeColor); // 초기 프레임
            Thread.Sleep(120); // 출력 간격

            AddNodeHackLog(node, traceIncrease, logs, "> connect --target " + nodeCode, "connecting...", nodeColor, 130);
            AddNodeHackLog(node, traceIncrease, logs, "> handshake://signal.node ........................ OK", "handshake complete", nodeColor, 130);
            AddNodeHackLog(node, traceIncrease, logs, "> scan.security.layer ............................ FOUND", "security layer found", nodeColor, 130);

            PlayTerminalProgressModal(node, traceIncrease, logs, "> downloading auth.fragment ", ConsoleColor.Cyan, 0, 32, 120);
            PlayTerminalProgressModal(node, traceIncrease, logs, "> downloading auth.fragment ", ConsoleColor.Cyan, 32, 68, 120);
            PlayTerminalProgressModal(node, traceIncrease, logs, "> downloading auth.fragment ", ConsoleColor.Cyan, 68, 100, 120);

            PlayTerminalCounterModal(node, traceIncrease, logs, "> deleting defense.cache ", ConsoleColor.Red, 12, 35);
            PlayTerminalCounterModal(node, traceIncrease, logs, "> overwriting process.memory ", ConsoleColor.Yellow, 48, 18);

            AddNodeHackLog(node, traceIncrease, logs, "> injecting VIRUS.EXE payload .................... READY", "payload ready", nodeColor, 150);
            AddNodeHackLog(node, traceIncrease, logs, "> trace.signature ................................ +" + traceIncrease + "%", "trace signature updated", nodeColor, 150);
            Thread.Sleep(200); // 완료 전 딜레이
            AddNodeHackLog(node, traceIncrease, logs, "> access granted", "access granted", nodeColor, 220);

            Thread.Sleep(360); // 다음 장면 전 여운
        }

        private void AddNodeHackLog(GridNode node, int traceIncrease, List<string> logs, string line, string status, ConsoleColor nodeColor, int delay)
        {
            logs.Add(line); // 로그 추가
            TrimNodeHackLogs(logs); // 로그 수 제한
            RenderNodeHackModal(node, traceIncrease, logs, status, nodeColor); // 모달 갱신
            Thread.Sleep(delay); // 출력 간격
        }

        private void TrimNodeHackLogs(List<string> logs)
        {
            while (logs.Count > 20) // 표시 가능 로그 초과 체크
            {
                logs.RemoveAt(0); // 오래된 로그 제거
            }
        }

        private void RenderNodeHackModal(GridNode node, int traceIncrease, List<string> logs, string status, ConsoleColor nodeColor)
        {
            string nodeCode = GetNodeLabel(node.Type); // 노드 코드
            string title = "TERMINAL ACCESS       // " + GetHackNodeName(node.Type); // 모달 제목

            BeginModal(title, ModalSize.Large); // 노드 접속 모달 시작
            WriteModalSegmentsLine(
                new ColorSegment(" TARGET NODE : ", ConsoleColor.DarkGray),
                new ColorSegment(nodeCode, nodeColor),
                new ColorSegment("        TYPE : ", ConsoleColor.DarkGray),
                new ColorSegment(GetHackNodeTypeText(nodeCode), ConsoleColor.Cyan));
            WriteModalSegmentsLine(
                new ColorSegment(" RISK        : ", ConsoleColor.DarkGray),
                new ColorSegment(GetHackNodeRiskText(nodeCode), GetHackRiskColor(nodeCode)),
                new ColorSegment("        TRACE : ", ConsoleColor.DarkGray),
                new ColorSegment("+" + traceIncrease + "%", ConsoleColor.Yellow));
            WriteModalSeparator();

            for (int i = 0; i < 20; i++) // 로그 영역 고정
            {
                if (i < logs.Count) // 출력할 로그 체크
                {
                    WriteModalTextLine(" " + logs[i], GetTerminalLogColor(logs[i], nodeColor)); // 로그 출력
                }
                else
                {
                    WriteModalTextLine(" ", ConsoleColor.DarkGray); // 빈 로그 줄
                }
            }

            WriteModalFooter(
                new ColorSegment(" STATUS : ", ConsoleColor.DarkGray),
                new ColorSegment(status, GetTerminalStatusColor(status)),
                new ColorSegment("   PLEASE WAIT", ConsoleColor.DarkGray)); // 자동 처리 Footer
            EndModal(); // 노드 접속 모달 종료
            HideCursor(); // 커서 유배
        }

        private ConsoleColor GetTerminalLogColor(string line, ConsoleColor nodeColor)
        {
            if (line.IndexOf("connect", StringComparison.OrdinalIgnoreCase) >= 0) return nodeColor; // 접속 로그
            if (line.IndexOf("OK", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Green; // 성공 로그
            if (line.IndexOf("FOUND", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Cyan; // 발견 로그
            if (line.IndexOf("downloading", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Cyan; // 다운로드 로그
            if (line.IndexOf("deleting", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Red; // 삭제 로그
            if (line.IndexOf("overwriting", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Yellow; // 덮어쓰기 로그
            if (line.IndexOf("READY", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Green; // 준비 완료
            if (line.IndexOf("trace", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Yellow; // TRACE 로그
            if (line.IndexOf("granted", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Green; // 접근 허가
            if (line.IndexOf("DONE", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Green; // 완료 로그
            return ConsoleColor.Gray; // 기본 로그
        }

        private ConsoleColor GetTerminalStatusColor(string status)
        {
            if (status.IndexOf("granted", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Green; // 접근 허가
            if (status.IndexOf("updated", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Yellow; // 갱신 상태
            if (status.IndexOf("ready", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Green; // 준비 상태
            if (status.IndexOf("found", StringComparison.OrdinalIgnoreCase) >= 0) return ConsoleColor.Cyan; // 발견 상태
            return ConsoleColor.DarkGray; // 기본 상태
        }

        private void PlayTerminalProgressModal(GridNode node, int traceIncrease, List<string> logs, string prefix, ConsoleColor color, int fromPercent, int toPercent, int totalDelay)
        {
            int[] steps = new int[] { fromPercent, (fromPercent + toPercent) / 2, toPercent }; // 진행률 단계

            for (int i = 0; i < steps.Length; i++) // 진행률 갱신
            {
                string bar = MakePercentBar(steps[i], 10); // 진행 바 생성
                AddNodeHackLog(node, traceIncrease, logs, prefix + bar + " " + steps[i] + "%", "auth fragment " + steps[i] + "%", color, totalDelay);
            }
        }

        private void PlayTerminalCounterModal(GridNode node, int traceIncrease, List<string> logs, string prefix, ConsoleColor color, int max, int delay)
        {
            int step = Math.Max(1, max / 4); // 카운터 증가 단위

            for (int value = 0; value <= max; value += step) // 카운터 진행
            {
                if (value > max) value = max; // 최대값 보정
                AddNodeHackLog(node, traceIncrease, logs, prefix + value + " / " + max, "cache rewrite " + value + " / " + max, color, delay);
            }

            AddNodeHackLog(node, traceIncrease, logs, prefix + max + " / " + max + " .... DONE", "cache rewrite complete", color, delay);
        }

        private void WriteTerminalLine(int top, string text, ConsoleColor color, int delay)
        {
            Console.SetCursorPosition(0, top); // 출력 위치 이동
            WriteLine(" " + text, color); // 터미널 줄 출력
            Thread.Sleep(delay); // 출력 간격
        }

        private void PlayTerminalProgress(int top, string prefix, ConsoleColor color, int fromPercent, int toPercent, int totalDelay)
        {
            int[] steps = new int[] { fromPercent, (fromPercent + toPercent) / 2, toPercent }; // 진행률 단계

            for (int i = 0; i < steps.Length; i++) // 진행률 갱신
            {
                Console.SetCursorPosition(0, top); // 출력 위치 이동

                string bar = MakePercentBar(steps[i], 10); // 진행 바 생성
                string text = " " + prefix + bar + " " + steps[i] + "%";

                WriteLine(text, color); // 진행률 출력
                Thread.Sleep(totalDelay); // 프레임 간격
            }
        }

        private void PlayTerminalCounter(int top, string prefix, ConsoleColor color, int max, int delay)
        {
            int step = Math.Max(1, max / 4); // 카운터 증가 단위

            for (int value = 0; value <= max; value += step) // 카운터 진행
            {
                if (value > max) value = max; // 최대값 보정

                Console.SetCursorPosition(0, top); // 출력 위치 이동
                WriteLine(" " + prefix + value + " / " + max, color); // 카운터 출력
                Thread.Sleep(delay); // 카운터 속도
            }

            Console.SetCursorPosition(0, top); // 최종 위치 이동
            WriteLine(" " + prefix + max + " / " + max + " .... DONE", color); // 완료 출력
        }

        private string MakePercentBar(int percent, int length)
        {
            if (percent < 0) percent = 0; // 최소값 보정
            if (percent > 100) percent = 100; // 최대값 보정

            int filled = percent * length / 100; // 채워진 칸 계산

            return "[" + new string('█', filled) + new string('░', length - filled) + "]"; // 진행 바
        }

        private void WriteCenteredText(string text, ConsoleColor color)
        {
            int width = TextUtil.GetDisplayWidth(text); // 텍스트 폭 계산
            int left = Math.Max(0, (InnerWidth - width) / 2); // 중앙 위치 계산

            WriteLine(TextUtil.Fit(new string(' ', left) + text, InnerWidth), color); // 중앙 출력
        }

        private string GetHackNodeTypeText(string nodeCode)
        {
            if (nodeCode == "SEC") return "전투";
            if (nodeCode == "FW") return "엘리트 전투";
            if (nodeCode == "SHP") return "상점";
            if (nodeCode == "MUT") return "변이 강화";
            if (nodeCode == "EVT") return "시스템 이벤트";
            if (nodeCode == "DAT") return "보상";
            if (nodeCode == "BOS") return "최종 보안 코어";
            return "시스템 노드";
        }

        private string GetHackNodeRiskText(string nodeCode)
        {
            if (nodeCode == "BOS") return "CRITICAL";
            if (nodeCode == "FW") return "HIGH";
            if (nodeCode == "SEC") return "LOW";
            if (nodeCode == "EVT") return "UNKNOWN";
            if (nodeCode == "SHP") return "SAFE";
            if (nodeCode == "MUT") return "SAFE";
            if (nodeCode == "DAT") return "NONE";
            return "NONE";
        }

        private ConsoleColor GetHackRiskColor(string nodeCode)
        {
            string risk = GetHackNodeRiskText(nodeCode); // 위험도 텍스트

            if (risk == "CRITICAL") return ConsoleColor.Red;
            if (risk == "HIGH") return ConsoleColor.Magenta;
            if (risk == "LOW") return ConsoleColor.Yellow;
            if (risk == "SAFE") return ConsoleColor.Green;
            if (risk == "NONE") return ConsoleColor.DarkGray;

            return ConsoleColor.Gray;
        }

        private void WriteHackLine(string text, ConsoleColor color, int delay)
        {
            Console.ForegroundColor = color;

            for (int i = 0; i < text.Length; i++) // 해킹 로그 출력
            {
                Console.Write(text[i]);
                Thread.Sleep(delay);
            }

            Console.WriteLine();
            Console.ResetColor();
        }

        private ConsoleColor GetHackNodeColor(NodeType type)
        {
            if (type == NodeType.Security) return ConsoleColor.Cyan;
            if (type == NodeType.Firewall) return ConsoleColor.Red;
            if (type == NodeType.Shop) return ConsoleColor.Yellow;
            if (type == NodeType.Mutation) return ConsoleColor.Green;
            if (type == NodeType.Event) return ConsoleColor.Magenta;
            if (type == NodeType.DataCache) return ConsoleColor.DarkCyan;
            if (type == NodeType.Boss) return ConsoleColor.Red;
            return ConsoleColor.Gray;
        }

        private string GetHackNodeName(NodeType type)
        {
            if (type == NodeType.Security) return "SECURITY PROCESS";
            if (type == NodeType.Firewall) return "FIREWALL CORE";
            if (type == NodeType.Shop) return "EXPLOIT MARKET";
            if (type == NodeType.Mutation) return "MUTATION LAB";
            if (type == NodeType.Event) return "SYSTEM EVENT";
            if (type == NodeType.DataCache) return "DATA CACHE";
            if (type == NodeType.Boss) return "KERNEL GATE";
            return "UNKNOWN NODE";
        }

        private string CorruptText(string text, Random random)
        {
            char[] result = text.ToCharArray();
            string chars = "#@!%&?░▒▓";

            for (int i = 0; i < result.Length; i++) // 글자 오염
            {
                if (result[i] != ' ' && random.Next(100) < 25) // 공백 제외 오염
                {
                    result[i] = chars[random.Next(chars.Length)];
                }
            }

            return new string(result);
        }

        private void WriteBootLine(string a, ConsoleColor aColor, string b, ConsoleColor bColor, string c, ConsoleColor cColor, int delay)
        {
            WriteBootPart(a, aColor, delay);
            WriteBootPart(b, bColor, delay);
            WriteBootPart(c, cColor, delay);
            Console.WriteLine();
        }

        private void WriteBootPart(string text, ConsoleColor color, int delay)
        {
            Console.ForegroundColor = color;

            for (int i = 0; i < text.Length; i++) // 부팅 텍스트 출력
            {
                Console.Write(text[i]);
                Thread.Sleep(delay);
            }

            Console.ResetColor();
        }
    }
}
