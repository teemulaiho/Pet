using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;

    public GameObject dialogueBox;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Button dialogueChoiceButton;

    int dialogueLines = 0;
    int playerChoiceLine = -1;

    Coroutine typeSentence;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void OpenDialogueBox()
    {
        dialogueBox.SetActive(true);
    }

    public void CloseDialogueBox()
    {
        dialogueBox.SetActive(false);
        DeActivatePlayerChoiceButton();
        playerChoiceLine = -1;
        dialogueLines = 0;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        OpenDialogueBox();

        nameText.text = dialogue.name;
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    /// <summary>
    /// Return true if not end of dialogue. Return false if end of dialogue.
    /// </summary>
    /// <returns></returns>
    public bool DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return false;
        }

        string sentence = sentences.Dequeue();
        dialogueLines++;


        if (typeSentence != null)
            StopCoroutine(typeSentence);

        typeSentence = StartCoroutine(TypeSentence(sentence, 0.016f, dialogueLines == playerChoiceLine));

        return true;
    }

    void ActivatePlayerChoiceButton()
    {
        dialogueChoiceButton.gameObject.SetActive(true);
    }

    void DeActivatePlayerChoiceButton()
    {
        dialogueChoiceButton.gameObject.SetActive(false);
    }

    public void SetButtonMethod(int methodType, int lineToActivatePlayerChoice)
    {
        playerChoiceLine = lineToActivatePlayerChoice;

        dialogueChoiceButton.onClick.RemoveAllListeners();

        if (methodType == 0)
        {
            string sceneToLoad = "RaceScene";
            dialogueChoiceButton.onClick.AddListener(() => SceneLoader.LoadScene(sceneToLoad));
        }
    }

    IEnumerator TypeSentence(string sentence, float typeSpeed, bool activatePlayerChoice)
    {
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        if (activatePlayerChoice)
            ActivatePlayerChoiceButton();
    }

    void EndDialogue()
    {
        Debug.Log("End of conversation.");

        CloseDialogueBox();
    }
}
