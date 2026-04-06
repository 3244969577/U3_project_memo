using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject crosshair;
    public GameObject gameOverUI;
    public GameObject gameWinUI;

    private GameObject NPCInteracting;  // 正在与Player对话的NPC

    public static GameManager instance;
    public static GameManager Instance => instance;

    public Flowchart flowchart;

    [Header("游戏暂停设置")]
    [Tooltip("当前游戏是否处于暂停状态")]
    public bool isPaused = false;
    [Tooltip("暂停时的时间缩放值")]
    private float pauseTimeScale = 0f;
    [Tooltip("正常游戏时的时间缩放值")]
    private float normalTimeScale = 1f; 

    private void Awake()
    {
        if (GameManager.instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        Debug.Assert(instance != null, "GameManager instance is null");
        DontDestroyOnLoad(gameObject);
    }
    
    public FloatingTextManager floatingTextManager;
    [HideInInspector]
    public float playerHealth;
    [HideInInspector]
    public int playerSkin;
    [HideInInspector]
    public int playerWeapon;
    [HideInInspector]
    public int coin = 0;
    [HideInInspector]
    public int speed;

    public void SetPlayerSkin(Sprite sprite)
    {
        if (player != null)
        {
            Player playerComponent = player.GetComponent<Player>();
            if (playerComponent != null && playerComponent.spriteRenderer != null)
            {
                playerComponent.spriteRenderer.sprite = sprite;
            }
        }
    }

    public void FreezeAllMovement()
    {
        if (player != null)
        {
            Player playerComponent = player.GetComponent<Player>();
            if (playerComponent != null)
            {
                playerComponent.FreezeMovement();
            }
        }
    }

    public void UnFreezeAllMovement()
    {
        if (player != null)
        {
            Player playerComponent = player.GetComponent<Player>();
            if (playerComponent != null)
            {
                playerComponent.UnFreezeMovement();
            }
        }
    }

    public int GetCoin()
    {
        return this.coin;
    }

    public void SaveState()
    {
        Debug.Log("Save state");
        string s = "";

        s += "0" + "|";
        s += coin.ToString();

        PlayerPrefs.SetString("SaveState", s);
    }

    public void LoadState()
    {
        Debug.Log("Load state");
    }

    public FloatingText ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        Debug.Log($"ShowText: {msg}");
        if (floatingTextManager != null)
        {
            return floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
        } else {
            Debug.LogError("FloatingTextManager is null");
            return null;
        }
    }


    public void RetryScene()
    {
        if (player != null)
        {
            player.GetComponent<Collider2D>().enabled = false;
        }
        this.FreezeAllMovement();
        gameOverUI.SetActive(true);
    }
    
    public void WinScene()
    {
        if (player != null)
        {
            player.GetComponent<Collider2D>().enabled = false;
        }
        this.FreezeAllMovement();
        gameWinUI.SetActive(true);
    }

    public void LoadScene(int level)
    {
        switch (level)
        {
            case 0:
                SceneManager.LoadScene("MainMenu");
                break;
            case 1:
                SceneManager.LoadScene("Map1");
                break;
            case 2:
                SceneManager.LoadScene("Map2");
                break;
            default:
                SceneManager.LoadScene("Menu");
                break;
        }
    }

    public void SetNPCInteracting(GameObject npc)
    {
        NPCInteracting = npc;
    }
    public GameObject GetNPCInteracting()
    {
        return NPCInteracting;
    }

    #region 游戏暂停/恢复接口

    public void PauseGame()
    {
        if (!isPaused)
        {
            isPaused = true;
            normalTimeScale = Time.timeScale;
            Time.timeScale = pauseTimeScale;
            Debug.Log("游戏已暂停");
        }
    }

    public void ResumeGame()
    {
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = normalTimeScale;
            Debug.Log("游戏已恢复");
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public bool IsGamePaused()
    {
        return isPaused;
    }

    #endregion

}
