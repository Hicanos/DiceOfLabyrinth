using System;

public class ArtifactBuff : IBuff
{
    public Func<DamageCondition, bool> JudgeCondition;
    public ArtifactEffectData Condition;
    public ArtifactEffectTypeEnum EffectType;

    public DetailedTurnState EffectTime;
    public float EffectValue;
    public int BuffDuration;

    public ArtifactBuff(Func<DamageCondition, bool> jungeCondition, ArtifactEffectData effectData, DetailedTurnState effectTime)
    {
        JudgeCondition = jungeCondition;
        EffectTime = effectTime;
        Condition = effectData;

        EffectValue = effectData.Value;

        //EffectType = ;
        BuffDuration = 0;
    }

    public void Action()
    {
        if (BattleManager.Instance.CurrentDetailedState != EffectTime) return;

        //if (JudgeCondition != null && JudgeCondition(Condition))
        //{
        //    BattleManager.Instance.EngravingAdditionalStatus.AdditionalStatus[(int)EffectType] += EffectValue;
        //}
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
