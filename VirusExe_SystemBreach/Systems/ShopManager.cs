using System;
using System.Collections.Generic;
using VirusExe.SystemBreach.Characters;
using VirusExe.SystemBreach.Core;
using VirusExe.SystemBreach.Rendering;

namespace VirusExe.SystemBreach.Systems
{
    // 상점 입력/거래 처리
    // 구매/판매 선택, KB 지불, 인벤토리 반영 관리
    public class ShopManager
    {
        private readonly ConsoleRenderer renderer; // 화면 출력 렌더러

        public ShopManager(ConsoleRenderer renderer)
        {
            this.renderer = renderer;
        }

        public void Open(Player player, int column)
        {
            int selectedIndex = 0; // 현재 선택 메뉴
            string message = "EXPLOIT MARKET 접속 완료"; // 하단 메시지
            List<string> buyStock = ShopStockGenerator.GenerateStock(player, column); // 상점 진입 시 재고 생성

            Console.CursorVisible = false; // 커서 숨김

            while (true) // 상점 메인 루프
            {
                renderer.RenderShopMainMenu(player, selectedIndex, message); // 메인 메뉴 출력

                ConsoleKey key = InputHelper.ReadKey(); // 키 입력

                if (key == ConsoleKey.W) // 위 이동 체크
                {
                    selectedIndex--; // 위 메뉴
                    if (selectedIndex < 0) selectedIndex = 2; // 순환
                }
                else if (key == ConsoleKey.S) // 아래 이동 체크
                {
                    selectedIndex++; // 아래 메뉴
                    if (selectedIndex > 2) selectedIndex = 0; // 순환
                }
                else if (key == ConsoleKey.E) // 선택 체크
                {
                    if (selectedIndex == 0) // 구매 메뉴 체크
                    {
                        OpenBuy(player, buyStock); // 구매 화면
                        message = "구매 창을 닫았습니다.";
                    }
                    else if (selectedIndex == 1) // 판매 메뉴 체크
                    {
                        OpenSell(player); // 판매 화면
                        message = "판매 창을 닫았습니다.";
                    }
                    else if (selectedIndex == 2) // 나가기 체크
                    {
                        return;
                    }
                }
                else if (key == ConsoleKey.Q) // 창닫기 체크
                {
                    return;
                }
            }
        }

        private void OpenBuy(Player player, List<string> buyStock)
        {
            int selectedIndex = 0; // 구매 선택 위치
            string message = "구매할 데이터를 선택하세요."; // 하단 메시지

            while (true) // 구매 루프
            {
                string[] buyItemNames = buyStock.ToArray(); // 현재 상점 재고 복사

                if (buyItemNames.Length == 0) selectedIndex = 0; // 빈 재고 보정
                else if (selectedIndex >= buyItemNames.Length) selectedIndex = buyItemNames.Length - 1; // 상한 보정
                else if (selectedIndex < 0) selectedIndex = 0; // 하한 보정

                renderer.RenderShopBuyScreen(player, buyItemNames, selectedIndex, message); // 구매 화면 출력

                ConsoleKey key = InputHelper.ReadKey(); // 키 입력

                if (key == ConsoleKey.W) // 위 이동 체크
                {
                    if (buyItemNames.Length > 0) selectedIndex--; // 위 상품
                    if (selectedIndex < 0) selectedIndex = buyItemNames.Length - 1; // 순환
                }
                else if (key == ConsoleKey.S) // 아래 이동 체크
                {
                    if (buyItemNames.Length > 0) selectedIndex++; // 아래 상품
                    if (selectedIndex >= buyItemNames.Length) selectedIndex = 0; // 순환
                }
                else if (key == ConsoleKey.E) // 구매 체크
                {
                    if (buyItemNames.Length == 0) // 상품 없음 체크
                    {
                        message = "구매 가능한 데이터가 없습니다.";
                    }
                    else
                    {
                        string itemName = buyItemNames[selectedIndex]; // 선택 상품
                        bool success = TryBuy(player, itemName, out message); // 구매 처리

                        if (success) // 구매 성공 체크
                        {
                            buyStock.Remove(itemName); // 구매한 재고 제거
                            if (selectedIndex >= buyStock.Count) selectedIndex = buyStock.Count - 1; // 선택 위치 보정
                            if (selectedIndex < 0) selectedIndex = 0; // 빈 재고 보정
                        }
                    }
                }
                else if (key == ConsoleKey.Q) // 창닫기 체크
                {
                    return; // 상점 메인으로 닫기
                }
            }
        }

