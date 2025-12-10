using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NPC_Talk : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    public Animator interactAnim;

    public List<DialogueSO> converstations;
    public DialogueSO currentConversation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        anim.Play("Idle");
        interactAnim.Play("Open");
    }

    private void OnDisable()
    {
        interactAnim.Play("Close");
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            if (GameManager.Instance.DialogueManager.isDialogueActive)
                GameManager.Instance.DialogueManager.AdvancedDialogue();
            else
            {
                if (GameManager.Instance.DialogueManager.CanStartDialogue())
                {
                    CheckForNewConverstation();
                    GameManager.Instance.DialogueManager.StartDialogue(currentConversation);
                }
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
}
