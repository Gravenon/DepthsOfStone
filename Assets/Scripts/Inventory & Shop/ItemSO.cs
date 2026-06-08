using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(fileName = "New Item")]
public class ItemSO : ScriptableObject
{
    public string itemNameKey;
    [TextArea] public string itemDescriptionKey;
    public Sprite itemIcon;

    public ItemType itemType;
    public bool isEXP;
    public int stackSize = 6;

    [Header("Stats")]
    public int currentHealth;
    public int maxHealth;
    public int speed;
    public int damage;

    [Header("For temporary Items")]
    public float duration;


    public string itemName => LocalizationSettings.StringDatabase.GetLocalizedString("Items", itemNameKey);
    public string itemDescription => LocalizationSettings.StringDatabase.GetLocalizedString("Items", itemDescriptionKey);


    public void Use()
    {
        if (StatsManager.Instance == null)
        {
            return;
        }

        if (maxHealth != 0) StatsManager.Instance.UpdateMaxHealth(maxHealth);
        if (currentHealth != 0) StatsManager.Instance.UpdateHealth(currentHealth);
        if (speed != 0) StatsManager.Instance.UpdateSpeed(speed);
        if (damage != 0) StatsManager.Instance.UpdateDamage(damage);
    }

    public void Unuse()
    {
        if (StatsManager.Instance == null)
        {
            return;
        }


        if (maxHealth != 0) StatsManager.Instance.UpdateMaxHealth(-maxHealth);
        if (currentHealth != 0) StatsManager.Instance.UpdateHealth(-currentHealth);
        if (speed != 0) StatsManager.Instance.UpdateSpeed(-speed);
        if (damage != 0) StatsManager.Instance.UpdateDamage(-damage);
    }
}
