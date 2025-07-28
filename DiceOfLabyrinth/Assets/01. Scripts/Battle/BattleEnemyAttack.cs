using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleEnemyAttack : MonoBehaviour
{
    SOEnemySkill enemySkillData;
    const int characterCount = 5;
    [SerializeField] float waitSecondEnemyAttack;
    [SerializeField] float tempWaitAttackAnimEnd;

    Dictionary<TargetDeterminationMehtod, Func<int, int, List<int>>> targetGetterDictionary = new Dictionary<TargetDeterminationMehtod, Func<int, int, List<int>>>();

    public bool isEnemyAttacking = false;

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

    public void EnemyAttackDealDamage()
    {
        BattleManager battleManager = BattleManager.Instance;
        BattleCharacter battleCharacter;
        int characterIndex;

        int skillLength = battleManager.Enemy.currentSkill.Skills.Length;
        List<int> targetIndexTest = new List<int>();

        for (int i = 0; i < skillLength; i++)
        {
            EnemySkill skill = enemySkillData.Skills[i];
            int targetCount = skill.TragetCount;
            int provability = skill.FrontLineProbability;
            int skillValue = enemySkillData.SkillValue;

            targetIndexTest = targetGetterDictionary[skill.Method](targetCount, provability);

            battleManager.Enemy.currentTargetIndex = targetIndexTest;
            for (int j = 0; j < targetIndexTest.Count; j++)
            {
                characterIndex = targetIndexTest[j];
                battleCharacter = battleManager.BattleGroup.BattleCharacters[characterIndex];

                int damage = skillValue * battleManager.Enemy.CurrentAtk - battleCharacter.CurrentDEF;
                if (damage < 0) damage = 0;

                battleManager.BattleGroup.CharacterHit(characterIndex, damage);
                UIManager.Instance.BattleUI.BattleUILog.MakeBattleLog(battleManager.Enemy.Data.EnemyName, battleCharacter.CharNameKr, damage, false);
                if (battleCharacter.IsDied) battleManager.BattleGroup.CharacterDead(characterIndex);

                if (skill.Debuff == EnemyDebuff.None) continue;
                else
                {
                    if (GetRandomRange(1, 100) <= skill.DebuffChance)
                    {
                        Debug.Log($"캐릭터{characterIndex + 1} {skill.Debuff}걸림");
                    }
                }
            }
        }
    }

    IEnumerator enemyAttack()
    {
        yield return new WaitForSeconds(waitSecondEnemyAttack);

        isEnemyAttacking = true;
        enemySkillData = BattleManager.Instance.Enemy.currentSkill;
        EnemyAttackTest();

        yield return new WaitForSeconds(tempWaitAttackAnimEnd);

        //isEnemyAttacking = false;
        BattleManager.Instance.StateMachine.currentState = BattleManager.Instance.I_PlayerTurnState;
    }    

    public void EnemyAttackTest()
    {
        BattleManager battleManager = BattleManager.Instance;

        int skillLength = battleManager.Enemy.currentSkill.Skills.Length;
        List<int> targetIndexTest = new List<int>();

        for (int i = 0; i < skillLength; i++)
        {
            EnemySkill skill = enemySkillData.Skills[i];
            int targetCount = skill.TragetCount;
            int provability = skill.FrontLineProbability;            

            targetIndexTest = targetGetterDictionary[skill.Method](targetCount, provability);

            battleManager.Enemy.currentTargetIndex = targetIndexTest;            
        }
        List<int> targetList = battleManager.Enemy.currentTargetIndex;

        battleManager.Enemy.iEnemy.UseActiveSkill(battleManager.Enemy.currentSkill_Index, targetList[0]);
    }

    #region 타겟 결정 메서드들
    private List<int> GetTargetFrontBackProbability(int targetCount = 1, int front = 80)
    {
        int frontBack = (int)BattleManager.Instance.BattleGroup.CurrentFormationType + 1;
        List<int> frontIndex = BattleManager.Instance.BattleGroup.FrontLine.ToList();
        List<int> BackIndex = BattleManager.Instance.BattleGroup.BackLine.ToList();
        List<int> targetIndex = new List<int>();

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
        List<int> targetIndex = new List<int>();
        List<int> frontIndex = BattleManager.Instance.BattleGroup.FrontLine;
        List<int> BackIndex = BattleManager.Instance.BattleGroup.BackLine;

        for (int i = 0; i < frontIndex.Count; i++)
        {
            targetIndex.Add(frontIndex[i]);
        }
        for(int i = 0; i < BackIndex.Count; i++)
        {
            targetIndex.Add(BackIndex[i]);
        }

        return targetIndex;
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
    #endregion

    private int GetRandomRange(int min, int max) => UnityEngine.Random.Range(min, max + 1);
}
