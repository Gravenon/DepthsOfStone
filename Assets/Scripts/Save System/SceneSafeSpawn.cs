using UnityEngine;

public class SceneSafeSpawn : MonoBehaviour
{
    public static SceneSafeSpawn Instance { get; private set; }

    [Tooltip("World position where the player will be placed when no saved position matches this scene.")]
    [SerializeField] private Vector3 safePosition;

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetSafePosition() => safePosition;

    // Draw a green cross gizmo in the editor so you can see/move the spawn point easily.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(safePosition, 0.4f);
        Gizmos.DrawLine(safePosition + Vector3.left * 0.6f, safePosition + Vector3.right * 0.6f);
        Gizmos.DrawLine(safePosition + Vector3.down * 0.6f, safePosition + Vector3.up   * 0.6f);
    }
}

