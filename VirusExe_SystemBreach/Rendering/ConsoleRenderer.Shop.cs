using System;
using System.Collections.Generic;
using VirusExe.SystemBreach.Characters;
using VirusExe.SystemBreach.Core;
using VirusExe.SystemBreach.Systems;

namespace VirusExe.SystemBreach.Rendering
{
	// 상점 화면 출력
	// 구매/판매 목록, 가격, 설명, 선택 커서 표시
	public partial class ConsoleRenderer
    {
        private const int ShopVisibleRows = 20; // 상점 목록 고정 줄 수

        public void RenderShopMainMenu(Player player, int selectedIndex, string message)
        {
            BeginModal("EXPLOIT MARKET       // DATA TRADER", ModalSize.Large); // 상점 메인 모달 시작

            WriteShopStatusLine(player); // 보유 KB 출력
            WriteModalSeparator();

            WriteShopMenuRow(0, selectedIndex, "BUY SOFTWARE", "소모성 침투 도구 구매");
            WriteShopMenuRow(1, selectedIndex, "SELL DATA", "보유 데이터 판매");
            WriteShopMenuRow(2, selectedIndex, "EXIT MARKET", "창닫기");

            WriteShopControlLine("선택", message); // Footer 1줄 + LOG
            EndModal(); // 상점 메인 모달 종료
            HideCursor(); // 커서 유배
        }

        public void RenderShopBuyScreen(Player player, string[] itemNames, int selectedIndex, string message)
        {
            int rowCount = ShopVisibleRows; // 상품 목록 영역 고정
            int itemCount = itemNames == null ? 0 : itemNames.Length; // 상품 수
            int scrollOffset = GetListScrollOffset(selectedIndex, itemCount, rowCount); // 선택 위치 기준 스크롤

            BeginModal("EXPLOIT MARKET       // BUY SOFTWARE", ModalSize.Large); // 대형 모달 고정
            WriteShopStatusLine(player); // 보유 KB 출력
            WriteModalSeparator();

            if (itemNames == null || itemNames.Length == 0) // 상품 없음 체크
            {
                WriteShopDualLine("   구매 가능한 데이터가 없습니다.", ConsoleColor.DarkGray, "ITEM INFO", ConsoleColor.Cyan);
                WriteShopDualLine(string.Empty, ConsoleColor.DarkGray, "MARKET CACHE EMPTY", ConsoleColor.DarkGray);

                for (int i = 2; i < rowCount; i++) // 빈 목록 높이 보정
                {
                    WriteShopDualLine(string.Empty, ConsoleColor.DarkGray, string.Empty, ConsoleColor.DarkGray);
                }
            }
            else
            {
                string selectedName = itemNames[selectedIndex]; // 선택 상품
                List<string> infoLines = BuildShopBuyInfoLines(selectedName); // 우측 정보

                for (int i = 0; i < rowCount; i++) // 고정 상품 줄 출력
                {
                    int actualIndex = scrollOffset + i; // 실제 상품 인덱스
                    string rightText = i < infoLines.Count ? infoLines[i] : string.Empty; // 설명 줄
                    ConsoleColor rightColor = GetShopInfoColor(rightText); // 설명 색상

                    if (actualIndex < itemNames.Length) // 표시 가능한 상품 체크
                    {
                        string itemName = itemNames[actualIndex]; // 상품명
                        bool selected = actualIndex == selectedIndex; // 선택 여부
                        WriteShopItemDualLine(itemName, GetShopBuyPrice(itemName) + "KB", selected, rightText, rightColor); // 태그/등급 색상 출력
                    }
                    else
                    {
                        WriteShopDualLine(string.Empty, ConsoleColor.DarkGray, rightText, rightColor); // 빈 줄 출력
                    }
                }
            }

            WriteShopControlLine("구매", message); // Footer 1줄 + LOG
            EndModal(); // 구매 모달 종료
            HideCursor(); // 커서 유배
        }

