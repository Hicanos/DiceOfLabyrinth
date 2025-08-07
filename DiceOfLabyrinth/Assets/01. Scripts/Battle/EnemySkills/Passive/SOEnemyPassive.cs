using UnityEngine;
using System;

public enum EnemyPassiveEffectEnum
{
    AdditionalAtk,
    AdditionalDef,
    AttackTargetBack,
    AttackTargetHighestAtk,
    RestoreHP,
    StrongWill,
    GetBarrier,
    LifeSteal
}

public enum EnemyPassiveConditionEnum
{
    HPRatio,
    StartBattle,
    UseSkillIndex
}

[CreateAssetMenu(fileName = "EnemySkills", menuName = "EnemySkill/Passive")]
public class SOEnemyPassive : ScriptableObject
{
    public int Index;
    public string Name;
    public string Description;
    public int UseCount;
    public EnemyPassiveEffect[] Effects;
}

[Serializable]
public class EnemyPassiveEffect
{
    public EnemyPassiveEffectEnum EffectType;
    public int EffectValue;

    public EnemyPassiveConditionEnum ConditionType;
    public int ConditionValue;    
}
