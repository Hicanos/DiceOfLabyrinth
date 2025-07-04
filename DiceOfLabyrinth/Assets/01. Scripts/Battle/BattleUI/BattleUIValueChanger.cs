using UnityEngine;
using TMPro;

public enum BattleTextUIEnum
{ 
    Cost,
    MonsterSkillName,
    MonsterSkillDescription,
    Turn,
}


public class BattleUIValueChanger : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] texts;
    [SerializeField] RectTransform enemyHPImage;

    public void ChangeUIText(BattleTextUIEnum uiEnum, string value)
    {
        texts[(int)uiEnum].text = value;
    }

    public void ChangeEnemyHpRatio(float value)
    {
        enemyHPImage.localScale = new Vector3(value, 1f, 1f);
    }
}
