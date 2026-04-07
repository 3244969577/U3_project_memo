
namespace GameStatusSystem.DifficultyCalculation
{
    /// <summary>
    /// 难度评估层输出结果
    /// </summary>
    public struct DifficultyEvaluation
    {
        /// <summary>
        /// 推荐难度系数 0.5~1.5
        /// </summary>
        public float RecommendedDifficulty;

        // 分项评分（用于调试）
        public float PlayerPowerScore;    // 玩家实力
        public float PerformanceScore;    // 操作表现
        public float SurvivalScore;       // 生存压力
        public float TotalScore;          // 总分 0~100
    }
}