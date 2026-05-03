using UnityEngine;

public class QuestLogUI : MonoBehaviour
{
    public void HandleQuestClicked(QuestSO quest)
    {
        Debug.Log($"Quest clicked: {quest.name}" );

        foreach (var objective in quest.objectives)
        {
            Debug.Log($"Objective: {objective.description} ");
        }
    }
}
