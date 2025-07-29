public class BattleStateEnterBattle : IBattleTurnState
{
    public void Enter()
    {
        UIManager uIManager = UIManager.Instance;
        DiceManager diceManager = DiceManager.Instance;

        uIManager.BattleUI.BattleUILog.MakeLogPool();
        uIManager.BattleUI.battleCanvas.worldCamera = diceManager.DiceCamera;
        diceManager.LoadDiceData();
        InputManager.Instance.BattleInputStart();
        BattleManager.Instance.EnterBattleSettings();
        diceManager.DiceSettingForBattle();

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
