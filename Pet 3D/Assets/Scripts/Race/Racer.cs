using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racer : MonoBehaviour
{
    private Animator animator;

    private float maxStamina;
    private float stamina;
    private float staminaDrainRate;
    private float staminaRechargeRate;
    private bool resting;

    private float maxSpeed;
    private float minSpeed;
    private float speed;
    private float targetSpeed;
    private float acceleration;
    private float accelerationBoost;

    private bool running;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        resting = false;
        speed = 0f;
    }

    private void Update()
    {
        if (running)
        {
            UpdateSpeed();

            Vector3 newPosition = transform.position;
            newPosition.x += speed * Time.deltaTime;
            transform.position = newPosition;

            UpdateStamina();
        }
    }

    private void UpdateStamina()
    {
        if (resting)
        {
            stamina += staminaRechargeRate * Time.deltaTime;
            if (stamina >= maxStamina)
            {
                stamina = maxStamina;
                Rest(false);
            }
        }
        else
        {
            stamina -= staminaDrainRate * Time.deltaTime;
            if (stamina <= 0)
            {
                stamina = 0f;
                Rest(true);
            }
        }

        stamina = Mathf.Clamp(stamina, 0f, maxStamina);
    }

    private void UpdateSpeed()
    {
        float totalAcceleration = acceleration + accelerationBoost;
        if (speed < targetSpeed)
            speed += totalAcceleration * Time.deltaTime;
        else if (speed > targetSpeed)
            speed -= totalAcceleration * Time.deltaTime;
        else
            speed = targetSpeed;
    }

    private void Rest(bool value)
    {
        if (value)
        {
            targetSpeed = minSpeed;
        }
        else
        {
            targetSpeed = maxSpeed;
            if (accelerationBoost > 0f)
                accelerationBoost = 0f;
        }
    }

    public void Release()
    {
        targetSpeed = maxSpeed;
        animator.SetBool("isMoving", true);
        running = true;
    }

    public void SetStats(PetStats stats)
    {
        maxStamina = stats.health;
        stamina = maxStamina;
        staminaDrainRate = 20.0f - stats.stamina;
        staminaRechargeRate = 10.0f + stats.stamina;
        
        maxSpeed = 3.0f;
        minSpeed = 2.0f;
        acceleration = 0.5f + 0.05f * stats.stamina;
        accelerationBoost = 1.0f;
    }
}
