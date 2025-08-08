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

        // 족보/시그니처 정보 확인 및 버프 적용 예시
        var diceRank = diceManager.DiceRank;

        // 족보에 맞는 패시브 스킬 자동 발동
        // 캐릭터가 가진 스킬 중 스킬 B의 발동 조건이 족보와 일치하는 경우

        // SumOver에 해당하는 주사위 값 계산 

        //int sumOver = 0;

        //for (int i = 0; i < diceManager.DiceResultCount.Length; i++)
        //{
        //    sumOver += diceManager.DiceResultCount[i] * (i + 1);
        //}


        //var signitureAmount = diceManager.SignitureAmount;
        // var signitureIndex = diceManager.SignitureIndex;


        // TODO: 여기서 조건에 맞는 패시브 스킬 자동 발동

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
