using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup canvasGroup;
    public Image portrait;
    public TMP_Text actorName;
    public TMP_Text dialogueText;
    public Button[] optionButtons;

    public bool isDialogueActive;

    private DialogueSO currentDialogue;
    private int dialogueIndex;

    private float lastDialogueEndTime;
    private float dialogueCooldown = .1f;


    private void Awake()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        foreach (var buttons in optionButtons)
            buttons.gameObject.SetActive(false);
    }

    public bool CanStartDialogue()
    {
        return Time.unscaledTime - lastDialogueEndTime >= dialogueCooldown;
    }


    public void StartDialogue(DialogueSO dialogueSO)
    {
        currentDialogue = dialogueSO;
        dialogueIndex = 0;
        isDialogueActive = true;
        ShowDialogue();
    }


    public void AdvancedDialogue()
    {
        if (dialogueIndex < currentDialogue.lines.Length)
        {
            ShowDialogue();
        }
        else if (currentDialogue.options != null && currentDialogue.options.Length > 0)
        {
            ShowChoices();
        }
        else
        {
            EndDialogue();
        }
    }

    private void ShowDialogue()
    {
        DialogueNode line = currentDialogue.lines[dialogueIndex];

        GameManager.Instance.DialogueHistoryTraker.RecordNPC(line.speaker);

        portrait.sprite = line.speaker.portrait;
        actorName.text = line.speaker.actorName;

        dialogueText.text = line.dialogueText;

        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        dialogueIndex++;
    }


    private void ShowChoices()
    {
        ClearOption();

        if (currentDialogue.options.Length > 0)
        {
            for (int i = 0; i < currentDialogue.options.Length; i++)
            {
                var option = currentDialogue.options[i];

                optionButtons[i].GetComponentInChildren<TMP_Text>().text = option.optionText;
                optionButtons[i].gameObject.SetActive(true);

                optionButtons[i].onClick.AddListener(() => ChoiseOption(option.nextDialogue));
            }
        }
        else
        {
            optionButtons[0].GetComponentInChildren<TMP_Text>().text = "End the dialogue";
            optionButtons[0].onClick.AddListener(EndDialogue);
            optionButtons[0].gameObject.SetActive(true);
        }

        EventSystem.current.SetSelectedGameObject(optionButtons[0].gameObject);
    }


    private void ChoiseOption(DialogueSO dialogueSO)
    {
        if (dialogueSO == null)
        {
            EndDialogue();
        }
        else
        {
            ClearOption();
            StartDialogue(dialogueSO);
        }
    }



    private void EndDialogue()
    {
        dialogueIndex = 0;
        isDialogueActive = false;
        ClearOption();

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        lastDialogueEndTime = Time.unscaledTime;
    }

    private void ClearOption()
    {
        foreach (var button in optionButtons)
        {
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
        }
    }

}
