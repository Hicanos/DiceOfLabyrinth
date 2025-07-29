using UnityEngine;

public class CharacterInfoButton : AbstractBattleButton
{
    static int staticIndex;
    int index;

    private void OnEnable()
    {
        UIManager uIManager = UIManager.Instance;
        index = staticIndex;
        staticIndex++;
        uIManager.BattleUI.Buttons.Add(this);
    }

    private void OnDisable()
    {
        UIManager uIManager = UIManager.Instance;
        uIManager.BattleUI.Buttons.Remove(this);
    }

    public override void Setting()
    {
    }

    public override void OnOffButton(DetailedTurnState state)
    {
        switch(state)
        {
            case DetailedTurnState.Roll:
                gameObject.transform.position = gameObject.transform.position + Vector3.up * 100;
                break;
            case DetailedTurnState.Attack:
                gameObject.transform.position = gameObject.transform.position - Vector3.up * 100;
                break;
        }
    }

    public override void OnPush()
    {
        UIManager.Instance.BattleUI.OpenCharacterInfo(index);
    }
}
