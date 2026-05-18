using System;
using System.Collections.Generic;
using VirusExe.SystemBreach.Characters;
using VirusExe.SystemBreach.Core;
using VirusExe.SystemBreach.DataGrid;

namespace VirusExe.SystemBreach.Rendering
{
    // SIGNAL GRID 화면 출력
    // 현재 위치 기준 4열 표시, 노드 색상, 상태 패널, Footer 출력 관리
    public partial class ConsoleRenderer
    {
        private const int GridVisibleColumns = 4; // 화면에 보이는 GRID 열 수
        private const int GridCellInnerWidth = 13; // 노드 내부 폭
        private const int GridCellOuterWidth = 15; // 노드 전체 폭
        private const int GridColumnGap = 7; // 노드 사이 간격
        private const int NodePanelLeftWidth = 44; // 노드 상세 좌측 폭
        private const int NodePanelRightWidth = 58; // 노드 상세 우측 폭
        private static readonly Random GridInfectionRandom = new Random(); // 감염 노이즈 랜덤 생성기

        public void RenderGrid(SignalGrid grid, Player player, int systemInfection)
        {
            ResetRenderCursor(); // 커서 초기화
            WriteGridHeader(systemInfection); // TRACE 포함 GRID 헤더 출력

            int startX = GetGridCameraStart(grid); // 현재 위치 기준 카메라 시작열
            int endX = Math.Min(grid.Width, startX + GridVisibleColumns); // 화면 표시 끝열

            RenderGridCameraLine(grid, startX, endX); // SCAN RANGE는 HEADER 바로 아래 고정
            WriteEmptyLine(); 
            WriteEmptyLine(); 
            RenderNodeRows(grid, player, startX, endX); // 노드 카드 3행 출력
            WriteEmptyLine(); 
            WriteEmptyLine();
            WriteEmptyLine();
            WriteSeparator();
            RenderCurrentNodePanel(grid, player, systemInfection); // 노드/플레이어 정보 패널 출력
            WriteSeparator();
            WriteControlLine(); // GRID 키 안내 1줄 출력
            WriteFooter();
            ClearRenderTail(); // 잔여 줄 제거
        }

        private void WriteGridHeader(int systemInfection)
        {
            string left = " TARGET SYSTEM       // SIGNAL GRID"; // 좌측 헤더
            string right = "TRACE : " + systemInfection + "%    "; // 우측 여백 포함 TRACE
            int remain = InnerWidth - TextUtil.GetDisplayWidth(left) - TextUtil.GetDisplayWidth(right); // 중간 여백 계산

            if (remain < 1) remain = 1; // 최소 여백 보정

            SetColor(ConsoleColor.Cyan);
            Console.Write("╔" + new string('═', InnerWidth) + "╗");
            ClearPhysicalLineRemainder(); // 오른쪽 잔상 제거
            Console.WriteLine();

            Console.Write("║");
            WriteColored(left, ConsoleColor.Cyan);
            Console.Write(new string(' ', remain));
            WriteColored(right, ConsoleColor.Green);
            SetColor(ConsoleColor.Cyan);
            Console.Write("║");
            Console.ResetColor();
            ClearPhysicalLineRemainder(); // 오른쪽 잔상 제거
            Console.WriteLine();

            SetColor(ConsoleColor.Cyan);
            Console.Write("╠" + new string('═', InnerWidth) + "╣");
            ClearPhysicalLineRemainder(); // 오른쪽 잔상 제거
            Console.WriteLine();
            Console.ResetColor();
        }