        private void OpenSell(Player player)
        {
            int selectedIndex = 0; // 판매 선택 위치
            string message = "판매할 데이터를 선택하세요."; // 하단 메시지

            while (true) // 판매 루프
            {
                List<string> itemNames = GetSellItemNames(player); // 판매 가능 목록

                if (selectedIndex >= itemNames.Count) selectedIndex = itemNames.Count - 1; // 상한 보정
                if (selectedIndex < 0) selectedIndex = 0; // 하한 보정

                renderer.RenderShopSellScreen(player, itemNames, selectedIndex, message); // 판매 화면 출력

                ConsoleKey key = InputHelper.ReadKey(); // 키 입력

                if (key == ConsoleKey.W) // 위 이동 체크
                {
                    if (itemNames.Count > 0) selectedIndex--; // 위 아이템
                    if (selectedIndex < 0) selectedIndex = itemNames.Count - 1; // 순환
                }
                else if (key == ConsoleKey.S) // 아래 이동 체크
                {
                    if (itemNames.Count > 0) selectedIndex++; // 아래 아이템
                    if (selectedIndex >= itemNames.Count) selectedIndex = 0; // 순환
                }
                else if (key == ConsoleKey.E) // 판매 체크
                {
                    if (itemNames.Count == 0) // 판매 아이템 없음 체크
                    {
                        message = "판매 가능한 데이터가 없습니다.";
                    }
                    else
                    {
                        message = Sell(player, itemNames[selectedIndex]); // 판매 처리
                    }
                }
                else if (key == ConsoleKey.Q) // 창닫기 체크
                {
                    return; // 상점 메인으로 닫기
                }
            }
        }

        private bool TryBuy(Player player, string itemName, out string message)
        {
            int price = GetBuyPrice(itemName); // 구매가 계산

            if (!player.SpendKb(price)) // KB 부족 체크
            {
                message = "KB 부족. " + ItemDatabase.GetDisplayName(itemName) + " 구매 실패.";
                return false; // 구매 실패
            }

            player.Inventory.Add(itemName, 1); // 아이템 지급
            message = ItemDatabase.GetDisplayName(itemName) + " 구매 완료. -" + price + "KB";
            return true; // 구매 성공
        }

        private string Sell(Player player, string itemName)
        {
            ItemData data = ItemDatabase.Get(itemName); // 아이템 데이터 조회

            if (data == null) // 미등록 아이템 체크
            {
                return "미등록 데이터는 판매할 수 없습니다.";
            }

            if (!player.Inventory.Remove(itemName, 1)) // 보유 수량 체크
            {
                return "DATA STORAGE에 존재하지 않습니다.";
            }

            int price = GetSellPrice(itemName); // 판매가 계산
            player.AddKb(price); // KB 지급

            return ItemDatabase.GetDisplayName(itemName) + " 판매 완료. +" + price + "KB";
        }

        private List<string> GetSellItemNames(Player player)
        {
            Dictionary<string, int> snapshot = player.Inventory.GetSnapshot(); // 인벤토리 복사
            List<string> names = new List<string>(); // 판매 목록

            foreach (KeyValuePair<string, int> pair in snapshot) // 보유 아이템 순회
            {
                if (pair.Value > 0) names.Add(pair.Key); // 수량 있는 아이템 추가
            }

            names.Sort(CompareShopItemNames); // 정렬
            return names;
        }

        private int CompareShopItemNames(string a, string b)
        {
            return ItemDatabase.CompareItemNames(a, b); // 공통 아이템 정렬 사용
        }

        private int GetBuyPrice(string itemName)
        {
            ItemData data = ItemDatabase.Get(itemName); // 데이터 조회
            if (data == null) return 9999; // 미등록 방지
            if (data.Type == ItemType.Material) return 80; // 강화 재료 구매가 고정

            return data.Value * 2; // 구매가
        }

        private int GetSellPrice(string itemName)
        {
            ItemData data = ItemDatabase.Get(itemName); // 데이터 조회
            if (data == null) return 0; // 미등록 가격

            return data.Value; // 판매 기준가
        }
    }
}