        public void RenderShopSellScreen(Player player, List<string> itemNames, int selectedIndex, string message)
        {
            int rowCount = ShopVisibleRows; // 판매 목록 영역 고정
            int itemCount = itemNames == null ? 0 : itemNames.Count; // 판매 아이템 수
            int scrollOffset = GetListScrollOffset(selectedIndex, itemCount, rowCount); // 선택 위치 기준 스크롤

            BeginModal("EXPLOIT MARKET       // SELL DATA", ModalSize.Large); // 대형 모달 고정
            WriteShopStatusLine(player); // 보유 KB 출력
            WriteModalSeparator();

            if (itemNames == null || itemNames.Count == 0) // 판매 목록 없음 체크
            {
                WriteShopDualLine("   판매 가능한 데이터가 없습니다.", ConsoleColor.DarkGray, "ITEM INFO", ConsoleColor.Cyan);
                WriteShopDualLine(string.Empty, ConsoleColor.DarkGray, "DATA STORAGE EMPTY", ConsoleColor.DarkGray);

                for (int i = 2; i < rowCount; i++) // 빈 목록 높이 보정
                {
                    WriteShopDualLine(string.Empty, ConsoleColor.DarkGray, string.Empty, ConsoleColor.DarkGray);
                }
            }
            else
            {
                string selectedName = itemNames[selectedIndex]; // 선택 아이템
                List<string> infoLines = BuildShopSellInfoLines(player, selectedName); // 우측 정보

                for (int i = 0; i < rowCount; i++) // 고정 판매 줄 출력
                {
                    int actualIndex = scrollOffset + i; // 실제 아이템 인덱스
                    string rightText = i < infoLines.Count ? infoLines[i] : string.Empty; // 설명 줄
                    ConsoleColor rightColor = GetShopInfoColor(rightText); // 설명 색상

                    if (actualIndex < itemNames.Count) // 표시 가능한 아이템 체크
                    {
                        string itemName = itemNames[actualIndex]; // 아이템 이름
                        int count = player.Inventory.GetCount(itemName); // 보유 수량
                        bool selected = actualIndex == selectedIndex; // 선택 여부
                        WriteShopItemDualLine(itemName, "x" + count, selected, rightText, rightColor); // 태그/등급 색상 출력
                    }
                    else
                    {
                        WriteShopDualLine(string.Empty, ConsoleColor.DarkGray, rightText, rightColor); // 빈 줄 출력
                    }
                }
            }

            WriteShopControlLine("판매", message); // Footer 1줄 + LOG
            EndModal(); // 판매 모달 종료
            HideCursor(); // 커서 유배
        }

        private void WriteShopStatusLine(Player player)
        {
            WriteModalSegmentsLine(
                new ColorSegment(" STORAGE : ", ConsoleColor.DarkGray),
                new ColorSegment(player.Kb + "KB", ConsoleColor.Yellow),
                new ColorSegment(TextUtil.Fit("", 30), ConsoleColor.DarkGray),
                new ColorSegment("MARKET NODE : ", ConsoleColor.DarkGray),
                new ColorSegment("ONLINE", ConsoleColor.Green));
        }

        private void WriteShopMenuRow(int index, int selectedIndex, string title, string description)
        {
            bool selected = index == selectedIndex; // 선택 여부
            string cursor = selected ? ">> " : "   "; // 커서
            ConsoleColor titleColor = selected ? ConsoleColor.Magenta : ConsoleColor.DarkGray; // 제목 색상
            ConsoleColor descColor = selected ? ConsoleColor.Gray : ConsoleColor.DarkGray; // 설명 색상

            WriteShopDualLine(cursor + title, titleColor, description, descColor); // 메뉴 줄
        }

        private void WriteShopControlLine(string executeText, string message)
        {
            WriteModalControlFooter(executeText, message, GetShopMessageColor(message)); // Footer 1줄 + LOG
        }

