using TMPro;
using UnityEngine;

public class QuestSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private TMP_Text questLevelText;
    
    public QuestSO currentQuest;
    
    public QuestLogUI questLogUI;

    private void OnValidate()
    {
        if(currentQuest != null)
            SetQuest(currentQuest);
        else
            gameObject.SetActive(false);
        
    }


    public void SetQuest(QuestSO quest)
    {
        currentQuest = quest;
        
        questNameText.text = quest.questName;
        questLevelText.text = "Lv." + quest.questLevel;
        
        gameObject.SetActive(true);
    }

    public void ClearSlot()
    {
        currentQuest = null;
        gameObject.SetActive(false);
    }

    public void OnSlotClicked()
    {
        questLogUI.HandleQuestClicked(currentQuest);
    }
    
}
