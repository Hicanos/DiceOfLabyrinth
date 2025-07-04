using NUnit.Framework;
using TMPro;
using UnityEngine;

public enum BattleTextUIEnum
{ 
    Cost,
    MonsterSkillName,
    MonsterSkillDescription,
    MonsterHP,
    Turn
}

public enum HPEnum
{
    Character1, Character2, Character3, Character4, Character5, enemy
}

public class BattleUIValueChanger : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] texts;
    public RectTransform[] HPs;

    public void ChangeUIText(BattleTextUIEnum uiEnum, string value)
    {
        texts[(int)uiEnum].text = value;
    }

    public void ChangeEnemyHpRatio(HPEnum hpEnum, float value)
    {
        HPs[(int)hpEnum].localScale = new Vector3(value, 1, 1);        
    }
}
