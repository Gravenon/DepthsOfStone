using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StatsManager : MonoBehaviour, IDataPersistence
{
    public static StatsManager Instance;
    public UseItem useItem;
    public StatsUI statsUI;
    public TMP_Text healthText;

    [Header("Combat Stats")]
    public int damage;
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
    [HideInInspector] public int baseDamage;
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
        healthText.text = "HP: " + currentHealth + "/ " + maxHealth;
    }

    public void UpdateHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth >= maxHealth)
            currentHealth = maxHealth;

        healthText.text = "HP: " + currentHealth + "/ " + maxHealth;
    }

    public void UpdateSpeed(int amount)
    {
        speed += amount;
        statsUI.UpdataAllStats();
    }

    public void UpdateDamage(int amount)
    {
        damage += amount;
        statsUI.UpdataAllStats();
    }

// ---------------------------------------------------------------
// IDataPersistence implementation
// ---------------------------------------------------------------
    public void LoadData(GameData data)
    {
        if (this == null || playerTransform == null) return;

        // Restore position only if it was saved in this same scene.
        // For a brand-new world (lastScene is empty), always spawn at origin.
        if (string.IsNullOrEmpty(data.lastScene))
            playerTransform.position = Vector3.zero;
        else if (data.lastScene == SceneManager.GetActiveScene().name)
            playerTransform.position = data.playerPosition;

        maxHealth = data.maxHealth != 0 ? data.maxHealth : baseMaxHealth;
        currentHealth = data.currentHealth != 0 ? data.currentHealth : baseCurrentHealth;
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
