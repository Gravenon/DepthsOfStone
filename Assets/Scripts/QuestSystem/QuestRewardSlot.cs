using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestRewardSlot : MonoBehaviour
{
    public Image  icon;
    public TMP_Text rewardQuantity;

    public void DisplayReward(Sprite sprite, int quantity)
    {
        icon.sprite = sprite;
        rewardQuantity.text = quantity.ToString();
    }
}
