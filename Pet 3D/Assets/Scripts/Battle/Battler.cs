using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battler : MonoBehaviour
{
    BattleManager battleManager;

    Animator animator;

    SpriteRenderer sr;

    public string Name { get; set; }

    public bool isPlayerPet;
    float maxHealth;
    float currentHealth;
    float strength;
    float baseDamage;

    public int Winnings { get; set; }

    public float RelativeCurrentHealth
    {
        get { return currentHealth / maxHealth; }
    }

    public void SetStats(PetStats stats)
    {
        Name = stats.name;
        this.name = Name;
        strength = stats.strength;

        if (!isPlayerPet)
            sr.color = Color.magenta;
    }

    public void Initialize(BattleManager bm)
    {
        battleManager = bm;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        maxHealth = 25f;
        currentHealth = maxHealth;

        baseDamage = Random.Range(3f, 6f);
    }

    public void SetPlayerPet(bool playerPet)
    {
        isPlayerPet = playerPet;
    }

    public void DoDamage(Battler target)
    {
        //Debug.Log("Called DoDamage to target: " + target.name);

        float totalDamage = baseDamage + strength + Random.Range(0, 3);
        target.TakeDamage(totalDamage);
        animator.SetTrigger("Eat");
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        BattleUI.UpdateDamageTakenUI(isPlayerPet, amount);

        if (currentHealth <= 0)
            battleManager.BattlerUnconscious();
    }

    public void AnimationStart()
    {

    }

    public void AnimationEnd()
    {
        battleManager.OnAnimationEnd();
    }
}
