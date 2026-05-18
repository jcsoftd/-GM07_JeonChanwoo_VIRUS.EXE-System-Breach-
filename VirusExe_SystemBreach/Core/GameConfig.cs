using VirusExe.SystemBreach.DataGrid;

namespace VirusExe.SystemBreach.Core
{
    // 게임 전체 공통 설정
    // 콘솔 크기, 보스 권한, TRACE 증가량 같이 여러 곳에서 같이 쓰는 값 관리
    public static class GameConfig
    {
        public const int ConsoleWidth = 110; // 콘솔 문자 너비
        public const int ConsoleHeight = 47; // 콘솔 문자 줄수
        public const int ConsolePixelWidth = 920; // 콘솔 창 가로 픽셀
        public const int ConsolePixelHeight = 830; // 콘솔 창 세로 픽셀
        public const int BossRequiredAccess = 10; // 필요한 ACCESS LEVEL
        public const int MaxSystemInfection = 100; // 추적도 최대값
        public const int ScanRevealRadius = 2; // SCAN 공개하는 전방 열 수

        public static int GetTraceIncreaseByNodeType(NodeType nodeType)
        {
            if (nodeType == NodeType.Security) return 4; // /Sec 클리어 TRACE
            if (nodeType == NodeType.Firewall) return 9; // /Fw 클리어 TRACE
            if (nodeType == NodeType.Event) return 3; // /Tmp 클리어 TRACE
            if (nodeType == NodeType.Boss) return 15; // Kernel 클리어 TRACE
            if (nodeType == NodeType.DataCache) return 0; // /Data TRACE 없음
            if (nodeType == NodeType.Shop) return 0; // /Mkt TRACE 없음
            if (nodeType == NodeType.Mutation) return 0; // /Lab TRACE 없음

            return 0;
        }
    }
}
