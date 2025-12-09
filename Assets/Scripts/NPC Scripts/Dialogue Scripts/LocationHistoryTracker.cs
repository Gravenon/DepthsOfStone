using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocationHistoryTracker : MonoBehaviour
{
    private readonly HashSet<LocationSO> locationVisited = new HashSet<LocationSO>();

    public void RecordLocation(LocationSO locationSO)
    {
        if (locationVisited.Add(locationSO))
        {
            Debug.Log($"Just visited to: {locationSO.displayName}");
        }
    }

    public bool HasVisited(LocationSO locationSO)
    {
        return locationVisited.Contains(locationSO);
    }
}
