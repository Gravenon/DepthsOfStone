using UnityEngine;
using TMPro;

public class QuestLogUI : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private TMP_Text questName;
    [SerializeField] private TMP_Text questDescription;
    [SerializeField] private QuestObjectivSlot[] objectiveSlots;
    [SerializeField] private QuestRewardSlot[] rewardSlots;

    [SerializeField] private CanvasGroup questDetailCanvasGroup;

    private QuestSO _currentQuest;

    private void Start()
    {
        SetDetailVisible(false);
    }

    public void HandleQuestClicked(QuestSO quest)
    {
        _currentQuest = quest;

        questName.text = quest.questName;
        questDescription.text = quest.questDescription;

        SetDetailVisible(true);
        DisplayObjectives();
        DisplayRewards();
    }

    public void CloseDetail()
    {
        _currentQuest = null;
        SetDetailVisible(false);
    }

    private void SetDetailVisible(bool visible)
    {
        if (questDetailCanvasGroup == null)
        {
            Debug.LogWarning("[QuestLogUI] questDetailCanvasGroup is not assigned!");
            return;
        }

        questDetailCanvasGroup.alpha = visible ? 1f : 0f;
        questDetailCanvasGroup.interactable = visible;
        questDetailCanvasGroup.blocksRaycasts = visible;
    }

    private void DisplayObjectives()
    {
        for (int i = 0; i < objectiveSlots.Length; i++)
        {
            if (i < _currentQuest.objectives.Count)
            {
                var objective = _currentQuest.objectives[i];
                questManager.UpdateObjectiveProgress(_currentQuest, objective);

                int currentAmount = questManager.GteCurrentAmount(_currentQuest, objective);
                string progress = questManager.GetProgressText(_currentQuest, objective);
                bool isComplete = currentAmount >= objective.requiredAmount;

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
            if (i < _currentQuest.rewards.Count)
            {
                var reward = _currentQuest.rewards[i];
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
