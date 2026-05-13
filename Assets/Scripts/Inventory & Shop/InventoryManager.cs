using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour, IDataPersistence
{
    public static InventoryManager Instance;

    public GameObject EqipmentPanel;
    public InputActionReference toggleInventoryAction;

    public InventorySlot[] inventorySlots;
    public EquipmentSlot[] equimentSlot;

    public UseItem useItem;
    public int coins;
    public TMP_Text coinText;
    public GameObject lootPrefab;
    public Transform player;

    public static event Action<int> OnExperienceGained;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Start() runs after sceneLoaded → LoadData(), so UpdateUI reflects restored save data.
        foreach (var slot in inventorySlots)
            slot.UpdateUI();
        foreach (var slot in equimentSlot)
            slot.UpdateUI();
        foreach (var slot in FindObjectsByType<EquippedSlot>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            slot.UpdateUI();
    }

    private void OnEnable()
    {
        Loot.OnItemLooted += AddItem;
        if (toggleInventoryAction != null)
            toggleInventoryAction.action.performed += OnToggleInventory;
    }

    private void OnDisable()
    {
        Loot.OnItemLooted -= AddItem;
        if (toggleInventoryAction != null)
            toggleInventoryAction.action.performed -= OnToggleInventory;
    }

    // -------------------------------------------------------
    // IDataPersistence
    // -------------------------------------------------------
    public void LoadData(GameData data)
    {
        foreach (var slot in inventorySlots)
        {
            slot.itemSO = null;
            slot.quantity = 0;
            slot.UpdateUI();
        }

        foreach (var slot in equimentSlot)
        {
            slot.itemSO = null;
            slot.quantity = 0;
            slot.UpdateUI();
        }

        var allEquippedSlots = FindObjectsByType<EquippedSlot>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var slot in allEquippedSlots)
            slot.ClearSlot();

        coins = data.coins;
        if (coinText != null)
            coinText.text = coins.ToString();

        ItemSOLibrary library = FindFirstObjectByType<ItemSOLibrary>();
        if (library == null) return;

        if (data.inventoryItems != null)
            foreach (var saved in data.inventoryItems)
            {
                ItemSO item = Array.Find(library.itemSOs, so => so != null && so.itemName == saved.itemName);
                if (item != null) AddItem(item, saved.quantity);
            }

        if (data.equipmentItems != null)
            foreach (var saved in data.equipmentItems)
            {
                ItemSO item = Array.Find(library.itemSOs, so => so != null && so.itemName == saved.itemName);
                if (item != null) AddItem(item, saved.quantity);
            }

        // Restore equipped slot visuals — stats are already restored by StatsManager.LoadData.
        if (data.equippedItems != null)
            foreach (var saved in data.equippedItems)
            {
                ItemSO item = Array.Find(library.itemSOs, so => so != null && so.itemName == saved.itemName);
                if (item == null) continue;
                EquippedSlot target = Array.Find(allEquippedSlots, s => (int)s.GetSlotItemType() == saved.itemTypeId);
                target?.RestoreGearVisual(item);
            }
    }

    public void SaveData(ref GameData data)
    {
        data.coins = coins;

        var invList = new List<SerializedSlot>();
        foreach (var slot in inventorySlots)
            if (slot.itemSO != null && slot.quantity > 0)
                invList.Add(new SerializedSlot { itemName = slot.itemSO.itemName, quantity = slot.quantity });
        data.inventoryItems = invList.ToArray();

        var eqList = new List<SerializedSlot>();
        foreach (var slot in equimentSlot)
            if (slot.itemSO != null && slot.quantity > 0)
                eqList.Add(new SerializedSlot { itemName = slot.itemSO.itemName, quantity = slot.quantity });
        data.equipmentItems = eqList.ToArray();

        var wornList = new List<SerializedEquippedSlot>();
        foreach (var slot in FindObjectsByType<EquippedSlot>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            ItemSO item = slot.GetEquippedItem();
            if (item != null)
                wornList.Add(new SerializedEquippedSlot { itemTypeId = (int)slot.GetSlotItemType(), itemName = item.itemName });
        }
        data.equippedItems = wornList.ToArray();
    }

    // -------------------------------------------------------
    // Inventory Logic
    // -------------------------------------------------------
    private void OnToggleInventory(InputAction.CallbackContext context)
    {
        if (EqipmentPanel != null)
            EqipmentPanel.SetActive(!EqipmentPanel.activeSelf);
    }

    public void AddItem(ItemSO itemSO, int quantity)
    {
        if (itemSO.itemType == ItemType.coins)
        {
            coins += quantity;
            coinText.text = coins.ToString();
            return;
        }

        if (itemSO.isEXP)
        {
            OnExperienceGained?.Invoke(quantity);
            return;
        }

        bool isEquipment = itemSO.itemType == ItemType.mainHand || itemSO.itemType == ItemType.head
            || itemSO.itemType == ItemType.body || itemSO.itemType == ItemType.legs
            || itemSO.itemType == ItemType.feet || itemSO.itemType == ItemType.relic;

        InventorySlot[] targetInvSlots = isEquipment ? null : inventorySlots;
        EquipmentSlot[] targetEqSlots = isEquipment ? equimentSlot : null;

        if (!isEquipment)
        {
            foreach (var slot in inventorySlots)
            {
                if (slot.itemSO != itemSO || slot.quantity >= itemSO.stackSize) continue;
                int add = Mathf.Min(itemSO.stackSize - slot.quantity, quantity);
                slot.quantity += add;
                quantity -= add;
                slot.UpdateUI();
                if (quantity <= 0) return;
            }
            foreach (var slot in inventorySlots)
            {
                if (slot.itemSO != null) continue;
                slot.itemSO = itemSO;
                slot.quantity = Mathf.Min(itemSO.stackSize, quantity);
                slot.UpdateUI();
                return;
            }
        }
        else
        {
            foreach (var slot in equimentSlot)
            {
                if (slot.itemSO != itemSO || slot.quantity >= itemSO.stackSize) continue;
                int add = Mathf.Min(itemSO.stackSize - slot.quantity, quantity);
                slot.quantity += add;
                quantity -= add;
                slot.UpdateUI();
                if (quantity <= 0) return;
            }
            foreach (var slot in equimentSlot)
            {
                if (slot.itemSO != null) continue;
                slot.itemSO = itemSO;
                slot.quantity = Mathf.Min(itemSO.stackSize, quantity);
                slot.UpdateUI();
                return;
            }
        }

        if (quantity > 0)
            DropLoot(itemSO, quantity);
    }

    public void RemoveItem(ItemSO itemSO, int quantity)
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.itemSO != itemSO) continue;
            if (slot.quantity > quantity)
            {
                slot.quantity -= quantity;
                slot.UpdateUI();
                return;
            }
            quantity -= slot.quantity;
            slot.itemSO = null;
            slot.quantity = 0;
            slot.UpdateUI();
        }
    }

    public void DropItem(InventorySlot slot)
    {
        DropLoot(slot.itemSO, 1);
        slot.quantity--;
        if (slot.quantity <= 0) slot.itemSO = null;
        slot.UpdateUI();
    }

    public void DropItem(EquipmentSlot slot)
    {
        DropLoot(slot.itemSO, 1);
        slot.quantity--;
        if (slot.quantity <= 0) slot.itemSO = null;
        slot.UpdateUI();
    }

    private void DropLoot(ItemSO itemSO, int quantity)
    {
        Loot loot = Instantiate(lootPrefab, player.position, Quaternion.identity).GetComponent<Loot>();
        loot.Initialize(itemSO, quantity);
    }

    public void UseItem(InventorySlot slot)
    {
        if (slot.itemSO == null || slot.quantity <= 0) return;
        useItem.ApplayItemEffects(slot.itemSO);
        slot.quantity--;
        if (slot.quantity <= 0) slot.itemSO = null;
        slot.UpdateUI();
    }

    public bool HasItem(ItemSO itemSO)
    {
        foreach (var slot in inventorySlots)
            if (slot.itemSO == itemSO && slot.quantity > 0) return true;
        return false;
    }

    public int GetItemCount(ItemSO itemSO)
    {
        int total = 0;
        foreach (var slot in inventorySlots)
            if (slot.itemSO == itemSO) total += slot.quantity;
        return total;
    }
}

public enum ItemType
{
    head, body, legs, mainHand, feet, relic, ore, coins, none
}
