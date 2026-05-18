using System;
using System.Collections.Generic;
using VirusExe.SystemBreach.Characters;

namespace VirusExe.SystemBreach.Systems
{
    // 상점 재고 생성
    // 현재 진행 구간과 변이에 맞춰 판매 목록 구성
    public static class ShopStockGenerator
    {
        private static readonly Random random = new Random(); // 상점 재고 랜덤

        public static List<string> GenerateStock(Player player, int column)
        {
            VirusMutation mutation = player == null ? VirusMutation.None : player.Mutation; // 현재 변이
            List<string> sourcePool = ItemPoolManager.GetNodeRewardPool(column, mutation); // 현재 열 기준 풀
            ItemPoolTier tier = ItemPoolManager.GetPoolTierByColumn(column); // 현재 구간
            List<string> stock = new List<string>(); // 최종 재고

            AddRandomItems(stock, FilterConsumables(sourcePool), 2); // 소비 아이템 2개
            AddRandomItems(stock, FilterMaterials(sourcePool), 2); // 강화 재료 2개
            AddRandomItems(stock, FilterWeapons(sourcePool), 2); // 무기 2개
            AddRandomItems(stock, FilterCommonGear(sourcePool), 2); // 공용 장비 2개
            AddRandomItems(stock, FilterMutationGear(sourcePool, mutation), GetMutationGearCount(tier, mutation)); // 전용 장비

            stock.Sort(ItemDatabase.CompareItemNames); // 표시 정렬
            return stock;
        }

        private static int GetMutationGearCount(ItemPoolTier tier, VirusMutation mutation)
        {
            if (mutation == VirusMutation.None) return 0; // 변이 전 전용 장비 제외
            if (tier == ItemPoolTier.Middle) return 1; // 중반 전용 장비 1개
            if (tier == ItemPoolTier.Late) return 2; // 후반 전용 장비 2개
            return 0; // 초반 전용 장비 제외
        }

        private static List<string> FilterConsumables(List<string> sourcePool)
        {
            return FilterBy(sourcePool, delegate(ItemData data)
            {
                return data.Type == ItemType.Consumable; // 소비 아이템 체크
            });
        }

        private static List<string> FilterMaterials(List<string> sourcePool)
        {
            return FilterBy(sourcePool, delegate(ItemData data)
            {
                return data.Type == ItemType.Material; // 강화 재료 체크
            });
        }

        private static List<string> FilterWeapons(List<string> sourcePool)
        {
            return FilterBy(sourcePool, delegate(ItemData data)
            {
                return data.Type == ItemType.Weapon; // 무기 체크
            });
        }

        private static List<string> FilterCommonGear(List<string> sourcePool)
        {
            return FilterBy(sourcePool, delegate(ItemData data)
            {
                return data.Type == ItemType.Gear && data.RequiredMutation == VirusMutation.None; // 공용 장비 체크
            });
        }

        private static List<string> FilterMutationGear(List<string> sourcePool, VirusMutation mutation)
        {
            if (mutation == VirusMutation.None) return new List<string>(); // 변이 전 제외

            return FilterBy(sourcePool, delegate(ItemData data)
            {
                return data.Type == ItemType.Gear && data.RequiredMutation == mutation; // 현재 변이 전용 장비 체크
            });
        }

        private static List<string> FilterBy(List<string> sourcePool, Predicate<ItemData> predicate)
        {
            List<string> result = new List<string>(); // 필터 결과

            for (int i = 0; i < sourcePool.Count; i++) // 후보 순회
            {
                ItemData data = ItemDatabase.Get(sourcePool[i]); // 아이템 데이터
                if (data != null && predicate(data)) result.Add(sourcePool[i]); // 조건 통과 추가
            }

            return result;
        }

        private static void AddRandomItems(List<string> stock, List<string> candidates, int count)
        {
            List<string> available = new List<string>(candidates); // 선택 후보 복사

            for (int i = 0; i < count && available.Count > 0; i++) // 요청 수만큼 선택
            {
                int index = random.Next(0, available.Count); // 랜덤 인덱스
                string itemName = available[index]; // 선택 아이템
                available.RemoveAt(index); // 중복 방지

                if (!stock.Contains(itemName)) stock.Add(itemName); // 재고 추가
            }
        }
    }
}
