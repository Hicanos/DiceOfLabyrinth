public class BattleButtonConfirm : AbstractBattleButton
{
    public override void GetButtonComponent()
    {

    }

    public override void OnOffButton(PlayerTurnState state)
    {
        switch (state)
        {            
            case PlayerTurnState.Enter:
                gameObject.SetActive(false);
                break;
            case PlayerTurnState.Roll:
                gameObject.SetActive(true);
                break;
            case PlayerTurnState.RollEnd:
                gameObject.SetActive(true);
                break;
            case PlayerTurnState.Confirm:
                gameObject.SetActive(false);
                break;
        }
    }

    public override void OnPush()
    {
        DiceManager.Instance.ground.SetActive(false);
        DiceManager.Instance.DiceBoard.SetActive(false);
        DiceManager.Instance.HideFakeDice();

        BattleManager.Instance.battlePlayerTurnState.ChangePlayerTurnState(PlayerTurnState.Confirm);
    }
}
