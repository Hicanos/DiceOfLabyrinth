using UnityEngine;
using System;
using TMPro;

public class BattleUI : MonoBehaviour
{
    public GameObject fixedDiceArea;
    public GameObject victoryUI;
    public GameObject defeatUI;
    public GameObject stagmaDisplayer;
    public Canvas     battleCanvas;

    [Header("AbstractButtons")]
    [SerializeField] AbstractBattleButton diceBackboard;
    [SerializeField] AbstractBattleButton roll;
    [SerializeField] AbstractBattleButton char1;
    [SerializeField] AbstractBattleButton char2;
    [SerializeField] AbstractBattleButton char3;
    [SerializeField] AbstractBattleButton char4;
    [SerializeField] AbstractBattleButton char5;
    [SerializeField] AbstractBattleButton patternDisplayer;

    [NonSerialized] public AbstractBattleButton[] Buttons = new AbstractBattleButton[8];

    [Header("Texts For Value Changer")]
    [SerializeField] TextMeshProUGUI cost;
    [SerializeField] TextMeshProUGUI monsterSkillName;
    [SerializeField] TextMeshProUGUI monsterSkillDescription;
    [SerializeField] TextMeshProUGUI rank;
    [SerializeField] TextMeshProUGUI reRoll;

    [NonSerialized] private TextMeshProUGUI[] texts;

    //[NonSerialized] private RectTransform[]   characterHPs;
    //[NonSerialized] private TextMeshProUGUI[] characterHPTexts;

    //[NonSerialized] private RectTransform[]   enemyHPs;
    //[NonSerialized] private TextMeshProUGUI[] enemyHPTexts;

    //[Header("Enemy HP Bars")]
    //[SerializeField] Image hPBar_Enemy;

    //[NonSerialized] public Image[] EnemyHPBars = new Image[1];

    public void Setting()
    {
        Buttons[0] = diceBackboard;
        Buttons[1] = roll;
        Buttons[2] = char1;
        Buttons[3] = char2;
        Buttons[4] = char3;
        Buttons[5] = char4;
        Buttons[6] = char5;
        Buttons[7] = patternDisplayer;

        texts = new TextMeshProUGUI[5];
        texts[0] = cost;
        texts[1] = monsterSkillName;
        texts[2] = monsterSkillDescription;
        texts[3] = rank;
        texts[4] = reRoll;

        //characterHPTexts = new TextMeshProUGUI[5];
        //characterHPTexts[0] = hPBar_Char1.GetComponentInChildren<TextMeshProUGUI>();
        //characterHPTexts[1] = hPBar_Char2.GetComponentInChildren<TextMeshProUGUI>();
        //characterHPTexts[2] = hPBar_Char3.GetComponentInChildren<TextMeshProUGUI>();
        //characterHPTexts[3] = hPBar_Char4.GetComponentInChildren<TextMeshProUGUI>();
        //characterHPTexts[4] = hPBar_Char5.GetComponentInChildren<TextMeshProUGUI>();

        //characterHPs = new RectTransform[5];
        //characterHPs[0] = hPBar_Char1.GetComponentsInChildren<RectTransform>()[1];
        //characterHPs[1] = hPBar_Char2.GetComponentsInChildren<RectTransform>()[1];
        //characterHPs[2] = hPBar_Char3.GetComponentsInChildren<RectTransform>()[1];
        //characterHPs[3] = hPBar_Char4.GetComponentsInChildren<RectTransform>()[1];
        //characterHPs[4] = hPBar_Char5.GetComponentsInChildren<RectTransform>()[1];

        //enemyHPs = new RectTransform[1];
        //enemyHPs[0] = hPBar_Enemy.GetComponentsInChildren<RectTransform>()[1];

        //enemyHPTexts = new TextMeshProUGUI[1];
        //enemyHPTexts[0] = hPBar_Enemy.GetComponentInChildren<TextMeshProUGUI>();

        //EnemyHPBars[0] = hPBar_Enemy;
    }

    /// <summary>
    /// 배틀에서 사용할 UI의 텍스트을 변경하는 메서드입니다.
    /// </summary>
    public void ChangeUIText(BattleTextUIEnum uiEnum, string value)
    {
        texts[(int)uiEnum].text = value;
    }

    public void SettingForHolding()
    {
        for (int i = 0; i < 5; i++)
        {
            DiceManager.Instance.DiceHolding.areas[i] = fixedDiceArea.transform.GetChild(i).gameObject;
        }
    }
}
