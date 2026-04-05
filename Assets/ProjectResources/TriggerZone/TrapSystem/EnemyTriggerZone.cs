using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人触发区域 - 当玩家进入时生成敌人
/// - 继承自TriggerZone
/// - 存储敌人预制体数组和生成半径
/// - 触发时会生成所有敌人预制体
/// </summary>
public class EnemyTriggerZone : TriggerZone
{
    public GameObject[] enemyPrefabs;
    public float radiusRange = 5f;


    protected override void Start()
    {
        triggerPoint.OnPlayerEnterTrigger += TriggerTrap;
    }

    private void TriggerTrap(object sender, System.EventArgs e)
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        foreach (GameObject obj in enemyPrefabs)
        {
            GameObject spawn = Instantiate(obj, transform.position, transform.rotation);
            spawn.transform.SetParent(this.transform.parent);
        }
    }
}
