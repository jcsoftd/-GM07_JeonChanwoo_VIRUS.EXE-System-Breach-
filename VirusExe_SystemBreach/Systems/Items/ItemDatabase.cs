using System;
using System.Collections.Generic;
using VirusExe.SystemBreach.Characters;

namespace VirusExe.SystemBreach.Systems
{
    // 아이템 데이터베이스
    // 전체 아이템 등록과 이름 기준 조회
    public static class ItemDatabase
    {
        private static readonly Dictionary<string, ItemData> items = new Dictionary<string, ItemData>(); // 아이템 원본 데이터

        static ItemDatabase()
        {
            RegisterConsumables(); // 소비 아이템 등록
            RegisterMaterials(); // 강화 재료 등록
            RegisterWeapons(); // 공용 무기 등록
            RegisterCommonGear(); // 공용 장비 등록
            RegisterMutationGear(); // 변이 전용 장비 등록
        }

        private static void RegisterConsumables()
        {
            Register(new ItemData(ItemNames.Patch, ItemType.Consumable, ItemGrade.Common, VirusMutation.None, 0, 0, 0, 16, "손상된 바이러스 코드를 복구하는 패치 데이터."));
            Register(new ItemData(ItemNames.EnergyCell, ItemType.Consumable, ItemGrade.Common, VirusMutation.None, 0, 0, 0, 12, "즉시 실행 ENERGY를 보충하는 압축 셀."));
            Register(new ItemData(ItemNames.ScanPulse, ItemType.Consumable, ItemGrade.Rare, VirusMutation.None, 0, 0, 0, 24, "가장 멀리 장악한 열 기준 전방 2열의 노드 정보를 공개하는 탐색 펄스."));
        }

        private static void RegisterMaterials()
        {
            Register(new ItemData(ItemNames.MemoryShard, ItemType.Material, ItemGrade.Common, VirusMutation.None, 0, 0, 0, 80, "ATK 강화에 사용하는 손상된 메모리 조각."));
            Register(new ItemData(ItemNames.CoreFragment, ItemType.Material, ItemGrade.Common, VirusMutation.None, 0, 0, 0, 80, "HEALTH 강화에 사용하는 코어 파편."));
            Register(new ItemData(ItemNames.EnergyCore, ItemType.Material, ItemGrade.Common, VirusMutation.None, 0, 0, 0, 80, "ENERGY 강화에 사용하는 압축 에너지 코어."));
        }

        private static void RegisterWeapons()
        {
            Register(new ItemData(ItemNames.BrokenInjector, ItemType.Weapon, ItemGrade.Common, VirusMutation.None, 2, 0, 0, 16, "손상된 삽입 도구. 약한 침투 패킷을 주입한다."));
            Register(new ItemData(ItemNames.PacketSpike, ItemType.Weapon, ItemGrade.Common, VirusMutation.None, 3, 0, 0, 22, "날카롭게 압축된 공격 패킷. 기본 피해를 조금 높인다."));
            Register(new ItemData(ItemNames.MemoryShiv, ItemType.Weapon, ItemGrade.Common, VirusMutation.None, 4, 0, 0, 28, "메모리 틈새를 찌르는 가벼운 코드 조각."));
            Register(new ItemData(ItemNames.MemoryBlade, ItemType.Weapon, ItemGrade.Rare, VirusMutation.None, 6, 0, 0, 44, "메모리 블록을 절단하는 공격 루틴."));
            Register(new ItemData(ItemNames.PayloadDriver, ItemType.Weapon, ItemGrade.Rare, VirusMutation.None, 8, 0, 0, 60, "페이로드를 강제로 밀어 넣는 침투 드라이버."));
            Register(new ItemData(ItemNames.RootkitNeedle, ItemType.Weapon, ItemGrade.Rare, VirusMutation.None, 10, 0, 0, 76, "권한 계층을 찌르는 루트킷 주입 장치."));
            Register(new ItemData(ItemNames.KernelLance, ItemType.Weapon, ItemGrade.Elite, VirusMutation.None, 13, 0, 0, 100, "커널 방어층을 관통하는 고출력 공격 모듈."));
            Register(new ItemData(ItemNames.ExploitReaper, ItemType.Weapon, ItemGrade.Elite, VirusMutation.None, 16, 0, 0, 130, "취약점을 수확하듯 잘라내는 실행 파일."));
            Register(new ItemData(ItemNames.ZeroDayClaw, ItemType.Weapon, ItemGrade.Legendary, VirusMutation.None, 20, 0, 0, 170, "알려지지 않은 취약점을 물어뜯는 최상급 침투 무기."));
        }

