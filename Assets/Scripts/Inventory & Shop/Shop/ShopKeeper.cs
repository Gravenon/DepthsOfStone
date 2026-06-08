using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShopKeeper : MonoBehaviour
{
    public static ShopKeeper currentShopKeeper;

    [SerializeField] private Animator anim;
    [SerializeField] private Animator ButtonAnim;
    [SerializeField] private CanvasGroup shopCanvasGroup;
    [SerializeField] private ShopManger shopManager;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference cancelAction;

    [Header("Shop Inventory")]
    [SerializeField] private List<ShopItems> shopItems;
    [SerializeField] private List<ShopItems> shopWeapons;
    [SerializeField] private List<ShopItems> shopArmour;

    [Header("Settings")]
    [SerializeField] private bool onlySell;

    public static event Action<ShopManger, bool> OnShopOpenClose;

    private bool _playerInRange;
    private bool _isShopOpen;

    public bool OnlySell => onlySell;


    private void Awake()
    {
        SetCanvasVisible(false);
        if (anim != null) anim.Play("Idle");
        else Debug.LogWarning("[ShopKeeper] Animator is not assigned!", this);
    }

    private void OnEnable()
    {
        if (interactAction != null) interactAction.action.performed += OnInteract;
        if (cancelAction != null) cancelAction.action.performed += OnCancel;
    }

    private void OnDisable()
    {
        if (interactAction != null) interactAction.action.performed -= OnInteract;
        if (cancelAction != null) cancelAction.action.performed -= OnCancel;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        _playerInRange = true;
        if (ButtonAnim != null) anim.SetBool("playerInRange", true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        _playerInRange = false;
        if (ButtonAnim != null) anim.SetBool("playerInRange", false);
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (_isShopOpen)
        {
            CloseShop(); 
            return;
        }
        if (_playerInRange) 
            OpenShop();
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (_isShopOpen) CloseShop();
    }


    private void OpenShop()
    {
        Time.timeScale = 0;
        currentShopKeeper = this;
        _isShopOpen = true;

        if (UIManager.Instance != null)
            UIManager.Instance.NotifyOpened(shopCanvasGroup, OnForcedClose);
        else
            SetCanvasVisible(true);

        OnShopOpenClose?.Invoke(shopManager, true);
        shopManager.SetAllShopItems(shopItems, shopWeapons, shopArmour);
        shopManager.RefreshInventoryDisplay();
        OpenItemShop();
    }

    private void CloseShop()
    {
        Time.timeScale = 1;
        currentShopKeeper = null;
        _isShopOpen = false;
        SetCanvasVisible(false);
        UIManager.Instance?.ForceClose();
        OnShopOpenClose?.Invoke(shopManager, false);
    }

    private void OnForcedClose()
    {
        Time.timeScale = 1;
        currentShopKeeper = null;
        _isShopOpen = false;
        OnShopOpenClose?.Invoke(shopManager, false);
    }

    private void SetCanvasVisible(bool visible)
    {
        shopCanvasGroup.alpha = visible ? 1 : 0;
        shopCanvasGroup.blocksRaycasts = visible;
        shopCanvasGroup.interactable = visible;
    }

    public void OpenItemShop() => shopManager.PopulateShopItem(shopItems);
    public void OpenWeaponShop() => shopManager.PopulateShopItem(shopWeapons);
    public void OpenArmourShop() => shopManager.PopulateShopItem(shopArmour);
}
