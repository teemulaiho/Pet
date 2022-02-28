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

    string currentInstruction;

    float movementSpeed = 2f;

    bool isActive;
    bool isMoving;
    bool roundActive;

    public float decideDt;
    float decideTimer;


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

        decideTimer = Random.Range(3f, 6f);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (transform.position != contestantSlot.transform.position)
            isMoving = true;

        beautyManager.onRoundStart += RoundStart;
        beautyManager.onRoundEnd += RoundEnd;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
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
    }

    private void DoAction()
    {
        if (isActive)
        {
            decideDt += Time.deltaTime;

            if (decideDt > decideTimer)
            {
                if (currentInstruction.Contains("Jumps"))
                {
                    animator.SetTrigger("Jump");
                }
                else if (currentInstruction.Contains("Eat"))
                {
                    animator.SetTrigger("Eat");
                }
                else if (currentInstruction.Contains("Sleep"))
                {
                    animator.SetTrigger("Sleep");
                }

                isActive = false;
                sr.color = Color.white;
                decideDt = 0f;
            }
        }
    }

    private void RoundStart()
    {
        sr.color = Color.green;
        roundActive = true;

        currentInstruction = beautyManager.GetCurrentInstruction();

        isActive = true;
    }

    private void RoundEnd()
    {
        roundActive = false;
    }
}
