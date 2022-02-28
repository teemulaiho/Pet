using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeautyManager : MonoBehaviour
{
    [SerializeField] Transform beautyContestantSlotParent;
    [SerializeField] List<ContestantSlot> contestantSlots;

    [SerializeField] Transform beautyContestantParent;
    [SerializeField] BeautyContestant beautyContestantPrefab;
    List<BeautyContestant> constestants;

    TMP_Text instruction;
    [SerializeField] List<string> instructions;

    List<Button> buttons;
    List<Slider> sliders;

    Button startButton;
    Slider timeLeftSlider;

    bool running;
    public float rounddt;
    public float roundTimer;
    public float timeLeft;

    public int numberOfContestants;

    public delegate void OnRoundStart();
    public event OnRoundStart onRoundStart;

    public delegate void OnRoundEnd();
    public event OnRoundStart onRoundEnd;

    public string GetCurrentInstruction() { return instruction.text; }


    private void Awake()
    {
        InitializeContestantSlots();
        InitializeContestants();
        GetUIElements();
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
        constestants = new List<BeautyContestant>();

        BeautyContestant constestant = null;
        ContestantSlot freeSlot = null;
        for (int i = 0; i < numberOfContestants; i++)
        {
            constestant = Instantiate(beautyContestantPrefab, beautyContestantParent);

            foreach (var slot in contestantSlots)
            {
                if (slot.IsFree)
                    freeSlot = slot;
            }

            constestant.Initialize(this, freeSlot);
            constestants.Add(constestant);
        }
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

        timeLeftSlider.maxValue = roundTimer;
        timeLeftSlider.value = timeLeftSlider.maxValue;
    }

    private void UpdateUI()
    {
        timeLeft = roundTimer - rounddt;
        timeLeftSlider.value = timeLeft;
    }

    private void NewInstruction()
    {
        if (instructions.Count == 0)
            return;

        int randomInstruction = Random.Range(0, instructions.Count);

        instruction.text = instructions[randomInstruction];
    }

    private void EndRound()
    {
        NewInstruction();

        running = false;
        startButton.interactable = true;

        onRoundEnd();
    }

    public void StartRound()
    {
        running = true;
        startButton.interactable = false;

        onRoundStart();
    }
}
