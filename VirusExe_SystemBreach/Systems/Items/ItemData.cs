using VirusExe.SystemBreach.Characters;

namespace VirusExe.SystemBreach.Systems
{
    // 아이템 원본 데이터
    // 타입/등급/가격/스탯/변이 제한 같은 아이템 속성 관리
    public class ItemData
    {
        public string Name { get; private set; } // 아이템 코드명
        public ItemType Type { get; private set; } // 아이템 분류
        public ItemGrade Grade { get; private set; } // 아이템 등급
        public VirusMutation RequiredMutation { get; private set; } // 전용 변이 조건
        public int AttackBonus { get; private set; } // ATK 증가량
        public int HealthBonus { get; private set; } // HEALTH 증가량
        public int EnergyBonus { get; private set; } // ENERGY 증가량
        public int Value { get; private set; } // 판매 기준가
        public string Description { get; private set; } // 설명 문구
        public string DisplayName { get { return GetDisplayTag() + " " + Name; } } // 태그 포함 표시명

        public ItemData(string name, ItemType type, ItemGrade grade, VirusMutation requiredMutation, int attackBonus, int healthBonus, int energyBonus, int value, string description)
        {
            Name = name; // 코드명 저장
            Type = type; // 분류 저장
            Grade = grade; // 등급 저장
            RequiredMutation = requiredMutation; // 전용 변이 저장
            AttackBonus = attackBonus; // ATK 보너스 저장
            HealthBonus = healthBonus; // HEALTH 보너스 저장
            EnergyBonus = energyBonus; // ENERGY 보너스 저장
            Value = value; // 판매 기준가 저장
            Description = description ?? string.Empty; // null 설명 방지
        }

        public bool IsEquipable()
        {
            return Type == ItemType.Weapon || Type == ItemType.Gear; // 장착 가능 여부
        }

        public bool CanEquip(VirusMutation mutation)
        {
            return RequiredMutation == VirusMutation.None || RequiredMutation == mutation; // 전용 조건 체크
        }

        public string GetDisplayTag()
        {
            if (Type == ItemType.Weapon) return "[WPN]"; // 무기 태그
            if (Type == ItemType.Consumable) return "[CONS]"; // 소비 태그
            if (Type == ItemType.Material) return "[MAT]"; // 재료 태그

            if (Type == ItemType.Gear) // 장비 태그 체크
            {
                if (RequiredMutation == VirusMutation.Ransomware) return "[R-GEAR]"; // 랜섬웨어 전용
                if (RequiredMutation == VirusMutation.Trojan) return "[T-GEAR]"; // 트로젠 전용
                if (RequiredMutation == VirusMutation.Adware) return "[A-GEAR]"; // 애드웨어 전용
                return "[GEAR]"; // 공용 장비
            }

            return "[DATA]"; // 알 수 없는 데이터
        }

        public string GetTypeLabel()
        {
            if (Type == ItemType.Weapon) return "WEAPON"; // 무기 표시
            if (Type == ItemType.Consumable) return "CONSUMABLE"; // 소비 표시
            if (Type == ItemType.Material) return "MATERIAL"; // 재료 표시

            if (Type == ItemType.Gear) // 장비 세부 표시
            {
                if (RequiredMutation == VirusMutation.Ransomware) return "RANSOMWARE GEAR"; // 랜섬웨어 전용 장비
                if (RequiredMutation == VirusMutation.Trojan) return "TROJAN GEAR"; // 트로젠 전용 장비
                if (RequiredMutation == VirusMutation.Adware) return "ADWARE GEAR"; // 애드웨어 전용 장비
                return "GEAR"; // 공용 장비
            }

            return "UNKNOWN"; // 알 수 없음
        }

        public string GetRequiredMutationLabel()
        {
            if (RequiredMutation == VirusMutation.Ransomware) return "RANSOMWARE.EXE"; // 랜섬웨어 조건
            if (RequiredMutation == VirusMutation.Trojan) return "TROJAN.EXE"; // 트로젠 조건
            if (RequiredMutation == VirusMutation.Adware) return "ADWARE.EXE"; // 애드웨어 조건
            return "NONE"; // 공용 아이템
        }
    }
}
