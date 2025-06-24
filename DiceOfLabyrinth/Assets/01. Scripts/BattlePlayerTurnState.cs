using UnityEngine;
using UnityEngine.UI;

public class BattlePlayerTurnState : IBattleTurnState
{
    BattleManager battleManager = BattleManager.Instance;
    public void Enter()
    {
        GetCost();
        battleManager.DiceRollButton.interactable = true;        
        battleManager.TurnEndButton.interactable = true;
    }

    public void BattleUpdate()
    {
        
    }

    public void Exit()
    {
        battleManager.DiceRollButton.interactable = false;
        battleManager.TurnEndButton.interactable = false;
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
