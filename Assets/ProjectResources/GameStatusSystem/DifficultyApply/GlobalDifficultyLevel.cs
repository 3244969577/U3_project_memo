using UnityEngine;


/// <summary>
/// 第四层：全局难度参数静态类
/// 所有敌人生成、敌人属性 都从这里读取
/// 由第三层 DifficultySystem 自动更新
/// </summary>
// public static class GlobalDifficultyLevel
// {
//     // 最终难度系数（来自第三层 DifficultySystem）
//     public static float CurrentDifficulty { get; private set; } = 1.0f;

//     // ====================== 敌人生成参数 ======================
//     // 基础数值（策划配置用，你也可以做成可配置SO）
//     private const float BASE_ENEMY_HEALTH = 100f;
//     private const float BASE_ENEMY_DAMAGE = 10f;
//     private const float BASE_ENEMY_SPEED = 2f;
//     private const float BASE_SPAWN_INTERVAL = 2f;
//     private const int BASE_MAX_ENEMY_COUNT = 8;

//     // ====================== 动态计算后的最终属性 ======================
//     public static float EnemyHealth => BASE_ENEMY_HEALTH * CurrentDifficulty;
//     public static float EnemyDamage => BASE_ENEMY_DAMAGE * Mathf.Pow(CurrentDifficulty, 0.7f); // 攻击增长放缓
//     public static float EnemyMoveSpeed => BASE_ENEMY_SPEED * (1 + (CurrentDifficulty - 1) * 0.3f);
//     public static float SpawnInterval => BASE_SPAWN_INTERVAL / CurrentDifficulty; // 难度越高刷越快
//     public static int MaxEnemyCount => Mathf.RoundToInt(BASE_MAX_ENEMY_COUNT * CurrentDifficulty);

//     // ====================== 由第三层调用更新 ======================
//     public static void UpdateDifficulty(float newDifficulty)
//     {
//         CurrentDifficulty = newDifficulty;
//     }
// }

namespace GameStatusSystem.DifficultyApply
{
    public static class GlobalDifficultyLevel
    {
        // 敌人生成参数
        private static float _enemyHealthMultiplier = 1f;
        private static float _enemyDamageMultiplier = 1f;
        private static float _enemySpeedMultiplier = 1f;
        private static float _enemySpawnRate = 1f;
        private static int _maxEnemies = 2;
        private static float _enemyScoreMultiplier = 1f;

        // 当前难度等级
        private static float _currentDifficultyLevel = 1f;

        // 公共属性访问器
        public static float EnemyHealthMultiplier => _enemyHealthMultiplier;
        public static float EnemyDamageMultiplier => _enemyDamageMultiplier;
        public static float EnemySpeedMultiplier => _enemySpeedMultiplier;
        public static float EnemySpawnRate => _enemySpawnRate;
        public static int MaxEnemies => _maxEnemies;
        public static float EnemyScoreMultiplier => _enemyScoreMultiplier;
        public static float CurrentDifficultyLevel => _currentDifficultyLevel;

        /// <summary>
        /// 根据难度等级更新敌人参数
        /// </summary>
        /// <param name="difficultyLevel">难度等级</param>
        public static void UpdateDifficulty(float difficultyLevel)
        {
            _currentDifficultyLevel = difficultyLevel;

            // 根据难度等级计算参数
            float difficultyMultiplier = 1f + (difficultyLevel - 1) * 0.15f;

            // 更新敌人属性倍数
            _enemyHealthMultiplier = 1f + (difficultyLevel - 1) * 0.2f;
            _enemyDamageMultiplier = 1f + (difficultyLevel - 1) * 0.15f;
            _enemySpeedMultiplier = 1f + (difficultyLevel - 1) * 0.1f;
            _enemySpawnRate = 1f + (difficultyLevel - 1) * 0.1f;
            _enemyScoreMultiplier = 1f + (difficultyLevel - 1) * 0.2f;

            // 更新最大敌人数量
            _maxEnemies = 2 + (int)((difficultyLevel - 1) * 2);
        }

        /// <summary>
        /// 重置难度参数到初始值
        /// </summary>
        public static void ResetDifficulty()
        {
            _enemyHealthMultiplier = 1f;
            _enemyDamageMultiplier = 1f;
            _enemySpeedMultiplier = 1f;
            _enemySpawnRate = 1f;
            _maxEnemies = 2;
            _enemyScoreMultiplier = 1f;
            _currentDifficultyLevel = 1;
        }

        /// <summary>
        /// 获取敌人的实际生命值
        /// </summary>
        /// <param name="baseHealth">基础生命值</param>
        /// <returns>实际生命值</returns>
        public static float GetEnemyHealth(float baseHealth)
        {
            return baseHealth * _enemyHealthMultiplier;
        }

        /// <summary>
        /// 获取敌人的实际伤害值
        /// </summary>
        /// <param name="baseDamage">基础伤害值</param>
        /// <returns>实际伤害值</returns>
        public static float GetEnemyDamage(float baseDamage)
        {
            return baseDamage * _enemyDamageMultiplier;
        }

        /// <summary>
        /// 获取敌人的实际速度
        /// </summary>
        /// <param name="baseSpeed">基础速度</param>
        /// <returns>实际速度</returns>
        public static float GetEnemySpeed(float baseSpeed)
        {
            return baseSpeed * _enemySpeedMultiplier;
        }

        /// <summary>
        /// 获取敌人的实际分数
        /// </summary>
        /// <param name="baseScore">基础分数</param>
        /// <returns>实际分数</returns>
        public static int GetEnemyScore(int baseScore)
        {
            return Mathf.RoundToInt(baseScore * _enemyScoreMultiplier);
        }
    }
}