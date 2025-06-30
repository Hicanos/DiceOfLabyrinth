using UnityEngine.UI;

public class BattleButtonConfirm : AbstractBattleButton
{
    Button button;
    public override void GetButtonComponent()
    {
        button = GetComponentInChildren<Button>();
    }

    public override void OnOffButton(PlayerTurnState state)
    {
        switch (state)
        {
            case PlayerTurnState.Enter:
                button.interactable = false;
                break;
            case PlayerTurnState.Roll:
                button.interactable = true;
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
        BattleManager.Instance.currentPlayerState = PlayerTurnState.Confirm;

        DiceManager.Instance.ground.SetActive(false);
        DiceManager.Instance.DiceBoard.SetActive(false);
        DiceManager.Instance.HideFakeDice();
        DiceManager.Instance.diceBackground.gameObject.SetActive(false);

        BattleManager.Instance.OnOffButton();
    }
}
