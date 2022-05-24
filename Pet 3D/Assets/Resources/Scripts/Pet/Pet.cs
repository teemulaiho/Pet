using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PetState
{
    None,
    Idle,
    Sleeping,
    FollowPlayer,
    GoToFood,
    Eating,
    ChaseBall,
    ReturnBall,
    Fainted,
    Dazed,
    Bored,
    ReturnToNest,
    TetherBall
}

public class Pet : MonoBehaviour
{
    private BehaviorTree behavior; // to do

    [Header("State")]

    public PetState state;
    public PetState State
    {
        get { return state; }
        set
        {
            if (State != value)
            {
                if (onStateChange != null)
                    onStateChange(state, value);
                state = value;
            }
        }
    }

    private Dictionary<PetState, float> stateCounterDictionary = new Dictionary<PetState, float>();

    public float stayInStateDT = 0f;

    private float stayInIdleStateCounter = 20f;
    private float stayInFollowStateCounter = 10;
    private float stayInBoredStateCounter = 25f;

    public delegate void StateChange(PetState from, PetState to);
    public event StateChange onStateChange;

    private int ballActionsMax = 6;
    private int ballActionsCurrent = 6;
    private float ballActionRechargeDuration = 30.0f;
    private float ballActionRechargeTimer = 0f;

    [Space]
    [Header("Stats")]
    public bool canLoseHunger;
    public bool canLoseEnergy;

    private float maxHealth = 100f;
    [SerializeField] private float health = 100f;

    private float maxHunger = 100f;
    [SerializeField] private float hunger = 100f;

    private float maxEnergy = 100f;
    [SerializeField] private float energy = 100f;

    private float maxHappiness = 100f;
    [SerializeField] private float happiness = 100f;

    [Space]
    [Header("Objects To Interact With")]

    [SerializeField] private Player player;
    [SerializeField] private Food food;
    [SerializeField] private Ball ball;
    [SerializeField] private Nest nest;
    [SerializeField] private TetherBall tetherBall;

    [SerializeField] private Entity grabbedObject;

    [SerializeField] private Vector3 waypoint;
    [SerializeField] private GameObject spawnPosition;

    private float hungerPreviousFrame = 0f;

    [Space]
    [Header("Behaviour Values")]

    private bool isWaitingForPlayerAction;

    public bool isMoving;
    public bool isPlayerMoving;

    public float timeSinceLastEnjoyableAction = 0f;
    private int happinessTimer = 15;

    private float healthLossRate = 0.75f;
    private float healthGainRate = 1.0f;

    private float hungerDt = 0f;
    private float hungerDtCounter = 20f;

    private float energydt = 0f;
    private float energydtCounter = 5f;

    private float actionDt = 0f;
    private float speed = 1.5f;

    private float senseUpdateTimer = 0f;
    private float senseUpdateInterval = 1f;

    private bool jumpCoolDownActive = false;
    private float jumpdt = 0f;
    private float jumpCoolDown = 3f;

    private float interactRange = 0.5f;
    private float followRange = 3f;
    private float detectionRange = 8f;
    private float callRange = 16f;
    private float wanderRange = 3f;

    private Vector3 previousPos = Vector3.zero;
    private float movementDirection = 0f;
    private float movementDirectionPreviousFrame = 0f;
    private Vector3 directionScale;

    private Vector3 previousPlayerPos = Vector3.zero;

    private bool grounded = false;
    private RaycastHit groundInfo;



    [Space]
    [Header("Animations")]

    [SerializeField] Animator petAnimator;
    [SerializeField] Animator hungerAnimator;
    [SerializeField] Animator reactionAnimator;

    [SerializeField] SpriteRenderer spriteRenderer;

    // sprite juggling testing
    public float spriteFlipThreshold;
    public float totalFlipSpriteCount;
    public float flipSpriteFalseCount;
    public float flipSpriteTrueCount;

    public bool isDoingBoredAction;


    float eatingAnimationLength = 0f;
    float idleAnimationLength = 0f;
    float runAnimationLength = 0f;
    float jumpAnimationLength = 0f;

