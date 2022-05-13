using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public void CloseDialoge()
    {
        FindObjectOfType<DialogueManager>().CloseDialogueBox();
    }

    public void TriggerDialogue(int dialoguePath = 0)
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue, dialoguePath);
    }

    /// <summary>
    /// Return true if not end of dialogue. Return false if end of dialogue.
    /// </summary>
    /// <returns></returns>
    public bool TriggerNextDialogueSentence()
    {
        return FindObjectOfType<DialogueManager>().DisplayNextSentence();
    }


    public void TriggerDialogueChoice(int choiceType)
    {
        FindObjectOfType<DialogueManager>().SetButtonMethod(choiceType, dialogue.activatePlayerChoiceLine);
    }

    public void TriggerDialogueChoice(string contestName)
    {
        FindObjectOfType<DialogueManager>().SetButtonMethod(contestName, dialogue.activatePlayerChoiceLine);
    }
}
