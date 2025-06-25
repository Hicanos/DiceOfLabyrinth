using UnityEngine;

/// <summary>
/// 배틀(전투)에서 사용되는 캐릭터 정보 및 기능
/// (실시간 변동 데이터, 버프 등)
/// </summary>
public class BattleCharacter : Character, IDamagable
{
    [Header("배틀 상태")]
    public int CurrentHP;
    public int CurrentATK;
    public int CurrentDEF;
    public int CurrentCritChance;
    public int CurrentCritDamage;
    public int FormationBonus;
    public GameObject GameObject; // 배틀에서 사용되는 게임 오브젝트, 스폰되는 캐릭터 모델

    /// <summary>
    /// 배틀용 초기화 (로비 정보 기반)
    /// </summary>
    public override void Initialize(CharacterSO so, int level = 1)
    {
        base.Initialize(so, level);
        ResetBattleData();
    }

    /// <summary>
    /// 배틀 데이터 초기화 (스테이지 시작/종료 시)
    /// </summary>
    public void ResetBattleData()
    {
        CurrentHP = GetMaxHP();
        CurrentATK = GetATK();
        CurrentDEF = GetDEF();
        CurrentCritChance = characterData.critChance;
        CurrentCritDamage = characterData.critDamage;
        FormationBonus = 0;
        // 기타 실시간 데이터 초기화
    }


    public void ApplyBuff(int atkBonus, int defBonus)
    {
        CurrentATK += atkBonus;
        CurrentDEF += defBonus;
    }

    public void TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }

    public void Heal(int amount)
    {
        throw new System.NotImplementedException();
    }
}
