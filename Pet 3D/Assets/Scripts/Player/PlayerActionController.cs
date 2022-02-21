using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionController : MonoBehaviour
{
    [SerializeField] Shop shop;

    [SerializeField] Canvas playerCanvas;
    [SerializeField] Image playerActionImage;

    float petInteractionDistance = 2f;
    Pet pet;
    bool isWithinPetInteractionDistance;
    bool isWithinShopInteractionDistance;

    public bool IsWithinPetInteractionDistance
    {
        get { return isWithinPetInteractionDistance; }
        set
        {
            isWithinPetInteractionDistance = value;
        }
    }

    public bool IsWithinShopInteractionDistance
    {
        get { return isWithinShopInteractionDistance; }
        set
        {
            isWithinShopInteractionDistance = value;
        }
    }


    [SerializeField] List<Sprite> actionSprites = new List<Sprite>();

    private void Awake()
    {
        pet = FindObjectOfType<Pet>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistanceBetweenPlayerAndPet();

        if (IsWithinPetInteractionDistance)
        {
            if (Input.GetKeyDown(KeyCode.F))
                Pet();
        }
        if (IsWithinShopInteractionDistance)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (shop)
                {
                    shop.Toggle();
                }
            }
        }

        UpdatePlayerCanvas();
    }

    private void UpdatePlayerCanvas()
    {
        if (IsWithinShopInteractionDistance)
        {
            playerActionImage.sprite = actionSprites[0];
            playerActionImage.color = Color.white;
        }
        else if (IsWithinPetInteractionDistance)
        {
            playerActionImage.sprite = actionSprites[0];
            playerActionImage.color = Color.white;
        }
        else
            playerActionImage.color = Color.clear;
    }

    private void CheckDistanceBetweenPlayerAndPet()
    {
        float dist = Vector3.Distance(transform.position, pet.transform.position);
        if (dist <= petInteractionDistance)
            IsWithinPetInteractionDistance = true;
        else
            IsWithinPetInteractionDistance = false;
    }

    void Pet()
    {
        pet.PetPet();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shop"))
        {
            IsWithinShopInteractionDistance = true;
            shop = other.GetComponentInParent<Shop>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Shop"))
        {
            IsWithinShopInteractionDistance = false;
            shop = null;
        }
    }
}
