using UnityEngine;
using GlobalEvents;

public class EnemyDrop : MonoBehaviour {
    private EventBinding<EnemyKilledEvent> enemyKilledEventBinding;

    [SerializeField] private ProbabilityList<GameObject> dropItems;

    private void Awake()
    {
        enemyKilledEventBinding = new EventBinding<EnemyKilledEvent>(OnEnemyKilled);
    }

    private void OnEnable()
    {
        EventBus<EnemyKilledEvent>.Register(enemyKilledEventBinding);
    }
    private void OnDisable()
    {
        EventBus<EnemyKilledEvent>.Deregister(enemyKilledEventBinding);
    }

    private void OnEnemyKilled(EnemyKilledEvent e)
    {
        if (e.enemy != gameObject)
        {
            return;
        }
        // 敌人被击杀，触发掉落逻辑
        if (dropItems.Count == 0)
        {
            return;
        }
        // 随机掉落一个物品
        GameObject dropItem = dropItems.GetRandomItem();
        if(dropItem != null)
        {
            Instantiate(dropItem, transform.position, Quaternion.identity);
        }
    }
}