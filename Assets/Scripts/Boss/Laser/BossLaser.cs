using System.Collections;
using UnityEngine;

public class BossLaser : MonoBehaviour
{
    [Header("Warning")]
    [SerializeField] private LineRenderer warningLine;
    [SerializeField] private float warningDuration = 1.5f;
    [SerializeField] private Color warningColor = Color.red;
    [SerializeField] private float laserHalfWidth = 15f;  // room width / 2

    [Header("Laser")]
    [SerializeField] private GameObject laserObject;
    [SerializeField] private float fireDuration = 1f;
    [SerializeField] private int damage = 10;
    [SerializeField] private LayerMask playerLayer;

    private Transform _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        if (laserObject != null)
            laserObject.SetActive(false);
    }

    public void Fire()
    {
        if (_player == null) return;
        StartCoroutine(LaserRoutine());
    }

    private IEnumerator LaserRoutine()
    {
        float targetY = _player.position.y;

        // Warning — horizontal line at player Y
        if (warningLine != null)
        {
            warningLine.startColor = warningColor;
            warningLine.endColor   = warningColor;
            warningLine.SetPosition(0, new Vector3(-laserHalfWidth, targetY, 0f));
            warningLine.SetPosition(1, new Vector3( laserHalfWidth, targetY, 0f));
            warningLine.enabled = true;
        }

        yield return new WaitForSeconds(warningDuration);

        // Fire — hide warning, show laser object
        if (warningLine != null) warningLine.enabled = false;
        if (laserObject  != null) laserObject.SetActive(true);

        // Reposition laser object to the locked Y
        if (laserObject != null)
        {
            Vector3 p = laserObject.transform.position;
            laserObject.transform.position = new Vector3(p.x, targetY, p.z);
        }

        float elapsed = 0f;
        while (elapsed < fireDuration)
        {
            // BoxCast across the whole row
            Vector2 origin = new Vector2(0f, targetY);
            RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, new Vector2(laserHalfWidth * 2f, 0.4f), 0f, Vector2.zero, 0f, playerLayer);
            foreach (var h in hits)
                h.collider.GetComponent<PlayerHealth>()?.ChangeHealth(-damage);

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (laserObject != null) laserObject.SetActive(false);
    }
}
