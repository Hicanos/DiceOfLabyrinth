using UnityEngine;

/// <summary>
/// 스킬을 만들기 위한 Scriptable Object
/// 이후 json을 이용하여 Skill을 사용하도록 변경
/// </summary>

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill/Create New Skill")]
public class SkillSO : ScriptableObject
{
   // 스킬 ID, 스킬 이름, 스킬 타입별로 상속(Passive, Active), 스킬 설명, 스킬 효과는 각자구현, 

    [SerializeField] protected string skillID;
    [SerializeField] protected string skillName;
    [SerializeField] protected string skillDescription;
}
