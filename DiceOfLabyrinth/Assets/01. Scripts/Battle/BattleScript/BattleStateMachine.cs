public interface IBattleTurnState
{
    void Enter();
    void BattleUpdate();
    void Exit();
}

public class BattleStateMachine
{
    public IBattleTurnState currentState;

    public BattleStateMachine(IBattleTurnState defaultState)
    {
        currentState = defaultState;
    }
    
    public void ChangeState(IBattleTurnState state)
    {
        currentState.Exit();
        currentState = state;
        currentState.Enter();
    }

    public void BattleUpdate()
    {
        currentState.BattleUpdate();
    }
}
