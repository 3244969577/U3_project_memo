
using UnityEngine;

public class GoldChest : Chest
{
    [SerializeField] protected GameObject coinPrefab;   
    public int amount;
    const string COLLECTED_ANIM = "onCollected";

    protected override void OnCollect()
    {
        if (!collected)
        {
            collected = true;
            animator.SetTrigger(COLLECTED_ANIM);
            SoundManager.instance.PlaySound("Chest");
            // 在半径5的范围内随机生成amount个金币
            for (int i = 0; i < amount; i++)
            {
                // 打印transform的位置坐标
                Debug.Log(transform.position);

                GameObject coin = Instantiate(coinPrefab, transform.position + Random.insideUnitSphere * 5f, transform.rotation);
                coin.transform.SetParent(this.transform.parent);
            }
            
        }
    }

    // protected override void OnCollect()
    // {
    //     if (!collected)
    //     {
    //         collected = true;
    //         animator.SetTrigger(COLLECTED_ANIM);
    //         SoundManager.instance.PlaySound("Chest");
    //         string text = "+" + amount.ToString() + " coin";
    //         GameManager.instance.ShowText(text, 100, Color.yellow, transform.position + new Vector3(0.5f, 1.75f, 0), Vector3.up, 2.0f);
    //         GameManager.instance.coin += amount;
    //     }
    // }
}
