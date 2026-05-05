using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<QuestSO, Dictionary<QuestObejtive, int>> questProgress = new();
    private List<QuestSO> comletedQuests = new();

    private void OnEnable()
    {
        QuestEvents.IsQuestComplete += IsQuestComplete;
    }

    private void OnDisable()
    {
        QuestEvents.IsQuestComplete -= IsQuestComplete;
    }


    
    #region Quest Accept Logic
    public bool IsQuestAccepted(QuestSO questSO)
    {
        return questProgress.ContainsKey(questSO);
    }
    
    public List<QuestSO> GetActiveQuests()
    {
        return new List<QuestSO>(questProgress.Keys);
    }
    
    public void AcceptQuest(QuestSO quest)
    {
        if (!questProgress.ContainsKey(quest))
            questProgress[quest] = new Dictionary<QuestObejtive, int>();

        foreach (var objective in quest.objectives)
        {
            UpdateObjectiveProgress(quest, objective);
        }
    }
    #endregion
    
    #region Quest Complete Logic
    public bool IsQuestComplete(QuestSO quest)
    {
        if (!questProgress.TryGetValue(quest, out var progressDictionary))
            return false;

        foreach (var objective in quest.objectives)
        {
            UpdateObjectiveProgress(quest, objective);
        }

        foreach (var objective in quest.objectives)
        {
            if (progressDictionary[objective] < objective.requiredAmount)
                return false;
        }
        
        return true;
    }

    public void CompleteQuest(QuestSO quest)
    {
        questProgress.Remove(quest);
        comletedQuests.Add(quest);

        foreach (var objective in quest.objectives)
        {
            if (objective.targetItem != null && objective.requiredAmount > 0)
            {
                InventoryManager.Instance.RemoveItem(objective.targetItem, objective.requiredAmount);
            }
                
        }
        
        foreach (var reward in quest.rewards)
        {
            InventoryManager.Instance.AddItem(reward.itemSo, reward.quantity);
        }
    }

    public bool GetCompleteQuest(QuestSO quest)
    {
        return comletedQuests.Contains(quest);
    }
    
    #endregion
    
    public void UpdateObjectiveProgress(QuestSO quest, QuestObejtive objective)
    {
        if (!questProgress.ContainsKey(quest))
            return;
        
        var progressDictionary = questProgress[quest];
        int newAmout = 0;
        
        if(objective.targetItem != null)
            newAmout = InventoryManager.Instance.GetItemCount(objective.targetItem);
        else if (objective.targetLocation != null && GameManager.Instance.LocationHistoryTracker.HasVisited(objective.targetLocation))
                 newAmout = objective.requiredAmount; // Mark as completed if visited
        else if (objective.targetNPC != null && GameManager.Instance.DialogueHistoryTraker.HasSpokenWith(objective.targetNPC))
            newAmout = objective.requiredAmount; // Mark as completed if interacted
            
        progressDictionary[objective] = newAmout;
    }
    
    public string GetProgressText(QuestSO questSO, QuestObejtive objective)
    {
        int currentAmount = GteCurrentAmount(questSO, objective); 
        
        if (currentAmount >= objective.requiredAmount)
            return "Completed";
        else if (objective.targetItem != null)
            return $"{currentAmount}/{objective.requiredAmount} {objective.targetItem.itemName}";
        else
            return "In Progress";
    }

    public int GteCurrentAmount(QuestSO questSO, QuestObejtive objective)
    {
        if(questProgress.TryGetValue(questSO, out var progressDictionary))
            if(progressDictionary.TryGetValue(objective, out var currentAmount))
                return currentAmount;
        return 0;
    }
}
