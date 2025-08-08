using UnityEngine;

public enum BattleTextUIEnum
{ 
    Cost,
    Rank,
    Reroll
}

public enum HPEnumCharacter
{
    Character1, Character2, Character3, Character4, Character5
}

public enum HPEnumEnemy
{
    enemy
}

public class BattleUIValueChanger : MonoBehaviour
{
    /// <summary>
    /// 배틀에서 사용할 UI의 텍스트을 변경하는 메서드입니다.
    /// </summary>
    public void ChangeUIText(BattleTextUIEnum uiEnum, string value)
    {
        UIManager.Instance.BattleUI.ChangeUIText(uiEnum, value);
    }

    /// <summary>
    /// 캐릭터의 체력바 비율과 텍스트를 변경하는 메서드입니다.
    /// </summary>
    public void ChangeCharacterHp(HPEnumCharacter hpEnum)
    {
        BattleCharacterInBattle character = BattleManager.Instance.PartyData.Characters[(int)hpEnum];
        
        int maxHP = character.MaxHP;
        int curHP = character.CurrentHP;

        string hpString;
        if (character.IsBarrierOn)
        {
            hpString = $"{curHP} / {maxHP} + ({character.CurrentBarrier})";
        }
        else
        {
            hpString = $"{curHP} / {maxHP}";
        }

        ChangeUIText(hpEnum, hpString);
        ChangeCharacterHpRatio(hpEnum);
    }

    /// <summary>
    /// 에너미의 체력바 비율과 텍스트를 변경하는 메서드입니다.
    /// </summary>
    public void ChangeEnemyHpUI(HPEnumEnemy hpEnum)
    {
        string hpString;
        BattleEnemy enemy = BattleManager.Instance.Enemy;

        if (enemy.CurrentBarrier > 0)
        {
            hpString = $"{enemy.CurrentHP} / {enemy.MaxHP} + ({enemy.CurrentBarrier})";
        }
        else
        {
            hpString = $"{enemy.CurrentHP} / {enemy.MaxHP}";
        }

        ChangeUIText(hpEnum, hpString);
        ChangeEnemyHpRatio(hpEnum);
    }

    /// <summary>
    /// 체력바 텍스트를 변경하는 메서드입니다.
    /// </summary>
    public void ChangeUIText(HPEnumCharacter uiEnum, string value)
    {
        BattleManager.Instance.PartyData.Characters[(int)uiEnum].CharacterHPTexts.text = value;
    }

    /// <summary>
    /// 캐릭터의 체력바 비율을 변경하는 메서드입니다.
    /// </summary>
    public void ChangeCharacterHpRatio(HPEnumCharacter hpEnum)
    {        
        BattleCharacterInBattle character = BattleManager.Instance.PartyData.Characters[(int)hpEnum];
        int index = (int)hpEnum;

        int totalHP = character.MaxHP + character.CurrentBarrier;
        int curHP = character.CurrentHP + character.CurrentBarrier;

        float hpRatio;
        float barrierRatio;
        float blinkRatio;

        if (curHP >= character.MaxHP)
        {
            hpRatio = (float)character.CurrentHP / curHP;
            barrierRatio = (float)character.CurrentBarrier / curHP;
            blinkRatio = 0;
        }
        else
        {
            hpRatio = (float)character.CurrentHP / totalHP;
            barrierRatio = (float)character.CurrentBarrier / totalHP;
            blinkRatio = 1 - (hpRatio + barrierRatio);
        }

        character.CharacterHPs.localScale = new Vector3(hpRatio, 1, 1);
        character.CharacterBarriers.localScale = new Vector3(barrierRatio, 1, 1);
        character.CharacterBlank.localScale = new Vector3(blinkRatio, 1, 1);
    }

    /// <summary>
    /// 에너미의 체력바 비율을 변경하는 메서드입니다.
    /// </summary>
    public void ChangeEnemyHpRatio(HPEnumEnemy hpEnum)
    {
        int index = (int)hpEnum;
        BattleEnemy enemy = BattleManager.Instance.Enemy;

        int totalHP = enemy.MaxHP + enemy.CurrentBarrier;
        int curHP = enemy.CurrentHP + enemy.CurrentBarrier;

        float hpRatio;
        float barrierRatio;
        float blinkRatio;

        if (curHP >= enemy.MaxHP)
        {
            hpRatio = (float)enemy.CurrentHP / curHP;
            barrierRatio = (float)enemy.CurrentBarrier / curHP;
            blinkRatio = 0;
        }
        else
        {
            hpRatio = (float)enemy.CurrentHP / totalHP;
            barrierRatio = (float)enemy.CurrentBarrier / totalHP;
            blinkRatio = 1 - (hpRatio + barrierRatio);
        }

        enemy.EnemyHPs.localScale = new Vector3(hpRatio, 1, 1);
        enemy.EnemyBarriers.localScale = new Vector3(barrierRatio, 1, 1);
        enemy.EnemyBlank.localScale = new Vector3(blinkRatio, 1, 1);
    }

    /// <summary>
    /// 체력바 텍스트를 변경하는 메서드입니다.
    /// </summary>
    public void ChangeUIText(HPEnumEnemy uiEnum, string value)
    {
        BattleManager.Instance.Enemy.EnemyHPTexts.text = value;
    }
}
