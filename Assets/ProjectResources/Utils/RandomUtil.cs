using System;
using UnityEngine;
public static class RandomUtil
{
    // 生成 15 位随机种子（兼容所有Unity版本）
    public static long GetRandomSeed()
    {
        System.Random rand = new System.Random();
        return (long)(rand.NextDouble() * 900000000000000L) + 100000000000000L;
    }
}