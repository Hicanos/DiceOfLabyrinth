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
        yield return new WaitForSeconds(0.8f);

        Debug.Log("Enemy Attack!");

        yield return new WaitForSeconds(0.8f);

        BattleManager.Instance.stateMachine.ChangeState(BattleManager.Instance.playerTurnState);
    }
}
