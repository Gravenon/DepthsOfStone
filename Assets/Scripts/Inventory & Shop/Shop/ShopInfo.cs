using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShopInfo : MonoBehaviour
{
    [SerializeField] private CanvasGroup infoPanel;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;

    [Header("Stat Fields")]
    [SerializeField] private TMP_Text[] statTexts;

    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void ShowItemInfo(ItemSO item)
    {
        infoPanel.alpha = 1;
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.itemDescription;

        var stats = BuildStatList(item);

        for (int i = 0; i < statTexts.Length; i++)
        {
            if (i < stats.Count)
            {
                statTexts[i].text = stats[i];
                statTexts[i].gameObject.SetActive(true);
            }
            else
            {
                statTexts[i].text = "";
                statTexts[i].gameObject.SetActive(false);
            }
        }
    }

    public void HideItemInfo()
    {
        infoPanel.alpha = 0;
        itemNameText.text = "";
        itemDescriptionText.text = "";
    }

    public void FollowMouse()
    {
        if (Pointer.current == null) return;
        if (_rect == null) _rect = GetComponent<RectTransform>();
        if (_rect == null) return;
        _rect.position = (Vector3)Pointer.current.position.ReadValue() + new Vector3(10f, -10f, 0f);
    }

    private static List<string> BuildStatList(ItemSO item)
    {
        var stats = new List<string>();
        if (item.currentHealth > 0) stats.Add("Health: " + item.currentHealth);
        if (item.maxHealth > 0) stats.Add("Max Health: " + item.maxHealth);
        if (item.speed > 0) stats.Add("Speed: " + item.speed);
        if (item.damage > 0) stats.Add("Damage: " + item.damage);
        if (item.duration > 0) stats.Add("Duration: " + item.duration);
        return stats;
    }
}
