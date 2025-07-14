using UnityEngine;

public enum PlayerTurnState
{
    BattleStart,
    Enter,
    Roll,           //주사위가 굴러가고 있는 상태
    RollEnd,
    Confirm,
    ConfirmEnd,     //플레이어 공격 모션이 끝나면
    EndTurn,
    BattleEnd
}

public abstract class AbstractBattleButton : MonoBehaviour
{
    public abstract void Setting();
    public abstract void OnOffButton(PlayerTurnState state);
    public abstract void OnPush();    
}
