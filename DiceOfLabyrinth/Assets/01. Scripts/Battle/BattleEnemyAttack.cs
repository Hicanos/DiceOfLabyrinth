using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

    IEnumerator enemyAttack()
    {
        yield return new WaitForSeconds(waitSecondEnemyAttack);

        isEnemyAttacking = true;
        enemySkillData = BattleManager.Instance.Enemy.currentSkill;
        SelectTarget();

        yield return new WaitForSeconds(tempWaitAttackAnimEnd);

        BattleManager.Instance.EndEnemyTurn();
    }

    public void SelectTarget()
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
        int frontBack = (int)BattleManager.Instance.PartyData.CurrentFormationType + 1;
        List<int> frontIndex = BattleManager.Instance.PartyData.FrontLine.ToList();
        List<int> BackIndex = BattleManager.Instance.PartyData.BackLine.ToList();
        List<int> targetIndex = new List<int>();

        for(int i = 0; i < targetCount; i++)
        {
            int randNum = GetRandomRange(1, 100);

            if (frontIndex.Count != 0 && randNum <= front)
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
        List<int> frontIndex = BattleManager.Instance.PartyData.FrontLine;
        List<int> BackIndex = BattleManager.Instance.PartyData.BackLine;

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
        BattleCharacterInBattle[] characters = BattleManager.Instance.PartyData.Characters;
        List<int> targetIndex = new List<int>();
        
        for (int i = 0; i < characterCount; i++)
        {
            if (characters[i].IsDead) continue;

            targetIndex.Add(i);
        }

        if(targetCount >= targetIndex.Count) return targetIndex;

        targetIndex.Sort();
        targetIndex.RemoveRange(targetCount, targetIndex.Count - targetCount);

        return targetIndex;
    }
    #endregion    

    public void EnemyAttackDealDamage()
    {
        BattleManager battleManager = BattleManager.Instance;
        BattleCharacterInBattle character;
        int characterIndex;

        int skillLength = battleManager.Enemy.currentSkill.Skills.Length;
        List<int> targetIndexTest = new List<int>();

        for (int i = 0; i < skillLength; i++)
        {
            EnemySkill skill = enemySkillData.Skills[i];
            int targetCount = skill.TragetCount;
            int provability = skill.FrontLineProbability;
            int skillValue = enemySkillData.SkillValue;

            targetIndexTest = battleManager.Enemy.currentTargetIndex;

            //battleManager.Enemy.currentTargetIndex = targetIndexTest;
            for (int j = 0; j < targetIndexTest.Count; j++)
            {
                characterIndex = targetIndexTest[j];
                character = battleManager.PartyData.Characters[characterIndex];

                Debug.Log($"에너미 어택 : {skillValue} * {battleManager.Enemy.CurrentAtk} - {character.CurrentDEF}");
                int damage = skillValue * battleManager.Enemy.CurrentAtk - character.CurrentDEF;
                if (damage < 0) damage = 0;

                UIManager.Instance.BattleUI.BattleUILog.WriteBattleLog(battleManager.Enemy.Data.EnemyName, character.CharNameKr, damage, false);
                character.TakeDamage(damage);

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

    private int GetRandomRange(int min, int max) => UnityEngine.Random.Range(min, max + 1);
}
