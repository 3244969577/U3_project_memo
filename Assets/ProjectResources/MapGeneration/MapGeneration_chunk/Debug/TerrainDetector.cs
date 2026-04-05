using UnityEngine;

/// <summary>
/// 地形检测器，用于检测玩家所在的地形类型
/// </summary>
public class TerrainDetector : MonoBehaviour
{
    [Header("检测配置")]
    public float CheckInterval = 0.5f; // 检测间隔
    private float _nextCheckTime = 0f;
    
    private void Start()
    {
        // Check if DebugUI exists
        if (DebugUI.Instance == null)
        {
            Debug.LogWarning("DebugUI instance not found, please add DebugUIPrefab to scene in edit mode");
        }
    }
    
    private void Update()
    {
        // 按照设定的间隔进行检测
        if (Time.time >= _nextCheckTime)
        {
            DetectTerrain();
            _nextCheckTime = Time.time + CheckInterval;
        }
    }
    
    /// <summary>
    /// Detect current terrain type
    /// </summary>
    private void DetectTerrain()
    {
        // Get player current position
        Vector2 playerPos = transform.position;
        Vector2Int worldPos = new Vector2Int(Mathf.FloorToInt(playerPos.x), Mathf.FloorToInt(playerPos.y));
        
        // Get corresponding chunk coordinate
        Vector2Int chunkCoord = ChunkCoordinateUtility.WorldToChunkCoord(worldPos);
        
        // Get chunk data from ChunkManager
        if (ChunkManager.Instance != null)
        {
            var chunkData = ChunkManager.Instance.GetChunkData(chunkCoord);
            if (chunkData != null)
            {
                // Get terrain type at this position
                TerrainType terrainType = chunkData.GetData<TerrainType>(worldPos);
                
                // Update DebugUI display
                if (DebugUI.Instance != null)
                {
                    DebugUI.Instance.AddDebugInfo("Terrain Type", terrainType.ToString());
                    DebugUI.Instance.AddDebugInfo("Position", worldPos.ToString());
                    DebugUI.Instance.AddDebugInfo("Chunk", chunkCoord.ToString());
                }
            }
            else
            {
                // Chunk not generated
                if (DebugUI.Instance != null)
                {
                    DebugUI.Instance.AddDebugInfo("Terrain Type", "Chunk not generated");
                    DebugUI.Instance.AddDebugInfo("Position", worldPos.ToString());
                    DebugUI.Instance.AddDebugInfo("Chunk", chunkCoord.ToString());
                }
            }
        }
        else
        {
            // ChunkManager not exists
            if (DebugUI.Instance != null)
            {
                DebugUI.Instance.AddDebugInfo("Terrain Type", "ChunkManager not found");
            }
        }
    }
}