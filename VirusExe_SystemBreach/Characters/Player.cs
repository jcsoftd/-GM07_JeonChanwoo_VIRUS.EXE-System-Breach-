using VirusExe.SystemBreach.Systems;

namespace VirusExe.SystemBreach.Characters
{
    // 플레이어 상태 관리
    // 레벨/변이/장비/자원 처리는 여기서
    // 전투 중 바로 쓰는 HEALTH, ENERGY, KB도 여기서 관리
    public class Player : CombatEntity
    {
        
        // 플레이어 시작값 / 성장값
        
        private const int StartLevel = 1; // 시작 레벨
        private const int StartExpToNext = 60; // 첫 레벨업 필요 EXP
        private const int StartHealth = 120; // 시작 HEALTH
        private const int StartEnergyMax = 60; // 시작 최대 ENERGY
        private const int StartEnergy = 45; // 시작 ENERGY
        private const int StartAttack = 18; // 시작 ATK
        private const int StartKb = 80; // 시작 KB

        private const int ExpGrowthPerLevel = 35; // 레벨업마다 필요 EXP 증가량
        private const int HealthGainPerLevel = 12; // 레벨업 HEALTH 증가량
        private const int EnergyGainPerLevel = 4; // 레벨업 ENERGY 증가량
        private const int AttackGainPerLevel = 2; // 레벨업 ATK 증가량
        private const int MutationUnlockLevel = 2; // 변이 해금 레벨

        private const int MinMaxHealth = 30; // 최대 HEALTH 최소값
        private const int MinMaxEnergy = 10; // 최대 ENERGY 최소값
        private const int MinAttack = 1; // ATK 최소값

        
        // 기본 상태
        
        public override string Name { get; protected set; } // 표시 이름

        public int Level { get; private set; } // 현재 레벨
        public int Exp { get; private set; } // 현재 EXP
        public int ExpToNext { get; private set; } // 다음 레벨 필요 EXP

        public int MaxStability { get; private set; } // 최대 HEALTH
        public int Stability { get; private set; } // 현재 HEALTH

        public int MaxEnergy { get; private set; } // 최대 ENERGY
        public int Energy { get; private set; } // 현재 ENERGY

        public int Attack { get; private set; } // 현재 ATK
        public int Kb { get; private set; } // 보유 KB
        public int AccessLevel { get; private set; } // Kernel 접근 권한

        
        // 전투 / 인벤토리 / 변이 상태
        
        public bool StealthActive { get; set; } // 1회성 은신 방어
        public Inventory Inventory { get; private set; } // 보유 아이템 저장소

        public ItemData EquippedWeapon { get; private set; } // 장착 무기
        public ItemData EquippedGear { get; private set; } // 장착 장비

        public VirusMutation Mutation { get; private set; } // 현재 변이
        public bool PendingMutation { get; private set; } // GRID 복귀 시 변이 선택 대기
        public bool TrojanSpoofAuthActive { get; private set; } // 다음 BACKDOOR 강화
        public int NextDamageReductionPercent { get; private set; } // 다음 피격 피해 감소율

        public bool HasMutation { get { return Mutation != VirusMutation.None; } } // 변이 여부
        public override int CurrentHealth { get { return Stability; } } // 전투 공통 HEALTH
        public override int MaxHealth { get { return MaxStability; } } // 전투 공통 최대 HEALTH
        public override bool IsAlive { get { return Stability > 0; } } // 생존 체크

        public Player()
        {
            Name = "VIRUS.EXE"; // 기본 플레이어명
            Level = StartLevel;
            Exp = 0;
            ExpToNext = StartExpToNext;

            MaxStability = StartHealth; // HEALTH 초기화
            Stability = MaxStability; // 시작은 풀피

            MaxEnergy = StartEnergyMax; // ENERGY 초기화
            Energy = StartEnergy; // 시작 ENERGY는 최대치보다 살짝 낮게

            Attack = StartAttack;
            Kb = StartKb;
            AccessLevel = 0;

            StealthActive = false;
            Mutation = VirusMutation.None; // 시작은 기본 VIRUS
            PendingMutation = false;
            TrojanSpoofAuthActive = false;
            NextDamageReductionPercent = 0;

            Inventory = new Inventory();
            GiveStartItems(); // 시작 아이템 지급
        }

