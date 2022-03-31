using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PetState
{
    None,
    Idle,
    Sleep,
    FollowPlayer,
    SearchForFood,
    Feeding,
    ChaseBall
}

public enum ObjectType
{
    None,
    Food,
    Ball
}

public class Pet : MonoBehaviour
{
    public PetState petState;

    public bool canLoseHealth;
    public bool canLoseEnergy;

    [SerializeField] private Player player;
    [SerializeField] private List<Ball> balls = new List<Ball>();
    [SerializeField] private List<Apple> apples = new List<Apple>();
    [SerializeField] private Apple apple;
    [SerializeField] private Ball ball;
    [SerializeField] private Ball capturedBall;

    [SerializeField] private FeedingArea feedingArea;
    [SerializeField] private GameObject goal;
    [SerializeField] private GameObject randomTarget;
    [SerializeField] private Vector3 randomNearbyPosition;

    Transform movementTargetTransform;
    Vector3 movementTarget;

    private float healthPreviousFrame = 0f;

    private float maxHealth = 100f;
    [SerializeField] private float currentHealth = Persistent.petStats.health;

    private float maxEnergy = 100f;
    [SerializeField] private float currentEnergy = Persistent.petStats.energy;

    private float healthdt = 0f;
    private float healthdtCounter = 20f;

    private float energydt = 0f;
    private float energydtCounter = 2f;

    private float actionDt = 0f;
    private float currentSpeed = 0f;
    private float speed = 1f;

    private float waitdt = 0f;
    private float waitdtCounter = 0f;

    private Vector3 previousPos = Vector3.zero;
    private float movementDirection = 0f;

    private bool reachedRandomNearbyPosition;
    private bool reachedIdleTarget;
    private bool spottedPlayer;
    private bool isEating;
    private bool isMoving;
    private bool isSleeping;
    private bool isFeeding;

    [SerializeField] Animator petAnimator;
    [SerializeField] Animator healthAnimator;
    [SerializeField] Animator reactionAnimator;

    [SerializeField] SpriteRenderer spriteRenderer;

    float eatingAnimationLength = 0f;
    float idleAnimationLength = 0f;
    float runAnimationLength = 0f;
    float jumpAnimationLength = 0f;

    public PetState SetPetState
    {
        get { return petState; }
        set { petState = value; }
    }

    public PetState GetCurrentState()
    {
        return petState;
    }

    public float GetCurrentRelativeHealth() { return CurrentRelativeHealth; }
    public float GetCurrentRelativeEnergy() { return CurrentRelativeEnergy; }
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
            if (isEating && value == !isEating) // Finished eating
            {
                if (apple)
                {
                    currentHealth += apple.HealthGain;
                    apple.Eat();
                    apple = null;
                    if (!canLoseEnergy)
                        canLoseEnergy = true;

                    if (CurrentRelativeHealth > 0.5f)
                        SetPetState = PetState.Idle;
                }
            }

