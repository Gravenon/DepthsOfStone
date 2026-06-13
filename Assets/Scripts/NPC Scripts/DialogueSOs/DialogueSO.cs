using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSO", menuName = "Dialogue/DialogueNode")]
public class DialogueSO : ScriptableObject
{
    public DialogueNode[] lines;
    public DialogueOption[] options;
    
    [Header("Quest Offer(Optional)")]
    public QuestSO offerQuestOnEnd;
    
    [Header("Completed Quest Requirements(Optional)")]
    public QuestSO[] requiredCompletedQuests;
    
    [Header("Quest Turn-In (Optional)")]
    public QuestSO turnInQuestOnEnd;
    
    [Header("Conditional Requiremnts (Optional)")]
    public AcotrSO[] requiredNPCs;
    public LocationSO[] requiredLocations;
    public ItemSO[] requiredItems;

    [Header("Control Flags")]
    public bool removeAfterPlay;
    public List<DialogueSO> removeTheseOnPlay;

    public bool IsConditionsMet()
    {
        if(requiredNPCs != null && requiredNPCs.Length > 0)
        {
            foreach (var npc in requiredNPCs)
            {
                if (npc == null) continue;
                if(!GameManager.Instance.DialogueHistoryTraker.HasSpokenWith(npc)) 
                    return false;
            }
        }
        if(requiredLocations != null && requiredLocations.Length > 0)
        {
            foreach (var location in requiredLocations)
            {
                if (location == null) continue;
                if (!GameManager.Instance.LocationHistoryTracker.HasVisited(location)) 
                    return false;
            }
        }

        if(requiredItems != null && requiredItems.Length > 0)
        {
            foreach (var item in requiredItems)
            {
                if (item == null) continue;
                if(!InventoryManager.Instance.HasItem(item))
                    return false;
            }
        }
        
        if(requiredCompletedQuests != null && requiredCompletedQuests.Length > 0)
        {
            var questManager = GameManager.Instance?.QuestManager;

            foreach (var quest in requiredCompletedQuests)
            {
                if (quest == null) continue;
                if (!questManager.IsQuestComplete(quest))
                {
                    return false;
                }
            }
        }
        return true;
    }
}

[System.Serializable]
public class DialogueNode
{
    public AcotrSO speaker;
    [TextArea(3,5)] public string dialogueText;
}


[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public DialogueSO nextDialogue;
    public QuestSO offerQuest;
}