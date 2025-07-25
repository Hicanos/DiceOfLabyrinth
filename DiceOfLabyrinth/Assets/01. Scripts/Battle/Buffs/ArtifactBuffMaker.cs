using UnityEngine;
using System;
using System.Collections.Generic;

public enum AdditionalStatusEnum
{
    AdditionalDamage,
    AdditionalElementDamage,
    AdditionalRoll,
    AdditionalSIgniture,
    AdditionalMaxCost,
    AdditionalStone,
    AdditionalAttack
}

public enum ArtifactEffectTypeEnum
{
    AdditionalStatus,
    AdditionalStatusWithSignitureCount,
    HealHPRatio,
    GetCost,
    EnemyDebuff,
    RemoveDebuff,
    GetBarrier,
    CharacterRevive
}

public enum ArtifactConditionTypeEnum
{
    None,
    PlayerTurnStartNum,
    PlayerTurnEndNum,
    DiceSignitureCount,
    Chace,
    IsBoss,
    CostSpendAmount
}

public enum ArtifactCallBackPoint
{
    None,
    CharacterHit,
    CharacterDie,
    SpendCost,
}

public class ArtifactBuffMaker
{
    public void MakeArtifactBuff()
    {
        List<ArtifactData> artifacts = BattleManager.Instance.BattleGroup.Artifacts;
        ArtifactDetailData detailData;

        for (int i = 0; i < artifacts.Count; i++)
        {
            if (artifacts[i] == null) return;

            for (int j = 0; j < artifacts[i].ArtifactEffects.Count; j++)
            {
                detailData = new ArtifactDetailData(artifacts[i].ArtifactEffects[j]);                                

                if(detailData.IsCallback)
                {
                    IBuff buff = new ArtifactBuffUpdate(detailData.ActionLocation, detailData, GetArtifactCondition(detailData), GetEffectAction(detailData));
                    switch (detailData.CallBackPoint)
                    {
                        case ArtifactCallBackPoint.SpendCost:
                            BattleManager.Instance.ArtifactBuffs.AddbuffsCallbackSpendCost(buff);
                            break;
                        case ArtifactCallBackPoint.CharacterHit:
                            BattleManager.Instance.ArtifactBuffs.AddbuffsCallbackCharacterHit(buff);
                            break;
                        case ArtifactCallBackPoint.CharacterDie:
                            BattleManager.Instance.ArtifactBuffs.AddbuffsCallbackCharacterDie(buff);
                            break;
                    }
                }
                else
                {
                    IBuff buff = new ArtifactBuff(detailData.ActionLocation, detailData, GetArtifactCondition(detailData), GetEffectAction(detailData));
                    BattleManager.Instance.ArtifactBuffs.AddArtifactBuff(buff);
                }
            }
        }
    }

    private Func<ArtifactDetailData, bool> GetArtifactCondition(ArtifactDetailData data)
    {
        switch (data.ConditionType)
        {
            case ArtifactConditionTypeEnum.PlayerTurnStartNum:
                return new Func<ArtifactDetailData, bool>(PlayerTurnStartNumCondition);
            case ArtifactConditionTypeEnum.PlayerTurnEndNum:
                return new Func<ArtifactDetailData, bool>(PlayerTurnEndNumCondition);
            case ArtifactConditionTypeEnum.DiceSignitureCount:
                return new Func<ArtifactDetailData, bool>(DiceSignitureCountCondition);
            case ArtifactConditionTypeEnum.Chace:
                return new Func<ArtifactDetailData, bool>(ChanceCondition);
            case ArtifactConditionTypeEnum.IsBoss:
                return new Func<ArtifactDetailData, bool>(IsBossCondition);
            case ArtifactConditionTypeEnum.CostSpendAmount:
                return new Func<ArtifactDetailData, bool>(CostSpendAmountCondition);
            default:
                return new Func<ArtifactDetailData, bool>(DefaultCondition);
        }
    }

    #region 조건 판별 메서드
    private bool DefaultCondition(ArtifactDetailData data)
    {
        return true;
    }

    private bool PlayerTurnStartNumCondition(ArtifactDetailData data)
    {
        if (BattleManager.Instance.CurrentDetailedState == DetailedTurnState.Enter && BattleManager.Instance.BattleTurn == data.ConditionValue) return true;
        else return false;
    }

    private bool PlayerTurnEndNumCondition(ArtifactDetailData data)
    {
        if (BattleManager.Instance.CurrentDetailedState == DetailedTurnState.EndTurn) return true;
        else return false;
    }

