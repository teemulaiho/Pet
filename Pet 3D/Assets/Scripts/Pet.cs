using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ObjectType
{
    None,
    Food,
    Ball
}

public class Pet : MonoBehaviour
{
    public bool canLoseHealth;

    [SerializeField] private List<Ball> balls = new List<Ball>();
    [SerializeField] private List<Apple> apples = new List<Apple>();
    [SerializeField] private Apple apple;
    [SerializeField] private Ball ball;
    [SerializeField] private Ball capturedBall;

    [SerializeField] private GameObject goal;




    private float maxHealth = 1f;
    [SerializeField] private float currentHealth = 1f;
    private float dt = 0f;
    private float dtCounter = 2f;
    private float actionDt = 0f;
    private float currentSpeed = 0f;
    private float speed = 1f;

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

        if (!capturedBall)
        {
            FindNearestBall();
            GoToBall();
        }
        else
        {
            GoToGoal();
        }



        UpdateAnimator();
    }

    void UpdateHealth()
    {
        if (!canLoseHealth)
            return;

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


    // TODO: IMPLEMENT FINDNEAREST FUNCTION.
    void FindNearest(ObjectType type)
    {
        if (type == ObjectType.Food)
        {

        }
        else if (type == ObjectType.Ball)
        {

        }
    }

    void FindNearestFood()
    {
        FindNearest(ObjectType.Food);
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


    void FindNearestBall()
    {
        FindNearest(ObjectType.Ball);
        balls.Clear();
        balls.AddRange(FindObjectsOfType<Ball>());
        ball = null;

        float minDistance = float.MaxValue;
        int ballIndex = int.MaxValue;
        int index = 0;

        foreach (var food in balls)
        {
            float dist = Vector3.Distance(transform.position, food.transform.position);

            if (dist < minDistance)
            {
                minDistance = dist;
                ballIndex = index;
            }

            index++;
        }

        if (ballIndex > balls.Count)
            return;

        ball = balls[ballIndex];
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

    void GoToBall()
    {
        if (ball && !isEating && !capturedBall)
        {
            float dist = Vector3.Distance(transform.position, ball.transform.position);

            if (dist > 0.2f)
            {
                transform.position = Vector3.MoveTowards(transform.position, ball.transform.position, Time.deltaTime * speed);
            }
        }
    }

    void GoToGoal()
    {
        if (ball && !isEating && capturedBall)
        {
            float dist = Vector3.Distance(transform.position, goal.transform.position);

            if (dist > 0.2f)
            {
                transform.position = Vector3.MoveTowards(transform.position, goal.transform.position, Time.deltaTime * speed);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            EatFood();
            petAnimator.SetTrigger("isEating");
        }
        else if (other.CompareTag("Ball"))
        {
            if (capturedBall != other.gameObject.transform.parent.GetComponent<Ball>())
                capturedBall = other.gameObject.transform.parent.GetComponent<Ball>().CaptureBall(this.transform);
        }
        else if (other.CompareTag("Goal"))
        {
            if (capturedBall)
            {
                capturedBall.ReleaseBall();
                capturedBall = null;
            }
        }
    }
}
