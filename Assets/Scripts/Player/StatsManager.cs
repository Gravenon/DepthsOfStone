using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StatsManager : MonoBehaviour, IDataPersistence
{
    public static StatsManager Instance;
    public static event System.Action OnHealthChanged;

    public UseItem useItem;
    public StatsUI statsUI;
    public TMP_Text healthText;

    [Header("Combat Stats")]
    public float damage;
    public float weaponRange;
    public float knockbackForce;
    public float knockbackTimre;
    public float stunTime;

    [Header("Movment Stats")]
    public int speed;
    [SerializeField] public Transform playerTransform;

    [Header("Health Stats")]
    public int maxHealth;
    public int currentHealth;

    // Base values as set in Inspector — never modified at runtime.
    // Used to initialise GameData for a brand-new world.
    [HideInInspector] public int baseMaxHealth;
    [HideInInspector] public int baseCurrentHealth;
    [HideInInspector] public int baseSpeed;
    [HideInInspector] public float baseDamage;
    [HideInInspector] public float baseWeaponRange;
    [HideInInspector] public float baseKnockbackForce;
    [HideInInspector] public float baseKnockbackTimre;
    [HideInInspector] public float baseStunTime;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Snapshot Inspector values as the true base before any item modifies them.
        baseMaxHealth = maxHealth;
        baseCurrentHealth = currentHealth;
        baseSpeed = speed;
        baseDamage = damage;
        baseWeaponRange = weaponRange;
        baseKnockbackForce = knockbackForce;
        baseKnockbackTimre = knockbackTimre;
        baseStunTime = stunTime;
    }

    public void UpdateMaxHealth(int amount)
    {
        maxHealth += amount;
        if (maxHealth < 0) maxHealth = 0;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        RefreshHealthText();
    }

    public void UpdateHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if (currentHealth < 0) currentHealth = 0;
        RefreshHealthText();
    }

    public void UpdateSpeed(int amount)
    {
        speed += amount;
        if (statsUI != null) statsUI.UpdataAllStats();
        else Debug.LogWarning("[StatsManager] statsUI is NULL — assign it in Inspector!");
    }

    public void UpdateDamage(float amount)
    {
        damage += amount;
        if (statsUI != null) statsUI.UpdataAllStats();
        else Debug.LogWarning("[StatsManager] statsUI is NULL — assign it in Inspector!");
    }

    public void UpdataKnockbackForce(int amount)
    {
        knockbackForce += amount;
        if (statsUI != null) statsUI.UpdataAllStats();
    }

    private void RefreshHealthText()
    {
        if (healthText != null)
            healthText.text = "HP: " + currentHealth + "/ " + maxHealth;
        else
            Debug.LogWarning("[StatsManager] healthText is NULL — assign it in Inspector!");

        OnHealthChanged?.Invoke();
    }

// ---------------------------------------------------------------
// IDataPersistence implementation
// ---------------------------------------------------------------
    public void LoadData(GameData data)
    {
        if (this == null || playerTransform == null) return;

        if (DataPersistenceeManager.IsRespawning && !string.IsNullOrEmpty(data.checkpointScene))
        {
            // Respawning from checkpoint: use checkpoint position and restore health to checkpoint health
            if (data.checkpointScene == SceneManager.GetActiveScene().name)
                playerTransform.position = data.checkpointPosition;

            maxHealth = data.maxHealth != 0 ? data.maxHealth : baseMaxHealth;
            currentHealth = data.checkpointHealth > 0 ? data.checkpointHealth : (data.maxHealth != 0 ? data.maxHealth : baseMaxHealth);
        }
        else
        {
            // Restore position only if it was saved in this same scene.
            // For a brand-new world (lastScene is empty), always spawn at origin.
            if (string.IsNullOrEmpty(data.lastScene))
                playerTransform.position = Vector3.zero;
            else if (data.lastScene == SceneManager.GetActiveScene().name)
                playerTransform.position = data.playerPosition;

            maxHealth = data.maxHealth != 0 ? data.maxHealth : baseMaxHealth;
            currentHealth = data.currentHealth != 0 ? data.currentHealth : baseCurrentHealth;
        }

        speed = data.speed != 0 ? data.speed : baseSpeed;
        damage = data.damage != 0 ? data.damage : baseDamage;
        weaponRange = data.weaponRange != 0 ? data.weaponRange : baseWeaponRange;
        knockbackForce = data.knockbackForce != 0 ? data.knockbackForce : baseKnockbackForce;
        knockbackTimre = data.knockbackTimre != 0 ? data.knockbackTimre : baseKnockbackTimre;
        stunTime = data.stunTime != 0 ? data.stunTime : baseStunTime;

        if (healthText != null)
            healthText.text = "HP: " + currentHealth + "/ " + maxHealth;

        if (statsUI != null)
            statsUI.UpdataAllStats();

        // Notify health bar UI so the slider refreshes after load/respawn
        OnHealthChanged?.Invoke();
    }

    public void SaveData(ref GameData data)
    {
        if (this == null || playerTransform == null) return;

        data.lastScene = SceneManager.GetActiveScene().name;
        data.playerPosition = playerTransform.position;

        data.maxHealth = maxHealth;
        data.currentHealth = currentHealth;
        data.speed = speed;
        data.damage = damage;
        data.weaponRange = weaponRange;
        data.knockbackForce = knockbackForce;
        data.knockbackTimre = knockbackTimre;
        data.stunTime = stunTime;

        data.saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }

}
