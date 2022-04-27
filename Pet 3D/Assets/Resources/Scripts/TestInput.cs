using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestInput : MonoBehaviour
{

    DialogueTrigger dialogueTrigger;
    private void Start()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (dialogueTrigger)
                dialogueTrigger.TriggerDialogue();
        }

        if (Input.GetKeyDown(KeyCode.B))
            FindObjectOfType<DialogueManager>().DisplayNextSentence();
    }

}
