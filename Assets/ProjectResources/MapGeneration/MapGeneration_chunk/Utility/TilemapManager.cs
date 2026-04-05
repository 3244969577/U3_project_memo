using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 全局Tilemap管理器（单例）
/// 管理所有区块共享的Tilemap
/// </summary>
public class TilemapManager : MonoBehaviour
{
    [Header("全局Tilemap配置")]
    public Tilemap BaseTerrainTilemap; // 所有地形
    public Tilemap FineStructureTilemap;
    public Tilemap DecorationTilemap;

    // 单例实例
    private static TilemapManager _instance;
    public static TilemapManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 创建管理器物体
                GameObject managerObj = new GameObject("TilemapManager");
                _instance = managerObj.AddComponent<TilemapManager>();
                _instance.EnsureTilemapsInitialized();
                
                // 确保场景切换时不被销毁
                DontDestroyOnLoad(managerObj);
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            EnsureTilemapsInitialized();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 确保所有Tilemap都已初始化
    /// </summary>
    private void EnsureTilemapsInitialized()
    {
        // 所有地形都不需要碰撞箱
    }
    
    /// <summary>
    /// 清理所有Tilemap
    /// </summary>
    public void ClearAllTilemaps()
    {
        TilemapUtility.ClearTilemapSafe(BaseTerrainTilemap);
        TilemapUtility.ClearTilemapSafe(FineStructureTilemap);
        TilemapUtility.ClearTilemapSafe(DecorationTilemap);
    }
}
