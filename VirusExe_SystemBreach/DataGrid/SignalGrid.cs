using System;
using System.Collections.Generic;

namespace VirusExe.SystemBreach.DataGrid
{
    // SIGNAL GRID 맵 생성/이동 관리
    // 13열 x 3행 맵을 만들고 현재 위치 기준 이동/스크롤에 필요한 정보 관리
    public class SignalGrid
    {
        private const int CenterRow = 1; // 중앙 행
        private const int StartColumn = 0; // 1열 시작
        private const int FirstCombatColumn = 1; // 2열 첫 전투
        private const int FirstGateColumn = 4; // 5열 FW 게이트
        private const int SecondGateColumn = 8; // 9열 FW 게이트
        private const int BossColumn = 12; // 13열 보스

        private readonly Random random = new Random(); 

        public GridNode[,] Nodes { get; private set; } // 노드맵 저장

        public int Width { get; private set; } // 맵 가로 저장

        public int Height { get; private set; } // 맵 세로 저장

        public int PlayerX { get; private set; } // 플레이어 x위치 저장

        public int PlayerY { get; private set; } // 플레이어 y위치 저장

        private struct GridSlot
        {
            public int X; // 슬롯 x좌표
            public int Y; // 슬롯 y좌표

            public GridSlot(int x, int y)
            {
                X = x; // x좌표 저장
                Y = y; // y좌표 저장
            }
        }

        public SignalGrid()
        {
            Width = 13; // 13열 침투 경로 설정
            Height = 3; // 3행 선택지 설정
            Nodes = new GridNode[Width, Height]; // 노드 배열 생성

            BuildRuleBasedRandomMap(); // 규칙 기반 랜덤맵 생성

            PlayerX = StartColumn; // 시작노드 x위치
            PlayerY = CenterRow; // 시작노드 y위치
            Nodes[PlayerX, PlayerY].IsActive = true; // 시작노드 활성화
            Nodes[PlayerX, PlayerY].IsVisible = true; // 시작노드 공개
            Nodes[PlayerX, PlayerY].IsCleared = true; // 시작노드 확보 처리
            Nodes[PlayerX, PlayerY].IsInfected = true; // 최초 감염 지점 표시

            UnlockAdjacentNodesFrom(PlayerX, PlayerY); // 시작지점 주변 개방
        }

        private void BuildRuleBasedRandomMap()
        {
            InitializeEmptyNodes(); // 전체 노드 초기화
            PlaceFixedCoreNodes(); // 시작/게이트/보스 배치

            BuildZone(1, 3, true); // 2~4열 1구역 생성
            BuildZone(5, 7, false); // 6~8열 2구역 생성
            BuildZone(9, 11, false); // 10~12열 3구역 생성

            ValidateZoneHasNoEmpty(1, 3); // 1구역 빈칸 보정
            ValidateZoneHasNoEmpty(5, 7); // 2구역 빈칸 보정
            ValidateZoneHasNoEmpty(9, 11); // 3구역 빈칸 보정
        }

        private void InitializeEmptyNodes()
        {
            for (int x = 0; x < Width; x++) // 전체 열 순회
                for (int y = 0; y < Height; y++) // 전체 행 순회
                    Nodes[x, y] = new GridNode(NodeType.Empty); // 기본 공백 배치
        }

        private void PlaceFixedCoreNodes()
        {
            Nodes[StartColumn, CenterRow] = new GridNode(NodeType.Start); // /Root 중앙 고정
            Nodes[FirstGateColumn, CenterRow] = new GridNode(NodeType.Firewall); // 5열 FW 게이트 고정
            Nodes[SecondGateColumn, CenterRow] = new GridNode(NodeType.Firewall); // 9열 FW 게이트 고정
            Nodes[BossColumn, CenterRow] = new GridNode(NodeType.Boss); // 13열 Kernel 고정
        }

