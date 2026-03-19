using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MultiNoiseMapGeneratorLayer", menuName = "MapGenerator/Layers/MultiNoiseMapGeneratorLayer")]
public class MultiNoiseMapGeneratorLayer : MapGeneratorLayer
{
    [Header("基础柏林噪声配置")]
    public float baseNoiseScale = 0.02f; // 基础频率（低频，控制大尺度地形）
    public int octaves = 4; // 噪声叠加层数（建议3-5层）
    public float frequencyLacunarity = 2f; // 频率倍率（每层频率 = 上一层 × 该值）
    public float amplitudeGain = 0.5f; // 振幅倍率（每层振幅 = 上一层 × 该值）
    public bool normalizeResult = true; // 是否归一化结果（保证最终值在0~1）

    [Header("地形阈值（0~1）")]
    public float waterThreshold = 0.2f;
    public float grassThreshold = 0.6f;
    public float mountainThreshold = 0.85f;
    public float rockThreshold = 0.95f;

    // 新增：噪声扭曲配置
    [Header("噪声扭曲（可选）")]
    public bool enableNoiseWarp = true;
    public float warpScale = 0.01f; // 扭曲频率（低频=大尺度扭曲）
    public float warpStrength = 10f; // 扭曲强度

    public override void Generate(MapGeneratorData data)
    {
        if (!isEnabled) return;
        
        // 固定种子，保证生成结果可复现
        Random.InitState(data.seed);
        // 每层噪声使用不同的偏移，避免叠加后出现重复纹理
        Vector2[] octaveOffsets = GenerateOctaveOffsets(octaves);
        
        // 预计算最大可能值（用于归一化）
        float maxPossibleValue = 0;
        float currentAmplitude = 1;
        for (int i = 0; i < octaves; i++)
        {
            maxPossibleValue += currentAmplitude;
            currentAmplitude *= amplitudeGain;
        }
        
        // 遍历所有网格生成基础地形
        for (int x = 0; x < data.mapWidth; x++)
        {
            for (int y = 0; y < data.mapHeight; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                // 计算多频率叠加后的噪声值
                float noiseValue = CalculateFractalNoise(x, y, octaveOffsets, maxPossibleValue);
                
                // 映射为地形类型
                TerrainType terrainType = GetTerrainTypeByNoise(noiseValue);
                data.baseTerrainData[pos] = terrainType;
            }
        }
        
        Debug.Log($"[{layerName}] 基础地形生成完成（叠加{octaves}层噪声），共生成 {data.baseTerrainData.Count} 个网格");
    }

    // 生成每层噪声的随机偏移（避免叠加后纹理重复）
    private Vector2[] GenerateOctaveOffsets(int octaves)
    {
        Vector2[] offsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = Random.Range(-100000f, 100000f);
            float offsetY = Random.Range(-100000f, 100000f);
            offsets[i] = new Vector2(offsetX, offsetY);
        }
        return offsets;
    }

    // 核心：计算多频率叠加的噪声值（分形布朗运动）
    private float CalculateFractalNoise(float x, float y, Vector2[] octaveOffsets, float maxPossibleValue)
    {
        float totalNoise = 0;
        float currentFrequency = baseNoiseScale;
        float currentAmplitude = 1;

        // 第一步：扭曲坐标（可选）
        if (enableNoiseWarp)
        {
            // 用低频噪声计算偏移量
            float warpX = Mathf.PerlinNoise(x * warpScale, y * warpScale) * warpStrength;
            float warpY = Mathf.PerlinNoise((x + 5.2f) * warpScale, (y + 1.3f) * warpScale) * warpStrength;
            // 偏移原始坐标
            x += warpX;
            y += warpY;
        }


        // 叠加每层噪声
        for (int i = 0; i < octaves; i++)
        {
            // 每层使用不同的偏移和频率
            float noiseX = (x + octaveOffsets[i].x) * currentFrequency;
            float noiseY = (y + octaveOffsets[i].y) * currentFrequency;
            // 柏林噪声返回0~1，转换为-0.5~0.5，让叠加后更自然（可选）
            float perlinValue = Mathf.PerlinNoise(noiseX, noiseY) * 2 - 1;
            // 加权叠加（振幅控制该层权重）
            totalNoise += perlinValue * currentAmplitude;

            // 更新频率和振幅
            currentFrequency *= frequencyLacunarity;
            currentAmplitude *= amplitudeGain;
        }

        // 归一化：将结果映射回0~1（关键，避免阈值失效）
        if (normalizeResult)
        {
            // 先将-1~1的范围映射到0~1，再除以最大可能值，保证最终值在0~1
            totalNoise = (totalNoise + 1) / 2;
            totalNoise /= maxPossibleValue;
        }
        else
        {
            // 非归一化时，强制限制在0~1（防止越界）
            totalNoise = Mathf.Clamp01(totalNoise);
        }

        return totalNoise;
    }

    // 噪声值 → 地形类型映射（保持不变）
    private TerrainType GetTerrainTypeByNoise(float noiseValue)
    {
        if (noiseValue < waterThreshold) return TerrainType.Water;
        if (noiseValue < grassThreshold) return TerrainType.Grass;
        if (noiseValue < mountainThreshold) return TerrainType.Mountain;
        if (noiseValue < rockThreshold) return TerrainType.Rock;
        return TerrainType.Sand;
    }
}