        private List<string> BuildShopBuyInfoLines(string itemName)
        {
            List<string> lines = new List<string>(); // 정보 줄
            ItemData data = ItemDatabase.Get(itemName); // 아이템 데이터

            lines.Add("ITEM INFO");
            lines.Add(string.Empty);

            if (data == null) // 미등록 체크
            {
                lines.Add(itemName);
                lines.Add("TYPE   : UNKNOWN");
                lines.Add("PRICE  : -");
                return lines;
            }

            lines.Add(data.DisplayName);
            lines.Add("TYPE   : " + data.GetTypeLabel());
            lines.Add("GRADE  : " + GetGradeLabel(data.Grade));
            if (data.RequiredMutation != VirusMutation.None) lines.Add("REQUIRE: " + data.GetRequiredMutationLabel()); // 전용 조건 표시

            if (data.AttackBonus != 0) lines.Add("ATK    : +" + data.AttackBonus); // ATK 표시
            if (data.HealthBonus != 0) lines.Add("HEALTH : +" + data.HealthBonus); // HEALTH 표시
            if (data.EnergyBonus != 0) lines.Add("ENERGY : +" + data.EnergyBonus); // ENERGY 표시

            lines.Add("PRICE  : " + GetShopBuyPrice(itemName) + "KB");
            lines.Add(string.Empty);
            lines.Add(data.Description);
            return lines;
        }

        private List<string> BuildShopSellInfoLines(Player player, string itemName)
        {
            List<string> lines = new List<string>(); // 정보 줄
            ItemData data = ItemDatabase.Get(itemName); // 아이템 데이터

            lines.Add("ITEM INFO");
            lines.Add(string.Empty);

            if (data == null) // 미등록 체크
            {
                lines.Add(itemName);
                lines.Add("TYPE   : UNKNOWN");
                lines.Add("VALUE  : -");
                return lines;
            }

            lines.Add(data.DisplayName);
            lines.Add("TYPE   : " + data.GetTypeLabel());
            lines.Add("GRADE  : " + GetGradeLabel(data.Grade));
            if (data.RequiredMutation != VirusMutation.None) lines.Add("REQUIRE: " + data.GetRequiredMutationLabel()); // 전용 조건 표시

            if (data.AttackBonus != 0) lines.Add("ATK    : +" + data.AttackBonus); // ATK 표시
            if (data.HealthBonus != 0) lines.Add("HEALTH : +" + data.HealthBonus); // HEALTH 표시
            if (data.EnergyBonus != 0) lines.Add("ENERGY : +" + data.EnergyBonus); // ENERGY 표시

            lines.Add("COUNT  : " + player.Inventory.GetCount(itemName)); // 수량 표시
            lines.Add("VALUE  : " + GetShopSellPrice(itemName) + "KB");
            lines.Add(string.Empty);
            lines.Add(data.Description);
            return lines;
        }

        private int GetShopBuyPrice(string itemName)
        {
            if (itemName == ItemNames.Patch) return 32; // PATCH 가격
            if (itemName == ItemNames.EnergyCell) return 24; // ENERGY_CELL 가격
            if (itemName == ItemNames.ScanPulse) return 48; // SCAN_PULSE 가격

            ItemData data = ItemDatabase.Get(itemName); // 데이터 조회
            if (data == null) return 9999; // 미등록 방지
            if (data.Type == ItemType.Material) return 80; // 강화 재료 구매가 고정

            return data.Value * 2; // 기본 구매가
        }

        private int GetShopSellPrice(string itemName)
        {
            ItemData data = ItemDatabase.Get(itemName); // 데이터 조회
            if (data == null) return 0; // 미등록 방지

            return data.Value; // 판매가
        }

