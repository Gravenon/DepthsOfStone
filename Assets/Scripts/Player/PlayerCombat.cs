using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public Animator anim;
    public PlayerMovment playerMovment;

    public float cooldown = 2f;
    public float knockbackForce = 5f;
    public float knockbackStunTime = 0.2f;
    public float hitStopDuration = 0.06f;

    private Vector3 baseAttackOffset;
    private float timer;

    private void Awake()
    {
        baseAttackOffset = attackPoint.localPosition;
    }

    private void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
    }

    public void Attack()
    {
        if (timer > 0) return;
        anim.SetBool("isAttacking", true);
        playerMovment.AudiManager.PlayHitSound();
        timer = cooldown;
        QuestEvents.OnPlayerAttacked?.Invoke();
    }

    // Called by animation event at the hit frame.
    public void DealDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, StatsManager.Instance.weaponRange, enemyLayers);

        bool hitAny = false;
        foreach (var enemy in enemies)
        {
            var enemyHealth = enemy.GetComponent<Enemy_Health>();
            var bossHealth  = enemy.GetComponent<Boss_Health>();

            if (enemyHealth == null && bossHealth == null) continue;

            enemyHealth?.ChangeHealth(-StatsManager.Instance.damage);
            enemyHealth?.Knockback(transform, knockbackForce, knockbackStunTime);
            bossHealth?.ChangeHealth(-StatsManager.Instance.damage);

            hitAny = true;
        }

        if (hitAny)
            StartCoroutine(HitStop(hitStopDuration));
    }

    // Called by animation event on the last frame of the attack.
    public void FinishAttacking()
    {
        anim.SetBool("isAttacking", false);
    }

    public void SetAttackPointDirection(Vector2 direction)
    {
        float dist = baseAttackOffset.magnitude;
        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
            attackPoint.localPosition = new Vector3(dist * Mathf.Sign(direction.x), -0.15f, baseAttackOffset.z);
        else
            attackPoint.localPosition = new Vector3(0f, dist * Mathf.Sign(direction.y), baseAttackOffset.z);
    }

    private IEnumerator HitStop(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}