        private void BuildZone(int startX, int endX, bool forceFirstCombat)
        {
            List<GridSlot> slots = CreateZoneSlots(startX, endX); // 구역 9칸 후보
            int securityCount = random.Next(3, 5); // /Sec 3~4개 고정

            if (forceFirstCombat) // 첫 구역 전투 고정
            {
                Nodes[FirstCombatColumn, CenterRow] = new GridNode(NodeType.Security); // 2열 2행 첫 전투 고정
                RemoveSlot(slots, FirstCombatColumn, CenterRow); // 고정칸 후보 제거
                securityCount--; // 고정 전투 포함
            }

            PlaceRandomNodes(slots, NodeType.Security, securityCount); // /Sec 고정 수량 배치
            PlaceRandomNodes(slots, NodeType.Shop, 1); // /Mkt 1개 고정
            PlaceRandomNodes(slots, NodeType.Mutation, 1); // /Lab 1개 고정
            PlaceRandomNodes(slots, NodeType.Event, 1); // /Tmp 최소 1개 보장
            PlaceRandomNodes(slots, NodeType.DataCache, 1); // /Data 최소 1개 보장
            FillRemainingZoneSlots(slots); // 남은 칸 이벤트/보상 채움
        }

        private List<GridSlot> CreateZoneSlots(int startX, int endX)
        {
            List<GridSlot> slots = new List<GridSlot>(); // 구역 후보 목록

            for (int x = startX; x <= endX; x++) // 구역 열 순회
                for (int y = 0; y < Height; y++) // 3행 순회
                    slots.Add(new GridSlot(x, y)); // 후보 등록

            return slots;
        }

        private void RemoveSlot(List<GridSlot> slots, int x, int y)
        {
            for (int i = slots.Count - 1; i >= 0; i--) // 후보 역순 순회
                if (slots[i].X == x && slots[i].Y == y) // 제거 대상 체크
                {
                    slots.RemoveAt(i); // 후보 제거
                    return;
                }
        }

        private void PlaceRandomNodes(List<GridSlot> slots, NodeType type, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (slots.Count <= 0)
                    return;

                int index = random.Next(0, slots.Count); // 랜덤 후보 선택
                GridSlot slot = slots[index]; // 선택 후보 가져오기
                Nodes[slot.X, slot.Y] = new GridNode(type); // 노드 배치

                slots.RemoveAt(index); // 사용 후보 제거
            }
        }

        private void FillRemainingZoneSlots(List<GridSlot> slots)
        {
            while (slots.Count > 0) // 남은 칸 순회
            {
                NodeType type = GetRandomEventOrDataType(); // 이벤트/보상 선택
                PlaceRandomNodes(slots, type, 1); // 남은 칸 배치
            }
        }

        private NodeType GetRandomEventOrDataType()
        {
            if (random.Next(0, 2) == 0) // 이벤트/보상 반반 체크
                return NodeType.Event; // 이벤트 노드

            return NodeType.DataCache; // 보상 노드
        }

        private void ValidateZoneHasNoEmpty(int startX, int endX)
        {
            for (int x = startX; x <= endX; x++) // 구역 열 순회
                for (int y = 0; y < Height; y++) // 3행 순회
                    if (Nodes[x, y].Type == NodeType.Empty) // 미할당 체크
                        Nodes[x, y] = new GridNode(GetRandomEventOrDataType()); // 안전 보정
        }

        public GridNode CurrentNode()
        {
            return Nodes[PlayerX, PlayerY];
        }

        public bool Move(int dx, int dy)
        {
            int nextX = PlayerX + dx; // 목표 x좌표 계산
            int nextY = PlayerY + dy; // 목표 y좌표 계산

            GridNode current = CurrentNode(); // 현재노드 가져오기

            if (nextX < 0 || nextX >= Width || nextY < 0 || nextY >= Height) // 맵 범위 체크
                return false;

            GridNode target = Nodes[nextX, nextY]; // 목표노드 가져오기

            if (target.Type == NodeType.Empty) // 공백 칸 체크
                return false;

            if (!target.IsActive) // 비활성 노드 체크
                return false;

            if (dx == 1 && !current.CanRight) // 오른쪽 신호선 체크
                return false;

            if (dx == -1 && !current.CanLeft) // 왼쪽 신호선 체크
                return false;

            if (dy == -1 && !current.CanUp) // 위쪽 신호선 체크
                return false;

            if (dy == 1 && !current.CanDown) // 아래쪽 신호선 체크
                return false;

            PlayerX = nextX; // 플레이어 x위치 갱신
            PlayerY = nextY; // 플레이어 y위치 갱신

            target.IsVisible = true; // 도달노드 공개

            return true;
        }

