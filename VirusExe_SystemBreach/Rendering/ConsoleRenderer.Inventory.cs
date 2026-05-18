using System;
using System.Collections.Generic;
using VirusExe.SystemBreach.Characters;
using VirusExe.SystemBreach.Core;
using VirusExe.SystemBreach.Systems;

namespace VirusExe.SystemBreach.Rendering
{
    // 인벤토리 화면 출력
    // 보유 아이템 목록, 상세 정보, 장착/사용 안내 표시
    public partial class ConsoleRenderer
    {
        private const int InventoryVisibleRows = 20; // 인벤토리 목록 고정 줄 수

        public void ShowInventory(Player player)
        {
            int selectedIndex = 0; // 현재 선택 인덱스
            string message = ""; // 하단 안내 메시지

            Console.CursorVisible = false; // 커서 숨김

            while (true) // 인벤토리 화면 유지
            {
                List<string> itemNames = GetInventoryItemNames(player); // 표시할 아이템 목록 생성

                if (selectedIndex >= itemNames.Count) selectedIndex = itemNames.Count - 1; // 선택 위치 상한 보정
                if (selectedIndex < 0) selectedIndex = 0; // 선택 위치 하한 보정

                RenderInventoryScreen(player, itemNames, selectedIndex, message); // 인벤토리 화면 출력

                ConsoleKey key = InputHelper.ReadKey(); // 키 입력 체크

                if (key == ConsoleKey.W) // 위 이동 입력 체크
                {
                    if (itemNames.Count > 0) selectedIndex--; // 선택 위치 위로 이동
                    if (selectedIndex < 0) selectedIndex = itemNames.Count - 1; // 위쪽 순환
                }
                else if (key == ConsoleKey.S) // 아래 이동 입력 체크
                {
                    if (itemNames.Count > 0) selectedIndex++; // 선택 위치 아래로 이동
                    if (selectedIndex >= itemNames.Count) selectedIndex = 0; // 아래쪽 순환
                }
                else if (key == ConsoleKey.E) // 장착 입력 체크
                {
                    if (itemNames.Count == 0) // 아이템 없음 체크
                    {
                        message = "장착할 데이터가 없습니다.";
                    }
                    else
                    {
                        string equipMessage; // 장착 결과 메시지
                        bool equipped = player.EquipItem(itemNames[selectedIndex], out equipMessage); // 선택 아이템 장착 시도
                        message = equipMessage; // 결과 메시지 저장

                        if (equipped) // 장착 성공 체크
                        {
                            selectedIndex = 0; // 목록 변경 후 선택 위치 초기화
                        }
                    }
                }
                else if (key == ConsoleKey.Q) // 종료 입력 체크
                {
                    break; // 창닫기
                }
            }
        }

        private List<string> GetInventoryItemNames(Player player)
        {
            Dictionary<string, int> snapshot = player.Inventory.GetSnapshot(); // 현재 인벤토리 복사
            List<string> names = new List<string>(); // 표시 목록

            foreach (KeyValuePair<string, int> pair in snapshot) // 보유 아이템 순회
            {
                if (pair.Value > 0) names.Add(pair.Key); // 수량 있는 아이템만 추가
            }

            names.Sort(CompareInventoryItemNames); // 장비 우선 정렬
            return names; // 정렬된 목록
        }

        private int CompareInventoryItemNames(string a, string b)
        {
            ItemData left = ItemDatabase.Get(a); // 왼쪽 아이템 데이터
            ItemData right = ItemDatabase.Get(b); // 오른쪽 아이템 데이터
            int leftOrder = GetInventorySortOrder(left); // 왼쪽 정렬 우선순위
            int rightOrder = GetInventorySortOrder(right); // 오른쪽 정렬 우선순위

            if (leftOrder != rightOrder) return leftOrder.CompareTo(rightOrder); // 분류 우선 정렬
            return string.Compare(a, b, StringComparison.Ordinal); // 이름 정렬
        }

        private int GetInventorySortOrder(ItemData data)
        {
            if (data == null) return 9; // 미등록 아이템 후순위
            if (data.Type == ItemType.Weapon) return 0; // 무기 우선
            if (data.Type == ItemType.Gear) return 1; // 장비 다음
            if (data.Type == ItemType.Consumable) return 2; // 소모품 다음
            if (data.Type == ItemType.Material) return 3; // 재료 후순위
            return 9; // 기본 후순위
        }

