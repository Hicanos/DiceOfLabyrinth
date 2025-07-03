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

        //int damage = BattleManager.Instance.enemy.ATK;
        //IDamagerable target = getTarget;
        //battleManager.DealDamage(target , damage);

        yield return new WaitForSeconds(1.2f);

        BattleManager.Instance.stateMachine.ChangeState(BattleManager.Instance.playerTurnState);
    }
}
