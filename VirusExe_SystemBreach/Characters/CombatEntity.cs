namespace VirusExe.SystemBreach.Characters
{
    // 전투 대상 공통 부모
    // Player와 Enemy가 전투에서 공통으로 쓰는 최소 규격
    public abstract class CombatEntity
    {
        public abstract string Name { get; protected set; } // 이름
        public abstract int CurrentHealth { get; } // 현재 체력
        public abstract int MaxHealth { get; } // 최대 체력
        public abstract bool IsAlive { get; } // 생존 체크
        public abstract void TakeDamage(int damage); // 피해 처리
    }
}
