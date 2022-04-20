using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailBox : MonoBehaviour
{
    UIController uiController;

    [SerializeField] Transform mailBoxUI;
    [SerializeField] Transform skillTreeUI;
    string newName;

    public delegate void OnWindowOpen(Transform openedObject);
    public event OnWindowOpen onWindowOpen;

    public delegate void OnWindowClose(Transform closedObject);
    public event OnWindowClose onWindowClose;

    public delegate void OnWindowToggle(Transform toggledObject);
    public event OnWindowToggle onWindowToggle;

    private void Awake()
    {
        uiController = FindObjectOfType<UIController>();
    }

    private void Start()
    {
        onWindowOpen += WindowOpenEvent;
        onWindowClose += WindowCloseEvent;
        onWindowToggle += WindowToggleEvent;
    }

    public void UpdateNewName(string name)
    {
        newName = name;
    }

    public void SetNewPetName()
    {
        if (newName.Length != 0)
            Persistent.petStats.name = newName;
    }

    public bool ToggleMailBoxUI()
    {
        mailBoxUI.gameObject.SetActive(!mailBoxUI.gameObject.activeSelf);

        onWindowToggle(mailBoxUI);
        return mailBoxUI.gameObject.activeSelf;
    }
    
    public void ToggleSkillTreeUI()
    {
        uiController.OpenUIWindow(skillTreeUI.gameObject);

        //skillTreeUI.gameObject.SetActive(!skillTreeUI.gameObject.activeSelf);
        //onWindowToggle(skillTreeUI);
    }

    private void WindowOpenEvent(Transform openedObject)
    {

    }

    private void WindowCloseEvent(Transform closedObject)
    {
        if (closedObject.gameObject.name.Contains("Mailbox"))
            CloseAllOpenWindows();
        else if (closedObject.gameObject.name.Contains("Skill"))
        {

        }
    }

    private void WindowToggleEvent(Transform toggledObject)
    {
        if (!toggledObject.gameObject.activeSelf)
            onWindowClose(toggledObject);
        else
            onWindowOpen(toggledObject);
    }

    private void CloseAllOpenWindows()
    {
        mailBoxUI.gameObject.SetActive(false);
        skillTreeUI.gameObject.SetActive(false);
    }
}
