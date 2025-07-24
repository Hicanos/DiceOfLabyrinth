using System;

public interface IBuff
{
    public void Action();
    public void CallBack();
    public void ReduceDuration();
}

public class EngravingBuff : IBuff
{
    public Func<DamageCondition, bool> JudgeCondition;
    public DamageCondition Condition;
    public DetailedTurnState EffectTime;
    public EffectTypeEnum EffectType;
    public float EffectValue;
    public int BuffDuration;

    public EngravingBuff(Func<DamageCondition, bool> jungeCondition, DamageCondition condition, DetailedTurnState effectTime)
    {
        JudgeCondition = jungeCondition;
        EffectTime = effectTime;
        Condition = condition;
        EffectType = condition.EffectType;
        EffectValue = condition.EffectValue;
        BuffDuration = condition.BuffDuration;
    }

    public void Action()
    {
        if (BattleManager.Instance.CurrentDetailedState != EffectTime) return;

        if (JudgeCondition != null && JudgeCondition(Condition))
        {
            BattleManager.Instance.EngravingAdditionalStatus.AdditionalStatus[(int)EffectType] += EffectValue;
        }
    }

    public void CallBack()
    {

    }

    public void ReduceDuration()
    {
        BuffDuration--;

        if (BuffDuration == 0)
        {
            BattleManager.Instance.EngravingAdditionalStatus.AdditionalStatus[(int)EffectType] -= EffectValue;
        }
    }
}
