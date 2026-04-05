using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 区块加载器（基于相机视野动态加载/卸载）
/// </summary>
public class ChunkLoader : MonoBehaviour
{
    [Header("相机配置")]
    public Camera MainCamera;
    
    [Header("范围配置")]
    public float LoadBuffer = 1.5f; // 加载缓冲系数，大于1表示在视野外提前加载
    public int UnloadRange = 3; // 卸载范围
    public int MaxConcurrentLoads = 2; // 最大同时加载数量

    // 内部状态
    private Bounds _lastCameraBounds;
    private float _checkTimer;
    private List<Vector2Int> _loadQueue = new List<Vector2Int>(); // 使用List以便排序
    private int _currentLoads = 0;
    private HashSet<Vector2Int> _loadingChunks = new HashSet<Vector2Int>(); // 正在加载的区块

    private void Update()
    {
        if (MainCamera == null || ChunkManager.Instance == null) return;
        
        // 定时检查相机位置和视野
        _checkTimer += Time.deltaTime;
        if (_checkTimer >= MapConstants.ChunkLoadCheckInterval)
        {
            _checkTimer = 0;
            CheckCameraBounds();
        }
        
        // 处理加载队列，控制并发数量
        if (_currentLoads < MaxConcurrentLoads && _loadQueue.Count > 0)
        {
            Vector2Int coord = _loadQueue[0];
            _loadQueue.RemoveAt(0);
            _loadingChunks.Add(coord);
            _currentLoads++;
            StartCoroutine(LoadChunk(coord));
        }
    }

    /// <summary>
    /// 检查相机视野并触发区块加载/卸载
    /// </summary>
    private void CheckCameraBounds()
    {
        // 获取相机视野范围
        Bounds cameraBounds = GetCameraBounds();
        
        // 计算视野范围内的所有区块
        List<Vector2Int> visibleChunks = CalculateChunksInBounds(cameraBounds);
        
        // 计算需要加载的区块
        List<Vector2Int> needLoad = new List<Vector2Int>();
        foreach (var chunkCoord in visibleChunks)
        {
            if (!ChunkManager.Instance.GeneratedChunkCoords.Contains(chunkCoord) && !_loadQueue.Contains(chunkCoord))
            {
                needLoad.Add(chunkCoord);
            }
        }
        
        // 计算需要卸载的区块
        List<Vector2Int> needUnload = CalculateChunksToUnload(cameraBounds, visibleChunks);

        // 将需要加载的区块加入队列，并按距离相机的远近排序
        foreach (var coord in needLoad)
        {
            if (!_loadQueue.Contains(coord) && !_loadingChunks.Contains(coord))
            {
                _loadQueue.Add(coord);
            }
        }
        
        // 按距离相机的远近排序，优先加载近的区块
        _loadQueue.Sort((a, b) => {
            Vector2 centerA = ChunkCoordinateUtility.ChunkToWorldCoord(a, new Vector2Int(MapConstants.ChunkSize / 2, MapConstants.ChunkSize / 2));
            Vector2 centerB = ChunkCoordinateUtility.ChunkToWorldCoord(b, new Vector2Int(MapConstants.ChunkSize / 2, MapConstants.ChunkSize / 2));
            float distanceA = Vector2.Distance(centerA, MainCamera.transform.position);
            float distanceB = Vector2.Distance(centerB, MainCamera.transform.position);
            return distanceA.CompareTo(distanceB);
        });
        
        // 执行卸载
        foreach (var coord in needUnload)
        {
            ChunkManager.Instance.RequestUnloadChunk(coord);
        }
        
        // 更新上次的相机视野
        _lastCameraBounds = cameraBounds;
    }

    /// <summary>
    /// 获取相机视野范围
    /// </summary>
    private Bounds GetCameraBounds()
    {
        float height = 2f * MainCamera.orthographicSize;
        float width = height * MainCamera.aspect;
        
        Vector3 center = MainCamera.transform.position;
        
        // 应用加载缓冲，提前加载视野外的区块
        return new Bounds(center, new Vector3(width * LoadBuffer, height * LoadBuffer, 0));
    }

    /// <summary>
    /// 计算视野范围内的所有区块
    /// </summary>
    private List<Vector2Int> CalculateChunksInBounds(Bounds bounds)
    {
        List<Vector2Int> chunks = new List<Vector2Int>();
        
        // 计算视野范围的最小和最大区块坐标
        Vector2Int minChunk = ChunkCoordinateUtility.WorldToChunkCoord(new Vector2Int(
            Mathf.FloorToInt(bounds.min.x),
            Mathf.FloorToInt(bounds.min.y)
        ));
        
        Vector2Int maxChunk = ChunkCoordinateUtility.WorldToChunkCoord(new Vector2Int(
            Mathf.CeilToInt(bounds.max.x),
            Mathf.CeilToInt(bounds.max.y)
        ));
        
        // 遍历视野范围内的所有区块
        for (int x = minChunk.x; x <= maxChunk.x; x++)
        {
            for (int y = minChunk.y; y <= maxChunk.y; y++)
            {
                chunks.Add(new Vector2Int(x, y));
            }
        }
        
        return chunks;
    }

    /// <summary>
    /// 加载单个区块
    /// </summary>
    private IEnumerator LoadChunk(Vector2Int coord)
    {
        // 检查区块是否已经加载
        if (ChunkManager.Instance.GeneratedChunkCoords.Contains(coord))
        {
            _loadingChunks.Remove(coord);
            _currentLoads--;
            yield break;
        }
        
        // 加载区块
        ChunkManager.Instance.RequestLoadChunk(coord);
        
        // 等待区块加载完成
        // 这里使用循环检查区块是否已加载
        float waitTime = 0f;
        float maxWaitTime = 5f; // 最大等待时间
        
        while (!ChunkManager.Instance.GeneratedChunkCoords.Contains(coord) && waitTime < maxWaitTime)
        {
            yield return new WaitForSeconds(0.1f);
            waitTime += 0.1f;
        }
        
        // 更新加载状态
        _loadingChunks.Remove(coord);
        _currentLoads--;
    }

    /// <summary>
    /// 计算需要卸载的区块
    /// </summary>
    private List<Vector2Int> CalculateChunksToUnload(Bounds cameraBounds, List<Vector2Int> visibleChunks)
    {
        List<Vector2Int> toUnload = new List<Vector2Int>();
        
        foreach (var chunkCoord in ChunkManager.Instance.GeneratedChunkCoords)
        {
            // 计算区块中心
            Vector2 chunkCenter = ChunkCoordinateUtility.ChunkToWorldCoord(chunkCoord, new Vector2Int(MapConstants.ChunkSize / 2, MapConstants.ChunkSize / 2));

            
            // 检查区块是否在卸载范围内
            if (!visibleChunks.Contains(chunkCoord) && !cameraBounds.Contains(new Vector3(chunkCenter.x, chunkCenter.y, 0)))
            {
                // 计算区块到相机中心的距离
                float distance = Vector2.Distance(chunkCenter, cameraBounds.center);
                
                // 如果距离超过卸载范围，则卸载
                if (distance > cameraBounds.extents.magnitude * 1.5f)
                {
                    toUnload.Add(chunkCoord);
                }
            }
        }
        
        return toUnload;
    }
}
