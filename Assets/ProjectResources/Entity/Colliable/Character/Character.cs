using UnityEngine;
using System.Collections;

/// <summary>
/// 角色基类 - 所有游戏角色的基础类
/// - 继承自Damageable，包含生命值系统
/// - 定义角色的基本属性和行为
/// </summary>
public abstract class Character : Damageable
{
    public float shield;
    public float speed;
    public float damage;
    [HideInInspector]
    public SpriteRenderer spriteRenderer;
    protected Animator animator;

    protected Rigidbody2D rb;

    protected bool moveAble = true;

    public virtual void Move() { }
    public virtual void Attack(Character target) { }
    public virtual void Die() { }
    public override void Update()
    {
        base.Update();
        if (this.healthBar.GetHealth() <= 0f)
        {
            this.Die();
        }
    }

    public override void Start()
    {
        base.Start();
        this.rb = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual Vector3 GetPosition()
    {
        return transform.position;
    }

    public void FreezeMovement()
    {
        this.moveAble = false;
    }
    public void UnFreezeMovement()
    {
        this.moveAble = true;
    }
    
    public Rigidbody2D GetRigidbody()
    {
        return this.rb;
    }
}