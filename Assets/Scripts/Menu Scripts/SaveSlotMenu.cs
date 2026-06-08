using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void OnSaveSlotSelected(SaveSlot slot)
    {
        DataPersistenceeManager.instance.ChangeSelectedProfileID(slot.GetProfileID());

        if (DataPersistenceeManager.instance.HasActiveGameData)
        {
            DataPersistenceeManager.SuppressNextSave = true;
            SceneManager.LoadScene(gameplaySceneName);
        }
        else
        {
            if (worldNameInput != null) worldNameInput.text = "";
            if (playerNameInput != null) playerNameInput.text = "";
            if (newGamePanel != null) newGamePanel.SetActive(true);
        }
    }

    public void OnCreateNewGameConfirmed()
    {
        string worldName = (worldNameInput != null && !string.IsNullOrWhiteSpace(worldNameInput.text))
            ? worldNameInput.text.Trim() : "World";
        string playerName = (playerNameInput != null && !string.IsNullOrWhiteSpace(playerNameInput.text))
            ? playerNameInput.text.Trim() : "Player";

        DataPersistenceeManager.instance.NewGame(worldName, playerName);
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnCreateNewGameCancelled()
    {
        if (newGamePanel != null) newGamePanel.SetActive(false);
    }

    public void OnDeleteSlotClicked(SaveSlot slot)
    {
        DataPersistenceeManager.instance.DeleteProfile(slot.GetProfileID());
        RefreshSlots();
    }

    private void RefreshSlots()
    {
        Dictionary<string, GameData> allProfilesData = DataPersistenceeManager.instance.GetAllProfilesGameData();

        foreach (SaveSlot slot in saveSlots)
        {
            GameData profileData = null;
            allProfilesData.TryGetValue(slot.GetProfileID(), out profileData);
            slot.SetData(profileData);
        slot.SetInteractable(true);
    }
    }
}
