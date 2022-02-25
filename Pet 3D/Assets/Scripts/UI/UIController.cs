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
    [SerializeField] ShopWindow shopWindow;

    [SerializeField] Image playerActionImage;
    [SerializeField] Sprite actionSprite;

    private void Awake()
    {
        pet = FindObjectOfType<Pet>();
        player = FindObjectOfType<Player>();
        IsOpen = false;
        eventWindow.SetActive(false);
        shopWindow.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePetInfo();
        UpdatePlayerAction();
        //if (!IsOpen)
        //{
        //    if (player.lookedAtObject && Input.GetKeyDown(KeyCode.F))
        //    {
        //        if (player.lookedAtObject.CompareTag("Shop"))
        //        {
        //            OpenShopWindow();
        //        }
        //        else if (player.lookedAtObject.CompareTag("Event"))
        //        {
        //            OpenEventWindow();
        //        }
        //    }
        //}
        //else
        //{

        //}
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
        Player.ReleaseCursor();
        Player.CanMove = false;
        IsOpen = true;
    }
    private void OnWindowClose()
    {
        Player.LockCursor();
        Player.CanMove = true;
        IsOpen = false;
    }
    public static bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void UpdatePlayerAction()
    {
        if (player.lookedAtObject)
        {
            playerActionImage.sprite = actionSprite;
            playerActionImage.color = Color.white;
        }
        else
            playerActionImage.color = Color.clear;
    }
}
