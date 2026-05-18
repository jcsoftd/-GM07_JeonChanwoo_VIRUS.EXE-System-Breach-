using System;

namespace VirusExe.SystemBreach.Rendering
{
    // 보스 ASCII 아트
    // KERNEL_CORE 페이즈별 비주얼과 공격/피격 모션 관리
    public partial class ConsoleRenderer
    {
        private string[] BuildBossArt(int phase, int frame)
        {
            if (phase >= 2) // 2페이즈 이상 체크
                return BuildBossPhase2Art(frame); // 2페이즈 눈 아트

            return BuildBossPhase1Art(frame); // 1페이즈 밀봉 코어
        }


        private string[] BuildBossPhase1Art(int frame)
        {
            int motion = (frame / 4) % 5; // 보스 호흡 속도 완화

            if (motion == 0) return BuildSealedCoreFrame(30.0, 5.8, motion); // 최소 응축
            if (motion == 1) return BuildSealedCoreFrame(34.0, 6.5, motion); // 기본 크기
            if (motion == 2) return BuildSealedCoreFrame(38.0, 7.0, motion); // 최대 팽창
            if (motion == 3) return BuildSealedCoreFrame(34.0, 6.5, motion); // 기본 수축

            return BuildSealedCoreFrame(30.0, 5.8, motion); // 다시 응축
        }


        private string[] BuildSealedCoreFrame(double radiusX, double radiusY, int motion)
        {
            const int width = 96; // 보스 아트 고정 폭
            const int height = 15; // 보스 아트 고정 높이
            const double centerX = (width - 1) / 2.0; // 좌우 대칭 중심축
            const double centerY = (height - 1) / 2.0; // 상하 대칭 중심축

            string[] art = new string[height]; // 생성 결과

            for (int y = 0; y < height; y++) // 세로 줄 생성
            {
                char[] chars = new string(' ', width).ToCharArray(); // 공백 바탕
                double dy = y - centerY; // 중심 기준 Y 거리

                for (int x = 0; x < width; x++) // 가로 문자 생성
                {
                    double dx = x - centerX; // 중심 기준 X 거리
                    double normalized = (dx * dx) / (radiusX * radiusX) + (dy * dy) / (radiusY * radiusY); // 타원 내부 체크

                    if (normalized > 1.0) continue; // 타원 밖은 공백 유지

                    chars[x] = GetSealedCoreChar(normalized, motion); // 밀도 문자 적용
                }

                art[y] = new string(chars); // 줄 저장
            }

            ApplySealedCoreText(art, motion); // 내부 상태 문구 적용

            return art; // 보스 아트
        }


        private char GetSealedCoreChar(double normalized, int motion)
        {
            if (normalized >= 0.88) return '@'; // 가장 바깥 단단한 껍질
            if (normalized >= 0.72) return '#'; // 외곽 장갑층

            if (motion == 2) // 최대 팽창 시 내부 과열
            {
                if (normalized >= 0.50) return '@'; // 내부 압력 상승
                if (normalized >= 0.28) return '%'; // 중심 밀도 상승
                return '$'; // 중심 오염 신호
            }

            if (motion == 0 || motion == 4) // 응축 시 단단한 내부
            {
                if (normalized >= 0.52) return '%'; // 내부 밀도층
                if (normalized >= 0.25) return '*'; // 중심 압축층
                return '#'; // 응축 코어
            }

            if (motion == 3) // 수축 중 오염 잔류
            {
                if (normalized >= 0.52) return '%'; // 내부 밀도층
                if (normalized >= 0.25) return '*'; // 중심 압축층
                return '$'; // 잔류 오염
            }

            if (normalized >= 0.52) return '%'; // 기본 내부 밀도층
            if (normalized >= 0.25) return '*'; // 기본 중심 압축층
            return '#'; // 기본 코어
        }


        private void ApplySealedCoreText(string[] art, int motion)
        {
            string line1 = " [ KERNEL_LOCK ] "; // 1줄 상태 문구
            string line2 = "   CORE_SEALED   "; // 2줄 상태 문구
            string line3 = "   ACCESS_DENIE  "; // 3줄 상태 문구

            if (motion == 0) // 응축 오염
            {
                line1 = " [ KE$NEL_LOCK ] "; // 텍스트 오염
                line2 = "   CORE_SEA$ED   "; // 텍스트 오염
                line3 = "  ACCESS_D$NIED  "; // 텍스트 오염
            }
            else if (motion == 2) // 최대 팽창 오염
            {
                line1 = " [ K$RN$L_L$CK ] "; // 텍스트 오염 강화
                line2 = "   C$RE_$EA$ED   "; // 텍스트 오염 강화
                line3 = "  ACC$SS_D$NI$D  "; // 텍스트 오염 강화
            }
            else if (motion == 3) // 수축 중 오염
            {
                line1 = " [ K$RNEL_L$CK ] "; // 텍스트 오염
                line2 = "   C$RE_SEA$ED   "; // 텍스트 오염
                line3 = "  ACC$SS_DENI$D  "; // 텍스트 오염
            }

            OverlayCenteredCoreText(art, 6, line1); // 중앙 1줄 배치
            OverlayCenteredCoreText(art, 7, line2); // 중앙 2줄 배치
            OverlayCenteredCoreText(art, 8, line3); // 중앙 3줄 배치
        }


        private void OverlayCenteredCoreText(string[] art, int row, string text)
        {
            if (art == null) return; // null 방지
            if (row < 0 || row >= art.Length) return; // 범위 체크
            if (string.IsNullOrEmpty(text)) return; // 빈 문구 방지

            char[] chars = art[row].ToCharArray(); // 대상 줄 문자
            int start = Math.Max(0, (chars.Length - text.Length) / 2); // 중앙 위치

            for (int i = 0; i < text.Length && start + i < chars.Length; i++) // 문구 삽입
            {
                chars[start + i] = text[i]; // 문자 교체
            }

            art[row] = new string(chars); // 줄 갱신
        }


        private static readonly string[] BossPhase3Shell = new string[]
        {
            "               . . . :: ++ %% ## @@@@ ## %% ++ :: . . .             ",
            "       [-][#] . ::: ++ %% ## @@▓▓▓▓▓▓▓@@ ## %% ++ ::: . [#][-]      ",
            "     [#] . :: ++ % ## @@▓▒░░░░░@@@@@@@░░░░░▒▓@@ ## % ++ :: . [#]    ",
            "   [#] : ++ %% # @@▓▒░    <<< ROOT_EYE >>>     ░▒▓@@ # %% ++ : [#]  ",
            " [#] : ++ % # @@▓▒░     .---==  11111  ==---.     ░▒▓@@ # % ++ : [#]",
            "|=| :: + % # @@▒░    [!] PRIVILEGE_DROP_OPEN [!]   ░▒@@ # % + :: |=|",
            "|=| : + % # @▒░         >>> CORE_OVERDRIVE <<<       ░▒@ # % + : |=|",
            "══════════▓▒░░        <<<    @ SYSTEM @     >>>      ░░▒▓═══════════",
            "|=| : + % # @▒░           >>> KERNEL_MELT <<<        ░▒@ # % + : |=|",
            "|=| :: + % # @@▒░    [!] SIGNAL_COLLAPSE_RUN [!]   ░▒@@ # % + :: |=|",
            " [#] : ++ % # @@▓▒░     '---==  00000  ==---'    ░▒▓@@ # % ++ : [#] ",
            "   [#] : ++ %% # @@▓▒░    <<< DATA_EYE >>>    ░▒▓@@ # %% ++ : [#]   ",
            "     [#] . :: ++ % ## @@▓▒░░░░░@@@@@@@░░░░░▒▓@@ ## % ++ :: . [#]    ",
            "       [-][#] . ::: ++ %% ## @@▓▓▓▓▓▓▓@@ ## %% ++ ::: . [#][-]      ",
            "               . . . :: ++ %% ## @@@@ ## %% ++ :: . . .             "
        };


