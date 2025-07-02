using UnityEngine;
using UnityEngine.UI;

public class BattleButtonSkill : AbstractBattleButton
{
    Button button;
    //int index;
    public override void GetButtonComponent()
    {
        button = GetComponent<Button>();
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
            case PlayerTurnState.Roll:
                button.interactable = false;
                break;
            case PlayerTurnState.Confirm:
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
        Debug.Log("캐릭터 스킬 사용");
    }
}
