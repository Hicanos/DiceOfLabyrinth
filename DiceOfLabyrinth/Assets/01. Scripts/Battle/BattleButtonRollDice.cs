using UnityEngine.UI;

public class BattleButtonRollDice : AbstractBattleButton
{
    Button button;
    public override void GetButtonComponent()
    {
        button = GetComponent<Button>();
    }

    public override void OnOffButton(PlayerTurnState state)
    {
        switch(state)
        {
            case PlayerTurnState.Enter:
                if (DiceManager.Instance.isRolling == false)
                {
                    button.interactable = true;
                }
                break;
            case PlayerTurnState.Roll:
                UnityEngine.Debug.Log("작동");
                button.interactable = false;
                break;
            case PlayerTurnState.RollEnd:
                if (DiceManager.Instance.rollCount == DiceManager.Instance.maxRollCount)
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
            case PlayerTurnState.EndTurn:

                break;
        }
    }

    public override void OnPush()
    {
        BattleManager.Instance.currentPlayerState = PlayerTurnState.Roll;

        DiceManager.Instance.RollDice();
        DiceManager.Instance.diceBackground.gameObject.SetActive(true);

        BattleManager.Instance.OnOffButton();
    }
}
