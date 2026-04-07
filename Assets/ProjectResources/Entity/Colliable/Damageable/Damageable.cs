using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameStatusSystem.PlayerStatus.Events;

public class Damageable : Collidable
{
    public float maxHealth;
    protected HealthBar healthBar = new HealthBar();
    protected GameObject lastAttacker; // 记录最后攻击者
    

    public override void Start() 
    {
        healthBar.Initialize(maxHealth);
    }
    public virtual void Update() 
    {

    }

    public virtual void GetDamaged(float value) 
    {
        this.healthBar.UpdateHealth(-value);
        
        // 触发玩家受击事件
        if (gameObject.CompareTag("Player"))
        {
            PlayerHitEvent hitEvent = new PlayerHitEvent {
                attacker = lastAttacker,
                damage = value,
                currentHealth = this.healthBar.GetHealth()
            };
            EventBus<PlayerHitEvent>.Raise(hitEvent);
        }
    }
    
    public virtual void GetDamaged(float value, GameObject attacker) 
    {
        this.lastAttacker = attacker;
        this.healthBar.UpdateHealth(-value);
        
        // 触发玩家受击事件
        if (gameObject.CompareTag("Player"))
        {
            PlayerHitEvent hitEvent = new PlayerHitEvent {
                attacker = attacker,
                damage = value
            };
            EventBus<PlayerHitEvent>.Raise(hitEvent);
        }
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
}
