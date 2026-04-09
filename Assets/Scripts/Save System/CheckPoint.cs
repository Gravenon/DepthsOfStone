using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [Tooltip("Destroy the trigger after the player saves once (one-time checkpoint).")]
    [SerializeField] private bool destroyAfterUse = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        DataPersistenceeManager.instance.SaveCheckpoint(other.transform.position);
        Debug.Log("[CheckPoint] Saved at: " + gameObject.name);

        if (destroyAfterUse)
            Destroy(gameObject);
    }
}