        private void GiveStartItems()
        {
            Inventory.Add(ItemNames.Patch, 2); // 시작 회복템
            Inventory.Add(ItemNames.EnergyCell, 1); // 시작 에너지템
            //Inventory.Add(ItemNames.ScanPulse, 1); // 시작 스캔템
            //Inventory.Add(ItemNames.BrokenInjector, 1); // 테스트 무기
            //Inventory.Add(ItemNames.CoreStabilizer, 1); // 테스트 장비
        }

        
        // HEALTH / ENERGY / KB 처리
        
        public override void TakeDamage(int damage)
        {
            if (damage < 0) 
                damage = 0; 

            Stability -= damage;

            if (Stability < 0) 
                Stability = 0; //0 고정
        }

        public void Heal(int amount)
        {
            Stability += amount; 

            if (Stability > MaxStability) 
                Stability = MaxStability; // 최대치 초과 방지
        }

        public void UseEnergy(int amount)
        {
            Energy -= amount; // ENERGY 소모
            if (Energy < 0) 
                Energy = 0; // 0 고정
        }

        public void RecoverEnergy(int amount)
        {
            Energy += amount; 

            if (Energy > MaxEnergy) 
                Energy = MaxEnergy; // 최대치 초과 방지
        }

        public bool HasEnergy(int amount)
        {
            return Energy >= amount; // 스킬 사용 가능 여부
        }

        public void AddKb(int amount)
        {
            Kb += amount; // KB 획득
        }

        public bool SpendKb(int amount)
        {
            if (Kb < amount) // 비용 부족
                return false;

            Kb -= amount; // KB 지불
            return true;
        }

        
        // EXP / 레벨업 / 강화
        
        public string AddExp(int amount)
        {
            Exp += amount; // 전투 보상 EXP 반영
            string message = string.Empty;

            while (Exp >= ExpToNext) // 한 번에 여러 레벨업도 처리
            {
                Exp -= ExpToNext;
                Level++;

                ExpToNext += ExpGrowthPerLevel; // 다음 레벨 요구량 증가
                MaxStability += HealthGainPerLevel;
                MaxEnergy += EnergyGainPerLevel;
                Attack += AttackGainPerLevel;

                Stability = MaxStability; // 레벨업 회복
                Energy = MaxEnergy;

                message += "레벨업! Lv." + Level + " / ATK+" + AttackGainPerLevel + " / HEALTH+" + HealthGainPerLevel + " / ENERGY+" + EnergyGainPerLevel + "\n";
            }

            if (Level >= MutationUnlockLevel && !HasMutation) // 변이 해금 조건
                PendingMutation = true; // 전투 중 바로 띄우지 않고 GRID 에서 처리

            return message;
        }

        public void IncreaseAttack(int amount)
        {
            Attack += amount; // Lab ATK 강화
            ClampCurrentStats();
        }

        public void IncreaseMaxStability(int amount)
        {
            MaxStability += amount; // Lab HEALTH 강화
            Stability += amount; // 현재 HEALTH 보정
            ClampCurrentStats();
        }

        public void IncreaseMaxEnergy(int amount)
        {
            MaxEnergy += amount; // Lab ENERGY 강화
            Energy += amount; // 현재 ENERGY 보정
            ClampCurrentStats();
        }

        
        // PAYLOAD MUTATION
        
        public bool ApplyMutation(VirusMutation mutation)
        {
            if (HasMutation) // 변이는 1회만 허용
                return false;

            if (mutation == VirusMutation.None) // None은 선택지 아님
                return false;

            Mutation = mutation;
            PendingMutation = false; // 변이 선택 완료

            if (mutation == VirusMutation.Ransomware)
            {
                Name = "RANSOMWARE.EXE";
                MaxStability += 40; 
                Stability += 40; // 증가분 즉시 회복
                Attack -= 4; //  ATK 감소
            }
            else if (mutation == VirusMutation.Trojan)
            {
                Name = "TROJAN.EXE";
                MaxStability -= 25; // 공격형이라 HEALTH 감소
                Attack += 6; 
            }
            else if (mutation == VirusMutation.Adware)
            {
                Name = "ADWARE.EXE";
                MaxStability += 20; 
                Stability += 20;
                Attack += 2;
            }

            ClampCurrentStats(); // 변이 후 최소/최대값 보정
            return true;
        }

