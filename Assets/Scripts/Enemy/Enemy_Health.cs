using System.Collections;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    public bool isKnockback;
    public bool isDead;

    public int currentHealth;
    public int maxHealth;
    public int expReward = 3;

    public delegate void EnemyDefeated(int exp);
    public static event EnemyDefeated OnEnemyDefeated;

    private Rigidbody2D rb;
    private Collider2D col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void ChangeHealth(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth + (int)amount, 0, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        col.enabled = false;
        // restore timeScale in case HitStop left it at 0
        Time.timeScale = 1f;
        foreach (var mb in GetComponents<MonoBehaviour>())
            mb.StopAllCoroutines();
        OnEnemyDefeated?.Invoke(expReward);
    }

    // Called by animation event on the last frame of the death animation.
    public void DestroyEnemy() => Destroy(gameObject);

    public void Knockback(Transform attacker, float force, float stunTime)
    {
        if (isDead) return;
        isKnockback = true;
        rb.linearVelocity = (transform.position - attacker.position).normalized * force;
        StartCoroutine(KnockbackRoutine(stunTime));
    }

    private IEnumerator KnockbackRoutine(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocity = Vector2.zero;
        isKnockback = false;
    }
}
