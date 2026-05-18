
using System;
using System.Collections.Generic;
using System.Threading;
using VirusExe.SystemBreach.Characters;
using VirusExe.SystemBreach.Core;
using VirusExe.SystemBreach.Systems;

namespace VirusExe.SystemBreach.Rendering
{
	// PAYLOAD MUTATION 선택 화면
	// 변이 카드, 코드 컴파일 느낌 연출, 선택 결과 표시
	public partial class ConsoleRenderer
    {
        private const int MutationSourceViewportLines = 20; // 코드 스크롤 표시 줄 수

        public void PlayPayloadMutationDetectedSequence()
        {
            renderBattleGlitch = true; // 화면 깨짐 활성화

            RenderMutationIntroGlitchFrame(
                "SIGNAL GRID RETURN INTERRUPTED",
                new string[]
                {
                    " FRAME BUFFER DESYNC",
                    " GRID LINK      : UNSTABLE",
                    " PAYLOAD HASH   : CORRUPTED",
                    " RETURN VECTOR  : BLOCKED"
                },
                140);

            RenderMutationIntroGlitchFrame(
                "VIRUS.EXE CORE DESYNC",
                new string[]
                {
                    " UNKNOWN PAYLOAD GROWTH DETECTED",
                    " COMBAT PROTOCOL : UNSTABLE",
                    " ENTITY SIGNATURE: REWRITING",
                    " MUTATION VECTOR : OPENING"
                },
                160);

            renderBattleGlitch = false; // 한글 출력 전 글리치 해제

            RenderMutationIntroMessageFrame(); // 변이 안내 문구

            for (int step = 0; step <= 16; step++) // 변이 분기 스캔
            {
                RenderMutationBranchScanFrame(step, 16); // 분기 스캔 프레임
                Thread.Sleep(step < 6 ? 70 : 45); // 뒤로 갈수록 빠르게
            }
        }

        public VirusMutation ShowPayloadMutationSelection(Player player)
        {
            int selectedIndex = 0; // 선택 변이

            while (true) // 선택 루프
            {
                RenderPayloadMutationSelection(selectedIndex); // 선택 화면 출력

                ConsoleKey key = Console.ReadKey(true).Key; // 키 입력

                if (key == ConsoleKey.A || key == ConsoleKey.LeftArrow) // 왼쪽 이동 체크
                {
                    selectedIndex--; // 이전 카드
                    if (selectedIndex < 0) selectedIndex = 2; // 순환
                }
                else if (key == ConsoleKey.D || key == ConsoleKey.RightArrow) // 오른쪽 이동 체크
                {
                    selectedIndex++; // 다음 카드
                    if (selectedIndex > 2) selectedIndex = 0; // 순환
                }
                else if (key == ConsoleKey.E) // 확정 체크
                {
                    if (selectedIndex == 0) return VirusMutation.Ransomware; // 랜섬웨어 선택
                    if (selectedIndex == 1) return VirusMutation.Trojan; // 트로젠 선택
                    return VirusMutation.Adware; // 애드웨어 선택
                }
            }
        }

        public void PlayPayloadMutationCompleteSequence(VirusMutation mutation)
        {
            List<string> sourceLines = GetMutationSourceLines(mutation); // 변이 코드 생성
            List<string> scrollBuffer = new List<string>(); // 스크롤 버퍼

            for (int i = 0; i < sourceLines.Count; i++) // 코드 자동 작성
            {
                scrollBuffer.Add(sourceLines[i]); // 새 코드 줄 추가
                RenderMutationSourceScrollFrame(mutation, scrollBuffer, i + 1, sourceLines.Count); // 최근 코드 출력
                Thread.Sleep(GetMutationSourceDelay(i + 1, sourceLines.Count)); // 점점 빨라지는 속도
            }

            string[] buildLogs = GetMutationBuildLogs(mutation); // 빌드 로그

            for (int i = 1; i <= buildLogs.Length; i++) // 빌드 로그 순차 출력
            {
                RenderMutationBuildFrame(mutation, buildLogs, i, false); // 빌드 프레임
                Thread.Sleep(150); // 빌드 로그 속도
            }

            for (int i = 0; i < 3; i++) // 완료 점멸
            {
                RenderMutationBuildFrame(mutation, buildLogs, buildLogs.Length, true); // 완료 강조
                Thread.Sleep(90);
                RenderMutationBuildFrame(mutation, buildLogs, buildLogs.Length, false); // 일반 표시
                Thread.Sleep(90);
            }

            Thread.Sleep(260); // GRID 복귀 전 여운
        }

        private void RenderMutationIntroGlitchFrame(string title, string[] lines, int delay)
        {
            BeginModalRedirect(title, ModalSize.Large); // 변이 글리치 모달 시작
            WriteEmptyLine();

            for (int i = 0; i < lines.Length; i++) // 글리치 로그 출력
            {
                WriteLine(lines[i], GetMutationIntroLineColor(lines[i])); // 로그 색상 출력
            }

            WriteModalFooterText("PAYLOAD EVENT   PLEASE WAIT", ConsoleColor.DarkGray); // 자동 처리 Footer
            EndModalRedirect(); // 변이 글리치 모달 종료
            HideCursor(); // 커서 유배
            Thread.Sleep(delay); // 프레임 딜레이
        }

