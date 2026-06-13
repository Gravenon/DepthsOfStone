using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    public List<SkillSlot> prerequisites; 

    public SkillSO skillSO;

    public int currentLevel;
    public bool isUnlocked;

    public Image skillIcon;
    public Button upgradeButton;
    public TMP_Text skillLevelText;

    public Sprite ActiveSkillIcon;
    public Sprite MaxLvlIcon;

    public GameObject[] runesIcons;


    public static event Action<SkillSlot, SkillType> OnAbilityPointSpent;
    public static event Action<SkillSlot> OnSkillMaxed;

    private void OnValidate()
    {
        if (skillLevelText != null && skillSO != null)
        {
            UpdateUI();
        }
    }

    public void TryUpgradeSkill()
    {
        if (isUnlocked && currentLevel < skillSO.maxLevel)
        {
            currentLevel++;
            OnAbilityPointSpent?.Invoke(this, skillSO.skillType);

            if(currentLevel >= skillSO.maxLevel)
            {
                OnSkillMaxed?.Invoke(this);
            }

            UpdateUI();
        }
    }

    public bool CanUnlockSkill()
    {
        foreach (SkillSlot slot in prerequisites)
        {
            if (slot == null || slot.skillSO == null)
            {
                Debug.LogWarning($"Skill '{skillSO?.name}' has a null prerequisite or prerequisite with null skillSO", this);
                continue;
            }
            
            if (!slot.isUnlocked || slot.currentLevel < slot.skillSO.maxLevel)
            {
                return false; 
            }
        }
        return true; 
    }


    public void Unlock()
    {
        isUnlocked = true;
        UpdateUI();
    }

    public void RefreshUI() => UpdateUI();

    private void SetButtonSprite(Sprite sprite)
    {
        var rt = upgradeButton.image.rectTransform;
        Vector2 originalSize = rt.sizeDelta;
        upgradeButton.image.sprite = sprite;
        rt.sizeDelta = originalSize;
    }

    private void UpdateUI()
    {
        skillIcon.sprite = skillSO.skillIcon;

        if(isUnlocked)
        {
            upgradeButton.interactable = true;
            skillLevelText.text = $"{currentLevel}/{skillSO.maxLevel}";
            skillIcon.color = Color.white;

            if (currentLevel >= skillSO.maxLevel)
            {
                SetButtonSprite(MaxLvlIcon);
                for (int i = 0; i < runesIcons.Length; i++)
                {
                    runesIcons[i].SetActive(true);
                }
            }
            else
                SetButtonSprite(ActiveSkillIcon);
        }
        else
        {
            skillLevelText.text = "Locked";
            skillIcon.color = Color.gray;
        }
    }
}
