using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedingArea : MonoBehaviour
{
    [SerializeField] private float healthGain;
    public float HealthGain() { return healthGain; }
}
