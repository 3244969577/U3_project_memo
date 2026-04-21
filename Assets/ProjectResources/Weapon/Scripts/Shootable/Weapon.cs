using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalEvents;

public abstract class Weapon : Collectible
{
    public float score = 100f;
    protected bool isAttached = false;


#region Weapon Abstract Methods
    // Weapon 预留方法，用于实现不同的攻击行为
    protected abstract void Attack(Vector3 targetPosition);
    protected abstract void UpdatePosition(Vector3 mousePosition);

#endregion

    protected override void Update()
    {
        if (this.isAttached)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.UpdatePosition(mousePosition);

            if (Input.GetButton("Fire1"))
            {
                this.Attack(mousePosition);
            }
        }
    }

    

    // 实现父类接口: 拾取逻辑
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 触发玩家获得装备事件
            EventBus<ObtainEquipmentEvent>.Raise(new ObtainEquipmentEvent { equipment = this.gameObject });

            Player player = collision.GetComponent<Player>();
            this.Attach(player);
            SoundManager.Instance.PlaySound("Pickup");
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
        }
    }

    protected void Attach(Player player)
    {
        this.transform.parent = player.transform;
        this.isAttached = true;
        this.transform.localRotation = Quaternion.identity;
        this.transform.localPosition = new Vector3(0.25f, 0.5f, 0f);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), this.GetComponent<Collider2D>(), true);
        }
    }
}