using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class QuestLogUI : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;

    [SerializeField] private TMP_Text questName;
    [SerializeField] private TMP_Text questDescription;
    [SerializeField] private QuestObjectivSlot[] objectiveSlots;
    [SerializeField] private QuestRewardSlot[] rewardSlots;

    [SerializeField] private CanvasGroup questCanvas;
    
    [SerializeField] private CanvasGroup acceptCanvasGroup;
    [SerializeField] private CanvasGroup declineCanvasGroup;
    [SerializeField] private CanvasGroup completeCanvasGroup;
    
    [SerializeField] private QuestSO NoAvailableQuestSO;
    [SerializeField] private QuestSlot[] questSlots;

    [Header("Input")]
    [SerializeField] private InputActionReference questLogAction;
    
    private QuestSO questSO;
    private bool _isOpen;

    private void Start()
    {
        SetCanvasState(questCanvas, false);
    }

    private void OnEnable()
    {
        QuestEvents.OnQuestOfferRequested += ShowQuestOffer;
        QuestEvents.OnQuestTurnInRequested += ShowQuestTurnIn;
        QuestEvents.OnQuestBoardExit += CloseQuestOffer;
        QuestEvents.OnQuestLogToggle += ToggleQuestLog;

        if (questLogAction != null)
            questLogAction.action.performed += OnQuestLogInput;
    }

    private void OnDisable()
    {
        QuestEvents.OnQuestOfferRequested -= ShowQuestOffer;
        QuestEvents.OnQuestTurnInRequested -= ShowQuestTurnIn;
        QuestEvents.OnQuestBoardExit -= CloseQuestOffer;
        QuestEvents.OnQuestLogToggle -= ToggleQuestLog;

        if (questLogAction != null)
            questLogAction.action.performed -= OnQuestLogInput;
    }

    private void OnQuestLogInput(InputAction.CallbackContext context)
    {
        ToggleQuestLog();
    }

    public void ToggleQuestLog()
    {
        if (_isOpen)
        {
            CloseQuestOffer();
        }
        else
        {
            OpenQuestLog();
        }
    }

    // Открыть журнал в режиме просмотра (без кнопок принятия/отказа/завершения)
    private void OpenQuestLog()
    {
        RefreshQuestList();

        SetCanvasState(acceptCanvasGroup, false);
        SetCanvasState(declineCanvasGroup, false);
        SetCanvasState(completeCanvasGroup, false);

        // Показать первый активный квест, если есть
        var activeQuests = questManager.GetActiveQuests();
        if (activeQuests.Count > 0)
            HandleQuestClicked(activeQuests[0]);
        else if (NoAvailableQuestSO != null)
            HandleQuestClicked(NoAvailableQuestSO);

        SetCanvasState(questCanvas, true);
        _isOpen = true;
    }

    #region Show Quest Method
    
    // Called via QuestEvents when player interacts with QuestBoard
    public void ShowQuestOffer(QuestSO incomingQuestSO)
    {
        // Press E again while open — close (toggle)
        if (_isOpen && questSO == incomingQuestSO)
        {
            CloseQuestOffer();
            return;
        }

        if (questManager.IsQuestAccepted(incomingQuestSO) || questManager.GetCompleteQuest(incomingQuestSO))
        {
            questSO = NoAvailableQuestSO;
            SetCanvasState(acceptCanvasGroup, false);
            SetCanvasState(declineCanvasGroup, true);
            SetCanvasState(completeCanvasGroup, false);
        }
        else
        {
            questSO = incomingQuestSO;
            SetCanvasState(acceptCanvasGroup, true);
            SetCanvasState(declineCanvasGroup, true);
            SetCanvasState(completeCanvasGroup, false);
        }

        HandleQuestClicked(questSO);
        SetCanvasState(questCanvas, true);
        _isOpen = true;
    }

    public void ShowQuestTurnIn(QuestSO incomingQuestSO)
    {
        questSO = incomingQuestSO;
        
        HandleQuestClicked(questSO);
        
        SetCanvasState(completeCanvasGroup, true);
        SetCanvasState(declineCanvasGroup, false);
        SetCanvasState(acceptCanvasGroup, false);
        SetCanvasState(questCanvas, true);
    }
    
    #endregion
    
    #region On Button Click Method
    public void OnAcceptQuestClicked()
    {
        QuestEvents.OnQuestAccepted?.Invoke(questSO);
        
        questManager.AcceptQuest(questSO);
        SetCanvasState(completeCanvasGroup, false);
        SetCanvasState(acceptCanvasGroup, false);

        RefreshQuestList();
        HandleQuestClicked(NoAvailableQuestSO);
    }

    public void OnDeclineQuestClicked()
    {
        CloseQuestOffer();
    }

    public void OnCompleteQuestClicked()
    {
        questManager.CompleteQuest(questSO);
        RefreshQuestList();
        CloseQuestOffer();
    }
    
    #endregion

    private void CloseQuestOffer()
    {
        _isOpen = false;
        SetCanvasState(questCanvas, false);
    }

    public void HandleQuestClicked(QuestSO quest)
    {
        questSO = quest;

        questName.text = quest.questName;
        questDescription.text = quest.questDescription;

        DisplayObjectives();
        DisplayRewards();
    }
    
    private void SetCanvasState(CanvasGroup canvasGroup, bool visible)
    {
        if (canvasGroup == null) return;
        
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }

    public void RefreshQuestList()
    {
        List<QuestSO> activeQuest = questManager.GetActiveQuests();

        for (int i = 0; i < questSlots.Length; i++)
        {
            if (i < activeQuest.Count)
            {
                questSlots[i].SetQuest(activeQuest[i]);
            }
            else
            {
                questSlots[i].ClearSlot();
            }
        }
    }

    private void DisplayObjectives()
    {
        for (int i = 0; i < objectiveSlots.Length; i++)
        {
            if (i < questSO.objectives.Count)
            {
                var objective = questSO.objectives[i];
                questManager.UpdateObjectiveProgress(questSO, objective);

                int currentAmount = questManager.GteCurrentAmount(questSO, objective);
                string progress   = questManager.GetProgressText(questSO, objective);
                bool isComplete   = currentAmount >= objective.requiredAmount;

                objectiveSlots[i].gameObject.SetActive(true);
                objectiveSlots[i].RefreshObjective(objective.description, progress, isComplete);
            }
            else
            {
                objectiveSlots[i].gameObject.SetActive(false);
            }
        }
    }
    
    private void DisplayRewards()
    {
        for (int i = 0; i < rewardSlots.Length; i++)
        {
            if (i < questSO.rewards.Count)
            {
                var reward = questSO.rewards[i];
                rewardSlots[i].DisplayReward(reward.itemSo.itemIcon, reward.quantity);
                rewardSlots[i].gameObject.SetActive(true);
            }
            else
            {
                rewardSlots[i].gameObject.SetActive(false);
            }
        }
    }
}
