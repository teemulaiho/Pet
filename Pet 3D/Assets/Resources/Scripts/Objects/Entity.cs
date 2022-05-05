using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Rigidbody rb;

    public virtual void Pickup() { Destroy(this.gameObject); }
}
