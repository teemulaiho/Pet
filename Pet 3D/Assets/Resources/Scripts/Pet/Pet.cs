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
    Bored
}

public class Pet : MonoBehaviour
{
    private BehaviorTree behavior; // to do
    private PetState state;

    public bool canLoseHealth;
    public bool canLoseEnergy;

    [SerializeField] private Player player;
    [SerializeField] private Food food;
    [SerializeField] private Ball ball;
    [SerializeField] private GameObject goal;

    [SerializeField] private Entity grabbedObject;

    [SerializeField] private Vector3 waypoint;
    [SerializeField] private GameObject spawnPosition;

    private float healthPreviousFrame = 0f;

    private float maxHealth = 100f;
    [SerializeField] private float health = 100f;

    private float maxEnergy = 100f;
    [SerializeField] private float energy = 100f;

    private float stayInStateDT = 0f;
    private float stayInIdleStateCounter = 20f;

    private float healthdt = 0f;
    private float healthdtCounter = 20f;

    private float energydt = 0f;
    private float energydtCounter = 2f;

    private float actionDt = 0f;
    private float speed = 1f;

    private float senseUpdateTimer = 0f;
    private float senseUpdateInterval = 1f;

    private float interactRange = 0.5f;
    private float followRange = 3f;
    private float detectionRange = 8f;
    private float callRange = 16f;
    private float wanderRange = 3f;

    private Vector3 previousPos = Vector3.zero;
    private float movementDirection = 0f;
    private float movementDirectionPreviousFrame = 0f;

    private bool isMoving;

    [SerializeField] Animator petAnimator;
    [SerializeField] Animator healthAnimator;
    [SerializeField] Animator reactionAnimator;

    [SerializeField] SpriteRenderer spriteRenderer;

    // sprite juggling testing
    public float spriteFlipThreshold;
    public float totalFlipSpriteCount;
    public float flipSpriteFalseCount;
    public float flipSpriteTrueCount;


    float eatingAnimationLength = 0f;
    float idleAnimationLength = 0f;
    float runAnimationLength = 0f;
    float jumpAnimationLength = 0f;

    Coroutine waitCoroutine;
    bool isWaiting;

    bool calledByPlayer;
    bool noticedPlayer;
    int noticedPlayerOnFrame;

    public float hoverPos = 0.3f;


    [SerializeField] Rigidbody rigidbody;
    [SerializeField] SphereCollider collider;

    [SerializeField] Transform feetPos;

    public PetState GetState() { return state; }
    public float HealthPercentage { get { return health / maxHealth; } }
    public float EnergyPercentage { get { return energy / maxEnergy; } }

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
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        collider.radius = interactRange - 0.05f;

        state = PetState.Idle;
        health = Persistent.petStats.health;
        energy = Persistent.petStats.energy;
        healthAnimator.SetFloat("Health", health);
        petAnimator.SetFloat("Health", health);
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

