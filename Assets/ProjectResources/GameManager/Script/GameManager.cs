using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;
using GlobalEvents;
using System.Runtime.CompilerServices;

public class GameManager : Singleton<GameManager>
{
    public SceneData sceneData;
    public Flowchart flowchart;
    public GameObject player;
    public GameObject crosshair;
    private GameObject NPCInteracting;  // 正在与Player对话的NPC

    [Header("游戏暂停设置")]
    [Tooltip("当前游戏是否处于暂停状态")]
    public bool isPaused = false;
    [Tooltip("暂停时的时间缩放值")]
    private float pauseTimeScale = 0f;
    [Tooltip("正常游戏时的时间缩放值")]
    private float normalTimeScale = 1f; 


    private EventBinding<PlayerDieEvent> playerDieBinding;
    private EventBinding<QuitEvent> gameOverBinding;
    private EventBinding<RetryEvent> retryBinding;
    private EventBinding<GamePauseEvent> pauseBinding;
    private EventBinding<GameResumeEvent> resumeBinding;

    private void Awake()
    {
        base.Awake();

        playerDieBinding = new EventBinding<PlayerDieEvent>(handlePlayerDie);
        gameOverBinding = new EventBinding<QuitEvent>(handleQuit);
        retryBinding = new EventBinding<RetryEvent>(handleRetry);
        pauseBinding = new EventBinding<GamePauseEvent>(PauseGame);
        resumeBinding = new EventBinding<GameResumeEvent>(ResumeGame);
    }

    private void OnEnable()
    {
        EventBus<PlayerDieEvent>.Register(playerDieBinding);
        EventBus<QuitEvent>.Register(gameOverBinding);
        EventBus<RetryEvent>.Register(retryBinding);
        EventBus<GamePauseEvent>.Register(pauseBinding);
        EventBus<GameResumeEvent>.Register(resumeBinding);
    }
    private void OnDisable()
    {
        EventBus<PlayerDieEvent>.Deregister(playerDieBinding);
        EventBus<QuitEvent>.Deregister(gameOverBinding);
        EventBus<RetryEvent>.Deregister(retryBinding);
        EventBus<GamePauseEvent>.Deregister(pauseBinding);
        EventBus<GameResumeEvent>.Deregister(resumeBinding);
    }

    
    public FloatingTextManager floatingTextManager;
    [HideInInspector]
    public int coin = 0;
    public int GetCoin()
    {
        return coin;
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


    private void handlePlayerDie(PlayerDieEvent e)
    {
        Debug.Log("Player died!");
        EventBus<GameOverEvent>.Raise(new GameOverEvent());
    }

    private void handleQuit(QuitEvent e)
    {
        Debug.Log("APPLICATION QUIT!");
        SceneManager.LoadScene(sceneData.startSceneName);
    }
    private void handleRetry(RetryEvent e)
    {
        Debug.Log("APPLICATION RETRY!");
        SceneManager.LoadScene(sceneData.mainGameSceneName);
    }


}
