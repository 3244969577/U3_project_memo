using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 未使用的Boss触发区域 - 生成Boss敌人
/// - 继承自EnemyTriggerZone
/// - 重写SpawnEnemies方法，使用随机位置生成敌人
/// - 敌人从指定半径范围内的随机位置出现
/// </summary>
public class BossTriggerZone : EnemyTriggerZone
{

    private void SpawnEnemies()
    {
        foreach (GameObject obj in enemyPrefabs)
        {
            Vector3 position = GetRandomPosition();
            Instantiate(obj, transform.position + position, transform.rotation);
        }
    }

    private Vector3 GetRandomPosition()
    {
        return Random.insideUnitCircle * this.radiusRange;
    }
}
