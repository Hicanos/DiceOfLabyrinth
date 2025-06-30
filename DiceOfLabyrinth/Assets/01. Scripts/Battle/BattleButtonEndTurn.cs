using UnityEngine.UI;

public class BattleButtonEndTurn : AbstractBattleButton
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
            case PlayerTurnState.Enter:
                button.interactable = false;
                break;
            case PlayerTurnState.Roll:

                break;
            case PlayerTurnState.Confirm:
                button.interactable = true;
                break;
            case PlayerTurnState.EndTurn:

                break;
        }
    }

    public override void OnPush()
    {
        BattleManager.Instance.currentPlayerState = PlayerTurnState.EndTurn;

        float diceWeighting = DiceManager.Instance.DiceBattle.GetDiceWeighting(); //족보별 계수

        //CharacterAttack(diceWeighting);

        //공격 애니메이션실행

        BattleManager.Instance.stateMachine.ChangeState(BattleManager.Instance.enemyTurnState);
        BattleManager.Instance.OnOffButton();
    }
}
