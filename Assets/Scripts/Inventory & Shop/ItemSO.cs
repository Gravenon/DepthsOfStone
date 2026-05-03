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
        if (StatsManager.Instance == null)
        {
            return;
        }
        
        if (maxHealth != 0)  StatsManager.Instance.UpdateMaxHealth(maxHealth);
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


        if (maxHealth != 0)  StatsManager.Instance.UpdateMaxHealth(-maxHealth);
        if (currentHealth != 0) StatsManager.Instance.UpdateHealth(-currentHealth);
        if (speed != 0)     StatsManager.Instance.UpdateSpeed(-speed);
        if (damage != 0)    StatsManager.Instance.UpdateDamage(-damage);
    }
}