        public void ResolveCurrentNode()
        {
            GridNode node = CurrentNode(); // 현재노드

            node.IsCleared = true; // 장악완료

            if (!node.IsReusableNode()) // 일반 노드 감염 체크
                node.IsInfected = true; // 감염 상태 표시

            UnlockAdjacentNodesFrom(PlayerX, PlayerY); // 인접노드 활성화
        }

        private void UnlockAdjacentNodesFrom(int x, int y)
        {
            TryUnlockNeighbor(x, y, 0, -1); // 위쪽 인접노드 개방
            TryUnlockNeighbor(x, y, 0, 1); // 아래쪽 인접노드 개방
            TryUnlockNeighbor(x, y, -1, 0); // 왼쪽 인접노드 개방
            TryUnlockNeighbor(x, y, 1, 0); // 오른쪽 인접노드 개방

            RefreshAllConnections(); // 전체 연결선 갱신
        }

        private void TryUnlockNeighbor(int x, int y, int dx, int dy)
        {
            int targetX = x + dx; // 대상 x좌표 계산
            int targetY = y + dy; // 대상 y좌표 계산

            if (targetX < 0 || targetX >= Width || targetY < 0 || targetY >= Height) // 맵 범위 체크
                return;

            GridNode target = Nodes[targetX, targetY]; // 인접노드 체크

            if (target.Type == NodeType.Empty) // 공백 칸 체크
                return;

            target.IsActive = true; // 인접노드 활성화
            target.IsVisible = true; // 인접노드 공개
        }

        private void RefreshAllConnections()
        {
            for (int y = 0; y < Height; y++) // 행 순회
                for (int x = 0; x < Width; x++) // 열 순회
                    RefreshNodeConnections(x, y); // 각 노드 연결선 갱신
        }

        private void RefreshNodeConnections(int x, int y)
        {
            GridNode node = Nodes[x, y]; // 갱신 대상 노드

            if (node.Type == NodeType.Empty || !node.IsActive)
            {
                node.CanUp = false;
                node.CanDown = false;
                node.CanLeft = false;
                node.CanRight = false;
                return;
            }

            node.CanUp = IsOpenNeighbor(x, y, 0, -1); // 위쪽 연결 체크
            node.CanDown = IsOpenNeighbor(x, y, 0, 1); // 아래쪽 연결 체크
            node.CanLeft = IsOpenNeighbor(x, y, -1, 0); // 왼쪽 연결 체크
            node.CanRight = IsOpenNeighbor(x, y, 1, 0); // 오른쪽 연결 체크
        }

        private bool IsOpenNeighbor(int x, int y, int dx, int dy)
        {
            int targetX = x + dx; // 대상 x좌표 계산
            int targetY = y + dy; // 대상 y좌표 계산

            if (targetX < 0 || targetX >= Width || targetY < 0 || targetY >= Height) // 맵 범위 체크
                return false;

            GridNode target = Nodes[targetX, targetY]; // 인접노드 체크

            return target.Type != NodeType.Empty && target.IsActive; // 이동 가능 노드 체크
        }

        public void RevealForwardFromClearedColumn(int forwardColumns)
        {
            int farthestClearedX = GetFarthestClearedColumn(); // 가장 멀리 장악한 열
            int revealEndX = Math.Min(Width - 1, farthestClearedX + Math.Max(0, forwardColumns)); // 공개 끝 열

            for (int x = 0; x <= revealEndX; x++) // 공개 대상 열 순회
            {
                for (int y = 0; y < Height; y++) // 행 순회
                {
                    if (Nodes[x, y].Type == NodeType.Empty) // 공백 칸 제외
                        continue;

                    Nodes[x, y].IsVisible = true; // 정보만 공개
                }
            }
        }

        private int GetFarthestClearedColumn()
        {
            int farthestX = StartColumn; // 최초 장악 열 기준

            for (int x = 0; x < Width; x++) // 전체 열 순회
                for (int y = 0; y < Height; y++) // 행 순회
                    if (Nodes[x, y].IsCleared && x > farthestX) // 장악한 최전방 열 체크
                        farthestX = x; 

            return farthestX;
        }

    }
}
