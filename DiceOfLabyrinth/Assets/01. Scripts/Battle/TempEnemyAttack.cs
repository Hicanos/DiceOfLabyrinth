using System.Collections;
using UnityEngine;

public class TempEnemyAttack : MonoBehaviour
{
    public void EnemyAttack()
    {
        StartCoroutine(enemyAttack());
    }

    public void EnemyAttackEnd()
    {
        StopCoroutine(enemyAttack());
    }

    IEnumerator enemyAttack()
    {
        yield return new WaitForSeconds(1.2f);

        Debug.Log("Enemy Attack!");

        //IDamagable target = getTarget;
        //int damage = BattleManager.Instance.TestEnemy.EnemyData.Atk - 
        //battleManager.DealDamage(target , damage);

        yield return new WaitForSeconds(1.2f);

        BattleManager.Instance.stateMachine.ChangeState(BattleManager.Instance.playerTurnState);
    }

    private void getTarget()
    {

    }
}
