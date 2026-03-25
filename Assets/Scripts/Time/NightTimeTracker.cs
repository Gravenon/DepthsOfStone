using System;
using UnityEngine;

public class NightTimeTracker : MonoBehaviour, IDataPersistence
{
    public static NightTimeTracker Instance { get; private set; }

    [Header("References")]
    [SerializeField] private WorldTime worldTime;

    // Runtime state (populated from GameData on load)
    private int countNight;
    private float nightTimeMinutes;
    private bool wasNight;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadData(GameData data)
    {
        ApplyGameData(data);
    }

    public void SaveData(ref GameData data)
    {
        PopulateGameData(data);
    }

    private void OnEnable()
    {
        if (worldTime == null)
            worldTime = WorldTime.Instance;

        if (worldTime != null)
            worldTime.WorldTimeChange += OnWorldTimeChange;
    }

    private void OnDisable()
    {
        if (worldTime != null)
            worldTime.WorldTimeChange -= OnWorldTimeChange;
    }

    private void Start()
    {
        if (worldTime == null)
            worldTime = WorldTime.Instance ?? FindFirstObjectByType<WorldTime>();

        if (worldTime != null)
        {
            worldTime.WorldTimeChange -= OnWorldTimeChange;
            worldTime.WorldTimeChange += OnWorldTimeChange;

            // Seed initial night state without counting a new night
            wasNight = IsNight(worldTime.CurrentTime);
        }
    }

    // ---------------------------------------------------------------
    // Public API
    // ---------------------------------------------------------------

    public int CountNight => countNight;
    public float NightTimeMinutes => nightTimeMinutes;

    public void PopulateGameData(GameData data)
    {
        data.countNight = countNight;
        data.nightTimeMinutes = nightTimeMinutes;
    }

    public void ApplyGameData(GameData data)
    {
        countNight = data.countNight;
        nightTimeMinutes = data.nightTimeMinutes;
    }


    private void OnWorldTimeChange(object sender, TimeSpan newTime)
    {
        bool isNightNow = IsNight(newTime);

        if (isNightNow)
            nightTimeMinutes += 1f; // WorldTime fires once per in-game minute

        // Crossing into night counts as a new night
        if (isNightNow && !wasNight)
            countNight++;

        wasNight = isNightNow;
    }

    private static bool IsNight(TimeSpan time)
    {
        int hour = time.Hours;
        // Night wraps around midnight: NightStartHour..23 or 0..NightEndHour-1
        return hour >= WorldTimeConstants.NightStartHour || hour < WorldTimeConstants.NightEndHour;
    }
}
