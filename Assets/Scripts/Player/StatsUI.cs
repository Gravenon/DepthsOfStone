using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    public GameObject[] statsSlots;

    private void Start()
    {
        UpdataAllStats();
    }

    private void OnEnable()
    {
        UpdataAllStats();
    }

    public void UpdateDameg()
    {
        statsSlots[0].GetComponentInChildren<TMP_Text>().text = "Damage: " + (int)StatsManager.Instance.damage; 
    }

    public void UpdateSpeed()
    {
        statsSlots[1].GetComponentInChildren<TMP_Text>().text = "Speed: " + StatsManager.Instance.speed;
    }

    public void UpdataAllStats()
    {
        UpdateDameg();
        UpdateSpeed();
    }
}
