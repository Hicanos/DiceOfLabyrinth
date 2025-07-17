using UnityEngine;

/// <summary>
/// 액티브 스킬을 만들기 위한 Scriptable Object
/// </summary>
[CreateAssetMenu(fileName = "NewActiveSkill", menuName = "Skill/Create New Active Skill")]
public class ActiveSO : SkillSO
{
    public int SkillCost;
    public int Target;
    public int SkillRule;
    public string BuffID;
    public string BuffID_2;
    public int BuffAmount;
    public int BuffAmount2;
    public int BuffProbability;
    public int BuffTurn;
    public float SkillValue;
    public float PlusValue;
    public int CoolTime;
}
