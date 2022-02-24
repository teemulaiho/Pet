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

    public bool IsOpen { get; set; }

    [Space]
    [Header("Pet Info")]
    [SerializeField] Slider healthBar;
    [SerializeField] Slider energyBar; 
    [SerializeField] TMP_Text petStateText;
    [SerializeField] GameObject eventWindow;

    [SerializeField] GameObject itemUIParent;
    [SerializeField] TMP_Text itemCountText;

    [SerializeField] Image playerActionImage;
    [SerializeField] List<Sprite> actionSprites = new List<Sprite>();

    private void Awake()
    {
        pet = FindObjectOfType<Pet>();
        player = FindObjectOfType<Player>();
        IsOpen = false;
        eventWindow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePetInfo();
        UpdatePlayerCAction();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (IsOpen)
            {
                Player.LockCursor();
                Player.CanMove = true;
                CloseUI();
                IsOpen = false;
            }
            else
            {
                Player.ReleaseCursor();
                Player.CanMove = false;
                OpenUI();
                IsOpen = true;
            }
        }
    }

    private void UpdatePetInfo()
    {
        UpdatePetStateInfo();
        UpdateSlider(healthBar, pet.GetCurrentRelativeHealth());
        UpdateSlider(energyBar, pet.GetCurrentRelativeEnergy());
    }

    private void UpdatePetStateInfo()
    {
        if (pet)
        {
            petStateText.text = pet.GetCurrentState().ToString();
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

    public void OpenUI()
    {
        eventWindow.SetActive(true);
    }
    public void CloseUI()
    {
        eventWindow.SetActive(false);
    }

    public static bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void UpdatePlayerCAction()
    {
        if (player.IsWithinShopInteractionDistance)
        {
            playerActionImage.sprite = actionSprites[0];
            playerActionImage.color = Color.white;
        }
        else if (player.IsWithinPetInteractionDistance)
        {
            playerActionImage.sprite = actionSprites[0];
            playerActionImage.color = Color.white;
        }
        else
            playerActionImage.color = Color.clear;
    }
}
