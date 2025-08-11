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
        EnemyPassiveMaker enemyPassiveMaker = new EnemyPassiveMaker();

        enemyPassiveMaker.MakeSkill();
    }

    public SOEnemyPassive GetPassiveSO(int index)
    {
        return enemyPassiveDatas[index];
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
