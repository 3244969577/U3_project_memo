using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 基础地形生成器（柏林噪声+多频率叠加）
/// </summary>
[Serializable]
public class BaseTerrainGenerator : GeneratorBase
{
    [Header("噪声配置")]
    [Tooltip("基础噪声频率（控制大尺度地形）")]
    public float BaseNoiseScale = 0.02f;
    
    [Tooltip("噪声叠加层数")]
    public int Octaves = 3;
    
    [Tooltip("频率倍率（每层×该值）")]
    public float FrequencyLacunarity = 2f;
    
    [Tooltip("振幅倍率（每层×该值）")]
    public float AmplitudeGain = 0.5f;
    
    [Tooltip("是否归一化噪声结果")]
    public bool NormalizeResult = true;

    [Header("地形阈值")]
    public float WaterThreshold = 0.3f;
    public float GrassThreshold = 0.6f;
    public float MountainThreshold = 0.85f;
    public float RockThreshold = 0.95f;

    // 噪声偏移缓存
    private Vector2[] _octaveOffsets;

    // 构造函数
    public BaseTerrainGenerator()
    {
        isEnabled = true;
    }

    /// <summary>
    /// 初始化生成器
    /// </summary>
    public override void Initialize()
    {
        // 使用全局种子生成噪声偏移，确保所有区块使用相同的偏移
        UnityEngine.Random.InitState(MapConstants.GlobalSeed);
        _octaveOffsets = GenerateOctaveOffsets();
    }

    /// <summary>
    /// 生成区块基础地形
    /// </summary>
    public override void Generate(IChunkData chunkData, Action onComplete = null)
    {
        if (!IsEnabled || chunkData == null)
        {
            onComplete?.Invoke();
            return;
        }

        try
        {
            Vector2Int chunkCoord = chunkData.ChunkCoord;
            BoundsInt chunkBounds = ChunkCoordinateUtility.GetChunkWorldBounds(chunkCoord);
            
            // 计算最大可能值（用于归一化）
            float maxPossibleValue = CalculateMaxPossibleValue();

            // 遍历区块所有坐标（包含缓冲区）
            for (int x = chunkBounds.xMin; x < chunkBounds.xMax; x++)
            {
                for (int y = chunkBounds.yMin; y < chunkBounds.yMax; y++)
                {
                    Vector2Int worldPos = new Vector2Int(x, y);
                    float noiseValue = CalculateFractalNoise(x, y, _octaveOffsets, maxPossibleValue);
                    TerrainType terrainType = GetTerrainType(noiseValue);
                    
                    chunkData.SetData(worldPos, terrainType);
                }
            }

            Debug.Log($"[BaseTerrainGenerator] 区块 {chunkCoord} 基础地形生成完成");
        }
        catch (Exception e)
        {
            Debug.LogError($"[BaseTerrainGenerator] 生成失败：{e.Message}");
        }
        finally
        {
            onComplete?.Invoke();
        }
    }

    /// <summary>
    /// 清理生成器
    /// </summary>
    public override void Cleanup()
    {
        // 清理逻辑
    }

    #region 内部计算方法
    /// <summary>
    /// 生成每层噪声的偏移
    /// </summary>
    private Vector2[] GenerateOctaveOffsets()
    {
        Vector2[] offsets = new Vector2[Octaves];
        for (int i = 0; i < Octaves; i++)
        {
            offsets[i] = new Vector2(
                UnityEngine.Random.Range(-100000f, 100000f),
                UnityEngine.Random.Range(-100000f, 100000f)
            );
        }
        return offsets;
    }

    /// <summary>
    /// 计算噪声最大可能值
    /// </summary>
    private float CalculateMaxPossibleValue()
    {
        float max = 0;
        float amplitude = 1;
        
        for (int i = 0; i < Octaves; i++)
        {
            max += amplitude;
            amplitude *= AmplitudeGain;
        }
        
        return max;
    }

    /// <summary>
    /// 计算多频率叠加噪声
    /// </summary>
    private float CalculateFractalNoise(float x, float y, Vector2[] offsets, float maxValue)
    {
        float total = 0;
        float frequency = BaseNoiseScale;
        float amplitude = 1;

        for (int i = 0; i < Octaves; i++)
        {
            float noiseX = (x + offsets[i].x) * frequency;
            float noiseY = (y + offsets[i].y) * frequency;
            
            // 转换为-1~1范围，叠加更自然
            float perlin = Mathf.PerlinNoise(noiseX, noiseY) * 2 - 1;
            total += perlin * amplitude;

            frequency *= FrequencyLacunarity;
            amplitude *= AmplitudeGain;
        }

        // 归一化到0~1
        if (NormalizeResult)
        {
            total = (total + 1) / 2; // 转换为0~1
            total /= maxValue;       // 归一化
        }
        
        return Mathf.Clamp01(total);
    }

    /// <summary>
    /// 噪声值转换为地形类型
    /// </summary>
    private TerrainType GetTerrainType(float noiseValue)
    {
        if (noiseValue < WaterThreshold) return TerrainType.Water;
        if (noiseValue < GrassThreshold) return TerrainType.Grass;
        if (noiseValue < MountainThreshold) return TerrainType.Mountain;
        if (noiseValue < RockThreshold) return TerrainType.Rock;
        
        return TerrainType.Sand;
    }
    #endregion
}
