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
            GetButton();
            ChangePlayerTurnState(PlayerTurnState.BattleStart);            

            battleManager.LoadMonsterPattern.Load();
        }

        ChangePlayerTurnState(PlayerTurnState.Enter);
    }

    public void BattleUpdate()
    {

    }

    public void Exit()
    {
        
    }

    public void ChangePlayerTurnState(PlayerTurnState state)
    {
        battleManager.currentPlayerState = state;
        OnOffButton();
    }

    private void OnOffButton()
    {
        foreach (AbstractBattleButton button in battleManager.BattleButtons)
        {
            button.OnOffButton(battleManager.currentPlayerState);
        }
    }

    public void GetButton()
    {
        foreach (AbstractBattleButton button in battleManager.BattleButtons)
        {
            button.GetButtonComponent();
        }
    }

    //private int AlivedCharacter() //작성 필요
    //{
    //    int num = 0;

    //    foreach (BattleCharacter character in battleManager.entryCharacters)
    //    {
    //        if(character.IsDied == false)
    //        {
    //            num++;
    //        }
    //    }
    //    return num;
    //}
}
