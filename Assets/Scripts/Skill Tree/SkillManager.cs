using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class SkillManager : MonoBehaviour
{

    public PlayerHealth playerHealth;
    public PlayerMovment playerMovement;
    
    private bool regenerationActive = false;
    private bool lastChanceActive = false;
    private bool lastChanceBonusApplied = false;
    private bool berserkUnlocked = false;
    private bool berserkActivated = false;
    private bool stoneSkinActive = false;
    private float baseDamage;
    private float damageReduction = 0f;

    private void OnEnable()
    {
        SkillSlot.OnAbilityPointSpent += HandleAbilityPointSpent;
    }

    private void OnDisable()
    {
        SkillSlot.OnAbilityPointSpent -= HandleAbilityPointSpent;
    }

    private void Update()
    {
        if (lastChanceActive)
        {
            CheckLastChanceBonus();
        }
    }

    private void HandleAbilityPointSpent(SkillSlot slot, SkillType skillType)
    {
        string skillName = slot.skillSO.skillName;

        switch (skillName)
        {
            case "Max Health Boost":
                StatsManager.Instance.UpdateMaxHealth(5);
                break;
            case "Stone Skin":
                if (!stoneSkinActive)
                {
                    stoneSkinActive = true;
                    damageReduction += 0.05f;
                }
                break;
            case "Regeneration":
                if (!regenerationActive)
                {
                    regenerationActive = true;
                    StartCoroutine(RegenerationCoroutine());
                }
                break;
            case "Last chance":
                lastChanceActive = true;
                baseDamage = StatsManager.Instance.damage;
                break;
            case "A powerful blow":
                StatsManager.Instance.UpdateDamage(1.05f);
                break;
            case "Light on his feet":
                StatsManager.Instance.UpdateSpeed(1);
                break;
            case "Punching":
                StatsManager.Instance.UpdataKnockbackForce(1);
                break;
            case "Berserk":
                berserkUnlocked = true;
                break;
            case "Dash":
                if (playerMovement != null)
                    playerMovement.UnlockDash();
                break;
            default:
                Debug.LogWarning($"No implementation for skill: {skillName}");
                break;
        }
    }

    private IEnumerator RegenerationCoroutine()
    {
        while (regenerationActive)
        {
            yield return new WaitForSeconds(2f);
            
            if (StatsManager.Instance.currentHealth < StatsManager.Instance.maxHealth)
            {
                StatsManager.Instance.UpdateHealth(1);
            }
        }
    }

    private void CheckLastChanceBonus()
    {
        float healthPercentage = (float)StatsManager.Instance.currentHealth / StatsManager.Instance.maxHealth;
        
        if (healthPercentage <= 0.3f && !lastChanceBonusApplied)
        {
            baseDamage = StatsManager.Instance.damage;
            float bonusDamage = baseDamage * 0.2f; 
            StatsManager.Instance.damage = baseDamage + bonusDamage;
            lastChanceBonusApplied = true;
        }
        else if (healthPercentage > 0.3f && lastChanceBonusApplied)
        {
            StatsManager.Instance.damage = baseDamage;
            lastChanceBonusApplied = false;
        }

        if (lastChanceActive && berserkUnlocked && healthPercentage <= 0.1f && !berserkActivated)
        {
            berserkActivated = true;
            StartCoroutine(BerserkRageCoroutine());
        }
    }

    private IEnumerator BerserkRageCoroutine()
    {        
        float savedDamage = StatsManager.Instance.damage;
        int savedSpeed = StatsManager.Instance.speed;
        
        StatsManager.Instance.damage += 5;
        StatsManager.Instance.UpdateSpeed(3);
        
        if (playerHealth != null)
            playerHealth.SetInvulnerable(true);
        
        yield return new WaitForSeconds(3f);
        
        StatsManager.Instance.damage = savedDamage;
        StatsManager.Instance.UpdateSpeed(-3);
        
        if (playerHealth != null)
            playerHealth.SetInvulnerable(false);
        
        Debug.Log("Берсерк закончился. Бонусы сняты.");
        
        yield return new WaitForSeconds(5f);
        berserkActivated = false;
    }

    public float ApplyDamageReduction(float incomingDamage)
    {
        if (stoneSkinActive)
        {
            return incomingDamage * (1f - damageReduction);
        }
        return incomingDamage;
    }

    public float GetDamageReduction()
    {
        return stoneSkinActive ? damageReduction : 0f;
    }
}
