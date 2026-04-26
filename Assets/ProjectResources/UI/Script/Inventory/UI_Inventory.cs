using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    const string TAG = "Inventory";
    private Inventory inventory;
    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;
    private float lastShownTime=0;

#region hook
    private void Awake()
    {
        itemSlotContainer = transform.Find("ItemSlotContainer");
        itemSlotTemplate = itemSlotContainer.Find("ItemSlotTemplate");
    }

    public void FixedUpdate()
    {
        if (Time.time - lastShownTime >= 5f)
        {
            Hidden();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) || 
            Input.GetKeyDown(KeyCode.Alpha2) ||
            Input.GetKeyDown(KeyCode.Alpha3) ||
            Input.GetKeyDown(KeyCode.Alpha4))
            Show();
    }
#endregion


#region 公共接口
    public void Show()
    {
        transform.gameObject.SetActive(true);
        lastShownTime = Time.time;
    }

    public void Hidden()
    {
        transform.gameObject.SetActive(false);
    }

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
        Show();
        RefreshInventoryItems();
    }
#endregion


#region helper
    private void RefreshInventoryItems()
    {
        int x = 0;
        int y = 0;
        float itemSlotCellSize = 125f;
        foreach(Collectible item in this.inventory.GetItemList())
        {
            // 1. 实例化一个物品槽位
            // 2. 设置物品槽位的位置
            // 3. 设置物品槽位的图片为物品的图片
            // 4. 增加物品槽位的索引
            RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);
            itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, y * itemSlotCellSize);
            Image image = itemSlotRectTransform.Find("Image").GetComponent<Image>();
            image.sprite = item.GetSprite();

            x++;
            if (x > 4)
            {
                x = 0;
                y++;
            }
        }
    }
#endregion


}
