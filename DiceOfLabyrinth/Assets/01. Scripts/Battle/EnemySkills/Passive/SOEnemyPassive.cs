using UnityEngine;
using System;

public enum EnemyPassiveEffectEnum
{
    AdditionalAtk,
    AdditionalDef,
    AttackTargetBack,
    AttackTargetHighestAtk,
    RestoreHP,
    GetBarrier,
    LifeSteal
}

public enum EnemyPassiveConditionEnum
{
    None,
    HPRatio,
    UseSkillIndex
}

public enum EnemyPassiveEffectLocationEnum
{
    EnemyHit,
    EnemyAttack,
    BattleStart
}

[CreateAssetMenu(fileName = "EnemySkills", menuName = "EnemySkill/Passive")]
public class SOEnemyPassive : ScriptableObject
{
    public int Index;
    public string Name;
    public string Description;
    
    public EnemyPassiveEffectLocationEnum EffectLocation;
    public EnemyPassiveEffectData[] Effects;
}

[Serializable]
public class EnemyPassiveEffectData
{
    public EnemyPassiveEffectEnum EffectType;
    public float EffectValue;

    public EnemyPassiveConditionEnum ConditionType;
    public float ConditionValue;

    public int UseCount;

    public string Description;
}
