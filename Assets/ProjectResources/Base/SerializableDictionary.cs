using System;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue>
{
    [SerializeField] private TKey[] keys;
    [SerializeField] private TValue[] values;

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (keys != null && values != null)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i].Equals(key))
                {
                    value = values[i];
                    return true;
                }
            }
        }

        value = default;
        return false;
    }
}