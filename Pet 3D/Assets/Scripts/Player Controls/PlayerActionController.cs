using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionController : MonoBehaviour
{
    [SerializeField] Canvas playerCanvas;
    [SerializeField] Image playerActionImage;
    
    float petInteractionDistance = 2f;
    Pet pet;
    bool isWithinInteractionDistance;



    [SerializeField] List<Sprite> actionSprites = new List<Sprite>();

    private void Awake()
    {
        pet = FindObjectOfType<Pet>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistanceBetweenPlayerAndPet();

        if (isWithinInteractionDistance)
        {
            if (Input.GetKeyDown(KeyCode.F))
                Pet();
        }

        UpdatePlayerCanvas();
    }

    private void UpdatePlayerCanvas()
    {
        if (isWithinInteractionDistance)
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
            isWithinInteractionDistance = true;
        else
            isWithinInteractionDistance = false;
    }

    void Pet()
    {
        pet.PetPet();
    }
}
