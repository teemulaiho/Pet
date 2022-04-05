using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Entity
{
    public Item itemData;

    public delegate void OnKickEvent(Transform kicker);
    public event OnKickEvent onKick;

    public void Kick(Transform kicker, Vector3 direction, float force)
    {
        if (!kicker.CompareTag("Pet"))
            this.transform.SetParent(null);

        rb.isKinematic = false;
        rb.AddForce(direction * force);

        if (onKick != null) // onKick is null if nothing has subsrcibed to it. (ie. Pet hasn't detected the ball yet.) -Teemu
            onKick(kicker);
    }

    public void Nudge(Vector3 direction, float force)
    {
        //rb.AddForce(direction * force);
        //Debug.Log("Ball nudging itself.");
    }

    public void Pickup()
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
