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
            case DetailedTurnState.Enter:
                ChangeEndTurnToRoll();
                rollButton.interactable = true;
                break;
            case DetailedTurnState.Roll:
                rollButton.interactable = false;
                UIManager.Instance.BattleUI.TempButton();
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
            case DetailedTurnState.Attack:
                rollButton.interactable = false;
                UIManager.Instance.BattleUI.TempButton();
                break;
            case DetailedTurnState.AttackEnd:
                rollButton.interactable = true;
                ChangeRollToEndTurn();
                break;
        }
    }

    public override void OnPush()
    {
        BattleManager battleManager = BattleManager.Instance;
        DiceManager diceManager = DiceManager.Instance;

        diceManager.DiceHolding.isCantFix = true;
        if (isRollOver == false)
        {
            diceManager.RollDice();

            battleManager.UIValueChanger.ChangeUIText(BattleTextUIEnum.Reroll, diceManager.RollRemain.ToString());
            diceManager.DiceHolding.GetFixedList();
            
            battleManager.BattlePlayerTurnState.ChangeDetailedTurnState(DetailedTurnState.Roll);
        }
        else
        {
            rollButton.interactable = false;

            battleManager.BattlePlayerTurnState.ChangeDetailedTurnState(DetailedTurnState.EndTurn);
            battleManager.EndPlayerTurn();
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
