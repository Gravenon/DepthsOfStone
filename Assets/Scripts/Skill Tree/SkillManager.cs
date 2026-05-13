using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkillManager : MonoBehaviour, IDataPersistence
{
    public PlayerHealth playerHealth;
    public PlayerMovment playerMovement;

    private bool regenerationActive;
    private bool lastChanceActive;
    private bool lastChanceBonusApplied;
    private bool berserkUnlocked;
    private bool berserkActivated;
    private bool stoneSkinActive;
    private float baseDamage;
    private float damageReduction;

    private void OnEnable()
    {
        SkillSlot.OnAbilityPointSpent += HandleAbilityPointSpent;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SkillSlot.OnAbilityPointSpent -= HandleAbilityPointSpent;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Stop Berserk coroutine on scene reload to prevent SetInvulnerable leaking across scenes.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        berserkActivated = false;
        playerHealth?.SetInvulnerable(false);
    }

    private void Update()
    {
        if (lastChanceActive)
            CheckLastChanceBonus();
    }

    private void HandleAbilityPointSpent(SkillSlot slot, SkillType skillType)
    {
        switch (slot.skillSO.skillName)
        {
            case "Max Health Boost":
                StatsManager.Instance.UpdateMaxHealth(15);
                break;
            case "Stone Skin":
                if (!stoneSkinActive)
                {
                    stoneSkinActive = true;
                    damageReduction += 0.15f;
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
                StatsManager.Instance.UpdateDamage(StatsManager.Instance.damage * 0.15f);
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
                playerMovement?.UnlockDash();
                break;
            default:
                Debug.LogWarning($"No implementation for skill: {slot.skillSO.skillName}");
                break;
        }
    }

    private IEnumerator RegenerationCoroutine()
    {
        while (regenerationActive)
        {
            yield return new WaitForSeconds(3f);
            if (StatsManager.Instance.currentHealth < StatsManager.Instance.maxHealth)
                StatsManager.Instance.UpdateHealth(2);
        }
    }

    private void CheckLastChanceBonus()
    {
        float hp = (float)StatsManager.Instance.currentHealth / StatsManager.Instance.maxHealth;

        if (hp <= 0.3f && !lastChanceBonusApplied)
        {
            baseDamage = StatsManager.Instance.damage;
            StatsManager.Instance.damage = baseDamage * 1.2f;
            lastChanceBonusApplied = true;
        }
        else if (hp > 0.3f && lastChanceBonusApplied)
        {
            StatsManager.Instance.damage = baseDamage;
            lastChanceBonusApplied = false;
        }

        if (berserkUnlocked && hp <= 0.1f && !berserkActivated)
        {
            berserkActivated = true;
            StartCoroutine(BerserkRageCoroutine());
        }
    }

    private IEnumerator BerserkRageCoroutine()
    {
        float savedDamage = StatsManager.Instance.damage;

        StatsManager.Instance.damage += 5;
        StatsManager.Instance.UpdateSpeed(3);
        playerHealth?.SetInvulnerable(true);

        yield return new WaitForSeconds(3f);

        StatsManager.Instance.damage = savedDamage;
        StatsManager.Instance.UpdateSpeed(-3);
        playerHealth?.SetInvulnerable(false);

        Debug.Log("[SkillManager] Berserk ended.");

        yield return new WaitForSeconds(5f);
        berserkActivated = false;
    }

    public float ApplyDamageReduction(float incomingDamage)
        => stoneSkinActive ? incomingDamage * (1f - damageReduction) : incomingDamage;

    public float GetDamageReduction()
        => stoneSkinActive ? damageReduction : 0f;

    // ─── IDataPersistence ───────────────────────────────────────────────────

    public void SaveData(ref GameData data)
    {
        // Skill effects are reflected in StatsManager data and SkillTreeManager slot data.
        // Nothing extra to save here.
    }

    public void LoadData(GameData data)
    {
        if (data.skillStates == null || data.skillStates.Length == 0) return;

        // Re-apply runtime behaviour flags for every saved skill.
        // We do NOT touch stats here — StatsManager already restores them.
        foreach (var saved in data.skillStates)
        {
            if (saved.currentLevel <= 0) continue;
            RestoreRuntimeEffect(saved.skillName);
        }
    }

    /// <summary>
    /// Restores only the runtime-behaviour side of a skill (flags/coroutines).
    /// Stat bonuses are intentionally skipped — StatsManager restores those.
    /// </summary>
    private void RestoreRuntimeEffect(string skillName)
    {
        switch (skillName)
        {
            case "Stone Skin":
                if (!stoneSkinActive)
                {
                    stoneSkinActive  = true;
                    damageReduction += 0.15f;
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
                baseDamage = StatsManager.Instance != null ? StatsManager.Instance.damage : 0f;
                break;
            case "Berserk":
                berserkUnlocked = true;
                break;
            case "Dash":
                playerMovement?.UnlockDash();
                break;
            // Stat-only skills (Max Health Boost, A powerful blow, Light on his feet, Punching)
            // are already handled by StatsManager's saved data — no action needed here.
        }
    }
}
