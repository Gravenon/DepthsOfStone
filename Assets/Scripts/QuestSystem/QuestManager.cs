using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<QuestSO, Dictionary<QuestObejtive, int>> questProgress = new();


    public void UpdateObjectiveProgress(QuestSO quest, QuestObejtive objective)
    {
        if (!questProgress.ContainsKey(quest))
            questProgress[quest] = new Dictionary<QuestObejtive, int>();
        
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
