using UnityEngine;
using System;

/// <summary>
/// 배틀(전투)에서 사용되는 캐릭터 정보 및 기능
/// (실시간 변동 데이터, 버프 등)
/// </summary>
public class BattleCharacter : Character, IDamagable
{
    [Header("배틀 상태")]
    // 전투 중 실시간으로 변동되는 데이터
    public int CurrentHP;
    public int CurrentATK;
    public int CurrentDEF;
    public int CurrentCritChance;
    public int CurrentCritDamage;

    [Header("처음 저장되는 기본 값")]
    private int initialHP; // 처음 생성 시의 HP (로비 정보 기반)
    private int initialATK; // 처음 생성 시의 공격력 (로비 정보 기반)
    private int initialDEF; // 처음 생성 시의 방어력 (로비 정보 기반)
    private int initialCritChance; // 처음 생성 시의 치명타 확률 (로비 정보 기반)
    private int initialCritDamage; // 처음 생성 시의 치명타 피해량 (로비 정보 기반)
    private int initialLevel; // 처음 생성 시의 레벨 (로비 정보 기반)

    // 액션 이벤트
    public event Action<int> OnHPChanged; // HP가 변경될 때 호출
    public event Action OnDied; // 캐릭터가 사망했을 때 호출
    public event Action OnRevived; // 캐릭터가 부활했을 때 호출


    // 캐릭터가 사용하는 스킬 데이터를 이후 SO에서 가져와 Header를 이용하여 여기에 등록

    private bool isDied; // 현재 HP가 0 이하인 경우 사망 상태



    /// <summary>
    /// 배틀용 초기화 (로비 정보 기반)
    /// </summary>
    public override void Initialize(CharacterSO so, int level = 1)
    {
        base.Initialize(so, level);
        ResetBattleData();
    }

    // 배틀용 초기화 시 로비캐릭터의 데이터를 기반으로 초기화
    public void InitializeFormLobby(LobbyCharacter lobbyChar)
    {
        lobbyChar.Initialize(lobbyChar.CharacterData, lobbyChar.Level);
        initialATK = lobbyChar.RegularATK;
        initialDEF = lobbyChar.RegularDEF;
        initialHP = lobbyChar.RegularHP;
        initialCritChance = lobbyChar.CritChance;
        initialCritDamage = lobbyChar.CritDamage;
        initialLevel = lobbyChar.Level;

        ResetBattleData();
        isDied = false; // 초기화 시 사망 상태 해제
    }


    /// <summary>
    /// 배틀 데이터 초기화 (스테이지 시작 시)
    /// </summary>
    public void ResetBattleData()
    {
        CurrentHP = initialHP;
        CurrentATK = initialATK;
        CurrentDEF = initialDEF;
        CurrentCritChance = initialCritChance;
        CurrentCritDamage = initialCritDamage;
        // FormationBonus 등은 필요시 별도 처리
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
            isDied = true;
            OnDied?.Invoke(); // 사망 이벤트 호출
        }
        OnHPChanged?.Invoke(CurrentHP); // HP 변경 이벤트 호출
    }

    public void Heal(int amount)
    {
        CurrentHP += amount;
        if( CurrentHP > GetMaxHP())
        {
            CurrentHP = GetMaxHP(); // 최대 HP 초과 방지
        }
        OnHPChanged?.Invoke(CurrentHP); // HP 변경 이벤트 호출
    }

    public void Attack()
    {
        // 공격 로직 구현 - 실제로는 배틀 매니저: Battle에서 플레이어 턴에 호출이 필요해보임. 추가 해야하는 변수가 너무 많음
        //대미지 계산식: [주사위 눈금 * 족보별계수 + {공격력 - 방어력}] * (버프 + 아티팩트 + 속성 + 패시브 + 각인)
        // 예시: int damage = (diceValue * familyCoefficient + (CurrentATK - CurrentDEF)) * (buff + artifact + element + passive + engraving);
        
        Debug.Log("공격");
    }

    public int AttackDamage()
    {
        //공격 기본 대미지 계산 로직
        return CurrentATK;
    }

    // 부활 처리
    // 바로 직전의 데이터값을 그대로 회복해야 함
    public void Revive()
    {
        if (isDied)
        {
            isDied = false;
            CurrentHP = GetMaxHP(); // 부활 시 최대 HP로 회복
            ResetBattleData(); // 배틀 데이터 초기화
            OnRevived?.Invoke(); // 부활 이벤트 호출
            OnHPChanged?.Invoke(CurrentHP); // HP 변경 이벤트 호출
        }
    }
}
