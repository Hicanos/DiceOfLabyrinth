using UnityEngine;
using static DesignEnums;

/// <summary>
/// 패시브 스킬을 만들기 위한 Scriptable Object
/// </summary>
[CreateAssetMenu(fileName = "NewPassiveSkill", menuName = "Skill/Create New Passive Skill")]
public class PassiveSO : SkillSO
{
    public string[] BuffIDs; // 버프 아이디 배열
}
