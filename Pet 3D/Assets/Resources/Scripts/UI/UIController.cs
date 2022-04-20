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
    
    MailBoxManager mailBoxManager;
    SkillTreeManager skillTreeManager;

    PauseMenu pauseMenu;

    List<GameObject> openWindows;

    private void Awake()
    {
        mouseLock = FindObjectOfType<MouseLock>();
        pauseMenu = FindObjectOfType<PauseMenu>();
        pet = FindObjectOfType<Pet>();
        player = FindObjectOfType<Player>();

        mailBoxManager = FindObjectOfType<MailBoxManager>();
        skillTreeManager = FindObjectOfType<SkillTreeManager>();

        eventWindow.SetActive(false);
        shopWindow.gameObject.SetActive(false);

        openWindows = new List<GameObject>();
    }

    private void Start()
    {
        mailBoxManager.gameObject.SetActive(false);
        skillTreeManager.gameObject.SetActive(false);

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
            if (!pauseMenu.SettingsWindowOpen())
            {
                OnWindowOpen();
                pauseMenu.ToggleSettingsUI();
            }
            else if (pauseMenu.SettingsWindowOpen())
            {
                OnWindowClose();
                pauseMenu.ToggleSettingsUI();
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

    public void OpenUIWindow(GameObject windowToOpen)
    {
        windowToOpen.gameObject.SetActive(true);

        if (!openWindows.Contains(windowToOpen))
            openWindows.Add(windowToOpen);

        OnWindowOpen();
    }

    public void CloseUIWindw(GameObject windowToClose, bool closeAllOpenWindows)
    {
        if (closeAllOpenWindows)
        {
            foreach (var window in openWindows)
                window.gameObject.SetActive(false);

            openWindows.Clear();
        }
        else
        {
            windowToClose.gameObject.SetActive(false);

            if (openWindows.Contains(windowToClose))
                openWindows.Remove(windowToClose);
        }

        if (openWindows.Count == 0)
            OnWindowClose();
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

    void OnObjectInteract(GameObject objectInteractedWith)
    {
        Debug.Log("Interacted with " + objectInteractedWith.name);

        if (objectInteractedWith.name.Contains("Mailbox"))
        {
            if (mailBoxManager.gameObject.activeSelf)
                CloseUIWindw(mailBoxManager.gameObject, true);
            else
                OpenUIWindow(mailBoxManager.gameObject);

            //if (mailbox.ToggleMailBoxUI())
            //    mouseLock.ReleaseCursor(false);
            //else
            //    mouseLock.LockCursor();
        }
    }
}
