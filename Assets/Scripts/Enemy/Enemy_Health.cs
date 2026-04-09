using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    private Rigidbody2D rb;
    public bool isKnockback;

    public int currentHealth = 100;
    public int maxHealth;

    public int expReward = 3; // Experience points awarded to the player upon defeating this enemy

    public delegate void EnemyDefeated(int expReward);
    public static event EnemyDefeated OnEnemyDefeated;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            OnEnemyDefeated(expReward); 
            Destroy(gameObject);
        }
    }

    public void Knockback(Transform attacker, float force, float stunTime)
    {
        if (rb == null) return;
        isKnockback = true;
        Vector2 direction = (transform.position - attacker.position).normalized;
        rb.linearVelocity = direction * force;
        StartCoroutine(KnockbackCounter(stunTime));
    }

    IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        if (rb != null) rb.linearVelocity = Vector2.zero;
        isKnockback = false;
    }

}