        private static readonly string[] BossPhase3PupilLayer = new string[]
        {
            "       [!] PRIVILEGE_DROP_OPEN [!]       ",
            "          >>> CORE_OVERDRIVE <<<         ",
            "     <<<      @ SYSTEM @      >>>        ",
            "          >>> KERNEL_MELT <<<            ",
            "       [!] SIGNAL_COLLAPSE_RUN [!]       ",
            "          '---==  00000  ==---'          ",
            "             <<< DATA_EYE >>>            ",
            "          .---==  11111  ==---.          ",
            "             <<< ROOT_EYE >>>            "
        };


        private static readonly string[] BossPhase3AttackPupilLayer = new string[]
        {
            "       [!] ROOT_EXECUTE_OPEN [!]         ",
            "          >>> PRIVILEGE_DROP <<<         ",
            "     <<<      @ PURGE @       >>>        ",
            "          >>> KERNEL_MELT <<<            ",
            "       [!] SIGNAL_COLLAPSE_RUN [!]       ",
            "          '---==  FATAL  ==---'          ",
            "            <<< SYSTEM_KILL >>>          ",
            "          .---==  ERROR  ==---.          ",
            "             <<< CORE_EYE >>>            "
        };


        private static readonly string[] BossPhase3HitPupilLayer = new string[]
        {
            "       [!] PRIV?LEGE_DROP [!]            ",
            "          >>> CORE_D?MAGED <<<           ",
            "     <<<      @ SYST?M @      >>>        ",
            "          >>> KERNEL_M!LT <<<            ",
            "       [!] SIGNAL_C?LLAPSE [!]           ",
            "          '---==  X000X  ==---'          ",
            "            <<< DATA_LOSS >>>            ",
            "          .---==  X111X  ==---.          ",
            "             <<< R?OT_EYE >>>            "
        };


        private static readonly int[,] BossPhase3IdleOffsets = new int[,]
        {
            { 0, 0 },
            { 7, -2 },
            { 11, 0 },
            { 7, 2 },
            { 0, 3 },
            { -7, 2 },
            { -11, 0 },
            { -7, -2 }
        };


        private static readonly int[,] BossPhase3AttackOffsets = new int[,]
        {
            { 0, 0 },
            { 5, -1 },
            { -9, 0 },
            { -14, 0 },
            { 9, 1 },
            { 0, 2 }
        };


        private static readonly int[,] BossPhase3HitOffsets = new int[,]
        {
            { -11, -1 },
            { 12, 1 },
            { -7, 2 },
            { 8, -2 },
            { 0, 0 }
        };


        private string[] BuildBossFinalArt(int frame)
        {
            return BuildBossPhase3LayeredArt(frame, BossPhase3PupilLayer, BossPhase3IdleOffsets, 1, false, false); // 3페이즈 대기
        }


        private string[] BuildBossPhase3AttackArt(int frame)
        {
            return BuildBossPhase3LayeredArt(frame, BossPhase3AttackPupilLayer, BossPhase3AttackOffsets, 4, true, false); // 3페이즈 공격
        }


        private string[] BuildBossPhase3HitArt(int frame)
        {
            return BuildBossPhase3LayeredArt(frame, BossPhase3HitPupilLayer, BossPhase3HitOffsets, 7, false, true); // 3페이즈 피격
        }


        private string[] BuildBossPhase3LayeredArt(int frame, string[] pupilLayer, int[,] offsets, int glitchLevel, bool attackMode, bool hitMode)
        {
            char[][] result = BuildBossPhase3ShellGrid(); // 고정 눈 틀 생성
            ClearBossPhase3PupilArea(result); // 내부 동공 영역 비움
            ApplyBossPhase3ShellGlitch(result, frame, glitchLevel); // 외곽 글리치 적용
            ApplyBossPhase3PupilLayer(result, pupilLayer, offsets, frame, glitchLevel); // 움직이는 동공 레이어 적용

            if (attackMode) ApplyBossPhase3AttackMarks(result, frame); // 공격 신호 적용
            if (hitMode) ApplyBossPhase3HitMarks(result, frame); // 피격 충격 적용

            return BossPhase3ToStringArray(result); // 최종 아트 반환
        }


        private char[][] BuildBossPhase3ShellGrid()
        {
            char[][] result = new char[BossPhase3Shell.Length][]; // 결과 격자

            for (int y = 0; y < BossPhase3Shell.Length; y++) // 줄 복사
            {
                result[y] = BossPhase3Shell[y].ToCharArray(); // 고정 틀 복사
            }

            return result; // 복사된 틀
        }


        private void ClearBossPhase3PupilArea(char[][] result)
        {
            for (int y = 0; y < result.Length; y++) // 전체 줄 검사
            {
                for (int x = 0; x < result[y].Length; x++) // 전체 칸 검사
                {
                    if (IsBossPhase3PupilMask(x, y)) // 동공 표시 영역 확인
                    {
                        result[y][x] = ' '; // 내부 문구 제거
                    }
                }
            }
        }


        private void ApplyBossPhase3PupilLayer(char[][] result, string[] pupilLayer, int[,] offsets, int frame, int glitchLevel)
        {
            int motion = frame % offsets.GetLength(0); // 동공 이동 단계
            int offsetX = offsets[motion, 0]; // 동공 X 이동
            int offsetY = offsets[motion, 1]; // 동공 Y 이동
            int startX = 18 + offsetX; // 동공 기준 X
            int startY = 3 + offsetY; // 동공 기준 Y

            for (int py = 0; py < pupilLayer.Length; py++) // 동공 레이어 Y
            {
                int targetY = startY + py; // 실제 출력 Y

                if (targetY < 0 || targetY >= result.Length) continue; // 위아래 잘림

                for (int px = 0; px < pupilLayer[py].Length; px++) // 동공 레이어 X
                {
                    int targetX = startX + px; // 실제 출력 X

                    if (targetX < 0 || targetX >= result[targetY].Length) continue; // 좌우 잘림
                    if (!IsBossPhase3PupilMask(targetX, targetY)) continue; // 눈 내부 마스크 확인

                    char ch = pupilLayer[py][px]; // 동공 문자

                    if (ch == ' ') continue; // 공백은 기존 틀 유지

                    result[targetY][targetX] = GetBossPhase3PupilGlitchChar(ch, frame + glitchLevel, px, py); // 글리치 문자 적용
                }
            }
        }


        private bool IsBossPhase3PupilMask(int x, int y)
        {
            switch (y)
            {
                case 2:
                    return x >= 25 && x <= 42;

                case 3:
                    return x >= 21 && x <= 46;

                case 4:
                    return x >= 19 && x <= 49;

                case 5:
                    return x >= 18 && x <= 50;

                case 6:
                    return x >= 17 && x <= 51;

                case 7:
                    return x >= 14 && x <= 53;

                case 8:
                    return x >= 17 && x <= 51;

                case 9:
                    return x >= 18 && x <= 50;

                case 10:
                    return x >= 19 && x <= 49;

                case 11:
                    return x >= 21 && x <= 46;

                case 12:
                    return x >= 25 && x <= 42;

                default:
                    return false;
            }
        }


        private char GetBossPhase3PupilGlitchChar(char ch, int frame, int x, int y)
        {
            int value = x * 7 + y * 13 + frame * 5; // 고정 글리치 패턴

            if (ch == '@' && value % 4 == 0) return 'O'; // 중앙 눈 변조
            if (ch == '0' && value % 5 == 0) return '1'; // 바이너리 반전
            if (ch == '1' && value % 5 == 0) return '0'; // 바이너리 반전
            if (value % 41 == 0) return '?'; // 문자 손상
            if (value % 37 == 0) return 'X'; // 문자 파손
            if (value % 31 == 0) return '!'; // 경고 노이즈

            return ch; // 기본 문자
        }


        private void ApplyBossPhase3ShellGlitch(char[][] result, int frame, int glitchLevel)
        {
            char[] glitchChars = new char[] { '#', '%', '+', '@', '▓', '▒', '░', '!', '?', 'X' }; // 외곽 글리치 후보
            int threshold = Math.Max(23, 92 - glitchLevel * 14); // 모드별 글리치 빈도

            for (int y = 0; y < result.Length; y++) // 전체 줄 검사
            {
                for (int x = 0; x < result[y].Length; x++) // 전체 칸 검사
                {
                    if (IsBossPhase3PupilMask(x, y)) continue; // 내부 동공 영역 제외

                    char ch = result[y][x]; // 현재 외곽 문자

                    if (ch == ' ') continue; // 공백 제외
                    if (ch == '[' || ch == ']' || ch == '|') continue; // 외곽 기준 문자 유지

                    int value = x * 3 + y * 11 + frame * 7 + glitchLevel * 17; // 외곽 글리치 패턴

                    if (value % threshold == 0) // 모드별 빈도 변조
                    {
                        result[y][x] = glitchChars[(x + y + frame + glitchLevel) % glitchChars.Length]; // 외곽 문자 변조
                    }
                }
            }
        }


