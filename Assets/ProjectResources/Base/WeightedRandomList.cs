using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeightedItem<T>
{
    [SerializeField] public T Item;
    [SerializeField] public float Weight = 1f;
}

[Serializable]
public class WeightedRandomList<T>
{
    [SerializeField] private List<WeightedItem<T>> items = new List<WeightedItem<T>>();
    
    // 计算总权重
    private float CalculateTotalWeight()
    {
        float totalWeight = 0f;
        foreach (var item in items)
        {
            totalWeight += item.Weight;
        }
        return totalWeight;
    }
    
    // 根据权重随机获取一个元素
    public T GetRandomItem()
    {
        if (items.Count == 0)
        {
            return default;
        }
        
        float totalWeight = CalculateTotalWeight();
        float randomValue = UnityEngine.Random.value * totalWeight;
        float currentWeight = 0f;
        
        foreach (var item in items)
        {
            currentWeight += item.Weight;
            if (randomValue <= currentWeight)
            {
                return item.Item;
            }
        }
        
        // 防止浮点数精度问题
        return items[items.Count - 1].Item;
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
    public void AddItem(T item, float weight = 1f)
    {
        items.Add(new WeightedItem<T> { Item = item, Weight = weight });
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
