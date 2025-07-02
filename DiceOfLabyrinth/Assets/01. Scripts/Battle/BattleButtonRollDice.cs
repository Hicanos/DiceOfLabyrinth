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
            case PlayerTurnState.BattleEnd:
                gameObject.SetActive(false);
                break;
        }
    }

    public override void OnPush()
    {
        BattleManager.Instance.currentPlayerState = PlayerTurnState.Roll;

        DiceManager.Instance.RollDice();

        BattleManager.Instance.OnOffButton();
    }
}