        private static void RegisterCommonGear()
        {
            Register(new ItemData(ItemNames.CrackedFirewall, ItemType.Gear, ItemGrade.Common, VirusMutation.None, 0, 15, 0, 18, "금이 간 방화벽 조각. 약간의 안정성을 제공한다."));
            Register(new ItemData(ItemNames.DebugShield, ItemType.Gear, ItemGrade.Common, VirusMutation.None, 0, 20, 0, 24, "오류 추적용 보호막. 치명적 손상을 조금 막아준다."));
            Register(new ItemData(ItemNames.ProcessPadding, ItemType.Gear, ItemGrade.Common, VirusMutation.None, 0, 25, 0, 30, "프로세스 외곽에 덧씌운 임시 보호층."));
            Register(new ItemData(ItemNames.CoreStabilizer, ItemType.Gear, ItemGrade.Rare, VirusMutation.None, 0, 35, 0, 48, "바이러스 코어의 안정성을 높이는 모듈."));
            Register(new ItemData(ItemNames.ProcessArmor, ItemType.Gear, ItemGrade.Rare, VirusMutation.None, 0, 45, 0, 64, "실행 프로세스를 보호하는 방어 계층."));
            Register(new ItemData(ItemNames.RecoveryDaemon, ItemType.Gear, ItemGrade.Elite, VirusMutation.None, 0, 55, 0, 82, "손상된 코드를 복구하는 보조 데몬."));
            Register(new ItemData(ItemNames.LowVoltCell, ItemType.Gear, ItemGrade.Common, VirusMutation.None, 0, 0, 8, 16, "불안정하지만 즉시 사용할 수 있는 저전압 셀."));
            Register(new ItemData(ItemNames.SignalBuffer, ItemType.Gear, ItemGrade.Common, VirusMutation.None, 0, 0, 10, 22, "신호 처리량을 잠시 확보하는 버퍼."));
            Register(new ItemData(ItemNames.CacheCell, ItemType.Gear, ItemGrade.Common, VirusMutation.None, 0, 0, 12, 28, "임시 실행 ENERGY를 저장하는 작은 캐시."));
            Register(new ItemData(ItemNames.EnergyCache, ItemType.Gear, ItemGrade.Rare, VirusMutation.None, 0, 0, 18, 46, "압축된 실행 ENERGY를 저장하는 캐시."));
            Register(new ItemData(ItemNames.ThreadBattery, ItemType.Gear, ItemGrade.Rare, VirusMutation.None, 0, 0, 24, 62, "병렬 스레드 실행을 보조하는 전력 모듈."));
            Register(new ItemData(ItemNames.OverloadCell, ItemType.Gear, ItemGrade.Elite, VirusMutation.None, 0, 0, 30, 80, "순간적인 과부하 실행을 버티는 고밀도 셀."));
        }

