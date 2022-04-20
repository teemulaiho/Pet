using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventWindow : MonoBehaviour
{
    UIController uiController;

    public delegate void OnWindowClose(bool isOpen);
    public event OnWindowClose onWindowClose;

    private void Awake()
    {
        uiController = FindObjectOfType<UIController>();
    }

    private void Start()
    {
        GetComponentInChildren<UIButton>().close += Close;
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    void Close()
    {
        onWindowClose(false);
        uiController.CloseUIWindw(this.gameObject, false);
    }
}
