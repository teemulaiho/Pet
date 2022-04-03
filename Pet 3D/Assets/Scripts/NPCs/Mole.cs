using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mole : NPC
{
    public DialogueTrigger dialogueTrigger;

    private void Start()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
    }

    public override void Interact()
    {
        Debug.Log("Overriding NPC Interact() with Mole.Interact()");

        dialogueTrigger.TriggerDialogue();
    }
}
