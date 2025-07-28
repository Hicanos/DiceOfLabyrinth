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
    public EffectTypeEnum EffectType;
    public float EffectValue;
    public int MaxBuffDuration;
    private int buffDuration;
    private Action<DamageCondition> effectAction;
    public bool isActive;

    public EngravingBuff(Func<DamageCondition, bool> jungeCondition, DamageCondition condition, Action<DamageCondition> effectAction)
    {
        JudgeCondition = jungeCondition;
        Condition = condition;
        EffectType = condition.EffectType;
        EffectValue = condition.EffectValue;
        MaxBuffDuration = condition.BuffDuration;
        buffDuration = MaxBuffDuration;
        this.effectAction = effectAction;
    }

    public void Action()
    {
        if (JudgeCondition != null && JudgeCondition(Condition))
        {
            isActive = true;
            buffDuration = MaxBuffDuration;
            effectAction?.Invoke(Condition);
        }
    }

    public void ReduceDuration()
    {
        buffDuration--;

        if (buffDuration == 0 && isActive)
        {            
            switch (EffectType)
            {
                case EffectTypeEnum.AdditionalDamage:
                    BattleManager.Instance.EngravingAdditionalStatus.AdditionalDamage -= EffectValue;
                    break;
                case EffectTypeEnum.AdditionalRoll:
                    BattleManager.Instance.EngravingAdditionalStatus.AdditionalRoll -= EffectValue;
                    break;
                case EffectTypeEnum.AdditionalStone:
                    BattleManager.Instance.EngravingAdditionalStatus.AdditionalStone -= EffectValue;
                    break;
            }
            isActive = false;
        }
    }
}
