using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battler : MonoBehaviour
{
    Animator animator;

    public string Name { get; set; }

    bool isPlayerPet;
    float maxHealth;
    float currentHealth;
    float strength;
    float baseDamage;

    public float RelativeCurrentHealth
    {
        get { return currentHealth / maxHealth; }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();

        maxHealth = 25f;
        currentHealth = maxHealth;

        strength = 1f;
        baseDamage = Random.Range(1f, 3f);
    }

    public void SetPlayerPet(bool playerPet)
    {
        isPlayerPet = playerPet;
    }

    public void DoDamage(Battler target)
    {
        float totalDamage = baseDamage + strength;
        target.TakeDamage(totalDamage);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        BattleUI.UpdateDamageTakenUI(isPlayerPet, amount);
    }
}