        private void RenderMutationIntroMessageFrame()
        {
            BeginModalRedirect("PAYLOAD MUTATION DETECTED", ModalSize.Large); // 변이 안내 모달 시작
            WriteEmptyLine();
            WriteLine(" VIRUS.EXE 내부 코드가 새로운 침투 방식으로 재구성됩니다.", ConsoleColor.Gray);
            WriteEmptyLine();
            WriteLine(" PAYLOAD CORE : UNSTABLE", ConsoleColor.Yellow);
            WriteLine(" MUTATION MAP : SCANNING", ConsoleColor.Cyan);
            WriteLine(" BRANCH COUNT : UNKNOWN", ConsoleColor.DarkGray);
            WriteModalFooterText("PAYLOAD EVENT   PLEASE WAIT", ConsoleColor.DarkGray); // 자동 처리 Footer
            EndModalRedirect(); // 변이 안내 모달 종료
            HideCursor(); // 커서 유배
            Thread.Sleep(300); // 안내 딜레이
        }

        private void RenderMutationBranchScanFrame(int step, int maxStep)
        {
            int percent = step * 100 / Math.Max(1, maxStep); // 스캔 진행률
            string bar = BuildMutationProgressBar(percent, 30); // 진행 바

            BeginModalRedirect("PAYLOAD MUTATION // BRANCH SCAN", ModalSize.Large); // 변이 스캔 모달 시작
            WriteLine(" VIRUS.EXE 내부 코드가 새로운 침투 방식으로 재구성됩니다.", ConsoleColor.Gray);
            WriteEmptyLine();
            WriteLine(" CORE SIGNATURE : VIRUS.EXE", ConsoleColor.Green);
            WriteLine(" BRANCH SCAN    : " + percent.ToString("000") + "%", ConsoleColor.Cyan);
            WriteLine(" " + bar, ConsoleColor.Green);
            WriteEmptyLine();
            WriteLine(BuildMutationBranchLine(step, 4, "RANSOMWARE STRAIN", ConsoleColor.Yellow), step >= 4 ? ConsoleColor.Yellow : ConsoleColor.DarkGray);
            WriteLine(BuildMutationBranchLine(step, 8, "TROJAN STRAIN", ConsoleColor.Magenta), step >= 8 ? ConsoleColor.Magenta : ConsoleColor.DarkGray);
            WriteLine(BuildMutationBranchLine(step, 12, "ADWARE STRAIN", ConsoleColor.Green), step >= 12 ? ConsoleColor.Green : ConsoleColor.DarkGray);
            WriteModalFooterText(step >= maxStep ? "SELECTOR UNLOCKED   PLEASE WAIT" : "SCANNING BRANCHES   PLEASE WAIT", step >= maxStep ? ConsoleColor.Magenta : ConsoleColor.DarkGray); // 자동 처리 Footer
            EndModalRedirect(); // 변이 스캔 모달 종료
            HideCursor(); // 커서 유배
        }

        private string BuildMutationBranchLine(int step, int revealStep, string name, ConsoleColor color)
        {
            if (step < revealStep) // 미발견 분기 체크
            {
                return " [???] ------------------------------ waiting signal";
            }

            int active = Math.Min(24, Math.Max(0, (step - revealStep + 1) * 4)); // 활성 경로 길이
            string route = new string('=', active) + new string('-', 24 - active); // 경로 표시
            return " [" + name + "] " + route + " DETECTED";
        }

        private ConsoleColor GetMutationIntroLineColor(string line)
        {
            if (line.IndexOf("DESYNC", StringComparison.Ordinal) >= 0) return ConsoleColor.Red; // 깨짐 로그
            if (line.IndexOf("UNSTABLE", StringComparison.Ordinal) >= 0) return ConsoleColor.Yellow; // 불안정 로그
            if (line.IndexOf("CORRUPTED", StringComparison.Ordinal) >= 0) return ConsoleColor.Magenta; // 손상 로그
            if (line.IndexOf("OPEN", StringComparison.Ordinal) >= 0 || line.IndexOf("OPENING", StringComparison.Ordinal) >= 0) return ConsoleColor.Cyan; // 개방 로그
            return ConsoleColor.Gray; // 기본 로그
        }

        private void RenderPayloadMutationSelection(int selectedIndex)
        {
            BeginModalRedirect("PAYLOAD MUTATION DETECTED", ModalSize.Large); // 변이 선택 모달 시작
            WriteLine(" VIRUS.EXE 내부 코드가 새로운 침투 방식으로 재구성됩니다.", ConsoleColor.Gray);
            WriteLine(" 하나의 변이 루트를 선택하세요.", ConsoleColor.DarkGray);
            WriteSeparator();
            WriteEmptyLine(); // 카드 상단 여백
            WriteMutationCardRows(selectedIndex); // 가로 카드 출력
            WriteEmptyLine(); // 카드 하단 여백
            WriteSeparator();
            WriteMutationDetailPanel(selectedIndex); // 상세 설명 출력
            WriteMutationControlLine(); // 조작 설명
            EndModalRedirect(); // 변이 선택 모달 종료
            HideCursor(); // 커서 유배
        }

