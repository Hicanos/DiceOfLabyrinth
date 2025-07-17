using UnityEngine;

/// <summary>
/// 액티브 스킬을 만들기 위한 Scriptable Object
/// </summary>
[CreateAssetMenu(fileName = "NewActiveSkill", menuName = "Skill/Create New Active Skill")]
public class ActiveSO : SkillSO
{
    public int SkillCost; //소모되는 스킬 코스트
    public string[] BuffIDs; // 버프 아이디 배열
    public string[] BuffAmounts; // 버프 양 배열
    // BuffID[0]은 BuffAmount[0]에 해당하는 버프 양을 적용함

    public int BuffProbability; // 버프 적용 확률(%)
    public int BuffTurn; // 버프가 지속되는 턴
    public float SkillValue; // 스킬 효과 값
    public float PlusValue; // 스킬이 레벨업 할 때 증가하는 값
    public int CoolTime; // 스킬의 쿨타임 (1은 매턴 사용 가능)
}
