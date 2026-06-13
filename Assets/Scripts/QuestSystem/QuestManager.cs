using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour, IDataPersistence
{
    private Dictionary<QuestSO, Dictionary<QuestObejtive, int>> questProgress = new();
    private List<QuestSO> completedQuests = new();

    private int _attackCount;
    private bool _dashUnlocked;

    private void OnEnable()
    {
        QuestEvents.IsQuestComplete += IsQuestComplete;
        QuestEvents.OnPlayerAttacked += OnPlayerAttacked;
        QuestEvents.OnDashUnlocked += OnDashUnlocked;
    }

    private void OnDisable()
    {
        QuestEvents.IsQuestComplete -= IsQuestComplete;
        QuestEvents.OnPlayerAttacked -= OnPlayerAttacked;
        QuestEvents.OnDashUnlocked -= OnDashUnlocked;
    }

    private void OnPlayerAttacked()
    {
        _attackCount++;
        foreach (var kv in questProgress)
            foreach (var objective in kv.Key.objectives)
                if (objective.isAttackObjective)
                    UpdateObjectiveProgress(kv.Key, objective);
    }

    private void OnDashUnlocked()
    {
        _dashUnlocked = true;
        foreach (var kv in questProgress)
            foreach (var objective in kv.Key.objectives)
                if (objective.isDashObjective)
                    UpdateObjectiveProgress(kv.Key, objective);
    }

    public void SaveData(ref GameData data)
    {
        var activeList = new List<SerializedQuestProgress>();
        foreach (var kv in questProgress)
        {
            var saved = new SerializedQuestProgress { questName = kv.Key.questName, objectiveProgress = new int[kv.Key.objectives.Count]};
            for (int i = 0; i < kv.Key.objectives.Count; i++)
            {
                kv.Value.TryGetValue(kv.Key.objectives[i], out int progress);
                saved.objectiveProgress[i] = progress;
            }
            activeList.Add(saved);
        }
        data.activeQuests = activeList.ToArray();

        var doneList = new List<SerializedCompletedQuest>();
        foreach (var q in completedQuests)
            doneList.Add(new SerializedCompletedQuest { questName = q.questName });
        data.completedQuests = doneList.ToArray();
    }

    public void LoadData(GameData data)
    {
        questProgress.Clear();
        completedQuests.Clear();

        QuestSOLibrary library = FindFirstObjectByType<QuestSOLibrary>();
        if (library == null) return;

        if (data.completedQuests != null)
            foreach (var saved in data.completedQuests)
            {
                QuestSO q = library.FindByName(saved.questName);
                if (q != null && !completedQuests.Contains(q))
                    completedQuests.Add(q);
            }

        if (data.activeQuests != null)
            foreach (var saved in data.activeQuests)
            {
                QuestSO quest = library.FindByName(saved.questName);
                if (quest == null) continue;

                var progressDict = new Dictionary<QuestObejtive, int>();
                for (int i = 0; i < quest.objectives.Count; i++)
                {
                    int savedProgress = saved.objectiveProgress != null && i < saved.objectiveProgress.Length ? saved.objectiveProgress[i] : 0;
                    progressDict[quest.objectives[i]] = savedProgress;
                }
                questProgress[quest] = progressDict;
            }
    }

    public bool IsQuestAccepted(QuestSO quest) => questProgress.ContainsKey(quest);
    public List<QuestSO> GetActiveQuests() => new List<QuestSO>(questProgress.Keys);
    public bool GetCompleteQuest(QuestSO quest) => completedQuests.Contains(quest);

    public void AcceptQuest(QuestSO quest)
    {
        if (!questProgress.ContainsKey(quest))
            questProgress[quest] = new Dictionary<QuestObejtive, int>();

        // Reset per-quest counters so previous quest progress doesn't carry over
        bool hasAttack = quest.objectives.Exists(o => o.isAttackObjective);
        bool hasDash   = quest.objectives.Exists(o => o.isDashObjective);
        if (hasAttack) _attackCount = 0;
        if (hasDash)   _dashUnlocked = false;

        foreach (var objective in quest.objectives)
            UpdateObjectiveProgress(quest, objective);
    }

    public bool IsQuestComplete(QuestSO quest)
    {
        // Already turned in — counts as complete
        if (completedQuests.Contains(quest)) return true;

        if (!questProgress.TryGetValue(quest, out var progress)) return false;
        foreach (var objective in quest.objectives)
            UpdateObjectiveProgress(quest, objective);
        foreach (var objective in quest.objectives)
            if (progress[objective] < objective.requiredAmount) return false;
        return true;
    }

    public void CompleteQuest(QuestSO quest)
    {
        questProgress.Remove(quest);
        completedQuests.Add(quest);
        foreach (var objective in quest.objectives)
            if (objective.targetItem != null && objective.requiredAmount > 0)
                InventoryManager.Instance.RemoveItem(objective.targetItem, objective.requiredAmount);
        foreach (var reward in quest.rewards)
            InventoryManager.Instance.AddItem(reward.itemSo, reward.quantity);
    }

    public void UpdateObjectiveProgress(QuestSO quest, QuestObejtive objective)
    {
        if (!questProgress.ContainsKey(quest)) return;

        int newAmount = 0;
        if (objective.isAttackObjective)
            newAmount = _attackCount;
        else if (objective.isDashObjective)
            newAmount = _dashUnlocked ? objective.requiredAmount : 0;
        else if (objective.targetItem != null)
            newAmount = InventoryManager.Instance.GetItemCount(objective.targetItem);
        else if (objective.targetLocation != null && GameManager.Instance.LocationHistoryTracker.HasVisited(objective.targetLocation))
            newAmount = objective.requiredAmount;
        else if (objective.targetNPC != null && GameManager.Instance.DialogueHistoryTraker.HasSpokenWith(objective.targetNPC))
            newAmount = objective.requiredAmount;

        questProgress[quest][objective] = newAmount;
    }

    public string GetProgressText(QuestSO quest, QuestObejtive objective)
    {
        int current = GetCurrentAmount(quest, objective);
        if (current >= objective.requiredAmount) return "Completed";
        if (objective.isAttackObjective) return $"{current}/{objective.requiredAmount} attacks";
        if (objective.isDashObjective) return "Unlock dash";
        if (objective.targetItem != null) return $"{current}/{objective.requiredAmount} {objective.targetItem.itemName}";
        return "In Progress";
    }

    public int GetCurrentAmount(QuestSO quest, QuestObejtive objective)
    {
        if (questProgress.TryGetValue(quest, out var progress))
            if (progress.TryGetValue(objective, out int amount))
                return amount;
        return 0;
    }

    // Alias kept for backwards compatibility
    public int GteCurrentAmount(QuestSO quest, QuestObejtive objective) => GetCurrentAmount(quest, objective);
}
