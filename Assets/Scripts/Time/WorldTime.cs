using System;
using System.Collections;
using UnityEngine;

public class WorldTime : MonoBehaviour, IDataPersistence
{
    public static WorldTime Instance { get; private set; }

    public event EventHandler<TimeSpan> WorldTimeChange;

    [SerializeField]
    private float dayLength; // Length of a full day in seconds

    private TimeSpan currentTime = new TimeSpan(6, 0, 0);
    public TimeSpan CurrentTime => currentTime;
    private float minuteLength => dayLength / WorldTimeConstants.MinutesInDay;
    private Coroutine minuteCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (transform.root != null)
            DontDestroyOnLoad(transform.root.gameObject);
        
        else
            DontDestroyOnLoad(gameObject);
        
    }

    private void Start()
    {
        if (dayLength <= 0f)
        {
            dayLength = 60f;
        }

        if (minuteCoroutine == null)
        {
            minuteCoroutine = StartCoroutine(AddMinute());
        }
    }

    private IEnumerator AddMinute()
    {
        while (true)
        {
            currentTime += TimeSpan.FromMinutes(1);
            WorldTimeChange?.Invoke(this, currentTime);
            yield return new WaitForSeconds(minuteLength);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void ResetToMorning()
    {
        currentTime = new TimeSpan(6, 0, 0);
        WorldTimeChange?.Invoke(this, currentTime);
    }

    public void LoadData(GameData data)
    {
        currentTime = TimeSpan.FromMinutes(data.worldTimeMinutes);
        WorldTimeChange?.Invoke(this, currentTime);
    }

    public void SaveData(ref GameData data)
    {
        data.worldTimeMinutes = (int)currentTime.TotalMinutes;
    }
}
