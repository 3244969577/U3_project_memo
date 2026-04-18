using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            // 已经存在，直接返回
            if (_instance != null)
                return _instance;

            // 找场景中已有的
            _instance = FindObjectOfType<T>();

            // 找不到，自动创建
            if (_instance == null)
            {
                var go = new GameObject($"[{typeof(T).Name}]");
                _instance = go.AddComponent<T>();
            }

            return _instance;
        }
    }
    
    protected virtual void Awake()
    {
        // 保证唯一
        if (_instance == null)
        {
            _instance = (T)this;
            // 可选：跨场景不销毁
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogError($"Multiple instances of {typeof(T).Name} found in scene!");
            // Destroy(gameObject);
        }
    }
}