        private void RenderNodeRows(SignalGrid grid, Player player, int startX, int endX)
        {
            for (int y = 0; y < grid.Height; y++) // 노드 행 출력
            {
                List<ColorSegment> headerLine = new List<ColorSegment>(); // 노드 경로 줄
                List<ColorSegment> typeLine = new List<ColorSegment>(); // 노드 타입 줄
                List<ColorSegment> currentLine = new List<ColorSegment>(); // 현재 위치 줄
                List<ColorSegment> stateLine = new List<ColorSegment>(); // 노드 상태 줄
                List<ColorSegment> frameLine = new List<ColorSegment>(); // 하단 프레임 줄
                List<ColorSegment> verticalLine = new List<ColorSegment>(); // 세로 연결선 줄

                for (int x = startX; x < endX; x++) // 표시 열 순회
                {
                    GridNode node = grid.Nodes[x, y]; // 현재 칸 노드
                    bool renderNode = ShouldRenderGridNode(node); // 노드 출력 여부
                    bool isCurrent = grid.PlayerX == x && grid.PlayerY == y; // 현재 위치 체크

                    if (renderNode) // 출력 노드 체크
                    {
                        AddNodeHeaderLine(headerLine, node, player, isCurrent); // 노드 경로 출력
                        AddNodeTypeLine(typeLine, node, player, isCurrent); // 노드 타입 출력
                        AddNodeCurrentLine(currentLine, node, player, isCurrent); // 현재 위치 출력
                        AddNodeStateLine(stateLine, node, player, isCurrent); // 노드 상태 출력
                        AddNodeFrameLine(frameLine, node, player, isCurrent); // 하단 프레임 출력
                        AddNodeVerticalLine(verticalLine, node, y, grid.Height); // 아래 연결선 출력
                    }
                    else
                    {
                        AddBlankNode(headerLine, typeLine, currentLine, stateLine, frameLine, verticalLine); // 빈칸 출력
                    }

                    if (x < endX - 1) // 표시 범위 마지막 열 체크
                    {
                        bool canConnect = renderNode && node.CanRight; // 오른쪽 연결 체크
                        ConsoleColor lineColor = canConnect ? GetNodeBorderColor(node, false) : ConsoleColor.DarkGray;

                        headerLine.Add(new ColorSegment(new string(' ', GridColumnGap), ConsoleColor.DarkGray));
                        typeLine.Add(new ColorSegment(new string(' ', GridColumnGap), ConsoleColor.DarkGray));
                        currentLine.Add(new ColorSegment(canConnect ? new string('-', GridColumnGap) : new string(' ', GridColumnGap), lineColor));
                        stateLine.Add(new ColorSegment(new string(' ', GridColumnGap), ConsoleColor.DarkGray));
                        frameLine.Add(new ColorSegment(new string(' ', GridColumnGap), ConsoleColor.DarkGray));
                        verticalLine.Add(new ColorSegment(new string(' ', GridColumnGap), ConsoleColor.DarkGray));
                    }
                }

                WriteCenteredSegments(headerLine);
                WriteCenteredSegments(typeLine);
                WriteCenteredSegments(currentLine);
                WriteCenteredSegments(stateLine);
                WriteCenteredSegments(frameLine);

                if (y < grid.Height - 1) // 마지막 행 체크
                {
                    WriteCenteredSegments(verticalLine);
                    WriteCenteredSegments(verticalLine);
                }
            }
        }

        private bool ShouldRenderGridNode(GridNode node)
        {
            if (node.Type != NodeType.Empty) return true; // 실제 노드 출력
            return node.IsVisible || node.IsActive || node.IsCleared; // 공개된 빈 노드 출력
        }

        private void AddBlankNode(List<ColorSegment> headerLine, List<ColorSegment> typeLine, List<ColorSegment> currentLine, List<ColorSegment> stateLine, List<ColorSegment> frameLine, List<ColorSegment> verticalLine)
        {
            string blank = new string(' ', GridCellOuterWidth); // 빈칸 폭
            headerLine.Add(new ColorSegment(blank, ConsoleColor.DarkGray));
            typeLine.Add(new ColorSegment(blank, ConsoleColor.DarkGray));
            currentLine.Add(new ColorSegment(blank, ConsoleColor.DarkGray));
            stateLine.Add(new ColorSegment(blank, ConsoleColor.DarkGray));
            frameLine.Add(new ColorSegment(blank, ConsoleColor.DarkGray));
            verticalLine.Add(new ColorSegment(blank, ConsoleColor.DarkGray));
        }

        private void AddNodeHeaderLine(List<ColorSegment> line, GridNode node, Player player, bool isCurrent)
        {
            string path = GetNodePathText(node, player); // 노드 경로명
            string label = path == "---" ? "[---]" : "[ " + path + " ]"; // 미체크 노드는 압축 표시
            int fill = GridCellOuterWidth - 2 - TextUtil.GetDisplayWidth(label); // 남은 상단선 길이
            ConsoleColor borderColor = GetNodeBorderColor(node, isCurrent); // 테두리 색상
            ConsoleColor pathColor = GetNodePathColor(node, player, isCurrent); // 경로명 색상

            if (fill < 0) // 경로명이 초과된 경우
            {
                label = TextUtil.Fit(label, GridCellOuterWidth - 2); // 라벨 폭 보정
                fill = 0; // 남은 길이 제거
            }

            line.Add(new ColorSegment(".", borderColor));
            line.Add(new ColorSegment(label, pathColor));
            line.Add(new ColorSegment(new string('-', fill), borderColor));
            line.Add(new ColorSegment(".", borderColor));
        }

        private void AddNodeTypeLine(List<ColorSegment> line, GridNode node, Player player, bool isCurrent)
        {
            if (IsInfectedDisplayNode(node)) // 감염 노드는 타입 제거 후 노이즈 출력
            {
                AddInfectedNoiseCell(line, null); // 감염 노이즈 출력
                return;
            }

            string type = GetNodeMiddleText(node, player); // 서브이름 출력
            ConsoleColor borderColor = GetNodeBorderColor(node, isCurrent); // 프레임 색상
            ConsoleColor typeColor = GetNodeMiddleColor(node, player); // 서브이름 색상

            AddNodeCell(line, type, typeColor, null, borderColor); // 서브이름 셀 출력
        }

