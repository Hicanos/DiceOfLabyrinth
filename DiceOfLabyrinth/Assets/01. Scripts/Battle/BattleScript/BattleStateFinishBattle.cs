using System.Collections.Generic;

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
        BattleManager battleManager = BattleManager.Instance;
        UIManager uIManager = UIManager.Instance;
        List<AbstractBattleButton> buttons = uIManager.BattleUI.Buttons;
        for (int i = buttons.Count -1; i >= 0; i--)
        {
            buttons[i].DeactiveButton();
        }
        //foreach (AbstractBattleButton button in UIManager.Instance.BattleUI.Buttons)
        //{
        //    button.DeactiveButton();
        //}
    }
}
