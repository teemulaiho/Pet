using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedCloud : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 endPos;
    float speed = 1f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Initialize()
    {
        startPos = transform.position;
        endPos = startPos + (-1 * transform.forward) + Vector3.up * 0.5f;
        animator.SetTrigger("Spawn");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, endPos, Time.deltaTime * speed);

        if (Vector3.Distance(transform.position, endPos) <= 0.2f)
        {
            Destroy(this.gameObject);
        }
    }
}
