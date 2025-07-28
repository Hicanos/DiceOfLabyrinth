public class BattleStateFinishBattle : IBattleTurnState
{
    public void BattleUpdate()
    {
        
    }

    public void Enter()
    {
        DiceManager.Instance.ResetSetting();
        InputManager.Instance.BattleInputEnd();
        UIManager.Instance.BattleUI.BattleUILog.TurnOffAllLogs();
        UIManager.Instance.BattleUI.battleCanvas.worldCamera = null;

        BattleManager.Instance.FinishBattleSetting();
        DeactiveAbstractButtons();
    }

    public void Exit()
    {
        
    }

    private void DeactiveAbstractButtons()
    {
        foreach (AbstractBattleButton button in UIManager.Instance.BattleUI.Buttons)
        {
            button.DeactiveButton();
        }
    }
}
