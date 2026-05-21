using System;
using UnityEngine;

[Serializable]
public class SerializedSlot
{
    public string itemName;
    public int quantity;
}

[Serializable]
public class SerializedEquippedSlot
{
    public int itemTypeId;
    public string itemName;
}

[Serializable]
public class SerializedQuestProgress
{
    public string questName;
    public int[] objectiveProgress; // прогресс каждой цели по индексу
}

[Serializable]
public class SerializedCompletedQuest
{
    public string questName;
}

[Serializable]
public class SerializedNPCConversationState
{
    public string npcId;
    public string[] remainingConversations; // names of DialogueSO still in the list
}

[Serializable]
public class SerializedSkillState
{
    public string skillName;
    public int currentLevel;
    public bool isUnlocked;
}

[Serializable]
public class GameData
{
    public string saveTime;
    public string nameWorld;
    public string namePlayer;

    // Scene where the player last saved (used to restore position correctly)
    public string lastScene;

    public int countNight;
    public float nightTimeMinutes;
    public int worldTimeMinutes;

    // Player position (only restored if lastScene matches the loaded scene)
    public Vector3 playerPosition;

    // Last activated checkpoint (used on respawn)
    public string checkpointScene;
    public Vector3 checkpointPosition;
    public int checkpointHealth;

    // Player level & experience
    public int playerLevel;
    public int playerCurrentExp;
    public int playerExpToNextLevel;

    // Player stats
    public int maxHealth;
    public int currentHealth;
    public int speed;
    public float damage;
    public float weaponRange;
    public float knockbackForce;
    public float knockbackTimre;
    public float stunTime;

    // Inventory
    public int coins;
    public SerializedSlot[] inventoryItems;
    public SerializedSlot[] equipmentItems;
    public SerializedEquippedSlot[] equippedItems;

    // First-time flags
    public bool introPlayed;

    // Arena / wave progress
    public int arenaWaveIndex;

    // Quest progress
    public SerializedQuestProgress[] activeQuests;
    public SerializedCompletedQuest[] completedQuests;

    // Skill tree
    public int savedCombatPoints;
    public int savedMagicPoints;
    public SerializedSkillState[] skillStates;

    // NPC dialogue state
    public SerializedNPCConversationState[] npcConversationStates;

    public GameData(string nameWorld, string playerName)
    {
        this.nameWorld = nameWorld;
        this.namePlayer = playerName;
        countNight = 0;
        nightTimeMinutes = 0f;
        worldTimeMinutes = 6 * 60;
        saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        lastScene = "";
        playerPosition = Vector3.zero;
        coins = 0;
        inventoryItems = new SerializedSlot[0];
        equipmentItems = new SerializedSlot[0];
        equippedItems = new SerializedEquippedSlot[0];
        arenaWaveIndex = 0;
        introPlayed = false;
        playerLevel = 1;
        playerCurrentExp = 0;
        playerExpToNextLevel = 50;
        activeQuests = new SerializedQuestProgress[0];
        completedQuests = new SerializedCompletedQuest[0];
        savedCombatPoints = 0;
        savedMagicPoints = 0;
        skillStates = new SerializedSkillState[0];
        npcConversationStates = new SerializedNPCConversationState[0];

        if (StatsManager.Instance != null)
        {
            maxHealth = StatsManager.Instance.baseMaxHealth;
            currentHealth = StatsManager.Instance.baseCurrentHealth;
            speed = StatsManager.Instance.baseSpeed;
            damage = StatsManager.Instance.baseDamage;
            weaponRange = StatsManager.Instance.baseWeaponRange;
            knockbackForce = StatsManager.Instance.baseKnockbackForce;
            knockbackTimre = StatsManager.Instance.baseKnockbackTimre;
            stunTime = StatsManager.Instance.baseStunTime;
        }
    }
}
