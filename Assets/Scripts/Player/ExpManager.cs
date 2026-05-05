using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ExpManager : MonoBehaviour
{
    public int level;
    public int currentExp;
    public int expToNextLevel = 10;
    public float expGrowthMultiplier = 1.5f;

    public Slider expBar; // Reference to the UI Slider for the experience bar
    public TMP_Text levelText; // Reference to the UI Text for displaying the current level

    public static event Action<int> OnLevelUp; // Event to notify when the player levels up
    
    public void Start()
    {
        UpdateUI(); // Initialize the UI at the start
    }

    public void GainExp(int amount)
    {
        currentExp += amount;
        CheckLevelUp();
    }

    void OnEnable()
    {
        Enemy_Health.OnEnemyDefeated += GainExp; // Subscribe to the event when an enemy is defeated
        InventoryManager.OnExperienceGained += GainExp; // Subscribe to the event when experience is gained from other sources (e.g., quests, items)
    }

    void OnDisable()
    {
        Enemy_Health.OnEnemyDefeated -= GainExp; // Unsubscribe from the event when the object is disabled
        InventoryManager.OnExperienceGained -= GainExp;
    }


    private void CheckLevelUp()
    {
        if (currentExp >= expToNextLevel)
        {
            LevelUp(); 
        }
        UpdateUI(); // Update the UI after gaining experience and potentially leveling up
    }

    private void LevelUp()
    {
        currentExp -= expToNextLevel;
        level++;
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * expGrowthMultiplier); // Increase exp needed for next level
        OnLevelUp?.Invoke(level); 

    }

    private void UpdateUI()
    {
        expBar.maxValue = expToNextLevel; // Set the max value of the experience bar to the required experience for the next level
        expBar.value = currentExp; // Update the experience bar
        levelText.text = level.ToString(); // Update the level text
    }
}