        public void ApplyNextDamageReduction(int percent)
        {
            if (percent > NextDamageReductionPercent) // 더 강한 방어만 덮어씀
                NextDamageReductionPercent = percent;
        }

        public int ConsumeNextDamageReductionPercent()
        {
            int value = NextDamageReductionPercent;
            NextDamageReductionPercent = 0; // 1회성 방어값
            return value;
        }

        public void ActivateTrojanSpoofAuth()
        {
            TrojanSpoofAuthActive = true; // 다음 BACKDOOR 강화
        }

        public bool ConsumeTrojanSpoofAuth()
        {
            bool active = TrojanSpoofAuthActive;
            TrojanSpoofAuthActive = false; // 1회성 인증 위장
            return active;
        }

        
        // 장비 장착
        
        public bool EquipItem(string itemName, out string message)
        {
            ItemData data = ItemDatabase.Get(itemName); // 아이템 원본 조회

            if (data == null)
            {
                message = "알 수 없는 데이터입니다.";
                return false;
            }

            if (!data.IsEquipable()) // 소비템/재료는 장착 불가
            {
                message = "장착 가능한 데이터가 아닙니다.";
                return false;
            }

            if (!data.CanEquip(Mutation)) // 변이 전용 장비 체크
            {
                message = "GEAR LINK FAILED. 현재 PAYLOAD 구조와 호환되지 않습니다.";
                return false;
            }

            if (!Inventory.Remove(itemName, 1)) // 실제 보유 여부 체크
            {
                message = "DATA STORAGE에 존재하지 않습니다.";
                return false;
            }

            if (data.Type == ItemType.Weapon)
            {
                EquipWeapon(data);
                message = data.DisplayName + " 장착 완료. ATK +" + data.AttackBonus;
                return true;
            }

            EquipGear(data);
            message = data.DisplayName + " 장착 완료.";
            return true;
        }

        private void EquipWeapon(ItemData data)
        {
            if (EquippedWeapon != null)
            {
                RemoveEquipmentStats(EquippedWeapon); // 기존 무기 스탯 제거
                Inventory.Add(EquippedWeapon.Name, 1); // 기존 무기 회수
            }

            EquippedWeapon = data;
            ApplyEquipmentStats(data); // 새 무기 스탯 적용
        }

        private void EquipGear(ItemData data)
        {
            if (EquippedGear != null)
            {
                RemoveEquipmentStats(EquippedGear); // 기존 장비 스탯 제거
                Inventory.Add(EquippedGear.Name, 1); // 기존 장비 회수
            }

            EquippedGear = data;
            ApplyEquipmentStats(data); // 새 장비 스탯 적용
        }

        private void ApplyEquipmentStats(ItemData data)
        {
            Attack += data.AttackBonus; // 무기 ATK
            MaxStability += data.HealthBonus; // 장비 HEALTH
            MaxEnergy += data.EnergyBonus; // 장비 ENERGY

            Stability += data.HealthBonus; // 장착 즉시 증가분 반영
            Energy += data.EnergyBonus;

            ClampCurrentStats();
        }

        private void RemoveEquipmentStats(ItemData data)
        {
            Attack -= data.AttackBonus; // 무기 ATK 제거
            MaxStability -= data.HealthBonus; // 장비 HEALTH 제거
            MaxEnergy -= data.EnergyBonus; // 장비 ENERGY 제거

            ClampCurrentStats();
        }

        private void ClampCurrentStats()
        {
            if (MaxStability < MinMaxHealth) MaxStability = MinMaxHealth; // 최대 HEALTH 하한
            if (MaxEnergy < MinMaxEnergy) MaxEnergy = MinMaxEnergy; // 최대 ENERGY 하한

            if (Stability > MaxStability) Stability = MaxStability; // 현재 HEALTH 상한
            if (Stability < 1 && IsAlive) Stability = 1; // 사망 상태는 건드리지 않음

            if (Energy > MaxEnergy) Energy = MaxEnergy; // 현재 ENERGY 상한
            if (Attack < MinAttack) Attack = MinAttack; // ATK 하한
        }

        public void AddAccessLevel(int amount)
        {
            AccessLevel += amount; 
        }
    }
}
