using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    /// <summary>
    /// Return true if not end of dialogue. Return false if end of dialogue.
    /// </summary>
    /// <returns></returns>
    public bool TriggerNextDialogueSentence()
    {
        return FindObjectOfType<DialogueManager>().DisplayNextSentence();
    }
}