    Coroutine waitCoroutine;
    bool isWaiting;

    bool calledByPlayer;
    bool noticedPlayer;
    int noticedPlayerOnFrame;

    [Space]
    [Header("Other")]

    public float hoverPos = 0.3f;


    [SerializeField] Rigidbody body;
    [SerializeField] SphereCollider coll;

    [SerializeField] Transform feetPos;

    //public PetState GetState() { return state; }

    public float HealthPercentage { get { return health / maxHealth; } }
    public float HungerPercentage { get { return hunger / maxHunger; } }
    public float EnergyPercentage { get { return energy / maxEnergy; } }
    public float HappinessPercentage { get { return happiness / maxHappiness; } }

    Player Player
    {
        get { return player; }
        set
        {
            player = value;

            if (player)
                player.onPetCall += PlayerCallPet;
        }
    }

    private void Awake()
    {
        Player = FindObjectOfType<Player>();
        spawnPosition = GameObject.FindGameObjectWithTag("SpawnPosition");
        body = GetComponent<Rigidbody>();
        nest = FindObjectOfType<Nest>();
        tetherBall = FindObjectOfType<TetherBall>();
    }

    private void Start()
    {
        coll.radius = interactRange - 0.05f;

        State = PetState.Idle;
        health = Persistent.petStats.health;
        hunger = Persistent.petStats.hunger;
        energy = Persistent.petStats.energy;
        hungerAnimator.SetFloat("Health", hunger);
        petAnimator.SetFloat("Health", hunger);
        previousPos = transform.position;

        if (player)
            previousPlayerPos = player.transform.position;

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

        if (petAnimator)
            petAnimator.gameObject.GetComponent<AnimationEvent>().onAnimationEnd += OnAnimationEnd;

        directionScale = transform.localScale;

        foreach (PetState state in System.Enum.GetValues(typeof(PetState)))
        {
            if (state == PetState.FollowPlayer)
                stateCounterDictionary.Add(state, stayInFollowStateCounter);
            else if (state == PetState.Idle)
                stateCounterDictionary.Add(state, stayInIdleStateCounter);
            else if (state == PetState.Bored)
                stateCounterDictionary.Add(state, stayInBoredStateCounter);
            else
                stateCounterDictionary.Add(state, -1);
        }

        onStateChange += OnStateChange;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            petAnimator.SetTrigger("Stretch");

        if (senseUpdateTimer < senseUpdateInterval) // sensing and deciding done slower than acting
        {
            senseUpdateTimer += Time.deltaTime;
        }
        else
        {
            Sense();
            Decide();

            senseUpdateTimer = 0f;
        }

        UpdateMovement();
        UpdateHealth();
        UpdateHunger();
        UpdateEnergy();
        UpdateCooldowns();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        Act();
    }

