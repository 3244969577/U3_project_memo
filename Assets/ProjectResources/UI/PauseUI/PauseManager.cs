using UnityEngine;
using GlobalEvents;

public class PauseManager : MonoBehaviour
{
    private bool isPaused = false;
    public KeyCode pauseKey = KeyCode.Escape;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
            {
                EventBus<GameResumeEvent>.Raise(new GameResumeEvent());
                isPaused = false;
            }
            else
            {
                EventBus<GamePauseEvent>.Raise(new GamePauseEvent());
                isPaused = true;
            }
        }
    }
}
