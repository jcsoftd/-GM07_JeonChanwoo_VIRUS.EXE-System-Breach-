using System;
using System.Collections.Generic;
using VirusExe.SystemBreach.Characters;

namespace VirusExe.SystemBreach.Systems
{
    // 아이템 보상 풀 관리
    // GRID 진행 구간과 변이에 따라 나올 수 있는 아이템 후보 구성
    public static class ItemPoolManager
    {
        public static ItemPoolTier GetPoolTierByColumn(int column)
        {
            int displayColumn = column + 1; // GRID 표시 열 변환

            if (displayColumn <= 3) return ItemPoolTier.Early; // 2~3열 중심 초반 풀
            if (displayColumn <= 6) return ItemPoolTier.Middle; // 4~6열 중반 풀
            return ItemPoolTier.Late; // 7~9열 후반 풀
        }

        public static List<string> GetShopItemNames(int column, VirusMutation mutation)
        {
            List<string> names = GetNodeRewardPool(column, mutation); // 구간 풀 기반 상품
            names.Sort(ItemDatabase.CompareItemNames); // 표시 정렬
            return names; // 상품 목록
        }

        public static string RollNodeRewardName(Random random, int column, VirusMutation mutation)
        {
            if (random == null) return null; // Random 누락 방지

            List<string> pool = GetNodeRewardPool(column, mutation); // 현재 열 보상 풀
            if (pool.Count <= 0) return null; // 후보 없음

            ItemGrade grade = RollGrade(random, GetPoolTierByColumn(column)); // 구간 기준 등급 선택
            List<string> gradePool = FilterByGrade(pool, grade); // 등급 후보 필터

            if (gradePool.Count <= 0) gradePool = pool; // 등급 후보 없으면 전체 후보 사용
            return gradePool[random.Next(0, gradePool.Count)]; // 최종 보상 선택
        }

        public static List<string> GetNodeRewardPool(int column, VirusMutation mutation)
        {
            ItemPoolTier tier = GetPoolTierByColumn(column); // 열 구간 계산
            List<string> names = new List<string>(); // 후보 목록

            if (tier == ItemPoolTier.Early) // 초반 풀 체크
            {
                AddEarlyItems(names); // 초반 아이템 추가
            }
            else if (tier == ItemPoolTier.Middle) // 중반 풀 체크
            {
                AddMiddleItems(names, mutation); // 중반 아이템 추가
            }
            else
            {
                AddLateItems(names, mutation); // 후반 아이템 추가
            }

            return names; // 후보 목록
        }

        private static void AddEarlyItems(List<string> names)
        {
            AddIfExists(names, ItemNames.BrokenInjector);
            AddIfExists(names, ItemNames.PacketSpike);
            AddIfExists(names, ItemNames.MemoryShiv);

            AddIfExists(names, ItemNames.CrackedFirewall);
            AddIfExists(names, ItemNames.DebugShield);
            AddIfExists(names, ItemNames.ProcessPadding);
            AddIfExists(names, ItemNames.LowVoltCell);
            AddIfExists(names, ItemNames.SignalBuffer);
            AddIfExists(names, ItemNames.CacheCell);

            AddIfExists(names, ItemNames.Patch);
            AddIfExists(names, ItemNames.EnergyCell);
            AddUpgradeMaterials(names); // 강화 재료 추가
        }

        private static void AddMiddleItems(List<string> names, VirusMutation mutation)
        {
            AddIfExists(names, ItemNames.MemoryBlade);
            AddIfExists(names, ItemNames.PayloadDriver);
            AddIfExists(names, ItemNames.RootkitNeedle);

            AddIfExists(names, ItemNames.CoreStabilizer);
            AddIfExists(names, ItemNames.ProcessArmor);
            AddIfExists(names, ItemNames.EnergyCache);
            AddIfExists(names, ItemNames.ThreadBattery);

            AddMutationMiddleGear(names, mutation); // 현재 변이 전용 장비

            AddIfExists(names, ItemNames.Patch);
            AddIfExists(names, ItemNames.EnergyCell);
            AddIfExists(names, ItemNames.ScanPulse);
            AddUpgradeMaterials(names); // 강화 재료 추가
        }