        private void ApplyBossPhase3AttackMarks(char[][] result, int frame)
        {
            int motion = frame % 6; // 공격 모션 단계

            if (motion == 0) // 조준 단계
            {
                OverlayBossPhase3TextAt(result, 6, 4, "<<<<===="); // 좌측 예열
                OverlayBossPhase3TextAt(result, 8, 56, "====>>>>"); // 우측 예열
                return;
            }

            if (motion == 1) // 압축 단계
            {
                OverlayBossPhase3TextAt(result, 5, 2, "<<<<====####"); // 상단 압축
                OverlayBossPhase3TextAt(result, 7, 0, "<<<<<<====####@@@@"); // 중앙 압축
                OverlayBossPhase3TextAt(result, 9, 2, "<<<<====####"); // 하단 압축
                return;
            }

            if (motion == 2 || motion == 3) // 발사 단계
            {
                OverlayBossPhase3TextAt(result, 4, 0, "<<<<====####@@@@"); // 상단 발사
                OverlayBossPhase3TextAt(result, 6, 0, "<<<<<<====####@@@@@@"); // 상중단 발사
                OverlayBossPhase3TextAt(result, 7, 0, "<<<<<<<<====####@@@@@@@@"); // 중앙 발사
                OverlayBossPhase3TextAt(result, 8, 0, "<<<<<<====####@@@@@@"); // 하중단 발사
                OverlayBossPhase3TextAt(result, 10, 0, "<<<<====####@@@@"); // 하단 발사
                return;
            }

            OverlayBossPhase3TextAt(result, 7, 3, "<<<==##@@"); // 잔류 신호
            OverlayBossPhase3TextAt(result, 7, 56, "@@##==>>>"); // 반동 신호
        }


        private void ApplyBossPhase3HitMarks(char[][] result, int frame)
        {
            int motion = frame % 5; // 피격 모션 단계

            if (motion == 0) // 충격 진입
            {
                OverlayBossPhase3TextAt(result, 5, 4, "!!X"); // 좌상 충격
                OverlayBossPhase3TextAt(result, 5, 61, "X!!"); // 우상 충격
                OverlayBossPhase3TextAt(result, 9, 4, "!!X"); // 좌하 충격
                OverlayBossPhase3TextAt(result, 9, 61, "X!!"); // 우하 충격
                return;
            }

            if (motion == 1 || motion == 2) // 충격 최대
            {
                OverlayBossPhase3TextAt(result, 4, 1, "<<!!XX"); // 상단 좌측 파손
                OverlayBossPhase3TextAt(result, 4, 61, "XX!!>>"); // 상단 우측 파손
                OverlayBossPhase3TextAt(result, 7, 0, "<<<!!XX"); // 중앙 좌측 파손
                OverlayBossPhase3TextAt(result, 7, 60, "XX!!>>>"); // 중앙 우측 파손
                OverlayBossPhase3TextAt(result, 10, 1, "<<!!XX"); // 하단 좌측 파손
                OverlayBossPhase3TextAt(result, 10, 61, "XX!!>>"); // 하단 우측 파손
                return;
            }

            OverlayBossPhase3TextAt(result, 7, 5, "<!X"); // 잔류 좌측 충격
            OverlayBossPhase3TextAt(result, 7, 60, "X!>"); // 잔류 우측 충격
        }


        private void OverlayBossPhase3TextAt(char[][] result, int row, int col, string text)
        {
            if (result == null) return; // null 방지
            if (row < 0 || row >= result.Length) return; // 줄 범위 확인
            if (string.IsNullOrEmpty(text)) return; // 빈 문구 방지

            if (col < 0) col = 0; // 시작 위치 보정

            for (int i = 0; i < text.Length && col + i < result[row].Length; i++) // 문구 삽입
            {
                result[row][col + i] = text[i]; // 문자 교체
            }
        }


        private string[] BossPhase3ToStringArray(char[][] result)
        {
            string[] lines = new string[result.Length]; // 결과 문자열

            for (int i = 0; i < result.Length; i++) // 줄 변환
            {
                lines[i] = new string(result[i]); // 문자 배열 변환
            }

            return lines; // 문자열 배열
        }


        private string[] BuildAttackBossArt(int phase, int frame)
        {
            if (phase >= 3) // 3페이즈 체크
            {
                return BuildBossPhase3AttackArt(frame); // 3페이즈 동공 공격
            }

            if (phase >= 2) // 2페이즈 체크
            {
                return BuildBossPhase2AttackArt(frame); // 2페이즈 눈 공격
            }

            return BuildBossPhase1AttackArt(frame); // 1페이즈 밀봉 코어 공격
        }


        private string[] BuildBossPhase1AttackArt(int frame)
        {
            int motion = frame % 2; // 공격모션 2단계

            if (motion == 0) // 공격 신호 압축
            {
                return BuildSealedCoreAttackFrame(34.0, 6.5, motion); // 압축 코어
            }

            return BuildSealedCoreAttackFrame(36.0, 6.8, motion); // 신호탄 발사
        }


        private string[] BuildSealedCoreAttackFrame(double radiusX, double radiusY, int motion)
        {
            const int width = 96; // 보스 아트 고정 폭
            const int height = 15; // 보스 아트 고정 높이
            const double centerX = (width - 1) / 2.0; // 좌우 대칭 중심축
            const double centerY = (height - 1) / 2.0; // 상하 대칭 중심축

            string[] art = new string[height]; // 생성 결과

            for (int y = 0; y < height; y++) // 세로 줄 생성
            {
                char[] chars = new string(' ', width).ToCharArray(); // 공백 바탕
                double dy = y - centerY; // 중심 기준 Y 거리

                for (int x = 0; x < width; x++) // 가로 문자 생성
                {
                    double dx = x - centerX; // 중심 기준 X 거리
                    double normalized = (dx * dx) / (radiusX * radiusX) + (dy * dy) / (radiusY * radiusY); // 타원 내부 체크

                    if (normalized > 1.0) continue; // 타원 밖은 공백 유지

                    chars[x] = GetSealedCoreAttackChar(normalized, motion); // 공격 상태 문자 적용
                }

                art[y] = new string(chars); // 줄 저장
            }

            ApplySealedCoreAttackText(art, motion); // 공격 상태 문구 적용
            ApplySealedCoreAttackBeam(art, motion); // 왼쪽 공격 신호 적용

            return art; // 공격 아트
        }


        private char GetSealedCoreAttackChar(double normalized, int motion)
        {
            if (normalized >= 0.88) return '@'; // 외곽 장갑 유지
            if (normalized >= 0.72) return '#'; // 장갑층 유지

            if (motion == 1) // 발사 순간 내부 과열
            {
                if (normalized >= 0.50) return '@'; // 내부 출력 상승
                if (normalized >= 0.28) return '%'; // 압축 신호층
                return '$'; // 공격 페이로드 중심
            }

            if (normalized >= 0.50) return '%'; // 공격 전 압축층
            if (normalized >= 0.28) return '*'; // 에너지 집속층
            return '$'; // 중심 페이로드
        }


        private void ApplySealedCoreAttackText(string[] art, int motion)
        {
            if (motion == 0) // 공격 준비
            {
                OverlayCenteredCoreText(art, 6, " [ PAYLOAD_CHARGE ] "); // 공격 준비 1줄
                OverlayCenteredCoreText(art, 7, "   SIGNAL_COMPRESS  "); // 공격 준비 2줄
                OverlayCenteredCoreText(art, 8, "    VECTOR_LOCKED   "); // 공격 준비 3줄
                return;
            }

            OverlayCenteredCoreText(art, 6, " [ PAYLOAD_FIRE ] "); // 공격 발사 1줄
            OverlayCenteredCoreText(art, 7, "   SIGNAL_BURST   "); // 공격 발사 2줄
            OverlayCenteredCoreText(art, 8, " VECTOR_RELEASED  "); // 공격 발사 3줄
        }


