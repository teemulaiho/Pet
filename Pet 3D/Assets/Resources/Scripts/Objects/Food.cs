using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Entity
{
    [SerializeField] protected float healthGain;
    public float HealthGain() { return healthGain; }

    public override void Pickup() 
    {
        Destroy(this.gameObject);
    }
}
