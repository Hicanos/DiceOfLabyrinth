public class BattleStatePlayerTurn : IBattleTurnState
{
    const int numFive = 5;
    BattleManager battleManager = BattleManager.Instance;
    UseBuff useBuff = new UseBuff();
    public void Enter()
    {
        UnityEngine.Debug.Log("enter");
        battleManager.BattleTurn++;
        
        battleManager.GetCost(AlivedCharacter());
        
        battleManager.EngravingBuffs.ReduceDuration();

        UIManager.Instance.BattleUI.BattleUILog.WriteBattleLog(true);
        battleManager.CostSpendedInTurn = 0;
        ChangeDetailedTurnState(DetailedTurnState.Enter);
    }

    public void BattleUpdate()
    {

    }

    public void Exit()
    {        
        DiceManager.Instance.DiceRankBefore = DiceManager.Instance.DiceRank;
    }

    public void ChangeDetailedTurnState(DetailedTurnState state)
    {
        battleManager.BattleTutorial.StartTutorial((int)state);

        battleManager.CurrentDetailedState = state;
        battleManager.ArtifactBuffs.Action();
        useBuff.UseBuffs(state);
        OnOffButton();
    }

    public void EndPlayerTurn()
    {
        battleManager.StateMachine.currentState = battleManager.I_EnemyTurnState;
    }

    private void OnOffButton()
    {
        foreach (AbstractBattleButton button in UIManager.Instance.BattleUI.Buttons)
        {
            button.OnOffButton(battleManager.CurrentDetailedState);
        }
    }

    private int AlivedCharacter()
    {
        int num = numFive - battleManager.PartyData.DeadIndex.Count;

        return num;
    }
}
