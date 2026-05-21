using System;
using System.Collections.Generic;
using System.Linq;
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

    private CanvasGroup _equipmentCG;

    // Key=(int)ItemType, Value=itemName. Static — переживает пересоздание между сценами.
    private static readonly Dictionary<int, string> _equippedCache = new Dictionary<int, string>();

    // Реестр слотов: заполняется в Awake() слотов, до вызова sceneLoaded → LoadData.
    private static readonly List<EquippedSlot> _registeredSlots = new List<EquippedSlot>();

    public static void RegisterSlot(EquippedSlot slot)   { if (!_registeredSlots.Contains(slot)) _registeredSlots.Add(slot); }
    public static void UnregisterSlot(EquippedSlot slot) => _registeredSlots.Remove(slot);

    public static ItemSO GetEquippedItemSO(int typeId)
    {
        if (!_equippedCache.TryGetValue(typeId, out string name)) return null;
        var lib = ItemSOLibrary.Instance != null ? ItemSOLibrary.Instance : FindFirstObjectByType<ItemSOLibrary>();
        return lib == null ? null : Array.Find(lib.itemSOs, so => so != null && so.itemName == name);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (EqipmentPanel != null)
        {
            _equipmentCG = EqipmentPanel.GetComponent<CanvasGroup>() ?? EqipmentPanel.AddComponent<CanvasGroup>();
            UIManager.SetVisible(_equipmentCG, false);
        }
        foreach (var slot in inventorySlots) slot.UpdateUI();
        foreach (var slot in equimentSlot)   slot.UpdateUI();
        foreach (var slot in _registeredSlots) slot.UpdateUI();
    }

    private void OnEnable()
    {
        Loot.OnItemLooted += AddItem;
        if (toggleInventoryAction != null) toggleInventoryAction.action.performed += OnToggleInventory;
    }

    private void OnDisable()
    {
        Loot.OnItemLooted -= AddItem;
        if (toggleInventoryAction != null) toggleInventoryAction.action.performed -= OnToggleInventory;
    }

    public void RegisterEquip(int typeId, string itemName)   => _equippedCache[typeId] = itemName;
    public void RegisterUnequip(int typeId)                  => _equippedCache.Remove(typeId);

    public void LoadData(GameData data)
    {
        _equippedCache.Clear();
        if (data.equippedItems != null)
            foreach (var s in data.equippedItems)
                if (!string.IsNullOrEmpty(s.itemName))
                    _equippedCache[s.itemTypeId] = s.itemName;

        var lib = ItemSOLibrary.Instance != null ? ItemSOLibrary.Instance : FindFirstObjectByType<ItemSOLibrary>();
        if (lib == null)
        {
            coins = data.coins;
            if (coinText != null) coinText.text = coins.ToString();
            return;
        }

        foreach (var slot in inventorySlots) { slot.itemSO = null; slot.quantity = 0; slot.UpdateUI(); }
        foreach (var slot in equimentSlot)   { slot.itemSO = null; slot.quantity = 0; slot.UpdateUI(); }
        foreach (var slot in _registeredSlots) slot.ClearSlot();

        coins = data.coins;
        if (coinText != null) coinText.text = coins.ToString();

        if (data.inventoryItems != null)
            foreach (var s in data.inventoryItems)
            { var item = Array.Find(lib.itemSOs, so => so != null && so.itemName == s.itemName); if (item != null) AddItem(item, s.quantity); }

        if (data.equipmentItems != null)
            foreach (var s in data.equipmentItems)
            { var item = Array.Find(lib.itemSOs, so => so != null && so.itemName == s.itemName); if (item != null) AddItem(item, s.quantity); }

        foreach (var slot in _registeredSlots)
        {
            var item = GetEquippedItemSO((int)slot.GetSlotItemType());
            if (item != null) slot.RestoreGearVisual(item);
        }
    }

    public void SaveData(ref GameData data)
    {
        data.coins = coins;

        if (ItemSOLibrary.Instance == null && FindFirstObjectByType<ItemSOLibrary>() == null) return;

        data.inventoryItems = inventorySlots
            .Where(s => s.itemSO != null && s.quantity > 0)
            .Select(s => new SerializedSlot { itemName = s.itemSO.itemName, quantity = s.quantity })
            .ToArray();

        data.equipmentItems = equimentSlot
            .Where(s => s.itemSO != null && s.quantity > 0)
            .Select(s => new SerializedSlot { itemName = s.itemSO.itemName, quantity = s.quantity })
            .ToArray();

        data.equippedItems = _equippedCache
            .Select(kvp => new SerializedEquippedSlot { itemTypeId = kvp.Key, itemName = kvp.Value })
            .ToArray();
    }

    private void OnToggleInventory(InputAction.CallbackContext ctx)
    {
        if (_equipmentCG == null) return;
        if (UIManager.Instance != null) UIManager.Instance.Toggle(_equipmentCG);
        else UIManager.SetVisible(_equipmentCG, _equipmentCG.alpha < 0.5f);
    }

    public void AddItem(ItemSO itemSO, int quantity)
    {
        if (itemSO.itemType == ItemType.coins) { coins += quantity; coinText.text = coins.ToString(); return; }
        if (itemSO.isEXP) { OnExperienceGained?.Invoke(quantity); return; }

        bool isOre  = itemSO.itemType == ItemType.ore;
        bool isGear = itemSO.itemType is ItemType.mainHand or ItemType.head or ItemType.body
                                      or ItemType.legs    or ItemType.feet  or ItemType.relic;

        if (isGear || isOre)
        {
            int max = isOre ? itemSO.stackSize : 1;
            foreach (var s in equimentSlot)
            { if (s.itemSO != itemSO || s.quantity >= max) continue; int add = Mathf.Min(max - s.quantity, quantity); s.quantity += add; quantity -= add; s.UpdateUI(); if (quantity <= 0) return; }
            foreach (var s in equimentSlot)
            { if (s.itemSO != null) continue; s.itemSO = itemSO; s.quantity = Mathf.Min(max, quantity); s.UpdateUI(); quantity -= s.quantity; if (quantity <= 0) return; }
            foreach (var s in inventorySlots)
            { if (s.itemSO != null) continue; s.itemSO = itemSO; s.quantity = Mathf.Min(max, quantity); s.UpdateUI(); quantity -= s.quantity; if (quantity <= 0) return; }
        }
        else
        {
            foreach (var s in inventorySlots)
            { if (s.itemSO != itemSO || s.quantity >= itemSO.stackSize) continue; int add = Mathf.Min(itemSO.stackSize - s.quantity, quantity); s.quantity += add; quantity -= add; s.UpdateUI(); if (quantity <= 0) return; }
            foreach (var s in inventorySlots)
            { if (s.itemSO != null) continue; s.itemSO = itemSO; s.quantity = Mathf.Min(itemSO.stackSize, quantity); s.UpdateUI(); return; }
        }

        if (quantity > 0) DropLoot(itemSO, quantity);
    }

    public void RemoveItem(ItemSO itemSO, int quantity)
    {
        foreach (var s in inventorySlots)
        {
            if (s.itemSO != itemSO) continue;
            if (s.quantity > quantity) { s.quantity -= quantity; s.UpdateUI(); return; }
            quantity -= s.quantity; s.itemSO = null; s.quantity = 0; s.UpdateUI();
        }
    }

    public void DropItem(InventorySlot slot)  { DropLoot(slot.itemSO, 1); slot.quantity--; if (slot.quantity <= 0) slot.itemSO = null; slot.UpdateUI(); }
    public void DropItem(EquipmentSlot slot)  { DropLoot(slot.itemSO, 1); slot.quantity--; if (slot.quantity <= 0) slot.itemSO = null; slot.UpdateUI(); }

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

    public bool HasItem(ItemSO itemSO)      => inventorySlots.Any(s => s.itemSO == itemSO && s.quantity > 0);
    public int  GetItemCount(ItemSO itemSO) => inventorySlots.Where(s => s.itemSO == itemSO).Sum(s => s.quantity);
}

public enum ItemType { head, body, legs, mainHand, feet, relic, ore, coins, none }
