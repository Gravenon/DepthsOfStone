using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovment : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator anim;

    private bool isKnockBack;
    private float lastHorizontalSign = 1f;

    public PlayerCombat playerCombat;
    public PlayerHealth playerHealth;

    private float horizontal;
    private float vertival;

    private Vector2 lastMoveDirection = Vector2.down;

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
    }

    void FixedUpdate()
    {
        if (playerHealth.isDead && isKnockBack)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (horizontal != 0)
        {
            float currentSign = Mathf.Sign(horizontal);
            if (currentSign != lastHorizontalSign)
            {
                lastHorizontalSign = currentSign;
                playerCombat.FlipAttackPoint();
            }
        }


        rb.linearVelocity = new Vector2(horizontal, vertival) * StatsManager.Instance.speed;

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
        if (!context.performed)
        {
            return;
        }

        playerCombat.Attack();
    }
    #endregion

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
}