        private void RenderMutationSourceScrollFrame(VirusMutation mutation, List<string> scrollBuffer, int currentLine, int totalLine)
        {
            int percent = currentLine * 100 / Math.Max(1, totalLine); // 진행률 계산
            int start = Math.Max(0, scrollBuffer.Count - MutationSourceViewportLines); // 최근 줄 시작
            string title = "MUTATION SOURCE GENERATOR // " + GetMutationDisplayName(mutation); // 타이틀

            BeginModalRedirect(title, ModalSize.Large); // 변이 코드 모달 시작
            WriteLine(" > writing " + GetMutationSourceFileName(mutation), ConsoleColor.DarkGray);
            WriteLine(" > source buffer scroll : recent " + MutationSourceViewportLines + " lines", ConsoleColor.DarkGray);
            WriteEmptyLine();

            for (int i = start; i < scrollBuffer.Count; i++) // 최근 코드 줄 출력
            {
                WriteLine(" " + scrollBuffer[i], GetMutationSourceColor(mutation, scrollBuffer[i])); // 코드 줄
            }

            for (int i = scrollBuffer.Count - start; i < MutationSourceViewportLines; i++) // 빈 코드 줄 보정
            {
                WriteLine("     |", ConsoleColor.DarkGray); // 빈 에디터 줄
            }

            WriteModalFooterText("SOURCE WRITE " + BuildMutationProgressBar(percent, 18) + " " + percent.ToString("000") + "%", ConsoleColor.Green); // 자동 처리 Footer
            EndModalRedirect(); // 변이 코드 모달 종료
            HideCursor(); // 커서 유배
        }

        private void RenderMutationBuildFrame(VirusMutation mutation, string[] logs, int visibleCount, bool flash)
        {
            BeginModalRedirect("MUTATION COMPILER // BUILD", ModalSize.Large); // 변이 빌드 모달 시작
            WriteLine(" target entity : VIRUS.EXE -> " + GetMutationEntityFileName(mutation), GetMutationColor(mutation));
            WriteLine(" source file   : " + GetMutationSourceFileName(mutation), ConsoleColor.DarkGray);
            WriteEmptyLine();

            for (int i = 0; i < logs.Length; i++) // 빌드 로그 출력
            {
                if (i < visibleCount) // 표시된 로그 체크
                {
                    WriteLine(" " + logs[i], logs[i].IndexOf("[OK]", StringComparison.Ordinal) >= 0 ? ConsoleColor.Green : ConsoleColor.Gray);
                }
                else
                {
                    WriteLine("", ConsoleColor.DarkGray); // 아직 미출력 줄
                }
            }

            WriteLine(" BUILD SUCCESS : " + GetMutationEntityFileName(mutation) + " READY", flash ? ConsoleColor.White : GetMutationColor(mutation));
            WriteLine(" COMBAT PROTOCOL REWRITTEN", ConsoleColor.Green);
            WriteModalFooterText(flash ? "PAYLOAD BUILD COMPLETE" : "PAYLOAD BUILD   PLEASE WAIT", flash ? ConsoleColor.Magenta : ConsoleColor.DarkGray); // 자동 처리 Footer
            EndModalRedirect(); // 변이 빌드 모달 종료
            HideCursor(); // 커서 유배
        }

        private List<string> GetMutationSourceLines(VirusMutation mutation)
        {
            int total = GetMutationSourceTotalLine(mutation); // 전체 코드 줄 수
            List<string> lines = new List<string>(); // 코드 줄 목록

            for (int line = 1; line <= total; line++) // 코드 줄 생성
            {
                lines.Add(FormatMutationSourceLine(line, BuildMutationSourceText(mutation, line))); // 줄 번호 포함
            }

            return lines; // 코드 줄
        }

        private int GetMutationSourceTotalLine(VirusMutation mutation)
        {
            if (mutation == VirusMutation.Ransomware) return 220; // 랜섬웨어 코드량
            if (mutation == VirusMutation.Trojan) return 180; // 트로젠 코드량
            if (mutation == VirusMutation.Adware) return 260; // 애드웨어 코드량
            return 160; // 기본 코드량
        }

        private string FormatMutationSourceLine(int line, string code)
        {
            return line.ToString("000") + " | " + code; // 코드 줄 포맷
        }

        private string BuildMutationSourceText(VirusMutation mutation, int line)
        {
            if (mutation == VirusMutation.Ransomware) return BuildRansomwareSourceText(line); // 랜섬웨어 코드
            if (mutation == VirusMutation.Trojan) return BuildTrojanSourceText(line); // 트로젠 코드
            if (mutation == VirusMutation.Adware) return BuildAdwareSourceText(line); // 애드웨어 코드
            return "noop();"; // 기본 코드
        }

