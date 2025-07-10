public class BattlePlayerTurnState : IBattleTurnState
{
    BattleManager battleManager = BattleManager.Instance;

    public void Enter()
    {
        battleManager.BattleTurn++;
        
        battleManager.GetCost(AlivedCharacter());

        if (battleManager.BattleTurn == 1)
        {
            Setting();
            ChangePlayerTurnState(PlayerTurnState.BattleStart);            

            //battleManager.LoadMonsterPattern.Load();
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
        battleManager.currentPlayerState = state;
        OnOffButton();
    }

    private void OnOffButton()
    {
        foreach (AbstractBattleButton button in battleManager.BattleButtons)
        {
            button.OnOffButton(battleManager.currentPlayerState);
        }
    }

    public void Setting()
    {
        foreach (AbstractBattleButton button in battleManager.BattleButtons)
        {
            button.Setting();
        }
    }

    private int AlivedCharacter()
    {
        int num = 0;

        foreach (BattleCharacter character in battleManager.battleCharacters)
        {
            if (character.IsDied == false)
            {
                num++;
            }
        }
        return num;
    }
}
