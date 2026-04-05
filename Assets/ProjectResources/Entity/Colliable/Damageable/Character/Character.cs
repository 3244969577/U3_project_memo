using UnityEngine;
using System.Collections;

/// <summary>
/// 角色基类 - 所有游戏角色的基础类
/// - 继承自Damageable，包含生命值系统
/// - 定义角色的基本属性和行为
/// </summary>
public abstract class Character : Damageable
{
    /// <summary>
    /// 角色的护盾值
    /// - 护盾可以吸收伤害，保护角色生命值
    /// </summary>
    public float shield;

    /// <summary>
    /// 角色的移动速度
    /// - 影响角色的移动和追逐速度
    /// </summary>
    public float speed;

    /// <summary>
    /// 角色的基础伤害值
    /// - 影响角色攻击时造成的伤害
    /// </summary>
    public float damage;

    /// <summary>
    /// 角色的精灵渲染器
    /// - 用于控制角色的视觉显示
    /// </summary>
    [HideInInspector]
    public SpriteRenderer spriteRenderer;

    /// <summary>
    /// 角色的动画控制器
    /// - 用于控制角色的动画播放
    /// </summary>
    protected Animator animator;

    /// <summary>
    /// 角色的刚体组件
    /// - 用于物理移动和碰撞
    /// </summary>
    protected Rigidbody2D rb;

    /// <summary>
    /// 角色是否可以移动
    /// - true: 角色可以移动
    /// - false: 角色被冻结，无法移动
    /// </summary>
    protected bool moveAble = true;

    protected virtual void Move() { }
    protected virtual void Attack(Damageable target) { }
    protected virtual void Die() { }
    protected override void Update()
    {
        base.Update();
        if (this.healthBar.GetHealth() <= 0f)
        {
            this.Die();
        }
    }

    protected override void Start()
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
}