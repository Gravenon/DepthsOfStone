using UnityEngine;

public class QuestSOLibrary : MonoBehaviour
{
    public QuestSO[] questSOs;

    public QuestSO FindByName(string questName)
    {
        foreach (var q in questSOs)
            if (q != null && q.questName == questName)
                return q;
        return null;
    }
}
