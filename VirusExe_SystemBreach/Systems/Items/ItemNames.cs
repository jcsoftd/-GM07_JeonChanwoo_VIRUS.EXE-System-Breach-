namespace VirusExe.SystemBreach.Systems
{
    // 아이템 이름 상수 모음
    // 문자열 오타를 줄이기 위해 아이템 코드를 한 곳에 모음
    public static class ItemNames
    {
        public const string Patch = "PATCH_32KB"; // HEALTH 회복 아이템
        public const string EnergyCell = "ENERGY_CELL_24KB"; // ENERGY 회복 아이템
        public const string ScanPulse = "SCAN_PULSE"; // 노드 스캔 아이템

        public const string MemoryShard = "MEMORY_SHARD"; // ATK 강화 재료
        public const string CoreFragment = "CORE_FRAGMENT"; // HEALTH 강화 재료
        public const string EnergyCore = "ENERGY_CORE"; // ENERGY 강화 재료

        public const string BrokenInjector = "BROKEN_INJECTOR"; // 초반 무기
        public const string PacketSpike = "PACKET_SPIKE"; // 초반 무기
        public const string MemoryShiv = "MEMORY_SHIV"; // 초반 무기
        public const string MemoryBlade = "MEMORY_BLADE"; // 중반 무기
        public const string PayloadDriver = "PAYLOAD_DRIVER"; // 중반 무기
        public const string RootkitNeedle = "ROOTKIT_NEEDLE"; // 중반 무기
        public const string KernelLance = "KERNEL_LANCE"; // 후반 무기
        public const string ExploitReaper = "EXPLOIT_REAPER"; // 후반 무기
        public const string ZeroDayClaw = "ZERO_DAY_CLAW"; // 최상급 무기

        public const string CrackedFirewall = "CRACKED_FIREWALL"; // 공용 HEALTH 장비
        public const string DebugShield = "DEBUG_SHIELD"; // 공용 HEALTH 장비
        public const string ProcessPadding = "PROCESS_PADDING"; // 공용 HEALTH 장비
        public const string CoreStabilizer = "CORE_STABILIZER"; // 공용 HEALTH 장비
        public const string ProcessArmor = "PROCESS_ARMOR"; // 공용 HEALTH 장비
        public const string RecoveryDaemon = "RECOVERY_DAEMON"; // 공용 HEALTH 장비
        public const string LowVoltCell = "LOW_VOLT_CELL"; // 공용 ENERGY 장비
        public const string SignalBuffer = "SIGNAL_BUFFER"; // 공용 ENERGY 장비
        public const string CacheCell = "CACHE_CELL"; // 공용 ENERGY 장비
        public const string EnergyCache = "ENERGY_CACHE"; // 공용 ENERGY 장비
        public const string ThreadBattery = "THREAD_BATTERY"; // 공용 ENERGY 장비
        public const string OverloadCell = "OVERLOAD_CELL"; // 공용 ENERGY 장비

        public const string BatteryPack = LowVoltCell; // 구버전 이름 호환

        public const string CipherCore = "CIPHER_CORE"; // 랜섬웨어 전용 장비
        public const string LockedVault = "LOCKED_VAULT"; // 랜섬웨어 전용 장비
        public const string RansomProtocol = "RANSOM_PROTOCOL"; // 랜섬웨어 전용 장비
        public const string BlackmailArchive = "BLACKMAIL_ARCHIVE"; // 랜섬웨어 전용 장비
        public const string SpoofedCertificate = "SPOOFED_CERTIFICATE"; // 트로젠 전용 장비
        public const string BackdoorFrame = "BACKDOOR_FRAME"; // 트로젠 전용 장비
        public const string GhostProcess = "GHOST_PROCESS"; // 트로젠 전용 장비
        public const string AuthMask = "AUTH_MASK"; // 트로젠 전용 장비
        public const string PopupEngine = "POPUP_ENGINE"; // 애드웨어 전용 장비
        public const string NotificationStack = "NOTIFICATION_STACK"; // 애드웨어 전용 장비
        public const string BannerFarm = "BANNER_FARM"; // 애드웨어 전용 장비
        public const string SpamRouter = "SPAM_ROUTER"; // 애드웨어 전용 장비
    }
}