        private string BuildRansomwareSourceText(int line)
        {
            if (line == 1) return "function Rewrite-Entity {";
            if (line == 2) return "    param($entity)";
            if (line == 3) return "";
            if (line == 4) return "    $entity.Name = \"RANSOMWARE.EXE\"";
            if (line == 5) return "    $entity.MaxHealth += 40";
            if (line == 6) return "    $entity.Attack -= 4";
            if (line == 7) return "}";
            if (line == 8) return "";
            if (line == 9) return "function Encrypt-Target {";
            if (line == 10) return "    param($target)";
            if (line == 11) return "";
            if (line == 12) return "    if ($target.IsEncrypted -eq $false) {";
            if (line == 13) return "        $target.Art.ReplaceRandom(\"$\")";
            if (line == 14) return "        $target.NextAttack.Reduce(50)";
            if (line == 15) return "        $target.IsEncrypted = $true";
            if (line == 16) return "    }";
            if (line == 17) return "}";
            if (line == 18) return "";
            if (line == 19) return "function Write-RansomNote {";
            if (line == 20) return "    param($target)";
            if (line == 21) return "";
            if (line == 22) return "    if ($target.IsEncrypted -eq $true) {";
            if (line == 23) return "        $wallet.KB += 20";
            if (line == 24) return "        $target.Art.Restore(\"$\")";
            if (line == 25) return "        $target.NextAttack.Reduce(50)";
            if (line == 26) return "    }";
            if (line == 27) return "}";
            if (line == 28) return "";
            if (line == 29) return "$skill[2] = \"ENCRYPT\"";
            if (line == 30) return "$skill[3] = \"RANSOM_NOTE\"";
            if (line == 31) return "";
            if (line == 32) return "for ($i = 0; $i -lt 64; $i++) {";
            if (line >= 33 && line <= 48) return "    $cipher.Block[" + (line - 33).ToString("00") + "].Inject(\"$\")";
            if (line == 49) return "    $cipher.CommitEncryptedBlock()";
            if (line == 50) return "}";
            if (line == 51) return "";
            if (line == 52) return "# ransom note templates generated";
            if (line == 53) return "$note[0] = \"$ KB REQUIRED $\"";
            if (line == 54) return "$note[1] = \"$ DATA LOCKED $\"";
            if (line == 55) return "$note[2] = \"$ PAY OR LOSE ACCESS $\"";
            if (line == 56) return "";

            if (line % 47 == 0) return "# generated encryption handler block x32";
            if (line % 43 == 0) return "if ($target.IsEncrypted -eq $true) { $wallet.KB += 20 }";
            if (line % 37 == 0) return "$target.Art.Restore(\"$\")";
            if (line % 31 == 0) return "$ransomNote.Show($note[" + (line % 3).ToString() + "])";
            if (line % 23 == 0) return "$target.NextAttack.Reduce(50)";
            if (line % 19 == 0) return "$cipher.RotateKey(" + line.ToString() + ")";
            if (line % 13 == 0) return "$target.Art.ReplaceRandom(\"$\")";
            if (line % 7 == 0) return "Write-RansomNote $target";
            if (line % 5 == 0) return "Encrypt-Target $target";

            return "$payload.Stream[" + line.ToString("000") + "].WriteEncryptedChunk()";
        }

        private string BuildTrojanSourceText(int line)
        {
            if (line == 1) return "struct ProcessMask";
            if (line == 2) return "{";
            if (line == 3) return "    bool trusted;";
            if (line == 4) return "    int  critRate;";
            if (line == 5) return "    int  damageGuard;";
            if (line == 6) return "};";
            if (line == 7) return "";
            if (line == 8) return "void RewriteEntity(Entity* entity)";
            if (line == 9) return "{";
            if (line == 10) return "    entity->Name = \"TROJAN.EXE\";";
            if (line == 11) return "    entity->MaxHealth -= 25;";
            if (line == 12) return "    entity->Attack += 6;";
            if (line == 13) return "}";
            if (line == 14) return "";
            if (line == 15) return "ProcessMask SpoofAuth(const char* processName)";
            if (line == 16) return "{";
            if (line == 17) return "    ProcessMask mask;";
            if (line == 18) return "    mask.trusted = true;";
            if (line == 19) return "    mask.critRate = 25;";
            if (line == 20) return "    mask.damageGuard = 50;";
            if (line == 21) return "    return mask;";
            if (line == 22) return "}";
            if (line == 23) return "";
            if (line == 24) return "void OpenBackdoor(ProcessMask* mask)";
            if (line == 25) return "{";
            if (line == 26) return "    if (mask->trusted)";
            if (line == 27) return "    {";
            if (line == 28) return "        Skill[2] = BACKDOOR;";
            if (line == 29) return "        Skill[3] = SPOOF_AUTH;";
            if (line == 30) return "    }";
            if (line == 31) return "}";
            if (line == 32) return "";
            if (line == 33) return "Entity* entity = GetProcess(\"VIRUS.EXE\");";
            if (line == 34) return "RewriteEntity(entity);";
            if (line == 35) return "ProcessMask mask = SpoofAuth(\"SYSTEM_SERVICE\");";
            if (line == 36) return "OpenBackdoor(&mask);";
            if (line == 37) return "";
            if (line == 38) return "for (int route = 0; route < 32; route++)";
            if (line == 39) return "{";
            if (line >= 40 && line <= 55) return "    BackdoorTable[" + (line - 40).ToString("00") + "].trusted = true;";
            if (line == 56) return "    BackdoorTable[route].critRate += 25;";
            if (line == 57) return "}";
            if (line == 58) return "";

            if (line % 41 == 0) return "/* auth mask table generated x16 */";
            if (line % 37 == 0) return "if (mask.trusted) { IncomingDamage *= 0.5f; }";
            if (line % 31 == 0) return "BindCriticalProtocol(200);";
            if (line % 29 == 0) return "CriticalRate += mask.critRate;";
            if (line % 23 == 0) return "ReduceIncomingDamage(mask.damageGuard);";
            if (line % 19 == 0) return "OpenBackdoor(&mask);";
            if (line % 13 == 0) return "mask = SpoofAuth(\"SYSTEM_SERVICE\");";
            if (line % 7 == 0) return "RouteTable[" + (line % 9).ToString() + "].SetTrusted(true);";
            if (line % 5 == 0) return "InjectHiddenAccess(vector_" + line.ToString("000") + ");";

            return "ShadowProcess[" + line.ToString("000") + "]->BlendIntoSystem();";
        }

