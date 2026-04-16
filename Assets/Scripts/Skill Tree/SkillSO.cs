using UnityEngine;

public enum SkillType
{
    Combat,
    Magic
}

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill Tree/Skill")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    public int maxLevel;
    public Sprite skillIcon;
    public SkillType skillType = SkillType.Combat;
}
