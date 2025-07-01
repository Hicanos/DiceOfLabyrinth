using UnityEngine;

public class BattlePlayerTurnState : IBattleTurnState
{
    BattleManager battleManager = BattleManager.Instance;

    public void Enter()
    {
        battleManager.BattleTurn++;
        Debug.Log($"Turn{BattleManager.Instance.BattleTurn}");
        Debug.Log("Player's turn");

        battleManager.GetCost(AlivedCharacter());

        if (battleManager.BattleTurn == 1) 
        {
            battleManager.GetButton();
            battleManager.LoadMonsterPattern.Load();
        }

        battleManager.LoadMonsterPattern.PrepareSkill();

        battleManager.currentPlayerState = PlayerTurnState.Enter;
        battleManager.OnOffButton();
    }

    public void BattleUpdate()
    {

    }

    public void Exit()
    {
        
    }

    public void Roll() //battleManager.DiceRollButton에 부착
    {

    }

    public void Confirm() //공격력 - 방어력 * (버프 + 아티팩트 + 속성 + 패시브) * 족보별 계수
    {        

    }

    public void EndTurn()
    {
        
    }

    public void Skill()
    {

    }

    private void CharacterAttack(float diceWeighting)
    {
        Debug.Log("공격!");
        for (int i = 0; i < battleManager.entryCharacters.Length; i++)
        {
            float atk = battleManager.entryCharacters[i].baseATK + battleManager.entryCharacters[i].plusATK;

            //battleManager.DealDamage(IDamagerable target , int damage);
        }
    }    

    private int AlivedCharacter() //작성 필요
    {
        int num = 0;

        return num;
    }
}
