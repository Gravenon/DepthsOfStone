using System;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public ItemSO itemSO;
    public SpriteRenderer sr;
    public Animator anim;

    public bool canBePickedUp = true;
    public int quantity;
    public static event Action<ItemSO, int> OnItemLooted;

    private void Start()
    {
        if(itemSO != null)
        {
            sr.sprite = itemSO.itemIcon;
            name = itemSO.itemName;
        }
    }

    //#if UNITY_EDITOR
    //    private void OnValidate()
    //    {
    //        if(Application.isPlaying && itemSO != null)
    //        {
    //            name = itemSO.itemName;
    //        }
    //    }
    //#endif

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
        if (collision.CompareTag("Player") && canBePickedUp == true)
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