        if (petAnimator)
            petAnimator.gameObject.GetComponent<AnimationEvent>().onAnimationEnd += OnAnimationEnd;
    }

    // Update is called once per frame
    void Update()
    {
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

        Act();

        UpdateMovement();
        UpdateHealth();
        UpdateEnergy();
        UpdateAnimator();
    }

    void Sense()
    {
        if (!calledByPlayer)
            Player = FindNearest<Player>(detectionRange);

        var newBall = FindNearest<Ball>();
        if (newBall && !newBall._isGhost)
            ball = newBall;

        if (ball)
            ball.onKick += OnBallKickEvent;

        food = FindNearest<Food>();
        goal = GameObject.FindGameObjectWithTag("Goal");

        if (rigidbody.velocity != Vector3.zero)
            rigidbody.velocity = Vector3.zero;
    }
    void Decide()
    {
        if (state == PetState.GoToFood)
        {
            if (!food) // if the food magically disappeared
            {
                state = PetState.Idle;
            }
            else if (Vector3.Distance(transform.position, food.transform.position) <= interactRange) // if you're within range of the food
            {
                actionDt = eatingAnimationLength;
                petAnimator.SetTrigger("isEating");
                state = PetState.Eating;
            }
        }
        else if (state == PetState.Eating)
        {
            if (food == null) // if you ate the food or it magically disappeared
            {
                canLoseEnergy = true;
                state = PetState.Idle;
            }
            else if (Vector3.Distance(transform.position, food.transform.position) > interactRange) // if it somehow got out of range while eating
            {
                canLoseEnergy = true;
                state = PetState.GoToFood;
            }
        }
        else if (state == PetState.ChaseBall)
        {
            if (!ball) // if the ball magically disappeared
            {
                state = PetState.Idle;
            }
            else if (Vector3.Distance(transform.position, ball.transform.position) <= interactRange) // if you're within range of the ball
            {
                if (Persistent.petStats.intellect < 2f) // if you a dummy
                {
                    Kick(ball);

                    Persistent.AddExperience(1f);
                    Persistent.AddIntellect(0.1f);
                }
                else if (Persistent.petStats.intellect >= 2f) // if you not so dummy
                {
                    if (goal) // if there is a goal in the vicinity
                    {
                        if (Persistent.petSkills._skillDictionary["Catch"]._unlocked)
                            Pickup(ball);

                        if (!ball.hasBounced)
                        {
                            Persistent.AddExperience(5f);
                            NotificationManager.ReceiveNotification(NotificationType.Experience, 5f);
                        }
                        state = PetState.ReturnBall;
                    }
                    else
                    {
                        Kick(ball);

                        Persistent.AddExperience(1f);
                        Persistent.AddIntellect(0.1f);
                    }
                }
            }
        }
        else if (state == PetState.ReturnBall)
        {
            if (!grabbedObject) // if whatever you were carrying magically disappeared
            {
                state = PetState.Idle;
            }
            else
            {
                if (!goal) // if the goal magically disappeared
                {
                    Drop();
                    state = PetState.Idle;
                }
                else if (Vector3.Distance(transform.position, goal.transform.position) <= interactRange) // if you're within range of the goal
                {
                    Persistent.AddIntellect(0.5f);
                    Destroy(grabbedObject.gameObject);
                    grabbedObject = null;
                    state = PetState.Idle;
                }
            }
        }
        else if (state == PetState.FollowPlayer)
        {
            //reasons to transfer to another state
            if (ball) // if player throws ball
            {
                state = PetState.ChaseBall;
                SpawnSpeedCloud();
            }

            if (!Player) // if the player disappeared
            {
                state = PetState.Idle;
                noticedPlayer = false;
            }
            else if (calledByPlayer)
            {
                if (Vector3.Distance(transform.position, Player.transform.position) < detectionRange)
                    calledByPlayer = false;
            }
            else if (Vector3.Distance(transform.position, Player.transform.position) > detectionRange) // temporary reason to stop following
            {
                state = PetState.Idle;
                noticedPlayer = false;
            }
        }
        else if (state == PetState.Sleeping)
        {
            if (EnergyPercentage > 0.9f)
            {
                petAnimator.SetBool("isSleeping", false);
                reactionAnimator.SetBool("isSleeping", false);
                state = PetState.Idle;
            }
        }
        else if (state == PetState.Idle)
        {
            if (HealthPercentage < 0.5f && food) // if you're hungry and there is food around
            {
                state = PetState.GoToFood;
                canLoseEnergy = false;
            }
            else if (EnergyPercentage < 0.1f)
            {
                petAnimator.SetBool("isSleeping", true);
                reactionAnimator.SetBool("isSleeping", true);
                reactionAnimator.SetTrigger("Sleep");
                state = PetState.Sleeping;
            }
            else if (calledByPlayer)
            {
                state = PetState.FollowPlayer;
            }
            else if (ball)
            {
                state = PetState.ChaseBall;
                SpawnSpeedCloud();
            }
            else if (Player && Vector3.Distance(transform.position, Player.transform.position) < detectionRange)
            {
                state = PetState.FollowPlayer;

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
    }

    void Act()
    {
        switch (state)
        {
            case PetState.Idle:
                {
                    Wander();
                    if (!isMoving && stayInStateDT < stayInIdleStateCounter)
                    {
                        stayInStateDT += Time.deltaTime;
                        if (stayInStateDT >= stayInIdleStateCounter)
                            petAnimator.SetTrigger("Stretch");
                    }  

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

                    if (!isMoving && stayInStateDT < stayInIdleStateCounter)
                    {
                        stayInStateDT += Time.deltaTime;
                        if (stayInStateDT >= stayInIdleStateCounter)
                            petAnimator.SetTrigger("Stretch");
                    }

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
        if (goal)
            MoveTowards(goal.transform.position, interactRange);
    }
    void GoToFood()
    {
        if (food)
            MoveTowards(food.transform.position, interactRange);
    }
    void MoveTowards(Vector3 targetPosition, float range = 0f)
    {
        if (Vector3.Distance(transform.position, targetPosition) > range)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            newPos.y = hoverPos;
            transform.position = newPos;
        }


        //Vector3 direction = (targetPosition - transform.position).normalized;
        //float distance = Vector3.Distance(transform.position, targetPosition);

        //Vector3 step = direction * Time.deltaTime * speed;

        //if (distance > range)
        //{
        //    if (step.magnitude > distance)
        //    {
        //        transform.position = targetPosition;
        //    }
        //    else
        //    {
        //        transform.position += step;
        //    }
        //}
    }

    void Eat()
    {
        if (food)
        {
            actionDt -= Time.deltaTime;

            if (actionDt <= 0f)
            {
                health += food.HealthGain();
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
        rigidbody.velocity = Vector3.zero; // reset pet velocity if it collided with ball. This fixes pet wobbly movement. -Teemu
    }

    void UpdateHealth()
    {
        if (!canLoseHealth)
            return;

        healthdt += Time.deltaTime;

        if (healthdt > healthdtCounter)
        {
            health -= 10f;

            if (HealthPercentage <= 0f)
                Feint();

            healthdt = 0f;
        }

        healthPreviousFrame = health;
    }

    void UpdateEnergy()
    {
        if (!canLoseEnergy || ball || food)
            return;

        energydt += Time.deltaTime;

        if (state != PetState.Sleeping)
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

    void UpdateAnimator()
    {
        if (health != healthPreviousFrame)
            healthAnimator.SetFloat("Health", health);

        if (state != PetState.Sleeping)
            petAnimator.SetFloat("Health", health);

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
                    spriteRenderer.flipX = true;
                else
                    spriteRenderer.flipX = false;

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
        if (state != PetState.Eating)
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
                energy -= 10f;
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
        else if (other.CompareTag("Goal"))
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
            stayInStateDT = 0f;
    }
}
