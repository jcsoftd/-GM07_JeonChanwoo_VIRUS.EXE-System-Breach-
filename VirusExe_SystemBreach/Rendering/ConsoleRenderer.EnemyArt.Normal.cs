namespace VirusExe.SystemBreach.Rendering
{
    public partial class ConsoleRenderer
    {
        // 일반 몬스터 ASCII 아트
        // 대기/공격/피격/사망 모션을 몬스터별로 관리
        private string[] BuildNormalEnemyArt(string enemyName, int frame)
        {
            switch (enemyName) // 일반 몬스터 이름별 상시 아트
            {
                case "SCAN_DAEMON": return BuildScanDaemonArt(frame);
                case "MEM_LEAK_ANOMALY": return BuildMemLeakAnomalyArt(frame);
                case "LOGIC_BOMB": return BuildLogicBombArt(frame);
                case "NULL_POINTER_VOID": return BuildNullPointerVoidArt(frame);
                case "PROTOCOL_MUNCHER": return BuildProtocolMuncherArt(frame);
                case "SANDBOX_ISOLATION": return BuildSandboxIsolationArt(frame);
                case "CIPHER_BLOCK_CHAIN": return BuildCipherBlockChainArt(frame);
                default: return BuildSecurityEnemyArt(frame); // 기본 일반몹 아트
            }
        }


        private string[] BuildHitNormalEnemyArt(string enemyName, int frame)
        {
            switch (enemyName) // 일반 몬스터 이름별 피격 아트
            {
                case "SCAN_DAEMON": return BuildHitScanDaemonArt(frame);
                case "MEM_LEAK_ANOMALY": return BuildHitMemLeakAnomalyArt(frame);
                case "LOGIC_BOMB": return BuildHitLogicBombArt(frame);
                case "NULL_POINTER_VOID": return BuildHitNullPointerVoidArt(frame);
                case "PROTOCOL_MUNCHER": return BuildHitProtocolMuncherArt(frame);
                case "SANDBOX_ISOLATION": return BuildHitSandboxIsolationArt(frame);
                case "CIPHER_BLOCK_CHAIN": return BuildHitCipherBlockChainArt(frame);
                default: return BuildHitSecurityEnemyArt(frame); // 기본 일반몹 피격 아트
            }
        }


        private string[] BuildAttackNormalEnemyArt(string enemyName, int frame)
        {
            switch (enemyName) // 일반 몬스터 이름별 공격 아트
            {
                case "SCAN_DAEMON": return BuildAttackScanDaemonArt(frame);
                case "MEM_LEAK_ANOMALY": return BuildAttackMemLeakAnomalyArt(frame);
                case "LOGIC_BOMB": return BuildAttackLogicBombArt(frame);
                case "NULL_POINTER_VOID": return BuildAttackNullPointerVoidArt(frame);
                case "PROTOCOL_MUNCHER": return BuildAttackProtocolMuncherArt(frame);
                case "SANDBOX_ISOLATION": return BuildAttackSandboxIsolationArt(frame);
                case "CIPHER_BLOCK_CHAIN": return BuildAttackCipherBlockChainArt(frame);
                default: return BuildAttackSecurityEnemyArt(frame); // 기본 일반몹 공격 아트
            }
        }


        private string[] BuildDeadNormalEnemyArt(string enemyName, int frame)
        {
            switch (enemyName) // 일반 몬스터 이름별 사망 아트
            {
                case "SCAN_DAEMON": return BuildDeadScanDaemonArt(frame);
                case "MEM_LEAK_ANOMALY": return BuildDeadMemLeakAnomalyArt(frame);
                case "LOGIC_BOMB": return BuildDeadLogicBombArt(frame);
                case "NULL_POINTER_VOID": return BuildDeadNullPointerVoidArt(frame);
                case "PROTOCOL_MUNCHER": return BuildDeadProtocolMuncherArt(frame);
                case "SANDBOX_ISOLATION": return BuildDeadSandboxIsolationArt(frame);
                case "CIPHER_BLOCK_CHAIN": return BuildDeadCipherBlockChainArt(frame);
                default: return BuildDeadSecurityEnemyArt(frame); // 기본 일반몹 사망 아트
            }
        }


        
        // SECURITY_PROC FALLBACK
        


        
        private string[] BuildSecurityEnemyArt(int frame)
        {
            int phase = (frame / 2) % 4; // 모션 단계
            string shift = phase == 1 ? " " : phase == 3 ? "  " : string.Empty; // 실루엣 좌우 흔들림
            string top = phase < 2 ? ".---------------." : "#===============#"; // 외곽선 굵기 변화
            string innerTop = phase < 2 ? "'---------------'" : "#===============#"; // 내부 외곽선 변화
            string eye = phase % 2 == 0 ? "0      0" : "O      o"; // 눈 점멸
            string core = phase == 0 ? "<CORE>" : phase == 1 ? "[CORE]" : phase == 2 ? "{CORE}" : "<PING>"; // 코어 맥동
            string node = phase < 2 ? "TRACE NODE" : "TRACE PING"; // 노드 상태 변화
            string signal = phase < 2 ? ">>>" : "<<<"; // 신호 변화
            string antenna = phase % 2 == 0 ? "/|\\     /|\\" : "\\|/     \\|/"; // 안테나 진동

            return new string[]
            {
                shift + "        " + top + "        ",
                shift + "     .--| SECURITY PROC |--.     ",
                shift + "     |  " + innerTop + "  |     ",
                shift + "     |      " + eye + "       |     ",
                shift + "     |        " + core + "       |     ",
                shift + "     |    .-----------.    |     ",
                shift + "     '--->| " + node + " |<--'     ",
                shift + "          '-----+-----'          ",
                shift + "          " + signal + "   |   " + signal + "          ",
                shift + "          " + antenna + "          "
            };
        }


        private string[] BuildHitSecurityEnemyArt(int frame)
        {
            string shift = frame % 2 == 0 ? "<<< " : "   >>> "; // 강한 피격 흔들림
            string noise = frame % 2 == 0 ? "!!" : "##"; // 피격 노이즈
            string eye = frame % 2 == 0 ? "X      X" : "x   !! X"; // 눈 손상
            string core = frame % 2 == 0 ? "<CRACK>" : "[ERROR]"; // 코어 오류
            string burst = frame % 2 == 0 ? "*** MEMORY BREACH ***" : "<<< DATA CORRUPTION >>>"; // 파티클
            string shell = frame % 2 == 0 ? "#===============#" : "X====!!====X"; // 외곽 파손

            return new string[]
            {
                shift + "      " + burst,
                shift + "      .--" + noise + "--" + shell + "--" + noise + ".",
                shift + " !!!--| SECUR!TY PROC |--!!!  ",
                shift + "   |  '--" + noise + "--X--" + noise + "--'  |     ",
                shift + " ##|      " + eye + "       |##   ",
                shift + "   |       " + core + "      |     ",
                shift + "   |    .--XX###XX--.  |     ",
                shift + "   '--X>| TRACE BREAK|<X--'    ",
                shift + "     !!! '--X---X--' !!!       ",
                shift + "            --X--              "
            };
        }


        private string[] BuildAttackSecurityEnemyArt(int frame)
        {
            string eye = frame % 2 == 0 ? "O      O" : "0      0"; // 공격 눈
            string core = frame % 3 == 0 ? "<LOCK>" : "[SCAN]"; // 공격 코어
            string pulse = frame % 2 == 0 ? "<<<<==== " : "<<<<---- "; // 왼쪽 공격 신호
            string signal = frame % 2 == 0 ? "TRACE_PING" : "PORT_SCAN "; // 공격 상태
            string gap = "         "; // 비공격 줄 정렬

            return new string[]
            {
                gap   + "      .---------------.        ",
                gap   + "   .--| TARGET LOCKED |--.     ",
                gap   + "   |  '---------------'  |     ",
                pulse + "   |      " + eye + "       |     ",
                pulse + "   |        " + core + "       |     ",
                pulse + "   |    .-----------.    |     ",
                pulse + "   '--->| " + signal + "|<--'     ",
                gap   + "        '-----+-----'          ",
                gap   + "          <<< | <<<            ",
                gap   + "          SECURITY PROC        "
            };
        }


        private string[] BuildDeadSecurityEnemyArt(int frame)
        {
            string noise = frame % 2 == 0 ? "..." : "x.x"; // 잔류 신호
            string core = frame % 2 == 0 ? "[CORE]" : "[NULL]"; // 정지 코어
            string signal = frame % 2 == 0 ? "TRACE LOST" : "PROC HALT "; // 사망 상태

            return new string[]
            {
                "        .---------------.        ",
                "     .--| SECURITY PROC |--.     ",
                "     |  '---------------'  |     ",
                "     |      x      x       |     ",
                "     |        " + core + "       |     ",
                "     |    .-----------.    |     ",
                "     '--->| " + signal + "|<--'     ",
                "          '-----x-----'          ",
                "              " + noise + "              ",
                "              --x--              "
            };
        }


        
        // SCAN_DAEMON
        

        private string[] BuildScanDaemonArt(int frame)
        {
            int phase = (frame / 2) % 4; // 모션 단계
            string shift = phase == 1 ? " " : phase == 3 ? "  " : string.Empty; // 실루엣 흔들림
            string top = phase < 2 ? ".---------." : "#=========#"; // 외곽선 굵기 변화
            string scan = phase % 2 == 0 ? "[  0101  ]" : "<[ 1010 ]>"; // 스캔값 점멸
            string eye = phase == 0 ? "EYE_PROC" : phase == 1 ? "EYE_SYNC" : phase == 2 ? "EYE_LOCK" : "EYE_PING"; // 눈 코어 변화
            string boxTop = phase < 2 ? ".------------." : "#============#"; // 내부 박스 변화
            string boxBot = phase < 2 ? "'------------'" : "#============#"; // 내부 박스 변화
            string trace = phase < 2 ? "TRACE" : "SCAN "; // 하단 상태 변화
            string leg = phase % 2 == 0 ? "/_/\\_\\" : "\\_\\/\\_/"; // 지지부 흔들림

            return new string[]
            {
                shift + "      " + top,
                shift + "  ___/   SCAN   \\___",
                shift + " /    " + scan + "    \\",
                shift + "|   " + boxTop + "   |",
                shift + "|   |  " + eye + "  |   |",
                shift + "|   " + boxBot + "   |",
                shift + " \\___   " + trace + "   ___/",
                shift + "     '--.____.--'",
                shift + "        " + leg,
                shift + "      SCAN_DAEMON"
            };
        }


        private string[] BuildHitScanDaemonArt(int frame)
        {
            string shift = frame % 2 == 0 ? "<<< " : "   >>> "; // 강한 피격 흔들림
            string scan = frame % 2 == 0 ? "[  XX0X  ]" : "<[ X1X ]>"; // 스캔값 손상
            string eye = frame % 2 == 0 ? "EYE_ERR!" : "EYE_BRK!"; // 눈 코어 오류
            string noise = frame % 2 == 0 ? "!!" : "##"; // 피격 노이즈
            string crack = frame % 2 == 0 ? "#====XX====#" : "X====##====X"; // 외곽 파손

            return new string[]
            {
                shift + "    ." + noise + "--SCAN--" + noise + ".",
                shift + "!!!_/   SCAN_X \\_!!!",
                shift + " /    " + scan + "    \\",
                shift + "|   " + crack + "   |",
                shift + "|   || " + eye + " ||   |",
                shift + "|   X====" + noise + "====X   |",
                shift + " \\_XX  TRACE_X  XX_/",
                shift + "   '--X____X--'",
                shift + "      /_X\\_X",
                shift + "    SCAN_DAEMON"
            };
        }


        private string[] BuildAttackScanDaemonArt(int frame)
        {
            string scan = frame % 2 == 0 ? "[>>PING>>]" : "[>>LOCK>>]"; // 공격 스캔 신호
            string eye = frame % 3 == 0 ? "EYE_LOCK" : "EYE_PROC"; // 타겟 고정
            string trace = frame % 2 == 0 ? "PORT_" : "TRACE"; // 공격 상태 변화
            string beam = frame % 2 == 0 ? "<<<<==== " : "<<<<---- "; // 왼쪽 공격 빔
            string gap = "         "; // 빔 없는 줄 정렬

            return new string[]
            {
                gap  + "   .---------.",
                gap  + "___/  TARGET  \\___",
                beam + "/    " + scan + "    \\",
                beam + "|   .------------.   |",
                beam + "|   |  " + eye + "  |   |",
                beam + "|   '------------'   |",
                gap  + "\\___   " + trace + "   ___/",
                gap  + "   '--.____.--'",
                gap  + "      /_/\\_\\",
                gap  + "   SCAN_DAEMON"
            };
        }


        private string[] BuildDeadScanDaemonArt(int frame)
        {
            string core = frame % 2 == 0 ? "EYE_LOST" : "NO_SIGNAL"; // 코어 정지
            string trace = frame % 2 == 0 ? "LOST " : "HALT "; // 상태 잔류
            string noise = frame % 2 == 0 ? "..." : "x.x"; // 잔류 신호
            string collapse = frame % 2 == 0 ? "     " : "   "; // 무너짐 위치 변화

            return new string[]
            {
                collapse + "  .----x----.",
                collapse + " _/   SCAN   \\_",
                collapse + "/    [ .... ]   \\",
                collapse + "|   .---xx---.   |",
                collapse + "|   | " + core + " |   |",
                collapse + "|   '---xx---'   |",
                collapse + "\\___   " + trace + "   _x/",
                collapse + "  '--x____x--'",
                collapse + "     /_x\\_x",
                collapse + "   " + noise + " SCAN_OFF"
            };
        }


        
        // MEM_LEAK_ANOMALY
        

        private string[] BuildMemLeakAnomalyArt(int frame)
        {
            int phase = (frame / 2) % 4; // 모션 단계
            string shift = phase == 1 ? " " : phase == 3 ? "  " : string.Empty; // 실루엣 흔들림
            string leftBits = phase % 2 == 0 ? "01010" : "10110"; // 좌측 비트 흐름
            string rightBits = phase % 2 == 0 ? "10101" : "01001"; // 우측 비트 흐름
            string leakTag = phase == 0 ? "[LEAK]" : phase == 1 ? "[DRIP]" : phase == 2 ? "[FLOW]" : "[SPIL]"; // 누수 상태
            string cap = phase < 2 ? "______" : "======"; // 외곽선 굵기 변화
            string flow = phase % 2 == 0 ? "~~~" : "^^^"; // 하단 흐름 변화

            return new string[]
            {
                shift + "    " + cap + " {0x00} _______",
                shift + "  _/      \\_    _/      \\_",
                shift + " /  " + leftBits + "   \\  /   " + rightBits + "  \\",
                shift + "|  " + leakTag + "    \\/    " + leakTag + "  |",
                shift + "|   ||        ||        ||  |",
                shift + " \\_||________||________||_/",
                shift + "    ||        ||        ||",
                shift + " " + flow + "/\\~~" + flow + "~~/\\~" + flow + "~~~/\\~~",
                shift + "      HEAP_FLOW_UNSTABLE"
            };
        }


        private string[] BuildHitMemLeakAnomalyArt(int frame)
        {
            string shift = frame % 2 == 0 ? "<<< " : "   >>> "; // 강한 피격 흔들림
            string noise = frame % 2 == 0 ? "##" : "!!"; // 피격 노이즈
            string leakTag = frame % 2 == 0 ? "[ERR!]" : "[DUMP]"; // 누수 오류
            string crack = frame % 2 == 0 ? "XX" : "##"; // 균열 표시
            string burst = frame % 2 == 0 ? "<<< HEAP BREACH >>>" : "*** MEMORY SPILL ***"; // 파티클

            return new string[]
            {
                shift + "   " + burst,
                shift + "    __" + noise + "___ {0xXX} ___" + noise + "_",
                shift + "  _/   X  \\_    _/  X   \\_",
                shift + " /  01X10  \\  /  10X01   \\",
                shift + "|  " + leakTag + "    \\/    " + leakTag + "  |",
                shift + "|   " + crack + "     XX ||     XX " + crack + "  |",
                shift + " \\_||___XX___||___XX___||_/",
                shift + "   !!||    ##  ||  ##    ||",
                shift + " ~~X/\\~~~###~/\\~~~###~/\\~~"
            };
        }


        private string[] BuildAttackMemLeakAnomalyArt(int frame)
        {
            string stream = frame % 2 == 0 ? "<<<<~~~~ " : "<<<<==== "; // 왼쪽 누수 공격
            string leakTag = frame % 2 == 0 ? "[LEAK]" : "[DUMP]"; // 공격 상태
            string leftBits = frame % 2 == 0 ? "11100" : "00111"; // 방출 비트
            string rightBits = frame % 2 == 0 ? "00011" : "11000"; // 방출 비트
            string gap = "         "; // 빔 없는 줄 정렬

            return new string[]
            {
                gap    + "    ______ {0x00} _______",
                gap    + "  _/  DUMP \\_    _/ LEAK \\_",
                stream + " /  " + leftBits + "   \\  /   " + rightBits + "  \\",
                stream + "|  " + leakTag + "    \\/    " + leakTag + "  |",
                stream + "|   ||        ||        ||  |",
                gap    + " \\_||________||________||_/",
                gap    + "    ||        ||        ||",
                gap    + " ~~~/\\~~~~~~~/\\~~~~~~~/\\~~",
                gap    + "      MEMORY_DRIP_ATTACK"
            };
        }


        private string[] BuildDeadMemLeakAnomalyArt(int frame)
        {
            string noise = frame % 2 == 0 ? "..." : "x.x"; // 잔류 신호
            string tag = frame % 2 == 0 ? "[NULL]" : "[VOID]"; // 누수 종료 상태
            string flow = frame % 2 == 0 ? "---" : "___"; // 흐름 정지

            return new string[]
            {
                "    ______ {0x00} _______",
                "  _/      x_    _x      \\_",
                " /  ....    \\  /    ....  \\",
                "|  " + tag + "    \\/    " + tag + "  |",
                "|   xx        xx        xx  |",
                " \\_||____xx__||__xx____||_/",
                "    ||        ||        ||",
                " " + flow + "/x\\-------/x\\-------/x\\--",
                "      " + noise + " HEAP_DRAINED"
            };
        }


        
        // LOGIC_BOMB
        

        private string[] BuildLogicBombArt(int frame)
        {
            int phase = (frame / 2) % 4; // 모션 단계
            string shift = phase == 1 ? " " : phase == 3 ? "  " : string.Empty; // 실루엣 흔들림
            string falseTag = phase % 2 == 0 ? "!FALSE" : "FALSE!"; // 논리값 점멸
            string trueTag = phase % 2 == 0 ? "!TRUE " : " TRUE!"; // 논리값 점멸
            string gateA = phase < 2 ? "AND" : "XOR"; // 논리 게이트 변화
            string gateB = phase < 2 ? "OR" : "NOR"; // 논리 게이트 변화
            string side = phase < 2 ? "==+====" : "##+####"; // 외곽선 굵기 변화

            return new string[]
            {
                shift + "        /\\  /\\ ",
                shift + "       /  \\/  \\",
                shift + "  .--<=[ " + falseTag + " ]=>--.",
                shift + "  |     /      \\    |",
                shift + side + "|   " + gateA + "  |====+==",
                shift + "  |     \\ " + gateB + "  /     |",
                shift + "  '--<=[ " + trueTag + " ]=>--'",
                shift + "       \\  /\\  /",
                shift + "        \\/  \\\'/",
                shift + "       LOGIC_BOMB"
            };
        }


        private string[] BuildHitLogicBombArt(int frame)
        {
            string shift = frame % 2 == 0 ? "<<< " : "   >>> "; // 강한 피격 흔들림
            string noise = frame % 2 == 0 ? "!!" : "##"; // 피격 노이즈
            string falseTag = frame % 2 == 0 ? "XFALSE" : "!F_X!"; // 손상 논리값
            string trueTag = frame % 2 == 0 ? "XTRUE " : "!T_X!"; // 손상 논리값
            string core = frame % 2 == 0 ? "ERR" : "BAD"; // 게이트 오류

            return new string[]
            {
                shift + "    " + noise + "  /\\  /\\  " + noise,
                shift + "       / X\\/X \\",
                shift + "  .XX<=[ " + falseTag + " ]=>XX.",
                shift + "##|     /  XX  \\    |##",
                shift + "==X====|   " + core + "  |====X==",
                shift + "##|     \\ X  X /    |##",
                shift + "  'XX<=[ " + trueTag + " ]=>XX'",
                shift + "       \\  /XX\\  /",
                shift + "        \\/ XX \\\'/",
                shift + "       LOGIC_BOMB"
            };
        }


        private string[] BuildAttackLogicBombArt(int frame)
        {
            string pulse = frame % 2 == 0 ? "<<<<==== " : "<<<<---- "; // 왼쪽 폭발 신호
            string falseTag = frame % 2 == 0 ? "!FALSE" : "FALSE!";
            string trueTag = frame % 2 == 0 ? "!TRUE " : " TRUE!";
            string trigger = frame % 2 == 0 ? "TRG" : "BOOM"; // 트리거 상태
            string gap = "         "; // 빔 없는 줄 정렬

            return new string[]
            {
                gap   + "        /\\  /\\ ",
                gap   + "       /  \\/  \\",
                pulse + "  .--<=[ " + falseTag + " ]=>--.",
                pulse + "  |     /      \\    |",
                pulse + "==+====|   " + trigger + " |====+==",
                pulse + "  |     \\ LOOP /    |",
                gap   + "  '--<=[ " + trueTag + " ]=>--'",
                gap   + "       \\  /\\  /",
                gap   + "        \\/  \\/",
                gap   + "      BOOLEAN_BLAST"
            };
        }


        private string[] BuildDeadLogicBombArt(int frame)
        {
            string noise = frame % 2 == 0 ? "..." : "x.x"; // 잔류 신호
            string falseTag = frame % 2 == 0 ? " FALSE" : "  OFF ";
            string trueTag = frame % 2 == 0 ? " TRUE " : "  OFF ";

            return new string[]
            {
                "        /x  x\\ ",
                "       /  \\/  \\",
                "  .--<=[ " + falseTag + " ]=>--.",
                "  |     /  --  \\    |",
                "==x====|  HALT |====x==",
                "  |     \\  --  /    |",
                "  '--<=[ " + trueTag + " ]=>--'",
                "       \\  /xx\\  /",
                "        \\/ xx \\/",
                "       " + noise + " DISARMED"
            };
        }


        
        // NULL_POINTER_VOID
        

        private string[] BuildNullPointerVoidArt(int frame)
        {
            int phase = (frame / 2) % 4; // 모션 단계
            string shift = phase == 1 ? " " : phase == 3 ? "  " : string.Empty; // 실루엣 흔들림
            string addr = phase % 2 == 0 ? "0x000000000000" : "0x00000000NULL"; // 주소값 점멸
            string state = phase < 2 ? "CRITICAL_VOID" : "DEREF_VOID"; // 중심 상태 변화
            string wave = phase < 2 ? "<<<<<" : ">>>>>"; // 외곽 파동 변화
            string shell = phase < 2 ? "[[[" : "###"; // 외곽선 굵기 변화

            return new string[]
            {
                shift + "    " + wave + " NULL_POINTER " + wave,
                shift + "  " + shell + "    " + addr + "    ]]]",
                shift + " [[    .----------------.    ]]",
                shift + "[     /  " + addr + " \\     ]",
                shift + "[    |   " + state + "   |    ]",
                shift + " [[   \\  Dereferencing  /  ]]",
                shift + "  " + shell + "  '----------------'  ]]",
                shift + "   <<<<" + wave + "<<<<>>>>" + wave + ">>>",
                shift + "       NULL_POINTER_VOID"
            };
        }


        private string[] BuildHitNullPointerVoidArt(int frame)
        {
            string shift = frame % 2 == 0 ? "<<< " : "   >>> "; // 강한 피격 흔들림
            string noise = frame % 2 == 0 ? "##" : "!!"; // 피격 노이즈
            string addr = frame % 2 == 0 ? "0x00XX00XX0000" : "0xNULL_NULL_ERR"; // 주소 손상
            string state = frame % 2 == 0 ? "SEG_FAULT_VOID" : "ACCESS_DENIED"; // 중심 오류

            return new string[]
            {
                shift + "  " + noise + "<<< NULL_POINTER >>>" + noise,
                shift + "[[[    " + addr + "    ]" + noise,
                shift + "[[    .------XX------.    ]]",
                shift + "[    /  0x0" + noise + "0XX0000 \\    ]",
                shift + "[   |   " + state + "   |   ]",
                shift + "[[   \\  Deref_X_ERR  /  ]]",
                shift + " " + noise + "[  '--XX------XX--'  ]]",
                shift + "  <<<<<<<<XXX>>>>>>>>>>",
                shift + "     NULL_POINTER_VOID"
            };
        }


        private string[] BuildAttackNullPointerVoidArt(int frame)
        {
            string pulse = frame % 2 == 0 ? "<<<<==== " : "<<<<---- "; // 왼쪽 공허 파동
            string addr = frame % 2 == 0 ? "0x000000000000" : "0xVOID_PULL_ERR"; // 공격 주소
            string state = frame % 2 == 0 ? "VOID_PULL_CORE" : "NULL_DEREF_NOW"; // 공격 상태
            string gap = "         "; // 비공격 줄 정렬

            return new string[]
            {
                pulse + "<<< NULL_POINTER >>>",
                pulse + "[[[    " + addr + "    ]]]",
                gap   + "[[    .----------------.    ]]",
                gap   + "[    /  0x000000000000 \\    ]",
                pulse + "[   |   " + state + "   |   ]",
                pulse + "[[   \\  Dereferencing  /  ]]",
                gap   + " [[[  '----------------'  ]]]",
                gap   + "  <<<<<<<<<<<<<>>>>>>>>>>>>",
                gap   + "      VOID_PULL_ATTACK"
            };
        }


        private string[] BuildDeadNullPointerVoidArt(int frame)
        {
            string noise = frame % 2 == 0 ? "..." : "x.x"; // 잔류 신호
            string addr = frame % 2 == 0 ? "0x00000000DEAD" : "0x00000000NULL"; // 소멸 주소
            string state = frame % 2 == 0 ? "VOID_COLLAPSE" : "NO_REFERENCE"; // 붕괴 상태

            return new string[]
            {
                "      <<<<< NULL_POINTER >>>>>",
                "  [[[    " + addr + "    ]]]",
                " [[    .------xx------.    ]]",
                "[     /  0x000000000000 \\     ]",
                "[    |   " + state + "   |    ]",
                " [[   \\  Deref_Halted  /  ]]",
                "  [[[  '--xx------xx--'  ]]]",
                "   <<<<<<<<<<<xx>>>>>>>>>",
                "       " + noise + " NULL_VOID"
            };
        }


        
        // PROTOCOL_MUNCHER
        

        private string[] BuildProtocolMuncherArt(int frame)
        {
            int phase = (frame / 2) % 4; // 모션 단계
            string shift = phase == 1 ? " " : phase == 3 ? "  " : string.Empty; // 실루엣 흔들림
            string eye = phase % 2 == 0 ? "(.)     (.)" : "(o)     (O)"; // 눈 점멸
            string mouth = phase % 2 == 0 ? "MMMMMMM" : "WWWWWWW"; // 입 움직임
            string edge = phase < 2 ? "----" : "===="; // 외곽선 굵기 변화
            string tag = phase < 2 ? "<SNIFFING>" : "<EATING>"; // 상태 변화

            return new string[]
            {
                shift + "  /\\_       _/\\",
                shift + " /   \\_____/   \\",
                shift + "|   " + eye + "  |",
                shift + "|     V_____V    |",
                shift + "|" + edge + "/" + mouth + "\\" + edge.Substring(0, 3) + "|",
                shift + "| < DATA_EATER > |",
                shift + "|____\\WWWWWWW/___|",
                shift + "     " + tag,
                shift + "   PROTOCOL_MUNCHER"
            };
        }


        private string[] BuildHitProtocolMuncherArt(int frame)
        {
            string shift = frame % 2 == 0 ? "<<< " : "   >>> "; // 강한 피격 흔들림
            string noise = frame % 2 == 0 ? "!!" : "##"; // 피격 노이즈
            string eye = frame % 2 == 0 ? "(X)     (X)" : "(.)     (X)"; // 눈 손상
            string mouth = frame % 2 == 0 ? "MMXMMMX" : "WWXWWWX"; // 입 손상

            return new string[]
            {
                shift + noise + " /\\_   X   _/\\ " + noise,
                shift + " / X \\_____/ X \\",
                shift + "|   " + eye + "   |",
                shift + "|     V__X__V     |",
                shift + "|--XX/" + mouth + "\\XX-|",
                shift + "|  < DATA_BREAK > |",
                shift + "|___X\\WWWXWWW/___|",
                shift + "     <SNIFF_ERR>",
                shift + "   PROTOCOL_MUNCHER"
            };
        }


        private string[] BuildAttackProtocolMuncherArt(int frame)
        {
            string bite = frame % 2 == 0 ? "<<<<MMMM " : "<<<<WWWW "; // 왼쪽 씹기 공격
            string eye = frame % 2 == 0 ? "(O)     (O)" : "(.)     (.)"; // 공격 눈
            string mouth = frame % 2 == 0 ? "MMMMMMM" : "WWWWWWW"; // 입 움직임
            string gap = "         "; // 비공격 줄 정렬

            return new string[]
            {
                gap  + "  /\\_       _/\\",
                gap  + " /   \\_____/   \\",
                bite + "|   " + eye + "  |",
                bite + "|     V_____V    |",
                bite + "|----/" + mouth + "\\---|",
                bite + "| < DATA_EATER > |",
                gap  + "|____\\WWWWWWW/___|",
                gap  + "     <PACKET_BITE>",
                gap  + "   PROTOCOL_MUNCHER"
            };
        }


        private string[] BuildDeadProtocolMuncherArt(int frame)
        {
            string noise = frame % 2 == 0 ? "..." : "x.x"; // 잔류 신호
            string mouth = frame % 2 == 0 ? "MMMMxMM" : "WWWWxWW"; // 정지한 입
            string tag = frame % 2 == 0 ? "<NO_PACKET>" : "<NO_SIGNAL>"; // 사망 상태

            return new string[]
            {
                "  /\\_       _/\\",
                " / x \\_____/ x \\",
                "|   (x)     (x)   |",
                "|     V__x__V     |",
                "|----/" + mouth + "\\---|",
                "|  < DATA_EMPTY > |",
                "|____\\WWWxWWW/___|",
                "     " + tag,
                "   " + noise + " MUNCHER_OFF"
            };
        }


        
        // SANDBOX_ISOLATION
        

        private string[] BuildSandboxIsolationArt(int frame)
        {
            int phase = (frame / 2) % 4; // 모션 단계
            string shift = phase == 1 ? " " : phase == 3 ? "  " : string.Empty; // 실루엣 흔들림
            string status = phase % 2 == 0 ? "VIRTUAL_ENV_READY" : "VIRTUAL_ENV_LOCKD"; // 가상환경 상태
            string access = phase < 2 ? "RESTRICTED_ACCESS" : "ISOLATED_PROCESS"; // 격리 상태
            string frameText = phase < 2 ? "SANDBOX_ZONE" : "QUARANT_ZONE"; // 상단 타이틀 점멸
            string border = phase < 2 ? "=====" : "#####"; // 외곽선 굵기 변화

            return new string[]
            {
                shift + " .-------------------------.",
                shift + "/" + border + "[ " + frameText + " ]" + border + "\\",
                shift + "||  .--------------------.  ||",
                shift + "||  | " + status + " |  ||",
                shift + "||  | " + access + " |  ||",
                shift + "||  '--------------------'  ||",
                shift + "\\==" + border + "==" + border + "===" + border + "==/",
                shift + "   [ STATUS: QUARANTINED ]",
                shift + "      SANDBOX_ISOLATION"
            };
        }


        private string[] BuildHitSandboxIsolationArt(int frame)
        {
            string shift = frame % 2 == 0 ? "<<< " : "   >>> "; // 강한 피격 흔들림
            string noise = frame % 2 == 0 ? "!!" : "##"; // 피격 노이즈
            string status = frame % 2 == 0 ? "VIRTUAL_ENV_BREACH" : "VIRTUAL_ENV_ERROR "; // 환경 오류
            string access = frame % 2 == 0 ? "RESTRICTED_BROKEN" : "ISOLATION_FAULT "; // 격리 실패

            return new string[]
            {
                shift + " ." + noise + "---------------------" + noise + ".",
                shift + "/==XX=[ SANDBOX_ZONE ]=XX==\\",
                shift + "||  .------XX----------.  ||",
                shift + "||  | " + status + " |  ||",
                shift + "||  | " + access + " |  ||",
                shift + "||  '------XX----------'  ||",
                shift + "\\==========XX==============/",
                shift + "   [ STATUS: BREACH_ERR ]",
                shift + "      SANDBOX_ISOLATION"
            };
        }


        private string[] BuildAttackSandboxIsolationArt(int frame)
        {
            string pulse = frame % 2 == 0 ? "<<<<==== " : "<<<<---- "; // 왼쪽 격리 파동
            string status = frame % 2 == 0 ? "VIRTUAL_ENV_READY" : "LOCK_TARGET_THREAD"; // 공격 상태
            string access = frame % 2 == 0 ? "RESTRICTED_ACCESS" : "THREAD_ISOLATED "; // 구금 상태
            string gap = "         "; // 비공격 줄 정렬

            return new string[]
            {
                gap   + " .-------------------------.",
                gap   + "/=====[ SANDBOX_ZONE ]=====\\",
                pulse + "||  .--------------------.  ||",
                pulse + "||  | " + status + " |  ||",
                pulse + "||  | " + access + " |  ||",
                pulse + "||  '--------------------'  ||",
                gap   + "\\==========================/",
                gap   + "   [ STATUS: CONTAINING ]",
                gap   + "      SANDBOX_ISOLATION"
            };
        }


        private string[] BuildDeadSandboxIsolationArt(int frame)
        {
            string noise = frame % 2 == 0 ? "..." : "x.x"; // 잔류 신호
            string status = frame % 2 == 0 ? "VIRTUAL_ENV_OFF  " : "VIRTUAL_ENV_NULL "; // 환경 종료
            string access = frame % 2 == 0 ? "RESTRICTED_LOST " : "ISOLATION_HALTED"; // 격리 종료

            return new string[]
            {
                " .-----------x------------.",
                "/=====[ SANDBOX_ZONE ]=====\\",
                "||  .------xx----------.  ||",
                "||  | " + status + " |  ||",
                "||  | " + access + " |  ||",
                "||  '------xx----------'  ||",
                "\\==========xx==============/",
                "   [ STATUS: RELEASED   ]",
                "      " + noise + " SANDBOX_OFF"
            };
        }


        
        // CIPHER_BLOCK_CHAIN
        

        private string[] BuildCipherBlockChainArt(int frame)
        {
            int phase = (frame / 2) % 4; // 모션 단계
            string shift = phase == 1 ? " " : phase == 3 ? "  " : string.Empty; // 실루엣 흔들림
            string iv = phase % 2 == 0 ? "IV01" : "IV10"; // 초기 벡터 점멸
            string b1 = phase < 2 ? "BLK1" : "ENC1"; // 블록 상태
            string b2 = phase < 2 ? "BLK2" : "ENC2"; // 블록 상태
            string hash = phase % 2 == 0 ? "HASH" : "KEY "; // 해시 상태
            string arrow = phase < 2 ? "=>" : "->"; // 체인 흐름
            string box = phase < 2 ? "+------+" : "#======#"; // 외곽선 굵기 변화

            return new string[]
            {
                shift + "  " + box + "  " + box + "  ",
                shift + "  | " + iv + " |" + arrow + "| " + b1 + " |" + arrow,
                shift + "  | ENC  |  | ENC  |  ",
                shift + "  " + box + "  " + box + "  ",
                shift + "     ||        ||     ",
                shift + "  " + box + "  " + box + "  ",
                shift + "  | " + b2 + " |" + arrow + "| " + hash + " |  ",
                shift + "  | ENC  |  | KEY  |  ",
                shift + "  " + box + "  " + box + "  ",
                "  CIPHER_LINK_DOWN"
            };
        }


        private string[] BuildHitCipherBlockChainArt(int frame)
        {
            string shift = frame % 2 == 0 ? "<<< " : "   >>> "; // 강한 피격 흔들림
            string noise = frame % 2 == 0 ? "!!" : "##"; // 피격 노이즈
            string iv = frame % 2 == 0 ? "IX01" : "Xv10"; // 손상 벡터
            string b1 = frame % 2 == 0 ? "BLX1" : "ERR1"; // 손상 블록
            string b2 = frame % 2 == 0 ? "BLX2" : "ERR2"; // 손상 블록
            string hash = frame % 2 == 0 ? "HSHX" : "KEYX"; // 손상 해시

            return new string[]
            {
                shift + "  +--XX--+  +--XX--+  ",
                shift + "  | " + iv + " |XX| " + b1 + " |XX",
                shift + "  | EXC  |  | EXC  |  ",
                shift + "  +--XX--+  +--XX--+  ",
                shift + "     X| " + noise + "     |X     ",
                shift + "  +--XX--+  +--XX--+  ",
                shift + "  | " + b2 + " |XX| " + hash + " |  ",
                shift + "  | EXC  |  | B" + noise + "  |  ",
                shift + "  +--XX--+  +--XX--+  ",
                "  CIPHER_LINK_DOWN"
            };
        }


        private string[] BuildAttackCipherBlockChainArt(int frame)
        {
            string pulse = frame % 2 == 0 ? "<<<<==== " : "<<<<---- "; // 왼쪽 암호 체인 공격
            string iv = frame % 2 == 0 ? "IV01" : "IV10"; // 초기 벡터
            string b1 = frame % 2 == 0 ? "BLK1" : "ENC1"; // 체인 블록
            string b2 = frame % 2 == 0 ? "BLK2" : "ENC2"; // 체인 블록
            string hash = frame % 2 == 0 ? "HASH" : "KEY "; // 해시 블록
            string gap = "         "; // 비공격 줄 정렬

            return new string[]
            {
                pulse + "  +------+  +------+  ",
                pulse + "  | " + iv + " |=>| " + b1 + " |=>",
                pulse + "  | ENC  |  | ENC  |  ",
                gap   + "  +------+  +------+  ",
                gap   + "     ||        ||     ",
                gap   + "  +------+  +------+  ",
                gap   + "  | " + b2 + " |=>| " + hash + " |  ",
                gap   + "  | ENC  |  | KEY  |  ",
                gap   + "  +------+  +------+  ",
                "  CIPHER_LINK_DOWN"
            };
        }


        private string[] BuildDeadCipherBlockChainArt(int frame)
        {
            string noise = frame % 2 == 0 ? "..." : "x.x"; // 잔류 신호
            string iv = frame % 2 == 0 ? "NULL" : "DEAD"; // 종료 벡터
            string b1 = frame % 2 == 0 ? "OFF1" : "BRK1"; // 종료 블록
            string b2 = frame % 2 == 0 ? "OFF2" : "BRK2"; // 종료 블록
            string hash = frame % 2 == 0 ? "LOST" : "ZERO"; // 종료 해시

            return new string[]
            {
                "  +-xxx--+  +--xxx-+  ",
                "  | " + iv + " |xx| " + b1 + " |xx",
                "  | OFF  |  | OFF  |  ",
                "  +--xx--+  +x-xx--+  ",
                "     xx        xx     ",
                "  +--xxx-+  +--xx--+  ",
                "  | " + b2 + " |xx| " + hash + " |  ",
                "  | OFF  |  | NIL  |  ",
                "  +xx----+  +---xx-+  ",
                "  CIPHER_LINK_DOWN"
            };
        }


    }
}