        private void AddNodeCurrentLine(List<ColorSegment> line, GridNode node, Player player, bool isCurrent)
        {
            if (IsInfectedDisplayNode(node)) // 감염 노드는 가운데 줄도 노이즈 처리
            {
                AddInfectedNoiseCell(line, isCurrent ? "[YOU]" : null); // 감염 노드 현재 위치 출력
                return;
            }

            string text = isCurrent ? "[YOU]" : string.Empty; // 현재 위치 표시
            ConsoleColor borderColor = GetNodeBorderColor(node, isCurrent); // 프레임 색상
            ConsoleColor textColor = isCurrent ? ConsoleColor.White : ConsoleColor.DarkGray; // 현재 위치 색상
            ConsoleColor? background = isCurrent ? (ConsoleColor?)ConsoleColor.DarkMagenta : null; // 현재 위치 배경

            AddNodeCell(line, text, textColor, background, borderColor); // 현재 위치 셀 출력
        }

        private void AddNodeStateLine(List<ColorSegment> line, GridNode node, Player player, bool isCurrent)
        {
            if (IsInfectedDisplayNode(node)) // 감염 노드는 [INFECT]로 통일
            {
                AddInfectedNoiseCell(line, "[INFECT]"); // 감염 상태 출력
                return;
            }

            string state = GetNodeStateText(node, player); // 상태 텍스트
            ConsoleColor borderColor = GetNodeBorderColor(node, isCurrent); // 프레임 색상
            ConsoleColor stateColor = GetNodeStateColor(state); // 상태 색상

            AddNodeCell(line, state, stateColor, null, borderColor); // 상태 셀 출력
        }

        private void AddNodeFrameLine(List<ColorSegment> line, GridNode node, Player player, bool isCurrent)
        {
            ConsoleColor borderColor = GetNodeBorderColor(node, isCurrent); // 테두리 색상

            line.Add(new ColorSegment("'" + new string('-', GridCellInnerWidth) + "'", borderColor));
        }

        private void AddNodeVerticalLine(List<ColorSegment> line, GridNode node, int y, int height)
        {
            string vertical = node.CanDown && y < height - 1 ? "       |       " : new string(' ', GridCellOuterWidth); // 아래 연결선
            ConsoleColor color = node.CanDown && y < height - 1 ? GetNodeBorderColor(node, false) : ConsoleColor.DarkGray;

            line.Add(new ColorSegment(vertical, color));
        }

        private void AddNodeCell(List<ColorSegment> line, string text, ConsoleColor textColor, ConsoleColor? background, ConsoleColor borderColor)
        {
            string centered = CenterCell(text, GridCellInnerWidth); // 중앙 정렬
            int start = string.IsNullOrEmpty(text) ? -1 : centered.IndexOf(text, StringComparison.Ordinal); // 텍스트 위치

            line.Add(new ColorSegment("|", borderColor));

            if (start < 0) // 표시 텍스트 없음
            {
                line.Add(new ColorSegment(centered, ConsoleColor.DarkGray));
            }
            else
            {
                string left = centered.Substring(0, start); // 왼쪽 여백
                string middle = text; // 실제 텍스트
                string right = centered.Substring(start + text.Length); // 오른쪽 여백

                if (left.Length > 0) line.Add(new ColorSegment(left, ConsoleColor.DarkGray));
                if (background.HasValue) line.Add(new ColorSegment(middle, textColor, background.Value));
                else line.Add(new ColorSegment(middle, textColor));
                if (right.Length > 0) line.Add(new ColorSegment(right, ConsoleColor.DarkGray));
            }

            line.Add(new ColorSegment("|", borderColor));
        }

        private void AddInfectedNoiseCell(List<ColorSegment> line, string marker)
        {
            string fitted = BuildInfectedNoiseLine(GridCellInnerWidth, marker); // 감염 노이즈 생성

            line.Add(new ColorSegment("|", ConsoleColor.DarkRed)); // 감염 노드 좌측 프레임

            if (!string.IsNullOrEmpty(marker)) // 강조 마커 체크
            {
                int markerIndex = fitted.IndexOf(marker, StringComparison.Ordinal); // 마커 위치 탐색

                if (markerIndex >= 0) // 마커 포함 체크
                {
                    string left = fitted.Substring(0, markerIndex); // 마커 왼쪽 노이즈
                    string right = fitted.Substring(markerIndex + marker.Length); // 마커 오른쪽 노이즈

                    if (left.Length > 0) line.Add(new ColorSegment(left, ConsoleColor.DarkRed)); // 왼쪽 노이즈 출력

                    if (marker == "[YOU]") line.Add(new ColorSegment(marker, ConsoleColor.White, ConsoleColor.DarkMagenta)); // 현재 위치 강조
                    else line.Add(new ColorSegment(marker, ConsoleColor.Red)); // 감염 상태 강조

                    if (right.Length > 0) line.Add(new ColorSegment(right, ConsoleColor.DarkRed)); // 오른쪽 노이즈 출력
                }
                else
                {
                    line.Add(new ColorSegment(fitted, ConsoleColor.DarkRed)); // 예외 시 전체 노이즈 출력
                }
            }
            else
            {
                line.Add(new ColorSegment(fitted, ConsoleColor.DarkRed)); // 일반 감염 노이즈 출력
            }

            line.Add(new ColorSegment("|", ConsoleColor.DarkRed)); // 감염 노드 우측 프레임
        }

