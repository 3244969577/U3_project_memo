using UnityEngine;

public class GeneratorTrigger : MonoBehaviour
{

    private bool isActive = false;
    private bool isOpen = false;
    public GameObject settingPanel;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Assert(settingPanel != null, "settingPanel is null");
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                isOpen = !isOpen;
                settingPanel.SetActive(isOpen);
                if (isOpen)
                {
                    GameManager.Instance.PauseGame();
                }
                else
                {
                    GameManager.Instance.ResumeGame();
                }
            }
        }
        else
        {
            settingPanel.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        isActive = true;
        Debug.Log("entity entered the trigger");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger");
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        isActive = false;
        Debug.Log("entity exited the trigger");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited the trigger");
        }
    }
}
