using System.Collections;
using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    public float speed;
    public float attackRange = 2f;
    public float attackCooldown = 2; // Time between attacks
    public float playerDetectRange = 5;
    public float stoppingDistance = 1.2f;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    private Enemy_Health enemyHealth;
    private Enemy_Combat enemyCombat;

    private float attackCooldownTimer;
    private int facingDirection = -1;
    private EnemyState enemyState;
    private Vector2 lastMoveDirection = Vector2.down;
    private Vector2 currentDirection = Vector2.down;

    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;

    [Header("Patrol")]
    public float patrolWidth = 5f;                // horizontal patrol area width
    public float patrolHeight = 5f;               // vertical patrol area height
    public float pauseDuration = 1f;              // pause duration at patrol point
    public float patrolSpeed = 2f;                // movement speed during patrol

    private Vector2 spawnPosition;
    private Vector2 patrolTarget;
    private bool isPaused = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyHealth = GetComponent<Enemy_Health>();
        enemyCombat = GetComponent<Enemy_Combat>();

        // patrol initialization from spawn point
        spawnPosition = transform.position;
        StartCoroutine(PauseAndPickNewDestination());

        GetComponent<Enemy_Combat>().OnAttackFinished += () =>
        {
            ChangeState(EnemyState.Chasing);
        };
    }

    void Update()
    {
        CheckForPlayer();

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        if (enemyHealth != null && enemyHealth.isKnockback) return;

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

        // Обновляем attack point только если не в состоянии атаки и не атакуем
        if (enemyState != EnemyState.Attacking && !enemyCombat.IsAttacking)
            enemyCombat.SetAttackPointDirection(lastMoveDirection);

        // Обновление направления для анимации
        UpdateAnimationDirection();
    }

    void Chase()
    {
        if (player.position.x > transform.position.x && facingDirection == -1 ||
                player.position.x < transform.position.x && facingDirection == 1)
        {
            Flip();
        }

        float distance = Vector2.Distance(transform.position, player.position);
        float stopAt = Mathf.Max(stoppingDistance, attackRange);
        if (distance > stopAt)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            lastMoveDirection = direction;
            rb.linearVelocity = direction * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;

            if (player != null)
                lastMoveDirection = (player.position - transform.position).normalized;
        }
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);

    }

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
        lastMoveDirection = direction;

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

            if (Vector2.Distance(transform.position, player.position) <= attackRange && attackCooldownTimer <= 0)
            {
                if (!enemyCombat.IsAttacking)
                {
                    attackCooldownTimer = attackCooldown;
                    ChangeState(EnemyState.Attacking);
                    enemyCombat.InitiateAttack();
                }
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
            enemyCombat.ResetAttack();
        }
    }

    void ChangeState(EnemyState newState)
    {
        enemyState = newState;

        anim.SetBool("isAttacking", enemyState == EnemyState.Attacking);
        anim.SetBool("isChasing", enemyState == EnemyState.Chasing);
        anim.SetBool("isPatrolling", enemyState == EnemyState.Patrolling);

        UpdateAnimationDirection();
    }

    void UpdateAnimationDirection()
    {
        if (rb.linearVelocity.magnitude > 0.01f)
        {
            currentDirection = rb.linearVelocity.normalized;
        }
        else if (lastMoveDirection.magnitude > 0.01f)
        {
            currentDirection = lastMoveDirection;
        }

        anim.SetFloat("DirectionX", currentDirection.x);
        anim.SetFloat("DirectionY", currentDirection.y);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (detectionPoint != null)
            Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);

        // patrol bounds visualization (matching NPC_Wander style)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(patrolWidth, patrolHeight, 0));

        Gizmos.color = Color.blue;
        if (enemyCombat != null)
            Gizmos.DrawWireSphere(transform.position, enemyCombat.weaponRange);
    }

}

public enum EnemyState
{
    Patrolling,
    Chasing,
    Attacking,
}