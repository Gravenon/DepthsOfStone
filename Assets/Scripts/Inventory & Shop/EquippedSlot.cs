using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EquippedSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image slotImage;
    [SerializeField] private TMP_Text slotName;
    // [SerializeField] private Image playerDisplayImage;

    [SerializeField] private ItemType itemType;

    private ItemSO itemSO;
    private bool slotInUse;

    [SerializeField] private Sprite emptySlotSprite;  

    private InventoryManager inventoryManager;
    private ItemSOLibrary itemLibrary;


    private void Start()
    {
        inventoryManager = InventoryManager.Instance;
        itemLibrary = FindAnyObjectByType<ItemSOLibrary>();
        if (inventoryManager == null && itemLibrary == null)
        {
            inventoryManager = FindAnyObjectByType<InventoryManager>();
            itemLibrary = FindAnyObjectByType<ItemSOLibrary>();

        }

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

        if (slotImage == null || slotName == null)
        {
            Debug.LogError("EquippedSlot: slotImage или slotName не назначены в Inspector.");
            return false;
        }

        itemSO = newItemSO;
        slotImage.sprite = itemSO.itemIcon;
        
        Color color = slotImage.color;
        color.a = 1f;
        slotImage.color = color;
        
        slotName.enabled = false;

        //Displaying items on the user 
        // playerDisplayImage.sprite = itemSprite;


        for(int i = 0; i < itemLibrary.itemSOs.Length; i++)
        {
            if(itemLibrary.itemSOs[i] == itemSO)
            {
                itemLibrary.itemSOs[i].Use();
                // break;
            }
        }

        slotInUse = true;
        return true;
    }

    public void UnequipGear()
    {
        if (!slotInUse || itemSO == null)
            return;

        if (inventoryManager == null)
        {
            inventoryManager = InventoryManager.Instance;
            if (inventoryManager == null)
                inventoryManager = FindAnyObjectByType<InventoryManager>();
        }

        inventoryManager.AddItem(itemSO, 1);

        for(int i = 0; i < itemLibrary.itemSOs.Length; i++)
        {
            if(itemLibrary.itemSOs[i] == itemSO)
            {
                itemLibrary.itemSOs[i].Unuse();
                // break;
            }
        }


        itemSO = null;
        slotInUse = false;
        SetEmptyVisual();
    }

    private void SetEmptyVisual()
    {
        slotImage.sprite = emptySlotSprite;

        Color color = slotImage.color;
        color.a = 0f;
        slotImage.color = color;

        slotName.enabled = true;
    }
}