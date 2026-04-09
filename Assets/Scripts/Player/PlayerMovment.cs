using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovment : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator anim;

    public PlayerCombat playerCombat;
    public PlayerHealth playerHealth;

    public AudiManager AudiManager;

    [Header("State")]
    private bool isKnockBack;
    private float lastHorizontalSign = 1f;

    [Header("Movement Settings")]
    private float horizontal;
    private float vertival;
    private Vector2 lastMoveDirection = Vector2.down;


    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 12f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private GameObject dashTrailObj;

    private TrailRenderer[] dashTrails;
    private bool isDashing;
    private bool canDash = true;

    private void Awake()
    {
        dashTrails = dashTrailObj.GetComponentsInChildren<TrailRenderer>();
    }

    void Update()
    {
        UpdateAnimations();
    }

    void UpdateAnimations()
    {
        if (horizontal != 0 || vertival != 0)
        {
            anim.SetBool("isWalking", true);
            lastMoveDirection = new Vector2(horizontal, vertival).normalized;
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

        anim.SetFloat("LastInputX", lastMoveDirection.x);
        anim.SetFloat("LastInputY", lastMoveDirection.y);

        playerCombat.SetAttackPointDirection(lastMoveDirection);
    }

    void FixedUpdate()
    {
        if (playerHealth.isDead && isKnockBack)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (!isKnockBack && !isDashing)
        {
            rb.linearVelocity = new Vector2(horizontal, vertival) * StatsManager.Instance.speed;
        }

    }

    #region  PLAYER_CONTROLS
    public void Move(InputAction.CallbackContext context)
    {
        anim.SetBool("isWalking", true);

        horizontal = context.ReadValue<Vector2>().x;
        vertival = context.ReadValue<Vector2>().y;

        anim.SetFloat("InputX", horizontal);
        anim.SetFloat("InputY", vertival);

    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        playerCombat.Attack();
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        StartCoroutine(DashRoutine());
        
    }
    #endregion

    private IEnumerator DashRoutine()
    {
        if (!canDash || isDashing) yield break;

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);
        canDash = false;
        isDashing = true;
        playerHealth.SetInvulnerable(isDashing);
        UpdateDashTrail(isDashing);

        Vector2 dashDirection = lastMoveDirection.normalized;

        if (dashDirection == Vector2.zero)
            dashDirection = new Vector2(lastHorizontalSign, 0);

        AudiManager.PlayDashSound();
        rb.linearVelocity = dashDirection * dashForce;

        yield return new WaitForSecondsRealtime(dashTime);

        rb.linearVelocity *= 0.3f;
        
        isDashing = false;
        playerHealth.SetInvulnerable(isDashing);
        UpdateDashTrail(isDashing);
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), false);

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void Knockback(Transform enemy, float force, float stunTime)
    {
        isKnockBack = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.linearVelocity = direction * force;
        StartCoroutine(KnockbackCounter(stunTime));
    }

    IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocity = Vector2.zero;
        isKnockBack = false;
    }

    private void UpdateDashTrail(bool isDashing)
    {
        foreach(var trail in dashTrails){
            trail.emitting = isDashing;
        }
    }
}
