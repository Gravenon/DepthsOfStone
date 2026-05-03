using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // NOT DontDestroyOnLoad — inventory belongs to a specific gameplay session.
            // All data is restored from the save file via LoadData() on scene load.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach (var slot in inventorySlots)
        {
            slot.UpdateUI();
        }
    }

    private void OnEnable()
    {
        Loot.OnItemLooted += AddItem;

        if (toggleInventoryAction != null)
        {
            toggleInventoryAction.action.performed += OnToggleInventory;
        }
    }

    private void OnDisable()
    {
        Loot.OnItemLooted -= AddItem;

        if (toggleInventoryAction != null)
        {
            toggleInventoryAction.action.performed -= OnToggleInventory;
        }
    }

    public void LoadData(GameData data)
    {
        // Clear inventory
        foreach (var slot in inventorySlots)
        {
            slot.itemSO = null;
            slot.quantity = 0;
            slot.UpdateUI();
        }

        // Clear equipment panel
        foreach (var slot in equimentSlot)
        {
            slot.itemSO = null;
            slot.quantity = 0;
            slot.UpdateUI();
        }

        // Clear equipped slots
        EquippedSlot[] equippedSlots = GetComponentsInChildren<EquippedSlot>(true);
        foreach (var slot in equippedSlots)
            slot.ClearSlot();

        // Restore coins
        coins = data.coins;
        if (coinText != null)
            coinText.text = coins.ToString();

        // Item restoration requires ItemSOLibrary from the gameplay scene.
        // If it's absent (main menu), items will be restored on next scene load.
        ItemSOLibrary library = FindFirstObjectByType<ItemSOLibrary>();
        if (library == null) return;

        if (data.inventoryItems != null)
            foreach (var saved in data.inventoryItems)
            {
                ItemSO item = System.Array.Find(library.itemSOs, so => so != null && so.itemName == saved.itemName);
                if (item != null) AddItem(item, saved.quantity);
            }

        if (data.equipmentItems != null)
            foreach (var saved in data.equipmentItems)
            {
                ItemSO item = System.Array.Find(library.itemSOs, so => so != null && so.itemName == saved.itemName);
                if (item != null) AddItem(item, saved.quantity);
            }

        // Restore equipped visuals — stats are already handled by StatsManager.LoadData
        equippedSlots = GetComponentsInChildren<EquippedSlot>(true);
        if (data.equippedItems != null)
            foreach (var saved in data.equippedItems)
            {
                ItemSO item = System.Array.Find(library.itemSOs, so => so != null && so.itemName == saved.itemName);
                if (item == null) continue;
                EquippedSlot target = System.Array.Find(equippedSlots, s => (int)s.GetSlotItemType() == saved.itemTypeId);
                if (target != null) target.RestoreGearVisual(item);
            }
    }

    public void SaveData(ref GameData data)
    {
        if (this == null) return;

        data.coins = coins;

        var invList = new System.Collections.Generic.List<SerializedSlot>();
        foreach (var slot in inventorySlots)
            if (slot.itemSO != null && slot.quantity > 0)
                invList.Add(new SerializedSlot { itemName = slot.itemSO.itemName, quantity = slot.quantity });
        data.inventoryItems = invList.ToArray();

        var eqList = new System.Collections.Generic.List<SerializedSlot>();
        foreach (var slot in equimentSlot)
            if (slot.itemSO != null && slot.quantity > 0)
                eqList.Add(new SerializedSlot { itemName = slot.itemSO.itemName, quantity = slot.quantity });
        data.equipmentItems = eqList.ToArray();

        EquippedSlot[] equippedSlots = GetComponentsInChildren<EquippedSlot>(true);
        var wornList = new System.Collections.Generic.List<SerializedEquippedSlot>();
        foreach (var slot in equippedSlots)
        {
            ItemSO item = slot.GetEquippedItem();
            if (item != null)
                wornList.Add(new SerializedEquippedSlot { itemTypeId = (int)slot.GetSlotItemType(), itemName = item.itemName });
        }
        data.equippedItems = wornList.ToArray();
    }

    private void OnToggleInventory(InputAction.CallbackContext context)
    {
        if (!context.performed || EqipmentPanel == null)
        {
            return;
        }

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

        bool isEquipment = itemSO.itemType == ItemType.mainHand || itemSO.itemType == ItemType.head
            || itemSO.itemType == ItemType.body  || itemSO.itemType == ItemType.legs
            || itemSO.itemType == ItemType.feet  || itemSO.itemType == ItemType.relic;

        if (!isEquipment)
        {
            foreach (var slot in inventorySlots)
            {
                if (slot.itemSO == itemSO && slot.quantity < itemSO.stackSize)
                {
                    int availableSpace = itemSO.stackSize - slot.quantity;
                    int amountToAdd = Mathf.Min(availableSpace, quantity);


                    slot.quantity += amountToAdd;
                    quantity -= amountToAdd;

                    slot.UpdateUI();

                    if (quantity <= 0)
                        return;
                }
            }

            foreach (var slot in inventorySlots)
            {
                if (slot.itemSO == null)
                {
                    int amountToAdd = Mathf.Min(itemSO.stackSize, quantity);
                    slot.itemSO = itemSO;
                    slot.quantity = amountToAdd;
                    slot.UpdateUI();
                    return;
                }

            }
        }
        else
        {
            Debug.Log($"[InventoryManager] Adding equipment '{itemSO.itemName}' x{quantity} to equipment panel. Panel slots: {equimentSlot.Length}");
            foreach (var slot in equimentSlot)
            {
                if (slot.itemSO == itemSO && slot.quantity < itemSO.stackSize)
                {
                    int availableSpace = itemSO.stackSize - slot.quantity;
                    int amountToAdd = Mathf.Min(availableSpace, quantity);


                    slot.quantity += amountToAdd;
                    quantity -= amountToAdd;

                    slot.UpdateUI();

                    if (quantity <= 0)
                        return;
                }

            }

            foreach (var slot in equimentSlot)
            {
                if (slot.itemSO == null)
                {
                    int amountToAdd = Mathf.Min(itemSO.stackSize, quantity);
                    slot.itemSO = itemSO;
                    slot.quantity = amountToAdd;
                    slot.UpdateUI();
                    return;
                }

            }
        }

        if(quantity > 0)
        {
            Debug.LogWarning($"[InventoryManager] No space for '{itemSO.itemName}' — dropping to world.");
            DropLoot(itemSO, quantity);
        }
    }

    public void DropItem(InventorySlot slot)
    {
        DropLoot(slot.itemSO, 1);
        slot.quantity--;
        if(slot.quantity <= 0)
        {
            slot.itemSO = null;
        }
        slot.UpdateUI();
    }

       public void DropItem(EquipmentSlot slot)
    {
        DropLoot(slot.itemSO, 1);
        slot.quantity--;
        if(slot.quantity <= 0)
        {
            slot.itemSO = null;
        }
        slot.UpdateUI();
    }

    private void DropLoot(ItemSO itemSO, int quantity)
    {
        Loot loot = Instantiate(lootPrefab, player.position, Quaternion.identity).GetComponent<Loot>();
        loot.Initialize(itemSO, quantity);
    }



    public void UseItem(InventorySlot slot)
    {
        if (slot.itemSO != null && slot.quantity >= 0)
        {
            Debug.Log("Trying to use item: " + slot.itemSO.itemName);

            useItem.ApplayItemEffects(slot.itemSO);

            slot.quantity--;

            if (slot.quantity <= 0)
            {
                slot.itemSO = null;
            }
            slot.UpdateUI();
        }
    }

    public bool HasItem(ItemSO itemSO)
    {
        foreach (var slot in inventorySlots)
        {
            if(slot.itemSO == itemSO && slot.quantity > 0)
                return true;
        }
        return false;
    }
}


public enum ItemType
{
    consumable,
    crafting,
    head,
    body,
    legs,
    mainHand,
    feet,
    relic,
    ore,
    coins,
    none
}
