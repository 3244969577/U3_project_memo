using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProbabilityItem<T>
{
    public T item;
    [Range(0f, 1f)] public float probability = 0.5f;

    public T Item => item;
    public float Probability => probability;
}

[Serializable]
public class ProbabilityList<T>
{
    [SerializeField] private List<ProbabilityItem<T>> items = new List<ProbabilityItem<T>>();
    
    // 根据概率随机获取一个元素
    // 如果没有任何元素被取到，返回null
    public T GetRandomItem()
    {
        if (items.Count == 0)
        {
            return default;
        }
        
        foreach (var item in items)
        {
            float randomValue = UnityEngine.Random.value;
            if (randomValue <= item.Probability)
            {
                return item.Item;
            }
        }
        
        // 没有元素被取到
        return default;
    }
    
    // 获取所有物品
    public List<T> GetAllItems()
    {
        List<T> result = new List<T>();
        foreach (var item in items)
        {
            result.Add(item.Item);
        }
        return result;
    }
    
    // 添加物品
    public void AddItem(T item, float probability = 0.5f)
    {
        // 确保概率在0-1之间
        probability = Mathf.Clamp01(probability);
        ProbabilityItem<T> probabilityItem = new ProbabilityItem<T>();
        
        // 使用反射设置私有字段
        System.Reflection.FieldInfo itemField = typeof(ProbabilityItem<T>).GetField("item", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        System.Reflection.FieldInfo probabilityField = typeof(ProbabilityItem<T>).GetField("probability", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (itemField != null)
        {
            itemField.SetValue(probabilityItem, item);
        }
        if (probabilityField != null)
        {
            probabilityField.SetValue(probabilityItem, probability);
        }
        
        items.Add(probabilityItem);
    }
    
    // 移除物品
    public void RemoveItem(T item)
    {
        items.RemoveAll(x => Equals(x.Item, item));
    }
    
    // 清空列表
    public void Clear()
    {
        items.Clear();
    }
    
    // 获取列表大小
    public int Count => items.Count;
}
