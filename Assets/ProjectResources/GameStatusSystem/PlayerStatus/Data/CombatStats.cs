using System.Numerics;
using UnityEngine;

namespace GameStatusSystem.PlayerStatus
{
    /// <summary>
    /// 玩家战斗状态类 - 存储玩家的战斗数据
    /// 例如：玩家击杀数、死亡数、命中数、未命中数、总射数数
    /// </summary>
    public class CombatStats
    {
        // 生命值相关
        public float currentHealth = 100f;
        // public float currentMana = 50f;
        // public float currentStamina = 100f;

        // 战斗属性
        // public float attackPower = 10f;
        // public float defensePower = 5f;
        // public float movementSpeed = 5f;

        // 战斗统计
        public int killStreak = 0;
        // public int totalDeaths = 0;
        public int totalScore = 0;
        public int totalHits = 0;
        public int totalShots = 0;
    }
}
