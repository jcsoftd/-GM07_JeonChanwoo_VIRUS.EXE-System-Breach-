namespace VirusExe.SystemBreach.Systems
{
    // 전투 밸런스 공통 수치
    // 실제 전투 계산과 화면 설명 문구가 같은 값을 보도록 관리
    public static class SkillBalanceData
    {
        public const int DamageRollMinPercent = 80; // 기본 피해 최소
        public const int DamageRollMaxPercent = 120; // 기본 피해 최대

        public const int CommonCriticalChance = 20; // 공통 치명타 확률
        public const int CommonCriticalMultiplierPercent = 150; // 공통 치명타 피해
        public const int TrojanCriticalChance = 35; // 트로젠 치명타 확률
        public const int TrojanCriticalMultiplierPercent = 200; // 트로젠 치명타 피해

        public const int DefaultAttackMultiplierPercent = 100; // 기본 공격 배율
        public const int DefaultSkillMultiplierPercent = 135; // 기본 강공격 배율
        public const int DefaultSkillEnergyCost = 12; // 기본 강공격 ENERGY

        public const int RansomwareEncryptEnergyCost = 10; // ENCRYPT ENERGY
        public const int RansomwareEncryptMultiplierPercent = 90; // ENCRYPT 피해 배율
        public const int RansomwareEncryptAttackReductionPercent = 35; // ENCRYPT 공격 약화율
        public const int RansomwareNoteEnergyCost = 16; // RANSOM_NOTE ENERGY
        public const int RansomwareNoteMultiplierPercent = 160; // RANSOM_NOTE 피해 배율
        public const int RansomwareNoteHealPercent = 20; // RANSOM_NOTE 흡혈률
        public const int RansomwareNoteKbGain = 20; // RANSOM_NOTE KB 보상

        public const int TrojanBackdoorEnergyCost = 10; // BACKDOOR ENERGY
        public const int TrojanBackdoorMultiplierPercent = 125; // BACKDOOR 피해 배율
        public const int TrojanSpoofAuthEnergyCost = 15; // SPOOF_AUTH ENERGY
        public const int TrojanSpoofBackdoorMultiplierPercent = 160; // SPOOF_AUTH 후 BACKDOOR 배율
        public const int TrojanSpoofCriticalBonusPercent = 30; // SPOOF_AUTH 후 치명타 보너스

        public const int AdwarePopupFloodEnergyCost = 10; // POPUP_FLOOD ENERGY
        public const int AdwarePopupFloodMultiplierPercent = 75; // POPUP_FLOOD 피해 배율
        public const int AdwarePopupFloodAttackReductionPercent = 50; // POPUP_FLOOD 공격 약화율
        public const int AdNotificationEnergyCost = 14; // AD_NOTIFICATION ENERGY
        public const int AdNotificationMultiplierPercent = 20; // AD_NOTIFICATION 즉시 피해 배율
        public const int AdNotificationTickPercent = 40; // AD_NOTIFICATION 1중첩 지속 피해
        public const int AdNotificationMaxStacks = 3; // AD_NOTIFICATION 최대 중첩

        public static string FormatMultiplier(int multiplierPercent)
        {
            if (multiplierPercent % 100 == 0) return (multiplierPercent / 100).ToString("0"); // 정수 배율
            return (multiplierPercent / 100.0).ToString("0.00").TrimEnd('0').TrimEnd('.'); // 소수 배율
        }
    }
}
