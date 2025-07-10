using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TempEnemyAttack : MonoBehaviour
{
    SOEnemySkill enemySkillData;
    List<int> targetIndexTest = new List<int>();

    Dictionary<TargetDeterminationMehtod, Func<int, int, List<int>>> targetGetterDictionary = new Dictionary<TargetDeterminationMehtod, Func<int, int, List<int>>>();

    Func<int, int, List<int>> getTargetFunc;
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

        List<int> targetIndex = getTargetFrontBackProbability();

        for(int i = 0;  i < targetIndex.Count; i++)
        {
            int damage = BattleManager.Instance.Enemy.CurrentAtk - BattleManager.Instance.battleCharacters[targetIndex[i]].CurrentDEF;
            BattleManager.Instance.battleCharacters[targetIndex[i]].TakeDamage(damage);
        }

        yield return new WaitForSeconds(1.2f);

        BattleManager.Instance.stateMachine.ChangeState(BattleManager.Instance.playerTurnState);
    }

    private List<int> getTargetFrontBackProbability(int targetCount = 1, int front = 80)
    {
        int frontBack = (int)BattleManager.Instance.currentFormationType + 1;
        List<int> frontIndex = new List<int>();
        List<int> BackIndex = new List<int>();
        List<int> targetIndex = new List<int>();

        for(int i = 0; i < frontBack; i++)
        {
            frontIndex.Add(i);
        }
        for(int i = frontBack; i < 5; i++)
        {
            BackIndex.Add(i);
        }


        for(int i = 0; i < targetCount; i++)
        {
            int randNum = UnityEngine.Random.Range(1, 101);

            if(randNum <= front)
            {
                int index = UnityEngine.Random.Range(0, frontIndex.Count);

                targetIndex.Add(frontIndex[index]);
                frontIndex.Remove(frontIndex[index]);
            }
            else
            {
                int index = UnityEngine.Random.Range(0, BackIndex.Count);

                targetIndex.Add(BackIndex[index]);
                BackIndex.Remove(BackIndex[index]);
            }
        }

        return targetIndex;
    }

    private List<int> getTargetAll(int targetCount, int front)
    {
        return new List<int> { 0,1,2,3,4};
    }

    public void test()
    {
        for (int i = 0; i < enemySkillData.Skills.Length; i++)
        {
            EnemySkill skill = enemySkillData.Skills[i];
            int targetCount = skill.TragetCount;
            int provability = skill.FrontLineProbability;

            targetIndexTest = targetGetterDictionary[skill.Method](targetCount, provability);
        }
    }

    public void EnemyAttackTest()
    {
        //BattleManager.Instance.Enemy.currentSkill.Skills
        for (int i = 0; i < targetIndexTest.Count; i++)
        {
            int damage = BattleManager.Instance.Enemy.CurrentAtk - BattleManager.Instance.battleCharacters[targetIndexTest[i]].CurrentDEF;
            BattleManager.Instance.battleCharacters[targetIndexTest[i]].TakeDamage(damage);
        }
    }
}