        private string BuildInfectedNoiseLine(int width, string marker)
        {
            if (string.IsNullOrEmpty(marker)) // 마커 없는 줄 체크
            {
                return BuildRandomInfectionChunk(width); // 전체 노이즈 생성
            }

            int markerWidth = TextUtil.GetDisplayWidth(marker); // 마커 폭 계산

            if (markerWidth >= width) // 마커가 셀보다 긴 경우
            {
                return TextUtil.Fit(marker, width); // 폭 보정
            }

            int remain = width - markerWidth; // 남은 폭 계산
            int leftWidth = remain / 2; // 왼쪽 노이즈 폭
            int rightWidth = remain - leftWidth; // 오른쪽 노이즈 폭

            return BuildRandomInfectionChunk(leftWidth) + marker + BuildRandomInfectionChunk(rightWidth); // 노이즈 + 마커 + 노이즈
        }

        private string BuildRandomInfectionChunk(int length)
        {
            char[] chars = new char[] { '#', '$', '%', '0', '1' }; // 감염 노이즈 문자 풀
            char[] result = new char[length]; // 결과 버퍼

            for (int i = 0; i < length; i++) // 필요한 길이만큼 반복
            {
                result[i] = chars[GridInfectionRandom.Next(chars.Length)]; // 랜덤 문자 선택
            }

            return new string(result); // 랜덤 노이즈
        }

        private bool IsInfectedDisplayNode(GridNode node)
        {
            if (node.Type == NodeType.Empty) return false; // 빈 노드는 감염 표시 제외
            return node.IsInfected || node.Type == NodeType.Start; // 시작/클리어 노드 감염 처리
        }

        private bool IsLockedDisplayNode(GridNode node)
        {
            if (node.Type == NodeType.Empty && !node.IsVisible) return false; // 숨김 빈칸
            if (!node.IsActive && node.Type != NodeType.Empty) return true; // 비활성 노드
            if (!node.IsVisible && node.Type != NodeType.Empty) return true; // 미공개 노드
            return false; // 접근 가능
        }

        private int GetGridCameraStart(SignalGrid grid)
        {
            int maxStart = Math.Max(0, grid.Width - GridVisibleColumns); // 최대 카메라 시작열
            int start = grid.PlayerX - 1; // 현재 노드 왼쪽 1열 확보

            if (start < 0) start = 0; // 좌측 범위 보정
            if (start > maxStart) start = maxStart; // 우측 범위 보정

            return start;
        }

        private void RenderGridCameraLine(SignalGrid grid, int startX, int endX)
        {
            List<ColorSegment> segments = new List<ColorSegment>();

            segments.Add(new ColorSegment(" SCAN RANGE :: [", ConsoleColor.DarkGray));

            for (int i = 0; i < grid.Width; i++) // 전체 GRID 범위 출력
            {
                if (i >= startX && i < endX) // 현재 화면에 표시 중인 열 체크
                {
                    segments.Add(new ColorSegment("+", ConsoleColor.Cyan));
                }
                else if (i < startX) // 지나간 열 체크
                {
                    segments.Add(new ColorSegment("-", ConsoleColor.DarkGray));
                }
                else // 아직 화면 밖 오른쪽 열 체크
                {
                    segments.Add(new ColorSegment("-", ConsoleColor.Gray));
                }
            }

            segments.Add(new ColorSegment("]", ConsoleColor.DarkGray));

            WriteSegmentsLine(segments);
            WriteEmptyLine();
        }

        private string GetNodePathText(GridNode node, Player player)
        {
            if (node.Type == NodeType.Empty && !node.IsVisible) return string.Empty; // 숨김 빈칸
            if (!node.IsVisible && node.Type != NodeType.Empty) return "---"; // 미공개 노드

            if (node.Type == NodeType.Start) return "/Root";
            if (node.Type == NodeType.Security) return "/Sec";
            if (node.Type == NodeType.Firewall) return "/Fw";
            if (node.Type == NodeType.Shop) return "/Mkt";
            if (node.Type == NodeType.Mutation) return "/Lab";
            if (node.Type == NodeType.Event) return "/Tmp";
            if (node.Type == NodeType.DataCache) return "/Data";
            if (node.Type == NodeType.Boss) return "Kernel";
            return "/Null";
        }

        private string GetNodeMiddleText(GridNode node, Player player)
        {
            if (node.Type == NodeType.Empty && !node.IsVisible) return string.Empty; // 숨김 빈칸
            if (!node.IsVisible && node.Type != NodeType.Empty) return "LOCK"; // 미공개 노드

            if (node.Type == NodeType.Start) return "ROOT";
            if (node.Type == NodeType.Security) return "COMBAT";
            if (node.Type == NodeType.Firewall) return "GATE";
            if (node.Type == NodeType.Shop) return "SHOP";
            if (node.Type == NodeType.Mutation) return "PAYLOAD";
            if (node.Type == NodeType.Event) return "EVENT";
            if (node.Type == NodeType.DataCache) return "CACHE";
            if (node.Type == NodeType.Boss) return "CORE";
            return "EMPTY";
        }

