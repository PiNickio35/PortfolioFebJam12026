using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private bool _isPaused;
    [SerializeField] private CanvasGroup pauseMenuCanvasGroup;

    private void Start()
    {
        //FirstPersonController.Instance.triggerPauseMenu.AddListener(PauseGame);
    }

    private void OnDisable()
    {
        //FirstPersonController.Instance.triggerPauseMenu.RemoveListener(PauseGame);
    }

    public void PauseGame()
    {
        _isPaused = !_isPaused;
        pauseMenuCanvasGroup.alpha = _isPaused ? 1 : 0;
        pauseMenuCanvasGroup.blocksRaycasts = _isPaused;
        pauseMenuCanvasGroup.interactable = _isPaused;
        Time.timeScale = _isPaused ? 0 : 1;
    }
}