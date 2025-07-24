using UnityEngine;
using System.Collections.Generic;

public enum ArtifactEffectTypeEnum
{
    AdditionalDamage,
    AdditionalElementDamage,
    AdditionalAttack,
    HealHPRatio,
    AdditionalRoll,
    AdditionalSIgniture,
    AdditionalMaxCost,
    GetCost,
    AdditionalStone,
    EnemyDebuff,
    RemoveDebuff,
    GetBarrier,
}

public enum ArtifactConditionTypeEnum
{
    None,
    BattleStart,
    PlayerTurnEnd,
    DiceSignitureCount,
    Chace,
    IsBoss
}

public enum ArtifactEffectTimeEnum
{
    EffectAtStart,
    EffectPerTurn,
    EffectByCondition
}

public enum ArtifactEffectDurationEnum
{
    Eternal,
    TurnBase
}

public enum ArtifactCallBackPoint
{
    None,
    CharacterHit,
    CharacterDie,
    SpendCost,

}

public class ArtifactBuffMaker : MonoBehaviour
{
    ArtifactBuffContainer artifactBuffs = new ArtifactBuffContainer();
    
    //public void MakeArtifactBuff()
    //{
    //    List<ArtifactData> artifacts = BattleManager.Instance.BattleGroup.Artifacts;

    //    for(int i = 0; i < artifacts.Count; i++)
    //    {
    //        if (artifacts[i] != null) return;

    //        for(int j = 0; j < artifacts[i].ArtifactEffects.Count; j++)
    //        {
    //            MakeTempArtifactData(artifacts[i].ArtifactEffects[j]);
    //            //GetArtifactCondition();
    //        }
    //    }
    //}

    //private Func<TempArtifactData, bool> GetArtifactCondition(TempArtifactData data)
    //{
    //    Func<TempArtifactData, bool> func;

    //    switch (data.ConditionType)
    //    {
    //        case ArtifactConditionTypeEnum.BattleStart:
    //            return new Func<TempArtifactData, bool>(BattleStartCondition);
                
    //    }

    //    return func;
    //}

    private bool BattleStartCondition(TempArtifactData data)
    {
        if (BattleManager.Instance.BattleTurn == 1) return true;
        else return false;
    }

    //private TempArtifactData MakeTempArtifactData(ArtifactEffectData data)
    //{
    //    TempArtifactData artifactData;

    //    switch (data.Type)
    //    {
    //        case ArtifactEffectData.EffectType.AdditionalElementDamage:
    //            artifactData = new TempArtifactData(ArtifactConditionTypeEnum.None, 0, ArtifactEffectDurationEnum.Eternal, 0, ArtifactEffectTypeEnum.AdditionalElementDamage, data.Value, ArtifactEffectTimeEnum.EffectAtStart, false);
    //            break;
    //        case ArtifactEffectData.EffectType.AdditionalDamage:

    //            break;
    //        case ArtifactEffectData.EffectType.AdditionalDiceRoll:

    //            break;
    //        case ArtifactEffectData.EffectType.AdditionalAttackCount:

    //            break;
    //        case ArtifactEffectData.EffectType.AdditionalDamageToBoss:

    //            break;
    //        case ArtifactEffectData.EffectType.AdditionalDamageIfHaveSignitureDice:

    //            break;
    //        case ArtifactEffectData.EffectType.HealingWhenStartBattle:

    //            break;
    //        case ArtifactEffectData.EffectType.DebuffToEnemyAtFirstTurn:

    //            break;
    //        case ArtifactEffectData.EffectType.RemoveDebuffPerTurn:

    //            break;
    //        case ArtifactEffectData.EffectType.CostRegenerationWhenUse10Cost:

    //            break;
    //        case ArtifactEffectData.EffectType.CostRegenerationEveryTurn:

    //            break;
    //        case ArtifactEffectData.EffectType.ReviveWhenDie:

    //            break;
    //        case ArtifactEffectData.EffectType.AdditionalMaxCost:

    //            break;
    //        case ArtifactEffectData.EffectType.AdditionalManaStone:

    //            break;
    //        case ArtifactEffectData.EffectType.CriticalChance:

    //            break;
    //        case ArtifactEffectData.EffectType.CriticalDamage:

    //            break;
    //        case ArtifactEffectData.EffectType.GenerateBarrier:

    //            break;
    //    }

    //    return artifactData;
    //}
}

public class TempArtifactData
{   
    private ArtifactConditionTypeEnum conditionType;
    private float conditionValue;

    private ArtifactEffectDurationEnum durationType;
    private int buffDuration;

    private ArtifactEffectTypeEnum effectType;
    private float effectValue;

    private bool isCallback;
    private ArtifactEffectTimeEnum effectTime;

    public ArtifactConditionTypeEnum ConditionType => conditionType;
    public float ConditionValue => conditionValue;
    public ArtifactEffectTypeEnum EffectType => effectType;
    public float EffectValue => effectValue;
    public ArtifactEffectTimeEnum EffectTime => effectTime;
    public ArtifactEffectDurationEnum DurationType => durationType;
    public int BuffDuration => buffDuration;
    public bool IsCallback => isCallback;
    public TempArtifactData(ArtifactConditionTypeEnum conditionType, float conditionValue, ArtifactEffectDurationEnum durationType, int buffDuration, ArtifactEffectTypeEnum effectType, float effectValue, ArtifactEffectTimeEnum effectTime, bool isCallback)
    {
        this.conditionType = conditionType;
        this.conditionValue = conditionValue;
        this.effectType = effectType;
        this.effectValue = effectValue;
        this.effectTime = effectTime;
        this.durationType = durationType;
        this.buffDuration = buffDuration;
        this.isCallback = isCallback;
    }
}
