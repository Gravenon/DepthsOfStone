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

    [Header("Play Button")]
    [SerializeField] private Button playButton;
    [SerializeField] private SceneChanger sceneChanger;

    private int _selectedIndex = -1;
    private CanvasGroup _infoCG;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (levelSelectCanvasGroup != null)
            UIManager.SetVisible(levelSelectCanvasGroup, false);

        if (infoPanel != null)
        {
            _infoCG = infoPanel.GetComponent<CanvasGroup>();
            if (_infoCG == null) _infoCG = infoPanel.AddComponent<CanvasGroup>();
            UIManager.SetVisible(_infoCG, false);
        }

        if (playButton != null) playButton.interactable = false;
    }

    public void ShowInfoByIndex(int index)
    {
        if (levels == null || index < 0 || index >= levels.Length) return;
        _selectedIndex = index;
        PopulateInfoPanel(levels[index]);
        if (playButton != null) playButton.interactable = true;
    }

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
        if (chosen == null) { Debug.LogWarning("[MineManager] No config with a valid sceneName found."); return; }
        if (sceneChanger == null) { Debug.LogWarning("[MineManager] SceneChanger not assigned."); return; }

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
            if (mineNameText != null) mineNameText.text = cfg.MineName;
            if (mineDescriptionText != null) mineDescriptionText.text = cfg.MineDescription;
            if (enemiesCountText != null) enemiesCountText.text = $"Enemies: {cfg.enemiesCount}";

            if (oresText != null && cfg.ores != null && cfg.ores.Length > 0)
            {
                //oresText.text = "Ores:";
                foreach (var ore in cfg.ores)
                {
                    if (ore?.orePrefab == null) continue;
                    Ore oreData = ore.orePrefab.GetComponent<Ore>();

                    if (oreData != null)
                    {
                        oresText.text = $"\n  {oreData.OreName}: {ore.minAmount}–{ore.maxAmount}";
                    }
                }
            }
        }

        if (_infoCG != null) UIManager.SetVisible(_infoCG, true);
    }

    public void ShowLevelSelect()
    {
        if (levelSelectCanvasGroup == null) return;
        if (UIManager.Instance != null)
            UIManager.Instance.NotifyOpened(levelSelectCanvasGroup, () => Time.timeScale = 1f);
        else
            UIManager.SetVisible(levelSelectCanvasGroup, true);
        Time.timeScale = 0f;
    }

    public void HideLevelSelect()
    {
        if (levelSelectCanvasGroup != null) UIManager.SetVisible(levelSelectCanvasGroup, false);
        if (_infoCG != null) UIManager.SetVisible(_infoCG, false);
        if (playButton != null) playButton.interactable = false;
        _selectedIndex = -1;
        Time.timeScale = 1f;
        UIManager.Instance?.ForceClose();
    }
}
