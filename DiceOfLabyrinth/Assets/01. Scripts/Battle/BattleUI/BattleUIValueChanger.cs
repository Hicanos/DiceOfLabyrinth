using UnityEngine;

public enum BattleTextUIEnum
{ 
    Cost,
    MonsterSkillName,
    MonsterSkillDescription,
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
        BattleCharGroup battleGroup = BattleManager.Instance.BattleGroup;

        int maxHP = battleGroup.BattleCharacters[(int)hpEnum].RegularHP;
        int curHP = battleGroup.BattleCharacters[(int)hpEnum].CurrentHP;

        string hpString;
        if (battleGroup.BarrierAmounts[(int)hpEnum] > 0)
        {
            hpString = $"{curHP} / {maxHP} + ({battleGroup.BarrierAmounts[(int)hpEnum]})";
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
        int maxHP = BattleManager.Instance.Enemy.MaxHP;
        int curHP = BattleManager.Instance.Enemy.CurrentHP;

        float ratio = (float)curHP / maxHP;

        ChangeUIText(hpEnum, $"{curHP} / {maxHP}");
        ChangeEnemyHpRatio(hpEnum, ratio);
    }

    /// <summary>
    /// 체력바 텍스트를 변경하는 메서드입니다.
    /// </summary>
    public void ChangeUIText(HPEnumCharacter uiEnum, string value)
    {
        BattleManager.Instance.BattleGroup.CharacterHPTexts[(int)uiEnum].text = value;
    }

    /// <summary>
    /// 캐릭터의 체력바 비율을 변경하는 메서드입니다.
    /// </summary>
    public void ChangeCharacterHpRatio(HPEnumCharacter hpEnum)
    {        
        BattleCharGroup battleGroup = BattleManager.Instance.BattleGroup;
        int index = (int)hpEnum;

        int totalHP = battleGroup.BattleCharacters[index].RegularHP + battleGroup.BarrierAmounts[index];
        int curHP = battleGroup.BattleCharacters[index].CurrentHP + battleGroup.BarrierAmounts[index];

        float hpRatio;
        float barrierRatio;
        float blinkRatio;

        if (totalHP >= battleGroup.BattleCharacters[index].RegularHP)
        {
            hpRatio = (float)battleGroup.BattleCharacters[index].CurrentHP / curHP;
            barrierRatio = (float)battleGroup.BarrierAmounts[index] / curHP;
            blinkRatio = 0;
        }
        else
        {
            hpRatio = (float)battleGroup.BattleCharacters[index].CurrentHP / totalHP;
            barrierRatio = (float)battleGroup.BarrierAmounts[index] / totalHP;
            blinkRatio = 1 - (hpRatio + barrierRatio);
        }
        
        

        battleGroup.CharacterHPs[index].localScale = new Vector3(hpRatio, 1, 1);
        battleGroup.CharacterBarriers[index].localScale = new Vector3(barrierRatio, 1, 1);
        battleGroup.CharacterBlank[index].localScale = new Vector3(blinkRatio, 1, 1);
    }

    /// <summary>
    /// 에너미의 체력바 비율을 변경하는 메서드입니다.
    /// </summary>
    public void ChangeEnemyHpRatio(HPEnumEnemy hpEnum, float value)
    {
        BattleManager.Instance.Enemy.EnemyHP.localScale = new Vector3(value, 1, 1);
    }

    /// <summary>
    /// 체력바 텍스트를 변경하는 메서드입니다.
    /// </summary>
    public void ChangeUIText(HPEnumEnemy uiEnum, string value)
    {
        BattleManager.Instance.Enemy.EnemyHPText.text = value;
    }
}
