using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    UIController uiController;
    CanvasGroup canvasGroup;
    Button quitButton;

    private void OnEnable()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    private void OnDisable()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    private void Awake()
    {
        uiController = FindObjectOfType<UIController>();
        canvasGroup = GetComponent<CanvasGroup>();
        quitButton = GetComponentInChildren<Button>();
        quitButton.onClick.AddListener(QuitGame);
    }

    private void Start()
    {
        GetComponentInChildren<UIButton>().close += Close;
    }

    void QuitGame()
    {
        Application.Quit();
    }

    public bool SettingsWindowOpen()
    {
        return canvasGroup.alpha == 1f;
    }

    void Close()
    {
        uiController.CloseUIWindw(this.gameObject, true);
    }
}
