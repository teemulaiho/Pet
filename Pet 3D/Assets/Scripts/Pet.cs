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
    public bool canLoseEnergy;

    [SerializeField] private PlayerController player;
    [SerializeField] private List<Ball> balls = new List<Ball>();
    [SerializeField] private List<Apple> apples = new List<Apple>();
    [SerializeField] private Apple apple;
    [SerializeField] private Ball ball;
    [SerializeField] private Ball capturedBall;

    [SerializeField] private GameObject goal;

    Transform movementTargetTransform;
    Vector3 movementTarget;


    private float healthPreviousFrame = 0f;


    private float maxHealth = 1f;
    [SerializeField] private float currentHealth = 1f;

    private float maxEnergy = 5f;
    [SerializeField] private float currentEnergy = 5;

    private float dt = 0f;
    private float dtCounter = 2f;

    private float energydt = 0f;
    private float energydtCounter = 2f;


    private float actionDt = 0f;
    private float currentSpeed = 0f;
    private float speed = 1f;

    private Vector3 previousPos = Vector3.zero;
    private float movementDirection = 0f;

    private bool spottedPlayer;
    private bool isEating;
    private bool isMoving;
    private bool isSleeping;

    [SerializeField] Animator petAnimator;
    [SerializeField] Animator healthAnimator;
    [SerializeField] Animator reactionAnimator;

    [SerializeField] SpriteRenderer spriteRenderer;

    float eatingAnimationLength = 0f;
    float idleAnimationLength = 0f;
    float runAnimationLength = 0f;
    float jumpAnimationLength = 0f;

    private float CurrentRelativeHealth { get { return currentHealth / maxHealth; } }

    private float CurrentRelativeEnergy { get { return currentEnergy / maxEnergy; } }

    private float CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    private float CurrentEnergy
    {
        get { return currentEnergy; }
        set { currentEnergy = value; }
    }

    public bool IsEating
    {
        get { return isEating; }
        set
        {
            isEating = value;
        }
    }

    public bool IsSleeping
    {
        get { return isSleeping; }
        set
        {
            isSleeping = value;
            petAnimator.SetBool("isSleeping", isSleeping);

            if (IsSleeping && !SpottedPlayer)
            {
                reactionAnimator.SetTrigger("Sleep");
            }

            reactionAnimator.SetBool("isSleeping", IsSleeping);
        }
    }

    public bool SpottedPlayer
    {
        get { return spottedPlayer; }
        set
        {
            spottedPlayer = value;

            if (spottedPlayer)
            {
                if (IsSleeping)
                {
                    IsSleeping = false;
                    reactionAnimator.SetTrigger("Notice");
                }

                CurrentEnergy = maxEnergy;
            }
        }
    }

    public Transform MovementTargetTransform
    {
        get { return movementTargetTransform; }
        set { movementTargetTransform = value; }
    }

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
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
        CheckForPlayer();
        UpdateMovement();
        UpdateHealth();
        UpdateEnergy();
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

    void CheckForPlayer()
    {
        if (ball || apple)
            return;

        float dist = float.MaxValue;

        if (player)
            dist = Vector3.Distance(transform.position, player.transform.position);

        if (dist < 5f && !SpottedPlayer)
        {
            SpottedPlayer = true;
        }
        else if (dist > 5f && SpottedPlayer)
        {
            SpottedPlayer = false;
        }
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

        healthPreviousFrame = CurrentHealth;
    }

    void UpdateEnergy()
    {
        if (!canLoseEnergy || ball || apple)
            return;

        energydt += Time.deltaTime;

        if (!IsSleeping)
        {
            if (energydt > energydtCounter)
            {
                currentEnergy -= 0.5f;

                if (CurrentRelativeEnergy < 0.1f)
                {
                    IsSleeping = true;
                    Feint();
                }

                energydt = 0f;
            }
        }
        else
        {
            if (energydt > energydtCounter)
            {
                currentEnergy += 0.5f;

                if (CurrentRelativeEnergy > 0.9f)
                {
                    IsSleeping = false;
                }

                energydt = 0f;
            }
        }
    }

    void UpdateAnimator()
    {
        if (CurrentHealth != healthPreviousFrame)
            healthAnimator.SetFloat("Health", currentHealth);

        if (!IsSleeping)
            petAnimator.SetFloat("Health", currentHealth);

        if (!IsEating)
        {
            petAnimator.SetBool("isMoving", isMoving);

            if (movementDirection > 0)
                spriteRenderer.flipX = false;
            else if (movementDirection < 0)
                spriteRenderer.flipX = true;
        }

        if (IsEating)
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

                IsEating = false;
                actionDt = 0f;
            }
        }


    }

    void UpdateMovement()
    {
        if (!IsEating)
        {
            float distance = Vector3.Distance(transform.position, previousPos);
            isMoving = distance > 0f;

            Vector3 prePosOnCamera = Camera.main.WorldToScreenPoint(previousPos);
            Vector3 curPosOnCamera = Camera.main.WorldToScreenPoint(transform.position);

            movementDirection = prePosOnCamera.magnitude - curPosOnCamera.magnitude;

            //float newDir = (prePosOnCamera.x - curPosOnCamera.x) ;
            //movementDirection = newDir > 0.025f ? newDir : movementDirection;

            //if (movementTargetTransform)
            //    transform.LookAt(movementTargetTransform);

            // Original
            //movementDirection = previousPos.x - transform.position.x;

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
        MovementTargetTransform = apple.transform;

        currentEnergy = maxEnergy;
        IsSleeping = false;
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
        MovementTargetTransform = ball.transform;
    }

    void GoToFood()
    {
        if (apple && !IsEating)
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
        if (ball && !IsEating && !capturedBall)
        {
            float dist = Vector3.Distance(transform.position, ball.transform.position);

            if (dist > 0.2f)
            {
                transform.position = Vector3.MoveTowards(transform.position, ball.transform.position, Time.deltaTime * speed);
            }

            if (IsSleeping)
                IsSleeping = false;
        }
    }

    void GoToGoal()
    {
        if (ball && !IsEating && capturedBall)
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
        IsEating = true;
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

    public void PetPet()
    {
        reactionAnimator.SetTrigger("PetPet");
    }
}
