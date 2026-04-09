using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask enemyLayers;

    private Vector3 baseAttackOffset;

    public Animator anim;

    private void Awake()
    {
        baseAttackOffset = attackPoint.localPosition;
    }

    public float colldown = 2;
    private float timer;

    public float knockbackForce = 5f;
    public float knockbackStunTime = 0.2f;
    public float hitStopDuration = 0.05f;

    public PlayerMovment playerMovment;

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public void Attack()
    {
        if (timer <= 0)
        {
            anim.SetBool("isAttacking", true);
            playerMovment.AudiManager.PlayHitSound();
            timer = colldown;
        }
    }

    public void DealDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, StatsManager.Instance.weaponRange, enemyLayers);

        if (enemies.Length > 0)
        {
            Enemy_Health enemyHealth = enemies[0].GetComponent<Enemy_Health>();
            enemyHealth.ChangeHealth(-StatsManager.Instance.damage);
            enemyHealth.Knockback(transform, knockbackForce, knockbackStunTime);
            StartCoroutine(HitSotp(hitStopDuration));
        }
    }

    public void FinishAttacking()
    {
        anim.SetBool("isAttacking", false);
    }

    public void SetAttackPointDirection(Vector2 direction)
    {
        float dist = baseAttackOffset.magnitude;
        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            attackPoint.localPosition = new Vector3(dist * Mathf.Sign(direction.x), -0.15f, baseAttackOffset.z);
        }
        else
        {
            attackPoint.localPosition = new Vector3(0f, dist * Mathf.Sign(direction.y), baseAttackOffset.z);
        }
    }

    IEnumerator HitSotp(float time)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1f;
    }





    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(attackPoint.position, StatsManager.Instance.weaponRange);
    // }
}
