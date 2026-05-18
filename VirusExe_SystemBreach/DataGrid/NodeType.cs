namespace VirusExe.SystemBreach.DataGrid
{
    // SIGNAL GRID 노드
    public enum NodeType
    {
        Start, // 시작 노드
        Security, // 일반 전투 노드
        Firewall, // 엘리트 전투 노드
        Shop, // 상점 노드
        Mutation, // 강화 노드
        Event, // 이벤트 노드
        DataCache, // 회복 노드
        Boss, // 최종 보스 노드
        Empty // 빈 노드
    }
}
