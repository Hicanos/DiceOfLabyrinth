using UnityEngine;

public class BattlePlayerTurnState : IBattleTurnState
{
    BattleManager battleManager = BattleManager.Instance;
    public void Enter()
    {
        GetCost();
        battleManager.DiceRollButton.enabled = true;
        battleManager.TurnEndButton.enabled = true;
    }

    public void BattleUpdate()
    {
        
    }

    public void Exit()
    {
        battleManager.DiceRollButton.enabled = false;
        battleManager.TurnEndButton.enabled = false;
    }

    public void Roll()
    {
        DiceManager.Instance.RollDice();
    }

    private void GetCost()
    {
        int cost = battleManager.CurrnetCost;
        
        cost = Mathf.Clamp(cost + AlivedCharacter(), 0, battleManager.MaxCost);

        battleManager.CurrnetCost = cost;
    }

    private int AlivedCharacter()
    {
        int num = 0;

        return num;
    }
}
