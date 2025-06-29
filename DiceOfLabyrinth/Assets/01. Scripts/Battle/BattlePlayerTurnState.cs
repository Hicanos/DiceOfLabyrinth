using UnityEngine;

public class BattlePlayerTurnState : IBattleTurnState
{
    BattleManager battleManager = BattleManager.Instance;

    float RankWeighting = 0;
    public void Enter()
    {
        battleManager.BattleTurn++;
        Debug.Log($"Turn{BattleManager.Instance.BattleTurn}");
        Debug.Log("Player's turn");

        GetCost(AlivedCharacter());
        

        battleManager.DiceRollButton.interactable = true;
        battleManager.ConfirmButton.interactable = false;

        if (battleManager.BattleTurn == 1) //첫턴일 경우 리스너 부착
        {
            battleManager.DiceRollButton.onClick.AddListener(Roll); //배틀 끝날때 해제해줘야함
            battleManager.ConfirmButton.onClick.AddListener(Attack);
        }
    }

    public void BattleUpdate()
    {

    }

    public void Exit()
    {
        
    }

    public void Roll() //battleManager.DiceRollButton에 부착
    {
        DiceManager.Instance.RollDice();

        battleManager.ConfirmButton.interactable = true;
    }

    public void Attack() //[주사위 눈금 *족보별계수 + { 공격력 - 방어력 * (1 - 방어력 관통률)}] *(버프 + 아티팩트 + 속성 + 패시브 + 각인)
    {        
        DiceManager.Instance.ground.SetActive(false);
        DiceManager.Instance.DiceBoard.SetActive(false);
        DiceManager.Instance.HideFakeDice();
        Debug.Log("공격!");
        float diceWeighting = DiceManager.Instance.DiceBattle.GetDiceWeighting(); //주사위 눈금 *족보별계수
        int signitureCount = DiceManager.Instance.DiceBattle.GetSignitureAmount();

        battleManager.DiceRollButton.interactable = false;
        battleManager.ConfirmButton.interactable = false;

        GetCost(signitureCount);        

        //공격 애니메이션실행

        battleManager.stateMachine.ChangeState(battleManager.enemyTurnState);
    }

    public void Skill()
    {

    }

    private void GetCost(int iNum)
    {
        int cost = battleManager.CurrnetCost;

        cost = Mathf.Clamp(cost + iNum, 0, battleManager.MaxCost);

        battleManager.CurrnetCost = cost;
        battleManager.costTest.text = cost.ToString();
    }

    private int AlivedCharacter() //작성 필요
    {
        int num = 0;

        return num;
    }
}
