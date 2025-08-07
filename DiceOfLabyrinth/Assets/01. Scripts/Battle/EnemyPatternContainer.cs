using UnityEngine;
using System.Collections.Generic;

public class EnemyPatternContainer : MonoBehaviour
{
    [SerializeField] SOEnemySkill[] enemySkillDatas;
    [SerializeField] SOEnemyPassive[] enemyPassiveDatas;

    private string skillName;
    private string skillDescription;

    public void PrepareSkill()
    {
        BattleManager battleManager = BattleManager.Instance;

        List<int> pattern = battleManager.Enemy.Data.ActiveSkills;
        int patternLength = pattern.Count;
        int Skill_Index = (battleManager.BattleTurn - 1) % patternLength;
        
        int skill_Index = pattern[Skill_Index];

        SOEnemySkill skill = enemySkillDatas[skill_Index];
        battleManager.Enemy.currentSkill = skill;
        battleManager.Enemy.currentSkill_Index = skill_Index;

        skillName = skill.SkillName;
        skillDescription = skill.SkillDescription;        
    }

    public void GetPassive()
    {
        BattleManager battleManager = BattleManager.Instance;
        EnemyPassiveMaker enemyPassiveMaker = new EnemyPassiveMaker();

        List<int> Indexes = battleManager.Enemy.Data.PassiveSkills;

        for(int i = 0; i < Indexes.Count; i++)
        {
            enemyPassiveMaker.MakePassive(enemyPassiveDatas[Indexes[i]].Effects, enemyPassiveDatas[Indexes[i]].UseCount);
        }
    }

    public string GetSkillNameText()
    {
        return skillName;
    }

    public string GetSkillDescriptionText()
    {
        return skillDescription;
    }
}