    void Sense()
    {
        if (!calledByPlayer)
            Player = FindNearest<Player>(detectionRange);

        var newBall = FindNearest<Ball>();
        if (!ball && newBall && !newBall._isGhost && !isWaitingForPlayerAction)
            ball = newBall;

        if (ball)
            ball.onKick += OnBallKickEvent;

        food = FindNearest<Food>();

        if (!tetherBall)
            tetherBall = FindObjectOfType<TetherBall>();

        if (player)
        {
            isPlayerMoving = previousPlayerPos != player.transform.position;
            previousPlayerPos = player.transform.position;

            if (isPlayerMoving)
                isWaitingForPlayerAction = false;
        }

        GroundCheck();
    }
    void Decide()
    {
        if (State == PetState.GoToFood)
        {
            if (!food) // if the food magically disappeared
            {
                State = PetState.Idle;
            }
            else if (Vector3.Distance(transform.position, food.transform.position) <= interactRange) // if you're within range of the food
            {
                actionDt = eatingAnimationLength;
                petAnimator.SetTrigger("isEating");
                State = PetState.Eating;
            }
        }
        else if (State == PetState.Eating)
        {
            if (food == null) // if you ate the food or it magically disappeared
            {
                canLoseEnergy = true;
                State = PetState.Idle;
            }
            else if (Vector3.Distance(transform.position, food.transform.position) > interactRange) // if it somehow got out of range while eating
            {
                canLoseEnergy = true;
                State = PetState.GoToFood;
            }
        }
        else if (State == PetState.ChaseBall)
        {
            if (!ball) // if the ball magically disappeared
            {
                State = PetState.Idle;
            }
            else if (Vector3.Distance(transform.position, ball.transform.position) <= interactRange) // if you're within range of the ball
            {
                int random = Random.Range(0, 50);
                if (random < Persistent.petStats.intellect * 10)
                {
                    Pickup(ball);
                    ballActionsCurrent -= 1;

                    if (!ball.hasBounced)
                    {
                        NotificationManager.ReceiveNotification(NotificationType.Experience, 5f);
                    }
                    State = PetState.ReturnBall;
                }
                else
                {
                    Kick(ball);
                    ballActionsCurrent -= 1;

                    if (ballActionsCurrent == 0)
                        State = PetState.Idle;

                    Persistent.AddIntellect(0.1f);
                }
            }
        }
        else if (State == PetState.ReturnBall)
        {
            if (!grabbedObject) // if whatever you were carrying magically disappeared
            {
                State = PetState.Idle;
            }
            else
            {
                if (player &&
                    Vector3.Distance(transform.position, player.transform.position) <= followRange) // if you're within range of the goal
                {
                    Drop();
                    isWaitingForPlayerAction = true;
                    Persistent.AddIntellect(0.5f);
                    //Destroy(grabbedObject.gameObject);
                    //grabbedObject = null;
                    State = PetState.Idle;
                }
            }
        }
        else if (State == PetState.FollowPlayer)
        {
            //reasons to transfer to another state
            if (ball && ballActionsCurrent > 0) // if player throws ball
            {
                State = PetState.ChaseBall;
                SpawnSpeedCloud();
            }

            if (!Player) // if the player disappeared
            {
                State = PetState.Idle;
                noticedPlayer = false;
            }
            else if (calledByPlayer)
            {
                if (Vector3.Distance(transform.position, Player.transform.position) < detectionRange)
                    calledByPlayer = false;
            }
            else if (Vector3.Distance(transform.position, Player.transform.position) > detectionRange) // temporary reason to stop following
            {
                State = PetState.Idle;
                noticedPlayer = false;
            }

            if (stayInStateDT >= stayInFollowStateCounter) // If Player hasn't done an action for a while, get bored
                State = PetState.Bored;
        }
        else if (State == PetState.Sleeping)
        {
            if (EnergyPercentage > 0.9f)
            {
                petAnimator.SetBool("isSleeping", false);
                reactionAnimator.SetBool("isSleeping", false);
                State = PetState.Idle;
            }
        }
        else if (State == PetState.Idle)
        {
            if (HungerPercentage < 0.5f && food) // if you're hungry and there is food around
            {
                State = PetState.GoToFood;
                canLoseEnergy = false;
            }
            else if (energy / maxEnergy <= 0.2f)
            {
                State = PetState.ReturnToNest;
            }
            else if (calledByPlayer)
            {
                State = PetState.FollowPlayer;
            }
            else if (ball && ballActionsCurrent > 0)
            {
                State = PetState.ChaseBall;
                SpawnSpeedCloud();
            }
            else if (tetherBall && tetherBall.gameObject.activeSelf)
            {
                State = PetState.TetherBall;
            }
            else if (Player && isPlayerMoving && Vector3.Distance(transform.position, Player.transform.position) < detectionRange)
            {
                State = PetState.FollowPlayer;

                noticedPlayer = true;
                noticedPlayerOnFrame = Time.frameCount;
            }
            else
            {
                float distance = Vector3.Distance(transform.position, waypoint);

                if (!isWaiting)
                {
                    if (distance > wanderRange)
                    {
                        waypoint = GetRandomPositionAround(transform.position, wanderRange);
                    }
                    else if (distance <= interactRange)
                    {
                        waypoint = GetRandomPositionAround(transform.position, wanderRange);
                    }

                    if (waitCoroutine != null)
                    {
                        StopCoroutine(waitCoroutine);
                        isWaiting = false;
                    }
                    waitCoroutine = StartCoroutine(Wait(5.0f));
                }
            }
        }
        else if (State == PetState.Dazed)
        {
        }
        else if (State == PetState.Bored)
        {
            //reasons to transfer to another state
            if (energy / maxEnergy <= 0.2f)
            {
                State = PetState.ReturnToNest;
            }
            else if (ball && ballActionsCurrent > 0) // if player throws ball
            {
                State = PetState.ChaseBall;
                SpawnSpeedCloud();
            }
            else if (isPlayerMoving)
            {
                State = PetState.FollowPlayer;
            }
            else if (stayInStateDT >= stateCounterDictionary[State] &&
                (int)stayInStateDT % (int)stateCounterDictionary[State] == 0)
            {
                int val = Random.Range(0, 2);

                if (val == 0)
                    State = PetState.Idle;
            }
        }
        else if (State == PetState.ReturnToNest)
        {
            if (nest)
            {
                if (WithinLimits(transform.position, nest.transform.position, interactRange))
                {
                    petAnimator.SetBool("isSleeping", true);
                    reactionAnimator.SetBool("isSleeping", true);
                    reactionAnimator.SetTrigger("Sleep");
                    State = PetState.Sleeping;
                }
            }
        }
    }

