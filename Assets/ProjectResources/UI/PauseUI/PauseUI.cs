using UnityEngine;
using GlobalEvents;

public class PauseUI : MonoBehaviour
{
    public GameObject pauseUIPanel;
    private EventBinding<GamePauseEvent> pauseBinding;
    private EventBinding<GameResumeEvent> resumeBinding;

#region EventBindings
    private void Awake()
    {
        pauseUIPanel.SetActive(false);
        pauseBinding = new EventBinding<GamePauseEvent>(OnPause);
        resumeBinding = new EventBinding<GameResumeEvent>(OnResume);
    }
    private void OnEnable()
    {
        EventBus<GamePauseEvent>.Register(pauseBinding);
        EventBus<GameResumeEvent>.Register(resumeBinding);
    }
    private void OnDisable()
    {
        EventBus<GamePauseEvent>.Deregister(pauseBinding);
        EventBus<GameResumeEvent>.Deregister(resumeBinding);
    }

    private void OnPause()
    {
        pauseUIPanel.SetActive(true);
    }
    private void OnResume()
    {
        pauseUIPanel.SetActive(false);
    }
#endregion

}
