using UnityEngine;
using UnityEngine.UI;

public class BattleButtonSkill : AbstractBattleButton
{
    BattleCharacter character;
    Button button;
    int index;
    public override void Setting()
    {
        GetIndex();
        button = GetComponent<Button>();
        character = BattleManager.Instance.BattleGroup.BattleCharacters[index-1];
    }

    public override void OnOffButton(DetailedTurnState state)
    {
        switch (state)
        {
            case DetailedTurnState.Enter:
                button.interactable = true;
                break;
            case DetailedTurnState.Attack:
                button.interactable = false;
                break;
            case DetailedTurnState.AttackEnd:
                button.interactable = true;
                break;
            case DetailedTurnState.EndTurn:
                button.interactable = false;
                break;
        }
    }

    public override void OnPush()
    {
        Debug.Log(character.CharNameKr + " 스킬 사용");
        character.UseActiveSkill(BattleManager.Instance.BattleGroup.BattleCharacters, BattleManager.Instance.Enemy);       
    }

    private void GetIndex()
    {
        index = gameObject.transform.GetSiblingIndex();
    }
}
