using UnityEngine;

public enum PlayerTurnState
{
    BattleStart,
    Enter,
    Roll,
    RollEnd,
    Confirm,
    EndTurn,
    BattleEnd
}

public abstract class AbstractBattleButton : MonoBehaviour
{
    public abstract void Setting();
    public abstract void OnOffButton(PlayerTurnState state);
    public abstract void OnPush();    
}
