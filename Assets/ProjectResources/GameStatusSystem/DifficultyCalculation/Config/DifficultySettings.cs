using UnityEngine;
using System;


namespace GameStatusSystem.DifficultyCalculation
{
    [System.Serializable]
    public class DifficultySettings
    {
        [Header("评分权重总和 = 1")]
        public float weightPower = 0.4f;
        public float weightPerformance = 0.4f;
        public float weightSurvival = 0.2f;

        [Header("难度输出范围")]
        public float minDifficulty = 0.5f;
        public float maxDifficulty = 1.5f;

        [Header("评分区间")]
        public float scoreLow = 30f;
        public float scoreMid = 60f;
        public float scoreHigh = 85f;
    }
}