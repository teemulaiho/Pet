using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racer : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer rend;

    public string Name { get; set; }
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

    private bool running;
    public bool Finished { get; set; }
    public int Rank { get; set; }
    public int Winnings { get; set; }

    public bool isPlayerPet; 

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        resting = false;
        Finished = false;
    }

    private void Start()
    {
        if (!isPlayerPet)
            rend.color = Color.magenta;
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

    private void UpdateSpeed()
    {

        if (speed < targetSpeed)
            speed += acceleration * Time.deltaTime;
        else if (speed > targetSpeed)
            speed -= acceleration * Time.deltaTime;
        else
            speed = targetSpeed;
    }

    private void UpdateStamina()
    {
        if (resting)
            stamina += staminaRechargeRate * Time.deltaTime;
        else
            stamina -= staminaDrainRate * Time.deltaTime;

        stamina = Mathf.Clamp(stamina, 0f, maxStamina);
        if (stamina == maxStamina)
            Rest(false);
        else if (stamina == 0f)
            Rest(true);
    }

    private void Rest(bool value)
    {
        if (value)
            targetSpeed = minSpeed;
        else
            targetSpeed = maxSpeed;

        resting = value;
    }

    public void Release()
    {
        targetSpeed = maxSpeed;
        animator.SetBool("isMoving", true);
        running = true;
    }

    public void SetStats(PetStats stats)
    {
        Name = stats.name;

        maxStamina = 100.0f;
        stamina = maxStamina;

        float drainModifier = Random.Range(-5f, 5f);
        staminaDrainRate = 25.0f - stats.stamina + drainModifier;

        float rechargeModifier = Random.Range(-5f, 5f);
        staminaRechargeRate = 20.0f + stats.stamina + rechargeModifier;
        
        maxSpeed = 4.0f;
        minSpeed = 3.0f;
        speed = 2.0f;

        float accelerationModifier = Random.Range(-0.1f, 0.1f);
        acceleration = 0.5f + 0.1f * stats.stamina + accelerationModifier;
    }

    public void SetColor(Color color)
    {
        rend.color = color;
    }
}