    void Act()
    {
        timeSinceLastEnjoyableAction += Time.deltaTime;
        if (timeSinceLastEnjoyableAction >= happinessTimer &&
            (int)timeSinceLastEnjoyableAction % happinessTimer == 0)
        {
            if (!IsEnjoyableState(state))
                happiness -= 10f;
            timeSinceLastEnjoyableAction = 0f;
        }


        switch (State)
        {
            case PetState.Idle:
                {
                    Wander();
                    break;
                }
            case PetState.Sleeping:
                {
                    Sleep();
                    break;
                }
            case PetState.FollowPlayer:
                {
                    FollowPlayer();
                    break;
                }
            case PetState.ChaseBall:
                {
                    ChaseBall();
                    break;
                }
            case PetState.ReturnBall:
                {
                    ReturnBall();
                    break;
                }
            case PetState.GoToFood:
                {
                    GoToFood();
                    break;
                }
            case PetState.Eating:
                {
                    Eat();
                    break;
                }
            case PetState.Bored:
                {
                    break;
                }
            case PetState.ReturnToNest:
                {
                    GoToNest();
                    break;
                }
            case PetState.TetherBall:
                {
                    if (GoToTetherBall())
                    {
                        if (!jumpCoolDownActive)
                        {
                            JumpTowards();
                            jumpCoolDownActive = true;
                        }
                        else
                        {
                            jumpdt += Time.deltaTime;
                            if (jumpdt >= jumpCoolDown)
                            {
                                jumpdt = 0f;
                                jumpCoolDownActive = false;
                            }
                        }
                    }

                    break;
                }
        }
    }

    void Sleep()
    {
        energydt += Time.deltaTime;

        if (energydt > energydtCounter)
        {
            energy += 5f;
            energydt = 0f;
        }
    }

    bool GoToNest()
    {
        if (!nest)
            return false;

        MoveTowards(nest.transform.position, interactRange);
        return Vector3.Distance(transform.position, nest.transform.position) <= interactRange;
    }

    bool GoToTetherBall()
    {
        MoveTowards(tetherBall.ballrb.transform.position, interactRange);
        return Vector3.Distance(transform.position, tetherBall.ballrb.transform.position) <= interactRange;
    }

    bool JumpTowards()
    {
        petAnimator.SetTrigger("Jump");
        //tetherBall.Nudge(transform.forward, 20f);

        return true;
    }

    void Wander()
    {
        MoveTowards(waypoint);
    }
    void FollowPlayer()
    {
        if (Player)
            MoveTowards(Player.transform.position, followRange);
    }
    void ChaseBall()
    {
        if (ball)
            MoveTowards(ball.transform.position, interactRange);
    }
    void ReturnBall()
    {
        if (player)
            MoveTowards(player.transform.position, followRange);
    }
    void GoToFood()
    {
        if (food)
            MoveTowards(food.transform.position, interactRange);
    }

