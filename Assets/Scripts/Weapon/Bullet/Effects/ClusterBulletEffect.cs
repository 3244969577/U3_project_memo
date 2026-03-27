using UnityEngine;

public class ClusterBulletEffect : MonoBehaviour, BulletEffect
{
    private Bullet bullet;
    public int clusterCount = 4; // 分裂数量
    public float angleRange = 360f; // 角度范围
    public GameObject bulletPrefab; // 子弹预制体

    public void Initialize(Bullet bullet)
    {
        this.bullet = bullet;
    }

    public void OnShoot()
    {
        // 子母弹效果在碰撞时触发，射击时不需要处理
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        // 只在击中目标时触发
        if (collision.gameObject.GetComponent<Damageable>() || collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Destructible")
        {
            if (clusterCount > 0 && bulletPrefab != null)
            {
                // 生成分裂子弹
                for (int i = 0; i < clusterCount; i++)
                {
                    // 计算每个子弹的角度（均匀分布）
                    float angle = (angleRange / clusterCount) * i;
                    float radians = angle * Mathf.Deg2Rad;
                    Vector2 clusterDirection = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

                    // 实例化子弹
                    GameObject clusterBulletObj = Instantiate(bulletPrefab, bullet.transform.position, Quaternion.identity);
                    Bullet clusterBullet = clusterBulletObj.GetComponent<Bullet>();
                    
                    if (clusterBullet != null)
                    {
                        // 复制属性
                        clusterBullet.damage = bullet.damage * 0.7f; // 子子弹伤害降低
                        clusterBullet.hitEffect = bullet.hitEffect;
                        
                        // 设置速度
                        Rigidbody2D rb = clusterBulletObj.GetComponent<Rigidbody2D>();
                        if (rb != null)
                        {
                            float speed = rb.linearVelocity.magnitude * 0.8f; // 子子弹速度稍慢
                            rb.linearVelocity = clusterDirection * speed;
                        }
                        
                        // 触发子子弹的射击效果
                        clusterBullet.OnShoot();
                    }
                }
            }
        }
    }

    public void Update()
    {
        // 不需要持续更新
    }
}
