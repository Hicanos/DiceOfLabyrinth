public class BattlePlayerTurnState : IBattleTurnState
{
    BattleManager battleManager = BattleManager.Instance;

    public void Enter()
    {
        battleManager.BattleTurn++;
        
        battleManager.GetCost(AlivedCharacter());

        if (battleManager.BattleTurn == 1)
        {
            AbstractButtonSetting();
            ChangePlayerTurnState(PlayerTurnState.BattleStart);
        }

        string stageString = $"{StageManager.Instance.stageSaveData.currentPhaseIndex} - {battleManager.BattleTurn}";
        ChangePlayerTurnState(PlayerTurnState.Enter);
    }

    public void BattleUpdate()
    {

    }

    public void Exit()
    {
        
    }

    public void ChangePlayerTurnState(PlayerTurnState state)
    {
        battleManager.CurrentPlayerState = state;
        OnOffButton();
    }

    public void EndPlayerTurn()
    {
        battleManager.StateMachine.ChangeState(battleManager.I_EnemyTurnState);
    }

    private void OnOffButton()
    {
        foreach (AbstractBattleButton button in UIManager.Instance.BattleUI.Buttons)
        {
            button.OnOffButton(battleManager.CurrentPlayerState);
        }
    }

    public void AbstractButtonSetting()
    {
        foreach (AbstractBattleButton button in UIManager.Instance.BattleUI.Buttons)
        {
            button.Setting();
        }
    }

    private int AlivedCharacter()
    {
        int num = 0;

        foreach (BattleCharacter character in battleManager.BattleGroup.BattleCharacters)
        {
            if (character.IsDied == false)
            {
                num++;
            }
        }
        return num;
    }
}
