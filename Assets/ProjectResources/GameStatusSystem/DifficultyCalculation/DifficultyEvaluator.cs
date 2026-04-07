using UnityEngine;
using GameStatusSystem.PlayerStatus;

namespace GameStatusSystem.DifficultyCalculation
{
    public static class DifficultyEvaluator
    {
        /// <summary>
        /// 核心评估方法
        /// </summary>
        /// <param name="snapshot">玩家状态快照</param>
        /// <param name="settings">难度配置</param>
        /// <returns>评估结果</returns>
        public static DifficultyEvaluation Evaluate(PlayerStatusSnapshot snapshot, DifficultySettings settings)
        {
            // 1. 计算分项评分（0~100）
            float powerScore = CalculatePlayerPowerScore(snapshot);
            float performanceScore = CalculatePerformanceScore(snapshot);
            float survivalScore = CalculateSurvivalScore(snapshot);

            // 2. 加权总分
            float totalScore =
                powerScore * settings.weightPower +
                performanceScore * settings.weightPerformance +
                survivalScore * settings.weightSurvival;

            totalScore = Mathf.Clamp(totalScore, 0, 100);

            // 3. 总分 → 难度系数
            float recommendedDiff = MapScoreToDifficulty(totalScore, settings);

            // 4. 返回结果
            return new DifficultyEvaluation
            {
                RecommendedDifficulty = recommendedDiff,
                PlayerPowerScore = powerScore,
                PerformanceScore = performanceScore,
                SurvivalScore = survivalScore,
                TotalScore = totalScore
            };
        }

        #region 1. 玩家基础强度评分（等级+装备+血量）
        private static float CalculatePlayerPowerScore(PlayerStatusSnapshot snapshot)
        {
            // 等级贡献 50%
            // float levelScore = Mathf.Clamp01(snapshot.Level / 50f) * 50f;

            
            float hpScore = snapshot.MaxHealth * .1f;

            // 装备贡献 20%
            float equipScore = Mathf.Clamp01(snapshot.EquipmentScore / 500f) * 20f;

            float score = hpScore + equipScore;
            return Mathf.Clamp(score, 0, 100);
        }
        #endregion

        #region 2. 操作表现评分（命中率 + 连杀）
        private static float CalculatePerformanceScore(PlayerStatusSnapshot snapshot)
        {
            // 命中率 30%
            float hitScore = snapshot.HitRate * 30f;

            // 连杀 40%
            float killScore = Mathf.Clamp01(snapshot.KillStreak / 30f) * 40f;

            // 击杀得分 30%
            float killScoreScore = snapshot.TotalScore * .03f;
            // 总得分
            float score = hitScore + killScore + killScoreScore;
            return Mathf.Clamp(score, 0, 100);
        }
        #endregion

        #region 3. 生存压力评分（死亡次数越少分数越高）
        private static float CalculateSurvivalScore(PlayerStatusSnapshot snapshot)
        {
            // 生存压力等于100 * 损失生命值百分比
            float survStress = (snapshot.MaxHealth - snapshot.CurrentHealth)/snapshot.MaxHealth * 100f;
            float survivalScore = 100 - survStress;
            return Mathf.Clamp(survivalScore, 0, 100);
        }
        #endregion

        #region 4. 分数 → 难度系数（线性映射）
        private static float MapScoreToDifficulty(float score, DifficultySettings settings)
        {
            float t = Mathf.InverseLerp(0, 100, score);
            return Mathf.Lerp(settings.minDifficulty, settings.maxDifficulty, t);
        }
        #endregion
    }
}