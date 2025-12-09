using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHistoryTraker : MonoBehaviour
{
    private readonly HashSet<AcotrSO> spokenNPCs = new HashSet<AcotrSO>();

    public void RecordNPC(AcotrSO acotrSO)
    {
        spokenNPCs.Add(acotrSO);

        Debug.Log($"Just spoke to {acotrSO.actorName}");
    }

    public bool HasSpokenWith(AcotrSO acotrSO)
    {
        return spokenNPCs.Contains(acotrSO);
    }
}
