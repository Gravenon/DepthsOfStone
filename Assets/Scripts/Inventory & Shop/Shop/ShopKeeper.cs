using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShopKeeper : MonoBehaviour
{
    public static ShopKeeper currentShopKeeper;

    public Animator anim;

    [SerializeField] private CanvasGroup shopCanvasGroup;
    [SerializeField] private ShopManger shopManager;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference cancelAction;

    [SerializeField] private List<ShopItems> shopItems;
    [SerializeField] private List<ShopItems> shopWeapons;
    [SerializeField] private List<ShopItems> shopArmour;

    [Header("Settings trade")]
    [SerializeField] private bool onlySell = false; 

    public static event Action<ShopManger, bool> OnShopOpenClose;
    private bool playerInRange;
    private bool isShopOpen;

    public bool OnlySell => onlySell;


    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed += OnInteract;
        }

        if (cancelAction != null)
        {
            cancelAction.action.performed += OnCancel;
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteract;
        }

        if (cancelAction != null)
        {
            cancelAction.action.performed -= OnCancel;
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (isShopOpen)
        {
            CloseShop();
            return;
        }

        if (!playerInRange)
            return;
        
        OpenShop();
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (!isShopOpen)
            return;
        
        CloseShop();
    }

    private void OpenShop()
    {
        Time.timeScale = 0;
        currentShopKeeper = this;
        isShopOpen = true;
        OnShopOpenClose?.Invoke(shopManager, true);
        shopCanvasGroup.alpha = 1;
        shopCanvasGroup.blocksRaycasts = true;
        shopCanvasGroup.interactable = true;
        OpenItemShop();
    }

    private void CloseShop()
    {
        Time.timeScale = 1;
        isShopOpen = false;
        currentShopKeeper = null;
        OnShopOpenClose?.Invoke(shopManager, false);
        shopCanvasGroup.alpha = 0;
        shopCanvasGroup.blocksRaycasts = false;
        shopCanvasGroup.interactable = false;
    }

    public void OpenItemShop()
    {
        shopManager.PopulateShopItem(shopItems);
    }

    public void OpenWeaponShop()
    {
        shopManager.PopulateShopItem(shopWeapons);
    }

    public void OpenArmourShop()
    {
        shopManager.PopulateShopItem(shopArmour);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("playerInRange", true);
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("playerInRange", false);
            playerInRange = false;
        }
    }
}
