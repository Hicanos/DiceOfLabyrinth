using UnityEngine;

/// <summary>
/// 로비에서 사용되는 캐릭터 정보 및 기능
/// (외부 강화, 경험치, 성장 등)
/// </summary>
[System.Serializable]
public class LobbyCharacter : Character
{
    [Header("로비 정보")]
    public int CurrentExp = 0;

    // Regular 속성을 BattleCharacter에서 가져감
    public int RegularATK; // (아무런 보정이 없는) 공격력
    public int RegularDEF; // (아무런 보정이 없는) 방어력
    public int RegularHP; // (아무런 보정이 없는) 체력
    public float CritChance; // 치명타 확률
    public float CritDamage; // 치명타 피해량
    public int MaxLevel = 20; // 최대 레벨


    // 로비 캐릭터 데이터
    public override void Initialize(CharacterSO so, int level = 1)
    {
        base.Initialize(so, level);
        // 로비 캐릭터의 기본 능력치 설정, 이후 레벨업 시 추가된 능력치는 계속 반영
        RegularATK = GetATK();
        RegularDEF = GetDEF();
        RegularHP = GetMaxHP();
        CritChance = CharacterData.critChance;
        CritDamage = CharacterData.critDamage;
    }


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
        GetExpToNextLevel();
        CurrentExp += exp;
        // 레벨업 조건 및 처리 로직 추가 가능
        // 현재 최대레벨은 20, 20에 도달하면 더이상 레벨이 증가하지 않음
        while (CurrentExp >= GetExpToNextLevel()&& Level < MaxLevel)
        {
            CurrentExp -= GetExpToNextLevel();
            LevelUP();
        }

        if (Level >= MaxLevel)
        {
            CurrentExp = 0;
        }
        // 변동된 값(CurrentExp) 저장
        DataSaver.Instance.SaveCharacter(this);
    }


    private void LevelUP()
    {
        if (Level < MaxLevel)
        {
            Level++;
            // 레벨이 올라가면 올라간만큼 기본 능력치 증가함
            GetMaxHP();
            RegularATK = GetATK();
            RegularDEF = GetDEF();
            RegularHP = GetMaxHP();
        }
    }

    /// <summary>
    /// 외부 강화 수치 포함 최대 HP
    /// </summary>
    public override int GetMaxHP()
    {
        return base.GetMaxHP();
    }

    public override int GetATK()
    {
        return base.GetATK();
    }

    public override int GetDEF()
    {
        return base.GetDEF();
    }

    public CharData GetCharData(CharDataLoader loader)
    {
        return loader.GetByCharID(CharacterData.charID);
    }
}
