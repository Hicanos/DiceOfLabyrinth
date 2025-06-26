using UnityEngine;

/// <summary>
/// 패시브 스킬을 만들기 위한 Scriptable Object
/// </summary>

[CreateAssetMenu(fileName = "NewPassiveSkill", menuName = "Skill/Create New Passive Skill")]
public class PassiveSO : SkillSO
{
    // 패시브 스킬은 특정 조건에서 자동으로 발동됨
    // 예: 캐릭터가 특정 상태일 때, 또는 특정 행동을 할 때 발동
    
}
