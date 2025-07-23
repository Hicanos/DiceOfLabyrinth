using UnityEngine;

public class CharacterInfoButton : AbstractBattleButton
{
    static int staticIndex;
    int index;

    private void OnEnable()
    {
        index = staticIndex;
        staticIndex++;
        UIManager.Instance.BattleUI.Buttons.Add(this);
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
            case DetailedTurnState.Confirm:
                gameObject.transform.position = gameObject.transform.position - Vector3.up * 100;
                break;
        }
    }

    public override void OnPush()
    {
        UIManager.Instance.BattleUI.OpenCharacterInfo(index);
    }
}
