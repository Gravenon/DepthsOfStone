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
        // Регистрируемся до sceneLoaded — гарантирует, что LoadData нас найдёт.
        InventoryManager.RegisterSlot(this);
        if (slotImage != null) SetEmptyVisual();
    }

    private void OnDestroy() => InventoryManager.UnregisterSlot(this);

    private void Start()
    {
        inventoryManager = InventoryManager.Instance ?? FindAnyObjectByType<InventoryManager>();
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (slotImage == null) return;
        if (slotInUse && itemSO != null)
        {
            slotImage.sprite = itemSO.itemIcon;
            var c = slotImage.color; c.a = 1f; slotImage.color = c;
            if (emptySlotImage != null) emptySlotImage.enabled = false;
        }
        else SetEmptyVisual();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && slotInUse) UnequipGear();
    }

    public bool EquipGear(ItemSO newItem)
    {
        if (newItem == null) return false;
        if (itemType != ItemType.none && newItem.itemType != itemType) return false;
        if (slotInUse) UnequipGear();

        itemSO = newItem; slotInUse = true;
        itemSO.Use();
        UpdateUI();

        var im = InventoryManager.Instance ?? FindAnyObjectByType<InventoryManager>();
        im?.RegisterEquip((int)itemType, itemSO.itemName);
        return true;
    }

    public void UnequipGear()
    {
        if (!slotInUse || itemSO == null) return;

        inventoryManager ??= InventoryManager.Instance ?? FindAnyObjectByType<InventoryManager>();
        if (inventoryManager == null) { Debug.LogError("[EquippedSlot] InventoryManager not found!", this); return; }

        int typeId = (int)itemType;
        ItemSO removed = itemSO;

        itemSO = null; slotInUse = false;
        UpdateUI();

        removed.Unuse();
        inventoryManager.RegisterUnequip(typeId);
        inventoryManager.AddItem(removed, 1);
    }

    public void ClearSlot()      { itemSO = null; slotInUse = false; SetEmptyVisual(); }
    public void RestoreGearVisual(ItemSO item) { if (item == null) return; itemSO = item; slotInUse = true; UpdateUI(); }

    private void SetEmptyVisual()
    {
        if (slotImage != null) { var c = slotImage.color; c.a = 0f; slotImage.color = c; }
        if (emptySlotImage != null) emptySlotImage.enabled = true;
    }

    public ItemSO   GetEquippedItem()  => slotInUse ? itemSO : null;
    public ItemType GetSlotItemType()  => itemType;
}