    void MoveTowards(Vector3 targetPosition, float range = 0f)
    {
        if (grounded)
        {
            if (Vector3.Distance(transform.position, targetPosition) > range)
            {
                Vector3 direction = targetPosition - transform.position;
                direction.y = 0;
                if (direction.magnitude > 1.0f)
                    direction.Normalize();

                direction = Vector3.Cross(Vector3.Cross(-direction, Vector3.up), groundInfo.normal);

                Vector3 targetVelocity = direction * speed;
                Vector3 velocityChange = targetVelocity - body.velocity;
                body.AddForce(velocityChange, ForceMode.VelocityChange);
            }
            else
            {
                body.velocity = Vector3.zero;
            }
        }

        //if (Vector3.Distance(transform.position, targetPosition) > range)
        //{
        //    Vector3 newPos = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        //    newPos.y = hoverPos;
        //    transform.position = newPos;
        //}
    }

    private void GroundCheck()
    {
        grounded = Physics.SphereCast(coll.bounds.center, 0.15f, -transform.up, out groundInfo, 0.1f);
    }

    void Eat()
    {
        if (food)
        {
            actionDt -= Time.deltaTime;

            if (actionDt <= 0f)
            {
                hunger += food.HealthGain();
                Destroy(food.gameObject);
                food = null;
            }
        }
    }
    void Pickup(Entity entity)
    {
        entity.transform.parent = transform;
        entity.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        entity.rb.isKinematic = true;
        //entity.rb.detectCollisions = false;

        grabbedObject = entity;
    }
    void Drop()
    {
        if (grabbedObject != null)
        {
            grabbedObject.transform.parent = null;
            grabbedObject.rb.isKinematic = false;
            grabbedObject.rb.detectCollisions = true;

            if (grabbedObject.gameObject == ball.gameObject)
                ball = null;

            grabbedObject = null;
        }
    }
    void Kick(Entity entity)
    {
        Vector3 directionVariance = Vector3.zero;

        directionVariance.x = Random.Range(-2f, 2f);
        directionVariance.y = Random.Range(0.5f, 2f);
        directionVariance.z = Random.Range(-2f, 2f);

        entity.rb.AddForce((transform.forward + directionVariance).normalized * Persistent.petStats.strength * 200f);
        body.velocity = Vector3.zero; // reset pet velocity if it collided with ball. This fixes pet wobbly movement. -Teemu
    }

    void UpdateHealth()
    {
        if (HungerPercentage <= 0.2f)
        {
            health -= Time.deltaTime * healthLossRate;
        }
        else if (HungerPercentage > 0.5f && EnergyPercentage > 0.5f)
        {
            health += Time.deltaTime * healthGainRate;
        }
        else
        {
            return;
        }

        health = Mathf.Clamp(health, 0f, maxHealth);
    }

    void UpdateHunger()
    {
        if (!canLoseHunger)
            return;

        hungerDt += Time.deltaTime;

        if (hungerDt > hungerDtCounter)
        {
            hunger -= 10f;

            if (HungerPercentage <= 0f)
                Feint();

            hungerDt = 0f;
        }

        hungerPreviousFrame = hunger;
    }

    void UpdateEnergy()
    {
        if (!canLoseEnergy || ball || food)
            return;

        energydt += Time.deltaTime;

        if (State != PetState.Sleeping)
        {
            if (energydt > energydtCounter)
            {
                energy -= 10f;

                energydt = 0f;
            }
        }
        else
        {
            if (energydt > energydtCounter)
            {
                energy += 0.5f;

                energydt = 0f;
            }
        }
    }

    void UpdateCooldowns()
    {
        if (ballActionsCurrent < ballActionsMax)
        {
            if (ballActionRechargeTimer < ballActionRechargeDuration)
            {
                ballActionRechargeTimer += Time.deltaTime;
            }
            else
            {
                ballActionsCurrent += 1;
                ballActionRechargeTimer = 0f;
            }
        }
    }

