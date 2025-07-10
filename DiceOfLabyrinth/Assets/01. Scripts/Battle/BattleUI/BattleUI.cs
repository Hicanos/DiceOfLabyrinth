using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] AbstractBattleButton endTurn;
    [SerializeField] AbstractBattleButton roll;
    [SerializeField] AbstractBattleButton char1;
    [SerializeField] AbstractBattleButton char2;
    [SerializeField] AbstractBattleButton char3;
    [SerializeField] AbstractBattleButton char4;
    [SerializeField] AbstractBattleButton char5;
    [SerializeField] AbstractBattleButton patternDisplayer;

    [NonSerialized] public AbstractBattleButton[] Buttons = new AbstractBattleButton[9];

    [Header("Texts For Value Changer")]
    [SerializeField] TextMeshProUGUI cost;
    [SerializeField] TextMeshProUGUI monsterSkillName;
    [SerializeField] TextMeshProUGUI monsterSkillDescription;
    [SerializeField] TextMeshProUGUI monsterHP;
    [SerializeField] TextMeshProUGUI rank;
    [SerializeField] TextMeshProUGUI reRoll;

    [NonSerialized] private TextMeshProUGUI[] texts;

    [Header("HPs For Ratio Changer")]
    [SerializeField] RectTransform hP_Char1;
    [SerializeField] RectTransform hP_Char2;
    [SerializeField] RectTransform hP_Char3;
    [SerializeField] RectTransform hP_Char4;
    [SerializeField] RectTransform hP_Char5;
    [SerializeField] RectTransform hP_Enemy;

    [NonSerialized] private RectTransform[] hps;

    [Header("HP Bars")]
    [SerializeField] Image hPBar_Char1;
    [SerializeField] Image hPBar_Char2;
    [SerializeField] Image hPBar_Char3;
    [SerializeField] Image hPBar_Char4;
    [SerializeField] Image hPBar_Char5;
    [SerializeField] Image hPBar_Enemy;

    [NonSerialized] public Image[] Images = new Image[6];

    public void Setting()
    {
        Buttons[0] = diceBackboard;
        Buttons[1] = endTurn;
        Buttons[2] = roll;
        Buttons[3] = char1;
        Buttons[4] = char2;
        Buttons[5] = char3;
        Buttons[6] = char4;
        Buttons[7] = char5;
        Buttons[8] = patternDisplayer;

        texts = new TextMeshProUGUI[6];
        texts[0] = cost;
        texts[1] = monsterSkillName;
        texts[2] = monsterSkillDescription;
        texts[3] = monsterHP;
        texts[4] = rank;
        texts[5] = reRoll;

        hps = new RectTransform[6];
        hps[0] = hP_Char1;
        hps[1] = hP_Char2;
        hps[2] = hP_Char3;
        hps[3] = hP_Char4;
        hps[4] = hP_Char5;
        hps[5] = hP_Enemy;

        Images[0] = hPBar_Char1;
        Images[1] = hPBar_Char2;
        Images[2] = hPBar_Char3;
        Images[3] = hPBar_Char4;
        Images[4] = hPBar_Char5;
        Images[5] = hPBar_Enemy;
    }

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
        hps[(int)hpEnum].localScale = new Vector3(value, 1, 1);
    }
}
