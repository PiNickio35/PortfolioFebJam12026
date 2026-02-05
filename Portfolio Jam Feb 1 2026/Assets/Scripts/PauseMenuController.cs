using System;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private CanvasGroup pauseMenuCanvasGroup;
    private bool _isPaused;

    private void Start()
    {
        playerInputHandler.onPause.AddListener(PauseGame);
    }

    private void OnDisable()
    {
        playerInputHandler.onPause.RemoveListener(PauseGame);
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