        private void RenderInventoryScreen(Player player, List<string> itemNames, int selectedIndex, string message)
        {
            Console.CursorVisible = false; // 커서 숨김

            int rowCount = InventoryVisibleRows; // 목록 영역 고정
            int scrollOffset = GetListScrollOffset(selectedIndex, itemNames.Count, rowCount); // 선택 위치 기준 스크롤

            BeginModal("DATA STORAGE       // INVENTORY", ModalSize.Large); // 대형 모달 고정
            WriteInventoryEquipmentLine(player); // 현재 장비 출력
            WriteModalSeparator();

            if (itemNames.Count == 0) // 인벤토리 비었는지 체크
            {
                WriteInventoryDualLine("   저장된 데이터가 없습니다.", ConsoleColor.DarkGray, "ITEM INFO", ConsoleColor.DarkGray);
                WriteInventoryDualLine(string.Empty, ConsoleColor.DarkGray, "DATA STORAGE가 비어 있습니다.", ConsoleColor.Gray);

                for (int i = 2; i < rowCount; i++) // 빈 목록 높이 보정
                {
                    WriteInventoryDualLine(string.Empty, ConsoleColor.DarkGray, string.Empty, ConsoleColor.DarkGray);
                }
            }
            else
            {
                string selectedName = itemNames[selectedIndex]; // 선택 아이템 이름
                List<string> infoLines = BuildInventoryInfoLines(player, selectedName); // 우측 정보 목록

                for (int i = 0; i < rowCount; i++) // 고정 목록 줄 출력
                {
                    int actualIndex = scrollOffset + i; // 실제 아이템 인덱스
                    string rightText = i < infoLines.Count ? infoLines[i] : string.Empty; // 우측 정보 텍스트
                    ConsoleColor rightColor = GetInventoryInfoColor(rightText); // 우측 정보 색상

                    if (actualIndex < itemNames.Count) // 표시 가능한 아이템 체크
                    {
                        string itemName = itemNames[actualIndex]; // 행 아이템 이름
                        int count = player.Inventory.GetCount(itemName); // 보유 수량
                        bool selected = actualIndex == selectedIndex; // 선택 행 체크
                        WriteInventoryItemDualLine(itemName, count, selected, rightText, rightColor); // 태그/등급 색상 출력
                    }
                    else
                    {
                        WriteInventoryDualLine(string.Empty, ConsoleColor.DarkGray, rightText, rightColor); // 빈 목록 행 출력
                    }
                }
            }

            WriteModalControlFooter("장착", message, GetInventoryMessageColor(message)); // Footer 1줄 + LOG
            EndModal(); // 인벤토리 모달 종료
            HideCursor(); // 커서 유배
        }

        private int GetListScrollOffset(int selectedIndex, int totalCount, int visibleCount)
        {
            if (totalCount <= visibleCount) return 0; // 스크롤 불필요
            if (selectedIndex < 0) selectedIndex = 0; // 선택 하한 보정
            if (selectedIndex >= totalCount) selectedIndex = totalCount - 1; // 선택 상한 보정
            if (selectedIndex < visibleCount) return 0; // 첫 페이지 유지

            return selectedIndex - visibleCount + 1; // 선택 줄 하단 고정
        }

        private void WriteInventoryEquipmentLine(Player player)
        {
            string weapon = player.EquippedWeapon == null ? "EMPTY" : player.EquippedWeapon.DisplayName + " (+" + player.EquippedWeapon.AttackBonus + " ATK)"; // 무기 표시
            string gear = player.EquippedGear == null ? "EMPTY" : GetGearEquipText(player.EquippedGear); // 장비 표시

            WriteModalSegmentsLine(
                new ColorSegment(" WEAPON : ", ConsoleColor.DarkGray),
                new ColorSegment(TextUtil.Fit(weapon, 36), player.EquippedWeapon == null ? ConsoleColor.DarkGray : GetItemGradeColor(player.EquippedWeapon)),
                new ColorSegment("GEAR : ", ConsoleColor.DarkGray),
                new ColorSegment(TextUtil.Fit(gear, 39), player.EquippedGear == null ? ConsoleColor.DarkGray : GetItemGradeColor(player.EquippedGear)));
        }

