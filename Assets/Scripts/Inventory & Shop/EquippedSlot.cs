using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EquippedSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image slotImage;
    [SerializeField] private Image emptySlotImage;
    // [SerializeField] private Image playerDisplayImage;

    [SerializeField] private ItemType itemType;

    private ItemSO itemSO;
    private bool slotInUse;
    
    private InventoryManager inventoryManager;


    private void Start()
    {
        inventoryManager = InventoryManager.Instance ?? FindAnyObjectByType<InventoryManager>();

        if (inventoryManager == null)
            Debug.LogError("[EquippedSlot] InventoryManager not found in scene!", this);

        SetEmptyVisual();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && slotInUse)
        {
            UnequipGear();
        }
    }

    public bool EquipGear(ItemSO newItemSO)
    {
        if (newItemSO == null)
            return false;

        if (itemType != ItemType.none && newItemSO.itemType != itemType)
            return false;

        if(slotInUse)
            UnequipGear();

        if (slotImage == null || emptySlotImage == null)
        {
            Debug.LogError("EquippedSlot: slotImage или slotName не назначены в Inspector.");
            return false;
        }

        itemSO = newItemSO;
        slotImage.sprite = itemSO.itemIcon;
        
        Color color = slotImage.color;
        color.a = 1f;
        slotImage.color = color;
        
        emptySlotImage.enabled = false;

        //Displaying items on the user 
        // playerDisplayImage.sprite = itemSprite;


        itemSO.Use();

        slotInUse = true;
        return true;
    }

    public void UnequipGear()
    {
        if (!slotInUse || itemSO == null)
            return;

        if (inventoryManager == null)
            inventoryManager = InventoryManager.Instance ?? FindAnyObjectByType<InventoryManager>();

        if (inventoryManager == null)
        {
            Debug.LogError("[EquippedSlot] Cannot unequip — InventoryManager is null!", this);
            return;
        }

        Debug.Log($"[EquippedSlot] Unequipping '{itemSO.itemName}', returning to inventory.");
        inventoryManager.AddItem(itemSO, 1);

        itemSO.Unuse();


        itemSO = null;
        slotInUse = false;
        SetEmptyVisual();
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
        if (itemSO == null) return;

        itemSO = null;
        slotInUse = false;
        SetEmptyVisual();
    }


    public ItemSO GetEquippedItem() => slotInUse ? itemSO : null;

    /// <summary>Returns the item type this slot accepts.</summary>
    public ItemType GetSlotItemType() => itemType;
    
    public void RestoreGearVisual(ItemSO item)
    {
        if (item == null) return;
        itemSO = item;
        slotImage.sprite = item.itemIcon;
        Color c = slotImage.color;
        c.a = 1f;
        slotImage.color = c;
        emptySlotImage.enabled = false;
        slotInUse = true;
    }
}