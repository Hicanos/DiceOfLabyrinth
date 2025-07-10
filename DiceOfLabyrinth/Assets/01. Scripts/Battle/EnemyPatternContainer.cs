using UnityEngine;
using System.Collections.Generic;

public class EnemyPatternContainer : MonoBehaviour
{
    [SerializeField] SOEnemySkill[] enemySkillDatas;

    public void PrepareSkill()
    {
        BattleManager battleManager = BattleManager.Instance;

        List<int> pattern = battleManager.Enemy.Data.ActiveSkills;
        int patternLength = battleManager.Enemy.Data.ActiveSkills.Count;
        int patternIndex = (battleManager.BattleTurn - 1) % patternLength;

        int skill_Index = pattern[patternIndex];

        SOEnemySkill skill = enemySkillDatas[skill_Index];
        battleManager.Enemy.currentSkill = skill;

        battleManager.UIValueChanger.ChangeUIText(BattleTextUIEnum.MonsterSkillName, $"{skill.SkillName} 준비중");
        battleManager.UIValueChanger.ChangeUIText(BattleTextUIEnum.MonsterSkillDescription, skill.SkillDescription);
    }    
}
