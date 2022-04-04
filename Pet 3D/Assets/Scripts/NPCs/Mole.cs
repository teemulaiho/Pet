using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoleState
{
    Idle,
    Moving
}

[System.Serializable]
public class Mole : NPC
{
    [SerializeField] MoleState moleState;

    Animator[] animators;
    Animator moleAnimator;
    Animator reactionAnimator;

    private float senseUpdateTimer = 0f;
    private float senseUpdateInterval = 3f;

    float distanceToPlayer = 0f;
    float visionRange = 5f;
    float interactRange = 3f;
    float wanderRange = 3f;
    Player player;
    public DialogueTrigger dialogueTrigger;
    bool dialogueInitiated;
    bool noticedPlayer;
    int noticedPlayerThisFrame;

    [SerializeField] private Vector3 waypoint;
    private Vector3 previousPos = Vector3.zero;
    bool isMoving;

    float speed = 1.0f;

    [SerializeField] Collider interactCollider;
    [SerializeField] Collider visionCollider;

    private void Start()
    {
        player = FindObjectOfType<Player>();

        animators = GetComponentsInChildren<Animator>();

        foreach (var anim in animators)
        {
            if (anim.name.Contains("Sprite"))
                moleAnimator = anim;
            else if (anim.name.Contains("Reaction"))
                reactionAnimator = anim;

        }
        dialogueTrigger = GetComponent<DialogueTrigger>();


        SetAnimationTransitions();
    }

    void Update()
    {
        if (senseUpdateTimer < senseUpdateInterval) // sensing and deciding done slower than acting
        {
            senseUpdateTimer += Time.deltaTime;
        }
        else
        {
            Decide();

            senseUpdateTimer = 0f;
        }

        Act();

        UpdatePlayerFocus();
        UpdateMovement();
        UpdateAnimator();
    }

    void Decide()
    {
        if (moleState == MoleState.Moving)
        {
            float distance = Vector3.Distance(transform.position, waypoint);

            if (distance > wanderRange)
                waypoint = GetRandomPositionAround(transform.position, wanderRange);
            else if (distance <= interactRange)
            {
                waypoint = GetRandomPositionAround(transform.position, wanderRange);
            }
        }
    }

    void Act()
    {
        if (moleState == MoleState.Moving)
        {
            if (distanceToPlayer > visionRange)
                Wander();
        }
        else if (moleState == MoleState.Idle)
        {

        }
    }

    void UpdateAnimator()
    {
        if (moleAnimator)
        {
            moleAnimator.SetFloat("distanceToPlayer", distanceToPlayer);
            moleAnimator.SetBool("isInteracting", distanceToPlayer <= interactRange);
            moleAnimator.SetBool("isPeeking", distanceToPlayer <= visionRange && distanceToPlayer > interactRange);
            moleAnimator.SetBool("isMoving", distanceToPlayer > visionRange);
        }

        if (reactionAnimator)
        {
            if (distanceToPlayer <= visionRange && noticedPlayerThisFrame == Time.frameCount)
                reactionAnimator.SetTrigger("Notice");
        }
    }

    void UpdateMovement()
    {
        float distance = Vector3.Distance(transform.position, previousPos);
        isMoving = distance > 0f;
        previousPos = transform.position;
    }

    void Wander()
    {
        MoveTowards(waypoint);
    }

    void MoveTowards(Vector3 targetPosition, float range = 0f)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);

        Vector3 step = direction * Time.deltaTime * speed;

        if (distance > range)
        {
            if (step.magnitude > distance)
            {
                transform.position = targetPosition;
            }
            else
            {
                transform.position += step;
            }
        }
    }

    Vector3 GetRandomPositionAround(Vector3 position, float range)
    {
        float direction = Random.Range(0f, 359f) * Mathf.Deg2Rad;
        float distance = Random.Range(1f, range);

        Vector3 newPos = position + new Vector3(distance * Mathf.Cos(direction), 0, distance * Mathf.Sin(direction));

        return newPos;
    }

    void UpdatePlayerFocus()
    {
        if (player)
        {
            distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= interactRange)
            {
                PlayerInInteractRange();
            }
            else
            {
                PlayerOutOfInteractRange();
            }

            if (distanceToPlayer <= visionRange && distanceToPlayer > interactRange)
            {
                PlayerInVisionRange();
            }
            else if (distanceToPlayer > visionRange)
            {
                PlayerOutOfVisionRange();
            }
        }
    }

    void PlayerInVisionRange()
    {
        if (!noticedPlayer)
        {
            noticedPlayer = true;
            noticedPlayerThisFrame = Time.frameCount;
        }
    }

    void PlayerOutOfVisionRange()
    {
        noticedPlayer = false;
    }

    void PlayerInInteractRange()
    {
        moleState = MoleState.Idle;
    }

    void PlayerOutOfInteractRange()
    {
        dialogueInitiated = false;
        moleState = MoleState.Moving;
        dialogueTrigger.CloseDialoge();
    }

    public override void Interact()
    {
        Debug.Log("Overriding NPC Interact() with Mole.Interact()");

        if (!dialogueInitiated)
        {
            dialogueTrigger.TriggerDialogue();
            dialogueTrigger.TriggerDialogueChoice(0);
            dialogueInitiated = true;
        }
        else
        {
            if (!dialogueTrigger.TriggerNextDialogueSentence())
                dialogueInitiated = false;
        }
    }

    void SetAnimationTransitions()
    {
        if (moleAnimator)
        {
            //var condition = GetComponent<AnimatorCondition>();
            //condition.mode = AnimatorConditionMode.Less;
            //condition.parameter = "playerDistance"; 
            //condition.threshold = visionRange;

            //var transitionInfo = moleAnimator.GetAnimatorTransitionInfo(0);
            //Debug.Log(transitionInfo);
        }
    }
}
