using UnityEngine;
using UnityEngine.UI;

public class BattleButtonConfirm : AbstractBattleButton
{
    [SerializeField] GameObject rankDisplayer;
    [SerializeField] GameObject backBoard;
    [SerializeField] Button confirmButton;
    DiceManager diceManager;

    public override void Setting()
    {
        diceManager = DiceManager.Instance;
    }

    public override void OnOffButton(DetailedTurnState state)
    {
        switch (state)
        {            
            case DetailedTurnState.Enter:
                gameObject.SetActive(true);
                confirmButton.interactable = false;
                break;
            case DetailedTurnState.Roll:
                backBoard.SetActive(true);
                confirmButton.interactable = false;
                break;
            case DetailedTurnState.RollEnd:
                gameObject.SetActive(true);
                confirmButton.interactable = true;
                break;
            case DetailedTurnState.Attack:
                backBoard.SetActive(false);
                confirmButton.interactable = false;
                break;
        }
    }

    public override void OnPush()
    {
        confirmButton.interactable = false;
        rankDisplayer.SetActive(true);
        diceManager.DiceHolding.FixAllDIce();
        diceManager.DiceHolding.isCantFix = true;
        diceManager.DiceBattle.GetDiceWeighting();

        DiceRankingEnum diceRank = diceManager.DiceRank;
        string st = $"{diceRank}\nX{diceManager.DiceBattle.DamageWeightTable[(int)diceRank]}";
        BattleManager.Instance.UIValueChanger.ChangeUIText(BattleTextUIEnum.Rank, st); //일단 이름만
        BattleManager.Instance.BattleTutorial.StartTutorial();
    }

    public void OnPushFinal()
    {
        confirmButton.interactable = true;
        rankDisplayer.SetActive(false);
        //diceManager.Ground.SetActive(false);
        diceManager.DiceBoard.SetActive(false);
        diceManager.HideFakeDice();
        diceManager.DiceHolding.isCantFix = false;

        //BattleManager.Instance.BattlePlayerTurnState.AbstractButtonPushed();
        BattleManager.Instance.BattlePlayerTurnState.ChangeDetailedTurnState(DetailedTurnState.Attack);

        float diceWeighting = DiceManager.Instance.DiceBattle.GetDamageWeighting(); //족보별 계수
        BattleManager battleManager = BattleManager.Instance;
        //공격 애니메이션실행
        battleManager.CharacterAttack.CharacterAttack(diceWeighting);
    }

    public void OnPushCancel()
    {
        confirmButton.interactable = true;
        rankDisplayer.SetActive(false);
        diceManager.DiceHolding.isCantFix = false;
        diceManager.DiceHolding.ReleaseDice();
    }
    
    public void OnPushShowArtifact()
    {

    }
}
