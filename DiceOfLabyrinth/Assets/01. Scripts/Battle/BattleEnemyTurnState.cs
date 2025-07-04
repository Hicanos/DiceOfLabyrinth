using UnityEngine;

public class BattleEnemyTurnState : IBattleTurnState
{
    TempEnemyAttack enemyAtack;
    public void Enter()
    {        
        Debug.Log("Enemy's turn");
        enemyAtack = GameObject.FindAnyObjectByType<TempEnemyAttack>().GetComponent<TempEnemyAttack>();
        enemyAtack.EnemyAttack();
    }

    public void BattleUpdate()
    {

    }

    public void Exit()
    {
        enemyAtack.EnemyAttackEnd();        
        DiceManager.Instance.ResetSetting();
    }
}
