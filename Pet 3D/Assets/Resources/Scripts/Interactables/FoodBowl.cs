using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBowl : MonoBehaviour
{
    public bool autoSpawn;
    [SerializeField] Transform foodPosition;

    float timer = 0f;
    float respawnTime = 3f;

    private void Update()
    {
        if (foodPosition.childCount == 0)
        {
            if (timer < respawnTime)
            {
                timer += Time.deltaTime;

                if (timer > respawnTime)
                    timer = respawnTime;
            }
            else
            {
                Food food = Instantiate(Persistent.itemDatabase.ItemByName("Apple").prefab, foodPosition.position, Quaternion.identity).GetComponent<Food>();
                food.transform.parent = foodPosition.transform;
                food.rb.isKinematic = true;

                timer = 0f;
            }
        }
    }
}
