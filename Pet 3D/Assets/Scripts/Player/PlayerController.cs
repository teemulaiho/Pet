using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] FoodSpawner foodSpawner;
    [SerializeField] BallThrower ballThrower;

    private void Awake()
    {
        foodSpawner = GetComponent<FoodSpawner>();
        ballThrower = GetComponent<BallThrower>();
    }

    // Start is called before the first frame update
    void Start()
    {
        foodSpawner.enabled = false;
        ballThrower.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Note: Unity Editor view doesn't update the visuals (ie. checkmark) of which script gets enabled.   
        {
            if (!foodSpawner.enabled)
                foodSpawner.enabled = true;

            ballThrower.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!ballThrower.enabled)
                ballThrower.enabled = true;

            foodSpawner.enabled = false;
        }
    }
}
