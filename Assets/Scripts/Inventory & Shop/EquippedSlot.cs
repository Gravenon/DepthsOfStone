using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquippedSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image slotImage;
    [SerializeField] private Image emptySlotImage;
    [SerializeField] private ItemType itemType;

    private ItemSO itemSO;
    private bool slotInUse;
    private InventoryManager inventoryManager;

    private void Awake()
    {
        // Awake runs before sceneLoaded → LoadData, so this safely initialises the empty state.
        // Does NOT reset itemSO/slotInUse, so UpdateUI() in Start() correctly reflects any
        // data already set by RestoreGearVisual() on inactive objects.
        if (slotImage != null)
            SetEmptyVisual();
    }

    private void Start()
    {
        inventoryManager = InventoryManager.Instance ?? FindAnyObjectByType<InventoryManager>();
        if (inventoryManager == null)
            Debug.LogError("[EquippedSlot] InventoryManager not found in scene!", this);

        // Start() runs after sceneLoaded → LoadData() → RestoreGearVisual().
        // UpdateUI() reads itemSO/slotInUse and displays the restored equipment correctly.
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (slotImage == null || emptySlotImage == null) return;

        if (slotInUse && itemSO != null)
        {
            slotImage.sprite = itemSO.itemIcon;
            Color c = slotImage.color;
            c.a = 1f;
            slotImage.color = c;
            emptySlotImage.enabled = false;
        }
        else
        {
            SetEmptyVisual();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && slotInUse)
            UnequipGear();
    }

    public bool EquipGear(ItemSO newItemSO)
    {
        if (newItemSO == null) return false;
        if (itemType != ItemType.none && newItemSO.itemType != itemType) return false;
        if (slotImage == null || emptySlotImage == null)
        {
            Debug.LogError("[EquippedSlot] slotImage or emptySlotImage not assigned in Inspector.", this);
            return false;
        }

        if (slotInUse) UnequipGear();

        itemSO = newItemSO;
        slotInUse = true;
        itemSO.Use();
        UpdateUI();
        return true;
    }

    public void UnequipGear()
    {
        if (!slotInUse || itemSO == null) return;

        if (inventoryManager == null)
            inventoryManager = InventoryManager.Instance ?? FindAnyObjectByType<InventoryManager>();

        if (inventoryManager == null)
        {
            Debug.LogError("[EquippedSlot] Cannot unequip — InventoryManager is null!", this);
            return;
        }

        itemSO.Unuse();
        inventoryManager.AddItem(itemSO, 1);

        itemSO = null;
        slotInUse = false;
        UpdateUI();
    }

    private void SetEmptyVisual()
    {
        Color color = slotImage.color;
        color.a = 0f;
        slotImage.color = color;
        emptySlotImage.enabled = true;
    }

    public void ClearSlot()
    {
        itemSO = null;
        slotInUse = false;
        if (slotImage != null) SetEmptyVisual();
    }

    /// <summary>
    /// Restores the slot from a save file — visuals only, no Use() call.
    /// Stats are already restored directly by StatsManager.LoadData.
    /// </summary>
    public void RestoreGearVisual(ItemSO item)
    {
        if (item == null) return;
        itemSO = item;
        slotInUse = true;
        if (slotImage != null) UpdateUI();
    }

    public ItemSO GetEquippedItem() => slotInUse ? itemSO : null;
    public ItemType GetSlotItemType() => itemType;
}