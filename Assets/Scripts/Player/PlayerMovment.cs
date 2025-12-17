using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class PlayerMovment : MonoBehaviour
{
    public int facingDirection = 1;

    public Rigidbody2D rb;
    public Animator anim;

    private bool isKnockBack;

    public PlayerCombat player_Combat;

    private void Start()
    {
        EnablePlayerMovment();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            player_Combat.Attack();
        }
    }

    private void OnEnable()
    {
        PlayerHealth.Died += DisablePlayerMovment;
    }

    private void OnDisable()
    {
        PlayerHealth.Died -= DisablePlayerMovment;
    }

    void FixedUpdate()
    {
        if (isKnockBack == false)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertival = Input.GetAxisRaw("Vertical");

            if (horizontal > 0 && transform.localScale.x < 0 ||
                horizontal < 0 && transform.localScale.x > 0)
            {
                Flip();
            }

            rb.linearVelocity = new Vector2(horizontal, vertival) * StatsManager.Instance.speed;
        }
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
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


    // new code

    private void DisablePlayerMovment()
    {
        //anim.enabled = false;
        rb.bodyType = RigidbodyType2D.Static;
    }
    public void EnablePlayerMovment()
    {
        //anim.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}
