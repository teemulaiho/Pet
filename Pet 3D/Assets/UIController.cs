using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIController : MonoBehaviour
{
    Pet pet;
    PlayerController player;


    [Space]
    [Header("Pet Info")]
    [SerializeField] TMP_Text petHealthText; 
    [SerializeField] TMP_Text petEnergyText; 
    [SerializeField] TMP_Text petStateText; 


    private void Awake()
    {
        pet = FindObjectOfType<Pet>();
        player = FindObjectOfType<PlayerController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePetInfo();
    }

    private void UpdatePetInfo()
    {
        UpdatePetHealthInfo();
        UpdatePetEnergyInfo();
        UpdatePetStateInfo();
    }

    private void UpdatePetStateInfo()
    {
        if (pet)
        {
            petStateText.text = "Pet State: " + pet.GetCurrentState().ToString();
        }
    }

    private void UpdatePetHealthInfo()
    {
        if (pet)
        {
            petHealthText.text = "Pet Health: " + pet.GetCurrentRelativeHealth() * 100f + " %";
        }
    }

    private void UpdatePetEnergyInfo()
    {
        if (pet)
        {
            petEnergyText.text = "Pet Energy: " + pet.GetCurrentRelativeEnergy() * 100f + " %";    
        } 
    }
}
