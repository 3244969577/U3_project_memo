using UnityEngine;

interface IPoolable
{
    public GameObject Prefab { get; }
    public void Recycle();
}