using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public GameObject fixedDiceArea;
    public FakeDiceHolding FakeDiceHolding;
    public GameObject victoryUI;
    public GameObject defeatUI;
    public GameObject CharacterInfo;
    public Canvas     battleCanvas;
    [SerializeField] BattleUICharacterInfo characterInfoUI;
    public BattleUILog      BattleUILog;
    public GameObject       BattleLogPrefab;
    [SerializeField] GameObject       tutorialBoard;
    [SerializeField] TextMeshProUGUI  tutorialText;
    public Button TutorialPushButton;
    public Button TutorialSkipButton;
    [SerializeField] Button tempInventoryButton;
    [SerializeField] Button tempPauseButton;

    [Header("AbstractButtons")]
    [SerializeField] AbstractBattleButton diceBackboard;
    [SerializeField] AbstractBattleButton roll;
    [SerializeField] AbstractBattleButton char1;
    [SerializeField] AbstractBattleButton char2;
    [SerializeField] AbstractBattleButton char3;
    [SerializeField] AbstractBattleButton char4;
    [SerializeField] AbstractBattleButton char5;
    [SerializeField] AbstractBattleButton characters;
    [SerializeField] AbstractBattleButton patternDisplayer;
    [SerializeField] AbstractBattleButton battleLog;

    public AbstractBattleButton Roll => roll;
    [NonSerialized] public List<AbstractBattleButton> Buttons = new List<AbstractBattleButton>();

    [Header("Texts For Value Changer")]
    [SerializeField] TextMeshProUGUI cost;
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
        Buttons.Add(characters);
        Buttons.Add(patternDisplayer);
        Buttons.Add(battleLog);

        texts = new TextMeshProUGUI[3];
        texts[0] = cost;
        texts[1] = rank;
        texts[2] = reRoll;
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

    public void TutorialBoardSetActive(bool value)
    {
        tutorialBoard.SetActive(value);
    }

    public void ChangeTutorialText(string text)
    {
        tutorialText.text = text;
    }

    public void TempButton()
    {
        tempInventoryButton.enabled = !tempInventoryButton.enabled;
        tempPauseButton.enabled = !tempPauseButton.enabled;
    }
}