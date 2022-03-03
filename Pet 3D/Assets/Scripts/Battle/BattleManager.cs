using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [SerializeField] List<Battler> battlers;
    [SerializeField] Battler battlerPrefab;
    [SerializeField] Transform playerPetParent;
    [SerializeField] Transform opponentPetParent;

    Battler playerPet;
    Battler opponentPet;

    public static bool running;
    bool isPlayerPetTurn;
    public static int round = 0;
    int turn = 0;

    float turnDT = 0f;
    float turnDuration = 5f;

    float waitDT = 0f;
    float waitDuration = 2f;

    private void Awake()
    {
        battlers = new List<Battler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerPet = Instantiate(battlerPrefab, playerPetParent);
        opponentPet = Instantiate(battlerPrefab, opponentPetParent);

        playerPet.SetPlayerPet(true);

        battlers.Add(playerPet);
        battlers.Add(opponentPet);

        battlers[0].Name = "Player Pet";
        battlers[1].Name = "AI Pet";

        foreach(var battler in battlers)
        {
            battler.Initialize(this);
        }

        BattleUI.SetNames(battlers);
        BattleUI.UpdateHealthValues(battlers);
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
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
                BattleUI.UpdateTurnAnimaton(isPlayerPetTurn);

                turn++;
                if (turn >= 2)
                {
                    round++;
                    running = false;
                    turn = 0;

                    BattleUI.UpdateControlButtons();
                    BattleUI.ResetTurnAnimations();
                }

                turnDT = 0f;
            }
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
}
