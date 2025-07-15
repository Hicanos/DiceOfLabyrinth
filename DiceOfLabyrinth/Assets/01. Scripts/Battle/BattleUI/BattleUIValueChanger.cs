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
    public void ChangeCharacterHpRatio(HPEnumCharacter hpEnum)
    {
        int maxHP = BattleManager.Instance.BattleGroup.BattleCharacters[(int)hpEnum].RegularHP;
        int curHP = BattleManager.Instance.BattleGroup.BattleCharacters[(int)hpEnum].CurrentHP;

        float ratio = (float)curHP / maxHP;

        ChangeUIText(hpEnum, $"{curHP} / {maxHP}");
        ChangeCharacterHpRatio(hpEnum, ratio);
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
    public void ChangeCharacterHpRatio(HPEnumCharacter hpEnum, float value)
    {
        BattleManager.Instance.BattleGroup.CharacterHPs[(int)hpEnum].localScale = new Vector3(value, 1, 1);
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
