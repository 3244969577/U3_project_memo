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
        // SceneManager.LoadScene(0);
        GameManager.instance.LoadScene(0);
        Debug.Log("GAME REPLAY!");
        gameObject.SetActive(false);

    }

}
