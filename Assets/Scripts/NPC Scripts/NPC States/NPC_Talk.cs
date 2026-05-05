using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPC_Talk : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    public Animator interactAnim;
    public InputActionReference interactAction;

    public List<DialogueSO> converstations;
    public DialogueSO currentConversation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        QuestEvents.OnQuestAccepted += OnQuestAccepted_RemoveOfferings;
    }

    private void OnDestroy()
    {
        QuestEvents.OnQuestAccepted -= OnQuestAccepted_RemoveOfferings;
    }

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed += OnInteract;
        }

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        anim.Play("Idle");
        interactAnim.Play("Open");
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteract;
        }

        interactAnim.Play("Close");
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.DialogueManager.isDialogueActive)
        {
            GameManager.Instance.DialogueManager.AdvancedDialogue();
        }
        else
        {
            if (GameManager.Instance.DialogueManager.CanStartDialogue())
            {
                CheckForNewConverstation();
                if (currentConversation != null)
                    GameManager.Instance.DialogueManager.StartDialogue(currentConversation);
            }
        }
    }

    private void CheckForNewConverstation()
    {
        for (int i = 0; i < converstations.Count; i++)
        {
            var convo = converstations[i];
            if (convo != null && convo.IsConditionsMet())
            {
                currentConversation = convo;

                //remove this if it's one-time only
                if (convo.removeAfterPlay)
                    converstations.RemoveAt(i);

                //remove any other dialogues that should be cleared when this one plays (like quest completion)
                if (convo.removeTheseOnPlay != null && convo.removeTheseOnPlay.Count > 0)
                {
                    foreach (var toRemove in convo.removeTheseOnPlay)
                    {
                        converstations.Remove(toRemove);
                    }
                }
                break;
            }
        }
    }

    private void OnQuestAccepted_RemoveOfferings(QuestSO acceptedQuest)
    {
        for (int i = converstations.Count - 1; i >= 0; i--)
        {
            var convo = converstations[i];
            if (convo == null)
                continue;
            
            if(convo.offerQuestOnEnd == acceptedQuest)
                converstations.RemoveAt(i);
        }
    }
}
