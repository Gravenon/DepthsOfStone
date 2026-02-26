using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IPointerClickHandler
{
    public ItemSO itemSO;
    public int quantity;

    public Image itemImage;

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
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                inventoryManager.DropItem(this);
            }
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
