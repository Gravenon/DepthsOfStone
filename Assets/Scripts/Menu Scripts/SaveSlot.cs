using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileID = "";

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI saveTimeText;
    [SerializeField] private TextMeshProUGUI nameWorldText;
    [SerializeField] private TextMeshProUGUI namePlayerText;
    [SerializeField] private TextMeshProUGUI countNightText;
    [SerializeField] private Button deleteButton;

    public void SetData(GameData data)
    {
        if (data == null)
        {
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
            if (deleteButton != null) deleteButton.gameObject.SetActive(false);
        }
        else
        {
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);
            if (deleteButton != null) deleteButton.gameObject.SetActive(true);

            saveTimeText.text = data.saveTime;
            nameWorldText.text = data.nameWorld;
            namePlayerText.text = data.namePlayer;
            countNightText.text = data.countNight.ToString();
        }
    }

    public string GetProfileID()
    {
        return this.profileID;
    }

    public string GetNameWorld()
    {
        return nameWorldText.text;
    }

    public string GetNamePlayer()
    {
        return namePlayerText.text;
    }

    public void SetInteractable(bool isInteractable)
    {
        GetComponent<Button>().interactable = isInteractable;
    }

}
