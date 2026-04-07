namespace GameStatusSystem.PlayerStatus
{
    public struct PlayerStatusSnapshot
    {
        // Core
        // public int Level;
        public float MaxHealth;
        public float EquipmentScore;

        // Combat
        public float CurrentHealth;
        public int KillStreak;
        // public int TotalDeaths;
        public int TotalScore;

        // StatsRecorder
        public float HitRate;
    }
}