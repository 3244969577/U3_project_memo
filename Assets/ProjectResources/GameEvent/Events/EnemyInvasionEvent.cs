using UnityEngine;

/// <summary>
/// 敌人入侵事件
/// 在随机位置生成敌人
/// </summary>
public class EnemyInvasionEvent : RandomEvent
{
    [Header("敌人入侵配置")]
    public GameObject enemyPrefab; // 敌人预制体
    public int minEnemyCount = 2; // 最小敌人数量
    public int maxEnemyCount = 5; // 最大敌人数量
    public float spawnRadius = 10f; // 生成半径

    public override void Trigger()
    {
        if (enemyPrefab == null || Player.instance == null)
            return;

        // 在玩家周围随机位置生成敌人
        Vector3 playerPos = Player.instance.transform.position;
        int enemyCount = Random.Range(minEnemyCount, maxEnemyCount + 1);

        for (int i = 0; i < enemyCount; i++)
        {
            // 随机生成位置
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(5f, spawnRadius);
            Vector3 spawnPos = playerPos + new Vector3(
                Mathf.Cos(angle) * distance,
                Mathf.Sin(angle) * distance,
                0f
            );

            // 生成敌人
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }

        // 显示事件消息
        GameManager.instance.ShowText("敌人入侵！", 120, Color.red, Player.instance.transform.position + new Vector3(0, 2, 0), Vector3.up, 3.0f);
    }

    public override float CalculateProbability()
    {
        // 可以根据游戏进度或其他因素调整概率
        return baseProbability;
    }
}
