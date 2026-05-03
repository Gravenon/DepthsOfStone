using UnityEngine;

/// <summary>
/// Bridges UI tab buttons to the active ShopKeeper's page switching methods.
/// Attach this to a persistent UI object inside the shop canvas.
/// </summary>
public class ShopButtonToggles : MonoBehaviour
{
    public void OpenItemShop()   => ShopKeeper.currentShopKeeper?.OpenItemShop();
    public void OpenWeaponShop() => ShopKeeper.currentShopKeeper?.OpenWeaponShop();
    public void OpenArmourShop() => ShopKeeper.currentShopKeeper?.OpenArmourShop();
}
