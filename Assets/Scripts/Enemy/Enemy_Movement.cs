using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR;

public class Enemy_Movement : MonoBehaviour
{
    public float speed;
    public float attackRange = 2f;
    public float attackCooldown = 2; // Time between attacks
    public float playerDetectRange = 5;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    private Enemy_Combat ec;

    private float attackCooldownTimer;
    private int facingDirection = -1; // 1 for right, -1 for left
    private EnemyState enemyState;

    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;

    [Header("Patrol")]
    public float patrolWidth = 5f;                // horizontal patrol area width
    public float patrolHeight = 5f;               // vertical patrol area height
    public float pauseDuration = 1f;              // pause duration at patrol point
    public float minLookDuration = 0.5f;          // min duration of the random "look" pause
    public float maxLookDuration = 1.5f;          // max duration of the random "look" pause
    public float patrolSpeed = 2f;                // movement speed during patrol

    private Vector2 spawnPosition;
    private Vector2 patrolTarget;
    private bool isPaused = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // patrol initialization from spawn point
        spawnPosition = transform.position;
        StartCoroutine(PauseAndPickNewDestination());
    }

    void Update()
    {
        CheckForPlayer();

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        if (enemyState == EnemyState.Chasing)
        {
            Chase();
        }
        else if (enemyState == EnemyState.Attacking)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else if (enemyState == EnemyState.Patrolling)
        {
            Patrol();
        }
    }

    void Chase()
    {
        if (player.position.x > transform.position.x && facingDirection == -1 ||
                player.position.x < transform.position.x && facingDirection == 1)
        {
            Flip();
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }


    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);

    }

    // --- Patrol behaviour -------------------------------------------------
    void Patrol()
    {
        if (isPaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (Vector2.Distance(transform.position, patrolTarget) < 0.1f)
        {
            StartCoroutine(PauseAndPickNewDestination());
            return;
        }

        Move();
    }

    private void Move()
    {
        Vector2 direction = (patrolTarget - (Vector2)transform.position).normalized;
        
        // flip sprite if moving opposite to facing
        if (direction.x < 0 && facingDirection == 1)
            Flip();
        else if (direction.x > 0 && facingDirection == -1)
            Flip();

        rb.linearVelocity = direction * patrolSpeed;
    }

    IEnumerator PauseAndPickNewDestination()
    {
        isPaused = true;
        rb.linearVelocity = Vector2.zero;
        
        // random pause with looking left/right
        float lookDuration = Random.Range(minLookDuration, maxLookDuration);
        yield return new WaitForSeconds(lookDuration);
        
        // randomly face left or right
        Flip();
        Flip();
        
        yield return new WaitForSeconds(pauseDuration);

        patrolTarget = GetRandomPatrolPoint();
        isPaused = false;
    }

    Vector2 GetRandomPatrolPoint()
    {
        float halfWidth = patrolWidth / 2f;
        float halfHeight = patrolHeight / 2f;
        int edge = Random.Range(0, 4);

        return edge switch
        {
            0 => new Vector2(spawnPosition.x - halfWidth, Random.Range(spawnPosition.y - halfHeight, spawnPosition.y + halfHeight)),
            1 => new Vector2(spawnPosition.x + halfWidth, Random.Range(spawnPosition.y - halfHeight, spawnPosition.y + halfHeight)),
            2 => new Vector2(Random.Range(spawnPosition.x - halfWidth, spawnPosition.x + halfWidth), spawnPosition.y - halfHeight),
            _ => new Vector2(Random.Range(spawnPosition.x - halfWidth, spawnPosition.x + halfWidth), spawnPosition.y + halfHeight),
        };
    }

    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);

        if (hits.Length > 0)
        {
            player = hits[0].transform;

            //if the player is in attack range AND cooldown is ready
            if (Vector2.Distance(transform.position, player.position) <= attackRange && attackCooldownTimer <= 0)
            {
                attackCooldownTimer = attackCooldown;
                ChangeState(EnemyState.Attacking);
            }
            else if (Vector2.Distance(transform.position, player.position) > attackRange && enemyState != EnemyState.Attacking)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            // no player found — continue patrolling
            ChangeState(EnemyState.Patrolling);
        }
    }

    void ChangeState(EnemyState newState)
    {
        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", false);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("isAttacking", false);
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("isChasing", false);
        else if (enemyState == EnemyState.Patrolling)
            anim.SetBool("isPatrolling", false);

        enemyState = newState;

        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", true);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("isAttacking", true);
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("isChasing", true);
        else if (enemyState == EnemyState.Patrolling)
            anim.SetBool("isPatrolling", true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (detectionPoint != null)
            Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);

        // patrol bounds visualization (matching NPC_Wander style)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(patrolWidth, patrolHeight, 0));
    }

}

public enum EnemyState
{
    Idle,
    Patrolling,
    Chasing,
    Attacking
}