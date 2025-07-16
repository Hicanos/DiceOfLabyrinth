using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BattleEnemyAttack : MonoBehaviour
{
    SOEnemySkill enemySkillData;
    const int characterCount = 5;
    [SerializeField] float waitSecondEnemyAttack;

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
        yield return new WaitForSeconds(waitSecondEnemyAttack);

        enemySkillData = BattleManager.Instance.Enemy.currentSkill;
        EnemyAttackTest();

        yield return new WaitForSeconds(waitSecondEnemyAttack);

        BattleManager.Instance.stateMachine.ChangeState(BattleManager.Instance.playerTurnState);
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

            BattleManager.Instance.Enemy.currentTargetIndex = targetIndexTest;
            for (int j = 0; j < targetIndexTest.Count; j++)
            {
                int damage = skillValue * BattleManager.Instance.Enemy.CurrentAtk - BattleManager.Instance.BattleGroup.BattleCharacters[targetIndexTest[i]].CurrentDEF;
                if (damage < 0) damage = 0;
                BattleManager.Instance.BattleGroup.BattleCharacters[targetIndexTest[j]].TakeDamage(damage);
                BattleManager.Instance.UIValueChanger.ChangeCharacterHpRatio((HPEnumCharacter)targetIndexTest[j]);
                Debug.Log($"skillValue({skillValue})*Atk({BattleManager.Instance.Enemy.CurrentAtk})-Def({BattleManager.Instance.BattleGroup.BattleCharacters[targetIndexTest[i]].CurrentDEF})");
                Debug.Log($"캐릭터{targetIndexTest[j] + 1}에게 {damage}데미지");

                if (skill.Debuff == EnemyDebuff.None) continue;
                else
                {
                    if (GetRandomRange(1,100) <= skill.DebuffChance)
                    {
                        Debug.Log($"캐릭터{targetIndexTest[j] + 1} {skill.Debuff}걸림");
                    }
                }
            }
        }
        List<int> targetList = BattleManager.Instance.Enemy.currentTargetIndex;

        BattleManager.Instance.Enemy.iEnemy.UseActiveSkill(BattleManager.Instance.Enemy.currentSkill_Index, targetList[0]);
    }

    private List<int> GetTargetFrontBackProbability(int targetCount = 1, int front = 80)
    {
        int frontBack = (int)BattleManager.Instance.BattleGroup.CurrentFormationType + 1;
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
            int randNum = GetRandomRange(1, 100);

            if(randNum <= front)
            {
                int index = GetRandomRange(0, frontIndex.Count - 1);
                
                targetIndex.Add(frontIndex[index]);
                frontIndex.Remove(frontIndex[index]);
            }
            else
            {
                int index = GetRandomRange(0, BackIndex.Count - 1);

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
        List<BattleCharacter> characters = BattleManager.Instance.BattleGroup.BattleCharacters;
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
    private int GetRandomRange(int min, int max) => UnityEngine.Random.Range(min, max + 1);
}
