using System;
using UnityEngine;

public class Loot : MonoBehaviour
{
    [SerializeField] private string id;

    [ContextMenu("Generate ID")]
    private void GenerateID()
    {
        id = Guid.NewGuid().ToString();
    }

    public ItemSO itemSO;
    public SpriteRenderer sr;
    public Animator anim;

    public bool canBePickedUp = true;
    public int quantity;
    public static event Action<ItemSO, int> OnItemLooted;

    private bool collected = false;

    private void Start()
    {
        if(itemSO != null)
        {
            sr.sprite = itemSO.itemIcon;
            name = itemSO.itemName;
        }
    }

    //save data for loot

    // public void LoadData(GameData data)
    // {
    //     data.itemCollected.TryGetValue(id, out collected);
    //     if (collected)
    //     {
    //         Destroy(gameObject);
    //     }
    // }

    // public void SaveData(ref GameData data)
    // {
    //     if (data.itemCollected.ContainsKey(id))
    //     {
    //         data.itemCollected.Remove(id);
    //     }
    //     data.itemCollected.Add(id, collected);
        

    // }

    private void OnValidate()
    {
        if (itemSO == null)
            return;

        UpdateAppearance();
    }

    public void Initialize(ItemSO itemSO , int quantity)
    {
        this.itemSO = itemSO;
        this.quantity = quantity;
        canBePickedUp = false;
        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        sr.sprite = itemSO.itemIcon;
        this.name = itemSO.itemName;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canBePickedUp == true && collected == false)
        {
            anim.Play("LootPickup");
            OnItemLooted?.Invoke(itemSO, quantity);
            Destroy(gameObject, 0.5f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canBePickedUp = true;
        }
    }
}
