using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SkillContestManager : MonoBehaviour
{
    [SerializeField] Transform skillContestantSlotParent;
    [SerializeField] List<ContestantSlot> contestantSlots;

    [SerializeField] Transform skillContestantParent;
    [SerializeField] SkillContestant skillContestantPrefab;
    List<SkillContestant> contestants;

    [SerializeField] List<TMP_Text> contestantScoreTexts;

    Dictionary<SkillContestant, string> contestantActions;
    Dictionary<SkillContestant, TMP_Text> contestantScores;

    TMP_Text instruction;
    [SerializeField] List<string> instructions;

    List<Button> buttons;
    List<Slider> sliders;

    Button startButton;
    Slider timeLeftSlider;

    bool running;
    float quickEndingdt;
    public float rounddt;
    public float roundTimer;
    public float timeLeft;

    public int numberOfContestants;
    int contestantActionsReceived;

    public delegate void OnRoundStart();
    public event OnRoundStart onRoundStart;

    public delegate void OnRoundEnd();
    public event OnRoundStart onRoundEnd;

    public string GetCurrentInstruction() { return instruction.text; }
    public string GetInstructionInIndex(int i) { if (i < instructions.Count) return instructions[i]; else return null; }
    public int GetInstructionCount() { return instructions.Count; }

    private void Awake()
    {
        contestantActions = new Dictionary<SkillContestant, string>();
        contestantScores = new Dictionary<SkillContestant, TMP_Text>();

        InitializeContestantSlots();
        InitializeContestants();
        GetUIElements();

        int i = 0;
        foreach (var contestant in contestants)
        {
            contestantScores.Add(contestant, contestantScoreTexts[i]);
            contestantScoreTexts[i].name = contestant.Name + " Score";
            i++;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUIElements();
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            rounddt += Time.deltaTime;

            UpdateUI();

            if (contestantActionsReceived == contestants.Count)
            {
                quickEndingdt += Time.deltaTime;

                if (quickEndingdt > 1f)
                {
                    Time.timeScale = 3f;
                    quickEndingdt = 0f;
                }
            }

            if (rounddt > roundTimer)
            {
                EndRound();
                rounddt = 0f;
            }
        }
    }

    private void InitializeContestantSlots()
    {

    }

    private void InitializeContestants()
    {
        contestants = new List<SkillContestant>();
        SkillContestant contestant = null;
        ContestantSlot freeSlot = null;
        Vector3 spawnPos = new Vector3();
        spawnPos.y = 10f;

        int playerPet = Random.Range(0, numberOfContestants);

        for (int i = 0; i < numberOfContestants; i++)
        {
            //constestant = Instantiate(beautyContestantPrefab, beautyContestantParent);
            contestant = Instantiate(skillContestantPrefab, spawnPos, Quaternion.identity, skillContestantParent);
            foreach (var slot in contestantSlots)
            {
                if (slot.IsFree)
                    freeSlot = slot;
            }

            contestant.Initialize(this, freeSlot);
            contestant.GetComponent<Rigidbody>().isKinematic = true;

            if (i == playerPet)
            {
                contestant.SetStats(Persistent.petStats);
            }
            else
            {
                PetStats stats = new PetStats();
                stats.name = "AI Pet " + i.ToString();
                stats.intellect = Random.Range(1, 3);

                contestant.SetStats(stats);
            }

            contestants.Add(contestant);
        }

        StartCoroutine(ReleasContestants());
    }
    private void GetUIElements()
    {
        instruction = GameObject.FindGameObjectWithTag("BeautyInstruction").GetComponent<TMP_Text>();

        buttons = new List<Button>();
        buttons.AddRange(FindObjectsOfType<Button>());

        sliders = new List<Slider>();
        sliders.AddRange(FindObjectsOfType<Slider>());
    }

    private void SetUIElements()
    {
        foreach (var b in buttons)
        {
            if (b.name.Contains("StartButton"))
                startButton = b;
        }

        foreach (var s in sliders)
        {
            if (s.name.Contains("TimeLeftSlider"))
                timeLeftSlider = s;
        }

        startButton.interactable = false;

        timeLeftSlider.maxValue = roundTimer;
        timeLeftSlider.value = timeLeftSlider.maxValue;
    }

    private void UpdateUI()
    {
        if (running)
        {
            timeLeft = roundTimer - rounddt;
            timeLeftSlider.value = timeLeft;
        }

        if (!running)
        {
            foreach (var contestant in contestants)
            {
                contestantScores[contestant].text = contestant.Name + ": " + contestant.GetScore().ToString();
            }
        }
    }

    private void NewInstruction()
    {
        if (instructions.Count == 0)
            return;

        int randomInstruction = Random.Range(0, instructions.Count);

        instruction.text = instructions[randomInstruction];
    }

    public void SetContestantAction(SkillContestant contestant, string action)
    {
        if (!contestantActions.ContainsKey(contestant))
            contestantActions.Add(contestant, action);

        contestantActions[contestant] = action;
        contestantActionsReceived++;
    }

    private void CheckContestantActions()
    {
        foreach (var pair in contestantActions)
        {
            if (pair.Value.Contains(instruction.text))
                pair.Key.AddScore(1);
        }

        UpdateUI();

        contestantActionsReceived = 0;
    }

    private void EndRound()
    {
        Time.timeScale = 1f;
        running = false;
        startButton.interactable = true;

        onRoundEnd();

        CheckContestantActions();
        NewInstruction();
    }

    public void StartRound()
    {
        running = true;
        startButton.interactable = false;

        onRoundStart();
    }

    public void ReturnHome()
    {
        SceneManager.LoadScene(0);
    }

    private IEnumerator ReleasContestants()
    {
        foreach (var contestant in contestants)
        {
            contestant.GetComponent<Rigidbody>().isKinematic = false;
            contestant.Release();

            yield return new WaitForSeconds(2f);
        }

        startButton.interactable = true;

        yield return null;
    }
}
