using UnityEngine;

public class GeneratorTrigger : MonoBehaviour
{

    private bool isActive = false;
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
                settingPanel.SetActive(true);
                GameManager.Instance.PauseGame();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                settingPanel.SetActive(false);
                GameManager.Instance.ResumeGame();
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
