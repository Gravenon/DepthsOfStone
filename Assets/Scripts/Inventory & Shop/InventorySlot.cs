using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public ItemSO itemSO;
    public int quantity;

    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text quantityText;

    private InventoryManager _inventoryManager;
    private static ShopManger _activeShop;

    private void Start() => _inventoryManager = GetComponentInParent<InventoryManager>();

    private void OnEnable()  => ShopKeeper.OnShopOpenClose += HandleShopStateChange;
    private void OnDisable() => ShopKeeper.OnShopOpenClose -= HandleShopStateChange;

    private void HandleShopStateChange(ShopManger shop, bool isOpen)
    {
        _activeShop = isOpen ? shop : null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (quantity <= 0) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (_activeShop != null)
            {
                _activeShop.SellItem(itemSO);
            }
            else
            {
                // Consumables that restore health can only be used below max health
                if (itemSO.currentHealth > 0 && StatsManager.Instance.currentHealth >= StatsManager.Instance.maxHealth)
                    return;
                if (itemSO.itemType != ItemType.ore)
                    _inventoryManager.UseItem(this);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            _inventoryManager.DropItem(this);
        }
    }

    public void UpdateUI()
    {
        if (quantity <= 0) itemSO = null;

        bool hasItem = itemSO != null;
        itemImage.gameObject.SetActive(hasItem);
        itemImage.sprite  = hasItem ? itemSO.itemIcon : null;
        quantityText.text = hasItem ? quantity.ToString() : "";
    }
}
