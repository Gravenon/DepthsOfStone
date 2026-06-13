using UnityEngine;

public class ShopButtonToggles : MonoBehaviour
{
    public void OpenItemShop()   => ShopKeeper.currentShopKeeper?.OpenItemShop();
    public void OpenWeaponShop() => ShopKeeper.currentShopKeeper?.OpenWeaponShop();
    public void OpenArmourShop() => ShopKeeper.currentShopKeeper?.OpenArmourShop();
}
