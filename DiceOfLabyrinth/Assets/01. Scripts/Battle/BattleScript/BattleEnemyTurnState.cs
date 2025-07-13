using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BattleEnemyTurnState : IBattleTurnState
{
    public void Enter()
    {
        Debug.Log("Enemy's turn");

        Attack();        
    }

    public void BattleUpdate()
    {

    }

    public void Exit()
    {
        BattleManager.Instance.EnemyAttack.EnemyAttackEnd();
        DiceManager.Instance.ResetSetting();
    }

    private void Attack()
    {
        BattleManager battleManager = BattleManager.Instance;
        List<int> targetList = battleManager.Enemy.currentTargetIndex;

        if (targetList.Count == 1)
        {
            battleManager.iEnemy.UseActiveSkill(battleManager.Enemy.currentSkill_Index, targetList[0]);
        }
        else
        {
            battleManager.iEnemy.UseActiveSkill(battleManager.Enemy.currentSkill_Index, targetList[0]);
        }

        battleManager.EnemyAttack.EnemyAttack();
    }
}
