using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalEvents;
using System.Reflection;

public class Damageable : Collidable
{
    public float maxHealth;
    protected HealthBar healthBar = new HealthBar();
    protected GameObject lastAttacker; // 记录最后攻击者
    protected bool isDead = false;


#region virtual methods
    public virtual void Die() { }

    public virtual void GetDamaged(float value, GameObject attacker) 
    {
        this.lastAttacker = attacker;
        this.healthBar.UpdateHealth(-value);
    }

    public virtual void RestoreHealth(float value)
    {
        this.healthBar.UpdateHealth(value);
    }

    public virtual float GetHealth()
    {
        return this.healthBar.GetHealth();
    }

    public virtual float GetMaxHealth()
    {
        return this.maxHealth;
    }
    
    public virtual GameObject GetLastAttacker()
    {
        return lastAttacker;
    }
#endregion
 

#region hook
    protected virtual void Awake()
    {
        bulletHitEventBinding = new EventBinding<BulletHitEvent>(HandleBulletHit);
    }

    public virtual void Start() 
    {
        healthBar.Initialize(maxHealth);
    }

    public virtual void Update()
    {
        if (this.healthBar.GetHealth() <= 0f)
        {
            if (!isDead) {
                EventBus<EntityDieEvent>.Raise(new EntityDieEvent { entity = gameObject });
                this.Die();
            }
            isDead = true;
        }
    }
#endregion


#region event bindings
    private EventBinding<BulletHitEvent> bulletHitEventBinding;
    
    private void OnEnable()
    {
        EventBus<BulletHitEvent>.Register(bulletHitEventBinding);
    }   

    private void OnDisable()
    {
        EventBus<BulletHitEvent>.Deregister(bulletHitEventBinding);
    }

    private void HandleBulletHit(BulletHitEvent e)
    {
        if (e.target != this.gameObject)
        {
            return;
        }

        this.GetDamaged(e.damage, e.attacker);
        GameManager.Instance.ShowText((-e.damage).ToString(), 100, Color.white, transform.position + new Vector3(0.5f, 1.75f, 0), Vector3.up, 2.0f);
    }
#endregion

    
}
