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
            battleManager.BattleGroup.ArtifactEffect.EffectWhenFirstTurn();
        }

        UIManager.Instance.BattleUI.BattleUILog.MakeBattleLog(true);
        string stageString = $"{StageManager.Instance.stageSaveData.currentPhaseIndex} - {battleManager.BattleTurn}";
        for (int i = 0; i < battleManager.BattleGroup.BattleEngravings.Length; i++)
        {
            battleManager.BattleGroup.BattleEngravings[i].GetEngravingEffectInTurnEnter();
        }
        battleManager.BattleGroup.ArtifactEffect.EffectPerTurn();
        ChangeDetailedTurnState(DetailedTurnState.Enter);
    }

    public void BattleUpdate()
    {

    }

    public void Exit()
    {        
        DiceManager.Instance.DiceRankBefore = DiceManager.Instance.DiceRank;

        battleManager.EngravingAdditionalValue.AdditionalDamage = 1;
        DiceManager.Instance.AdditionalRollCount = 0;
    }

    public void ChangeDetailedTurnState(DetailedTurnState state)
    {
        battleManager.CurrentPlayerState = state;

        OnOffButton();
    }

    //private void ChangePlayerTurnState()
    //{
    //    switch (battleManager.CurrentPlayerState)
    //    {
    //        case PlayerTurnState.Enter:
    //            battleManager.CurrentPlayerState = PlayerTurnState.Roll;
    //            break;
    //        case PlayerTurnState.Roll:
    //            battleManager.CurrentPlayerState = PlayerTurnState.Confirm;
    //            break;
    //        case PlayerTurnState.Confirm:
    //            battleManager.CurrentPlayerState = PlayerTurnState.EndTurn;
    //            break;
    //    }

    //    OnOffButton();
    //}

    //public void AbstractButtonPushed()
    //{
    //    ChangePlayerTurnState();
    //}

    public void EndPlayerTurn()
    {
        battleManager.StateMachine.currentState = battleManager.I_EnemyTurnState;
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

    //public void EffectEngravings(PlayerTurnState state)
    //{
    //    List<EngravingData> engravings = battleManager.BattleGroup.Engravings;

    //    for (int i = 0; i < engravings.Count; i++)
    //    {

    //    }
    //}
}
