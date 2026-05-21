using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour, IDataPersistence
{
    public SkillSlot[] skillSlots;
    
    [Header("Combat Points")]
    public TMP_Text combatPointsText;
    public int availableCombatPoints;
    
    [Header("Magic Points")]
    public TMP_Text magicPointsText;
    public int availableMagicPoints;

    private void OnEnable()
    {
        SkillSlot.OnAbilityPointSpent += HandleAbilityPointSpent;
        SkillSlot.OnSkillMaxed += HandleSkillMaxed;
        ExpManager.OnLevelUp += HandleLevelUp;
    }

    private void OnDisable()
    {
        SkillSlot.OnAbilityPointSpent -= HandleAbilityPointSpent;
        SkillSlot.OnSkillMaxed -= HandleSkillMaxed;
        ExpManager.OnLevelUp -= HandleLevelUp;
    }

    private void Start()
    {
        foreach (var slot in skillSlots)
        {
            slot.upgradeButton.onClick.AddListener(() => CheckAvailablePoints(slot));
        }
        UpdateAllPointsUI();
    }

    private void CheckAvailablePoints(SkillSlot slot)
    {
        SkillType skillType = slot.skillSO.skillType;
        
        if (skillType == SkillType.Combat && availableCombatPoints > 0)
        {
            slot.TryUpgradeSkill();
        }
        else if (skillType == SkillType.Magic && availableMagicPoints > 0)
        {
            slot.TryUpgradeSkill();
        }
    }

    private void HandleAbilityPointSpent(SkillSlot slot, SkillType skillType)
    {
        if (skillType == SkillType.Combat && availableCombatPoints > 0)
        {
            availableCombatPoints--;
            UpdateCombatPointsUI();
        }
        else if (skillType == SkillType.Magic && availableMagicPoints > 0)
        {
            availableMagicPoints--;
            UpdateMagicPointsUI();
        }
    }

    private void HandleSkillMaxed(SkillSlot skillSlot)
    {
        foreach (SkillSlot slot in skillSlots)
        {
            if(slot == null) continue; 
            
            if(!slot.isUnlocked && slot.CanUnlockSkill())
                slot.Unlock();
        }
    }

    private void HandleLevelUp(int level)
    {
        availableCombatPoints += 2;
        
        if (level % 5 == 0)
        {
            availableMagicPoints++;
        }
        
        UpdateAllPointsUI();
    }

    private void UpdateCombatPointsUI()
    {
        if (combatPointsText != null)
            combatPointsText.text = $"Combat Points: {availableCombatPoints}";
    }

    private void UpdateMagicPointsUI()
    {
        if (magicPointsText != null)
            magicPointsText.text = $"Magic Points: {availableMagicPoints}";
    }

    private void UpdateAllPointsUI()
    {
        UpdateCombatPointsUI();
        UpdateMagicPointsUI();
    }

    // ─── IDataPersistence ───────────────────────────────────────────────────

    public void SaveData(ref GameData data)
    {
        data.savedCombatPoints = availableCombatPoints;
        data.savedMagicPoints  = availableMagicPoints;

        var states = new List<SerializedSkillState>();
        foreach (var slot in skillSlots)
        {
            if (slot == null || slot.skillSO == null) continue;
            states.Add(new SerializedSkillState
            {
                skillName = slot.skillSO.skillName,
                currentLevel = slot.currentLevel,
                isUnlocked = slot.isUnlocked
            });
        }
        data.skillStates = states.ToArray();
    }

    public void LoadData(GameData data)
    {
        if (data.skillStates == null || data.skillStates.Length == 0) return;

        availableCombatPoints = data.savedCombatPoints;
        availableMagicPoints  = data.savedMagicPoints;

        var lookup = data.skillStates.ToDictionary(s => s.skillName);

        foreach (var slot in skillSlots)
        {
            if (slot == null || slot.skillSO == null) continue;
            if (!lookup.TryGetValue(slot.skillSO.skillName, out var saved)) continue;

            slot.isUnlocked   = saved.isUnlocked;
            slot.currentLevel = saved.currentLevel;
            slot.RefreshUI(); // update visuals without firing events
        }

        UpdateAllPointsUI();
    }
}