        private string GetNodeStateText(GridNode node, Player player)
        {
            if (node.Type == NodeType.Empty && !node.IsVisible) return string.Empty; // 숨김 빈칸

            if (IsInfectedDisplayNode(node)) return "INFECT"; // 감염 노드

            if (!node.IsActive && node.Type != NodeType.Empty) return "DENIED"; // 비활성 노드
            if (!node.IsVisible && node.Type != NodeType.Empty) return "DENIED"; // 미공개 노드

            if (node.Type == NodeType.Empty) return "VOID"; // 빈 노드

            if (node.Type == NodeType.Boss && player.AccessLevel < GameConfig.BossRequiredAccess)
            {
                return "DENIED"; // Kernel 권한 부족
            }

            return "ONLINE"; // 진입 가능 노드 통일
        }

        private ConsoleColor GetNodePathColor(GridNode node, Player player, bool isCurrent)
        {
            if (IsInfectedDisplayNode(node)) return ConsoleColor.Red; // 감염 노드명
            if (IsLockedDisplayNode(node)) return ConsoleColor.DarkGray; // DENIED 노드명

            if (node.Type == NodeType.Start) return ConsoleColor.Red; // 감염 시작점
            if (node.Type == NodeType.Security) return ConsoleColor.Cyan; // 일반 전투
            if (node.Type == NodeType.Firewall) return ConsoleColor.Blue; // 엘리트 전투
            if (node.Type == NodeType.Shop) return ConsoleColor.Green; // 상점
            if (node.Type == NodeType.Mutation) return ConsoleColor.Green; // 업그레이드
            if (node.Type == NodeType.Event) return ConsoleColor.Yellow; // 이벤트
            if (node.Type == NodeType.DataCache) return ConsoleColor.Yellow; // 보상
            if (node.Type == NodeType.Boss) return ConsoleColor.White; // Kernel

            return ConsoleColor.DarkGray; // 기타
        }

        private ConsoleColor GetNodeMiddleColor(GridNode node, Player player)
        {
            if (IsInfectedDisplayNode(node)) return ConsoleColor.Red; // 감염 내부
            if (IsLockedDisplayNode(node)) return ConsoleColor.DarkGray; // DENIED 내부

            if (node.Type == NodeType.Start) return ConsoleColor.Red; // Root
            if (node.Type == NodeType.Security) return ConsoleColor.Cyan; // 일반 전투
            if (node.Type == NodeType.Firewall) return ConsoleColor.Blue; // 엘리트 전투
            if (node.Type == NodeType.Shop) return ConsoleColor.Green; // 상점
            if (node.Type == NodeType.Mutation) return ConsoleColor.Green; // 업그레이드
            if (node.Type == NodeType.Event) return ConsoleColor.Yellow; // 이벤트
            if (node.Type == NodeType.DataCache) return ConsoleColor.Yellow; // 보상
            if (node.Type == NodeType.Boss) return ConsoleColor.White; // Kernel

            return ConsoleColor.DarkGray; // 기타
        }

        private ConsoleColor GetNodeStateColor(string state)
        {
            if (state == "INFECT") return ConsoleColor.Red; // 감염
            if (state == "ONLINE") return ConsoleColor.Green; // 진입 가능 공통
            if (state == "DENIED") return ConsoleColor.DarkGray; // 접근 불가
            if (state == "VOID") return ConsoleColor.DarkGray; // 빈 영역

            return ConsoleColor.Gray; // 기본 상태
        }

        private ConsoleColor GetNodeBorderColor(GridNode node, bool isCurrent)
        {
            if (IsInfectedDisplayNode(node)) return ConsoleColor.DarkRed; // 감염 프레임
            if (IsLockedDisplayNode(node)) return ConsoleColor.DarkGray; // DENIED 프레임

            if (node.Type == NodeType.Security) return ConsoleColor.DarkCyan; // 전투 계열
            if (node.Type == NodeType.Firewall) return ConsoleColor.DarkCyan; // 전투 계열
            if (node.Type == NodeType.Boss) return ConsoleColor.DarkCyan; // 전투 계열
            if (node.Type == NodeType.Shop) return ConsoleColor.DarkGreen; // 상점/업그레이드 계열
            if (node.Type == NodeType.Mutation) return ConsoleColor.DarkGreen; // 상점/업그레이드 계열
            if (node.Type == NodeType.Event) return ConsoleColor.DarkYellow; // 이벤트/보상 계열
            if (node.Type == NodeType.DataCache) return ConsoleColor.DarkYellow; // 이벤트/보상 계열
            if (node.Type == NodeType.Start) return ConsoleColor.DarkRed; // 시작 루트

            return ConsoleColor.DarkGray; // 기타
        }

        private string CenterCell(string text, int width)
        {
            int textWidth = TextUtil.GetDisplayWidth(text);
            if (textWidth >= width) return TextUtil.Fit(text, width); // 셀 폭 초과 체크

            int left = (width - textWidth) / 2;
            int right = width - textWidth - left;
            return new string(' ', left) + text + new string(' ', right);
        }

