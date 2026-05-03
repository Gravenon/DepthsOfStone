using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [Header("UI References")]
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Image itemImage;
    [SerializeField] private ShopManger shopManager;
    [SerializeField] private ShopInfo shopInfo;

    public ItemSO itemSO { get; private set; }
    public int price { get; private set; }
    public float ransomProcentage { get; private set; }

    public void Initialize(ItemSO item, int itemPrice, float buybackRate)
    {
        itemSO = item;
        price = itemPrice;
        ransomProcentage = buybackRate;

        itemImage.sprite = item.itemIcon;
        priceText.text = itemPrice.ToString();
    }

    // Called by the Buy button in the UI
    public void OnBuyButtonClicked() => shopManager.TryBuyItem(itemSO, price);

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemSO != null) shopInfo.ShowItemInfo(itemSO);
    }

    public void OnPointerExit(PointerEventData eventData) => shopInfo.HideItemInfo();

    public void OnPointerMove(PointerEventData eventData)
    {
        if (itemSO != null) shopInfo.FollowMouse();
    }
}
