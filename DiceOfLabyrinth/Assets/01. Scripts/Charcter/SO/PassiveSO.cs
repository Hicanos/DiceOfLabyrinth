using UnityEngine;
using static DesignEnums;

/// <summary>
/// 패시브 스킬을 만들기 위한 Scriptable Object
/// </summary>
[CreateAssetMenu(fileName = "NewPassiveSkill", menuName = "Skill/Create New Passive Skill")]
public class PassiveSO : SkillSO
{
    // 패시브 스킬은 특정 조건에서 자동으로 발동됨
    // 조건은 SkillRule에 정의되어있음
    // SkillRule 중 DeckMaid가 조건인 경우, 또다른 Enum으로 족보(Triple, Quadruple 등)를 확인해야함
    // SkillRule 중 SumOver가 조건인 경우, 주사위 합이 특정값 이상이어야 함 (이를 담을 int 변수 필요)
    // 모든 패시브 스킬 조건에는 공통적으로, 주사위를 굴린 캐릭터의 주사위에서 '시그니처 넘버'가 발생해야 함
    // 패시브 스킬은 조건만 만족하면 자동으로 발동되며 버프도 반드시 적용됨

   
    // 시그니처 넘버 = 캐릭터SO의 SignatureNumber와 동일

    public string[] BuffIDs; // 버프 아이디 배열
    public BuffAmount[] BuffAmounts; // 버프 양 배열
    public int BuffTurn; // 버프 지속 턴
    public float SkillValue; // 스킬 효과 수치
    public float PlusValue; // 추가 효과 수치
    public int CoolTime; // 재사용 대기 시간
}