        private static void RegisterMutationGear()
        {
            Register(new ItemData(ItemNames.CipherCore, ItemType.Gear, ItemGrade.Rare, VirusMutation.Ransomware, 0, 45, 8, 90, "암호화 루틴을 안정적으로 유지하는 랜섬웨어 전용 코어."));
            Register(new ItemData(ItemNames.LockedVault, ItemType.Gear, ItemGrade.Rare, VirusMutation.Ransomware, 0, 70, 0, 110, "탈취한 데이터를 금고화해 바이러스 코어를 보호하는 장비."));
            Register(new ItemData(ItemNames.RansomProtocol, ItemType.Gear, ItemGrade.Elite, VirusMutation.Ransomware, 0, 55, 15, 125, "협박 프로토콜을 유지하기 위한 전용 실행 프레임."));
            Register(new ItemData(ItemNames.BlackmailArchive, ItemType.Gear, ItemGrade.Elite, VirusMutation.Ransomware, 0, 35, 25, 130, "인질화된 데이터 조각을 저장하는 압축 아카이브."));

            Register(new ItemData(ItemNames.SpoofedCertificate, ItemType.Gear, ItemGrade.Rare, VirusMutation.Trojan, 2, 0, 18, 95, "정상 인증서처럼 위장해 침투 효율을 높이는 장비."));
            Register(new ItemData(ItemNames.BackdoorFrame, ItemType.Gear, ItemGrade.Rare, VirusMutation.Trojan, 3, 25, 0, 120, "백도어 경로를 안정적으로 유지하는 트로이 목마 전용 프레임."));
            Register(new ItemData(ItemNames.GhostProcess, ItemType.Gear, ItemGrade.Elite, VirusMutation.Trojan, 0, 0, 40, 115, "탐지되지 않는 유령 프로세스로 위장해 실행 자원을 확보한다."));
            Register(new ItemData(ItemNames.AuthMask, ItemType.Gear, ItemGrade.Elite, VirusMutation.Trojan, 2, 35, 12, 140, "권한 검증을 속이기 위한 인증 위장 모듈."));

            Register(new ItemData(ItemNames.PopupEngine, ItemType.Gear, ItemGrade.Rare, VirusMutation.Adware, 0, 20, 22, 95, "팝업 오염을 계속 생성하는 애드웨어 전용 엔진."));
            Register(new ItemData(ItemNames.NotificationStack, ItemType.Gear, ItemGrade.Rare, VirusMutation.Adware, 0, 0, 38, 105, "알림 패킷을 대량으로 쌓아두는 실행 스택."));
            Register(new ItemData(ItemNames.BannerFarm, ItemType.Gear, ItemGrade.Elite, VirusMutation.Adware, 0, 30, 18, 115, "광고 배너를 증식시켜 대상 신호를 방해하는 장비."));
            Register(new ItemData(ItemNames.SpamRouter, ItemType.Gear, ItemGrade.Elite, VirusMutation.Adware, 0, 25, 30, 130, "스팸 패킷을 우회 전송해 방해 루틴을 유지한다."));
        }

        public static void Register(ItemData data)
        {
            items[data.Name] = data; // 이름 기준으로 데이터 등록
        }

        public static ItemData Get(string itemName)
        {
            if (string.IsNullOrEmpty(itemName)) return null; // 빈 이름 방지
            if (!items.ContainsKey(itemName)) return null; // 미등록 아이템 체크
            return items[itemName];
        }

        public static bool IsEquipable(string itemName)
        {
            ItemData data = Get(itemName); // 아이템 데이터 조회
            return data != null && data.IsEquipable(); // 장착 가능 여부
        }

        public static string GetDisplayName(string itemName)
        {
            ItemData data = Get(itemName); // 아이템 데이터 조회
            return data == null ? itemName : data.DisplayName; // 태그 포함 이름
        }

        public static List<ItemData> GetAllItems()
        {
            return new List<ItemData>(items.Values); // 전체 원본 데이터 복사
        }

        public static int CompareItemNames(string a, string b)
        {
            ItemData left = Get(a); // 왼쪽 아이템
            ItemData right = Get(b); // 오른쪽 아이템
            int leftOrder = GetSortOrder(left); // 왼쪽 정렬값
            int rightOrder = GetSortOrder(right); // 오른쪽 정렬값

            if (leftOrder != rightOrder) return leftOrder.CompareTo(rightOrder); // 타입 우선
            if (left != null && right != null && left.Grade != right.Grade) return left.Grade.CompareTo(right.Grade); // 등급 정렬
            return string.Compare(a, b, StringComparison.Ordinal); // 이름 정렬
        }

        private static int GetSortOrder(ItemData data)
        {
            if (data == null) return 9; // 미등록 후순위
            if (data.Type == ItemType.Weapon) return 0; // 무기 우선
            if (data.Type == ItemType.Gear) return 1; // 장비 다음
            if (data.Type == ItemType.Consumable) return 2; // 소비 다음
            if (data.Type == ItemType.Material) return 3; // 재료 다음
            return 9; // 기본 후순위
        }

        public static string GetRandomEquipmentName(Random random)
        {
            string itemName = ItemPoolManager.RollNodeRewardName(random, 1, VirusMutation.None); // 기존 호출 호환용
            return string.IsNullOrEmpty(itemName) ? ItemNames.BrokenInjector : itemName; // 기본 무기 보정
        }
    }
}