        private void RenderCurrentNodePanel(SignalGrid grid, Player player, int systemInfection)
        {
            GridNode node = grid.CurrentNode(); // 현재 노드 가져오기
            string path = GetPanelPath(node, player); // 노드명 표시
            string type = GetPanelType(node); // 타입 표시
            string action = GetPanelAction(node, player); // 행동 표시
            string risk = GetNodeRisk(node); // 위험도 표시
            string trace = GetPanelTrace(node); // TRACE 표시
            string reward = GetPanelReward(node); // 보상 표시
            string state = GetNodeStateText(node, player); // 상태 표시
            string desc = GetPanelDescription(node, player); // 설명 표시

            WriteTargetNodeTitleLine(path, player, systemInfection, node); // 대상 노드 제목줄
            WriteGridThinSeparator(); // 노드 정보 구분선
            WritePanelPairLine("TYPE", type, GetPanelTypeColor(node), "TRACE", trace, ConsoleColor.Yellow);
            WritePanelPairLine("ACTION", action, GetPanelActionColor(action), "RISK", risk, GetRiskColor(risk));
            WritePanelPairLine("REWARD", reward, GetPanelRewardColor(reward), "STATE", state, GetNodeStateColor(state));
            WritePanelDescription(desc);
            WriteSeparator();
            WritePlayerInfoPanel(player); // VIRUS.EXE 정보 출력
        }

        private void WriteTargetNodeTitleLine(string path, Player player, int systemInfection, GridNode node)
        {
            string leftPrefix = " TARGET NODE : "; // 좌측 라벨
            string accessPrefix = "ACCESS : "; // ACCESS 라벨
            string tracePrefix = "TRACE : "; // TRACE 라벨
            string accessText = player.AccessLevel + " / " + GameConfig.BossRequiredAccess; // ACCESS 값
            string traceBar = MakeBar(systemInfection, GameConfig.MaxSystemInfection, 10); // TRACE 게이지
            string traceValue = systemInfection + " / " + GameConfig.MaxSystemInfection + "%"; // TRACE 수치
            int pathWidth = Math.Max(1, StatusRightColumnStart - TextUtil.GetDisplayWidth(leftPrefix) - 1); // ACCESS 전 노드명 폭
            int accessValueWidth = Math.Max(1, StatusTraceColumnStart - StatusRightColumnStart - TextUtil.GetDisplayWidth(accessPrefix) - 1); // ACCESS 값 폭
            int traceValueWidth = Math.Max(1, InnerWidth - StatusRightPadding - StatusTraceColumnStart - TextUtil.GetDisplayWidth(tracePrefix) - TextUtil.GetDisplayWidth(traceBar) - 1); // TRACE 값 폭
            ConsoleColor traceColor = GetPercentColor(systemInfection, false); // TRACE 색상
            List<ColorSegment> segments = new List<ColorSegment>(); // 고정 열 세그먼트

            segments.Add(new ColorSegment(leftPrefix, ConsoleColor.DarkGray));
            segments.Add(new ColorSegment(TextUtil.Fit(path, pathWidth), GetNodePathColor(node, player, true)));
            PadSegmentsToColumn(segments, StatusRightColumnStart); // LEVEL / ACCESS 열 정렬
            segments.Add(new ColorSegment(accessPrefix, ConsoleColor.DarkGray));
            segments.Add(new ColorSegment(TextUtil.Fit(accessText, accessValueWidth), ConsoleColor.Cyan));
            PadSegmentsToColumn(segments, StatusTraceColumnStart); // EXP / TRACE 열 정렬
            segments.Add(new ColorSegment(tracePrefix, ConsoleColor.DarkGray));
            segments.Add(new ColorSegment(traceBar, traceColor));
            segments.Add(new ColorSegment(" " + TextUtil.Fit(traceValue, traceValueWidth), traceColor));

            WriteSegmentsLine(segments);
        }

        private void WritePlayerInfoPanel(Player player)
        {
            WritePlayerHudStatus(player, false); // GRID/전투 공통 VIRUS.EXE 상태 패널
        }

        private string GetEquippedItemDisplayName(VirusExe.SystemBreach.Systems.ItemData item)
        {
            if (item == null) return "-"; // 장착 없음
            return item.DisplayName; // 태그 포함 아이템명
        }

        private string GetPayloadDisplayName(Player player)
        {
            if (player.Mutation == VirusMutation.Ransomware) return "RANSOMWARE.EXE"; // 랜섬웨어 변이
            if (player.Mutation == VirusMutation.Trojan) return "TROJAN.EXE"; // 트로젠 변이
            if (player.Mutation == VirusMutation.Adware) return "ADWARE.EXE"; // 애드웨어 변이
            return "VIRUS.EXE"; // 기본 페이로드
        }

        private void WriteGridThinSeparator()
        {
            SetColor(ConsoleColor.Cyan);
            Console.Write(ApplyBattleGlitch("╟" + new string('─', InnerWidth) + "╢"));
            ClearPhysicalLineRemainder(); // 오른쪽 잔상 제거
            Console.WriteLine();
            Console.ResetColor();
        }

