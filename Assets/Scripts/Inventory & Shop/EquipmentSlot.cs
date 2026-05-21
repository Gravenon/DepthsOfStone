using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, IPointerClickHandler
{
    public ItemSO itemSO;
    public int quantity;

    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text quantityText;

    [Header("Equipped Slots")]
    public EquippedSlot headSlot;
    public EquippedSlot bodySlot;
    public EquippedSlot legsSlot;
    public EquippedSlot weaponSlot;
    public EquippedSlot accessorySlot;
    public EquippedSlot feetSlot;

    private InventoryManager _inventoryManager;
    private static ShopManger _activeShop;

    private void Start()
    {
        _inventoryManager = GetComponentInParent<InventoryManager>();
        if (_inventoryManager == null) _inventoryManager = InventoryManager.Instance;
    }

    private void OnEnable()  => ShopKeeper.OnShopOpenClose += HandleShopStateChange;
    private void OnDisable() => ShopKeeper.OnShopOpenClose -= HandleShopStateChange;

    private void HandleShopStateChange(ShopManger shop, bool isOpen) => _activeShop = isOpen ? shop : null;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (quantity <= 0) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (_activeShop != null) _activeShop.SellItem(itemSO);
            else TryEquipGear();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (_inventoryManager != null) _inventoryManager.DropItem(this);
        }
    }

    private void TryEquipGear()
    {
        if (itemSO == null) return;
        EquippedSlot target = GetTargetSlot(itemSO.itemType);
        if (target == null) return;
        if (!target.EquipGear(itemSO)) return;
        quantity--;
        UpdateUI();
    }

    private EquippedSlot GetTargetSlot(ItemType type)
    {
        switch (type)
        {
            case ItemType.head:     return headSlot;
            case ItemType.body:     return bodySlot;
            case ItemType.legs:     return legsSlot;
            case ItemType.mainHand: return weaponSlot;
            case ItemType.relic:    return accessorySlot;
            case ItemType.feet:     return feetSlot;
            default:                return null;
        }
    }

    public void UpdateUI()
    {
        if (quantity <= 0) itemSO = null;

        bool hasItem = itemSO != null;
        itemImage.gameObject.SetActive(hasItem);
        if (hasItem) itemImage.sprite = itemSO.itemIcon;

        if (quantityText != null)
            quantityText.text = hasItem && quantity > 1 ? quantity.ToString() : "";
    }
}
