using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManger : MonoBehaviour
{
    [SerializeField] private ShopSlot[] shopSlots;
    [SerializeField] private InventoryManager inventoryManager;

    [Header("Equipment Mirror (Shop Canvas)")]
    [Tooltip("EquipmentSlot components inside ShopCanvas that mirror the player's equipment panel")]
    [SerializeField] private EquipmentSlot[] shopEquipmentSlots;

    // All items across every shop page — populated by ShopKeeper on open
    private List<ShopItems> _allItems = new List<ShopItems>();


    public void SetAllShopItems(List<ShopItems> items, List<ShopItems> weapons, List<ShopItems> armour)
    {
        _allItems.Clear();
        if (items != null)   _allItems.AddRange(items);
        if (weapons != null) _allItems.AddRange(weapons);
        if (armour != null)  _allItems.AddRange(armour);
    }

    public void PopulateShopItem(List<ShopItems> items)
    {
        for (int i = 0; i < shopSlots.Length; i++)
        {
            if (i < items.Count)
            {
                shopSlots[i].Initialize(items[i].itemSO, items[i].price, items[i].ransomProcentage);
                shopSlots[i].gameObject.SetActive(true);
            }
            else
            {
                shopSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public void TryBuyItem(ItemSO item, int price)
    {
        if (ShopKeeper.currentShopKeeper != null && ShopKeeper.currentShopKeeper.OnlySell)
        {
            Debug.Log("[Shop] This merchant does not sell items.");
            return;
        }

        if (item == null || inventoryManager.coins < price)
        {
            Debug.Log("[Shop] Purchase failed — insufficient coins or invalid item.");
            return;
        }

        if (!HasSpaceForItem(item))
        {
            Debug.Log("[Shop] Purchase failed — no space in inventory.");
            return;
        }

        inventoryManager.coins -= price;
        inventoryManager.coinText.text = inventoryManager.coins.ToString();
        inventoryManager.AddItem(item, 1);
        SyncEquipmentMirror();
    }

    /// Sells one unit of an item from the player's inventory and awards coins based on the buyback rate.
    public void SellItem(ItemSO item)
    {
        if (item == null) return;

        if (!TryRemoveOneFromInventory(item)) return;

        inventoryManager.coins += GetSellReward(item);
        inventoryManager.coinText.text = inventoryManager.coins.ToString();
        SyncEquipmentMirror();
    }

    /// Refreshes the real equipment slots UI and syncs the shop-side mirror.
    public void RefreshInventoryDisplay()
    {
        foreach (var slot in inventoryManager.equimentSlot)
            slot.UpdateUI();

        SyncEquipmentMirror();
    }

    /// Copies data from the player's real equipment slots into the shop canvas mirror slots.
    public void SyncEquipmentMirror()
    {
        if (shopEquipmentSlots == null || shopEquipmentSlots.Length == 0) return;

        EquipmentSlot[] real = inventoryManager.equimentSlot;
        for (int i = 0; i < shopEquipmentSlots.Length; i++)
        {
            shopEquipmentSlots[i].itemSO   = i < real.Length ? real[i].itemSO   : null;
            shopEquipmentSlots[i].quantity = i < real.Length ? real[i].quantity : 0;
            shopEquipmentSlots[i].UpdateUI();
        }
    }

    // ─── Private Helpers ──────────────────────────────────────────────────────

    private static bool IsEquipmentType(ItemType type)
    {
        switch (type)
        {
            case ItemType.mainHand:
            case ItemType.head:
            case ItemType.body:
            case ItemType.legs:
            case ItemType.feet:
            case ItemType.relic:
                return true;
            default:
                return false;
        }
    }

    private bool HasSpaceForItem(ItemSO item)
    {
        if (IsEquipmentType(item.itemType))
        {
            foreach (var slot in inventoryManager.equimentSlot)
                if (slot.itemSO == null || (slot.itemSO == item && slot.quantity < item.stackSize))
                    return true;
            return false;
        }

        foreach (var slot in inventoryManager.inventorySlots)
            if (slot.itemSO == null || (slot.itemSO == item && slot.quantity < item.stackSize))
                return true;

        return false;
    }

    // Removes one unit from equipment slots first, then regular slots. Returns false if item not found.
    private bool TryRemoveOneFromInventory(ItemSO item)
    {
        foreach (var slot in inventoryManager.equimentSlot)
        {
            if (slot.itemSO == item && slot.quantity > 0)
            {
                DecrementSlot(slot);
                return true;
            }
        }

        foreach (var slot in inventoryManager.inventorySlots)
        {
            if (slot.itemSO == item && slot.quantity > 0)
            {
                DecrementSlot(slot);
                return true;
            }
        }

        return false;
    }

    private static void DecrementSlot(EquipmentSlot slot)
    {
        slot.quantity--;
        if (slot.quantity <= 0) slot.itemSO = null;
        slot.UpdateUI();
    }

    private static void DecrementSlot(InventorySlot slot)
    {
        slot.quantity--;
        if (slot.quantity <= 0) slot.itemSO = null;
        slot.UpdateUI();
    }

    private int GetSellReward(ItemSO item)
    {
        foreach (var entry in _allItems)
            if (entry.itemSO == item)
                return (int)Math.Ceiling(entry.price * entry.ransomProcentage);
        return 0;
    }
}

[Serializable]
public class ShopItems
{
    public ItemSO itemSO;
    public int price;

    [Tooltip("Fraction of the purchase price the player receives when selling (0.0 – 1.0)")]
    public float ransomProcentage;

}