        private void ApplySealedCoreAttackBeam(string[] art, int motion)
        {
            if (motion == 0) // 압축 단계 체크
            {
                OverlayBossTextAt(art, 7, 5, "<<<<===="); // 약한 예열 신호
                return;
            }

            OverlayBossTextAt(art, 6, 2, "<<<<<<====####"); // 상단 발사 신호
            OverlayBossTextAt(art, 7, 0, "<<<<<<<<====####@@@@"); // 중앙 발사 신호
            OverlayBossTextAt(art, 8, 2, "<<<<<<====####"); // 하단 발사 신호
        }


        private string[] BuildHitBossArt(int phase, int frame)
        {
            if (phase >= 3) // 3페이즈 체크
            {
                return BuildBossPhase3HitArt(frame); // 3페이즈 동공 피격
            }

            if (phase >= 2) // 2페이즈 체크
            {
                return BuildBossPhase2HitArt(frame); // 2페이즈 눈 피격
            }

            return BuildBossPhase1HitArt(frame); // 1페이즈 밀봉 코어 피격
        }


        private string[] BuildBossPhase1HitArt(int frame)
        {
            int motion = frame % 3; // 피격모션 3단계

            if (motion == 0) // 충격 진입
            {
                return BuildSealedCoreHitFrame(34.0, 6.5, motion); // 기본 충격
            }

            if (motion == 1) // 충격 최대치
            {
                return BuildSealedCoreHitFrame(36.0, 6.8, motion); // 외곽 충격파
            }

            return BuildSealedCoreHitFrame(32.0, 6.2, motion); // 충격 흡수
        }


        private string[] BuildSealedCoreHitFrame(double radiusX, double radiusY, int motion)
        {
            const int width = 96; // 보스 아트 고정 폭
            const int height = 15; // 보스 아트 고정 높이
            const double centerX = (width - 1) / 2.0; // 좌우 대칭 중심축
            const double centerY = (height - 1) / 2.0; // 상하 대칭 중심축

            string[] art = new string[height]; // 생성 결과

            for (int y = 0; y < height; y++) // 세로 줄 생성
            {
                char[] chars = new string(' ', width).ToCharArray(); // 공백 바탕
                double dy = y - centerY; // 중심 기준 Y 거리

                for (int x = 0; x < width; x++) // 가로 문자 생성
                {
                    double dx = x - centerX; // 중심 기준 X 거리
                    double normalized = (dx * dx) / (radiusX * radiusX) + (dy * dy) / (radiusY * radiusY); // 타원 내부 체크

                    if (normalized > 1.0) continue; // 타원 밖은 공백 유지

                    chars[x] = GetSealedCoreHitChar(normalized, dx, dy, motion); // 피격 상태 문자 적용
                }

                art[y] = new string(chars); // 줄 저장
            }

            ApplySealedCoreHitText(art, motion); // 피격 상태 문구 적용
            ApplySealedCoreImpactMarks(art, motion); // 외곽 충격 표시 적용

            return art; // 피격 아트
        }


        private char GetSealedCoreHitChar(double normalized, double dx, double dy, int motion)
        {
            int shock = ((int)(Math.Abs(dx) * 1.7 + Math.Abs(dy) * 4.0) + motion) % 4; // 좌우대칭 충격 패턴

            if (normalized >= 0.88) // 가장 바깥 껍질 체크
            {
                if (motion == 1) return shock < 2 ? 'X' : '!'; // 충격 최대치
                return '@'; // 껍질 유지
            }

            if (normalized >= 0.72) // 장갑층 체크
            {
                if (motion == 1) return shock == 0 ? '!' : '#'; // 장갑층 진동
                return '#'; // 장갑층 유지
            }

            if (motion == 1) // 충격 최대치
            {
                if (normalized >= 0.50) return '%'; // 내부 압력층
                if (normalized >= 0.25) return '$'; // 오염 충격
                return '!'; // 중심 충격
            }

            if (motion == 2) // 충격 흡수
            {
                if (normalized >= 0.50) return '%'; // 내부 밀도층
                if (normalized >= 0.25) return '*'; // 흡수층
                return '$'; // 잔류 노이즈
            }

            if (normalized >= 0.50) return '%'; // 기본 내부 밀도층
            if (normalized >= 0.25) return '*'; // 기본 중심 압축층
            return '#'; // 기본 코어
        }


        private void ApplySealedCoreHitText(string[] art, int motion)
        {
            if (motion == 1) // 충격 최대치
            {
                OverlayCenteredCoreText(art, 6, " [ IMP$CT_LOCK ] "); // 피격 1줄
                OverlayCenteredCoreText(art, 7, "  SHELL_HOLD!NG  "); // 피격 2줄
                OverlayCenteredCoreText(art, 8, "  DAMAGE_D$NIED  "); // 피격 3줄
                return;
            }

            if (motion == 2) // 충격 흡수
            {
                OverlayCenteredCoreText(art, 6, " [ KERNEL_LOCK ] "); // 회복 1줄
                OverlayCenteredCoreText(art, 7, "  SHELL_STABLE   "); // 회복 2줄
                OverlayCenteredCoreText(art, 8, "  ACCESS_DENIED  "); // 회복 3줄
                return;
            }

            OverlayCenteredCoreText(art, 6, " [ IMPACT_LOCK ] "); // 피격 1줄
            OverlayCenteredCoreText(art, 7, "  SHELL_HOLDING  "); // 피격 2줄
            OverlayCenteredCoreText(art, 8, "  DAMAGE_DENIED  "); // 피격 3줄
        }


        private void ApplySealedCoreImpactMarks(string[] art, int motion)
        {
            if (motion == 1) // 충격 최대치
            {
                OverlayBossTextAt(art, 4, 7, "!!X"); // 좌상 충격
                OverlayBossTextAt(art, 4, 86, "X!!"); // 우상 충격
                OverlayBossTextAt(art, 7, 3, "<<<!!"); // 좌측 충격파
                OverlayBossTextAt(art, 7, 88, "!!>>>"); // 우측 충격파
                OverlayBossTextAt(art, 10, 7, "!!X"); // 좌하 충격
                OverlayBossTextAt(art, 10, 86, "X!!"); // 우하 충격
                return;
            }

            OverlayBossTextAt(art, 7, 5, "<<!"); // 좌측 잔류 충격
            OverlayBossTextAt(art, 7, 88, "!>>"); // 우측 잔류 충격
        }


        private void OverlayBossTextAt(string[] art, int row, int col, string text)
        {
            if (art == null) return; // null 방지
            if (row < 0 || row >= art.Length) return; // 범위 체크
            if (string.IsNullOrEmpty(text)) return; // 빈 문구 방지

            if (col < 0) col = 0; // 시작 위치 보정

            char[] chars = art[row].ToCharArray(); // 대상 줄 문자

            for (int i = 0; i < text.Length && col + i < chars.Length; i++) // 문구 삽입
            {
                chars[col + i] = text[i]; // 문자 교체
            }

            art[row] = new string(chars); // 줄 갱신
        }


        private string[] BuildDeadBossArt(int frame)
        {
            int deathFrame = renderBossDeathFrame >= 0 ? renderBossDeathFrame : frame % 5; // 사망 연출 프레임 선택

            if (deathFrame == 0) return BuildBossPhase3DeathFrame0(); // 코어 균열
            if (deathFrame == 1) return BuildBossPhase3DeathFrame1(); // 눈깔 붕괴
            if (deathFrame == 2) return BuildBossPhase3DeathFrame2(); // 시스템 정지
            if (deathFrame == 3) return BuildBossPhase3DeathFrame3(); // 신호 소실

            return BuildBossPhase3DeathFrame4(); // 완전 소멸
        }


