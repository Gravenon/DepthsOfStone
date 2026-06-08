using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingsTabManager : MonoBehaviour
{
    [System.Serializable]
    public class SettingsTab
    {
        public string tabName;
        public Button button;
        public GameObject panel;
    }

    [SerializeField] private List<SettingsTab> tabs = new List<SettingsTab>();

    private int currentTabIndex = -1;

    private void Start()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            int capturedIndex = i;
            if (tabs[i].button != null)
                tabs[i].button.onClick.AddListener(() => OpenTab(capturedIndex));
        }

        CloseAll();
    }

    public void OpenTab(int index)
    {
        if (index < 0 || index >= tabs.Count) return;

        // close if same tab clicked again
        if (index == currentTabIndex)
        {
            tabs[currentTabIndex].panel?.SetActive(false);
            currentTabIndex = -1;
            return;
        }

        if (currentTabIndex >= 0 && currentTabIndex < tabs.Count)
            tabs[currentTabIndex].panel?.SetActive(false);

        currentTabIndex = index;
        tabs[currentTabIndex].panel?.SetActive(true);
    }

    public void OpenTab(string tabName)
    {
        int idx = tabs.FindIndex(t => t.tabName == tabName);
        if (idx >= 0)
            OpenTab(idx);
        else
            Debug.LogWarning($"[SettingsTabManager] Tab '{tabName}' not found!");
    }

    public void CloseAll()
    {
        foreach (var tab in tabs)
            tab.panel?.SetActive(false);
        currentTabIndex = -1;
    }
}
