using System.Collections.Generic;

namespace VirusExe.SystemBreach.Systems
{
    // 보상 결과 데이터
    // 전투 종료 후 화면에 보여줄 EXP/KB/아이템 결과 묶음
    public class RewardResult
    {
        public int Exp { get; private set; } // 획득 EXP
        public int Kb { get; private set; } // 획득 KB
        public string ItemName { get; private set; } // 획득 아이템 코드명
        public string LevelUpMessage { get; private set; } // 레벨업 메시지

        public bool HasExp { get { return Exp > 0; } } // EXP 보상 여부
        public bool HasKb { get { return Kb > 0; } } // KB 보상 여부
        public bool HasItem { get { return !string.IsNullOrEmpty(ItemName); } } // 아이템 보상 여부
        public bool HasLevelUp { get { return !string.IsNullOrEmpty(LevelUpMessage); } } // 레벨업 여부
        public bool HasAnyReward { get { return HasExp || HasKb || HasItem || HasLevelUp; } } // 보상 존재 여부

        public RewardResult(int exp, int kb, string itemName, string levelUpMessage)
        {
            Exp = exp; // EXP 저장
            Kb = kb; // KB 저장
            ItemName = itemName; // 아이템 저장
            LevelUpMessage = levelUpMessage ?? string.Empty; // null 방지
        }

        public static RewardResult Empty()
        {
            return new RewardResult(0, 0, null, string.Empty); // 빈 보상
        }

        public static RewardResult ItemOnly(string itemName)
        {
            return new RewardResult(0, 0, itemName, string.Empty); // 아이템 단독 보상
        }

        public string GetItemRewardText()
        {
            if (!HasItem) return string.Empty; // 아이템 없음 체크
            return ItemDatabase.GetDisplayName(ItemName) + " x1"; // 아이템 보상 문구
        }

        public string GetBattleDataText()
        {
            List<string> parts = new List<string>(); // 기본 보상 조각

            if (HasExp) parts.Add("EXP +" + Exp); // EXP 표시
            if (HasKb) parts.Add("KB +" + Kb + "KB"); // KB 표시

            if (parts.Count <= 0) return "-"; // 기본 보상 없음
            return string.Join(" / ", parts.ToArray()); // 기본 보상 문구
        }

        public string GetFieldMessage()
        {
            List<string> parts = new List<string>(); // 필드 메시지 조각

            if (HasExp) parts.Add("EXP +" + Exp); // EXP 표시
            if (HasKb) parts.Add("KB +" + Kb + "KB"); // KB 표시
            if (HasItem) parts.Add("DROP " + GetItemRewardText()); // 아이템 표시
            if (HasLevelUp) parts.Add(LevelUpMessage.Trim()); // 레벨업 표시

            if (parts.Count <= 0) return string.Empty; // 표시 없음
            return string.Join(" / ", parts.ToArray()); // 필드 메시지
        }
    }
}
