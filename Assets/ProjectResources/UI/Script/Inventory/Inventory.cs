using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private List<Collectible> itemList;

    public Inventory()
    {
        this.itemList = new List<Collectible>();

    }

    public void AddItem(Collectible item)
    {
        this.itemList.Add(item);
    }
    public void RemoveItem(Collectible item)
    {
        this.itemList.Remove(item);
    }

    public Collectible GetItem(int index)
    {
        if (index < this.itemList.Count)
            return this.itemList[index];
        else
            return null;
    }

    public List<Collectible> GetItemList()
    {
        return this.itemList;
    }
}
