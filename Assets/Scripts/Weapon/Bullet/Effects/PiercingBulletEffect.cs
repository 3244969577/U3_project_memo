using UnityEngine;
using System.Collections.Generic;

public class PiercingBulletEffect : MonoBehaviour, BulletEffect
{
    private Bullet bullet;
    public int maxPierceCount = 3; // 最大穿透数量
    private List<GameObject> piercedTargets = new List<GameObject>(); // 已穿透的目标

    public void Initialize(Bullet bullet)
    {
        this.bullet = bullet;
    }

    public void OnShoot()
    {
        // 穿透效果在碰撞时触发，射击时不需要处理
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        // 只处理可伤害的目标
        if (collision.gameObject.GetComponent<Damageable>())
        {
            // 检查是否已经穿透过这个目标
            if (!piercedTargets.Contains(collision.gameObject))
            {
                // 添加到已穿透列表
                piercedTargets.Add(collision.gameObject);
                
                // 检查是否达到最大穿透数量
                if (piercedTargets.Count >= maxPierceCount)
                {
                    // 达到最大穿透数量，销毁子弹
                    if (bullet.hitEffect != null)
                    {
                        GameObject effect = Instantiate(bullet.hitEffect, bullet.transform.position, Quaternion.identity);
                        Destroy(effect, 1f);
                    }
                    Destroy(bullet.gameObject);
                }
                else
                {
                    // 继续穿透，忽略碰撞
                    Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>(), true);
                }
            }
        }
    }

    public void Update()
    {
        // 不需要持续更新
    }
}
