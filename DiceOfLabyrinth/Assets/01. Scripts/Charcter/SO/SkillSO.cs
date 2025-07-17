using NUnit.Framework;
using UnityEngine;

/// <summary>
/// 스킬을 만들기 위한 Scriptable Object
/// 이후 json을 이용하여 Skill을 사용하도록 변경
/// </summary>

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill/Create New Skill")]
public class SkillSO : ScriptableObject
{
    //액티브와 패시브가 공통적으로 사용하는 내용들
    // 스킬 아이디, 스킬한글명, 스킬영어명, 스킬 설명, 스킬 아이콘, 스킬 레벨업 요구사항, 

    public string SkillID;
    public string SkillNameKr;
    public string SkillNameEn;
    // 스킬 타입, 스킬 대상, 스킬 규칙은 Enum으로 정의됨
    // public SkillType SkillType;
    // public SkillRule SkillRule; 
    // public SkillTarget SkillTarget;
    public string SkillDescription;
    public Sprite SkillIcon;
    //레벨업 요구 사항
    //public List<SkillLevelUpRequirement> LevelUpRequirements
}
