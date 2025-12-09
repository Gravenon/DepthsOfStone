using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSO", menuName = "Dialogue/DialogueNode")]
public class DialogueSO : ScriptableObject
{
    public DialogueNode[] lines;
    public DialogueOption[] options;
    
    [Header("Conditional Requiremnts (Optional)")]
    public AcotrSO[] requiredNPCs;
    public LocationSO[] requiredLocations;
    public ItemSO[] requiredItems;

    [Header("Control Flags")]
    public bool removeAfterPlay;
    public List<DialogueSO> removeTheseOnPlay;

    public bool IsConditionsMet()
    {
        if(requiredNPCs.Length > 0)
        {
            foreach (var npc in requiredNPCs)
            {
                if(!GameManager.Instance.DialogueHistoryTraker.HasSpokenWith(npc)) 
                    return false;
            }
        }
        if(requiredLocations.Length > 0)
        {
            foreach (var location in requiredLocations)
            {
                if (!GameManager.Instance.LocationHistoryTracker.HasVisited(location)) 
                    return false;
            }
        }

        if(requiredItems.Length > 0)
        {
            foreach (var item in requiredItems)
            {
                if(!InventoryManager.Instance.HasItem(item))
                    return false;
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
}