        private string GetGearEquipText(ItemData gear)
        {
            string text = gear.DisplayName; // 태그 포함 장비명
            if (gear.AttackBonus > 0) text += " (+" + gear.AttackBonus + " ATK)"; // ATK 보너스 표시
            if (gear.HealthBonus > 0) text += " (+" + gear.HealthBonus + " HEALTH)"; // HEALTH 보너스 표시
            if (gear.EnergyBonus > 0) text += " (+" + gear.EnergyBonus + " ENERGY)"; // ENERGY 보너스 표시
            return text; // 장비 표시 텍스트
        }

        private List<string> BuildInventoryInfoLines(Player player, string itemName)
        {
            List<string> lines = new List<string>(); // 우측 정보 줄 목록
            ItemData data = ItemDatabase.Get(itemName); // 아이템 데이터 조회

            lines.Add("ITEM INFO");
            lines.Add(string.Empty);

            if (data == null) // 미등록 아이템인지 체크
            {
                lines.Add(itemName);
                lines.Add("TYPE   : UNKNOWN");
                lines.Add("등록되지 않은 데이터입니다.");
                return lines;
            }

            lines.Add(data.DisplayName + GetEquippedTag(player, data));
            lines.Add("TYPE   : " + data.GetTypeLabel());
            lines.Add("GRADE  : " + GetGradeLabel(data.Grade));
            if (data.RequiredMutation != VirusMutation.None) lines.Add("REQUIRE: " + data.GetRequiredMutationLabel()); // 전용 조건 표시

            if (data.AttackBonus != 0) lines.Add("ATK    : +" + data.AttackBonus); // ATK 표시
            if (data.HealthBonus != 0) lines.Add("HEALTH : +" + data.HealthBonus); // HEALTH 표시
            if (data.EnergyBonus != 0) lines.Add("ENERGY : +" + data.EnergyBonus); // ENERGY 표시

            lines.Add("VALUE  : " + data.Value + "KB");
            lines.Add(string.Empty);
            lines.Add(data.Description);
            lines.Add(string.Empty);

            if (data.IsEquipable()) // 장착 가능 여부 체크
            {
                lines.Add("E : 장착");
                lines.Add("판매는 상점에서만 가능합니다.");
            }
            else
            {
                lines.Add("전투/노드 전용 데이터입니다.");
                lines.Add("판매는 상점에서만 가능합니다.");
            }

            return lines;
        }

        private string GetEquippedTag(Player player, ItemData data)
        {
            if (player.EquippedWeapon != null && player.EquippedWeapon.Name == data.Name) return " [EQUIPPED]"; // 무기 장착 표시
            if (player.EquippedGear != null && player.EquippedGear.Name == data.Name) return " [EQUIPPED]"; // 장비 장착 표시
            return string.Empty; // 장착 아님
        }

        private ConsoleColor GetItemTagColor(ItemData data)
        {
            if (data == null) return ConsoleColor.Gray; // 미등록 색상
            if (data.Type == ItemType.Weapon) return ConsoleColor.Yellow; // 무기 태그
            if (data.Type == ItemType.Gear) return ConsoleColor.Cyan; // 장비 계열 태그
            if (data.Type == ItemType.Consumable) return ConsoleColor.Green; // 소비 태그
            if (data.Type == ItemType.Material) return ConsoleColor.Magenta; // 재료 태그
            return ConsoleColor.Gray; // 기본 색상
        }

        private ConsoleColor GetItemGradeColor(ItemData data)
        {
            if (data == null) return ConsoleColor.Gray; // 미등록 색상
            if (data.Grade == ItemGrade.Common) return ConsoleColor.White; // COMMON 색상
            if (data.Grade == ItemGrade.Rare) return ConsoleColor.Cyan; // RARE 색상
            if (data.Grade == ItemGrade.Elite) return ConsoleColor.Yellow; // ELITE 색상
            if (data.Grade == ItemGrade.Legendary) return ConsoleColor.Magenta; // LEGENDARY 색상
            return ConsoleColor.Gray; // 기본 색상
        }

        private string GetGradeLabel(ItemGrade grade)
        {
            if (grade == ItemGrade.Common) return "COMMON"; // 일반 등급
            if (grade == ItemGrade.Rare) return "RARE"; // 중급 등급
            if (grade == ItemGrade.Elite) return "ELITE"; // 고급 등급
            if (grade == ItemGrade.Legendary) return "LEGENDARY"; // 최상급 등급
            return "UNKNOWN"; // 알 수 없음
        }

