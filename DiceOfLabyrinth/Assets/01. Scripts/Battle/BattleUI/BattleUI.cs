using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;

public class BattleUI : MonoBehaviour
{
    public GameObject fixedDiceArea;
    public GameObject victoryUI;
    public GameObject defeatUI;
    public GameObject stagmaDisplayer;
    public GameObject CharacterInfo;
    public Canvas     battleCanvas;
    [SerializeField] BattleUICharacterInfo characterInfoUI;
    public BattleUILog BattleUILog;
    public GameObject BattleLogPrefab;

    [Header("AbstractButtons")]
    [SerializeField] AbstractBattleButton diceBackboard;
    [SerializeField] AbstractBattleButton roll;
    [SerializeField] AbstractBattleButton char1;
    [SerializeField] AbstractBattleButton char2;
    [SerializeField] AbstractBattleButton char3;
    [SerializeField] AbstractBattleButton char4;
    [SerializeField] AbstractBattleButton char5;
    [SerializeField] AbstractBattleButton patternDisplayer;

    public AbstractBattleButton Roll => roll;
    [NonSerialized] public List<AbstractBattleButton> Buttons = new List<AbstractBattleButton>();

    [Header("Texts For Value Changer")]
    [SerializeField] TextMeshProUGUI cost;
    [SerializeField] TextMeshProUGUI monsterSkillName;
    [SerializeField] TextMeshProUGUI monsterSkillDescription;
    [SerializeField] TextMeshProUGUI rank;
    [SerializeField] TextMeshProUGUI reRoll;

    [NonSerialized] private TextMeshProUGUI[] texts;

    public void Setting()
    {
        Buttons.Add(diceBackboard);
        Buttons.Add(roll);
        Buttons.Add(char1);
        Buttons.Add(char2);
        Buttons.Add(char3);
        Buttons.Add(char4);
        Buttons.Add(char5);
        Buttons.Add(patternDisplayer);

        texts = new TextMeshProUGUI[5];
        texts[0] = cost;
        texts[1] = monsterSkillName;
        texts[2] = monsterSkillDescription;
        texts[3] = rank;
        texts[4] = reRoll;
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

    public void OpenCharacterInfo(int index)
    {
        characterInfoUI.UpdateCharacterInfo(index);

        CharacterInfo.SetActive(true);
    }
}
