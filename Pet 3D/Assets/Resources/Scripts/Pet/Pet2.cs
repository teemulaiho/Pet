using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet2 : MonoBehaviour
{
    private BehaviorTree behavior;

    private float maxHealth = 100f;
    [SerializeField] private float health = Persistent.petStats.health;

    private float maxEnergy = 100f;
    [SerializeField] private float energy = Persistent.petStats.energy;

    private SpriteRenderer spriteRenderer;

    [SerializeField] Animator petAnimator;
    [SerializeField] Animator healthAnimator;
    [SerializeField] Animator reactionAnimator;
}
