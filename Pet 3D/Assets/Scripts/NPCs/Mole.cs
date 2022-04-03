using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mole : NPC
{
    float interactRange = 5f;
    Player player;
    bool dialogueInitiated;
    public DialogueTrigger dialogueTrigger;

    private void Start()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
    }

    void Update()
    {
        if (player)
        {
            if (Vector3.Distance(transform.position, player.transform.position) > interactRange)
            {
                dialogueInitiated = false;
                player = null;
            }
        }
    }

    public override void SetPlayer(Player p)
    {
        player = p;
    }

    public override void Interact()
    {
        Debug.Log("Overriding NPC Interact() with Mole.Interact()");

        if (!dialogueInitiated)
        {
            dialogueTrigger.TriggerDialogue();
            dialogueInitiated = true;
        }
        else
        {
            if (!dialogueTrigger.TriggerNextDialogueSentence())
                dialogueInitiated = false;
        }
    }
}
