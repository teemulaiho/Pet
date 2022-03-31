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

    public Ball CaptureBall(Transform newParent, bool isTakingToGoal)
    {
        if (isTakingToGoal)
        {
            IsTakingToGoal(newParent);
            //transform.parent = newParent;
            //Vector3 newLocalPos = new Vector3(0f, 0.5f, 0f);
            //transform.localPosition += newLocalPos;
            //rb.isKinematic = true;
            //Destroy(rb);
        }

        return this;
    }

    /// <summary>
    /// Returns true if pet is already set as parent transform.
    /// </summary>
    /// <param name="newParent"></param>
    /// <returns></returns>
    public bool IsTakingToGoal(Transform newParent)
    {
        if (transform.parent == newParent)
            return true;

        transform.parent = newParent;
        Vector3 newLocalPos = new Vector3(0f, 0.5f, 0f);
        transform.localPosition += newLocalPos;
        rb.isKinematic = true;
        Destroy(rb);
        return false;
    }

    public void Kick(Vector3 direction, float force)
    {
        Vector3 directionVariance = Vector3.zero;

        directionVariance.x = Random.Range(-2f, 2f);
        directionVariance.y = Random.Range(0.5f, 2f);
        directionVariance.z = Random.Range(-2f, 2f);

        rb.AddForce((direction + directionVariance) * force * 100f);
    }

    public void Nudge(Vector3 direction, float force)
    {
        rb.AddForce(direction * force);
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
        if (other.CompareTag("DropCatcher"))
        {
            Debug.Log("Ball OnTriggerEnter With DropCatcher.");
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pet"))
        {
            Nudge(Vector3.up, 20f);
        }
    }

}
