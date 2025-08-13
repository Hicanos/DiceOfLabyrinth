public class BattleStateEnterBattle : IBattleTurnState
{
    public void Enter()
    {
        UIManager uIManager = UIManager.Instance;
        DiceManager diceManager = DiceManager.Instance;
        BattleManager battleManager = BattleManager.Instance;

        uIManager.BattleUI.BattleUILog.MakeLogPool();
        uIManager.BattleUI.battleCanvas.worldCamera = diceManager.DiceCamera;
        diceManager.LoadDiceData();
        InputManager.Instance.BattleInputStart();
        battleManager.EnterBattleSettings();
        diceManager.DiceSettingForBattle();
        battleManager.Enemy.PassiveContainer.ActionPassiveBattleStart();
        AbstractButtonSetting();
    }
    public void BattleUpdate()
    {
        
    }

    public void Exit()
    {
        
    }

    private void AbstractButtonSetting()
    {
        foreach (AbstractBattleButton button in UIManager.Instance.BattleUI.Buttons)
        {
            button.Setting();
            button.ActiveButton();
        }
    }
}
