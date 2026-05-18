using VirusExe.SystemBreach.Systems;

namespace VirusExe.SystemBreach.Characters
{
    // 보안 프로세스 / 엘리트 / 보스 공통 데이터
    // 일반/엘리트/보스는 IsElite, IsBoss로 구분
    // 변이 스킬에서 쓰는 암호화/팝업/알림 상태도 여기서 관리
    public class Enemy : CombatEntity
    {
        public override string Name { get; protected set; } // 적 이름
        public int MaxHp { get; private set; } // 적 최대 HP
        public int Hp { get; private set; } // 적 현재 HP
        public int AttackMin { get; private set; } // 적 최소 ATK
        public int AttackMax { get; private set; } // 적 최대 ATK
        public int ExpReward { get; private set; } // 처치 시 지급 EXP
        public int KbMin { get; private set; } // 처치 시 지급 최소 KB
        public int KbMax { get; private set; } // 처치 시 지급 최대 KB
        public bool IsBoss { get; private set; } // 보스 여부
        public bool IsElite { get; private set; } // 엘리트 여부
        public bool IsEncrypted { get; private set; } // 랜섬웨어 암호화 상태
        public int EncryptionNoiseLevel { get; private set; } // $ 오염 강도
        public bool PopupOverlayActive { get; private set; } // 팝업 오염 표시
        public int AdNotificationStacks { get; private set; } // 알림 상태이상 중첩
        public int NextAttackDamageReductionPercent { get; private set; } // 다음 공격 피해 감소율

        public override int CurrentHealth { get { return Hp; } } // 현재 HEALTH
        public override int MaxHealth { get { return MaxHp; } } // 최대 HEALTH
        public override bool IsAlive { get { return Hp > 0; } } // 생존 체크

        public Enemy(string name, int hp, int attackMin, int attackMax, int expReward, int kbMin, int kbMax, bool isElite, bool isBoss)
        {
            Name = name; // 적 이름
            MaxHp = hp; // 최대 HP
            Hp = hp; // 현재 HP
            AttackMin = attackMin; // 최소 ATK
            AttackMax = attackMax; // 최대 ATK
            ExpReward = expReward; // EXP 보상
            KbMin = kbMin; // 최소 KB 보상
            KbMax = kbMax; // 최대 KB 보상
            IsElite = isElite; // 엘리트 여부
            IsBoss = isBoss; // 보스 여부
            IsEncrypted = false; // 암호화 해제
            EncryptionNoiseLevel = 0; // 오염 없음
            PopupOverlayActive = false; // 팝업 없음
            AdNotificationStacks = 0; // 알림 중첩 없음
            NextAttackDamageReductionPercent = 0; // 피해 감소 없음
        }

        public override void TakeDamage(int damage)
        {
            if (damage < 0) // 음수 피해 체크
                damage = 0;

            Hp -= damage; 

            if (Hp < 0) // HP 하한 체크
                Hp = 0;
        }

        public void RestoreHealthFull()
        {
            Hp = MaxHp; // 현재 HP 최대치 복구
        }

        public void ApplyEncryption(int noiseLevel)
        {
            IsEncrypted = true; // 암호화 상태 적용
            EncryptionNoiseLevel = noiseLevel; // $ 오염 강도 저장
        }

        public void ClearEncryption()
        {
            IsEncrypted = false; // 암호화 해제
            EncryptionNoiseLevel = 0; // $ 오염 제거
        }

        public void ActivatePopupOverlay()
        {
            PopupOverlayActive = true; // 팝업 오염 표시
        }

        public void AddAdNotificationStack()
        {
            if (AdNotificationStacks < SkillBalanceData.AdNotificationMaxStacks) // 최대 중첩 체크
                AdNotificationStacks++; // 알림 중첩 증가
        }

        public void ReduceMaxHealth(int amount, int minimum)
        {
            int targetMax = MaxHp - amount; // 감소 후 최대 HP

            if (targetMax < minimum) // 최소 최대 HP 체크
                targetMax = minimum;

            MaxHp = targetMax; // 최대 HP 적용

            if (Hp > MaxHp) // 현재 HP 상한 체크
                Hp = MaxHp;
        }

        public void ApplyNextAttackDamageReduction(int percent)
        {
            if (percent > NextAttackDamageReductionPercent) // 더 큰 감소율 체크
                NextAttackDamageReductionPercent = percent; // 감소율 저장
        }

        public int ConsumeNextAttackDamageReductionPercent()
        {
            int value = NextAttackDamageReductionPercent; // 감소율 저장
            NextAttackDamageReductionPercent = 0; // 1회 사용 후 해제
            return value;
        }
    }
}
