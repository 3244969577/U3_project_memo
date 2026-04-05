using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 对象池管理器
/// 用于管理预制体的对象池，避免频繁创建和销毁对象
/// </summary>
public class ObjectPoolManager : MonoBehaviour
{
    /// <summary>
    /// 单例实例
    /// </summary>
    public static ObjectPoolManager Instance { get; private set; }

    /// <summary>
    /// 对象池字典
    /// Key: 预制体实例ID
    /// Value: 该预制体的对象池
    /// </summary>
    private Dictionary<int, Queue<GameObject>> _objectPools = new Dictionary<int, Queue<GameObject>>();

    /// <summary>
    /// 父对象，用于存放所有池化对象
    /// </summary>
    private GameObject _poolParent;

    private void Awake()
    {
        // 单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 创建父对象
        _poolParent = new GameObject("ObjectPool");
        DontDestroyOnLoad(_poolParent);
    }

    /// <summary>
    /// 从对象池获取对象
    /// </summary>
    /// <param name="prefab">预制体</param>
    /// <param name="position">位置</param>
    /// <param name="rotation">旋转</param>
    /// <returns>对象实例</returns>
    public GameObject GetObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int prefabId = prefab.GetInstanceID();

        // 检查对象池是否存在
        if (!_objectPools.ContainsKey(prefabId))
        {
            _objectPools[prefabId] = new Queue<GameObject>();
        }

        Queue<GameObject> pool = _objectPools[prefabId];

        // 如果对象池为空，创建新对象
        if (pool.Count == 0)
        {
            GameObject obj = Instantiate(prefab, position, rotation);
            obj.SetActive(false);
            obj.transform.SetParent(_poolParent.transform);
            pool.Enqueue(obj);
        }

        // 从对象池取出对象
        GameObject instance = pool.Dequeue();
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        instance.SetActive(true);

        return instance;
    }

    /// <summary>
    /// 回收对象到对象池
    /// </summary>
    /// <param name="obj">要回收的对象</param>
    public void ReturnObject(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        // 获取对象的预制体
        GameObject prefab = PrefabUtility.GetPrefabParent(obj) as GameObject;
        if (prefab == null)
        {
            // 如果不是预制体实例，直接销毁
            Destroy(obj);
            return;
        }

        int prefabId = prefab.GetInstanceID();

        // 检查对象池是否存在
        if (!_objectPools.ContainsKey(prefabId))
        {
            _objectPools[prefabId] = new Queue<GameObject>();
        }

        // 回收对象
        obj.SetActive(false);
        obj.transform.SetParent(_poolParent.transform);
        _objectPools[prefabId].Enqueue(obj);
    }

    /// <summary>
    /// 清理指定预制体的对象池
    /// </summary>
    /// <param name="prefab">预制体</param>
    public void ClearPool(GameObject prefab)
    {
        int prefabId = prefab.GetInstanceID();
        if (_objectPools.ContainsKey(prefabId))
        {
            Queue<GameObject> pool = _objectPools[prefabId];
            while (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                Destroy(obj);
            }
            _objectPools.Remove(prefabId);
        }
    }

    /// <summary>
    /// 清理所有对象池
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var pool in _objectPools.Values)
        {
            while (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                Destroy(obj);
            }
        }
        _objectPools.Clear();
    }
}

/// <summary>
/// 预制体工具类
/// </summary>
public static class PrefabUtility
{
    /// <summary>
    /// 获取对象的预制体父对象
    /// </summary>
    /// <param name="obj">对象</param>
    /// <returns>预制体</returns>
    public static Object GetPrefabParent(Object obj)
    {
        #if UNITY_EDITOR
        return UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(obj);
        #else
        return null;
        #endif
    }
}