using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetherBall : MonoBehaviour
{
    [SerializeField] public Rigidbody ballrb;

    public Rigidbody GetBallRb()
    {
        return ballrb;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            Nudge(transform.forward, 10f);
    }

    public void Nudge(Vector3 direction, float nudgeForce)
    {
        ballrb.AddForce(direction * nudgeForce, ForceMode.Impulse);
    }
}