        private string BuildAdwareSourceText(int line)
        {
            if (line == 1) return "const entity = load(\"VIRUS.EXE\");";
            if (line == 2) return "";
            if (line == 3) return "function rewriteEntity(entity) {";
            if (line == 4) return "    entity.name = \"ADWARE.EXE\";";
            if (line == 5) return "    entity.maxHealth += 20;";
            if (line == 6) return "    entity.attack += 2;";
            if (line == 7) return "}";
            if (line == 8) return "";
            if (line == 9) return "function spawnPopup(target) {";
            if (line == 10) return "    popup.spawn(\"SYSTEM WARNING\");";
            if (line == 11) return "    popup.spawn(\"FREE KB BOOST!!!\");";
            if (line == 12) return "    popup.spawn(\"ALLOW NOTIFICATION?\");";
            if (line == 13) return "";
            if (line == 14) return "    target.maxHealth -= 20;";
            if (line == 15) return "    target.nextAttackPower *= 0.6;";
            if (line == 16) return "}";
            if (line == 17) return "";
            if (line == 18) return "function pushNotification(target) {";
            if (line == 19) return "    target.notification.stack += 1;";
            if (line == 20) return "    target.notification.tickDamage = 10;";
            if (line == 21) return "}";
            if (line == 22) return "";
            if (line == 23) return "skill[2] = \"POPUP_FLOOD\";";
            if (line == 24) return "skill[3] = \"AD_NOTIFICATION\";";
            if (line == 25) return "";
            if (line == 26) return "for (let i = 0; i < 64; i++) {";
            if (line >= 27 && line <= 42) return "    popup.spawn(randomAdTemplate(" + (line - 27).ToString("00") + "));";
            if (line == 43) return "    screen.injectNoiseLayer(i);";
            if (line == 44) return "}";
            if (line == 45) return "";
            if (line == 46) return "while (notification.stack > 0) {";
            if (line == 47) return "    target.damage(10);";
            if (line == 48) return "    notification.flash();";
            if (line == 49) return "}";
            if (line == 50) return "";

            if (line % 53 == 0) return "/* popup template pack injected x64 */";
            if (line % 47 == 0) return "screen.overlay.push(\"AD_LAYER_" + (line % 12).ToString() + "\");";
            if (line % 41 == 0) return "notification.stackable = true;";
            if (line % 37 == 0) return "notification.tickDamage = 10;";
            if (line % 31 == 0) return "target.maxHealth = Math.max(10, target.maxHealth - 20);";
            if (line % 23 == 0) return "target.nextAttackPower *= 0.6;";
            if (line % 17 == 0) return "pushNotification(target);";
            if (line % 13 == 0) return "popup.spawn(\"ALLOW NOTIFICATION?\");";
            if (line % 7 == 0) return "popup.spawn(\"FREE KB BOOST!!!\");";
            if (line % 5 == 0) return "popup.spawn(\"SYSTEM WARNING\");";
            if (line % 3 == 0) return "screen.noise.inject(layer_" + (line % 8).ToString() + ");";

            return "adQueue.push(banner_" + line.ToString("000") + ");";
        }

        private int GetMutationSourceDelay(int currentLine, int totalLine)
        {
            if (currentLine < 12) return 72; // 초반 느림
            if (currentLine < 36) return 34; // 가속 시작
            if (currentLine < totalLine - 30) return 7; // 고속 스크롤
            if (currentLine < totalLine - 10) return 16; // 마무리 감속
            return 32; // 마지막 체크 속도
        }

        private string[] GetMutationBuildLogs(VirusMutation mutation)
        {
            if (mutation == VirusMutation.Ransomware) // 랜섬웨어 빌드 로그
            {
                return new string[]
                {
                    "source buffer flushed       [OK]",
                    "entity rewrite              [OK]",
                    "stat mutation               [OK]",
                    "cipher module compiled      [OK]",
                    "ransom note generated       [OK]",
                    "kb demand protocol linked   [OK]",
                    "payload link                [OK]"
                };
            }

            if (mutation == VirusMutation.Trojan) // 트로젠 빌드 로그
            {
                return new string[]
                {
                    "source buffer flushed       [OK]",
                    "entity rewrite              [OK]",
                    "auth mask compiled          [OK]",
                    "backdoor route linked       [OK]",
                    "critical protocol mapped    [OK]",
                    "spoof layer injected        [OK]",
                    "payload link                [OK]"
                };
            }

            return new string[] // 애드웨어 빌드 로그
            {
                "source buffer flushed       [OK]",
                "entity rewrite              [OK]",
                "popup module bundled        [OK]",
                "notification stack linked   [OK]",
                "screen noise layer injected [OK]",
                "ad overlay protocol mapped  [OK]",
                "payload link                [OK]"
            };
        }

