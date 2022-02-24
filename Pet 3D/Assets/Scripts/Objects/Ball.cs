using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Item item;
    protected Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public Ball CaptureBall(Transform newParent)
    {
        transform.parent = newParent;
        Vector3 newLocalPos = new Vector3(0f,0.5f,0f);
        transform.localPosition += newLocalPos;
        rb.isKinematic = true;
        Destroy(rb);

        return this;
    }

    public void ReleaseBall()
    {
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            //Debug.Log("collision with ground.");
            //rb.velocity = Vector3.zero;
            //rb.angularVelocity = Vector3.zero;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FoodCatcher"))
        {
            Debug.Log("Ball OnTriggerEnter With FoodCatcher.");
            Destroy(this.gameObject);
        }
    }
}