        private string[] BuildBossPhase3DeathFrame0()
        {
            return new string[]
            {
                "               . . . :: ++ %% ## @@!! ## %% ++ :: . . .             ",
                "       [-][#] . ::: ++ %% ## @@▓▓XXX▓▓@@ ## %% ++ ::: . [#][-]      ",
                "     [#] . :: ++ % ## @@▓▒░░░░░@@X?X@@░░░░░▒▓@@ ## % ++ :: . [#]    ",
                "   [#] : ++ %% # @@▓▒░    <<< ROOT_?YE >>>     ░▒▓@@ # %% ++ : [#]  ",
                " [#] : ++ % # @@▓▒░     .---==  11X11  ==---.     ░▒▓@@ # % ++ : [#]",
                "|=| :: + % # @@▒░    [!] PRIVILEGE_DROP_FAIL [!]   ░▒@@ # % + :: |=|",
                "|=| : + % # @▒░         >>> CORE_DAMAGED <<<        ░▒@ # % + : |=|",
                "══════════▓▒░░        <<<    @ SYST?M @     >>>      ░░▒▓═══════════",
                "|=| : + % # @▒░           >>> KERNEL_M?LT <<<        ░▒@ # % + : |=|",
                "|=| :: + % # @@▒░    [!] SIGNAL_COLLAPSE_ERR [!]   ░▒@@ # % + :: |=|",
                " [#] : ++ % # @@▓▒░     '---==  00X00  ==---'    ░▒▓@@ # % ++ : [#] ",
                "   [#] : ++ %% # @@▓▒░    <<< DATA_LOSS >>>    ░▒▓@@ # %% ++ : [#]   ",
                "     [#] . :: ++ % ## @@▓▒░░░░░@@X?X@@░░░░░▒▓@@ ## % ++ :: . [#]    ",
                "       [-][#] . ::: ++ %% ## @@▓▓XXX▓▓@@ ## %% ++ ::: . [#][-]      ",
                "               . . . :: ++ %% ## @@!! ## %% ++ :: . . .             "
            };
        }


        private string[] BuildBossPhase3DeathFrame1()
        {
            return new string[]
            {
                "            . . . :: ++ %% ## @X!!X@ ## %% ++ :: . . .            ",
                "      [-][#] . ::: ++ %% ## @@▓X!!X▓@@ ## %% ++ ::: . [#][-]       ",
                "      [#] . :: ++ % ## @@▓▒░░X?X@@@@@X?X░░▒▓@@ ## % ++ :: . [#]     ",
                "   [#] : ++ %% # @@▓▒░    <<< R?OT_EYE >>>     ░▒▓@@ # %% ++ : [#] ",
                " [#] : ++ % # @@▓▒░     .---==  X1X1X  ==---.     ░▒▓@@ # % ++ : [#]",
                "|=| :: + % # @@▒░    [X] PRIVILEGE_DROP_NULL [X]   ░▒@@ # % + :: |=|",
                "|=| : + % # @▒░         >>> CORE_OVERLOAD <<<        ░▒@ # % + : |=|",
                "══════XX══▓▒░░        <<<    @ ERR?R @     >>>      ░░▒▓══XX═══════",
                "|=| : + % # @▒░           >>> KERNEL_BREAK <<<       ░▒@ # % + : |=|",
                "|=| :: + % # @@▒░    [X] SIGNAL_COLLAPSE_END [X]   ░▒@@ # % + :: |=|",
                " [#] : ++ % # @@▓▒░     '---==  X0X0X  ==---'    ░▒▓@@ # % ++ : [#] ",
                "   [#] : ++ %% # @@▓▒░    <<< DATA_VOID >>>    ░▒▓@@ # %% ++ : [#]  ",
                "      [#] . :: ++ % ## @@▓▒░░X?X@@@@@X?X░░▒▓@@ ## % ++ :: . [#]     ",
                "      [-][#] . ::: ++ %% ## @@▓X!!X▓@@ ## %% ++ ::: . [#][-]       ",
                "            . . . :: ++ %% ## @X!!X@ ## %% ++ :: . . .            "
            };
        }


        private string[] BuildBossPhase3DeathFrame2()
        {
            return new string[]
            {
                " .  %   #      *      .  :  +  %  #  %  +  :  .      *      #   %  . ",
                "   *   #   %      +      :  +  %  %  +  :      +      %   #   *   ",
                " %   *   #      +      : :  +  %  %  +  : :      +      #   *   % ",
                "   #   *      +      : : :  +  %  %  +  : : :      +      *   #   ",
                " *   #      +      : : : :  +  %  %  +  : : : :      +      #   * ",
                "   %      +      : : : : :             : : : : :      +      %   ",
                " #      +      : : : :    [!] CORE BREACH [!]    : : : :      +      # ",
                "    ---=--==---===---==== [ SYSTEM_HALT_0x00 ] ====---===---==--=---",
                " #      +      : : : :    [!] KERNEL NULL [!]    : : : :      +      # ",
                "   %      +      : : : : :             : : : : :      +      %   ",
                " *   #      +      : : : :  +  %  %  +  : : : :      +      #   * ",
                "   #   *      +      : : :  +  %  %  +  : : :      +      *   #   ",
                " %   *   #      +      : :  +  %  %  +  : :      +      #   *   % ",
                "   *   #   %      +      :  +  %  %  +  :      +      %   #   *   ",
                " .  %   #      *      .  :  +  %  #  %  +  :  .      *      #   %  . "
            };
        }


        private string[] BuildBossPhase3DeathFrame3()
        {
            return new string[]
            {
                " .      #      *     .  :  +     #     +  :  .      *      #      . ",
                "       #   %              :     %  %     :             %   #       ",
                " %       #              : :     %  %     : :             #       % ",
                "   #          +      : : :                 : : :      +          #   ",
                " *          +      : : : :       . .       : : : :      +          * ",
                "          +      : : : : :     .     .     : : : : :      +          ",
                " #      +      : : : :       CORE NULL       : : : :      +      # ",
                "    ---=--==---===---==== [ SIGNAL_LOST ] ====---===---==--=---",
                " #      +      : : : :       DATA VOID       : : : :      +      # ",
                "          +      : : : : :     .     .     : : : : :      +          ",
                " *          +      : : : :       . .       : : : :      +          * ",
                "   #          +      : : :                 : : :      +          #   ",
                " %       #               : :     %  %     : :             #       % ",
                "       #   %               :     %  %     :             %   #       ",
                " .       #      *      .  :  +     #     +  :  .      *      #      . "
            };
        }


        private string[] BuildBossPhase3DeathFrame4()
        {
            return new string[]
            {
                " .             .           .        .           .             . ",
                "        .             :                  :             .        ",
                "   .          +              .  .              +          .   ",
                "              : :                            : :              ",
                "      .      : : :        .        .        : : :      .      ",
                "            : : : :                       : : : :            ",
                "       +    : :        KERNEL CORE LOST        : :    +       ",
                "     ----=---==----==== [ 0x00000000 ] ====----==---=----",
                "       +    : :         ROOT ACCESS END        : :    +       ",
                "            : : : :                       : : : :            ",
                "      .      : : :        .        .        : : :      .      ",
                "              : :                            : :              ",
                "   .          +              .  .              +          .   ",
                "        .             :                  :             .        ",
                " .             .           .        .           .             . "
            };
        }


        
        // BOSS PHASE 2 EYE ART
        

