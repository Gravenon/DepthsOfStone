using UnityEngine;

// Blocks passage until the required quest is turned in.
// Attach to any GameObject with a Collider2D.
public class QuestColliderGate : MonoBehaviour
{
    [SerializeField] private QuestSO requiredQuest;

    private Collider2D _col;

    private void Awake() => _col = GetComponent<Collider2D>();

    private void Start()
    {
        QuestEvents.OnQuestTurnInRequested += OnQuestTurnedIn;

        // Already completed on load (returning player) — open immediately
        if (GameManager.Instance?.QuestManager.IsQuestComplete(requiredQuest) == true)
            _col.enabled = false;
    }

    private void OnDestroy() => QuestEvents.OnQuestTurnInRequested -= OnQuestTurnedIn;

    private void OnQuestTurnedIn(QuestSO quest)
    {
        if (quest == requiredQuest)
            _col.enabled = false;
    }
}

