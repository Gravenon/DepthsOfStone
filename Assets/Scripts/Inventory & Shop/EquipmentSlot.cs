using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IPointerClickHandler
{
    public ItemSO itemSO;
    public int quantity;

    public Image itemImage;

    public EquippedSlot headSlot, bodySlot, legsSlot, weaponSlot, accessorySlot, feetSlot;
    
    private InventoryManager inventoryManager;
    private static ShopManger activeShop;
 

    private void Start()
    {
        inventoryManager = GetComponentInParent<InventoryManager>();
    }

    private void OnEnable()
    {
        ShopKeeper.OnShopOpenClose += HendleShopStateChange;
    }

    private void OnDisable()
    {
        ShopKeeper.OnShopOpenClose -= HendleShopStateChange;
    }

    private void HendleShopStateChange(ShopManger shopManager, bool isOpen)
    {
        activeShop = isOpen ? shopManager : null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (quantity > 0)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (activeShop != null)
                {
                    activeShop.SellItem(itemSO);
                    quantity--;
                    UpdateUI();
                }
                else
                {
                    // Check if the item is a piece of equipment and if the player can equip it
                    EquipGear();
                    UpdateUI();
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                inventoryManager.DropItem(this);
            }
        }

    }

    private void EquipGear()
    {
        if (itemSO == null)
            return;

        EquippedSlot targetSlot = GetTargetSlot(itemSO.itemType);
        if (targetSlot == null)
            return;

        bool equipped = targetSlot.EquipGear(itemSO);
        if (!equipped)
            return;

        quantity--;
        UpdateUI();
    }

    private EquippedSlot GetTargetSlot(ItemType type)
    {
        switch (type)
        {
            case ItemType.head:
                return headSlot;
            case ItemType.body:
                return bodySlot;
            case ItemType.legs:
                return legsSlot;
            case ItemType.mainHand:
                return weaponSlot;
            case ItemType.relic:
                return accessorySlot;
            case ItemType.feet:
                return feetSlot;
            default:
                return null;
        }
    }

    public void UpdateUI()
    {
        if (quantity <= 0)
            itemSO = null;

        if (itemSO != null)
        {
            itemImage.sprite = itemSO.itemIcon;
            itemImage.gameObject.SetActive(true);
        }
        else
        {
            itemImage.gameObject.SetActive(false);
        }

    }
}