        private string[] BuildBossPhase2Art(int frame)
        {
            int motion = frame % 44; // Phase2 눈깜빡임 주기 단축

            if (motion < 12) // 뜬 눈 유지
            {
                return BuildBossPhase2OpenEyeShake(frame); // 동공지진
            }

            // 1차 빠른 깜빡임
            if (motion == 12) return BuildBossPhase2EyeFrame1(); // 감김 1
            if (motion == 13) return BuildBossPhase2EyeFrame2(); // 감김 2
            if (motion == 14) return BuildBossPhase2EyeFrame3(); // 감김 3
            if (motion == 15) return BuildBossPhase2EyeFrame4(); // 감김 4
            if (motion == 16) return BuildBossPhase2EyeFrame5(); // 완전 감김
            if (motion == 17) return BuildBossPhase2EyeFrame4(); // 열림 1
            if (motion == 18) return BuildBossPhase2EyeFrame3(); // 열림 2
            if (motion == 19) return BuildBossPhase2EyeFrame2(); // 열림 3
            if (motion == 20) return BuildBossPhase2EyeFrame1(); // 열림 4

            if (motion < 24) // 짧은 뜬눈
            {
                return BuildBossPhase2OpenEyeShake(frame); // 동공지진
            }

            // 2차 빠른 깜빡임
            if (motion == 24) return BuildBossPhase2EyeFrame1(); // 감김 1
            if (motion == 25) return BuildBossPhase2EyeFrame2(); // 감김 2
            if (motion == 26) return BuildBossPhase2EyeFrame3(); // 감김 3
            if (motion == 27) return BuildBossPhase2EyeFrame4(); // 감김 4
            if (motion == 28) return BuildBossPhase2EyeFrame5(); // 완전 감김
            if (motion == 29) return BuildBossPhase2EyeFrame4(); // 열림 1
            if (motion == 30) return BuildBossPhase2EyeFrame3(); // 열림 2
            if (motion == 31) return BuildBossPhase2EyeFrame2(); // 열림 3
            if (motion == 32) return BuildBossPhase2EyeFrame1(); // 열림 4

            return BuildBossPhase2OpenEyeShake(frame); // 긴 뜬눈 유지
        }


        private string[] BuildBossPhase2OpenEyeShake(int frame)
        {
            int shake = frame % 8; // 동공지진 패턴

            if (shake == 1 || shake == 5) // 왼쪽 흔들림 체크
            {
                return BuildBossPhase2OpenEyeLeftPupil(); // 동공 왼쪽 이동
            }

            if (shake == 3 || shake == 7) // 오른쪽 흔들림 체크
            {
                return BuildBossPhase2OpenEyeRightPupil(); // 동공 오른쪽 이동
            }

            return BuildBossPhase2EyeFrame0(); // 기본 뜬눈
        }


        private string[] BuildBossPhase2EyeFrame0()
        {
            return new string[] // 뜬 눈 원본
            {
                "            .... ++++ %%%% #### @@@@ #### %%%% ++++ ....            ",
                "        ... +++ %%% ### @@@@@@@@@@@@@@@@@@@@@@ ### %%% +++ ...       ",
                "      ... ++ %% ## @@@@@@@@**************@@@@@@@@ ## %% ++ ...       ",
                "      . ++ %% ## @@@@@@*** [ SYSTEM_PANIC ] ***@@@@@@ ## %% ++ .     ",
                "    . + %% ## @@@@@**      === WARNING ===      **@@@@@ ## %% + .    ",
                "    . + % ## @@@@**     [!] CORE_OVERDRIVE [!]    **@@@@ ## % + .    ",
                " . . + % # @@@**          >>> 0x00000000 <<<         **@@@ # % + . . ",
                " . + + % # @@**            SINGULARITY_CORE           **@@ # % + + . ",
                " . . + % # @@@**          >>> 0xFFFFFFFF <<<         **@@@ # % + . . ",
                "    . + % ## @@@@**     [!] PRIVILEGE_DROP [!]    **@@@@ ## % + .    ",
                "    . + %% ## @@@@@**      ===============      **@@@@@ ## %% + .    ",
                "      . ++ %% ## @@@@@@*** [ KERNEL_MELT ] ***@@@@@@ ## %% ++ .      ",
                "      ... ++ %% ## @@@@@@@@**************@@@@@@@@ ## %% ++ ...       ",
                "        ... +++ %%% ### @@@@@@@@@@@@@@@@@@@@@@ ### %%% +++ ...       ",
                "            .... ++++ %%%% #### @@@@ #### %%%% ++++ ....            ",
            };
        }

        private string[] BuildBossPhase2OpenEyeLeftPupil()
        {
            return new string[] // 동공 왼쪽 흔들림
            {
                "            .... ++++ %%%% #### @@@@ #### %%%% ++++ ....            ",
                "        ... +++ %%% ### @@@@@@@@@@@@@@@@@@@@@@ ### %%% +++ ...       ",
                "      ... ++ %% ## @@@@@@@@**************@@@@@@@@ ## %% ++ ...       ",
                "      . ++ %% ## @@@@@@***[ SYSTEM_PANIC ]  ***@@@@@@ ## %% ++ .     ",
                "    . + %% ## @@@@@**     === WARNING ===       **@@@@@ ## %% + .    ",
                "    . + % ## @@@@**    [!] CORE_OVERDRIVE [!]     **@@@@ ## % + .    ",
                " . . + % # @@@**         >>> 0x00000000 <<<          **@@@ # % + . . ",
                " . + + % # @@**           SINGULARITY_CORE            **@@ # % + + . ",
                " . . + % # @@@**         >>> 0xFFFFFFFF <<<          **@@@ # % + . . ",
                "    . + % ## @@@@**    [!] PRIVILEGE_DROP [!]     **@@@@ ## % + .    ",
                "    . + %% ## @@@@@**     ===============       **@@@@@ ## %% + .    ",
                "      . ++ %% ## @@@@@@***[ KERNEL_MELT ]  ***@@@@@@ ## %% ++ .      ",
                "      ... ++ %% ## @@@@@@@@**************@@@@@@@@ ## %% ++ ...       ",
                "        ... +++ %%% ### @@@@@@@@@@@@@@@@@@@@@@ ### %%% +++ ...       ",
                "            .... ++++ %%%% #### @@@@ #### %%%% ++++ ....            ",
            };
        }

        private string[] BuildBossPhase2OpenEyeRightPupil()
        {
            return new string[] // 동공 오른쪽 흔들림
            {
                "            .... ++++ %%%% #### @@@@ #### %%%% ++++ ....            ",
                "        ... +++ %%% ### @@@@@@@@@@@@@@@@@@@@@@ ### %%% +++ ...       ",
                "      ... ++ %% ## @@@@@@@@**************@@@@@@@@ ## %% ++ ...       ",
                "      . ++ %% ## @@@@@@***  [ SYSTEM_PANIC ]***@@@@@@ ## %% ++ .     ",
                "    . + %% ## @@@@@**       === WARNING ===     **@@@@@ ## %% + .    ",
                "    . + % ## @@@@**      [!] CORE_OVERDRIVE [!]   **@@@@ ## % + .    ",
                " . . + % # @@@**           >>> 0x00000000 <<<        **@@@ # % + . . ",
                " . + + % # @@**             SINGULARITY_CORE          **@@ # % + + . ",
                " . . + % # @@@**           >>> 0xFFFFFFFF <<<        **@@@ # % + . . ",
                "    . + % ## @@@@**      [!] PRIVILEGE_DROP [!]   **@@@@ ## % + .    ",
                "    . + %% ## @@@@@**       ===============     **@@@@@ ## %% + .    ",
                "      . ++ %% ## @@@@@@***  [ KERNEL_MELT ]***@@@@@@ ## %% ++ .      ",
                "      ... ++ %% ## @@@@@@@@**************@@@@@@@@ ## %% ++ ...       ",
                "        ... +++ %%% ### @@@@@@@@@@@@@@@@@@@@@@ ### %%% +++ ...       ",
                "            .... ++++ %%%% #### @@@@ #### %%%% ++++ ....            ",
            };
        }

        private string[] BuildBossPhase2EyeFrame1()
        {
            return new string[] // 감김 프레임 1
            {
                "            .... ++++ %%%% #### @@@@ #### %%%% ++++ ....            ",
                "        ... +++ %%% ### @@@@@@@@@@@@@@@@@@@@@@ ### %%% +++ ...       ",
                "      ... ++ %% ## @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ ## %% ++ ...       ",
                "      . ++ %% ## @@@@@@@@@** SYSTEM_PANIC **@@@@@@@@@ ## %% ++ .     ",
                "    . + %% ## @@@@@@@**    === WARNING ===    **@@@@@@@ ## %% + .    ",
                "    . + % ## @@@@**     [!] CORE_OVERDRIVE [!]    **@@@@ ## % + .    ",
                " . . + % # @@@**          >>> 0x00000000 <<<         **@@@ # % + . . ",
                " . + + % # @@**            SINGULARITY_CORE           **@@ # % + + . ",
                " . . + % # @@@**          >>> 0xFFFFFFFF <<<         **@@@ # % + . . ",
                "    . + % ## @@@@**     [!] PRIVILEGE_DROP [!]    **@@@@ ## % + .    ",
                "    . + %% ## @@@@@@@**    ===============    **@@@@@@@ ## %% + .    ",
                "      . ++ %% ## @@@@@@@@@** KERNEL_MELT **@@@@@@@@@ ## %% ++ .      ",
                "      ... ++ %% ## @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ ## %% ++ ...       ",
                "        ... +++ %%% ### @@@@@@@@@@@@@@@@@@@@@@ ### %%% +++ ...       ",
                "            .... ++++ %%%% #### @@@@ #### %%%% ++++ ....            ",
            };
        }