    private bool DiceSignitureCountCondition(ArtifactDetailData data)
    {
        if (DiceManager.Instance.SignitureAmount > 0) return true;
        else return false;
    }

    private bool ChanceCondition(ArtifactDetailData data)
    {
        int iNum = UnityEngine.Random.Range(1, 101);
        if (iNum <= data.ConditionValue) return true;
        else return false;
    }

    private bool IsBossCondition(ArtifactDetailData data)
    {
        if (BattleManager.Instance.IsBoss) return true;
        else return false;
    }
    private bool CostSpendAmountCondition(ArtifactDetailData data)
    {
        if (BattleManager.Instance.CostSpendedInTurn >= data.ConditionValue) return true;
        else return false;
    }
    #endregion

    private Action<ArtifactDetailData> GetEffectAction(ArtifactDetailData data)
    {
        switch (data.EffectType)
        {
            case ArtifactEffectTypeEnum.AdditionalStatus:
                return new Action<ArtifactDetailData>(AdditionalDamageAction);
            case ArtifactEffectTypeEnum.AdditionalStatusWithSignitureCount:
                return new Action<ArtifactDetailData>(AdditionalStatusWithSignitureCountAction);
            case ArtifactEffectTypeEnum.HealHPRatio:
                return new Action<ArtifactDetailData>(HealHPRatioAction);
            case ArtifactEffectTypeEnum.GetCost:
                return new Action<ArtifactDetailData>(GetCostAction);
            case ArtifactEffectTypeEnum.EnemyDebuff:
                return new Action<ArtifactDetailData>(EnemyDebuffAction);
            case ArtifactEffectTypeEnum.RemoveDebuff:
                return new Action<ArtifactDetailData>(RemoveDebuffAction);
            case ArtifactEffectTypeEnum.GetBarrier:
                return new Action<ArtifactDetailData>(GetBarrierAction);
            case ArtifactEffectTypeEnum.CharacterRevive:
                return new Action<ArtifactDetailData>(CharacterReviveAction);
            default:
                return new Action<ArtifactDetailData>(DefaultAction);
        }
    }

    #region 액션 메서드
    private void DefaultAction(ArtifactDetailData data)
    {
        Debug.Log("Can't find ArtifactEffectType");
    }
    private void AdditionalDamageAction(ArtifactDetailData data)
    {
        BattleManager.Instance.ArtifactAdditionalStatus.AdditionalStatus[(int)data.additionalStatus] += data.EffectValue;
    }
    private void AdditionalStatusWithSignitureCountAction(ArtifactDetailData data)
    {
        BattleManager.Instance.ArtifactAdditionalStatus.AdditionalStatus[(int)data.additionalStatus] += data.EffectValue * DiceManager.Instance.SignitureAmount;
    }
    private void HealHPRatioAction(ArtifactDetailData data)
    {
        List<BattleCharacter> characters = BattleManager.Instance.BattleGroup.BattleCharacters;
        for (int i = 0; i < characters.Count; i++)
        {
            if(characters[i].IsDied) continue;

            float healAmount = characters[i].RegularHP * data.EffectValue;
            characters[i].Heal((int)healAmount);
        }        
    }
    private void GetCostAction(ArtifactDetailData data)
    {
        BattleManager.Instance.GetCost((int)data.EffectValue);
    }
    private void EnemyDebuffAction(ArtifactDetailData data)
    {
        Debug.Log("디버프 아티펙트 활성");
    }
    private void RemoveDebuffAction(ArtifactDetailData data)
    {
        Debug.Log("디버프 제거 아티펙트 활성");
    }
    private void GetBarrierAction(ArtifactDetailData data)
    {
        Debug.Log("방어막 아티펙트 활성");
    }
    private void CharacterReviveAction(ArtifactDetailData data)
    {
        Debug.Log("부활 아티펙트 활성");
    }
    #endregion
}

public class ArtifactDetailData
{
    public ArtifactConditionTypeEnum ConditionType;
    public float ConditionValue;

    public ArtifactEffectTypeEnum EffectType;
    public float EffectValue;
    public AdditionalStatusEnum additionalStatus;

    public DetailedTurnState ActionLocation;

    public bool IsCallback;
    public ArtifactCallBackPoint CallBackPoint;

    private void Init(DetailedTurnState state, ArtifactConditionTypeEnum conType, float conValue, ArtifactEffectTypeEnum effType, float effValue, AdditionalStatusEnum addStatus = (AdditionalStatusEnum)0, bool isCallback = false, ArtifactCallBackPoint point = 0)
    {
        ActionLocation = state;  ConditionType = conType; ConditionValue = conValue;
        EffectType = effType; EffectValue = effValue; additionalStatus = addStatus;
        IsCallback = isCallback; CallBackPoint = point;
    }

