using UnityEngine;
using System.Collections.Generic;

public class BulletEffectManager : MonoBehaviour
{
    private List<BulletEffect> effects = new List<BulletEffect>();
    private Bullet bullet;

    public void Initialize(Bullet bullet)
    {
        this.bullet = bullet;
        
        // 获取所有附加的效果组件
        BulletEffect[] effectComponents = GetComponents<BulletEffect>();
        // Debug.Log("BulletEffectManager Initialize, effect count: " + effectComponents.Length);
        foreach (var effect in effectComponents)
        {
            effect.Initialize(bullet);
            effects.Add(effect);
        }
        // Debug.Log("BulletEffectManager Initialized, effects count: " + effects.Count);
    }

    public void OnShoot()
    {
        foreach (var effect in effects)
        {
            effect.OnShoot();
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (var effect in effects)
        {
            effect.OnCollisionEnter2D(collision);
        }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        foreach (var effect in effects)
        {
            // 创建一个碰撞事件对象用于效果管理器
            Collision2D collisionEvent = new Collision2D();
            effect.OnCollisionEnter2D(collisionEvent);
        }
    }

    public void Update()
    {
        foreach (var effect in effects)
        {
            effect.Update();
        }
    }
}
