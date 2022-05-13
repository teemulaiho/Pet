using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    MouseLock mouseLock;

    private Queue<string> sentences;

    public GameObject dialogueBox;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Button dialogueChoiceButton;

    int dialogueLines = 0;
    int playerChoiceLine = -1;

    [SerializeField] string sentenceBeingTyped;
    [SerializeField] bool isTyping;
    Coroutine typeSentence;

    EventManager eventManager;

    private void Awake()
    {
        mouseLock = FindObjectOfType<MouseLock>();
        eventManager = FindObjectOfType<EventManager>();
    }

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

        mouseLock.LockCursor();
    }

    public void StartDialogue(Dialogue dialogue, int dialoguePath = 0)
    {
        OpenDialogueBox();

        nameText.text = dialogue.name;
        sentences.Clear();

        if (dialoguePath == 0)
        {
            foreach (string sentence in dialogue.sentences)
            {
                sentences.Enqueue(sentence);
            }
        }
        else if (dialoguePath == 1)
        {
            foreach (string sentence in dialogue.sentences2)
            {
                sentences.Enqueue(sentence);
            }
        }

        DisplayNextSentence();
    }

    /// <summary>
    /// Return true if not end of dialogue. Return false if end of dialogue.
    /// </summary>
    /// <returns></returns>
    public bool DisplayNextSentence()
    {
        if (dialogueLines == playerChoiceLine && !dialogueChoiceButton.gameObject.activeSelf)
        {
            ActivatePlayerChoiceButton();
        }
        else if (sentences.Count == 0)
        {
            EndDialogue();
            return false;
        }

        if (!isTyping)
        {
            if (sentences.Count != 0)
            {
                string sentence = sentences.Dequeue();

                if (typeSentence != null)
                    StopCoroutine(typeSentence);

                typeSentence = StartCoroutine(TypeSentence(sentence, 0.008f, dialogueLines == playerChoiceLine));
            }
            dialogueLines++;
        }
        else
        {
            if (typeSentence != null)
                StopCoroutine(typeSentence);

            dialogueText.text = sentenceBeingTyped;
            isTyping = false;
        }

        return true;
    }

    void ActivatePlayerChoiceButton()
    {
        dialogueChoiceButton.gameObject.SetActive(true);
        mouseLock.ReleaseCursor(false);
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

            if (eventManager)
            {
                Mole mole = FindObjectOfType<Mole>();
                if (mole)
                    dialogueChoiceButton.onClick.AddListener(() => eventManager.StartEvent(mole.GetCurrentEvent()));
            }
            //else
            //    dialogueChoiceButton.onClick.AddListener(() => SceneLoader.LoadScene(sceneToLoad));
        }
    }

    public void SetButtonMethod(string contestName, int lineToActivatePlayerChoice)
    {
        playerChoiceLine = lineToActivatePlayerChoice;

        dialogueChoiceButton.onClick.RemoveAllListeners();

        string sceneToLoad = "";

        if (contestName.Contains("Race"))
        {
            sceneToLoad = "RaceScene";
            dialogueChoiceButton.GetComponentInChildren<TMP_Text>().text = "Start Race";
        }
        else if (contestName.Contains("Skill"))
        {
            sceneToLoad = "SkillContestScene";
            dialogueChoiceButton.GetComponentInChildren<TMP_Text>().text = "Start Skill Contest";
        }
        else if (contestName.Contains("Battle"))
        {
            sceneToLoad = "BattleScene";
            dialogueChoiceButton.GetComponentInChildren<TMP_Text>().text = "Start Battle";
        }


        if (eventManager)
        {
            Mole mole = FindObjectOfType<Mole>();
            if (mole)
                dialogueChoiceButton.onClick.AddListener(() => eventManager.StartEvent(mole.GetCurrentEvent()));
        }
        //else
        //    dialogueChoiceButton.onClick.AddListener(() => SceneLoader.LoadScene(sceneToLoad));
    }

    IEnumerator TypeSentence(string sentence, float typeSpeed, bool activatePlayerChoice)
    {
        sentenceBeingTyped = sentence;
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        if (activatePlayerChoice)
            ActivatePlayerChoiceButton();

        isTyping = false;
    }

    void EndDialogue()
    {
        CloseDialogueBox();
    }
}
