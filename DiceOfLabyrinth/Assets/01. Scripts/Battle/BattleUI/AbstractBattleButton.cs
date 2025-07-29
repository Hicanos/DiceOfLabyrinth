using UnityEngine;

public enum DetailedTurnState
{
    Enter,
    Roll,           //주사위가 굴러가고 있는 상태
    RollEnd,
    Attack,
    AttackEnd,     //플레이어 공격 모션이 끝나면
    EndTurn
}

public abstract class AbstractBattleButton : MonoBehaviour
{
    public abstract void Setting();
    public abstract void OnOffButton(DetailedTurnState state);
    public abstract void OnPush();
    public void ActiveButton()
    {
        gameObject.SetActive(true);
    }
    public void DeactiveButton()
    {
        gameObject.SetActive(false);
    }
}