        private ConsoleColor GetInventoryInfoColor(string text)
        {
            if (string.IsNullOrEmpty(text)) return ConsoleColor.DarkGray; // 빈 줄 색상
            if (text == "ITEM INFO") return ConsoleColor.Cyan; // 정보 제목 색상
            if (text.IndexOf("[EQUIPPED]", StringComparison.Ordinal) >= 0) return ConsoleColor.Green; // 장착 표시 색상
            if (text.StartsWith("TYPE", StringComparison.Ordinal)) return ConsoleColor.DarkGray; // 타입 라벨 색상
            if (text.StartsWith("GRADE", StringComparison.Ordinal)) return ConsoleColor.Cyan; // 등급 라벨 색상
            if (text.StartsWith("REQUIRE", StringComparison.Ordinal)) return ConsoleColor.Magenta; // 전용 조건 색상
            if (text.StartsWith("ATK", StringComparison.Ordinal)) return ConsoleColor.Yellow; // ATK 색상
            if (text.StartsWith("HEALTH", StringComparison.Ordinal)) return ConsoleColor.Green; // HEALTH 색상
            if (text.StartsWith("ENERGY", StringComparison.Ordinal)) return ConsoleColor.Cyan; // ENERGY 색상
            if (text.StartsWith("VALUE", StringComparison.Ordinal)) return ConsoleColor.Yellow; // 가격 색상
            if (text.StartsWith("E", StringComparison.Ordinal)) return ConsoleColor.Magenta; // 조작 안내 색상
            if (text.IndexOf("상점", StringComparison.Ordinal) >= 0) return ConsoleColor.DarkGray; // 판매 제한 색상
            return ConsoleColor.Gray; // 기본 정보 색상
        }

        private void WriteInventoryItemDualLine(string itemName, int count, bool selected, string rightText, ConsoleColor rightColor)
        {
            const int leftWidth = 45; // 좌측 리스트 폭
            const int dividerWidth = 5; // 중앙 구분 폭
            int rightWidth = modalInnerWidth - leftWidth - dividerWidth; // 우측 설명 폭
            ItemData data = ItemDatabase.Get(itemName); // 아이템 데이터 조회
            string cursor = selected ? ">> " : "   "; // 선택 커서
            string tag = data == null ? "[DATA]" : data.GetDisplayTag(); // 타입 태그
            string name = data == null ? itemName : data.Name; // 코드명
            string countText = " x" + count; // 수량 표시
            int fixedWidth = TextUtil.GetDisplayWidth(" " + cursor + tag + " " + countText); // 고정 폭 계산
            int nameWidth = Math.Max(1, leftWidth - fixedWidth); // 이름 출력 폭

            WriteModalSegmentsLine(
                new ColorSegment(" " + cursor, selected ? ConsoleColor.Magenta : ConsoleColor.DarkGray),
                new ColorSegment(tag, GetItemTagColor(data)),
                new ColorSegment(" ", ConsoleColor.DarkGray),
                new ColorSegment(TextUtil.Fit(name, nameWidth), GetItemGradeColor(data)),
                new ColorSegment(countText, ConsoleColor.DarkGray),
                new ColorSegment("  |  ", ConsoleColor.DarkGray),
                new ColorSegment(TextUtil.Fit(rightText, rightWidth), rightColor));
        }

        private void WriteInventoryDualLine(string leftText, ConsoleColor leftColor, string rightText, ConsoleColor rightColor)
        {
            const int leftWidth = 45; // 좌측 리스트 폭
            const int dividerWidth = 5; // 중앙 구분 폭
            int rightWidth = modalInnerWidth - leftWidth - dividerWidth; // 우측 설명 폭

            WriteModalSegmentsLine(
                new ColorSegment(TextUtil.Fit(" " + leftText, leftWidth), leftColor),
                new ColorSegment("  |  ", ConsoleColor.DarkGray),
                new ColorSegment(TextUtil.Fit(rightText, rightWidth), rightColor));
        }

        private ConsoleColor GetInventoryMessageColor(string message)
        {
            if (string.IsNullOrEmpty(message)) return ConsoleColor.DarkGray; // 메시지 없음
            if (message.IndexOf("없", StringComparison.Ordinal) >= 0 || message.IndexOf("불가", StringComparison.Ordinal) >= 0) return ConsoleColor.Red; // 실패
            if (message.IndexOf("장착", StringComparison.Ordinal) >= 0 || message.IndexOf("완료", StringComparison.Ordinal) >= 0) return ConsoleColor.Green; // 성공
            return ConsoleColor.Gray; // 기본
        }

    }
}
