using UnityEngine;

/// <summary>
/// Library of all QuestSO assets in the scene.
/// Add this component to any GameObject in the gameplay scene
/// and assign all QuestSO assets to the questSOs array in the Inspector.
/// </summary>
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
