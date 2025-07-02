using UnityEngine;

public class BattlePlayerTurnState : IBattleTurnState
{
    BattleManager battleManager = BattleManager.Instance;

    public void Enter()
    {
        battleManager.BattleTurn++;
        Debug.Log($"Turn{BattleManager.Instance.BattleTurn}");
        Debug.Log("Player's turn");

        BattleManager.Instance.battleCoroutine.GetMonsterPattern();
        //battleManager.GetCost(AlivedCharacter());

        if (battleManager.BattleTurn == 1)
        {
            battleManager.currentPlayerState = PlayerTurnState.BattleStart;
            battleManager.OnOffButton();
            battleManager.GetButton();            

            battleManager.LoadMonsterPattern.Load();
        }

        battleManager.currentPlayerState = PlayerTurnState.Enter;
        battleManager.OnOffButton();
    }

    public void BattleUpdate()
    {

    }

    public void Exit()
    {
        
    }    

    private int AlivedCharacter() //작성 필요
    {
        int num = 0;

        foreach (BattleCharacter character in battleManager.entryCharacters)
        {
            if(character.isDied == false)
            {
                num++;
            }
        }
        return num;
    }
}
