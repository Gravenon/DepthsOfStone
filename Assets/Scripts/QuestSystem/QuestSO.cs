using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestSO", menuName = "QuestSO")]
public class QuestSO : ScriptableObject
{
    public string questName;
    [TextArea] public string questDescription;
    public int questLevel;
    
    public List<QuestObejtive> objectives;
    public List<QuestReward> rewards;
    
}
    
    [System.Serializable] 
    public class QuestObejtive
    {
        public string description;

        [SerializeField] private Object trarget;
        public ItemSO targetItem => trarget as ItemSO;
        public AcotrSO targetNPC => trarget as AcotrSO;
        public LocationSO targetLocation => trarget as LocationSO;

        public bool isAttackObjective; // no target needed — counts player attack swings
        public bool isDashObjective;   // no target needed — completed when dash is unlocked

        public int requiredAmount;
        public int currentAmount;
    }

    [System.Serializable]
    public class QuestReward
    {
        public ItemSO itemSo;
        public int quantity;
    }