        private string GetMutationSourceFileName(VirusMutation mutation)
        {
            if (mutation == VirusMutation.Ransomware) return "ransom_payload.ps1"; // 랜섬웨어 파일
            if (mutation == VirusMutation.Trojan) return "trojan_core.cpp"; // 트로젠 파일
            if (mutation == VirusMutation.Adware) return "adware_popup.js"; // 애드웨어 파일
            return "unknown_payload.src"; // 기본 파일
        }

        private string GetMutationEntityFileName(VirusMutation mutation)
        {
            if (mutation == VirusMutation.Ransomware) return "RANSOMWARE.EXE"; // 랜섬웨어 엔티티
            if (mutation == VirusMutation.Trojan) return "TROJAN.EXE"; // 트로젠 엔티티
            if (mutation == VirusMutation.Adware) return "ADWARE.EXE"; // 애드웨어 엔티티
            return "VIRUS.EXE"; // 기본 엔티티
        }

        private ConsoleColor GetMutationSourceColor(VirusMutation mutation, string line)
        {
            if (string.IsNullOrEmpty(line)) return ConsoleColor.DarkGray; // 빈 줄
            if (line.IndexOf("/*", StringComparison.Ordinal) >= 0 || line.IndexOf("#", StringComparison.Ordinal) >= 0) return ConsoleColor.DarkGray; // 주석
            if (line.IndexOf("function", StringComparison.Ordinal) >= 0 || line.IndexOf("void", StringComparison.Ordinal) >= 0 || line.IndexOf("struct", StringComparison.Ordinal) >= 0) return ConsoleColor.Cyan; // 선언부
            if (line.IndexOf("{", StringComparison.Ordinal) >= 0 || line.IndexOf("}", StringComparison.Ordinal) >= 0) return ConsoleColor.DarkGray; // 블록
            if (line.IndexOf("if", StringComparison.Ordinal) >= 0 || line.IndexOf("for", StringComparison.Ordinal) >= 0 || line.IndexOf("while", StringComparison.Ordinal) >= 0) return ConsoleColor.Yellow; // 제어문
            if (line.IndexOf("Rewrite", StringComparison.Ordinal) >= 0 || line.IndexOf("rewrite", StringComparison.Ordinal) >= 0 || line.IndexOf("Name", StringComparison.Ordinal) >= 0 || line.IndexOf("name", StringComparison.Ordinal) >= 0) return ConsoleColor.Cyan; // 엔티티 변경
            if (line.IndexOf("skill", StringComparison.Ordinal) >= 0 || line.IndexOf("Skill", StringComparison.Ordinal) >= 0) return ConsoleColor.Magenta; // 스킬 바인딩
            if (line.IndexOf("[OK]", StringComparison.Ordinal) >= 0) return ConsoleColor.Green; // 성공 로그

            if (mutation == VirusMutation.Ransomware) // 랜섬웨어 색상
            {
                if (line.IndexOf("$", StringComparison.Ordinal) >= 0) return ConsoleColor.Yellow; // 랜섬웨어 문법
                if (line.IndexOf("cipher", StringComparison.Ordinal) >= 0 || line.IndexOf("ransom", StringComparison.Ordinal) >= 0) return ConsoleColor.Yellow; // 암호화/협박
            }

            if (mutation == VirusMutation.Trojan) // 트로젠 색상
            {
                if (line.IndexOf("Auth", StringComparison.Ordinal) >= 0 || line.IndexOf("Backdoor", StringComparison.Ordinal) >= 0) return ConsoleColor.Magenta; // 인증/백도어
                if (line.IndexOf("trusted", StringComparison.Ordinal) >= 0 || line.IndexOf("mask", StringComparison.Ordinal) >= 0) return ConsoleColor.Magenta; // 위장
                if (line.IndexOf("->", StringComparison.Ordinal) >= 0 || line.IndexOf("*", StringComparison.Ordinal) >= 0) return ConsoleColor.Gray; // C/C++ 느낌
            }

            if (mutation == VirusMutation.Adware) // 애드웨어 색상
            {
                if (line.IndexOf("popup", StringComparison.Ordinal) >= 0 || line.IndexOf("notification", StringComparison.Ordinal) >= 0) return ConsoleColor.Green; // 팝업/알림
                if (line.IndexOf("screen", StringComparison.Ordinal) >= 0 || line.IndexOf("overlay", StringComparison.Ordinal) >= 0) return ConsoleColor.Yellow; // 화면 오염
            }

            return ConsoleColor.Gray; // 기본 코드
        }

        private string BuildMutationProgressBar(int percent, int length)
        {
            if (percent < 0) percent = 0; // 최소값 보정
            if (percent > 100) percent = 100; // 최대값 보정

            int filled = percent * length / 100; // 채워진 칸
            return "[" + new string('+', filled) + new string('-', length - filled) + "]"; // 진행 바
        }