    public ArtifactDetailData(ArtifactEffectData data)
    {
        switch (data.Type)
        {
            case ArtifactEffectData.EffectType.AdditionalElementDamage:
                Init(DetailedTurnState.Enter, ArtifactConditionTypeEnum.PlayerTurnStartNum, 1, ArtifactEffectTypeEnum.AdditionalStatus, data.Value, AdditionalStatusEnum.AdditionalElementDamage);
                break;
            case ArtifactEffectData.EffectType.AdditionalDamage:
                Init(DetailedTurnState.Enter, ArtifactConditionTypeEnum.PlayerTurnStartNum, 1, ArtifactEffectTypeEnum.AdditionalStatus, data.Value, AdditionalStatusEnum.AdditionalDamage);
                break;
            case ArtifactEffectData.EffectType.AdditionalDiceRoll:
                Init(DetailedTurnState.Enter, ArtifactConditionTypeEnum.PlayerTurnStartNum, 1, ArtifactEffectTypeEnum.AdditionalStatus, data.Value);
                break;
            case ArtifactEffectData.EffectType.AdditionalDamageToBoss:
                Init(DetailedTurnState.Enter, ArtifactConditionTypeEnum.IsBoss, 0, ArtifactEffectTypeEnum.AdditionalStatus, data.Value, AdditionalStatusEnum.AdditionalDamage);
                break;
            case ArtifactEffectData.EffectType.AdditionalMaxCost:
                Init(DetailedTurnState.Enter, ArtifactConditionTypeEnum.PlayerTurnStartNum, 1, ArtifactEffectTypeEnum.AdditionalStatus, data.Value, AdditionalStatusEnum.AdditionalMaxCost);
                break;
            case ArtifactEffectData.EffectType.AdditionalManaStone:
                Init(DetailedTurnState.Enter, ArtifactConditionTypeEnum.PlayerTurnStartNum, 1, ArtifactEffectTypeEnum.AdditionalStatus, data.Value, AdditionalStatusEnum.AdditionalStone);
                break;
            case ArtifactEffectData.EffectType.AdditionalDamageIfHaveSignitureDice:
                Init(DetailedTurnState.Attack, ArtifactConditionTypeEnum.DiceSignitureCount, 0, ArtifactEffectTypeEnum.AdditionalStatusWithSignitureCount, data.Value, AdditionalStatusEnum.AdditionalDamage);
                break;
            case ArtifactEffectData.EffectType.AdditionalAttackCount:

                break;
            case ArtifactEffectData.EffectType.HealingWhenStartBattle:
                Init(DetailedTurnState.Enter, ArtifactConditionTypeEnum.PlayerTurnStartNum, 1, ArtifactEffectTypeEnum.HealHPRatio, data.Value);
                break;
            case ArtifactEffectData.EffectType.DebuffToEnemyAtFirstTurn:
                Init(DetailedTurnState.Enter, ArtifactConditionTypeEnum.PlayerTurnStartNum, 1, ArtifactEffectTypeEnum.EnemyDebuff, data.Value);
                break;
            case ArtifactEffectData.EffectType.RemoveDebuffPerTurn:
                Init(DetailedTurnState.EndTurn, ArtifactConditionTypeEnum.PlayerTurnEndNum, 1, ArtifactEffectTypeEnum.RemoveDebuff, data.Value);
                break;            
            case ArtifactEffectData.EffectType.CostRegenerationEveryTurn:
                Init(DetailedTurnState.Enter, ArtifactConditionTypeEnum.PlayerTurnStartNum, 1, ArtifactEffectTypeEnum.GetCost, data.Value);
                break;            
            

            case ArtifactEffectData.EffectType.CostRegenerationWhenUse10Cost:
                Init(0,ArtifactConditionTypeEnum.CostSpendAmount, 10, ArtifactEffectTypeEnum.GetCost, data.Value, 0, true, ArtifactCallBackPoint.SpendCost);
                break;
            case ArtifactEffectData.EffectType.ReviveWhenDie:
                Init(0,ArtifactConditionTypeEnum.Chace, 60, ArtifactEffectTypeEnum.CharacterRevive, data.Value, 0, true, ArtifactCallBackPoint.CharacterDie);
                break;
            case ArtifactEffectData.EffectType.GenerateBarrier:
                Init(0,ArtifactConditionTypeEnum.None, 1, ArtifactEffectTypeEnum.GetBarrier, data.Value,0,true, ArtifactCallBackPoint.CharacterHit);
                break;
        }
    }
}