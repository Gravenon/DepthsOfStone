using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    public GameObject[] statsSlots;

    private bool _initialized;

    private void Start()
    {
        _initialized = true;
        UpdataAllStats();
    }

    private void OnEnable()
    {
        // OnEnable fires before Start on first enable — skip until initialized
        if (_initialized) UpdataAllStats();
    }

    public void UpdateDameg()
    {
        if (StatsManager.Instance == null || statsSlots.Length < 1) return;
        statsSlots[0].GetComponentInChildren<TMP_Text>().text = "Damage: " + (int)StatsManager.Instance.damage;
    }

    public void UpdateSpeed()
    {
        if (StatsManager.Instance == null || statsSlots.Length < 2) return;
        statsSlots[1].GetComponentInChildren<TMP_Text>().text = "Speed: " + StatsManager.Instance.speed;
    }

    public void UpdataAllStats()
    {
        UpdateDameg();
        UpdateSpeed();
    }
}
