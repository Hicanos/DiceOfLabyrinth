using UnityEngine;
using System;

public enum TargetDeterminationMehtod
{
    All,
    Random,
    FrontBackProbability,
    FrontBackCount,
    HpLow
}

public enum EnemyBuff
{
    None,
    Heal
}

public enum EnemyDebuff
{
    None,
    Restriction,
    Burn,
    Stun,
    Bleeding
}

[CreateAssetMenu(fileName = "EnemySkills", menuName = "EnemySkill/Skill")]
public class SOEnemySkill : ScriptableObject
{
    public string   Index;
    public int      SkillIndex;
    public string   SkillName;
    public string   SkillDescription;
    public int      SkillValue;
    public int      AttackTimes;

    public EnemySkill[] Skills;
}

[Serializable]
public struct EnemySkill
{
    public TargetDeterminationMehtod Method;

    public int TragetCount;
    public int FrontLineProbability;
    public int FrontLineCount;

    public EnemyBuff    Buff;
    public int          BuffChance;
    public EnemyDebuff  Debuff;
    public int          DebuffChance;
}
