using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BeautyContestant : MonoBehaviour
{
    BeautyManager beautyManager;
    ContestantSlot contestantSlot;
    Animator animator;
    TMP_Text instruction;
    SpriteRenderer sr;
    Rigidbody rb;

    string currentInstruction;
    string actionDone = "";

    float movementSpeed = 2f;

    bool isReleased;
    bool isActive;
    bool isMoving;
    bool roundActive;

    public float decideDt;
    float decideTimer;

    int score;


    public void Initialize(BeautyManager bm, ContestantSlot slot)
    {
        beautyManager = bm;
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

        beautyManager.onRoundStart += RoundStart;
        beautyManager.onRoundEnd += RoundEnd;

        sr.flipX = transform.position.x - contestantSlot.transform.position.x < 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving && isReleased)
            Move();

        if (roundActive)
            DoAction();
    }

    private void Move()
    {
        if (Vector3.Distance(transform.position, contestantSlot.transform.position) > 0.5f)
            transform.position = Vector3.MoveTowards(transform.position, contestantSlot.transform.position, Time.deltaTime * movementSpeed);
        else
            isMoving = false;

        animator.SetBool("isMoving", isMoving);
    }

    private void DoAction()
    {
        if (isActive)
        {
            decideDt += Time.deltaTime;

            if (decideDt > decideTimer)
            {
                if (currentInstruction.Contains("Jump"))
                {
                    animator.SetTrigger("Jump");
                    actionDone = "Jump";
                    rb.AddForce(Vector3.up * 200f);
                }
                else if (currentInstruction.Contains("Eat"))
                {
                    animator.SetTrigger("Eat");
                    actionDone = "Eat";
                }
                else if (currentInstruction.Contains("Sleep"))
                {
                    animator.SetBool("isSleeping", true);
                    //animator.SetTrigger("Sleep");
                    actionDone = "Sleep";
                }

                beautyManager.SetContestantAction(this, actionDone);

                isActive = false;
                sr.color = Color.white;
                decideDt = 0f;
            }
        }
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
        currentInstruction = beautyManager.GetCurrentInstruction();
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