        private string[] BuildBossPhase2EyeFrame2()
        {
            return new string[] // 감김 프레임 2
            {
                "            .... ++++ %%%% #### @@@@ #### %%%% ++++ ....            ",
                "        ... +++ %%% ### @@@@@####@@@@@@@@@@@@@ ### %%% +++ ...       ",
                "      ... ++ %% ## @@@@@@@@@@@@@@@@@@@@@@%%%%@@@@ ## %% ++ ...       ",
                "      . ++ %% ## @@@##@@@@*@@@@@@@@@@@@@@@@*@@@@@@@@@ ## %% ++ .     ",
                "    . + %% ## @@@@@@@@%%@@##@@@@ARNIN@@@@@@%%%%@@@@@@@@ ## %% + .    ",
                "    . + % ## @@@@@@@@@@**!] CORE_OVERDRIVE [**@@@@@@@@@@ ## % + .    ",
                " . . + % # @@@@@**        >>> 0x00000000 <<<       **@@@@@ # % + . . ",
                " . + + % # @@**            SINGULARITY_CORE           **@@ # % + + . ",
                " . . + % # @@@@@**        >>> 0xFFFFFFFF <<<       **@@@@@ # % + . . ",
                "    . + % ## @@@@@@@@@@**!] PRIVILEGE_DROP [**@@@@@@@@@@ ## % + .    ",
                "    . + %% ## @@@@@@@@@%%%%@@@@@=====@@@@@@@@@%%%%@@@@@ ## %% + .    ",
                "      . ++ %% ## @@@@@%%%%@@@@@@@@@@@@@@@@@####@@@@@ ## %% ++ .      ",
                "      ... ++ %% ## @@@@####@@@@@@@@@@@@@@@@@@@@@@ ## %% ++ ...       ",
                "        ... +++ %%% ### @@@@@@@@@@@@@@@@@@@@@@ ### %%% +++ ...       ",
                "            .... ++++ %%%% #### @@@@ #### %%%% ++++ ....            ",
            };
        }

        private string[] BuildBossPhase2EyeFrame3()
        {
            return new string[] // 감김 프레임 3
            {
                "            .... ++++ %%%% #### @@@@ #### %%%% ++++ ....            ",
                "        ... +++ %%% ### @@@@@####@@@@@@@@@@@@@ ### %%% +++ ...       ",
                "      ... ++ %% ## @@@@@@@@@@@@@@@@@@@@@@%%%%@@@@ ## %% ++ ...       ",
                "      . ++ %% ## @@@##@@@@*@@@@@@@@@@@@@@@@*@@@@@@@@@ ## %% ++ .     ",
                "    . + %% ## @@@@@@@@%%@@##@@@@@@@@@@@@@@@%%%%@@@@@@@@ ## %% + .    ",
                "    . + % ## @@@@@@@@@@@@@@**ORE_OVERDRI**@@@@@@@@@@@@@@ ## % + .    ",
                " . . + % # @@@@@@@@@@**   >>> 0x00000000 <<<  **@@@@@@@@@@ # % + . . ",
                " . + + % # @@******        SINGULARITY_CORE       ******@@ # % + + . ",
                " . . + % # @@@@@@@@@@**   >>> 0xFFFFFFFF <<<  **@@@@@@@@@@ # % + . . ",
                "    . + % ## @@@@@@@@@@@@@@**RIVILEGE_DR**@@@@@@@@@@@@@@ ## % + .    ",
                "    . + %% ## @@@@@@@@@%%%%@@@@@@@@@@@@@@@@@@@%%%%@@@@@ ## %% + .    ",
                "      . ++ %% ## @@@@@%%%%@@@@@@@@@@@@@@@@@####@@@@@ ## %% ++ .      ",
                "      ... ++ %% ## @@@@####@@@@@@@@@@@@@@@@@@@@@@ ## %% ++ ...       ",
                "        ... +++ %%% ### @@@@@@@@@@@@@@@@@@@@@@ ### %%% +++ ...       ",
                "            .... ++++ %%%% #### @@@@ #### %%%% ++++ ....            ",
            };
        }

        private string[] BuildBossPhase2EyeFrame4()
        {
            return new string[] // 감김 프레임 4
            {
                "            .... ++++ %%%% #### @@@@ #### %%%% ++++ ....            ",
                "        ... +++ %%% ### @@@@@####@@@@@@@@@@@@@ ### %%% +++ ...       ",
                "      ... ++ %% ## @@@@@@@@@%%@@@@@@@@@@%%%%@@@@ ## %% ++ ...       ",
                "      . ++ %% ## @@@##@@@@*@@@@@@@@@@@@@@@@*@@@@@@@@@ ## %% ++ .     ",
                "    . + %% ## @@@@@@@@%%@@##@@@@@%%@@@@@@@@%%%%@@@@@@@@ ## %% + .    ",
                "    . + % ## @@@@@@@@@@@@@@#@@@@@@@@@@@@@%%@@@@@@@@@@@@@ ## % + .    ",
                " . . + % # @@@@@@@@@@@@@@@@@@@@**000**@@@@@@@@@@@@@@@@@@@@ # % + . . ",
                " . + + % # @@**********    SINGULARITY_CORE   **********@@ # % + + . ",
                " . . + % # @@@@@@@@@@@@@%%@@@@@**FFF**@@@@@@@@@@@@@@@@@@@@ # % + . . ",
                "    . + % ## @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ ## % + .    ",
                "    . + %% ## @@@@@@@@@%%%%@@@@@@@@@@@%%@@@@@@%%%%@@@@@ ## %% + .    ",
                "      . ++ %% ## @@@@@%%%%@@@@@@@@@@@@@@@@@####@@@@@ ## %% ++ .      ",
                "      ... ++ %% ## @@@@####@@@@@@@@@@@@@@@@@@@@@@ ## %% ++ ...       ",
                "        ... +++ %%% ### @@@@@@@@@@@@@@@@@@@@@@ ### %%% +++ ...       ",
                "            .... ++++ %%%% #### @@@@ #### %%%% ++++ ....            ",
            };
        }

        private string[] BuildBossPhase2EyeFrame5()
        {
            return new string[] // 감김 프레임 5
            {
                "            .... ++++ %%%% #### @@@@ #### %%%% ++++ ....            ",
                "        ... +++ %%% ### @@@@@@@@@@@@@@@@@@@@@@ ### %%% +++ ...       ",
                "      ... ++ %% ## @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ ## %% ++ ...       ",
                "      . ++ %% ## @@@@@@@***@@@@@@@@@@@@@@@@***@@@@@@ ## %% ++ .     ",
                "    . + %% ## @@@@@*@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*@@@@@ ## %% + .    ",
                "    . + % ## @@@@*@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*@@@@ ## % + .    ",
                " . . + % # @@@**@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@**@@@ # % + . . ",
                " . + + % # @@*******************************************@@ # % + + . ",
                " . . + % # @@@**@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*@@@ # % + . . ",
                "    . + % ## @@@@*@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*@@@@ ## % + .    ",
                "    . + %% ## @@@@@*@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*@@@@@ ## %% + .    ",
                "      . ++ %% ## @@@@@@**@@@@@@@@@@@@@@@@@@@@*@@@@@@ ## %% ++ .      ",
                "      ... ++ %% ## @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ ## %% ++ ...       ",
                "        ... +++ %%% ### @@@@@@@@@@@@@@@@@@@@@@ ### %%% +++ ...       ",
                "            .... ++++ %%%% #### @@@@ #### %%%% ++++ ....            ",
            };
        }

        
        // BOSS PHASE 2 HIT / ATTACK ART
        

