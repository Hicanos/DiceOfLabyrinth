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
            case PlayerTurnState.BattleStart:
                gameObject.SetActive(true);
                break;
            case PlayerTurnState.Enter:
                button.interactable = false;
                break;
            case PlayerTurnState.Confirm:
                button.interactable = true;
                break;
            case PlayerTurnState.EndTurn:
                button.interactable = false;
                break;
            case PlayerTurnState.BattleEnd:
                gameObject.SetActive(false);
                break;
        }
    }

    public override void OnPush()
    {
        BattleManager.Instance.currentPlayerState = PlayerTurnState.EndTurn;

        float diceWeighting = DiceManager.Instance.DiceBattle.GetDiceWeighting(); //족보별 계수

        //BattleManager.Instance.CharacterAttack(diceWeighting);
        //CharacterAttack(diceWeighting);

        //공격 애니메이션실행

        BattleManager.Instance.stateMachine.ChangeState(BattleManager.Instance.enemyTurnState);
        BattleManager.Instance.OnOffButton();
    }
}
