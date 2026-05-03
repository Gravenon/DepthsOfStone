using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MineLevel
{
    public string levelName;
    public MineConfig[] configs;
}

public class MineManager : MonoBehaviour
{
    [Header("Mine Levels")]
    public MineLevel[] levels;

    [Header("Info Panel")]
    public GameObject infoPanel;
    [SerializeField] private TMP_Text mineNameText;
    [SerializeField] private TMP_Text mineDescriptionText;
    [SerializeField] private TMP_Text enemiesCountText;
    [SerializeField] private TMP_Text oresText;

    [Header("Level Select")]
    public CanvasGroup levelSelectCanvasGroup;
    public GameObject levelSelectUI;

    [Header("Play Button")]
    [SerializeField] private Button playButton;
    [SerializeField] private SceneChanger sceneChanger;

    private int _selectedIndex = -1;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SetCanvasGroup(levelSelectCanvasGroup, false);
        if (levelSelectUI != null) levelSelectUI.SetActive(false);
        if (infoPanel     != null) infoPanel.SetActive(false);
        if (playButton    != null) playButton.interactable = false;
    }

    // Called by difficulty buttons: 0=Easy, 1=Medium, 2=Hard
    public void ShowInfoByIndex(int index)
    {
        if (levels == null || index < 0 || index >= levels.Length) return;
        _selectedIndex = index;
        PopulateInfoPanel(levels[index]);
        if (playButton != null) playButton.interactable = true;
    }

    // Called by the Play button
    public void PlaySelectedLevel()
    {
        if (_selectedIndex < 0 || _selectedIndex >= levels.Length)
        {
            Debug.LogWarning("[MineManager] No level selected.");
            return;
        }

        MineConfig[] configs = levels[_selectedIndex].configs;
        if (configs == null || configs.Length == 0)
        {
            Debug.LogWarning("[MineManager] Selected level has no configs.");
            return;
        }

        MineConfig chosen = System.Array.Find(configs, c => c != null && !string.IsNullOrEmpty(c.sceneName));
        if (chosen == null)
        {
            Debug.LogWarning("[MineManager] No config with a valid sceneName found.");
            return;
        }

        if (sceneChanger == null)
        {
            Debug.LogWarning("[MineManager] SceneChanger not assigned.");
            return;
        }

        sceneChanger.sceneName = chosen.sceneName;
        HideLevelSelect();
        sceneChanger.ChangeScene();
    }

    private void PopulateInfoPanel(MineLevel level)
    {
        if (level == null || level.configs == null || level.configs.Length == 0) return;

        foreach (MineConfig cfg in level.configs)
        {
            if (cfg == null) continue;

            if (mineNameText        != null) mineNameText.text        = "Name: " + cfg.MineName;
            if (mineDescriptionText != null) mineDescriptionText.text = cfg.itemDescription;
            if (enemiesCountText    != null) enemiesCountText.text    = $"Enemies: {cfg.enemiesCount}";

            if (oresText != null && cfg.ores != null && cfg.ores.Length > 0)
            {
                oresText.text = "Ores:";
                foreach (var ore in cfg.ores)
                {
                    if (ore?.orePrefab == null) continue;
                    oresText.text += $"\n  {ore.orePrefab.name}: {ore.minAmount}–{ore.maxAmount}";
                }
            }
        }

        if (infoPanel != null) infoPanel.SetActive(true);
    }

    public void ShowLevelSelect()
    {
        SetCanvasGroup(levelSelectCanvasGroup, true);
        if (levelSelectUI != null) levelSelectUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void HideLevelSelect()
    {
        SetCanvasGroup(levelSelectCanvasGroup, false);
        if (levelSelectUI != null) levelSelectUI.SetActive(false);
        Time.timeScale = 1f;
    }

    private static void SetCanvasGroup(CanvasGroup cg, bool visible)
    {
        if (cg == null) return;
        cg.alpha = visible ? 1f : 0f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }
}
