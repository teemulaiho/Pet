using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [SerializeField] BattleScoreScreen scoreScreen;

    [SerializeField] List<Battler> battlers;
    [SerializeField] Battler battlerPrefab;
    [SerializeField] Transform playerPetParent;
    [SerializeField] Transform opponentPetParent;

    Battler playerPet;
    Battler opponentPet;

    private bool battleOver;
    public static bool running;
    bool isPlayerPetTurn;
    public static int round = 0;
    int turn = 0;

    float turnDT = 0f;
    float turnDuration = 5f;

    float waitDT = 0f;
    float waitDuration = 2f;

    private int prizePool;

    private void Awake()
    {
        battlers = new List<Battler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        prizePool = 125;

        playerPet = Instantiate(battlerPrefab, playerPetParent);
        opponentPet = Instantiate(battlerPrefab, opponentPetParent);

        {
            PetStats stats = new PetStats();
            stats.name = "AI Pet";
            stats.intellect = Random.Range(1, 3);
            stats.strength = Random.Range(1f, 6f);
            opponentPet.SetStats(stats);
        }

        playerPet.SetStats(Persistent.petStats);
        playerPet.SetPlayerPet(true);

        battlers.Add(playerPet);
        battlers.Add(opponentPet);

        foreach (var battler in battlers)
        {
            battler.Initialize(this);
        }

        BattleUI.SetNames(battlers);
        BattleUI.UpdateHealthValues(battlers);
    }

    // Update is called once per frame
    void Update()
    {
        if (running && !battleOver)
        {
            turnDT += Time.deltaTime;

            if (turnDT > turnDuration)
            {
                if (isPlayerPetTurn)
                {
                    playerPet.DoDamage(opponentPet);
                }
                else
                {
                    opponentPet.DoDamage(playerPet);
                }

                isPlayerPetTurn = !isPlayerPetTurn;

                BattleUI.UpdateTurnText(battlers, isPlayerPetTurn);
                BattleUI.UpdateHealthValues(battlers);

                turn++;
                if (turn < 2)
                    Invoke("UpdateTurnAnimation", 1.5f);
                //BattleUI.UpdateTurnAnimaton(isPlayerPetTurn);

                if (turn >= 2)
                {
                    round++;
                    running = false;
                    turn = 0;

                    BattleUI.UpdateControlButtons();

                    Invoke("ResetTurnAnimations", 1f);
                    //BattleUI.ResetTurnAnimations();
                }

                turnDT = 0f;
            }
        }

        if (battleOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                SceneManager.LoadScene("HomeScene");
        }
    }

    public void StartBattle()
    {
        running = true;
        if (round == 0)
            isPlayerPetTurn = (1 == Random.Range(0, 2));

        BattleUI.UpdateTurnAnimaton(isPlayerPetTurn);
        BattleUI.UpdateTurnText(battlers, isPlayerPetTurn);
        BattleUI.UpdateControlButtons();
    }

    public void ReturnHome()
    {
        SceneManager.LoadScene(0);
    }

    public void OnAnimationStart()
    {

    }

    public void OnAnimationEnd()
    {

    }

    private void UpdateTurnAnimation()
    {
        BattleUI.UpdateTurnAnimaton(isPlayerPetTurn);
    }

    private void ResetTurnAnimations()
    {
        BattleUI.ResetTurnAnimations();
    }

    public void BattlerUnconscious()
    {
        battleOver = true;

        Invoke("EndBattle", 2f);
    }

    private void EndBattle()
    {
        ResetTurnAnimations();
        UpdateContestantRanks();
        DistributeContestantRewards();
        DistributePlayerPetExperience();
        scoreScreen.gameObject.SetActive(true);
        battleOver = true;
    }

    private int SortByWinnings(Battler a, Battler b)
    {
        return a.Winnings.CompareTo(b.Winnings);
    }

    private void UpdateContestantRanks()
    {
        SetContestantWinnings();

        battlers.Sort(SortByWinnings); // Sort from smallest to largest.
        battlers.Reverse(); // Higher score is better.

        int i = 0;
        foreach (Battler battler in battlers)
        {
            scoreScreen.namePlates[i].time.text = "";
            scoreScreen.namePlates[i].petName.text = battler.Name;
            scoreScreen.namePlates[i].place.text = (i + 1).ToString();
            scoreScreen.namePlates[i].winnings.text = battler.Winnings.ToString();
            i++;
        }

        for (int j = i; j < scoreScreen.namePlates.Length; j++)
        {
            scoreScreen.namePlates[j].gameObject.SetActive(false);
        }
    }
    private void SetContestantWinnings()
    {
        foreach (Battler battler in battlers)
        {
            if (battler.RelativeCurrentHealth > 0)
                battler.Winnings = prizePool;
        }
    }

    private void DistributeContestantRewards()
    {
        foreach (Battler battler in battlers)
        {
            if (battler.isPlayerPet)
                if (Persistent.playerInventory != null)
                    Persistent.playerInventory.IncreaseMoney(battler.Winnings);
        }
    }

    private void DistributePlayerPetExperience()
    {
        foreach (Battler battler in battlers)
        {
            if (battler.isPlayerPet)
            {
                if (battler.Winnings > 0)
                    Persistent.AddExperience(10f);
            }
        }
    }
}
