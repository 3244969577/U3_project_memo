using UnityEngine;


// [CreateAssetMenu(fileName = "BaseTerrainLayer", menuName = "MapGenerator/Layers/BaseTerrainLayer")]
public class PerlinNoiseLayer : MapGeneratorLayer
{
    [Header("柏林噪声配置")]
    public float noiseScale = 0.02f;
    [Header("地形阈值（0~1）")]
    public float waterThreshold = 0.3f;
    public float grassThreshold = 0.6f;
    public float mountainThreshold = 0.85f;
    public float rockThreshold = 0.95f;

    public override void Generate(MapGeneratorData data)
    {
        if (!isEnabled) return;
        
        // 固定种子，保证生成结果可复现
        Random.InitState(data.seed);
        float offsetX = Random.Range(0f, 100000f);
        float offsetY = Random.Range(0f, 100000f);
        
        // 遍历所有网格生成基础地形
        for (int x = 0; x < data.mapWidth; x++)
        {
            for (int y = 0; y < data.mapHeight; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                // 计算柏林噪声值
                float noiseValue = Mathf.PerlinNoise(
                    (x + offsetX) * noiseScale, 
                    (y + offsetY) * noiseScale
                );
                
                // 映射为地形类型
                TerrainType terrainType = GetTerrainTypeByNoise(noiseValue);
                data.baseTerrainData[pos] = terrainType;
            }
        }
        
        Debug.Log($"[{layerName}] 基础地形生成完成，共生成 {data.baseTerrainData.Count} 个网格");
    }

    // 噪声值 → 地形类型映射
    private TerrainType GetTerrainTypeByNoise(float noiseValue)
    {
        if (noiseValue < waterThreshold) return TerrainType.Water;
        if (noiseValue < grassThreshold) return TerrainType.Grass;
        if (noiseValue < mountainThreshold) return TerrainType.Mountain;
        if (noiseValue < rockThreshold) return TerrainType.Rock;
        return TerrainType.Sand;
    }
}