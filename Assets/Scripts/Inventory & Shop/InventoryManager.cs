using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;


    public GameObject EqipmentPanel;
    public InputActionReference toggleInventoryAction;

    public InventorySlot[] inventorySlots;
    public EquipmentSlot[] equimentSlot;

    public UseItem useItem;
    public int coins;
    public TMP_Text coinText;
    public GameObject lootPrefab;
    public Transform player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach (var slot in inventorySlots)
        {
            slot.UpdateUI();
        }
    }

    private void OnEnable()
    {
        Loot.OnItemLooted += AddItem;

        if (toggleInventoryAction != null)
        {
            toggleInventoryAction.action.performed += OnToggleInventory;
        }
    }

    private void OnDisable()
    {
        Loot.OnItemLooted -= AddItem;

        if (toggleInventoryAction != null)
        {
            toggleInventoryAction.action.performed -= OnToggleInventory;
        }
    }

    private void OnToggleInventory(InputAction.CallbackContext context)
    {
        if (!context.performed || EqipmentPanel == null)
        {
            return;
        }

        EqipmentPanel.SetActive(!EqipmentPanel.activeSelf);
    }

    public void AddItem(ItemSO itemSO, int quantity)
    {
        if (itemSO.itemType == ItemType.coins)
        {
            coins += quantity;
            coinText.text = coins.ToString();
            return;
        }

        if ( itemSO.itemType != ItemType.mainHand && itemSO.itemType != ItemType.head && itemSO.itemType != ItemType.body && itemSO.itemType != ItemType.legs && itemSO.itemType != ItemType.feet)
        {
            foreach (var slot in inventorySlots)
            {
                if (slot.itemSO == itemSO && slot.quantity < itemSO.stackSize)
                {
                    int availableSpace = itemSO.stackSize - slot.quantity;
                    int amountToAdd = Mathf.Min(availableSpace, quantity);


                    slot.quantity += amountToAdd;
                    quantity -= amountToAdd;

                    slot.UpdateUI();

                    if (quantity <= 0)
                        return;
                }
            }

            foreach (var slot in inventorySlots)
            {
                if (slot.itemSO == null)
                {
                    int amountToAdd = Mathf.Min(itemSO.stackSize, quantity);
                    slot.itemSO = itemSO;
                    slot.quantity = quantity;
                    slot.UpdateUI();
                    return;
                }

            }
        }
        else
        {
            foreach (var slot in equimentSlot)
            {
                if (slot.itemSO == itemSO && slot.quantity < itemSO.stackSize)
                {
                    int availableSpace = itemSO.stackSize - slot.quantity;
                    int amountToAdd = Mathf.Min(availableSpace, quantity);


                    slot.quantity += amountToAdd;
                    quantity -= amountToAdd;

                    slot.UpdateUI();

                    if (quantity <= 0)
                        return;
                }

            }

            foreach (var slot in equimentSlot)
            {
                if (slot.itemSO == null)
                {
                    int amountToAdd = Mathf.Min(itemSO.stackSize, quantity);
                    slot.itemSO = itemSO;
                    slot.quantity = quantity;
                    slot.UpdateUI();
                    return;
                }

            }
        }

        if(quantity > 0)
            DropLoot(itemSO, quantity);
    }

    public void DropItem(InventorySlot slot)
    {
        DropLoot(slot.itemSO, 1);
        slot.quantity--;
        if(slot.quantity <= 0)
        {
            slot.itemSO = null;
        }
        slot.UpdateUI();
    }

       public void DropItem(EquipmentSlot slot)
    {
        DropLoot(slot.itemSO, 1);
        slot.quantity--;
        if(slot.quantity <= 0)
        {
            slot.itemSO = null;
        }
        slot.UpdateUI();
    }

    private void DropLoot(ItemSO itemSO, int quantity)
    {
        Loot loot = Instantiate(lootPrefab, player.position, Quaternion.identity).GetComponent<Loot>();
        loot.Initialize(itemSO, quantity);
    }



    public void UseItem(InventorySlot slot)
    {
        if (slot.itemSO != null && slot.quantity >= 0)
        {
            Debug.Log("Trying to use item: " + slot.itemSO.itemName);

            useItem.ApplayItemEffects(slot.itemSO);

            slot.quantity--;

            if (slot.quantity <= 0)
            {
                slot.itemSO = null;
            }
            slot.UpdateUI();
        }
    }

    public bool HasItem(ItemSO itemSO)
    {
        foreach (var slot in inventorySlots)
        {
            if(slot.itemSO == itemSO && slot.quantity > 0)
                return true;
        }
        return false;
    }
}


public enum ItemType
{
    consumable,
    crafting,
    head,
    body,
    legs,
    mainHand,
    feet,
    relic,
    ore,
    coins,
    none
}