        private static void AddLateItems(List<string> names, VirusMutation mutation)
        {
            AddIfExists(names, ItemNames.KernelLance);
            AddIfExists(names, ItemNames.ExploitReaper);
            AddIfExists(names, ItemNames.ZeroDayClaw);

            AddIfExists(names, ItemNames.RecoveryDaemon);
            AddIfExists(names, ItemNames.OverloadCell);

            AddMutationLateGear(names, mutation); // 현재 변이 전용 장비

            AddIfExists(names, ItemNames.Patch);
            AddIfExists(names, ItemNames.EnergyCell);
            AddIfExists(names, ItemNames.ScanPulse);
            AddUpgradeMaterials(names); // 강화 재료 추가
        }

        private static void AddUpgradeMaterials(List<string> names)
        {
            AddIfExists(names, ItemNames.MemoryShard); // ATK 강화 재료
            AddIfExists(names, ItemNames.CoreFragment); // HEALTH 강화 재료
            AddIfExists(names, ItemNames.EnergyCore); // ENERGY 강화 재료
        }

        private static void AddMutationMiddleGear(List<string> names, VirusMutation mutation)
        {
            if (mutation == VirusMutation.Ransomware) // 랜섬웨어 변이 체크
            {
                AddIfExists(names, ItemNames.CipherCore);
                AddIfExists(names, ItemNames.LockedVault);
            }
            else if (mutation == VirusMutation.Trojan) // 트로젠 변이 체크
            {
                AddIfExists(names, ItemNames.SpoofedCertificate);
                AddIfExists(names, ItemNames.BackdoorFrame);
            }
            else if (mutation == VirusMutation.Adware) // 애드웨어 변이 체크
            {
                AddIfExists(names, ItemNames.PopupEngine);
                AddIfExists(names, ItemNames.NotificationStack);
            }
        }

        private static void AddMutationLateGear(List<string> names, VirusMutation mutation)
        {
            if (mutation == VirusMutation.Ransomware) // 랜섬웨어 변이 체크
            {
                AddIfExists(names, ItemNames.RansomProtocol);
                AddIfExists(names, ItemNames.BlackmailArchive);
            }
            else if (mutation == VirusMutation.Trojan) // 트로젠 변이 체크
            {
                AddIfExists(names, ItemNames.GhostProcess);
                AddIfExists(names, ItemNames.AuthMask);
            }
            else if (mutation == VirusMutation.Adware) // 애드웨어 변이 체크
            {
                AddIfExists(names, ItemNames.BannerFarm);
                AddIfExists(names, ItemNames.SpamRouter);
            }
        }

        private static void AddIfExists(List<string> names, string itemName)
        {
            if (ItemDatabase.Get(itemName) != null && !names.Contains(itemName)) // 등록 아이템 체크
            {
                names.Add(itemName); // 후보 추가
            }
        }

        private static List<string> FilterByGrade(List<string> names, ItemGrade grade)
        {
            List<string> result = new List<string>(); // 등급 후보

            for (int i = 0; i < names.Count; i++) // 후보 순회
            {
                ItemData data = ItemDatabase.Get(names[i]); // 아이템 데이터
                if (data != null && data.Grade == grade) result.Add(names[i]); // 등급 일치 체크
            }

            return result; // 필터 결과
        }

        private static ItemGrade RollGrade(Random random, ItemPoolTier tier)
        {
            int roll = random.Next(0, 100); // 등급 주사위

            if (tier == ItemPoolTier.Early) // 초반 구간 체크
            {
                return roll < 85 ? ItemGrade.Common : ItemGrade.Rare; // COMMON 중심
            }

            if (tier == ItemPoolTier.Middle) // 중반 구간 체크
            {
                if (roll < 35) return ItemGrade.Common; // COMMON 일부
                if (roll < 90) return ItemGrade.Rare; // RARE 중심
                return ItemGrade.Elite; // ELITE 낮은 확률
            }

            if (roll < 35) return ItemGrade.Rare; // RARE 일부
            if (roll < 90) return ItemGrade.Elite; // ELITE 중심
            return ItemGrade.Legendary; // LEGENDARY 낮은 확률
        }
    }
}
