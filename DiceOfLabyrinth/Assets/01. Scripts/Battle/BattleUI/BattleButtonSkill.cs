using UnityEngine;
using UnityEngine.TextCore.Text;
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
        character = BattleManager.Instance.BattleGroup.BattleCharacters[index];
    }

    public override void OnOffButton(PlayerTurnState state)
    {
        switch (state)
        {
            case PlayerTurnState.BattleStart:
                gameObject.transform.parent.gameObject.SetActive(true);
                break;
            case PlayerTurnState.Enter:
                button.interactable = true;
                break;
            case PlayerTurnState.Confirm:
                button.interactable = false;
                break;
            case PlayerTurnState.ConfirmEnd:
                button.interactable = true;
                break;
            case PlayerTurnState.EndTurn:
                button.interactable = false;
                break;
            case PlayerTurnState.BattleEnd:
                gameObject.transform.parent.gameObject.SetActive(false);
                break;
        }
    }

    public override void OnPush()
    {
        Debug.Log(character.CharNameKr + " 스킬 사용");
    }

    private void GetIndex()
    {
        index = gameObject.transform.GetSiblingIndex();
    }
}
