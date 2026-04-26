using System.Collections.Generic;
using UnityEngine;

// 单一物体对象池
public class ObjectPool
{
    private readonly Queue<GameObject> _poolQueue;
    private readonly GameObject _prefab;
    private readonly Transform _parentRoot;

    public ObjectPool(GameObject prefab, Transform root = null)
    {
        _prefab = prefab;
        _parentRoot = root;
        _poolQueue = new Queue<GameObject>();
    }

    // 取出物体
    public GameObject Get()
    {
        GameObject obj;
        if (_poolQueue.Count > 0)
        {
            obj = _poolQueue.Dequeue();
        }
        else
        {
            // 池子里没有就实例化
            obj = Object.Instantiate(_prefab, _parentRoot);
        }

        obj.SetActive(true);
        return obj;
    }

    // 回收物体
    public void Release(GameObject obj)
    {
        obj.SetActive(false);
        _poolQueue.Enqueue(obj);
    }
}