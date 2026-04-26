using UnityEngine;
using System.Collections;

/// <summary>
/// + 移动控制
/// + 攻击控制
/// </summary>
public abstract class Character : Damageable
{
    public float speed;
    public float damage;

    [HideInInspector]
    public SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected Rigidbody2D rb;
    protected bool moveAble = true;


# region virtual methods
    public virtual void Move() { }

    public virtual void Attack(Character target) { }

    public virtual Vector3 GetPosition()
    {
        return transform.position;
    }
# endregion


#region hook
    protected override void Awake()
    {
        base.Awake();
        this.rb = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }
#endregion


#region helper
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
#endregion
}