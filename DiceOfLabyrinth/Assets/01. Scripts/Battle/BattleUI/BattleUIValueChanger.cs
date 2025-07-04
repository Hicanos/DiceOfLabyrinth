using UnityEngine;
using TMPro;

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

    /// <summary>
    /// 배틀에서 사용할 UI의 텍스트을 변경하는 메서드입니다.
    /// </summary>
    public void ChangeUIText(BattleTextUIEnum uiEnum, string value)
    {
        texts[(int)uiEnum].text = value;
    }

    /// <summary>
    /// 캐릭터와 에너미의 체력바 비율을 변경하는 메서드입니다.
    /// </summary>
    public void ChangeEnemyHpRatio(HPEnum hpEnum, float value)
    {
        HPs[(int)hpEnum].localScale = new Vector3(value, 1, 1);        
    }
}
