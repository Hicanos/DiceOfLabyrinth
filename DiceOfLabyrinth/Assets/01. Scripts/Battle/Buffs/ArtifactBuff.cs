using System;

public class ArtifactBuff : IBuff
{
    public ArtifactDetailData Data;
    public Func<ArtifactDetailData, bool> JudgeCondition;
    public Action<ArtifactDetailData> EffectAction;

    public DetailedTurnState EffectTime;

    public int BuffDuration;

    public ArtifactBuff(ArtifactDetailData data, Func<ArtifactDetailData, bool> judgeCondition, Action<ArtifactDetailData> effectAction)
    {
        Data = data;
        JudgeCondition = judgeCondition;
        EffectAction = effectAction;

        BuffDuration = 0;
    }

    public void Action()
    {
        if (BattleManager.Instance.CurrentDetailedState != EffectTime) return;
        if (JudgeCondition(Data) == false) return;

        EffectAction(Data);
    }

    public void ReduceDuration()
    {
        //BuffDuration--;

        //if (BuffDuration == 0)
        //{
            
        //}
    }
}

public class ArtifactBuffUpdate : IBuff
{
    ArtifactDetailData Data;
    public Func<ArtifactDetailData, bool> JudgeCondition;
    public Action<ArtifactDetailData> EffectAction;

    public DetailedTurnState EffectTime;

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
        BuffDuration--;
    }

    private void ReduceUseCount()
    {
        BuffUseCount--;
    }
}