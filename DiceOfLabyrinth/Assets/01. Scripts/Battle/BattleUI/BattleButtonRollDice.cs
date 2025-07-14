using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleButtonRollDice : AbstractBattleButton
{
    Button button;
    bool isRollOver = false;
    TextMeshProUGUI text;

    public override void Setting()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public override void OnOffButton(PlayerTurnState state)
    {
        switch (state)
        {
            case PlayerTurnState.BattleStart:
                gameObject.SetActive(true);
                break;
            case PlayerTurnState.Enter:
                ChangeEndTurnToRoll();
                button.interactable = true;
                break;
            case PlayerTurnState.Roll:
                button.interactable = false;
                break;
            case PlayerTurnState.RollEnd:
                if (DiceManager.Instance.RollRemain == 0)
                {
                    button.interactable = false;
                }
                else
                {
                    button.interactable = true;
                }
                break;
            case PlayerTurnState.Confirm:
                button.interactable = false;
                break;
            case PlayerTurnState.ConfirmEnd:
                ChangeRollToEndTurn();
                button.interactable = true;
                //버튼 외형 바꾸기
                break;
            case PlayerTurnState.BattleEnd:
                gameObject.SetActive(false);
                break;
        }
    }

    public override void OnPush()
    {
        DiceManager.Instance.DiceHolding.isCantFix = true;
        if (isRollOver == false)
        {
            DiceManager.Instance.RollDice();

            BattleManager.Instance.UIValueChanger.ChangeUIText(BattleTextUIEnum.Reroll, DiceManager.Instance.RollRemain.ToString());
            DiceManager.Instance.DiceHolding.GetFixedList();
            BattleManager.Instance.battlePlayerTurnState.ChangePlayerTurnState(PlayerTurnState.Roll);
        }
        else
        {
            button.interactable = false;

            BattleManager.Instance.battlePlayerTurnState.EndPlayerTurn();
        }
    }

    private void ChangeRollToEndTurn()
    {
        isRollOver = true;
        text.text = "End Turn";
    }

    private void ChangeEndTurnToRoll()
    {
        isRollOver = false;
        text.text = "Roll";
    }
}
