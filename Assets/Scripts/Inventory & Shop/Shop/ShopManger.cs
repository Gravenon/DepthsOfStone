using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShopManger : MonoBehaviour
{
    [SerializeField] private ShopSlot[] shopSlots;

    [SerializeField] private InventoryManager inventoryManager;

    public void PopulateShopItem(List<ShopItems> shopItems)
    {
        for (int i = 0; i < shopItems.Count && i < shopSlots.Length; i++)
        {
            ShopItems shopItem = shopItems[i];
            shopSlots[i].Initialize(shopItem.itemSO, shopItem.price);
            shopSlots[i].gameObject.SetActive(true);
        }

        for (int i = shopItems.Count; i < shopSlots.Length; i++)
        {
            shopSlots[i].gameObject.SetActive(false);
        }
    }

    public void TryBuyItem(ItemSO itemSO, int price)
    {
        if (itemSO != null && inventoryManager.coins >= price)
        {
            if (HasSpaceForItem(itemSO))
            {
                inventoryManager.coins -= price;
                inventoryManager.coinText.text = inventoryManager.coins.ToString();
                inventoryManager.AddItem(itemSO, 1);
            }
        }
    }

    private bool HasSpaceForItem(ItemSO itemSO)
    {
        foreach (var slot in inventoryManager.inventorySlots)
        {
            if (slot.itemSO == itemSO && slot.quantity < itemSO.stackSize)
                return true;
            else if (slot.itemSO == null)
                return true;
        }
        return false;
    }


    public void SellItem(ItemSO itemSO)
    {
        if(itemSO == null)
            return;

        foreach (var slot in shopSlots)
        {
            if (slot.itemSO == itemSO)
            {

                int reward = (int)Math.Ceiling(slot.price / 5.0);
                inventoryManager.coins += reward;
                inventoryManager.coinText.text = inventoryManager.coins.ToString();
            }

        }
    }

}



[System.Serializable]
public class ShopItems
{
    public ItemSO itemSO;
    public int price;

}