    private void CheckStateStay()
    {
        if (stayInStateDT >= stateCounterDictionary[State] &&
            (int)stayInStateDT % (int)stateCounterDictionary[State] == 0)
        {
            if (State == PetState.Bored)
            {
                if (!isDoingBoredAction)
                {
                    if (petAnimator.GetBool("Break")) // If Break -trigger is still active from previous actions (ie. moving)
                        petAnimator.SetBool("Break", false);

                    petAnimator.SetTrigger("Stretch");
                    isDoingBoredAction = true;
                }
            }
        }
    }

    void UpdateAnimator()
    {
        CheckStateStay();

        if (State == PetState.Sleeping)
        {
            if (WithinLimits(transform.position, nest.transform.position, interactRange))
            {
                petAnimator.SetBool("isSleeping", true);
                reactionAnimator.SetBool("isSleeping", true);
            }
        }

        if (hunger != hungerPreviousFrame)
            hungerAnimator.SetFloat("Health", hunger);

        if (State != PetState.Sleeping)
            petAnimator.SetFloat("Health", hunger);

        if (isMoving)
            petAnimator.SetTrigger("Break");

        petAnimator.SetBool("isMoving", isMoving);

        if (!WithinLimits(movementDirection, movementDirectionPreviousFrame, spriteFlipThreshold))
        {
            if (movementDirection > 0)
            {
                flipSpriteFalseCount++;
                //spriteRenderer.flipX = false;

            }
            else if (movementDirection < 0)
            {
                flipSpriteTrueCount++;
                //spriteRenderer.flipX = true;
            }

            totalFlipSpriteCount = flipSpriteFalseCount + flipSpriteTrueCount;

            if (totalFlipSpriteCount > 10f)
            {
                if (flipSpriteTrueCount / totalFlipSpriteCount > 0.5f)
                {
                    spriteRenderer.flipX = true;
                    if (directionScale.x > 0)
                    {
                        directionScale.x *= -1f;
                        transform.localScale = directionScale;
                    }
                }
                else
                {
                    spriteRenderer.flipX = false;

                    if (directionScale.x < 0)
                    {
                        directionScale.x *= -1f;
                        transform.localScale = directionScale;
                    }
                }

                totalFlipSpriteCount = 0f;
                flipSpriteFalseCount = 0f;
                flipSpriteTrueCount = 0f;
            }
        }

        if (reactionAnimator)
        {
            if (noticedPlayer && noticedPlayerOnFrame == Time.frameCount)
                reactionAnimator.SetTrigger("Notice");
        }
    }

    void UpdateMovement()
    {
        if (isMoving)
            stayInStateDT = 0f;
        else
            stayInStateDT += Time.deltaTime;

        if (State != PetState.Eating)
        {
            float distance = Vector3.Distance(transform.position, previousPos);
            isMoving = distance > 0f;

            if (Camera.main != null)
            {
                Vector3 prePosOnCamera = Camera.main.WorldToScreenPoint(previousPos);
                Vector3 curPosOnCamera = Camera.main.WorldToScreenPoint(transform.position);

                movementDirectionPreviousFrame = movementDirection;
                //movementDirection = prePosOnCamera.magnitude - curPosOnCamera.magnitude;
                //movementDirection = (prePosOnCamera.x - curPosOnCamera.x) > 0 ? 1 : -1;
                movementDirection = (prePosOnCamera.x - curPosOnCamera.x);
            }

            previousPos = transform.position;
        }
        else
            isMoving = false;
    }

    bool WithinLimits(float movementDirection, float movementDirectionPreviousFrame, float threshold)
    {
        float difference = Mathf.Abs(movementDirectionPreviousFrame - movementDirection);

        if (difference < threshold) // is within threshold value.
            return true;
        else // is not within threshold value.
            return false;
    }

    bool WithinLimits(Vector3 pos, Vector3 comparison, float range)
    {
        return Vector3.Distance(pos, comparison) <= range;
    }

