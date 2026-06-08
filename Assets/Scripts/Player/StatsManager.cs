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

    [Header("Movement Stats")]
    public int speed;
    [SerializeField] public Transform playerTransform;

    [Header("Health Stats")]
    public int maxHealth;
    public int currentHealth;

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
        if (Instance == null) Instance = this; else { Destroy(gameObject); return; }

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
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        RefreshHealthText();
    }

    public void UpdateSpeed(int amount)
    {
        speed += amount;
        statsUI?.UpdataAllStats();
    }

    public void UpdateDamage(float amount)
    {
        damage += amount;
        statsUI?.UpdataAllStats();
    }

    public void UpdataKnockbackForce(int amount)
    {
        knockbackForce += amount;
        statsUI?.UpdataAllStats();
    }

    private void RefreshHealthText()
    {
        if (healthText != null) healthText.text = $"HP: {currentHealth}/ {maxHealth}";
        OnHealthChanged?.Invoke();
    }

    public void LoadData(GameData data)
    {
        if (this == null || playerTransform == null) return;

        if (DataPersistenceeManager.IsRespawning)
        {
            if (!string.IsNullOrEmpty(data.checkpointScene) && data.checkpointScene == SceneManager.GetActiveScene().name)
                playerTransform.position = data.checkpointPosition;
            else if (!string.IsNullOrEmpty(data.lastScene) && data.lastScene == SceneManager.GetActiveScene().name)
                playerTransform.position = data.playerPosition;

            int respawnMax = DataPersistenceeManager.RespawnMaxHealth > 0 ? DataPersistenceeManager.RespawnMaxHealth
                           : (data.maxHealth != 0 ? data.maxHealth : baseMaxHealth);
            maxHealth = respawnMax;
            currentHealth = maxHealth;
            DataPersistenceeManager.RespawnMaxHealth = 0;
        }
        else
        {
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

        if (healthText != null) healthText.text = $"HP: {currentHealth}/ {maxHealth}";
        statsUI?.UpdataAllStats();
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
