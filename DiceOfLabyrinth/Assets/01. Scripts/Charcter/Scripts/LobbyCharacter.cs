using UnityEngine;

/// <summary>
/// 로비에서 사용되는 캐릭터 정보 및 기능
/// (외부 강화, 경험치, 성장 등)
/// </summary>
public class LobbyCharacter : Character
{
    [Header("로비 정보")]
    public int TotalExp = 0;
    public int ExternalATK = 0; // 장비, 성장 등 외부 강화
    public int ExternalDEF = 0;
    public int ExternalHP = 0;

    public int CurrentExp = 0;
    public int RegularATK; // (아무런 보정이 없는) 공격력
    public int RegularDEF; // (아무런 보정이 없는) 방어력
    public int RegularHP; // (아무런 보정이 없는) 체력(BattleCharacter의 MaxHP와 동일)
    public int CritChance; // 치명타 확률
    public int CritDamage; // 치명타 피해량

    // 경험치 수식은 현재 레벨 N에서 N+1로 넘어갈 때, 필요한 경험치는 250×(N+1)

    // 다음 레벨로 넘어가기 위한 경험치 계산
    public int GetExpToNextLevel()
    {
        return 250 * (Level + 1);
    }

    /// <summary>
    /// 경험치 추가 및 레벨업 처리
    /// </summary>
    public void AddExp(int exp)
    {
        TotalExp += exp;
        // 레벨업 조건 및 처리 로직 추가 가능
    }

    /// <summary>
    /// 외부 강화 수치 포함 최대 HP
    /// </summary>
    public override int GetMaxHP()
    {
        return base.GetMaxHP() + ExternalHP;
    }

    public override int GetATK()
    {
        return base.GetATK() + ExternalATK;
    }

    public override int GetDEF()
    {
        return base.GetDEF() + ExternalDEF;
    }
}
