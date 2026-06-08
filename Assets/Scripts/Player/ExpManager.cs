using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ExpManager : MonoBehaviour, IDataPersistence
{
    public int level;
    public int currentExp;
    public int expToNextLevel = 60;
    public float expGrowthMultiplier = 1.65f;

    public Slider expBar;
    public TMP_Text levelText;

    public static event Action<int> OnLevelUp;

    public void GainExp(int amount)
    {
        currentExp += amount;
        CheckLevelUp();
    }

    void OnEnable()
    {
        Enemy_Health.OnEnemyDefeated += GainExp;
        InventoryManager.OnExperienceGained += GainExp;
    }

    void OnDisable()
    {
        Enemy_Health.OnEnemyDefeated -= GainExp;
        InventoryManager.OnExperienceGained -= GainExp;
    }

    private void CheckLevelUp()
    {
        // loop handles multiple level-ups from a single large exp gain
        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            level++;
            expToNextLevel = Mathf.RoundToInt(expToNextLevel * expGrowthMultiplier);
            OnLevelUp?.Invoke(level);
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (expBar != null)
        {
            expBar.maxValue = expToNextLevel;
            expBar.value = currentExp;
        }
        if (levelText != null) levelText.text = level.ToString();
    }

    public void SaveData(ref GameData data)
    {
        data.playerLevel = level;
        data.playerCurrentExp = currentExp;
        data.playerExpToNextLevel = expToNextLevel;
    }

    public void LoadData(GameData data)
    {
        level          = data.playerLevel          > 0 ? data.playerLevel          : 1;
        currentExp     = data.playerCurrentExp;
        expToNextLevel = data.playerExpToNextLevel > 0 ? data.playerExpToNextLevel : 50;
        UpdateUI();
    }
}
