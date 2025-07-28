using System;

public class ArtifactBuffUpdate : IBuff
{
    ArtifactDetailData Data;
    public Func<ArtifactDetailData, bool> JudgeCondition;
    public Action<ArtifactDetailData> EffectAction;

    public int BuffDuration;
    public int BuffUseCount;
    public bool isActiveThisTurn;

    public ArtifactBuffUpdate(ArtifactDetailData data, Func<ArtifactDetailData, bool> judgeCondition, Action<ArtifactDetailData> effectAction)
    {
        Data = data;
        JudgeCondition = judgeCondition;
        EffectAction = effectAction;
        BuffUseCount = 1;
    }

    public void Action()
    {
        if (JudgeCondition(Data) == false) return;
        if (isActiveThisTurn) return;
        if (BuffUseCount == 0) return;

        isActiveThisTurn = true;

        EffectAction(Data);
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