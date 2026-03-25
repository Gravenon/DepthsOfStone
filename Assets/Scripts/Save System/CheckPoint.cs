using UnityEngine;

/// <summary>
/// Place this trigger in the scene. When the player walks through it the game
/// is saved: position, stats and inventory are all written to disk.
/// Replaces the old TriggerSave component.
/// </summary>
public class CheckPoint : MonoBehaviour
{
    [Tooltip("Destroy the trigger after the player saves once (one-time checkpoint).")]
    [SerializeField] private bool destroyAfterUse = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        DataPersistenceeManager.instance.SaveGame();
        Debug.Log("[CheckPoint] Saved at: " + gameObject.name);

        if (destroyAfterUse)
            Destroy(gameObject);
    }
}
