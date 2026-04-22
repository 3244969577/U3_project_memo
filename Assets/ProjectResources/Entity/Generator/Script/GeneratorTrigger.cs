using UnityEngine;
using EasyUI.Toast;

public class GeneratorTrigger : MonoBehaviour
{

    private bool isActive = false;
    private bool isOpen = false;
    public GameObject settingPanel;

    // private KeyCode triggerKey;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Debug.Assert(settingPanel != null, "settingPanel is null");
        // triggerKey = HotKeyManager.Instance.HotKeySetting.NPCGenerationPanelKey;
    }

    // Update is called once per frame
    void Update()
    {
        // if (isActive)
        // {
        //     if (Input.GetKeyDown(triggerKey))
        //     {
        //         isOpen = !isOpen;
        //         settingPanel.SetActive(isOpen);
        //         if (isOpen)
        //         {
        //             GameManager.Instance.PauseGame();
        //         }
        //         else
        //         {
        //             GameManager.Instance.ResumeGame();
        //         }
        //     }
        // }
        // else
        // {
        //     settingPanel.SetActive(false);
        // }
    }


#region 公开方法
    public void OpenPanel()
    {
        isOpen = true;
        settingPanel.SetActive(isOpen);
        GameManager.Instance.PauseGame();
    }
    public void ClosePanel()
    {
        isOpen = false;
        settingPanel.SetActive(isOpen);
        GameManager.Instance.ResumeGame();
    }
#endregion


    void OnMouseDown()
    {
        OpenPanel();
    }

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     isActive = true;
    // }
    
    // void OnTriggerExit2D(Collider2D other)
    // {
    //     isActive = false;
    // }

    private void OnDrawGizmos()
    {
        // 绘制触发器的Gizmo
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        Gizmos.color = isActive ? Color.red : Color.green;
    }
}