        private void WriteMutationCardRows(int selectedIndex)
        {
            const int cardWidth = 26; // 카드 고정 폭
            const int cardGap = 3; // 카드 간격
            string[][] cards = new string[][] // 카드 아트
            {
                GetRansomwareCard(selectedIndex == 0, cardWidth),
                GetTrojanCard(selectedIndex == 1, cardWidth),
                GetAdwareCard(selectedIndex == 2, cardWidth)
            };

            int totalWidth = cardWidth * 3 + cardGap * 2; // 카드 묶음 전체 폭
            int leftPadding = Math.Max(0, (modalInnerWidth - totalWidth) / 2); // 모달 중앙 정렬
            string gap = new string(' ', cardGap); // 카드 사이 공백

            for (int row = 0; row < cards[0].Length; row++) // 카드 줄 출력
            {
                WriteSegmentsLine(
                    new ColorSegment(new string(' ', leftPadding), ConsoleColor.DarkGray),
                    new ColorSegment(cards[0][row], selectedIndex == 0 ? ConsoleColor.Yellow : ConsoleColor.DarkGray),
                    new ColorSegment(gap, ConsoleColor.DarkGray),
                    new ColorSegment(cards[1][row], selectedIndex == 1 ? ConsoleColor.Magenta : ConsoleColor.DarkGray),
                    new ColorSegment(gap, ConsoleColor.DarkGray),
                    new ColorSegment(cards[2][row], selectedIndex == 2 ? ConsoleColor.Green : ConsoleColor.DarkGray));
            }
        }

        private string[] GetRansomwareCard(bool selected, int cardWidth)
        {
            return BuildMutationCard(selected, cardWidth, "RANSOMWARE STRAIN", new string[]
            {
                "$#10$#10$#10$#10$#10$#10$#",
                "$  !!! FILES LOCKED !!!  $",
                "$                        $",
                "$  ####  YOUR DATA  $$$$ $",
                "$  1010  ENCRYPTED  #$#0 $",
                "$                        $",
                "$   1 BTC  PAY_OR_DIE    $",
                "$#10$#10$#10$#10$#10$#10$#"
            });
        }

        private string[] GetTrojanCard(bool selected, int cardWidth)
        {
            return BuildMutationCard(selected, cardWidth, "TROJAN STRAIN", new string[]
            {
                "+------------------------+",
                "| FROM: bank@totally.real|",
                "| SUBJ: Urgent! Open Now |",
                "| ====================== |",
                "| Dear user, click here: |",
                "|  >> INVOICE_2024.exe<< |",
                "|     ^^ TROJAN ^^       |",
                "+------------------------+"
            });
        }

        private string[] GetAdwareCard(bool selected, int cardWidth)
        {
            return BuildMutationCard(selected, cardWidth, "ADWARE STRAIN", new string[]
            {
                "+---------+    +---------+",
                "| BUY NOW |----+--+ AD!! |",
                "|   $$$   | CLICK!|      |",
                "+---------+       |+-----+",
                "  |  +----------+ |    |  ",
                "  +--| FREE PC  |-+  +---+",
                "     | SCAN !!! |----| $ |",
                "     +----------+    +---+"
            });
        }

        private string[] BuildMutationCard(bool selected, int cardWidth, string title, string[] artLines)
        {
            List<string> lines = new List<string>(); // 카드 줄 목록
            string titleText = selected ? ">> " + title + " <<" : "   " + title + "   "; // 선택 강조
            lines.Add(CenterMutationCardText(titleText, cardWidth)); // 제목 중앙 정렬

            for (int i = 0; i < artLines.Length; i++) // 아트 줄 순회
            {
                lines.Add(CenterMutationCardText(artLines[i], cardWidth)); // 아트 중앙 정렬
            }

            return lines.ToArray(); // 카드
        }

        private string CenterMutationCardText(string text, int width)
        {
            if (text == null) text = string.Empty; // null 방지

            int displayWidth = TextUtil.GetDisplayWidth(text); // 표시 폭
            if (displayWidth >= width) return TextUtil.Fit(text, width); // 초과 시 보정

            int left = (width - displayWidth) / 2; // 좌측 여백
            return TextUtil.Fit(new string(' ', left) + text, width); // 중앙 정렬
        }

        private void WriteMutationDetailPanel(int selectedIndex)
        {
            VirusMutation mutation = GetMutationByIndex(selectedIndex); // 선택 변이
            string[] lines = GetMutationDetailLines(mutation); // 상세 설명
            ConsoleColor color = GetMutationColor(mutation); // 변이 색상

            for (int i = 0; i < lines.Length; i++) // 설명 줄 출력
            {
                WriteLine(" " + lines[i], i == 0 ? color : ConsoleColor.Gray); // 제목 강조
            }
        }

