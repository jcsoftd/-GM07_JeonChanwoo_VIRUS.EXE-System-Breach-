namespace VirusExe.SystemBreach.Rendering
{
    // 엘리트 몬스터 ASCII 아트
    // /Fw 전투에서 쓰는 큰 보안 프로세스 모션 관리
    public partial class ConsoleRenderer
    {
        private string[] BuildEliteEnemyArt(string enemyName, int frame)
        {
            switch (enemyName) // 엘리트 몬스터 이름별 상시 아트
            {
                case "PROXY_SINGULARITY": return BuildProxySingularityArt(frame);
                case "SYN_FLOOD_GATE": return BuildSynFloodGateArt(frame);
                case "IC_CRYPTO_GATE": return BuildIcCryptoGateArt(frame);
                default: return BuildProxySingularityArt(frame); // 기본 엘리트 fallback
            }
        }


        private string[] BuildHitEliteEnemyArt(string enemyName, int frame)
        {
            switch (enemyName) // 엘리트 몬스터 이름별 피격 아트
            {
                case "PROXY_SINGULARITY": return BuildHitProxySingularityArt(frame);
                case "SYN_FLOOD_GATE": return BuildHitSynFloodGateArt(frame);
                case "IC_CRYPTO_GATE": return BuildHitIcCryptoGateArt(frame);
                default: return BuildHitProxySingularityArt(frame); // 기본 엘리트 피격 fallback
            }
        }


        private string[] BuildAttackEliteEnemyArt(string enemyName, int frame)
        {
            switch (enemyName) // 엘리트 몬스터 이름별 공격 아트
            {
                case "PROXY_SINGULARITY": return BuildAttackProxySingularityArt(frame);
                case "SYN_FLOOD_GATE": return BuildAttackSynFloodGateArt(frame);
                case "IC_CRYPTO_GATE": return BuildAttackIcCryptoGateArt(frame);
                default: return BuildAttackProxySingularityArt(frame); // 기본 엘리트 공격 fallback
            }
        }


        private string[] BuildDeadEliteEnemyArt(string enemyName, int frame)
        {
            switch (enemyName) // 엘리트 몬스터 이름별 사망 아트
            {
                case "PROXY_SINGULARITY": return BuildDeadProxySingularityArt(frame);
                case "SYN_FLOOD_GATE": return BuildDeadSynFloodGateArt(frame);
                case "IC_CRYPTO_GATE": return BuildDeadIcCryptoGateArt(frame);
                default: return BuildDeadProxySingularityArt(frame); // 기본 엘리트 사망 fallback
            }
        }


        
        // PROXY_SINGULARITY
        


        

        private string[] BuildProxySingularityArt(int frame)
        {
            int phase = (frame / 2) % 4; // 모션 속도 완화
            string shift = phase == 1 ? " " : phase == 3 ? "  " : string.Empty; // 좌우 미세 이동
            string link = phase % 2 == 0 ? "-------" : "======="; // 링크 선 굵기 변화
            string shell = phase % 2 == 0 ? "=====================" : "#####################"; // 외곽 밀도 변화
            string core = phase % 2 == 0 ? " VORTEX " : " VOIDFX "; // 코어 맥동
            string orb = phase % 2 == 0 ? "(%)" : "(@)"; // 흡수 노드 점멸
            string route = phase % 2 == 0 ? "REQ_407 REDIRECT" : "LOOPBACK ROUTE  "; // 라우팅 상태

            return new string[]
            {
                shift + "              |        |        |              ",
                shift + "       " + link + "+--------+--------+" + link + "       ",
                shift + "    .-" + shell + "-.    ",
                shift + "   /      [ PROXY_TRAFFIC_TUNNEL ]       \\   ",
                shift + "  |     ---=====[ " + core + " ]=====---     |  ",
                shift + "---+--- [ " + orb + "   " + orb + "   " + orb + "   " + orb + " ] ---+---",
                shift + "  |   | >>> " + route + " <<< |    |  ",
                shift + "---+--- [ " + orb + "   " + orb + "   " + orb + "   " + orb + " ] ---+---",
                shift + "  |     ---=====[ LOOPBACK ]=====---      |  ",
                shift + "   \\    [ ERR : INFINITE_LOOPBACK ]     /   ",
                shift + "    '-" + shell + "-'    ",
                shift + "       " + link + "+--------+--------+" + link + "       ",
                shift + "              |        |        |              "
            };
        }


        

        private string[] BuildHitProxySingularityArt(int frame)
        {
            string noise = frame % 2 == 0 ? "!!" : "##"; // 피격 노이즈
            string core = frame % 2 == 0 ? "V0RT_X" : "VORT_ERR"; // 코어 오류
            string req = frame % 2 == 0 ? "REQ_4XX BROKEN" : "LOOP_ERR TRACE"; // 라우트 오류
            string orb = frame % 2 == 0 ? "(X)" : "(%)"; // 흡수 노드 손상
            string shift = frame % 2 == 0 ? "<<" : "  >>"; // 좌우 흔들림

            return new string[]
            {
                shift + "              |    X   |   X    |              ",
                shift + "        ---XX-+----" + noise + "--+----" + noise + "--+--XX---        ",
                shift + "     .-====" + noise + "========XX========" + noise + "====-.     ",
                shift + "    /   [ PROXY_TRAFFIC_BREACH ]   \\    ",
                shift + "   |   ----===[" + core + "]===----    |   ",
                shift + "XX-+-- [ " + orb + "  (X)  " + orb + "  (X) ] --+-XX",
                shift + "   |   | >>> " + req + " <<< |   |   ",
                shift + "XX-+-- [ (X)  " + orb + "  (X)  " + orb + " ] --+-XX",
                shift + "   |   ----===[ LOOP_X ]===----     |   ",
                shift + "    \\  [ ERR : REDIRECT_FAILURE ] /    ",
                shift + "     '-====XX============XX====-'      ",
                shift + "        ---XX-+----XX----+--XX---      ",
                shift + "              |    X   |   X    |      "
            };
        }


        

        private string[] BuildAttackProxySingularityArt(int frame)
        {
            string pulse = frame % 2 == 0 ? "<<<<==== " : "<<<<---- "; // 왼쪽 프록시 파동
            string core = frame % 2 == 0 ? "VORTEX" : "PULLER"; // 흡수 상태
            string req = frame % 2 == 0 ? "REQ_407 REDIRECT" : "VOID_ROUTE OPEN"; // 공격 상태
            string orb = frame % 2 == 0 ? "(%)" : "(@)"; // 흡수 노드 강화
            string gap = "         "; // 비공격 줄 정렬

            return new string[]
            {
                gap   + "              |        |        |              ",
                gap   + "       -------+--------+--------+-------       ",
                pulse + "    .-=================================-.      ",
                pulse + "   /      [ REVERSE_PROXY_ACTIVE ]       \\     ",
                pulse + "  |     ----=====[  " + core + "  ]=====----     |    ",
                pulse + "--+--- [  " + orb + "   " + orb + "   " + orb + "   " + orb + "  ] ---+--",
                pulse + "  |    |  >>> " + req + " <<<   |     |    ",
                pulse + "--+--- [  " + orb + "   " + orb + "   " + orb + "   " + orb + "  ] ---+--",
                gap   + "  |     ----=====[ LOOPBACK ]=====----    |    ",
                gap   + "   \\    [ ENERGY_ROUTE_COLLAPSE ]       /      ",
                gap   + "    '-=================================-'     ",
                gap   + "       -------+--------+--------+-------      ",
                gap   + "              |        |        |             "
            };
        }


        

        private string[] BuildDeadProxySingularityArt(int frame)
        {
            string noise = frame % 2 == 0 ? "..." : "x.x"; // 잔류 신호
            string core = frame % 2 == 0 ? "VOID_OFF" : "NO_ROUTE"; // 코어 정지
            string orb = frame % 2 == 0 ? "(x)" : "(.)"; // 붕괴 노드

            return new string[]
            {
                "                  x        x        x                  ",
                "           ---x---+----x---+---x----+---x---           ",
                "        .-====xx=================xx====-.              ",
                "       /      [ PROXY_TUNNEL_CLOSED ]     \\             ",
                "      |     ----=====[ " + core + " ]=====----     |        ",
                "   ---x--- [  " + orb + "   (x)   " + orb + "   (x)  ] ---x---    ",
                "      |    |  >>> REDIRECT_LOST <<<    |     |        ",
                "   ---x--- [  (x)   " + orb + "   (x)   " + orb + "  ] ---x---    ",
                "      |     ----=====[ LOOP_END ]=====----   |        ",
                "       \\    [ ERR : SINGULARITY_FADED ]   /          ",
                "        '-====xx=================xx====-'              ",
                "           ---x---+----x---+---x----+---x---           ",
                "                  " + noise + "      " + noise + "      " + noise + "             "
            };
        }


        
        // SYN_FLOOD_GATE
        


        private string[] BuildSynFloodGateArt(int frame)
        {
            int phase = (frame / 2) % 4; // 모션 속도 완화
            string shift = phase == 1 ? " " : phase == 3 ? "  " : string.Empty; // 좌우 미세 이동
            string block = phase % 2 == 0 ? "#######" : "%%%%%%%"; // 포트 블록 밀도 변화
            string line = phase % 2 == 0 ? "----------------------------" : "============================"; // 상단 선 굵기 변화
            string rate = phase % 2 == 0 ? "THROTTLE" : "LIMITING "; // 제한 상태
            string gate = phase % 3 == 0 ? "INTERRUPT" : "DROP_REQ "; // 차단 상태
            string queue = phase % 2 == 0 ? "QUEUE_BACKLOG_MAX" : "QUEUE_BACKLOG_999"; // 큐 상태
            string node = phase % 2 == 0 ? "[OOO]" : "[0O0]"; // 큐 노드 점멸

            return new string[]
            {
                shift + "  [ SYN_FLOOD_GATE : ACTIVE ]                    ",
                shift + "  +-------+" + line + "+         ",
                shift + "  |" + block + "|  [ RATE_LIMIT: " + rate + " ]  |         ",
                shift + "  +-------+---===[ " + gate + " ]===------+         ",
                shift + "          |      " + node + "   " + node + "   " + node + "  |         ",
                shift + "          |       >>> DELAY_INJECT <<<  |         ",
                shift + "          +-------======================+         ",
                shift + "          |" + block + "|                               ",
                shift + "          +-------+--- [ HARDWARE_HALT ]          ",
                shift + "                  |    " + queue + "          ",
                shift + "                  |    STATUS: REJECTING          ",
                shift + "                  +---------------------+         ",
                shift + "                  |#####################|         "
            };
        }


        private string[] BuildHitSynFloodGateArt(int frame)
        {
            string noise = frame % 2 == 0 ? "!!" : "##"; // 피격 노이즈
            string rate = frame % 2 == 0 ? "THROTTLE_X" : "LIMIT_ERR "; // 제한 오류
            string queue = frame % 2 == 0 ? "QUEUE_BACKLOG_ERR" : "QUEUE_OVERFLOW_X"; // 큐 오류
            string shift = frame % 2 == 0 ? "<<" : "  >>"; // 좌우 흔들림

            return new string[]
            {
                shift + " [ " + noise + " SYN_FLOOD_GATE : BREACH " + noise + " ]          ",
                shift + " +---XX--+----------------------XX----+       ",
                shift + " |###XX##|  [ RATE_LIMIT: " + rate + " ] |      ",
                shift + " +---XX--+---===[ INTERRUPT_X ]===----+      ",
                shift + "         |    [XOO]   [OXO]   [OOX]  |       ",
                shift + "         |     >>> DELAY_BREAK <<<    |       ",
                shift + "         +---XX--====================+       ",
                shift + "         |###XX##|                            ",
                shift + "         +---XX--+--- [ HARDWARE_ERR ]        ",
                shift + "                 |    " + queue + "        ",
                shift + "                 |    STATUS: DESYNC          ",
                shift + "                 +----------XX---------+      ",
                shift + "                 |########XX###########|      "
            };
        }


        private string[] BuildAttackSynFloodGateArt(int frame)
        {
            string pulse = frame % 2 == 0 ? "<<<<==== " : "<<<<---- "; // 왼쪽 지연 공격
            string gate = frame % 2 == 0 ? "SYN_FLOOD_GATE" : "PORT_SHIELD_ON"; // 게이트 상태
            string rate = frame % 2 == 0 ? "THROTTLE" : "DROP_REQ "; // 공격 상태
            string gap = "         "; // 비공격 줄 정렬

            return new string[]
            {
                pulse + "[ " + gate + " : ACTIVE ]             ",
                pulse + "+-------+----------------------------+      ",
                pulse + "|#######|  [ RATE_LIMIT: " + rate + " ]  |      ",
                pulse + "+-------+---===[ INTERRUPT ]===------+      ",
                pulse + "        |      [OOO]   [OOO]   [OOO] |      ",
                pulse + "        |       >>> DELAY_INJECT <<< |      ",
                gap   + "        +-------=====================+      ",
                gap   + "        |#######|                            ",
                gap   + "        +-------+--- [ HARDWARE_HALT ]       ",
                gap   + "                |    QUEUE_BACKLOG_MAX       ",
                gap   + "                |    STATUS: REJECTING       ",
                gap   + "                +---------------------+      ",
                gap   + "                |#####################|      "
            };
        }


        private string[] BuildDeadSynFloodGateArt(int frame)
        {
            string noise = frame % 2 == 0 ? "..." : "x.x"; // 잔류 신호
            string rate = frame % 2 == 0 ? "OFFLINE " : "NO_LIMIT"; // 제한 종료
            string queue = frame % 2 == 0 ? "QUEUE_DRAINED___" : "QUEUE_NULL______"; // 큐 정지

            return new string[]
            {
                "  [ SYN_FLOOD_GATE : HALTED ]                    ",
                "  +---xx--+-----------------------xx---+         ",
                "  |###xx##|  [ RATE_LIMIT: " + rate + " ]  |         ",
                "  +---xx--+---===[ NO_SIGNAL ]===------+         ",
                "          |      [xOO]   [OxO]   [OOx]  |         ",
                "          |       >>> DELAY_END <<<     |         ",
                "          +---xx--=====================+         ",
                "          |###xx##|                               ",
                "          +---xx--+--- [ HARDWARE_OFF ]           ",
                "                  |    " + queue + "          ",
                "                  |    STATUS: REJECT_OFF         ",
                "                  +----------xx---------+         ",
                "                  " + noise + "############" + noise + "         "
            };
        }


        
        // IC_CRYPTO_GATE
        


        private string[] BuildIcCryptoGateArt(int frame)
        {
            int phase = (frame / 2) % 4; // 모션 속도 완화
            string shift = phase == 1 ? " " : phase == 3 ? "  " : string.Empty; // 좌우 미세 이동
            string pin = phase % 2 == 0 ? "|" : "!"; // 핀 진동
            string bus = phase % 2 == 0 ? "+--+--+--+--+" : "#==#==#==#==#"; // 회로 외곽 굵기 변화
            string logic = phase % 2 == 0 ? "XOR/AND/OR " : "AND/XOR/NOR"; // 논리 게이트 변화
            string split = phase % 2 == 0 ? "LOGIC_SPLIT" : "DATA_SPLIT "; // 분할 상태
            string crush = phase % 2 == 0 ? "DATA_CRUSH" : "HASH_CRUSH"; // 분쇄 상태
            string chip = phase % 2 == 0 ? "[ IC_CRYPTO_GATE ]" : "[ IC_CRYPTO_CORE ]"; // 중앙 칩 상태

            return new string[]
            {
                shift + "      o--------o        o--------o      ",
                shift + "      " + pin + "  " + pin + "  " + pin + "  " + pin + "        " + pin + "  " + pin + "  " + pin + "  " + pin + "      ",
                shift + "   " + bus + "--------" + bus + "   ",
                shift + "===|        " + chip + "       |===",
                shift + "===|       |== " + logic + " ==|     |===",
                shift + "===|       [ " + split + " ]         |===",
                shift + "===|  o---o                    o---o |===",
                shift + "===|  |###|  >>> " + crush + " <<< |###| |===",
                shift + "===|  o---o                    o---o |===",
                shift + "===|       [STATUS: INBOUND_HALT]    |===",
                shift + "   " + bus + "--------" + bus + "   ",
                shift + "      " + pin + "  " + pin + "  " + pin + "  " + pin + "        " + pin + "  " + pin + "  " + pin + "  " + pin + "      ",
                shift + "      o--------o        o--------o      "
            };
        }


        private string[] BuildHitIcCryptoGateArt(int frame)
        {
            string noise = frame % 2 == 0 ? "!!" : "##"; // 피격 노이즈
            string logic = frame % 2 == 0 ? "X0R / XND / ERR" : "BAD / XOR / NUL"; // 논리 오류
            string split = frame % 2 == 0 ? "LOGIC_BREAKER " : "DATA_FAULT_XX "; // 분할 오류
            string crush = frame % 2 == 0 ? "DATA_BREACH" : "HASH_CRACK"; // 분쇄 오류
            string shift = frame % 2 == 0 ? "<<" : "  >>"; // 좌우 흔들림

            return new string[]
            {
                shift + "   o----XX--o        o--XX----o      ",
                shift + "   |  X  |  X        X  |  X  |      ",
                shift + "+--+--XX--+--+----XX--+--XX--+--+   ",
                shift + "==| " + noise + "     [ IC_CRYPTO_ERR ]      " + noise + " |==",
                shift + "==|      |== " + logic + " ==|   |==",
                shift + "==|      [ " + split + " ]       |==",
                shift + "==|  o-X-o                    o-X-o|==",
                shift + "==|  |#X#|  >>> " + crush + " <<< |#X#||==",
                shift + "==|  o-X-o                    o-X-o|==",
                shift + "==|      [STATUS: PIN_FAILURE]   |==",
                shift + "+--XX--+--+--+----XX--+--+--XX--+   ",
                shift + "   X  |  X  |        |  X  |  X      ",
                shift + "   o----XX--o        o--XX----o      "
            };
        }


        private string[] BuildAttackIcCryptoGateArt(int frame)
        {
            string pulse = frame % 2 == 0 ? "<<<<==== " : "<<<<---- "; // 왼쪽 회로 공격
            string logic = frame % 2 == 0 ? "XOR / AND / OR" : "XOR / XOR / OR "; // 논리 공격
            string split = frame % 2 == 0 ? "LOGIC_SPLITTER" : "PACKET_SPLITTER"; // 패킷 분해
            string crush = frame % 2 == 0 ? "DATA_CRUSH" : "KEY_CRUSH "; // 분쇄 공격
            string gap = "         "; // 비공격 줄 정렬

            return new string[]
            {
                gap   + "      o--------o        o--------o      ",
                gap   + "      |  |  |  |        |  |  |  |      ",
                pulse + "   +--+--+--+--+--------+--+--+--+--+   ",
                pulse + "===|        [ IC_CRYPTO_GATE ]       |===",
                pulse + "===|       |== " + logic + " ==|    |===",
                pulse + "===|       [ " + split + " ]        |===",
                pulse + "===|  o---o                    o---o |===",
                pulse + "===|  |###|  >>> " + crush + " <<< |###| |===",
                gap   + "===|  o---o                    o---o |===",
                gap   + "===|       [STATUS: INBOUND_HALT]    |===",
                gap   + "   +--+--+--+--+--------+--+--+--+--+   ",
                gap   + "      |  |  |  |        |  |  |  |      ",
                gap   + "      o--------o        o--------o      "
            };
        }


        private string[] BuildDeadIcCryptoGateArt(int frame)
        {
            string noise = frame % 2 == 0 ? "..." : "x.x"; // 잔류 신호
            string logic = frame % 2 == 0 ? "OFF / OFF / NIL" : "NUL / NUL / ERR"; // 논리 정지
            string split = frame % 2 == 0 ? "LOGIC_OFFLINE " : "DATA_ROUTE_LOST"; // 분할 정지
            string crush = frame % 2 == 0 ? "DATA_DUST " : "KEY_FADED "; // 분쇄 종료

            return new string[]
            {
                "      o----xx--o        o--xx----o      ",
                "      x  |  x  |        |  x  |  x      ",
                "   +--+--xx--+--+----xx--+--xx--+--+   ",
                "===|        [ IC_CRYPTO_HALTED ]     |===",
                "===|       |== " + logic + " ==|    |===",
                "===|       [ " + split + " ]        |===",
                "===|  o-x-o                    o-x-o |===",
                "===|  |x#x|  >>> " + crush + " <<< |x#x| |===",
                "===|  o-x-o                    o-x-o |===",
                "===|       [STATUS: GATE_OFFLINE]   |===",
                "   +--xx--+--+--+----xx--+--+--xx--+   ",
                "      x  |  x  |        |  x  |  x      ",
                "      " + noise + "------x        x------" + noise + "      "
            };
        }


    }
}
