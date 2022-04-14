using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    public static List<Animator> animators;
    public static Animator animator;

    public static Animator damageAnimator;

    public static List<Button> controlButtons;

    public static List<Slider> petHealthSliders;
    public static List<TMP_Text> uiTexts;

    static TMP_Text turnText;

    static TMP_Text playerPetName;
    static TMP_Text opponentPetName;

    static TMP_Text playerDamageTakenText;
    static TMP_Text opponentDamageTakenText;

    private void Awake()
    {
        animators = new List<Animator>();
        animators.AddRange(GetComponentsInChildren<Animator>());

        animator = GetComponent<Animator>();

        foreach(Animator anim in animators)
        {
            if (anim.name.Contains("Damage"))
            {
                damageAnimator = anim;
            }
        }

        controlButtons = new List<Button>();
        petHealthSliders = new List<Slider>();
        uiTexts = new List<TMP_Text>();

        controlButtons.AddRange(GetComponentsInChildren<Button>());
        petHealthSliders.AddRange(GetComponentsInChildren<Slider>());
        uiTexts.AddRange(GetComponentsInChildren<TMP_Text>());
    }

    private void Start()
    {
        foreach (var text in uiTexts)
        {
            if (text.name.Contains("PlayerPetName"))
            {
                playerPetName = text;
            }
            else if (text.name.Contains("PlayerPetDamageTaken"))
            {
                playerDamageTakenText = text;
            }
            else if (text.name.Contains("OpponentPetName"))
            {
                opponentPetName = text;
            }
            else if (text.name.Contains("OpponentPetDamageTaken"))
            {
                opponentDamageTakenText = text;
            }
            else if (text.name.Contains("Turn"))
            {
                turnText = text;
                turnText.gameObject.SetActive(false);
            }
        }
    }

    public static void SetNames(List<Battler> battlers)
    {
        int i = 0;
        foreach (var name in uiTexts)
        {
            if (name.name.Contains("Name"))
            {
                name.text = battlers[i].Name;
                i++;
            }
        }
    }

    public static void UpdateHealthValues(List<Battler> battlers)
    {
        int i = 0;

        foreach (var slider in petHealthSliders)
        {
            slider.value = battlers[i].RelativeCurrentHealth;
            i++;
        }
    }

    public static void UpdateTurnText(List<Battler> battlers, bool isPlayerTurn)
    {
        string currentTurnName = null;

        if (isPlayerTurn)
            currentTurnName = battlers[0].Name;
        else
            currentTurnName = battlers[1].Name;

        turnText.text = currentTurnName;

        if (!turnText.gameObject.activeSelf)
            turnText.gameObject.SetActive(true);
    }

    public static void UpdateDamageTakenUI(bool isPlayerPetTurn, float damageTaken)
    {
        //Debug.Log("Called UpdateDamageTakenUI with parameter isPlayerTurn: " + isPlayerPetTurn);

        int damageTakenInt = Mathf.CeilToInt(damageTaken);

        string damageTakenString = damageTakenInt.ToString();

        if (isPlayerPetTurn)
        {
            //animator.SetTrigger("PlayerPetTakeDamage");
            damageAnimator.SetTrigger("PlayerPetTakeDamage");
            playerDamageTakenText.text = damageTakenString;
        }
        else
        {
            //animator.SetTrigger("OpponentPetTakeDamage");
            damageAnimator.SetTrigger("OpponentPetTakeDamage");
            opponentDamageTakenText.text = damageTakenString;
        }
    }

    public static void UpdateTurnAnimaton(bool isPlayerPetTurn)
    {
        //Debug.Log("Called UpdateTurnAnimation with parameter isPlayerTurn: " + isPlayerPetTurn);

        if (isPlayerPetTurn)
        {
            animator.SetBool("OpponentPetTurn", false);
            animator.SetBool("PlayerPetTurn", true);
        }
        else
        {
            animator.SetBool("PlayerPetTurn", false);
            animator.SetBool("OpponentPetTurn", true);
        }
    }

    public static void ResetTurnAnimations()
    {
        //Debug.Log("Called ResetTurnAnimations");

        animator.SetBool("OpponentPetTurn", false);
        animator.SetBool("PlayerPetTurn", false);
    }

    /// <summary>
    /// Changes button interactable depending on if running or !running.
    /// Changes button text depending on which round it is.
    /// </summary>
    public static void UpdateControlButtons()
    {
        foreach (var button in controlButtons)
        {
            if (button.name.Contains("Start"))
            {
                button.interactable = !BattleManager.running;

                if (BattleManager.round != 0)
                    button.GetComponentInChildren<TMP_Text>().text = "Continue";

            }
        }
    }
}
