using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillContestant : MonoBehaviour
{
    SkillContestManager skillContestManager;
    ContestantSlot contestantSlot;
    Animator animator;
    TMP_Text instruction;
    SpriteRenderer sr;
    Rigidbody rb;

    string currentInstruction;
    string actionDecided = "";
    string actionDone = "";

    float movementSpeed = 2f;

    public bool isPlayerPet { get; set; }
    bool isReleased;
    bool isActive;
    bool isMoving;
    bool roundActive;

    public float decideDt;
    float decideTimer;

    int score;
    public int Rank { get; set; }
    public int Winnings { get; set; }

    // Pet Stats
    public string Name { get; set; }
    private float intellect;

    public void SetStats(PetStats stats)
    {
        Name = stats.name;
        this.name = Name;
        intellect = stats.intellect;

        if (!isPlayerPet)
            sr.color = Color.magenta;
    }

    public void Initialize(SkillContestManager bm, ContestantSlot slot)
    {
        skillContestManager = bm;
        contestantSlot = slot;
        contestantSlot.IsFree = false;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        instruction = GameObject.FindGameObjectWithTag("BeautyInstruction").GetComponent<TMP_Text>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody>();

        decideTimer = Random.Range(1f, 6f);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (transform.position != contestantSlot.transform.position)
            isMoving = true;

        skillContestManager.onRoundStart += RoundStart;
        skillContestManager.onRoundEnd += RoundEnd;

        sr.flipX = transform.position.x - contestantSlot.transform.position.x < 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving && isReleased)
            Move();

        if (roundActive)
            Act();
    }

    private void Move()
    {
        if (Vector3.Distance(transform.position, contestantSlot.transform.position) > 0.5f)
            transform.position = Vector3.MoveTowards(transform.position, contestantSlot.transform.position, Time.deltaTime * movementSpeed);
        else
            isMoving = false;

        animator.SetBool("isMoving", isMoving);
    }

    private void Act()
    {
        if (isActive)
        {
            decideDt += Time.deltaTime;

            if (decideDt > decideTimer)
            {
                DecideAction();
                DoAction();
            }
        }
    }

    private void DecideAction()
    {
        float instructionCount = (float) skillContestManager.GetInstructionCount();
        float chanceOfCorrectAnswer = intellect / instructionCount; // 2 / 3 = 66%
        float randomValue = Random.Range(0f, 100f);

        if (randomValue <= chanceOfCorrectAnswer)
            actionDecided = skillContestManager.GetCurrentInstruction();
        else
        {
            int randomIndex = Random.Range(0, skillContestManager.GetInstructionCount());

            actionDecided = skillContestManager.GetInstructionInIndex(randomIndex);
        }
    }

    private void DoAction()
    {
        if (actionDecided.Contains("Jump"))
        {
            animator.SetTrigger("Jump");
            actionDone = "Jump";
            rb.AddForce(Vector3.up * 200f);
        }
        else if (actionDecided.Contains("Eat"))
        {
            animator.SetTrigger("Eat");
            actionDone = "Eat";
        }
        else if (actionDecided.Contains("Sleep"))
        {
            animator.SetBool("isSleeping", true);
            //animator.SetTrigger("Sleep");
            actionDone = "Sleep";
        }

        //if (currentInstruction.Contains("Jump"))
        //{
        //    animator.SetTrigger("Jump");
        //    actionDone = "Jump";
        //    rb.AddForce(Vector3.up * 200f);
        //}
        //else if (currentInstruction.Contains("Eat"))
        //{
        //    animator.SetTrigger("Eat");
        //    actionDone = "Eat";
        //}
        //else if (currentInstruction.Contains("Sleep"))
        //{
        //    animator.SetBool("isSleeping", true);
        //    //animator.SetTrigger("Sleep");
        //    actionDone = "Sleep";
        //}

        skillContestManager.SetContestantAction(this, actionDone);

        isActive = false;
        sr.color = Color.white;
        decideDt = 0f;
    }

    private void ResetAnimatorValues()
    {
        animator.Rebind();
        animator.Update(0f);
    }

    private void RoundStart()
    {
        sr.flipX = false;
        ResetAnimatorValues();    
        roundActive = true;
        currentInstruction = skillContestManager.GetCurrentInstruction();
        isActive = true;
    }

    private void RoundEnd()
    {
        roundActive = false;
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
    }

    public int GetScore()
    {
        return score;
    }

    public void Release()
    {
        isReleased = true;
    }
}
