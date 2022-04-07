using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class UIController : MonoBehaviour
{
    private Pet pet;
    private Player player;

    private MouseLock mouseLock;

    [Space]
    [Header("Pet Info")]
    [SerializeField] TMP_Text petName;
    [SerializeField] TMP_Text petLevel;
    [SerializeField] Slider healthBar;
    [SerializeField] Slider energyBar;
    [SerializeField] TMP_Text petStateText;
    [SerializeField] GameObject eventWindow;
    [SerializeField] ShopWindow shopWindow;

    [Space]
    [Header("Pet Stats")]
    [SerializeField] TMP_Text petIntelligence;
    [SerializeField] TMP_Text petStrength;
    [SerializeField] TMP_Text petStamina;
    [SerializeField] TMP_Text petExperience;

    [Space]
    [Header("Player")]
    [SerializeField] Transform playerUIActionParent;
    [SerializeField] Image playerActionImage;
    [SerializeField] TMP_Text playerActionText;
    [SerializeField] Sprite actionSprite;

    [SerializeField] Transform playerUIAction2Parent;
    [SerializeField] Image playerAction2Background;
    [SerializeField] Image playerAction2Image;
    [SerializeField] TMP_Text playerAction2Text;
    [SerializeField] Sprite action2Sprite;

    [Space]
    [Header("Other")]
    [SerializeField] MailBox mailbox; // the gameobject, not the UI object

    SettingsMenu settingsMenu;

    private void Awake()
    {
        mouseLock = FindObjectOfType<MouseLock>();
        settingsMenu = FindObjectOfType<SettingsMenu>();
        pet = FindObjectOfType<Pet>();
        player = FindObjectOfType<Player>();
        eventWindow.SetActive(false);
        shopWindow.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (player)
            player.onGameObjectInteraction += OnObjectInteract;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePetInfo();
        UpdatePlayerAction();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (shopWindow.gameObject.activeSelf)
                CloseShopWindow();
            else if (eventWindow.activeSelf)
                CloseEventWindow();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!settingsMenu.SettingsWindowOpen())
            {
                OnWindowOpen();
                settingsMenu.ToggleSettingsUI();
            }
            else if (settingsMenu.SettingsWindowOpen())
            {
                OnWindowClose();
                settingsMenu.ToggleSettingsUI();
            }
        }
    }

    private void UpdatePetInfo()
    {
        UpdatePetStateInfo();
        UpdateSlider(healthBar, pet.HealthPercentage);
        UpdateSlider(energyBar, pet.EnergyPercentage);
    }

    private void UpdatePetStateInfo()
    {
        if (pet)
        {
            petName.text = Persistent.petStats.name;
            petLevel.text = Persistent.petStats.level.ToString();
            petStateText.text = pet.GetState().ToString();
            petIntelligence.text = Persistent.petStats.intellect.ToString();
            petStrength.text = Persistent.petStats.strength.ToString();
            petStamina.text = Persistent.petStats.stamina.ToString();
            petExperience.text = Persistent.petStats.experience.ToString();
        }
    }

    private void UpdateSlider(Slider slider, float value)
    {
        slider.value = Mathf.Clamp(value, 0.15f, 1.0f);

        Image fillImage = slider.fillRect.GetComponent<Image>();

        if (slider.value > 0.5f)
            fillImage.color = Color.Lerp(Color.yellow, Color.green, (slider.value - 0.5f) * 2.0f);
        else
            fillImage.color = Color.Lerp(Color.red, Color.yellow, slider.value * 2.0f);
    }

    public void OpenShopWindow()
    {
        shopWindow.gameObject.SetActive(true);
        OnWindowOpen();
    }
    public void CloseShopWindow()
    {
        shopWindow.gameObject.SetActive(false);
        OnWindowClose();
    }
    public void OpenEventWindow()
    {
        eventWindow.SetActive(true);
        OnWindowOpen();
    }
    public void CloseEventWindow()
    {
        eventWindow.SetActive(false);
        OnWindowClose();
    }

    private void OnWindowOpen()
    {
        player.ReleaseCursor();
        player.CanMove = false;
    }
    private void OnWindowClose()
    {
        player.LockCursor();
        player.CanMove = true;
    }
    public static bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void UpdatePlayerAction()
    {
        if (player.lookedAtObject)
        {
            if (player.lookedAtObject.CompareTag("Pet"))
            {
                playerActionText.text = "Pet";
            }
            else if (player.lookedAtObject.CompareTag("Shop"))
            {
                playerActionText.text = "Open Shop";
            }
            else if (player.lookedAtObject.CompareTag("Event"))
            {
                playerActionText.text = "Open Event";
            }
            else if (player.lookedAtObject.CompareTag("NPC"))
            {
                playerActionText.text = "Talk";
            }
            else if (player.lookedAtObject.CompareTag("Ball"))
            {
                playerAction2Image.sprite = action2Sprite;

                playerActionText.text = "Pickup";
                playerAction2Text.text = "Kick";

                playerAction2Image.color = Color.white;
                playerUIAction2Parent.GetComponent<CanvasGroup>().alpha = 0.75f;
            }
            else if (player.lookedAtObject.CompareTag("Mailbox"))
            {
                playerActionText.text = "Open Mailbox";
            }

            playerActionImage.sprite = actionSprite;
            playerUIActionParent.GetComponent<CanvasGroup>().alpha = 0.75f;
            //playerActionImage.color = Color.white;
        }
        else
        {
            playerActionText.text = "";
            playerAction2Text.text = "";
            playerUIActionParent.GetComponent<CanvasGroup>().alpha = 0f;
            playerUIAction2Parent.GetComponent<CanvasGroup>().alpha = 0f;
            //playerActionImage.color = Color.clear;
        }
    }

    void OnObjectInteract(string objectInteractedWith)
    {
        Debug.Log("Interacted with " + objectInteractedWith);

        if (objectInteractedWith.Contains("Mailbox"))
        {
            if (mailbox.ToggleMailBoxUI())
                mouseLock.ReleaseCursor();
            else
                mouseLock.LockCursor();
        }
    }
}
