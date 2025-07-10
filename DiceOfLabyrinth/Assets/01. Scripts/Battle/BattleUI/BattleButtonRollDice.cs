using UnityEngine.UI;

public class BattleButtonRollDice : AbstractBattleButton
{
    Button button;
    public override void Setting()
    {
        button = GetComponent<Button>();
    }

    public override void OnOffButton(PlayerTurnState state)
    {
        switch (state)
        {
            case PlayerTurnState.BattleStart:
                gameObject.SetActive(true);
                break;
            case PlayerTurnState.Enter:
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
            case PlayerTurnState.BattleEnd:
                gameObject.SetActive(false);
                break;
        }
    }

    public override void OnPush()
    {
        DiceManager.Instance.RollDice();

        BattleManager.Instance.UIValueChanger.ChangeUIText(BattleTextUIEnum.Reroll, DiceManager.Instance.RollRemain.ToString());
        DiceManager.Instance.DiceHolding.GetFixedList();
        BattleManager.Instance.battlePlayerTurnState.ChangePlayerTurnState(PlayerTurnState.Roll);
    }
}
