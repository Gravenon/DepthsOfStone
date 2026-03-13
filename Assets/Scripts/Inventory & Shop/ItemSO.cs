using UnityEngine;

[CreateAssetMenu(fileName = "New Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    [TextArea]public string itemDescription;  
    public Sprite itemIcon;

    public ItemType itemType;
    public int stackSize = 6;

    [Header("Stats")]
    public int currentHealth;
    public int maxHealth;
    public int speed;
    public int damage;

    [Header("For temporary Items")]
    public float duration;



    public void Use()
    {
        switch (itemType)
        {
            case ItemType.mainHand:
                StatsManager.Instance.UpdateHealth(currentHealth);
                StatsManager.Instance.UpdateMaxHealth(maxHealth);
                StatsManager.Instance.UpdateSpeed(speed);
                StatsManager.Instance.UpdateDamage(damage);
                break;
            case ItemType.head:
                StatsManager.Instance.UpdateHealth(currentHealth);
                StatsManager.Instance.UpdateMaxHealth(maxHealth);
                StatsManager.Instance.UpdateSpeed(speed);
                StatsManager.Instance.UpdateDamage(damage);
                break;
            case ItemType.body:
                StatsManager.Instance.UpdateHealth(currentHealth);
                StatsManager.Instance.UpdateMaxHealth(maxHealth);
                StatsManager.Instance.UpdateSpeed(speed);
                StatsManager.Instance.UpdateDamage(damage);
                break;
            case ItemType.legs:
                StatsManager.Instance.UpdateHealth(currentHealth);
                StatsManager.Instance.UpdateMaxHealth(maxHealth);
                StatsManager.Instance.UpdateSpeed(speed);
                StatsManager.Instance.UpdateDamage(damage);
                break;
            case ItemType.relic:
                StatsManager.Instance.UpdateHealth(currentHealth);
                StatsManager.Instance.UpdateMaxHealth(maxHealth);
                StatsManager.Instance.UpdateSpeed(speed);
                StatsManager.Instance.UpdateDamage(damage);
                break;
            case ItemType.feet:
                StatsManager.Instance.UpdateHealth(currentHealth);
                StatsManager.Instance.UpdateMaxHealth(maxHealth);
                StatsManager.Instance.UpdateSpeed(speed);
                StatsManager.Instance.UpdateDamage(damage);
                break;      
        }
    }

    public void Unuse()
    {
         switch (itemType)
        {
            case ItemType.mainHand:
                StatsManager.Instance.UpdateHealth(-currentHealth);
                StatsManager.Instance.UpdateMaxHealth(-maxHealth);
                StatsManager.Instance.UpdateSpeed(-speed);
                StatsManager.Instance.UpdateDamage(-damage);
                break;
            case ItemType.head:
                StatsManager.Instance.UpdateHealth(-currentHealth);
                StatsManager.Instance.UpdateMaxHealth(-maxHealth);
                StatsManager.Instance.UpdateSpeed(-speed);
                StatsManager.Instance.UpdateDamage(-damage);
                break;
            case ItemType.body:
                StatsManager.Instance.UpdateHealth(-currentHealth);
                StatsManager.Instance.UpdateMaxHealth(-maxHealth);
                StatsManager.Instance.UpdateSpeed(-speed);
                StatsManager.Instance.UpdateDamage(-damage);
                break;
            case ItemType.legs:
                StatsManager.Instance.UpdateHealth(-currentHealth);
                StatsManager.Instance.UpdateMaxHealth(-maxHealth);
                StatsManager.Instance.UpdateSpeed(-speed);
                StatsManager.Instance.UpdateDamage(-damage);
                break;
            case ItemType.relic:
                StatsManager.Instance.UpdateHealth(-currentHealth);
                StatsManager.Instance.UpdateMaxHealth(-maxHealth);
                StatsManager.Instance.UpdateSpeed(-speed);
                StatsManager.Instance.UpdateDamage(-damage);
                break;
            case ItemType.feet:
                StatsManager.Instance.UpdateHealth(-currentHealth);
                StatsManager.Instance.UpdateMaxHealth(-maxHealth);
                StatsManager.Instance.UpdateSpeed(-speed);
                StatsManager.Instance.UpdateDamage(-damage);
                break;      
        }    
    }
}
