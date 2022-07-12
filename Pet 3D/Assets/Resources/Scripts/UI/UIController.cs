using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Linq;

public class UIController : MonoBehaviour
{
    private Pet pet;
    private Player player;

    private MouseLock mouseLock;

    [Space]
    [Header("Shop & Event Windows")]
    [SerializeField] EventWindow eventWindow;
    [SerializeField] ShopWindow shopWindow;

    [Space]
    [Header("Pet Info")]
    [SerializeField] TMP_Text petName;
    [SerializeField] TMP_Text petLevel;
    [SerializeField] Slider healthBar;
    [SerializeField] Slider hungerBar;
    [SerializeField] Slider energyBar;
    [SerializeField] Slider happinessBar;
    [SerializeField] TMP_Text petStateText;

    [Space]
    [Header("Pet Stats")]
    [SerializeField] Transform petInfoUIParent;
    [SerializeField] TMP_Text petIntelligence;
    [SerializeField] TMP_Text petStrength;
    [SerializeField] TMP_Text petStamina;

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

    [SerializeField] bool quantitySliderActive;
    [SerializeField] Slider quantitySlider;
    [SerializeField] Button purchaseButton;
    [SerializeField] RectTransform selectedUIButton;
    [SerializeField] List<Button> uiButtons;

    public EventWindow GetEventWindow() { return eventWindow; }
    public ShopWindow GetShopWindow() { return shopWindow; }

    private void Awake()
    {
        mouseLock = FindObjectOfType<MouseLock>();
        pauseMenu = FindObjectOfType<PauseMenu>();
        pet = FindObjectOfType<Pet>();
        player = FindObjectOfType<Player>();

        mailBoxManager = FindObjectOfType<MailBoxManager>();
        skillTreeManager = FindObjectOfType<SkillTreeManager>();

        openWindows = new List<GameObject>();

        uiButtons = new List<Button>();
        uiButtons.AddRange(FindObjectsOfType<Button>());
    }

    private void Start()
    {
        mailBoxManager.gameObject.SetActive(false);
        skillTreeManager.gameObject.SetActive(false);

        pauseMenu?.gameObject.SetActive(false);

        if (player)
            player.onGameObjectInteraction += OnObjectInteract;

        eventWindow.gameObject.SetActive(false);
        shopWindow.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePetInfo();
        UpdatePlayerAction();

        if (Input.GetKeyDown(KeyCode.Escape))
            CloseUIWindw(null, true);

        if (Input.GetKeyDown(KeyCode.I))
            Toggle(pauseMenu.gameObject, true);

        if (Input.GetKeyDown(KeyCode.Q))
            ToggleAnimation(petInfoUIParent.gameObject);

        if (Input.GetKeyDown(KeyCode.Return))
            Select();

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            ChangeSelectedObject(-1);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ChangeSelectedObject(1);

        if (Input.GetKeyDown(KeyCode.UpArrow))
            ChangeSelectedObject(5);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            ChangeSelectedObject(-5);
    }

    private void ChangeSelectedObject(int v)
    {
        if (uiButtons.Count == 0)
            return;

        if (quantitySliderActive)
        {
            quantitySlider.value += v;
            return;
        }

        if (!selectedUIButton)
            selectedUIButton = uiButtons[0].GetComponent<RectTransform>();

        int currentIndex = -1;
        if (selectedUIButton.TryGetComponent<Button>(out Button b))
            currentIndex = uiButtons.IndexOf(b);

        if (Mathf.Abs(v) < 2)
            currentIndex -= v;
        else
            currentIndex += v;

        if (currentIndex >= uiButtons.Count)
            currentIndex = 0;
        else if (currentIndex < 0)
            currentIndex = uiButtons.Count - 1;

        if (selectedUIButton.transform.Find("Border"))
            selectedUIButton.transform.Find("Border").gameObject.SetActive(false);
        else
            selectedUIButton.transform.GetChild(0).gameObject.SetActive(false);

        selectedUIButton = uiButtons[currentIndex].GetComponent<RectTransform>();

        if (selectedUIButton.transform.Find("Border"))
            selectedUIButton.transform.Find("Border").gameObject.SetActive(true);
        else
            selectedUIButton.transform.GetChild(0).gameObject.SetActive(true);

        if (shopWindow)
        {
            if (selectedUIButton.TryGetComponent(out ShopItemSlot itemSlot))
            {
                shopWindow.SelectSlot(itemSlot);
            }
        }
    }

