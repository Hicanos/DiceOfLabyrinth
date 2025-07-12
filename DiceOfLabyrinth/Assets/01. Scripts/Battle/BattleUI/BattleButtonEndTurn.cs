using UnityEngine.UI;

public class BattleButtonEndTurn : AbstractBattleButton
{
    Button button;
    public override void Setting()
    {
        button = GetComponent<Button>();
    }

    public override void OnOffButton(PlayerTurnState state)
    {
        switch (state)
        {
            case PlayerTurnState.BattleStart:
                gameObject.SetActive(true);
                break;
            case PlayerTurnState.Enter:
                button.interactable = false;
                break;
            case PlayerTurnState.ConfirmEnd:
                button.interactable = true;
                break;
            case PlayerTurnState.EndTurn:
                button.interactable = false;
                break;
            case PlayerTurnState.BattleEnd:
                gameObject.SetActive(false);
                break;
        }
    }

    public override void OnPush()
    {
        button.interactable = false;
        
        BattleManager.Instance.battlePlayerTurnState.EndPlayerTurn();
    }
}
