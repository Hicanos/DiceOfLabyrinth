using UnityEngine;

public enum PlayerTurnState
{
    Enter,
    Roll,
    RollEnd,
    Confirm,
    EndTurn
}

public abstract class AbstractBattleButton : MonoBehaviour
{
    public abstract void GetButtonComponent();
    public abstract void OnOffButton(PlayerTurnState state);
    public abstract void OnPush();    
}
