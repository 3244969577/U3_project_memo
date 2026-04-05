using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Collectible
{

    /// <summary>
    /// 武器是否已被玩家装备
    /// - true: 武器已被玩家装备
    /// - false: 武器未被玩家装备
    /// </summary>
    protected bool isAttached = false;

    /// <summary>
    /// 武器的耐久度
    /// - 当耐久度降低到0时，武器可能会损坏或失效
    /// </summary>
    public float durability = 0;

    /// <summary>
    /// 使用武器的消耗
    /// - 每次使用武器时会减少对应值的耐久度
    /// </summary>
    public float useCost = 0;

    protected abstract void Attack();
    public abstract void UpdatePosition(Vector2 mousePosition);
    protected abstract void UpdateDurability(float value);

    protected virtual void OnDurabilityBreak() {
        // 武器耐久度为0时的处理逻辑
        // 禁用掉武器的交互
        Debug.Log("武器耐久度为0，已禁用交互");
    }

    protected override void Update()
    {

        if (this.isAttached)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.UpdatePosition(mousePosition);
        }

        if (Input.GetButton("Fire1"))
        {
            if (this.isAttached)
            {
                this.Attack();
                this.UpdateDurability(this.useCost);
            }
        }
    }

    public void Attach(Player player)
    {
        this.transform.parent = player.transform;
        this.isAttached = true;
        this.transform.localRotation = Quaternion.identity;
        this.transform.localPosition = new Vector3(0.25f, 0.5f, 0f);
    }

    // 实现父类接口
    
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            Debug.Log("武器被玩家拾取");
            Player player = collision.GetComponent<Player>();
            this.Attach(player);
            player.EquipWeapon(this);
            SoundManager.instance.PlaySound("Pickup");
            player.AddItemToInventory(this);
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
        }
    }
}