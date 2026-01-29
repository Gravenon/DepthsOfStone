using System;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

public class WorldTime : MonoBehaviour
{
    public event EventHandler<TimeSpan> WorldTimeChange;

    [SerializeField]
    private float dayLength; // Length of a full day in seconds

    private TimeSpan currentTime = new TimeSpan(6, 0, 0);
    public TimeSpan CurrentTime => currentTime;
    private float minuteLength => dayLength / WorldTimeConstants.MinutesInDay;

    private void Start()
    {
        StartCoroutine(AddMinute());
    }

    private IEnumerator AddMinute()
    {
        currentTime += TimeSpan.FromMinutes(1);
        WorldTimeChange?.Invoke(this, currentTime);
        yield return new WaitForSeconds(minuteLength);
        StartCoroutine(AddMinute());
    }
}
