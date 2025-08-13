using System;

public class ArtifactBuffUpdate : IBuff
{
    ArtifactDetailData Data;
    public Func<float, bool> JudgeCondition;
    public Action<float> EffectAction;

    public int BuffDuration;
    public int BuffUseCount;
    public bool isActiveThisTurn;

    public ArtifactBuffUpdate(ArtifactDetailData data, Func<float, bool> judgeCondition, Action<float> effectAction)
    {
        Data = data;
        JudgeCondition = judgeCondition;
        EffectAction = effectAction;
        BuffUseCount = 1;
    }

    public void Action()
    {
        if (JudgeCondition(Data.ConditionValue) == false) return;
        if (isActiveThisTurn) return;
        if (BuffUseCount == 0) return;

        isActiveThisTurn = true;

        EffectAction(Data.EffectValue);
        ReduceUseCount();
    }

    public void ReduceDuration()
    {
        if (isActiveThisTurn == false) return;

        BuffDuration--;
    }

    private void ReduceUseCount()
    {
        BuffUseCount--;
    }
}