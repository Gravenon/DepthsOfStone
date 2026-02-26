using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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
}
