using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    CanvasGroup canvasGroup;
    Button quitButton;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        quitButton = GetComponentInChildren<Button>();
        quitButton.onClick.AddListener(QuitGame);
    }

    void QuitGame()
    {
        Application.Quit();
    }

    public bool SettingsWindowOpen()
    {
        return canvasGroup.alpha == 1f;
    }

    public void ToggleSettingsUI()
    {
        if (canvasGroup.alpha == 0f)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    transform.GetChild(i).gameObject.SetActive(!transform.GetChild(i).gameObject.activeSelf);
        //}
    }
}
