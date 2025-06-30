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
        //battleManager.DealDamage(IDamagerable target , int damage);

        yield return new WaitForSeconds(1.2f);

        BattleManager.Instance.stateMachine.ChangeState(BattleManager.Instance.playerTurnState);
    }
}
