using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuController : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private CanvasGroup pauseMenuCanvasGroup, winStateCanvasGroup, bloodOutlineCanvasGroup;
    [SerializeField] private TextMeshProUGUI winStateTitle, winStateSubtitle;
    [SerializeField] private Image winStateImage;
    [SerializeField] private Sprite[] winStateSprites;
    private bool _isPaused;

    private void Start()
    {
        playerInputHandler.onPause.AddListener(PauseGame);
    }

    private void OnDisable()
    {
        playerInputHandler.onPause.RemoveListener(PauseGame);
    }

    private void PauseGame()
    {
        _isPaused = !_isPaused;
        Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _isPaused;
        pauseMenuCanvasGroup.alpha = _isPaused ? 1 : 0;
        pauseMenuCanvasGroup.blocksRaycasts = _isPaused;
        pauseMenuCanvasGroup.interactable = _isPaused;
        Time.timeScale = _isPaused ? 0 : 1;
    }
    
    public void ChangeWinStateUI(bool isWin)
    {
        playerInputHandler.onPause.RemoveListener(PauseGame);
        bloodOutlineCanvasGroup.alpha = isWin ? 0 : 1;
        winStateTitle.text = isWin ? "Made that Cheese" : "Eaten";
        winStateSubtitle.text = isWin ? "You Win" : "You Lose";
        winStateImage.sprite = winStateSprites[isWin ? 1 : 0];
        winStateCanvasGroup.DOFade(1, 2f).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            winStateCanvasGroup.blocksRaycasts = true;
            winStateCanvasGroup.interactable = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        });
        
    }
}