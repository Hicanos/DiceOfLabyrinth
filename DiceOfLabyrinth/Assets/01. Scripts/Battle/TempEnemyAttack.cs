using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TempEnemyAttack : MonoBehaviour
{
    SOEnemySkill enemySkillData;
    const int characterCount = 5;
    Dictionary<TargetDeterminationMehtod, Func<int, int, List<int>>> targetGetterDictionary = new Dictionary<TargetDeterminationMehtod, Func<int, int, List<int>>>();

    public void Start()
    {        
        targetGetterDictionary.Add(TargetDeterminationMehtod.FrontBackProbability, GetTargetFrontBackProbability);
        targetGetterDictionary.Add(TargetDeterminationMehtod.All, GetTargetAll);
        targetGetterDictionary.Add(TargetDeterminationMehtod.HpLow, GetTargetLowHp);
    }

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

        enemySkillData = BattleManager.Instance.Enemy.currentSkill;
        EnemyAttackTest();

        yield return new WaitForSeconds(1.2f);

        BattleManager.Instance.stateMachine.ChangeState(BattleManager.Instance.playerTurnState);
    }

    private List<int> GetTargetFrontBackProbability(int targetCount = 1, int front = 80)
    {
        int frontBack = (int)BattleManager.Instance.currentFormationType + 1;
        List<int> frontIndex = new List<int>();
        List<int> BackIndex = new List<int>();
        List<int> targetIndex = new List<int>();

        for(int i = 0; i < frontBack; i++)
        {
            frontIndex.Add(i);
        }
        for(int i = frontBack; i < characterCount; i++)
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

    private List<int> GetTargetAll(int targetCount, int value = 0)
    {
        return new List<int> { 0,1,2,3,4};
    }

    private List<int> GetTargetLowHp(int targetCount, int value = 0)
    {
        List<BattleCharacter> characters = BattleManager.Instance.battleCharacters;
        List<int> targetIndex = new List<int>();
        
        for (int i = 0; i < characterCount; i++)
        {
            if (characters[i].IsDied) continue;

            targetIndex.Add(i);
        }

        if(targetCount >= targetIndex.Count) return targetIndex;

        targetIndex.Sort();
        targetIndex.RemoveRange(targetCount, targetIndex.Count - targetCount);

        return targetIndex;
    }

    public void EnemyAttackTest()
    {
        int skillLength = BattleManager.Instance.Enemy.currentSkill.Skills.Length;
        List<int> targetIndexTest = new List<int>();

        for (int i = 0; i < skillLength; i++)
        {
            EnemySkill skill = enemySkillData.Skills[i];
            int targetCount = skill.TragetCount;
            int provability = skill.FrontLineProbability;
            int skillValue = enemySkillData.SkillValue;

            targetIndexTest = targetGetterDictionary[skill.Method](targetCount, provability);

            for (int j = 0; j < targetIndexTest.Count; j++)
            {                
                int damage = skillValue * BattleManager.Instance.Enemy.CurrentAtk - BattleManager.Instance.battleCharacters[targetIndexTest[i]].CurrentDEF;
                if (damage < 0) damage = 0;                
                BattleManager.Instance.battleCharacters[targetIndexTest[j]].TakeDamage(damage);
                BattleManager.Instance.UIValueChanger.ChangeCharacterHpRatio((HPEnumCharacter)targetIndexTest[j]);
                Debug.Log($"캐릭터{targetIndexTest[j]+1}에게 {damage}데미지");

                if(skill.Debuff == EnemyDebuff.None) continue;
                else
                {
                    if(UnityEngine.Random.Range(1, 101) <= 20)
                    {
                        Debug.Log($"캐릭터{targetIndexTest[j] + 1} {skill.Debuff}걸림");
                    }
                }
            }
        }
    }
}
