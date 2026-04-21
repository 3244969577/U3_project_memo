using UnityEngine;
using UnityEngine.SceneManagement;
using GlobalEvents;
public class GameOverUI : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("APPLICATION QUIT!");
        EventBus<QuitEvent>.Raise(new QuitEvent());
    }
    public void Retry()
    {
        Debug.Log("APPLICATION RETRY!");
        EventBus<RetryEvent>.Raise(new RetryEvent());
    }

}