        private string[] GetMutationDetailLines(VirusMutation mutation)
        {
            if (mutation == VirusMutation.Ransomware) // 랜섬웨어 설명
            {
                return new string[]
                {
                    "RANSOMWARE STRAIN",
                    "ROLE    : 제어 / 흡혈 / KB 회수 / 안정형 페이로드",
                    "STYLE   : ENCRYPT 후 RANSOM_NOTE로 뜯어내는 생존형 빌드",
                    "PASSIVE : MaxHealth +40 / ATK -4",
                    "SKILL 1 : ENCRYPT      - ATK x" + SkillBalanceData.FormatMultiplier(SkillBalanceData.RansomwareEncryptMultiplierPercent) + " / ENERGY " + SkillBalanceData.RansomwareEncryptEnergyCost + " / 적 다음 공격 " + SkillBalanceData.RansomwareEncryptAttackReductionPercent + "% 감소",
                    "SKILL 2 : RANSOM_NOTE  - ATK x" + SkillBalanceData.FormatMultiplier(SkillBalanceData.RansomwareNoteMultiplierPercent) + " / ENERGY " + SkillBalanceData.RansomwareNoteEnergyCost + " / KB +" + SkillBalanceData.RansomwareNoteKbGain + " / 피해 " + SkillBalanceData.RansomwareNoteHealPercent + "% 회복"
                };
            }

            if (mutation == VirusMutation.Trojan) // 트로젠 설명
            {
                return new string[]
                {
                    "TROJAN STRAIN",
                    "ROLE    : 순수 폭딜 / 치명타 / 디버프 없는 전사형 페이로드",
                    "STYLE   : BACKDOOR 중심의 고위험 치명타 공격 빌드",
                    "PASSIVE : MaxHealth -25 / ATK +6",
                    "SKILL 1 : BACKDOOR    - ATK x" + SkillBalanceData.FormatMultiplier(SkillBalanceData.TrojanBackdoorMultiplierPercent) + " / ENERGY " + SkillBalanceData.TrojanBackdoorEnergyCost + " / 치명 " + SkillBalanceData.TrojanCriticalChance + "% / 피해 x" + SkillBalanceData.FormatMultiplier(SkillBalanceData.TrojanCriticalMultiplierPercent) ,
                    "SKILL 2 : SPOOF_AUTH  - ENERGY " + SkillBalanceData.TrojanSpoofAuthEnergyCost + " / 다음 BACKDOOR x" + SkillBalanceData.FormatMultiplier(SkillBalanceData.TrojanSpoofBackdoorMultiplierPercent) + " / 치명 +" + SkillBalanceData.TrojanSpoofCriticalBonusPercent + "%"
                };
            }

            return new string[] // 애드웨어 설명
            {
                "ADWARE STRAIN",
                "ROLE    : 교란 / 반복 피해 / 상태이상 누적형 페이로드",
                "STYLE   : POPUP 방해 + AD_NOTIFICATION 누적 피해 빌드",
                "PASSIVE : MaxHealth +20 / ATK +2",
                "SKILL 1 : POPUP_FLOOD     - ATK x" + SkillBalanceData.FormatMultiplier(SkillBalanceData.AdwarePopupFloodMultiplierPercent) + " / ENERGY " + SkillBalanceData.AdwarePopupFloodEnergyCost + " / 적 다음 공격 " + SkillBalanceData.AdwarePopupFloodAttackReductionPercent + "% 감소",
                "SKILL 2 : AD_NOTIFICATION - ATK x" + SkillBalanceData.FormatMultiplier(SkillBalanceData.AdNotificationMultiplierPercent) + " / ENERGY " + SkillBalanceData.AdNotificationEnergyCost + " / 매턴 ATK x" + SkillBalanceData.FormatMultiplier(SkillBalanceData.AdNotificationTickPercent) + " / 최대 " + SkillBalanceData.AdNotificationMaxStacks + "중첩"
            };
        }

        private void WriteMutationControlLine()
        {
            WriteModalFooter(
                new ColorSegment(" A/D", ConsoleColor.Cyan),
                new ColorSegment(" 이동   ", ConsoleColor.DarkGray),
                new ColorSegment("E", ConsoleColor.Green),
                new ColorSegment(" 변이 확정", ConsoleColor.DarkGray));
        }

        private VirusMutation GetMutationByIndex(int index)
        {
            if (index == 0) return VirusMutation.Ransomware; // 랜섬웨어
            if (index == 1) return VirusMutation.Trojan; // 트로젠
            return VirusMutation.Adware; // 애드웨어
        }

        private string GetMutationDisplayName(VirusMutation mutation)
        {
            if (mutation == VirusMutation.Ransomware) return "RANSOMWARE STRAIN"; // 랜섬웨어
            if (mutation == VirusMutation.Trojan) return "TROJAN STRAIN"; // 트로젠
            if (mutation == VirusMutation.Adware) return "ADWARE STRAIN"; // 애드웨어
            return "UNKNOWN STRAIN"; // 알 수 없음
        }

        private string GetMutationSkillTwoName(VirusMutation mutation)
        {
            if (mutation == VirusMutation.Ransomware) return "ENCRYPT"; // 랜섬웨어 2번
            if (mutation == VirusMutation.Trojan) return "BACKDOOR"; // 트로젠 2번
            if (mutation == VirusMutation.Adware) return "POPUP_FLOOD"; // 애드웨어 2번
            return "UNKNOWN"; // 알 수 없음
        }

        private string GetMutationSkillThreeName(VirusMutation mutation)
        {
            if (mutation == VirusMutation.Ransomware) return "RANSOM_NOTE"; // 랜섬웨어 3번
            if (mutation == VirusMutation.Trojan) return "SPOOF_AUTH"; // 트로젠 3번
            if (mutation == VirusMutation.Adware) return "AD_NOTIFICATION"; // 애드웨어 3번
            return "UNKNOWN"; // 알 수 없음
        }

        private ConsoleColor GetMutationColor(VirusMutation mutation)
        {
            if (mutation == VirusMutation.Ransomware) return ConsoleColor.Yellow; // 랜섬웨어 색상
            if (mutation == VirusMutation.Trojan) return ConsoleColor.Magenta; // 트로젠 색상
            if (mutation == VirusMutation.Adware) return ConsoleColor.Green; // 애드웨어 색상
            return ConsoleColor.Gray; // 기본 색상
        }
    }
}
