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
    public GameObject CharObj; // 배틀에서 사용되는 게임 오브젝트, 스폰되는 캐릭터 모델
    public GameObject UIChar; // 배틀 UI에서 사용되는 캐릭터 UI 오브젝트
    // 캐릭터가 사용하는 스킬 데이터를 Header를 이용하여 여기에 등록

    public bool IsDied; // 현재 HP가 0 이하인 경우 사망 상태



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


    public void ApplyATK(int amount)
    {
        CurrentATK += amount;
    }
    public void ApplyDEF(int amount)
    {
        CurrentDEF += amount;
    }

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        if (CurrentHP < 0)
        {
            CurrentHP = 0;
            // 캐릭터 사망 처리
            IsDied = true;
        }
    }

    public void Heal(int amount)
    {
        CurrentHP += amount;
        if( CurrentHP > GetMaxHP())
        {
            CurrentHP = GetMaxHP(); // 최대 HP 초과 방지
        }
    }

    public void Attack()
    {
        // 공격 로직 구현 - 실제로는 배틀 매니저: Battle에서 플레이어 턴에 호출이 필요해보임. 추가 해야하는 변수가 너무 많음
        //대미지 계산식: [주사위 눈금 * 족보별계수 + {공격력 - 방어력 * (1-방어력 관통률)}] * (버프 + 아티팩트 + 속성 + 패시브 + 각인)
        // 예시: int damage = (diceValue * familyCoefficient + (CurrentATK - CurrentDEF * (1 - defensePenetration))) * (buff + artifact + element + passive + engraving);
        
        Debug.Log("공격");
    }

    public int AttackDamage()
    {
        //공격 기본 대미지 계산 로직
        return CurrentATK;
    }
}
