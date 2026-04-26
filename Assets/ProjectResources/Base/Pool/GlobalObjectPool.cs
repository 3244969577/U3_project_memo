using System.Collections.Generic;
using UnityEngine;

public class GlobalObjectPool : Singleton<GlobalObjectPool>
{
    private readonly Dictionary<GameObject, ObjectPool> _poolDict = new();

    // 所有回收物体统一父节点，层级整洁
    [Header("池物体挂载父节点")]
    public GameObject poolRoot;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (!_poolDict.ContainsKey(prefab))
        {
            // 没有这个池子 → 自动新建对应池子
            _poolDict.Add(prefab, new ObjectPool(prefab, poolRoot.transform));
            Debug.Log($"创建新池子：{prefab.name}");
        }
        else
        {
            Debug.Log($"从池子：{prefab.name}获取对象");
        }

        GameObject obj = _poolDict[prefab].Get();
        obj.transform.SetPositionAndRotation(pos, rot);
        return obj;
    }

    public void Recycle(GameObject obj, GameObject sourcePrefab)
    {
        Debug.Log($"回收对象：{obj.name}，来源：池子：{sourcePrefab.name}");
        if (_poolDict.TryGetValue(sourcePrefab, out var pool))
        {
            pool.Release(obj);
        }
        else
        {
            // 不存在对应池子直接销毁（兜底）
            Destroy(obj);
        }
    }

    public void Recycle(GameObject obj)
    {
        IPoolable poolable = obj.GetComponent<IPoolable>();
        // Debug.Assert(poolable != null, $"回收对象：{obj.name}，未实现IPoolable接口");
        Debug.Log(poolable.Prefab);
        Recycle(obj, poolable.Prefab);
    }
}