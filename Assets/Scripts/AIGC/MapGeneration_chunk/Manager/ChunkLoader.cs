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

    // 内部状态
    private Bounds _lastCameraBounds;
    private float _checkTimer;
    private Queue<Vector2Int> _loadQueue = new Queue<Vector2Int>();
    private bool _isLoading = false;

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
        
        // 处理加载队列
        if (!_isLoading && _loadQueue.Count > 0)
        {
            StartCoroutine(ProcessLoadQueue());
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

        // 将需要加载的区块加入队列
        foreach (var coord in needLoad)
        {
            _loadQueue.Enqueue(coord);
        }
        
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
    /// 处理加载队列，尽可能加载所有能够加载的区块
    /// </summary>
    private IEnumerator ProcessLoadQueue()
    {
        _isLoading = true;
        
        // 处理所有队列中的区块
        while (_loadQueue.Count > 0)
        {
            Vector2Int coord = _loadQueue.Dequeue();
            ChunkManager.Instance.RequestLoadChunk(coord);
            yield return new WaitForSeconds(0.05f); // 稍微延迟，避免一次性生成过多
        }
        
        _isLoading = false;
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