    T FindNearest<T>(float range = 0f) where T : Component
    {
        T nearest = null;
        float closestDistance = float.MaxValue;

        foreach (T entity in FindObjectsOfType<T>())
        {
            float distance = Vector3.Distance(transform.position, entity.transform.position);

            if (range > 0f)
                if (distance > range)
                    continue;

            if (nearest != null)
            {
                if (distance < closestDistance)
                {
                    nearest = entity;
                    closestDistance = distance;
                }
            }
            else
            {
                nearest = entity;
                closestDistance = distance;
            }
        }

        return nearest;
    }

    Vector3 GetRandomPositionAround(Vector3 position, float range)
    {
        float direction = Random.Range(0f, 359f) * Mathf.Deg2Rad;
        float distance = Random.Range(1f, range);

        Vector3 newPos = position + new Vector3(distance * Mathf.Cos(direction), 0, distance * Mathf.Sin(direction));
        newPos.y = hoverPos;

        return newPos;
    }

    void Feint()
    {

    }


    public void PetPet()
    {
        reactionAnimator.SetTrigger("PetPet");
    }

    void Respawn()
    {
        if (spawnPosition)
            transform.position = spawnPosition.transform.position;
        else
            transform.position = new Vector3(5, 6, 5);
    }

    IEnumerator Wait(float timeToWaitInSeconds)
    {
        isWaiting = true;
        yield return new WaitForSecondsRealtime(timeToWaitInSeconds);
        isWaiting = false;
    }

    void OnBallKickEvent(Transform kicker)
    {
        if (kicker != this.transform)
        {
            grabbedObject = null;
        }
    }

    void SpawnSpeedCloud()
    {
        SpeedCloud newCloud = Instantiate(Resources.Load<SpeedCloud>("Prefabs/Effects/SpeedCloud"), feetPos.transform.position, transform.rotation, null);
        newCloud.Initialize();
    }

    void PlayerCallPet()
    {
        if (!Persistent.CheckIfSkillUnlocked("Whistle"))
            return;

        if (!Player)
            Player = FindObjectOfType<Player>();

        float dist = Vector3.Distance(transform.position, Player.transform.position);
        if (dist < callRange)
            calledByPlayer = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Ball ball = collision.gameObject.GetComponent<Ball>();
            if (ball._isThrown)
            {
                State = PetState.Dazed;
                //petAnimator.SetBool("isDazed", true);
                petAnimator.SetTrigger("Dazed");
                reactionAnimator.SetTrigger("Dazed");
                energy -= 10f;

                body.velocity = Vector3.zero;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {

        }
        else if (other.CompareTag("Ball"))
        {

        }
        else if (other.CompareTag("DropCatcher"))
        {
            Debug.Log("Pet OnTriggerEnter With DropCatcher.");
            Respawn();
        }
    }

    void OnAnimationEnd(string animationThatEnded)
    {
        if (animationThatEnded.Contains("Stretch"))
        {
            petAnimator.SetTrigger("Break");
            isDoingBoredAction = false;
        }
        else if (animationThatEnded.Contains("Dazed"))
        {
            reactionAnimator.SetTrigger("Dazed");
            State = PetState.Idle;
        }
        else if (animationThatEnded.Contains("Jump"))
        {
            tetherBall.Nudge(transform.forward, 20f);
        }
    }

    void OnStateChange(PetState from, PetState to)
    {
        if (IsPlayerInteractionState(to))
            timeSinceLastEnjoyableAction = 0f;

        if (from == PetState.Bored)
        {
            isDoingBoredAction = false;
        }

        Debug.Log("Pet State Change from: " + from + " to: " + to);
        stayInStateDT = 0f;
    }

    bool IsPlayerInteractionState(PetState stateToCheck)
    {
        if (stateToCheck == PetState.ChaseBall ||
            stateToCheck == PetState.FollowPlayer ||
            stateToCheck == PetState.ReturnBall)
            return true;


        return false;
    }

    bool IsEnjoyableState(PetState currentState)
    {
        if (currentState == PetState.ChaseBall ||
            currentState == PetState.ReturnBall)
            return true;

        return false;

    }
}
