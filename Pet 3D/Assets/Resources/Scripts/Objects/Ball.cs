using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Entity
{
    public Item itemData;

    public delegate void OnKickEvent(Transform kicker);
    public event OnKickEvent onKick;

    public bool hasBounced = true;
    public bool _isGhost;

    public void Throw(Vector3 startPos, Vector3 direction, float force, bool isGhost)
    {
        _isGhost = isGhost;
        GetComponent<Rigidbody>().AddForce(direction * force);
    }

    public void Kick(Transform kicker, Vector3 direction, float force)
    {
        if (kicker.CompareTag("Player"))
        {
            this.transform.SetParent(null);
            hasBounced = false;
        }

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
        if (_isGhost)
            return;

        if (collision.collider.CompareTag("Ground"))
        {
            //Debug.Log("collision with ground.");
            //rb.velocity = Vector3.zero;
            //rb.angularVelocity = Vector3.zero;
            hasBounced = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_isGhost)
            return;

        if (other.CompareTag("DropCatcher"))
        {
            Debug.Log("Ball OnTriggerEnter With DropCatcher.");
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (_isGhost)
            return;

        if (collision.gameObject.CompareTag("Pet"))
        {
            Nudge(Vector3.up, 20f);
        }
    }
}
