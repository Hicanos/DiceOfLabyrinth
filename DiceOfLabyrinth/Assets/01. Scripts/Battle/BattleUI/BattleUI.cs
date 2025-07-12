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
    [SerializeField] TextMeshProUGUI rank;
    [SerializeField] TextMeshProUGUI reRoll;

    [NonSerialized] private TextMeshProUGUI[] texts;

    [NonSerialized] private RectTransform[]   characterHPs;
    [NonSerialized] private TextMeshProUGUI[] characterHPTexts;

    [NonSerialized] private RectTransform[]   enemyHPs;
    [NonSerialized] private TextMeshProUGUI[] enemyHPTexts;

    [Header("Character HP Bars")]
    [SerializeField] Image hPBar_Char1;
    [SerializeField] Image hPBar_Char2;
    [SerializeField] Image hPBar_Char3;
    [SerializeField] Image hPBar_Char4;
    [SerializeField] Image hPBar_Char5;

    [NonSerialized] public Image[] CharacterHPBars = new Image[5];

    [Header("Enemy HP Bars")]
    [SerializeField] Image hPBar_Enemy;

    [NonSerialized] public Image[] EnemyHPBars = new Image[1];

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

        texts = new TextMeshProUGUI[5];
        texts[0] = cost;
        texts[1] = monsterSkillName;
        texts[2] = monsterSkillDescription;
        texts[3] = rank;
        texts[4] = reRoll;

        characterHPTexts = new TextMeshProUGUI[5];
        characterHPTexts[0] = hPBar_Char1.GetComponentInChildren<TextMeshProUGUI>();
        characterHPTexts[1] = hPBar_Char2.GetComponentInChildren<TextMeshProUGUI>();
        characterHPTexts[2] = hPBar_Char3.GetComponentInChildren<TextMeshProUGUI>();
        characterHPTexts[3] = hPBar_Char4.GetComponentInChildren<TextMeshProUGUI>();
        characterHPTexts[4] = hPBar_Char5.GetComponentInChildren<TextMeshProUGUI>();

        characterHPs = new RectTransform[5];
        characterHPs[0] = hPBar_Char1.GetComponentsInChildren<RectTransform>()[1];
        characterHPs[1] = hPBar_Char2.GetComponentsInChildren<RectTransform>()[1];
        characterHPs[2] = hPBar_Char3.GetComponentsInChildren<RectTransform>()[1];
        characterHPs[3] = hPBar_Char4.GetComponentsInChildren<RectTransform>()[1];
        characterHPs[4] = hPBar_Char5.GetComponentsInChildren<RectTransform>()[1];

        enemyHPs = new RectTransform[1];
        enemyHPs[0] = hPBar_Enemy.GetComponentsInChildren<RectTransform>()[1];

        enemyHPTexts = new TextMeshProUGUI[1];
        enemyHPTexts[0] = hPBar_Enemy.GetComponentInChildren<TextMeshProUGUI>();

        CharacterHPBars[0] = hPBar_Char1;
        CharacterHPBars[1] = hPBar_Char2;
        CharacterHPBars[2] = hPBar_Char3;
        CharacterHPBars[3] = hPBar_Char4;
        CharacterHPBars[4] = hPBar_Char5;

        EnemyHPBars[0] = hPBar_Enemy;
    }

    /// <summary>
    /// 배틀에서 사용할 UI의 텍스트을 변경하는 메서드입니다.
    /// </summary>
    public void ChangeUIText(BattleTextUIEnum uiEnum, string value)
    {
        texts[(int)uiEnum].text = value;
    }

    /// <summary>
    /// 체력바 텍스트를 변경하는 메서드입니다.
    /// </summary>
    public void ChangeUIText(HPEnumCharacter uiEnum, string value)
    {
        characterHPTexts[(int)uiEnum].text = value;
    }

    /// <summary>
    /// 체력바 텍스트를 변경하는 메서드입니다.
    /// </summary>
    public void ChangeUIText(HPEnumEnemy uiEnum, string value)
    {
        enemyHPTexts[(int)uiEnum].text = value;
    }

    /// <summary>
    /// 캐릭터의 체력바 비율을 변경하는 메서드입니다.
    /// </summary>
    public void ChangeCharacterHpRatio(HPEnumCharacter hpEnum, float value)
    {
        characterHPs[(int)hpEnum].localScale = new Vector3(value, 1, 1);
    }

    /// <summary>
    /// 에너미의 체력바 비율을 변경하는 메서드입니다.
    /// </summary>
    public void ChangeEnemyHpRatio(HPEnumEnemy hpEnum, float value)
    {
        enemyHPs[0].localScale = new Vector3(value, 1, 1);
    }
}