        private string[] BuildBossPhase2AttackArt(int frame)
        {
            int motion = frame % 4; // 2페이즈 공격모션 4단계

            if (motion == 0) return BuildBossPhase2AttackFrame0(); // 동공 조준
            if (motion == 1) return BuildBossPhase2AttackFrame1(); // 신호 압축
            if (motion == 2) return BuildBossPhase2AttackFrame2(); // 왼쪽 발사

            return BuildBossPhase2AttackFrame1(); // 발사 후 잔류
        }


        private string[] BuildBossPhase2AttackFrame0()
        {
            string[] art = BuildBossPhase2EyeFrame0(); // 원본 열린 눈 유지

            OverlayCenteredCoreText(art, 5, "     [!] TARGET_LOCK [!]    "); // 공격 조준
            OverlayCenteredCoreText(art, 6, "       >>> 0xBADC0DE <<<       "); // 공격 주소
            OverlayCenteredCoreText(art, 7, "        SINGULARITY_AIM        "); // 동공 조준
            OverlayCenteredCoreText(art, 8, "       >>> 0xFEEDFACE <<<      "); // 공격 주소
            OverlayBossTextAt(art, 7, 6, "<<<<"); // 약한 예열 신호

            return art; // 공격 프레임
        }


        private string[] BuildBossPhase2AttackFrame1()
        {
            string[] art = BuildBossPhase2EyeFrame0(); // 원본 열린 눈 유지

            OverlayCenteredCoreText(art, 5, "    [!] VECTOR_COMPRESS [!]   "); // 신호 압축
            OverlayCenteredCoreText(art, 6, "      $$$ 0xBADC0DE $$$       "); // 페이로드 충전
            OverlayCenteredCoreText(art, 7, "       SINGULARITY_CHARGE      "); // 중심 충전
            OverlayCenteredCoreText(art, 8, "      $$$ 0xFEEDFACE $$$      "); // 페이로드 충전
            OverlayBossTextAt(art, 6, 4, "<<<<===="); // 상단 예열
            OverlayBossTextAt(art, 7, 2, "<<<<<<===="); // 중앙 예열
            OverlayBossTextAt(art, 8, 4, "<<<<===="); // 하단 예열

            return art; // 공격 프레임
        }


        private string[] BuildBossPhase2AttackFrame2()
        {
            string[] art = BuildBossPhase2EyeFrame0(); // 원본 열린 눈 유지

            OverlayCenteredCoreText(art, 5, "      [!] VECTOR_RELEASE [!]   "); // 신호 발사
            OverlayCenteredCoreText(art, 6, "       <<< SIGNAL_BURST <<<    "); // 공격 방향
            OverlayCenteredCoreText(art, 7, "        SINGULARITY_FIRE       "); // 동공 발사
            OverlayCenteredCoreText(art, 8, "       <<< SIGNAL_BURST <<<    "); // 공격 방향
            OverlayBossTextAt(art, 5, 0, "<<<<====####"); // 상단 발사
            OverlayBossTextAt(art, 6, 0, "<<<<<<====####@@@@"); // 상중단 발사
            OverlayBossTextAt(art, 7, 0, "<<<<<<<<====####@@@@@@"); // 중앙 발사
            OverlayBossTextAt(art, 8, 0, "<<<<<<====####@@@@"); // 하중단 발사
            OverlayBossTextAt(art, 9, 0, "<<<<====####"); // 하단 발사

            return art; // 공격 프레임
        }


        private string[] BuildBossPhase2HitArt(int frame)
        {
            int motion = frame % 4; // 2페이즈 피격모션 4단계

            if (motion == 0) return BuildBossPhase2HitFrame0(); // 충격 진입
            if (motion == 1) return BuildBossPhase2HitFrame1(); // 충격 최대
            if (motion == 2) return BuildBossPhase2HitFrame2(); // 동공 흔들림

            return BuildBossPhase2HitFrame3(); // 복구
        }


        private string[] BuildBossPhase2HitFrame0()
        {
            string[] art = BuildBossPhase2EyeFrame0(); // 원본 열린 눈 유지

            OverlayCenteredCoreText(art, 5, "      [!] IMPACT_ALERT [!]    "); // 충격 경고
            OverlayCenteredCoreText(art, 6, "       >>> 0x00000000 <<<      "); // 동공 유지
            OverlayCenteredCoreText(art, 7, "        SINGULARITY_CORE       "); // 동공 유지
            OverlayCenteredCoreText(art, 8, "       >>> 0xFFFFFFFF <<<      "); // 동공 유지
            OverlayBossTextAt(art, 6, 5, "!!"); // 좌측 충격
            OverlayBossTextAt(art, 6, 64, "!!"); // 우측 충격
            OverlayBossTextAt(art, 8, 5, "!!"); // 좌측 충격
            OverlayBossTextAt(art, 8, 64, "!!"); // 우측 충격

            return art; // 피격 프레임
        }


        private string[] BuildBossPhase2HitFrame1()
        {
            string[] art = BuildBossPhase2EyeFrame0(); // 원본 열린 눈 유지

            OverlayCenteredCoreText(art, 4, "       XX  WARNING  XX        "); // 상단 충격
            OverlayCenteredCoreText(art, 5, "    [!] C0RE_0VERDR!VE [!]    "); // 피격 오염
            OverlayCenteredCoreText(art, 6, "       >>> 0x00XX0000 <<<     "); // 주소 깨짐
            OverlayCenteredCoreText(art, 7, "       S!NGULARITY_C0RE       "); // 동공 충격
            OverlayCenteredCoreText(art, 8, "       >>> 0xFFFFXXFF <<<     "); // 주소 깨짐
            OverlayCenteredCoreText(art, 9, "    [!] PR!VILEGE_DR0P [!]    "); // 피격 오염
            OverlayCenteredCoreText(art, 10, "       XX  KERNEL_MELT  XX    "); // 하단 충격
            OverlayBossTextAt(art, 5, 0, "<<!!"); // 좌측 충격파
            OverlayBossTextAt(art, 5, 67, "!!>>"); // 우측 충격파
            OverlayBossTextAt(art, 7, 0, "<<<!!"); // 좌측 충격파
            OverlayBossTextAt(art, 7, 66, "!!>>>"); // 우측 충격파
            OverlayBossTextAt(art, 9, 0, "<<!!"); // 좌측 충격파
            OverlayBossTextAt(art, 9, 67, "!!>>"); // 우측 충격파

            return art; // 피격 프레임
        }


        private string[] BuildBossPhase2HitFrame2()
        {
            string[] art = BuildBossPhase2OpenEyeLeftPupil(); // 동공 좌측 튐

            OverlayCenteredCoreText(art, 5, "     [!] CORE_STABILIZE [!]   "); // 복구 시작
            OverlayCenteredCoreText(art, 6, "       >>> 0x00000000 <<<      "); // 동공 복구
            OverlayCenteredCoreText(art, 7, "       SINGULARITY_CORE        "); // 동공 복구
            OverlayCenteredCoreText(art, 8, "       >>> 0xFFFFFFFF <<<      "); // 동공 복구
            OverlayBossTextAt(art, 7, 4, "<!"); // 잔류 충격
            OverlayBossTextAt(art, 7, 68, "!>"); // 잔류 충격

            return art; // 피격 프레임
        }


        private string[] BuildBossPhase2HitFrame3()
        {
            string[] art = BuildBossPhase2EyeFrame0(); // 원본 열린 눈 복귀

            OverlayCenteredCoreText(art, 5, "    [!] CORE_OVERDRIVE [!]    "); // 원본 상태 복구
            OverlayCenteredCoreText(art, 6, "       >>> 0x00000000 <<<      "); // 원본 상태 복구
            OverlayCenteredCoreText(art, 7, "        SINGULARITY_CORE       "); // 원본 상태 복구
            OverlayCenteredCoreText(art, 8, "       >>> 0xFFFFFFFF <<<      "); // 원본 상태 복구
            OverlayCenteredCoreText(art, 9, "     [!] PRIVILEGE_DROP [!]   "); // 원본 상태 복구

            return art; // 피격 프레임
        }


    }
}
