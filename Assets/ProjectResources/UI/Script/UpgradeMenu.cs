// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// public class UpgradeMenu : MonoBehaviour
// {
//     private int upgradeFactor = 1;
//     Transform healthTransform;
//     Transform speedTransform;
//     Text healthText;
//     Text speedText;
//     Player player;

//     public int HEALTH_UPGRADE_PRICE = 100;
//     public int SPEED_UPGRADE_PRICE = 100;

//     private void Awake()
//     {
//         transform.gameObject.SetActive(false);
//         gameObject.SetActive(false);
//     }

//     protected void Start()
//     {
//         Transform frameTransform = transform.Find("HeroAttributes");
//         healthTransform = frameTransform.Find("Attributes").Find("HealthAttribute").Find("HealthAmount");
//         speedTransform = frameTransform.Find("Attributes").Find("SpeedAttribute").Find("SpeedAmount");
//         healthText = healthTransform.gameObject.GetComponent<Text>();
//         speedText = speedTransform.gameObject.GetComponent<Text>();
        
//         if (GameManager.Instance.player != null)
//         {
//             player = GameManager.Instance.player.GetComponent<Player>();
//         }
        
//         UpdateValues();
//     }

//     private void UpdateValues()
//     {
//         if (player != null)
//         {
//             healthText.text = "HEALTH: " + ((int)player.GetMaxHealth()).ToString();
//             speedText.text = "SPEED: " + ((int)player.speed).ToString();
//         }
//     }
    
//     public void UpgradeHealth()
//     {
//         if (player == null) return;
        
//         int coin = GameManager.Instance.GetCoin();
//         if (coin >= HEALTH_UPGRADE_PRICE)
//         {
//             player.maxHealth += upgradeFactor;
//             GameManager.Instance.coin -= HEALTH_UPGRADE_PRICE;
//             UpdateValues();
//         }
//     }
    
//     public void UpgradeSpeed()
//     {
//         if (player == null) return;
        
//         int coin = GameManager.Instance.GetCoin();
//         if (coin >= SPEED_UPGRADE_PRICE)
//         {
//             player.speed += upgradeFactor;
//             GameManager.Instance.coin -= SPEED_UPGRADE_PRICE;
//             UpdateValues();
//         }
//     }

//     public void Toggle()
//     {
//         if (transform.gameObject.activeSelf)
//         {
//             Cursor.visible = false;
//             this.transform.gameObject.SetActive(false);
//             this.gameObject.SetActive(false);
//             GameManager.Instance.UnFreezeAllMovement();
//         }
//         else
//         {
//             Cursor.visible = true;
//             this.gameObject.SetActive(true);
//             this.transform.gameObject.SetActive(true);
//             GameManager.Instance.FreezeAllMovement();
//         }
//     }
// }