            isEating = value;
        }
    }

    public bool IsFeeding
    {
        get { return isFeeding; }
        set
        {
            isFeeding = value;

            if (isFeeding)
                SetPetState = PetState.Feeding;
        }
    }

    public bool IsSleeping
    {
        get { return isSleeping; }
        set
        {
            isSleeping = value;

            if (isSleeping)
            {
                SetPetState = PetState.Sleep;
            }

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

                if (spottedPlayer)
                {
                    SetPetState = PetState.FollowPlayer;
                }
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
        player = FindObjectOfType<Player>();
        feedingArea = FindObjectOfType<FeedingArea>();
    }

    private void Start()
    {
        Debug.Log("pet intellect: " + Persistent.petStats.intellect);

        SetPetState = PetState.Idle;
        randomNearbyPosition = transform.position;
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

        Sense();

        switch (petState)
        {
            case PetState.Idle:
            {
                Play();
                break;
            }
            case PetState.Sleep:
            {
                RestoreEnergy();
                break;
            }
            case PetState.FollowPlayer:
            {
                GoToPlayer();
                break;
            }
            case PetState.ChaseBall:
            {

                if (!capturedBall)
                {
                    FindNearestBall();
                    GoToBall();
                }
                else
                {
                    if (Persistent.petStats.intellect < 2f)
                    {
                        KickBall();
                    }
                    else if (Persistent.petStats.intellect >= 2f)
                    {
                        if (!goal)
                            goal = GameObject.FindGameObjectWithTag("Goal");

                        if (goal)
                        {
                            if (ball.IsTakingToGoal(this.transform))
                                GoToGoal();
                        }
                        else
                            KickBall();
                    }
                }
                break;
            }
            case PetState.SearchForFood:
            {
                if (!apple)
                    if (!FindNearestFood())
                        GoToFeedingArea();
                GoToFood();
                break;
            }
            case PetState.Feeding:
            {
                Feed();
                break;
            }
        }

        UpdateAnimator();
    }

    void Sense()
    {
        if (!capturedBall)
        {
            FindNearestBall();
        }
    }

    void CheckForPlayer()
    {
        if (ball || apple)
            return;

        if (!player)
        {
            player = FindObjectOfType<Player>();
            if (!player)
                return;
        }

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

    void RestoreEnergy()
    {
        energydt += Time.deltaTime;

        if (energydt > energydtCounter)
        {
            CurrentEnergy += 5f;
            energydt = 0f;
        }
    }

    void UpdateHealth()
    {
        if (!canLoseHealth)
            return;

        healthdt += Time.deltaTime;

        if (healthdt > healthdtCounter)
        {
            CurrentHealth -= 10f;

            if (CurrentRelativeHealth < 0.5f)
            {
                SetPetState = PetState.SearchForFood;
                canLoseEnergy = false;
            }

            if (CurrentRelativeHealth <= 0f)
                Feint();

            healthdt = 0f;
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
                CurrentEnergy -= 10f;

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
                    SetPetState = PetState.Idle;
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

            if (Camera.main != null)
            {
                Vector3 prePosOnCamera = Camera.main.WorldToScreenPoint(previousPos);
                Vector3 curPosOnCamera = Camera.main.WorldToScreenPoint(transform.position);

                movementDirection = prePosOnCamera.magnitude - curPosOnCamera.magnitude;
            }

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

    bool FindNearestFood()
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
            return false;

        apple = apples[foodIndex];
        MovementTargetTransform = apple.transform;

        currentEnergy = maxEnergy;
        IsSleeping = false;

        return true;
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

        SetPetState = PetState.ChaseBall;
    }

    void GoToPlayer()
    {
        if (!player)
        {
            SetPetState = PetState.Idle;
            return;
        }

        float dist = Vector3.Distance(transform.position, player.transform.position);

        if (dist > 5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * speed);
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

    void GoToFeedingArea()
    {
        if (feedingArea && !IsEating)
        {
            float dist = Vector3.Distance(transform.position, feedingArea.transform.position);

            if (dist > 0.2f)
            {
                transform.position = Vector3.MoveTowards(transform.position, feedingArea.transform.position, Time.deltaTime * speed);
            }
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

    void KickBall()
    {
        if (ball && !IsEating && capturedBall)
        {
            if (Vector3.Distance(transform.position, ball.transform.position) < 1f)
                ball.Kick(transform.forward, Persistent.petStats.strength);

            capturedBall = null;

            Persistent.petStats.intellect += 0.1f;
            Debug.Log("pet intellect: " + Persistent.petStats.intellect);
        }
    }

    void EatFood()
    {
        isMoving = false;
        petAnimator.SetBool("isMoving", isMoving);
        IsEating = true;
        actionDt = eatingAnimationLength;
    }

    void Feed()
    {
        if (IsFeeding)
        {
            CurrentHealth += feedingArea.HealthGain();

            if (!canLoseEnergy)
                canLoseEnergy = true;

            if (CurrentRelativeHealth > 0.9f)
            {
                IsFeeding = false;
                SetPetState = PetState.Idle;
            }
        }
    }

    bool Wait(float timeToWait)
    {
        waitdtCounter = timeToWait;
        if (UpdateWaitTimer() >= waitdtCounter)
        {
            ResetWaitTimer();
            return true;
        }

        return false;
    }

    float UpdateWaitTimer()
    {
        return waitdt += Time.deltaTime;
    }

    void ResetWaitTimer()
    {
        waitdt = 0f;
    }

    void Play()
    {
        if (reachedIdleTarget)
        {
            if (Wait(2f))
            {
                reachedIdleTarget = false;
                randomTarget = null;
                reachedRandomNearbyPosition = true;
            }
        }

        if (!randomTarget)
            randomTarget = FindRandomGameObjectNearby("Environment");

        // if no objects in scene, walk around
        if (!randomTarget && reachedRandomNearbyPosition)
        {
            randomNearbyPosition = FindRandomPositionNearby();
            reachedRandomNearbyPosition = false;
        }

        if (!reachedIdleTarget)
        {
            if (randomTarget)
                transform.position = Vector3.MoveTowards(transform.position, randomTarget.transform.position, Time.deltaTime * speed);
            else
                transform.position = Vector3.MoveTowards(transform.position, randomNearbyPosition, Time.deltaTime * speed);
        }

        if (randomTarget)
        {
            if (Vector3.Distance(transform.position, randomTarget.transform.position) < 2.5f)
                reachedIdleTarget = true;
        }
        else
        {
            if (Vector3.Distance(transform.position, randomNearbyPosition) < 0.5f)
                reachedIdleTarget = true;
        }
    }

    void LookAround()
    {

    }

    Vector3 FindRandomPositionNearby()
    {
        Vector3 newPos = transform.position;

        newPos.x += Random.Range(-3f, 3f);
        newPos.z += Random.Range(-3f, 3f);

        return newPos;
    }

    GameObject FindRandomGameObjectNearby(string gameObjectTag)
    {
        List<GameObject> nearbyObjects = new List<GameObject>();
        nearbyObjects.AddRange(GameObject.FindGameObjectsWithTag(gameObjectTag));

        if (nearbyObjects.Count == 0)
            return null;

        nearbyObjects.Sort(SortByDistanceToPet);

        int randomIndex = Random.Range(0, nearbyObjects.Count);
        return nearbyObjects[randomIndex];
    }

    int SortByDistanceToPet(GameObject a, GameObject b)
    {
        return DistanceToPet(a).CompareTo(DistanceToPet(b));
    }

    float DistanceToPet(GameObject comparison)
    {
        return Vector3.Distance(transform.position, comparison.transform.position);
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
        else if (other.CompareTag("FeedingArea"))
        {
            IsFeeding = true;
            EatFood();
            petAnimator.SetTrigger("isEating");
        }
        else if (other.CompareTag("Ball"))
        {
            if (capturedBall != other.gameObject.transform.parent.GetComponent<Ball>())
            {
                capturedBall = other.gameObject.transform.parent.GetComponent<Ball>().CaptureBall(this.transform, Persistent.petStats.intellect >= 2f);
            }
        }
        else if (other.CompareTag("Goal"))
        {
            if (capturedBall)
            {
                capturedBall.ReleaseBall();
                capturedBall = null;
                SetPetState = PetState.Idle;
            }
        }
    }

    public void PetPet()
    {
        reactionAnimator.SetTrigger("PetPet");
    }

}
