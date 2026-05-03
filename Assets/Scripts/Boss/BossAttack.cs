using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public int damage = 20;
    public int damageInRage = 40;

    public Vector3 attackOffset;
    public float attackRange = 1f;
    public LayerMask attackMask;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private BossLaser laser;

    private Transform _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Attack()
    {
        Vector3 pos = transform.position
            + transform.right * attackOffset.x
            + transform.up   * attackOffset.y;

        Collider2D col = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        col?.GetComponent<PlayerHealth>()?.ChangeHealth(-damage);
    }

    public void RangedAttack()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("[BossAttack] bulletPrefab or firePoint is not assigned.");
            return;
        }

        Vector2 dir = (_player.position - firePoint.position).normalized;
        Instantiate(bulletPrefab, firePoint.position, Quaternion.identity)
            .GetComponent<BossProjectile>()?.Init(dir);
    }

    public void RageAttack()
    {
        Vector3 pos = transform.position
            + transform.right * attackOffset.x
            + transform.up   * attackOffset.y;

        Collider2D col = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        col?.GetComponent<PlayerHealth>()?.ChangeHealth(-damageInRage);
    }

    public void LaserAttack()
    {
        if (laser == null)
        {
            Debug.LogWarning("[BossAttack] BossLaser is not assigned.");
            return;
        }

        laser.Fire();
    }
}
