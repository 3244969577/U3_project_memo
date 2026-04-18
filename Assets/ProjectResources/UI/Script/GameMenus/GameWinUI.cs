using UnityEngine;
using UnityEngine.SceneManagement;
public class GameWinUI : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("APPLICATION QUIT!");
        Application.Quit();
    }
    public void Replay()
    {
        Debug.Log("GAME REPLAY!");
        gameObject.SetActive(false);
    }

}