        private void WritePanelPairLine(string leftLabel, string leftValue, ConsoleColor leftColor, string rightLabel, string rightValue, ConsoleColor rightColor)
        {
            string leftPrefix = " " + TextUtil.Fit(leftLabel, 7) + " : "; // 좌측 라벨
            string rightPrefix = TextUtil.Fit(rightLabel, 7) + " : "; // 우측 라벨

            int leftValueWidth = Math.Max(1, StatusRightColumnStart - TextUtil.GetDisplayWidth(leftPrefix) - 2); // 우측 열 전까지 값 폭
            int rightValueWidth = Math.Max(1, InnerWidth - StatusRightPadding - StatusRightColumnStart - TextUtil.GetDisplayWidth(rightPrefix)); // 우측 값 폭

            List<ColorSegment> segments = new List<ColorSegment>(); // 고정 열 세그먼트

            segments.Add(new ColorSegment(leftPrefix, ConsoleColor.DarkGray));
            segments.Add(new ColorSegment(TextUtil.Fit(leftValue, leftValueWidth), leftColor));

            PadSegmentsToColumn(segments, StatusRightColumnStart); // 오른쪽 라벨 시작 열 고정

            segments.Add(new ColorSegment(rightPrefix, ConsoleColor.DarkGray));
            segments.Add(new ColorSegment(TextUtil.Fit(rightValue, rightValueWidth), rightColor));

            WriteSegmentsLine(segments);
        }

        private void WritePanelDescription(string desc)
        {
            WriteSegmentsLine(
                new ColorSegment(" ", ConsoleColor.DarkGray),
                new ColorSegment(TextUtil.Fit("DESC", 7), ConsoleColor.DarkGray),
                new ColorSegment(" : ", ConsoleColor.DarkGray),
                new ColorSegment(TextUtil.Fit(desc, InnerWidth - 12), ConsoleColor.White));
        }

        private string GetPanelPath(GridNode node, Player player)
        {
            return GetNodePathText(node, player); // 현재 경로
        }

        private string GetPanelType(GridNode node)
        {
            if (node.Type == NodeType.Start) return "INITIAL BREACH";
            if (node.Type == NodeType.Security) return "SECURITY PROCESS";
            if (node.Type == NodeType.Firewall) return "FIREWALL GATEWAY";
            if (node.Type == NodeType.Shop) return "EXPLOIT MARKET";
            if (node.Type == NodeType.Mutation) return "PAYLOAD LAB";
            if (node.Type == NodeType.Event) return "TEMP CACHE";
            if (node.Type == NodeType.DataCache) return "DATA CACHE";
            if (node.Type == NodeType.Boss) return "KERNEL CORE";
            if (node.Type == NodeType.Empty) return "EMPTY SIGNAL";
            return "UNKNOWN";
        }

        private string GetPanelAction(GridNode node, Player player)
        {
            if (node.Type == NodeType.Start) return "START";
            if (node.Type == NodeType.Security) return "COMBAT";
            if (node.Type == NodeType.Firewall) return "COMBAT";
            if (node.Type == NodeType.Shop) return "SHOP";
            if (node.Type == NodeType.Mutation) return "UPGRADE";
            if (node.Type == NodeType.Event) return "EVENT";
            if (node.Type == NodeType.DataCache) return "LOOT";
            if (node.Type == NodeType.Boss && player.AccessLevel < GameConfig.BossRequiredAccess) return "DENIED";
            if (node.Type == NodeType.Boss) return "FINAL COMBAT";
            return "NONE";
        }

        private string GetPanelReward(GridNode node)
        {
            if (node.Type == NodeType.Security) return "KB / ITEM DROP";
            if (node.Type == NodeType.Firewall) return "KB / HIGH ITEM DROP";
            if (node.Type == NodeType.Shop) return "BUY / SELL";
            if (node.Type == NodeType.Mutation) return "MATERIAL UPGRADE";
            if (node.Type == NodeType.Event) return "RANDOM";
            if (node.Type == NodeType.DataCache) return "RECOVERY / ITEM";
            if (node.Type == NodeType.Boss) return "ENDING";
            return "NONE";
        }

        private string GetPanelTrace(GridNode node)
        {
            int traceValue = GameConfig.GetTraceIncreaseByNodeType(node.Type); // 노드별 TRACE 증가량
            if (node.Type == NodeType.Boss) return "+" + traceValue + "%"; // Kernel TRACE 표시
            return "+" + traceValue + "%"; // 실제 증가량 표시
        }

