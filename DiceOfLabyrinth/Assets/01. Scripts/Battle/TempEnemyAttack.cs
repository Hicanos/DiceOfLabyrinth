using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

        List<int> targetIndex = getTarget();

        for(int i = 0;  i < targetIndex.Count; i++)
        {
            int damage = BattleManager.Instance.Enemy.CurrentAtk - BattleManager.Instance.battleCharacters[targetIndex[i]].CurrentDEF;
            BattleManager.Instance.battleCharacters[targetIndex[i]].TakeDamage(damage);
        }

        yield return new WaitForSeconds(1.2f);

        BattleManager.Instance.stateMachine.ChangeState(BattleManager.Instance.playerTurnState);
    }

    private List<int> getTarget(int targetCount = 1, int front = 80)
    {
        int frontBack = (int)BattleManager.Instance.currentFormationType + 1;
        List<int> targetIndex = new List<int>();

        for(int i = 0; i < targetCount; i++)
        {
            int randNum = Random.Range(1, 101);

            if(randNum <= front)
            {
                targetIndex.Add(Random.Range(0, frontBack));
            }
            else
            {
                targetIndex.Add(Random.Range(frontBack, 6));
            }
        }

        return targetIndex;
    }
}
