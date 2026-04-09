using System.Collections;
using UnityEngine;
using System;

public class Enemy_Combat : MonoBehaviour
{
    public Action OnAttackFinished;

    public int damage = 1;
    public Transform attackPoint;
    public float weaponRange;
    public float knockbackForce;
    public float stunTime;
    public float hitStopDuration = 0.03f;
    public LayerMask playerLayer;

    private Vector2 attackDirection;
    private Vector3 baseAttackOffset;
    private bool attackPointLocked;

    [SerializeField] private float windupTime = 0.5f;
    [SerializeField] private float attackSpeed = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Transform player;
    private Coroutine attackRoutine;

    private bool isAttacking;
    private bool hasHit;
    public bool IsAttacking => isAttacking;

    void Awake()
    {
        baseAttackOffset = attackPoint.localPosition;
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void SetAttackPointDirection(Vector2 direction)
    {
        if (attackPointLocked) return;

        float dist = baseAttackOffset.magnitude;
        float scaleSign = Mathf.Sign(transform.localScale.x);
        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            attackPoint.localPosition = new Vector3(dist * Mathf.Sign(direction.x) * scaleSign, -0.15f, baseAttackOffset.z);
        }
        else
        {
            attackPoint.localPosition = new Vector3(0f, dist * Mathf.Sign(direction.y), baseAttackOffset.z);
        }
    }
    
    public void InitiateAttack()
    {
        if (isAttacking) return;

        isAttacking = true;
        hasHit = false;
        
        attackDirection = (player.position - transform.position).normalized;
        SetAttackPointDirection(attackDirection);
        attackPointLocked = true;

        rb.linearVelocity = Vector2.zero;
    }

    public void FlashTelegraph()
    {
        if (attackRoutine != null)
            StopCoroutine(attackRoutine);
            
        attackRoutine = StartCoroutine(FlashTelegraphRoutine());
    }

    public void StartAttacking()
    {
        if (hasHit) return;
        
        rb.linearVelocity = attackDirection * attackSpeed;
        Attack();
        StartCoroutine(StopAttackMovement());
    }

    private IEnumerator StopAttackMovement()
    {
        yield return new WaitForSeconds(0.05f);
        rb.linearVelocity = Vector2.zero;
    }

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
            PlayerHealth health = hit.GetComponent<PlayerHealth>();

            if (health != null)
            {
                hasHit = true;
                health.ChangeHealth(-damage);

                StartCoroutine(HitStop(hitStopDuration));

                hit.GetComponent<PlayerMovment>().Knockback(transform, knockbackForce, stunTime);

                break; 
            }
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

    IEnumerator FlashTelegraphRoutine()
    {
        float t = 0;

        while (t < windupTime)
        {
            float pulse = Mathf.PingPong(t * 10f, 1f);
            spriteRenderer.color = Color.Lerp(Color.red, Color.white, pulse);

            t += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = Color.white;
    }

    IEnumerator HitStop(float time)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1f;
    }
}
