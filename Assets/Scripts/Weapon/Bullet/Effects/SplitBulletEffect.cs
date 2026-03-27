using UnityEngine;

public class SplitBulletEffect : MonoBehaviour, BulletEffect
{
    private Bullet bullet;
    public int splitCount = 3; // 分裂数量
    public float angleRange = 30f; // 角度范围
    public GameObject bulletPrefab; // 子弹预制体

    public void Initialize(Bullet bullet)
    {
        this.bullet = bullet;
    }

    public void OnShoot()
    {
        if (splitCount <= 1 || bulletPrefab == null)
            return;

        Debug.Log("SplitBulletEffect OnShoot");
        // 计算基础方向和速度
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        Vector2 direction = bulletRb.linearVelocity.normalized;
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float speed = bulletRb.linearVelocity.magnitude;

        // 生成分裂子弹
        for (int i = 0; i < splitCount; i++)
        {
            // 计算每个子弹的角度，确保均匀分布且不包含原方向
            // 角度范围平均分配，排除原方向
            float angleStep = angleRange / splitCount;
            float angle;
            
            if (i < splitCount / 2)
            {
                // 左侧角度
                angle = baseAngle - angleRange / 2 + angleStep * (i + 0.5f);
            }
            else
            {
                // 右侧角度
                angle = baseAngle + angleStep * (i - splitCount / 2 + 0.5f);
            }
            
            float radians = angle * Mathf.Deg2Rad;
            Vector2 splitDirection = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

            // 实例化子弹
            GameObject splitBulletObj = Instantiate(bulletPrefab, bullet.transform.position, Quaternion.identity);
            Bullet splitBullet = splitBulletObj.GetComponent<Bullet>();
            
            if (splitBullet != null)
            {
                // 设置速度
                Rigidbody2D rb = splitBulletObj.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = splitDirection * speed;
                }
                
                // 触发分裂子弹的射击效果
                splitBullet.OnShoot();
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        // 分裂子弹效果在射击时触发，碰撞时不需要处理
    }

    public void Update()
    {
        // 不需要持续更新
    }
}
