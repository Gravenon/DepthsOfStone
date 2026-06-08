using System.Collections;
using UnityEngine;

public class NPC_Wander : MonoBehaviour
{
    [Header("Wander Area")]
    public float wanderWidth = 5;
    public float wanderHeight = 5;
    public Vector2 startitngPosition;

    public float pauseDuration = 1;
    public float speed = 2;
    public Vector2 target;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isPaused;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        StartCoroutine(PauseAndPickNewDestation());
    }

    private void Update()
    {
        if (isPaused)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (Vector2.Distance(transform.position, target) < .1f)
            StartCoroutine(PauseAndPickNewDestation());

        Move();
    }

    private void Move() 
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        if (direction.x < 0 && transform.localScale.x > 0 || direction.x > 0 && transform.localScale.x < 0)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        rb.linearVelocity = direction * speed;
    }


    IEnumerator PauseAndPickNewDestation()
    {
        isPaused = true;
        yield return new WaitForSeconds(pauseDuration);

        target = GetRandomPosition();
        isPaused = false;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(!enabled) return;
        StartCoroutine(PauseAndPickNewDestation());
    }

    private Vector2 GetRandomPosition()
    {
        float halfWidth = wanderWidth / 2;
        float halfHeight = wanderHeight / 2;
        int edge = Random.Range(0, 4);

        return edge switch
        {
            0 => new Vector2(startitngPosition.x - halfWidth, Random.Range(startitngPosition.y - halfHeight, startitngPosition.y + halfHeight)),
            1 => new Vector2(startitngPosition.x + halfWidth, Random.Range(startitngPosition.y - halfHeight, startitngPosition.y + halfHeight)),
            2 => new Vector2(Random.Range(startitngPosition.x - halfWidth, startitngPosition.x + halfWidth), startitngPosition.y - halfHeight),
            _ => new Vector2(Random.Range(startitngPosition.x - halfWidth, startitngPosition.x + halfWidth), startitngPosition.y + halfHeight),
        };
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(startitngPosition, new Vector3(wanderWidth, wanderHeight, 0));
    }
}
