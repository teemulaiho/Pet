using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    [SerializeField] private List<Apple> apples = new List<Apple>();
    [SerializeField] private Apple apple;
    private float maxHealth = 1f;
    [SerializeField] private float currentHealth = 1f;
    private float dt = 0f;
    private float dtCounter = 2f;
    private float actionDt = 0f;
    private float currentSpeed = 0f;
    private float speed = 2f;

    private Vector3 previousPos = Vector3.zero;
    private float movementDirection = 0f;

    private bool isEating;
    private bool isMoving;

    [SerializeField] Animator petAnimator;
    [SerializeField] Animator healthAnimator;

    [SerializeField] SpriteRenderer spriteRenderer;

    float eatingAnimationLength = 0f;
    float idleAnimationLength = 0f;
    float runAnimationLength = 0f;
    float jumpAnimationLength = 0f;


    private float CurrentRelativeHealth { get { return currentHealth / maxHealth; } }

    private float CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        healthAnimator.SetFloat("Health", currentHealth);
        petAnimator.SetFloat("Health", currentHealth);
        previousPos = transform.position;

        var clips = petAnimator.runtimeAnimatorController.animationClips;
        foreach (var clip in clips)
        {
            switch (clip.name)
            {
                case "Eat":
                    eatingAnimationLength = clip.length;
                    break;
                case "Idle":
                    idleAnimationLength = clip.length;
                    break;
                case "Run":
                    runAnimationLength = clip.length;
                    break;
                case "Jump":
                    jumpAnimationLength = clip.length;
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        UpdateHealth();
        GoToFood();
        UpdateAnimator();
    }

    void UpdateHealth()
    {
        dt += Time.deltaTime;

        if (dt > dtCounter)
        {
            currentHealth -= 0.1f;

            if (CurrentRelativeHealth < 0.5f)
            {
                FindNearestFood();
            }

            if (CurrentRelativeHealth <= 0f)
                Feint();

            dt = 0f;
        }
    }

    void UpdateAnimator()
    {
        healthAnimator.SetFloat("Health", currentHealth);
        petAnimator.SetFloat("Health", currentHealth);

        if (!isEating)
        {
            petAnimator.SetBool("isMoving", isMoving);

            if (movementDirection > 0)
                spriteRenderer.flipX = false;
            else if (movementDirection < 0)
                spriteRenderer.flipX = true;
        }

        if (isEating)
        {
            actionDt -= Time.deltaTime;

            if (actionDt <= 0f)
            {
                if (apple)
                {
                    currentHealth += apple.HealthGain;
                    apple.Eat();
                    apple = null;
                }
                isEating = false;
                actionDt = 0f;
            }
        }
    }

    void UpdateMovement()
    {
        if (!isEating)
        {
            float distance = Vector3.Distance(transform.position, previousPos);
            isMoving = distance > 0f;
            movementDirection = previousPos.x - transform.position.x;
            previousPos = transform.position;
        }
        else
            isMoving = false;
    }

    void FindNearestFood()
    {
        apples.Clear();
        apples.AddRange(FindObjectsOfType<Apple>());
        apple = null;

        float minDistance = float.MaxValue;
        int foodIndex = int.MaxValue;
        int index = 0;

        foreach (var food in apples)
        {
            float dist = Vector3.Distance(transform.position, food.transform.position);

            if (dist < minDistance)
            {
                minDistance = dist;
                foodIndex = index;
            }

            index++;
        }

        if (foodIndex > apples.Count)
            return;

        apple = apples[foodIndex];
    }

    void GoToFood()
    {
        if (apple && !isEating)
        {
            float dist = Vector3.Distance(transform.position, apple.transform.position);

            if (dist > 0.2f)
            {
                transform.position = Vector3.MoveTowards(transform.position, apple.transform.position, Time.deltaTime * speed);
            }
        }
    }


    void EatFood()
    {
        isMoving = false;
        petAnimator.SetBool("isMoving", isMoving);
        isEating = true;
        actionDt = eatingAnimationLength;
    }

    void Feint()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Food"))
        {
            EatFood();
            petAnimator.SetTrigger("isEating");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            EatFood();
            petAnimator.SetTrigger("isEating");
        }
    }
}
