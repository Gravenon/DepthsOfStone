using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPC_Talk : MonoBehaviour, IDataPersistence
{
    private Rigidbody2D rb;
    private Animator anim;
    public Animator interactAnim;
    public InputActionReference interactAction;

    [SerializeField] private string npcId;

    public List<DialogueSO> converstations;
    public DialogueSO currentConversation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()  => QuestEvents.OnQuestAccepted += OnQuestAccepted_RemoveOfferings;
    private void OnDestroy() => QuestEvents.OnQuestAccepted -= OnQuestAccepted_RemoveOfferings;

    private void OnEnable()
    {
        if (interactAction != null)
            interactAction.action.performed += OnInteract;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        anim.Play("Idle");
        interactAnim.Play("Open");
    }

    private void OnDisable()
    {
        if (interactAction != null)
            interactAction.action.performed -= OnInteract;

        interactAnim.Play("Close");
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.DialogueManager.isDialogueActive)
        {
            GameManager.Instance.DialogueManager.AdvancedDialogue();
            return;
        }

        if (!GameManager.Instance.DialogueManager.CanStartDialogue()) return;

        CheckForNewConverstation();
        if (currentConversation != null)
            GameManager.Instance.DialogueManager.StartDialogue(currentConversation);
    }

    private void CheckForNewConverstation()
    {
        currentConversation = null;

        for (int i = 0; i < converstations.Count; i++)
        {
            var convo = converstations[i];
            if (convo == null || !convo.IsConditionsMet()) continue;

            currentConversation = convo;

            if (convo.removeAfterPlay)
                converstations.RemoveAt(i);

            if (convo.removeTheseOnPlay != null)
                foreach (var toRemove in convo.removeTheseOnPlay)
                    converstations.Remove(toRemove);

            break;
        }
    }

    private void OnQuestAccepted_RemoveOfferings(QuestSO acceptedQuest)
    {
        for (int i = converstations.Count - 1; i >= 0; i--)
        {
            if (converstations[i] != null && converstations[i].offerQuestOnEnd == acceptedQuest)
                converstations.RemoveAt(i);
        }
    }


    public void SaveData(ref GameData data)
    {
        if (string.IsNullOrEmpty(npcId)) return;

        // Collect names of DialogueSO still in the list
        string[] remaining = converstations.Where(c => c != null).Select(c => c.name).ToArray();

        // Replace or add entry for this NPC
        var list = data.npcConversationStates?.ToList() ?? new List<SerializedNPCConversationState>();
        var entry = list.FirstOrDefault(e => e.npcId == npcId);
        if (entry == null)
        {
            entry = new SerializedNPCConversationState { npcId = npcId };
            list.Add(entry);
        }
        entry.remainingConversations = remaining;
        data.npcConversationStates = list.ToArray();
    }

    public void LoadData(GameData data)
    {
        if (string.IsNullOrEmpty(npcId)) return;
        if (data.npcConversationStates == null) return;

        var entry = data.npcConversationStates.FirstOrDefault(e => e.npcId == npcId);
        if (entry == null) return; // never saved — keep full list

        var remainingSet = new HashSet<string>(entry.remainingConversations ?? System.Array.Empty<string>());

        // Remove dialogues that are no longer in the saved remaining set
        converstations.RemoveAll(c => c != null && !remainingSet.Contains(c.name));
    }
}
