using UnityEngine;
using System;

public interface IBuff
{
    public void Action();
    public void ReduceDuration();
}

public class EngravingBuff : IBuff
{
    public Func<DamageCondition, bool> JudgeCondition;
    public DamageCondition Condition;
    public DetailedTurnState EffectTime;
    public EffectTypeEnum EffectType;
    public float EffectValue;
    public int MaxBuffDuration;
    private int buffDuration;

    public bool isActive;

    public EngravingBuff(Func<DamageCondition, bool> jungeCondition, DamageCondition condition, DetailedTurnState effectTime)
    {
        JudgeCondition = jungeCondition;
        EffectTime = effectTime;
        Condition = condition;
        EffectType = condition.EffectType;
        EffectValue = condition.EffectValue;
        MaxBuffDuration = condition.BuffDuration;
        buffDuration = MaxBuffDuration;
    }

    public void Action()
    {
        if (BattleManager.Instance.CurrentDetailedState != EffectTime) return;

        if (JudgeCondition != null && JudgeCondition(Condition))
        {
            isActive = true;
            buffDuration = MaxBuffDuration;
            BattleManager.Instance.EngravingAdditionalStatus.AdditionalStatus[(int)EffectType] += EffectValue;
        }
    }

    public void ReduceDuration()
    {
        buffDuration--;

        if (buffDuration == 0 && isActive)
        {
            BattleManager.Instance.EngravingAdditionalStatus.AdditionalStatus[(int)EffectType] -= EffectValue;
            isActive = false;
        }
    }
}
