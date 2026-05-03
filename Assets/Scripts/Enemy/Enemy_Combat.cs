using System;
using System.Collections;
using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    public Action OnAttackFinished;

    public float damage = 1f;
    public float weaponRange;
    public float knockbackForce;
    public float stunTime;
    public LayerMask playerLayer;

    [SerializeField] private float hitStopDuration = 0.03f;
    [SerializeField] private float windupTime = 0.5f;
    [SerializeField] private float attackSpeed = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public Transform attackPoint;

    private Rigidbody2D rb;
    private Transform player;
    private SkillManager skillManager;
    private Coroutine flashRoutine;

    private Vector2 attackDirection;
    private Vector3 baseAttackOffset;
    private bool attackPointLocked;
    private bool isAttacking;
    private bool hasHit;

    public bool IsAttacking => isAttacking;
    public float AttackReach => baseAttackOffset.magnitude + weaponRange;

    private void Awake()
    {
        baseAttackOffset = attackPoint.localPosition;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        skillManager = FindFirstObjectByType<SkillManager>();
    }

    public void SetAttackPointDirection(Vector2 direction)
    {
        if (attackPointLocked) return;

        float dist = baseAttackOffset.magnitude;
        float scaleSign = Mathf.Sign(transform.localScale.x);

        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
            attackPoint.localPosition = new Vector3(dist * Mathf.Sign(direction.x) * scaleSign, -0.15f, baseAttackOffset.z);
        else
            attackPoint.localPosition = new Vector3(0f, dist * Mathf.Sign(direction.y), baseAttackOffset.z);
    }

    public void InitiateAttack()
    {
        if (isAttacking || player == null) return;

        isAttacking = true;
        hasHit = false;
        attackDirection = (player.position - transform.position).normalized;
        SetAttackPointDirection(attackDirection);
        attackPointLocked = true;
        rb.linearVelocity = Vector2.zero;
    }

    // Animation Event: frame 0 — red pulse telegraph before attack
    public void FlashTelegraph()
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    // Animation Event: mid-swing — lunge forward and deal damage
    public void StartAttacking()
    {
        if (hasHit) return;
        rb.linearVelocity = attackDirection * attackSpeed;
        Attack();
        StartCoroutine(StopLunge());
    }

    // Animation Event: last frame — reset state
    public void ChangeState()
    {
        rb.linearVelocity = Vector2.zero;
        spriteRenderer.color = Color.white;
        attackPointLocked = false;
        isAttacking = false;
        hasHit = false;
        OnAttackFinished?.Invoke();
    }

    public void Attack()
    {
        if (hasHit) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);
        foreach (var hit in hits)
        {
            var health = hit.GetComponent<PlayerHealth>();
            if (health == null) continue;

            hasHit = true;
            float finalDamage = skillManager != null ? skillManager.ApplyDamageReduction(damage) : damage;
            health.ChangeHealth(-finalDamage);
            hit.GetComponent<PlayerMovment>()?.Knockback(transform, knockbackForce, stunTime);
            StartCoroutine(HitStop());
            break;
        }
    }

    public void ResetAttack()
    {
        StopAllCoroutines();
        rb.linearVelocity = Vector2.zero;
        spriteRenderer.color = Color.white;
        isAttacking = false;
        attackPointLocked = false;
        hasHit = false;
    }

    private IEnumerator StopLunge()
    {
        yield return new WaitForSeconds(0.05f);
        rb.linearVelocity = Vector2.zero;
    }

    private IEnumerator FlashRoutine()
    {
        float t = 0f;
        while (t < windupTime)
        {
            spriteRenderer.color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(t * 10f, 1f));
            t += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = Color.white;
    }

    private IEnumerator HitStop()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1f;
    }
}
