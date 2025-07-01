using UnityEngine;

public class BattlePlayerTurnState : IBattleTurnState
{
    BattleManager battleManager = BattleManager.Instance;

    public void Enter()
    {
        battleManager.BattleTurn++;
        Debug.Log($"Turn{BattleManager.Instance.BattleTurn}");
        Debug.Log("Player's turn");

        //battleManager.GetCost(AlivedCharacter());

        if (battleManager.BattleTurn == 1)
        {
            battleManager.GetButton();
            battleManager.LoadMonsterPattern.Load();
        }

        battleManager.LoadMonsterPattern.PrepareSkill();

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
