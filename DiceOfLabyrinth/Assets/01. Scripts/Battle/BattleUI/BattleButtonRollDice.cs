using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleButtonRollDice : AbstractBattleButton
{
    [SerializeField] Button rollButton;
    [SerializeField] TextMeshProUGUI text;
    bool isRollOver = false;

    public override void Setting()
    {

    }

    public override void OnOffButton(DetailedTurnState state)
    {
        switch (state)
        {
            case DetailedTurnState.BattleStart:
                gameObject.SetActive(true);
                break;
            case DetailedTurnState.Enter:
                ChangeEndTurnToRoll();
                rollButton.interactable = true;
                break;
            case DetailedTurnState.Roll:
                rollButton.interactable = false;
                break;
            case DetailedTurnState.RollEnd:
                if (DiceManager.Instance.RollRemain == 0)
                {
                    rollButton.interactable = false;
                }
                else
                {
                    rollButton.interactable = true;
                }
                break;
            case DetailedTurnState.Confirm:
                rollButton.interactable = false;
                break;
            case DetailedTurnState.ConfirmEnd:
                ChangeRollToEndTurn();
                rollButton.interactable = true;
                break;
            case DetailedTurnState.EndTurn:
                rollButton.interactable = false;
                break;
            case DetailedTurnState.BattleEnd:
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

            //BattleManager.Instance.BattlePlayerTurnState.AbstractButtonPushed();
            BattleManager.Instance.BattlePlayerTurnState.ChangeDetailedTurnState(DetailedTurnState.Roll);
        }
        else
        {
            rollButton.interactable = false;

            //BattleManager.Instance.BattlePlayerTurnState.AbstractButtonPushed();
            BattleManager.Instance.BattlePlayerTurnState.EndPlayerTurn();
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
