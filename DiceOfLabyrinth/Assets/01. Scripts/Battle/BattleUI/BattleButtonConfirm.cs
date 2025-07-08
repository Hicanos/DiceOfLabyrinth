using UnityEngine;
using UnityEngine.UI;

public class BattleButtonConfirm : AbstractBattleButton
{
    GameObject rankDisplayer;
    DiceManager diceManager;
    Button confirmButton;

    public override void GetButtonComponent()
    {
        rankDisplayer = gameObject.transform.GetChild(2).gameObject;
        diceManager = DiceManager.Instance;
        confirmButton = gameObject.GetComponentInChildren<Button>();
    }

    public override void OnOffButton(PlayerTurnState state)
    {
        switch (state)
        {            
            case PlayerTurnState.Enter:
                gameObject.SetActive(false);
                break;
            case PlayerTurnState.Roll:
                gameObject.SetActive(true);
                break;
            case PlayerTurnState.RollEnd:
                gameObject.SetActive(true);
                break;
            case PlayerTurnState.Confirm:
                gameObject.SetActive(false);
                break;
        }
    }

    public override void OnPush()
    {
        confirmButton.interactable = false;
        rankDisplayer.SetActive(true);
        diceManager.DiceHolding.FixAllDIce();
        DiceManager.Instance.DiceHolding.isCantFix = true;
        DiceManager.Instance.DiceBattle.GetDiceWeighting();
        BattleManager.Instance.UIValueChanger.ChangeUIText(BattleTextUIEnum.Rank, diceManager.DiceRank.ToString()); //일단 이름만
    }

    public void OnPushFinal()
    {
        confirmButton.interactable = true;
        rankDisplayer.SetActive(false);
        diceManager.ground.SetActive(false);
        diceManager.DiceBoard.SetActive(false);
        diceManager.HideFakeDice();
        DiceManager.Instance.DiceHolding.isCantFix = false;
        BattleManager.Instance.battlePlayerTurnState.ChangePlayerTurnState(PlayerTurnState.Confirm);
    }

    public void OnPushCancel()
    {
        confirmButton.interactable = true;
        rankDisplayer.SetActive(false);
        DiceManager.Instance.DiceHolding.isCantFix = false;
        diceManager.DiceHolding.ReleaseDice();
    }
}
