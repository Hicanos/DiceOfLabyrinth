using UnityEngine;
using System;

/// <summary>
/// 배틀(전투)에서 사용되는 캐릭터 정보 및 기능
/// (실시간 변동 데이터, 버프 등)
/// </summary>
public class BattleCharacter : IDamagable
{
    /*
     * var battleData = CharacterManager.Instance.RegisterBattleCharacterData("charID");
     */


    // 캐릭터의 고유 데이터
    public string CharID { get; private set; }
    public CharacterSO CharacterData { get; private set; }

    // LobbyCharacter 데이터 참조 (전투 시작 시 복사)
    public int Level { get; private set; }
    public int RegularATK { get; private set; }
    public int RegularDEF { get; private set; }
    public int RegularHP { get; private set; }
    public float CritChance { get; private set; }
    public float CritDamage { get; private set; }

    [Header("배틀 상태")]
    // 전투 중 실시간 변동 데이터
    public int CurrentHP;
    public int CurrentATK;
    public int CurrentDEF;
    public float CurrentCritChance;
    public float CurrentCritDamage;

    [Header("처음 저장되는 기본 값")]
    private int initialHP;
    private int initialATK;
    private int initialDEF;
    private float initialCritChance;
    private float initialCritDamage;
    private int initialLevel;

    public bool IsDied { get; private set; }

    public event Action<int> OnHPChanged;
    public event Action OnDied;
    public event Action OnRevived;

    public BattleCharacter(CharacterSO so)
    {
        SetCharacterSO(so);
    }

    public BattleCharacter(string charID)
    {
        SetCharID(charID);
    }


    /// <summary>
    /// 외부에서 CharacterSO만 세팅하면 자동으로 LobbyCharacter를 찾아 초기화
    /// </summary>
    public void SetCharacterSO(CharacterSO so)
    {
        CharacterData = so;
        CharID = so.charID;
        InitializeFromLobbyAuto();
    }

    /// <summary>
    /// 외부에서 charID만 세팅해도 자동으로 LobbyCharacter를 찾아 초기화
    /// </summary>
    public void SetCharID(string charID)
    {
        CharID = charID;
        CharacterData = CharacterManager.Instance.AllCharacters.TryGetValue(charID, out var so) ? so : null;
        InitializeFromLobbyAuto();
    }

    /// <summary>
    /// CharacterSO/charID를 기반으로 자동으로 LobbyCharacter를 찾아 초기화
    /// </summary>
    private void InitializeFromLobbyAuto()
    {
        LobbyCharacter lobbyChar = null;
        if (CharacterData != null)
            lobbyChar = CharacterManager.Instance.GetLobbyCharacterBySO(CharacterData);
        else if (!string.IsNullOrEmpty(CharID))
            lobbyChar = CharacterManager.Instance.GetLobbyCharacterByID(CharID);

        if (lobbyChar == null)
        {
            // MonoBehaviour가 아니므로 Debug.LogError 대신 예외 처리
            throw new InvalidOperationException($"BattleCharacter: 해당하는 LobbyCharacter를 찾을 수 없습니다. (charID: {CharID})");
        }

        // LobbyCharacter의 모든 주요 데이터를 복사
        Level = lobbyChar.Level;
        RegularATK = lobbyChar.RegularATK;
        RegularDEF = lobbyChar.RegularDEF;
        RegularHP = lobbyChar.RegularHP;
        CritChance = lobbyChar.CritChance;
        CritDamage = lobbyChar.CritDamage;
        CharacterData = lobbyChar.CharacterData;

        // 초기값 저장
        initialATK = RegularATK;
        initialDEF = RegularDEF;
        initialHP = RegularHP;
        initialCritChance = CritChance;
        initialCritDamage = CritDamage;
        initialLevel = Level;

        // 전투용 데이터 초기화
        ResetBattleData();
        IsDied = false;
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
    }

    public void ApplyATK(int amount) => CurrentATK += amount;
    public void ApplyDEF(int amount) => CurrentDEF += amount;

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        if (CurrentHP < 0)
        {
            CurrentHP = 0;
            // 캐릭터 사망 처리
            IsDied = true;
            OnDied?.Invoke(); // 사망 이벤트 호출
        }
        OnHPChanged?.Invoke(CurrentHP); // HP 변경 이벤트 호출
    }

    public void Heal(int amount)
    {
        CurrentHP += amount;
        if (CurrentHP > initialHP)
        {
            CurrentHP = initialHP; // 최대 HP 초과 방지
        }
        OnHPChanged?.Invoke(CurrentHP); // HP 변경 이벤트 호출
    }

    public int AttackDamage() => CurrentATK;

    public void Revive()
    {
        if (IsDied)
        {
            IsDied = false;
            CurrentHP = initialHP; // 부활 시 최대 HP로 회복
            ResetBattleData(); // 배틀 데이터 초기화
            OnRevived?.Invoke(); // 부활 이벤트 호출
            OnHPChanged?.Invoke(CurrentHP); // HP 변경 이벤트 호출
        }
    }
}
