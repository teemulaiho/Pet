using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SkillContestManager : MonoBehaviour
{
    [SerializeField] SkillContestScoreScreen scoreScreen;

    [SerializeField] Transform skillContestantSlotParent;
    [SerializeField] List<ContestantSlot> contestantSlots;

    [SerializeField] Transform skillContestantParent;
    [SerializeField] SkillContestant skillContestantPrefab;
    List<SkillContestant> contestants;

    [SerializeField] List<TMP_Text> uiTexts;
    [SerializeField] List<TMP_Text> contestantScoreTexts;

    TMP_Text roundInfo;

    Dictionary<SkillContestant, string> contestantActions;
    Dictionary<SkillContestant, TMP_Text> contestantScores;

    TMP_Text instruction;
    [SerializeField] List<string> instructions;

    List<Button> buttons;
    List<Slider> sliders;

    Button startButton;
    Slider timeLeftSlider;

    bool contestOver;
    bool running;
    float quickEndingdt;
    public float rounddt;
    public float roundTimer;
    public float timeLeft;

    private int prizePool;

    public int numberOfRounds;
    private int currentRound = 0;
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
        prizePool = 225;

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
            contestantScoreTexts[i].text = contestant.name;
            i++;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUIElements();
        UpdateUI();
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
                if (currentRound <= numberOfRounds)
                {
                    EndRound();
                    rounddt = 0f;
                }
            }
        }

        if (contestOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                SceneManager.LoadScene("HomeScene");
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
                contestant.isPlayerPet = true;
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
        uiTexts = new List<TMP_Text>();
        uiTexts.AddRange(FindObjectsOfType<TMP_Text>());

        foreach (var text in uiTexts)
        {
            if (text.name.Contains("RoundInfoText"))
                roundInfo = text;
        }


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
                contestantScores[contestant].text = contestant.Name + ": " + "\n" + contestant.GetScore().ToString();
            }

            roundInfo.text = "Rounds Left: " + "\n" + (numberOfRounds - currentRound).ToString();
        }
    }

    private int SortByScore(SkillContestant a, SkillContestant b)
    {
        return a.GetScore().CompareTo(b.GetScore());
    }

    private void UpdateContestantRanks()
    {
        contestants.Sort(SortByScore); // Sort from smallest to largest.
        contestants.Reverse(); // Higher score is better.

        int i = 0;
        foreach (SkillContestant contestant in contestants)
        {
            i++;
            contestant.Rank = i;
        }

        SetContestantWinnings();

        i = 0;
        foreach (SkillContestant contestant in contestants)
        {
            scoreScreen.namePlates[i].time.text = contestant.GetScore().ToString();
            scoreScreen.namePlates[i].petName.text = contestant.name;
            scoreScreen.namePlates[i].place.text = (i + 1).ToString();
            scoreScreen.namePlates[i].winnings.text = contestant.Winnings.ToString();
            i++;
        }

        for (int j = i; j < scoreScreen.namePlates.Length; j++)
        {
            scoreScreen.namePlates[j].gameObject.SetActive(false);
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

        if (currentRound >= numberOfRounds)
        {
            running = false;
            EndGame();
        }
        else
            NewInstruction();

        UpdateUI();
    }

    private void EndGame()
    {
        UpdateContestantRanks();
        DistributeContestantRewards();
        DistributePlayerPetExperience();
        scoreScreen.gameObject.SetActive(true);

        contestOver = true;
    }

    public void StartRound()
    {
        running = true;
        startButton.interactable = false;
        currentRound++;

        onRoundStart();
    }

    public void ReturnHome()
    {
        SceneManager.LoadScene(0);
    }

    private void SetContestantWinnings()
    {
        foreach (SkillContestant contestant in contestants)
        {
            if (contestant.GetScore() == 0)
                contestant.Winnings = 0;
            else
                contestant.Winnings = prizePool / (contestant.Rank);
        }
    }

    private void DistributeContestantRewards()
    {
        foreach (SkillContestant contestant in contestants)
        {
            if (contestant.isPlayerPet)
                if (Persistent.playerInventory != null)
                    Persistent.playerInventory.IncreaseMoney(contestant.Winnings);
        }
    }

    private void DistributePlayerPetExperience()
    {
        foreach (SkillContestant contestant in contestants)
        {
            if (contestant.isPlayerPet)
            {
                if (contestant.Rank > 0)
                    Persistent.AddExperience(10f);
            }
        }
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
