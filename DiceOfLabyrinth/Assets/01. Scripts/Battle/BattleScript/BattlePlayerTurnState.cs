public class BattlePlayerTurnState : IBattleTurnState
{
    BattleManager battleManager = BattleManager.Instance;
    public DetailedTurnState DetailedTurnState;

    public void Enter()
    {
        battleManager.BattleTurn++;
        
        battleManager.GetCost(AlivedCharacter());

        if (battleManager.BattleTurn == 1)
        {
            AbstractButtonSetting();
            ChangeDetailedTurnState(DetailedTurnState.BattleStart);
        }
        else
        {
            battleManager.EngravingBuffs.ReduceDuration();
        }

        UIManager.Instance.BattleUI.BattleUILog.MakeBattleLog(true);
        string stageString = $"{StageManager.Instance.stageSaveData.currentPhaseIndex} - {battleManager.BattleTurn}";
        battleManager.CostSpendedInTurn = 0;
        ChangeDetailedTurnState(DetailedTurnState.Enter);
    }

    public void BattleUpdate()
    {

    }

    public void Exit()
    {        
        DiceManager.Instance.DiceRankBefore = DiceManager.Instance.DiceRank;

        //DiceManager.Instance.AdditionalRollCount = 0;
    }

    public void ChangeDetailedTurnState(DetailedTurnState state)
    {
        battleManager.CurrentDetailedState = state;
        battleManager.EngravingBuffs.Action();
        battleManager.ArtifactBuffs.Action();
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

    //public void EffectEngravings(PlayerTurnState state)
    //{
    //    List<EngravingData> engravings = battleManager.BattleGroup.Engravings;

    //    for (int i = 0; i < engravings.Count; i++)
    //    {

    //    }
    //}
}
