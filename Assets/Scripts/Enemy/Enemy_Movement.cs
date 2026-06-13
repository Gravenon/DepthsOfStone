using System.Collections;
using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    [Header("Combat")]
    public float speed = 3.4f;
    public float attackCooldown = 1.7f;
    public float playerDetectRange = 6.5f;
    public float stoppingDistance = 1.2f;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    [Header("Patrol")]
    public float patrolWidth = 5f;
    public float patrolHeight = 5f;
    public float pauseDuration = 1f;
    public float patrolSpeed = 2f;

    private Rigidbody2D rb;
    private Animator anim;
    private Transform player;
    private Enemy_Health enemyHealth;
    private Enemy_Combat enemyCombat;

    private EnemyState enemyState;
    private Vector2 lastMoveDirection = Vector2.down;
    private Vector2 currentDirection = Vector2.down;
    private float attackCooldownTimer;
    private int facingDirection = -1;

    private Vector2 spawnPosition;
    private Vector2 patrolTarget;
    private bool isPaused;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyHealth = GetComponent<Enemy_Health>();
        enemyCombat = GetComponent<Enemy_Combat>();

        spawnPosition = transform.position;
        StartCoroutine(PauseAndPickNewDestination());

        enemyCombat.OnAttackFinished += () => ChangeState(EnemyState.Chasing);
    }

    private void Update()
    {
        if (enemyHealth.isDead && enemyState != EnemyState.Dead)
        {
            ChangeState(EnemyState.Dead);
            return;
        }

        if (enemyState == EnemyState.Dead) return;

        CheckForPlayer();

        if (attackCooldownTimer > 0)
            attackCooldownTimer -= Time.deltaTime;

        if (enemyHealth.isKnockback) return;

        switch (enemyState)
        {
            case EnemyState.Chasing: Chase();   
            break;
            case EnemyState.Attacking: rb.linearVelocity = Vector2.zero; 
            break;
            case EnemyState.Patrolling: Patrol(); 
            break;
        }

        if (enemyState != EnemyState.Attacking && !enemyCombat.IsAttacking)
            enemyCombat.SetAttackPointDirection(lastMoveDirection);

        UpdateAnimationDirection();
    }

    private void Chase()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        float stopAt = Mathf.Max(stoppingDistance, enemyCombat.AttackReach);
        Vector2 toPlayer = (player.position - transform.position).normalized;

        if ((player.position.x > transform.position.x && facingDirection == -1) ||
            (player.position.x < transform.position.x && facingDirection == 1))
            Flip();

        if (distance > stopAt)
        {
            lastMoveDirection = toPlayer;
            rb.linearVelocity = toPlayer * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            lastMoveDirection = toPlayer;
        }
    }

    private void Patrol()
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

        Vector2 direction = (patrolTarget - (Vector2)transform.position).normalized;
        lastMoveDirection = direction;

        if (direction.x < 0 && facingDirection ==  1) Flip();
        else if (direction.x > 0 && facingDirection == -1) Flip();

        rb.linearVelocity = direction * patrolSpeed;
    }

    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);

        if (hits.Length > 0)
        {
            player = hits[0].transform;
            float distance    = Vector2.Distance(transform.position, player.position);
            float attackReach = enemyCombat.AttackReach;

            if (distance <= attackReach && attackCooldownTimer <= 0 && !enemyCombat.IsAttacking)
            {
                attackCooldownTimer = attackCooldown;
                ChangeState(EnemyState.Attacking);
                enemyCombat.InitiateAttack();
            }
            else if (distance > attackReach && enemyState != EnemyState.Attacking)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            ChangeState(EnemyState.Patrolling);
            enemyCombat.ResetAttack();
        }
    }

    private void ChangeState(EnemyState newState)
    {
        if (enemyState == EnemyState.Dead) return;

        enemyState = newState;

        if (newState == EnemyState.Dead)
        {
            rb.linearVelocity = Vector2.zero;
            StopAllCoroutines();
            anim.SetBool("isAttacking", false);
            anim.SetBool("isChasing", false);
            anim.SetBool("isPatrolling", false);
            anim.SetBool("isDead", true);
            return;
        }

        anim.SetBool("isAttacking", enemyState == EnemyState.Attacking);
        anim.SetBool("isChasing", enemyState == EnemyState.Chasing);
        anim.SetBool("isPatrolling", enemyState == EnemyState.Patrolling);
        UpdateAnimationDirection();
    }

    private void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void UpdateAnimationDirection()
    {
        if (rb.linearVelocity.magnitude > 0.01f)
            currentDirection = rb.linearVelocity.normalized;
        else if (lastMoveDirection.magnitude > 0.01f)
            currentDirection = lastMoveDirection;

        anim.SetFloat("DirectionX", currentDirection.x);
        anim.SetFloat("DirectionY", currentDirection.y);
    }

    private IEnumerator PauseAndPickNewDestination()
    {
        isPaused = true;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(pauseDuration);
        patrolTarget = GetRandomPatrolPoint();
        isPaused = false;
    }

    private Vector2 GetRandomPatrolPoint()
    {
        float hw = patrolWidth  / 2f;
        float hh = patrolHeight / 2f;
        return Random.Range(0, 4) switch
        {
            0 => new Vector2(spawnPosition.x - hw, Random.Range(spawnPosition.y - hh, spawnPosition.y + hh)),
            1 => new Vector2(spawnPosition.x + hw, Random.Range(spawnPosition.y - hh, spawnPosition.y + hh)),
            2 => new Vector2(Random.Range(spawnPosition.x - hw, spawnPosition.x + hw), spawnPosition.y - hh),
            _ => new Vector2(Random.Range(spawnPosition.x - hw, spawnPosition.x + hw), spawnPosition.y + hh),
        };
    }
}

public enum EnemyState { Patrolling, Chasing, Attacking, Dead }
