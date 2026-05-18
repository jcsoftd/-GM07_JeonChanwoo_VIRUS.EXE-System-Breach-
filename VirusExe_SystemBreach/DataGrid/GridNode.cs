namespace VirusExe.SystemBreach.DataGrid
{
    // SIGNAL GRID의 노드 한 칸 데이터
    // 노드 타입, 클리어 여부, 연결 방향, 감염 상태 관리
    public class GridNode
    {
        public NodeType Type { get; set; } // 노드타입 저장

        public bool IsCleared { get; set; }

        public bool IsVisible { get; set; }

        public bool IsActive { get; set; }

        public bool IsInfected { get; set; } // VIRUS.EXE 감염 여부 저장

        public bool CanUp { get; set; }

        public bool CanDown { get; set; }

        public bool CanLeft { get; set; }

        public bool CanRight { get; set; }

        public GridNode(NodeType type)
        {
            Type = type; // 노드타입 설정

            IsCleared = false; // 미장악 상태 시작
            IsVisible = false; // 숨김 상태 시작
            IsActive = false; // 진행 후 활성화
            IsInfected = false; // 미감염 상태

            CanUp = false; // 신호선 닫힘
            CanDown = false; // 아래쪽 신호선 닫힘
            CanLeft = false; // 왼쪽 신호선 닫힘
            CanRight = false; // 오른쪽 신호선 닫힘
        }

        public bool IsReusableNode()
        {
            return Type == NodeType.Start || Type == NodeType.Boss; // 시작/보스 반복노드
        }
    }
}
