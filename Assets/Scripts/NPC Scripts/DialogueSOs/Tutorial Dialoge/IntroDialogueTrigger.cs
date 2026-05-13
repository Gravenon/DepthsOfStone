using UnityEngine;

public class IntroDialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueSO introDialogue;

    public void TriggerIntroDialogue()
    {
        GameManager.Instance.DialogueManager.StartDialogue(introDialogue);
    }
}