    private void Select()
    {
        if (!selectedUIButton)
            return;

        if (selectedUIButton.TryGetComponent(out Button b))
        {
            if (quantitySliderActive)
            {
                purchaseButton.onClick.Invoke();
                quantitySlider.value = quantitySlider.minValue;
            }
            else
                b.onClick.Invoke();

            if (b.onClick.GetPersistentEventCount() > 0)
            {
                if (b.onClick.GetPersistentMethodName(0).Contains("Select"))
                {
                    quantitySliderActive = !quantitySliderActive;
                }
            }
        }
    }

    private void UpdatePetInfo()
    {
        UpdatePetStateInfo();
        UpdateSlider(healthBar, pet.HealthPercentage);
        UpdateSlider(hungerBar, pet.HungerPercentage);
        UpdateSlider(energyBar, pet.EnergyPercentage);
        UpdateSlider(happinessBar, pet.HappinessPercentage);
    }

    private void UpdatePetStateInfo()
    {
        if (pet)
        {
            petName.text = Persistent.petStats.name;
            petLevel.text = Persistent.petStats.level.ToString();
            petStateText.text = pet.State.ToString();
            petIntelligence.text = Persistent.petStats.intellect.ToString();
            petStrength.text = Persistent.petStats.strength.ToString();
            petStamina.text = Persistent.petStats.stamina.ToString();
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

    public bool OpenUIWindow(GameObject windowToOpen)
    {
        if (windowToOpen.name.Contains("Shop") || windowToOpen.CompareTag("Shop"))
            windowToOpen = shopWindow.gameObject;
        else if (windowToOpen.name.Contains("Event") || windowToOpen.CompareTag("Event"))
            windowToOpen = eventWindow.gameObject;

        windowToOpen.gameObject.SetActive(true);

        if (!openWindows.Contains(windowToOpen))
            openWindows.Add(windowToOpen);

        OnWindowOpen();

        return windowToOpen.activeSelf;
    }

    public bool CloseUIWindw(GameObject windowToClose, bool closeAllOpenWindows = false)
    {
        if (windowToClose != null)
        {
            if (windowToClose.name.Contains("Shop") || windowToClose.CompareTag("Shop"))
            {
                windowToClose = shopWindow.gameObject;
                quantitySlider.value = 0;
                quantitySliderActive = false;
            }
            else if (windowToClose.name.Contains("Event") || windowToClose.CompareTag("Event"))
                windowToClose = eventWindow.gameObject;

            windowToClose.gameObject.SetActive(false);
        }

        if (closeAllOpenWindows)
        {
            foreach (var window in openWindows)
                window.gameObject.SetActive(false);

            openWindows.Clear();
        }
        else
        {
            if (openWindows.Contains(windowToClose))
                openWindows.Remove(windowToClose);
        }

        if (openWindows.Count == 0)
            OnWindowClose();

        if (windowToClose)
            return windowToClose.activeSelf;
        else
            return false;
    }

    private void OnWindowOpen()
    {
        player.DeselectItem();
        player.ReleaseCursor();

        uiButtons.Clear();

        List<Button> unsortedList = new List<Button>();
        unsortedList.AddRange(FindObjectsOfType<Button>());
        
        uiButtons = unsortedList.OrderBy(o => o.name).ToList();
        uiButtons.Reverse();
    }
    private void OnWindowClose()
    {
        player.LockCursor();
    }
    public static bool IsMouseOverUI()
    {
        if (EventSystem.current != null)
            return EventSystem.current.IsPointerOverGameObject();
        else return false;
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
            else if (player.lookedAtObject.CompareTag("Food"))
            {
                playerActionText.text = "Pickup";
            }
            else if (player.lookedAtObject.CompareTag("Tetherball"))
            {
                playerActionText.text = "Push";
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
        }
    }

    void Toggle(GameObject objectToToggle, bool closeAllOpenWindows)
    {
        if (objectToToggle.activeSelf)
            CloseUIWindw(objectToToggle, closeAllOpenWindows);
        else
            OpenUIWindow(objectToToggle);
    }

    void ToggleAnimation(GameObject objectToAnimate)
    {
        if (objectToAnimate.name.Contains("PetInfo"))
        {
            objectToAnimate.GetComponent<Animator>().SetTrigger("Show");
        }
    }
}
