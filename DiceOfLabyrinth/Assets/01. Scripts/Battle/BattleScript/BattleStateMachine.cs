using UnityEngine.AI;

public interface IBattleTurnState
{
    void Enter();
    void BattleUpdate();
    void Exit();
}

public class BattleStateMachine
{
    public IBattleTurnState currentState;    

    public void ChangeState(IBattleTurnState state)
    {
        if(currentState != null)
        {
            currentState.Exit();
        }
        currentState = state;
        currentState.Enter();
    }

    public void BattleUpdate()
    {
        if (BattleManager.Instance.IsBattle == false) return;
        currentState.BattleUpdate();
    }
}
