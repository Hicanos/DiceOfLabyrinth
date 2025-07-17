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

    public override void OnOffButton(PlayerTurnState state)
    {
        switch (state)
        {
            case PlayerTurnState.BattleStart:
                gameObject.SetActive(true);
                break;
            case PlayerTurnState.Enter:
                ChangeEndTurnToRoll();
                rollButton.interactable = true;
                break;
            case PlayerTurnState.Roll:
                rollButton.interactable = false;
                break;
            case PlayerTurnState.RollEnd:
                if (DiceManager.Instance.RollRemain == 0)
                {
                    rollButton.interactable = false;
                }
                else
                {
                    rollButton.interactable = true;
                }
                break;
            case PlayerTurnState.Confirm:
                rollButton.interactable = false;
                break;
            case PlayerTurnState.ConfirmEnd:
                ChangeRollToEndTurn();
                rollButton.interactable = true;
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
            BattleManager.Instance.BattlePlayerTurnState.ChangePlayerTurnState(PlayerTurnState.Roll);
        }
        else
        {
            rollButton.interactable = false;

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