        private ConsoleColor GetShopInfoColor(string text)
        {
            if (string.IsNullOrEmpty(text)) return ConsoleColor.DarkGray; // 빈 줄
            if (text == "ITEM INFO") return ConsoleColor.Cyan; // 정보 제목
            if (text.StartsWith("TYPE", StringComparison.Ordinal)) return ConsoleColor.DarkGray; // 타입
            if (text.StartsWith("GRADE", StringComparison.Ordinal)) return ConsoleColor.Cyan; // 등급
            if (text.StartsWith("REQUIRE", StringComparison.Ordinal)) return ConsoleColor.Magenta; // 전용 조건
            if (text.StartsWith("PRICE", StringComparison.Ordinal)) return ConsoleColor.Yellow; // 구매가
            if (text.StartsWith("VALUE", StringComparison.Ordinal)) return ConsoleColor.Yellow; // 판매가
            if (text.StartsWith("COUNT", StringComparison.Ordinal)) return ConsoleColor.Cyan; // 수량
            if (text.StartsWith("ATK", StringComparison.Ordinal)) return ConsoleColor.Yellow; // ATK
            if (text.StartsWith("HEALTH", StringComparison.Ordinal)) return ConsoleColor.Green; // HEALTH
            if (text.StartsWith("ENERGY", StringComparison.Ordinal)) return ConsoleColor.Cyan; // ENERGY
            if (text.StartsWith("E", StringComparison.Ordinal)) return ConsoleColor.Magenta; // 조작

            return ConsoleColor.Gray; // 기본
        }

        private void WriteShopItemDualLine(string itemName, string tailText, bool selected, string rightText, ConsoleColor rightColor)
        {
            const int leftWidth = 45; // 좌측 폭
            const int dividerWidth = 5; // 구분 폭
            int rightWidth = modalInnerWidth - leftWidth - dividerWidth; // 우측 폭
            ItemData data = ItemDatabase.Get(itemName); // 아이템 데이터 조회
            string cursor = selected ? ">> " : "   "; // 선택 커서
            string tag = data == null ? "[DATA]" : data.GetDisplayTag(); // 타입 태그
            string name = data == null ? itemName : data.Name; // 코드명
            string tail = "  " + tailText; // 가격/수량 표시
            int fixedWidth = TextUtil.GetDisplayWidth(" " + cursor + tag + " " + tail); // 고정 폭 계산
            int nameWidth = Math.Max(1, leftWidth - fixedWidth); // 이름 폭 계산

            WriteModalSegmentsLine(
                new ColorSegment(" " + cursor, selected ? ConsoleColor.Magenta : ConsoleColor.DarkGray),
                new ColorSegment(tag, GetItemTagColor(data)),
                new ColorSegment(" ", ConsoleColor.DarkGray),
                new ColorSegment(TextUtil.Fit(name, nameWidth), GetItemGradeColor(data)),
                new ColorSegment(tail, ConsoleColor.DarkGray),
                new ColorSegment("  |  ", ConsoleColor.DarkGray),
                new ColorSegment(TextUtil.Fit(rightText, rightWidth), rightColor));
        }

        private void WriteShopDualLine(string leftText, ConsoleColor leftColor, string rightText, ConsoleColor rightColor)
        {
            const int leftWidth = 45; // 좌측 폭
            const int dividerWidth = 5; // 구분 폭
            int rightWidth = modalInnerWidth - leftWidth - dividerWidth; // 우측 폭

            WriteModalSegmentsLine(
                new ColorSegment(TextUtil.Fit(" " + leftText, leftWidth), leftColor),
                new ColorSegment("  |  ", ConsoleColor.DarkGray),
                new ColorSegment(TextUtil.Fit(rightText, rightWidth), rightColor));
        }

        private ConsoleColor GetShopMessageColor(string message)
        {
            if (string.IsNullOrEmpty(message)) return ConsoleColor.DarkGray; // 메시지 없음
            if (message.IndexOf("부족", StringComparison.Ordinal) >= 0 || message.IndexOf("없", StringComparison.Ordinal) >= 0) return ConsoleColor.Red; // 실패
            if (message.IndexOf("구매", StringComparison.Ordinal) >= 0 || message.IndexOf("판매", StringComparison.Ordinal) >= 0 || message.IndexOf("완료", StringComparison.Ordinal) >= 0) return ConsoleColor.Green; // 성공
            return ConsoleColor.Gray; // 기본
        }

    }
}
