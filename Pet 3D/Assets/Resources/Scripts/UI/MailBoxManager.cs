using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailBoxManager : MonoBehaviour
{
    UIController uiController;

    private void Awake()
    {
        uiController = FindObjectOfType<UIController>(); 
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<UIButton>().close += Close;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Close()
    {
        uiController.CloseUIWindw(this.gameObject, true);
    }
}
