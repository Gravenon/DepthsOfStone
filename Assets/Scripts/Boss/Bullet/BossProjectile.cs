using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 15;
    private Vector2 _direction;

    public void Init(Vector2 direction)
    {
        _direction = direction.normalized;
        Destroy(gameObject, 5f);
    }

    void Update() =>
        transform.Translate(_direction * speed * Time.deltaTime);

    void OnTriggerEnter2D(Collider2D col)
    {
        PlayerHealth health = col.GetComponent<PlayerHealth>();
        if (health == null) return;
        health.ChangeHealth(-damage);
        Destroy(gameObject);
    }
}