using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Hades-style slot selection:
/// • Filled slot  → click to continue that world immediately.
/// • Empty slot   → click to open the inline name form; confirm to create a new world.
/// </summary>
public class SaveSlotMenu : MonoBehaviour
{
    public static SaveSlotMenu Instance { get; private set; }

    [Header("Scene Settings")]
    [SerializeField] private string gameplaySceneName = "VilageDwarfs";

    [Header("New Game Form (child panel, hidden by default)")]
    [SerializeField] private GameObject newGamePanel;
    [SerializeField] private TMP_InputField worldNameInput;
    [SerializeField] private TMP_InputField playerNameInput;

    private SaveSlot[] saveSlots;

    private void Awake()
    {
        Instance = this;
        saveSlots = GetComponentsInChildren<SaveSlot>(true);
    }

    private void OnEnable()
    {
        if (newGamePanel != null) newGamePanel.SetActive(false);
        RefreshSlots();
    }

    // ---------------------------------------------------------------
    // Called by each SaveSlot button OnClick — pass (this) from the slot.
    // ---------------------------------------------------------------
    public void OnSaveSlotSelected(SaveSlot slot)
    {
        DataPersistenceeManager.instance.ChangeSelectedProfileID(slot.GetProfileID());

        if (DataPersistenceeManager.instance.HasActiveGameData)
        {
            // Filled slot — continue this world straight away.
            SceneManager.LoadScene(gameplaySceneName);
        }
        else
        {
            // Empty slot — show the inline name form.
            if (worldNameInput != null)  worldNameInput.text  = "";
            if (playerNameInput != null) playerNameInput.text = "";
            if (newGamePanel != null)    newGamePanel.SetActive(true);
        }
    }

    // Called by the "Create" / "Confirm" button inside newGamePanel.
    public void OnCreateNewGameConfirmed()
    {
        string worldName  = (worldNameInput  != null && !string.IsNullOrWhiteSpace(worldNameInput.text))
                            ? worldNameInput.text.Trim()  : "World";
        string playerName = (playerNameInput != null && !string.IsNullOrWhiteSpace(playerNameInput.text))
                            ? playerNameInput.text.Trim() : "Player";

        DataPersistenceeManager.instance.NewGame(worldName, playerName);
        SceneManager.LoadScene(gameplaySceneName);
    }

    // Called by the "Back" / "Cancel" button inside newGamePanel.
    public void OnCreateNewGameCancelled()
    {
        if (newGamePanel != null) newGamePanel.SetActive(false);
    }

    // Called by the Delete button on each SaveSlot — pass (this) from the slot.
    public void OnDeleteSlotClicked(SaveSlot slot)
    {
        DataPersistenceeManager.instance.DeleteProfile(slot.GetProfileID());
        RefreshSlots();
    }

    // ---------------------------------------------------------------

    private void RefreshSlots()
    {
        Dictionary<string, GameData> allProfilesData = DataPersistenceeManager.instance.GetAllProfilesGameData();

        foreach (SaveSlot slot in saveSlots)
        {
            GameData profileData = null;
            allProfilesData.TryGetValue(slot.GetProfileID(), out profileData);
            slot.SetData(profileData);
            slot.SetInteractable(true); // every slot is always clickable
        }
    }
}