        private string GetPanelDescription(GridNode node, Player player)
        {
            if (node.Type == NodeType.Start) return "VIRUS.EXE가 최초로 침투한 루트 디렉토리입니다.";
            if (node.Type == NodeType.Security) return "보안 프로세스가 감시 중인 디렉토리입니다. 진입 시 전투가 발생합니다.";
            if (node.Type == NodeType.Firewall) return "침투 경로를 차단하는 방화벽 게이트입니다. 일반 전투보다 강한 전투가 발생합니다.";
            if (node.Type == NodeType.Shop) return "침투 도구를 거래하는 암시장 노드입니다. 아이템 구매와 데이터 판매가 가능합니다.";
            if (node.Type == NodeType.Mutation) return "페이로드 구조를 조정하는 실험실입니다. 강화 재료를 사용해 능력치를 올릴 수 있습니다.";
            if (node.Type == NodeType.Event) return "불안정한 임시 캐시 영역입니다. 예측할 수 없는 랜덤 이벤트가 발생합니다.";
            if (node.Type == NodeType.DataCache) return "탈취 가능한 데이터 캐시입니다. 회복 효과 또는 아이템 보상을 획득할 수 있습니다.";
            if (node.Type == NodeType.Boss && player.AccessLevel < GameConfig.BossRequiredAccess) return "대상 시스템의 핵심 실행 영역입니다. ACCESS LEVEL " + GameConfig.BossRequiredAccess + " 이상에서 진입할 수 있습니다.";
            if (node.Type == NodeType.Boss) return "Kernel 실행 권한이 열렸습니다. 최종 KERNEL_CORE 침투 전투를 시작할 수 있습니다.";
            if (node.Type == NodeType.Empty) return "유효한 신호가 감지되지 않는 빈 시스템 영역입니다.";
            return "아직 스캔되지 않은 미확인 디렉토리입니다.";
        }

        private ConsoleColor GetPanelTypeColor(GridNode node)
        {
            if (IsInfectedDisplayNode(node)) return ConsoleColor.Red; // 감염 노드
            if (IsLockedDisplayNode(node)) return ConsoleColor.DarkGray; // DENIED 노드

            if (node.Type == NodeType.Security) return ConsoleColor.Cyan; // 일반 전투
            if (node.Type == NodeType.Firewall) return ConsoleColor.Blue; // 엘리트 전투
            if (node.Type == NodeType.Boss) return ConsoleColor.White; // Kernel
            if (node.Type == NodeType.Shop) return ConsoleColor.Green; // 상점
            if (node.Type == NodeType.Mutation) return ConsoleColor.Green; // 업그레이드
            if (node.Type == NodeType.Event) return ConsoleColor.Yellow; // 이벤트
            if (node.Type == NodeType.DataCache) return ConsoleColor.Yellow; // 보상
            if (node.Type == NodeType.Start) return ConsoleColor.Red; // Root

            return ConsoleColor.DarkGray; // 기타
        }

        private ConsoleColor GetPanelActionColor(string action)
        {
            if (action == "COMBAT") return ConsoleColor.Cyan; // 전투
            if (action == "FINAL COMBAT") return ConsoleColor.White; // 최종 전투
            if (action == "SHOP") return ConsoleColor.Green; // 상점
            if (action == "UPGRADE") return ConsoleColor.Green; // 업그레이드
            if (action == "EVENT") return ConsoleColor.Yellow; // 이벤트
            if (action == "LOOT") return ConsoleColor.Yellow; // 보상
            if (action == "START") return ConsoleColor.Red; // 시작 감염
            if (action == "DENIED") return ConsoleColor.DarkGray; // 접근 불가

            return ConsoleColor.DarkGray; // 기타
        }

        private ConsoleColor GetPanelRewardColor(string reward)
        {
            if (reward.IndexOf("BUY", StringComparison.Ordinal) >= 0) return ConsoleColor.Green; // 거래
            if (reward.IndexOf("BOOST", StringComparison.Ordinal) >= 0) return ConsoleColor.Green; // 업그레이드
            if (reward.IndexOf("RANDOM", StringComparison.Ordinal) >= 0) return ConsoleColor.Yellow; // 랜덤 이벤트
            if (reward.IndexOf("RECOVERY", StringComparison.Ordinal) >= 0) return ConsoleColor.Yellow; // 보상
            if (reward.IndexOf("ITEM", StringComparison.Ordinal) >= 0) return ConsoleColor.Yellow; // 아이템 보상
            if (reward.IndexOf("ENDING", StringComparison.Ordinal) >= 0) return ConsoleColor.White; // 엔딩
            if (reward.IndexOf("KB", StringComparison.Ordinal) >= 0) return ConsoleColor.Cyan; // 전투 보상

            return ConsoleColor.DarkGray; // 보상 없음
        }

        private string GetNodeRisk(GridNode node)
        {
            if (node.Type == NodeType.Boss) return "CRITICAL";
            if (node.Type == NodeType.Firewall) return "HIGH";
            if (node.Type == NodeType.Security) return "LOW";
            if (node.Type == NodeType.Event) return "UNKNOWN";
            if (node.Type == NodeType.DataCache) return "SAFE";
            if (node.Type == NodeType.Shop) return "SAFE";
            if (node.Type == NodeType.Mutation) return "SAFE";

            return "NONE";
        }


        private ConsoleColor GetRiskColor(string risk)
        {
            if (risk == "CRITICAL") return ConsoleColor.White; // 핵심 시스템
            if (risk == "HIGH") return ConsoleColor.Blue; // 엘리트 전투
            if (risk == "LOW") return ConsoleColor.Cyan; // 일반 전투
            if (risk == "SAFE") return ConsoleColor.Green; // 정비/보상 지역
            if (risk == "UNKNOWN") return ConsoleColor.Yellow; // 예측 불가
            if (risk == "NONE") return ConsoleColor.DarkGray; // 위험 없음

            return ConsoleColor.Gray; // 기본값
        }
    }
}
