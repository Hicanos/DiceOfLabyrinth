using UnityEngine;
using System;
using System.Collections.Generic;

public enum ArtifactEffectTypeEnum
{
    AdditionalDamage,
    AdditionalElementDamage,
    AdditionalRoll,
    AdditionalMaxCost,
    AdditionalStone,
    AdditionalAttack,
    AdditionalSIgniture,
    AdditionalStatusWithSignitureCount,
    HealHPRatio,
    GetCost,
    EnemyDebuff,
    RemoveDebuff,
    GetBarrier,
    CharacterRevive,
    AdditionalAttackCount
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

public enum ArtifactCallBackLocation
{
    TurnEnter,
    CharacterAttack,
    CharacterHit,
    CharacterDie,
    SpendCost,
}

public class ArtifactBuffMaker
{
    public void MakeArtifactBuff()
    {
        List<ArtifactData> artifacts = BattleManager.Instance.PartyData.Artifacts;
        ArtifactDetailData detailData;
        Action<IBuff> AddBuffAction;

        for (int i = 0; i < artifacts.Count; i++)
        {
            if (artifacts[i] == null) return;

            for (int j = 0; j < artifacts[i].ArtifactEffects.Count; j++)
            {
                detailData = new ArtifactDetailData(artifacts[i].ArtifactEffects[j]);                                

                IBuff buff = new ArtifactBuffUpdate(detailData, GetArtifactCondition(detailData), GetEffectAction(detailData));

                AddBuffAction = GetCallBackLocationAction(detailData);
                AddBuffAction?.Invoke(buff);
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
            case ArtifactEffectTypeEnum.AdditionalDamage:
                return new Action<ArtifactDetailData>(AdditionalDamageAction);
            case ArtifactEffectTypeEnum.AdditionalElementDamage:
                return new Action<ArtifactDetailData>(AdditionalElementDamageAction);
            case ArtifactEffectTypeEnum.AdditionalRoll:
                return new Action<ArtifactDetailData>(AdditionalRollAction);            
            case ArtifactEffectTypeEnum.AdditionalMaxCost:
                return new Action<ArtifactDetailData>(AdditionalMaxCostAction);
            case ArtifactEffectTypeEnum.AdditionalStone:
                return new Action<ArtifactDetailData>(AdditionalStoneAction);
            case ArtifactEffectTypeEnum.AdditionalAttack:
                return new Action<ArtifactDetailData>(AdditionalAttackAction);
            case ArtifactEffectTypeEnum.AdditionalSIgniture:
                return new Action<ArtifactDetailData>(AdditionalSignitureAction);
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
            case ArtifactEffectTypeEnum.AdditionalAttackCount:
                return new Action<ArtifactDetailData>(AdditionalAttackCountAction);
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
        BattleManager.Instance.ArtifactAdditionalStatus.AdditionalDamage += data.EffectValue;
    }
    private void AdditionalElementDamageAction(ArtifactDetailData data)
    {
        BattleManager.Instance.ArtifactAdditionalStatus.AdditionalElementDamage += data.EffectValue;
    }
    private void AdditionalRollAction(ArtifactDetailData data)
    {
        BattleManager.Instance.ArtifactAdditionalStatus.AdditionalRoll += data.EffectValue;
    }
    private void AdditionalMaxCostAction(ArtifactDetailData data)
    {
        BattleManager.Instance.ArtifactAdditionalStatus.AdditionalMaxCost += data.EffectValue;
    }
    private void AdditionalStoneAction(ArtifactDetailData data)
    {
        BattleManager.Instance.ArtifactAdditionalStatus.AdditionalStone += data.EffectValue;
    }
    private void AdditionalAttackAction(ArtifactDetailData data)
    {
        BattleManager.Instance.ArtifactAdditionalStatus.AdditionalAttack += data.EffectValue;
    }
    private void AdditionalSignitureAction(ArtifactDetailData data)
    {
        BattleManager.Instance.ArtifactAdditionalStatus.AdditionalSIgniture += data.EffectValue;
    }    
    private void AdditionalStatusWithSignitureCountAction(ArtifactDetailData data)
    {
        BattleManager.Instance.ArtifactAdditionalStatus.AdditionalSIgniture += data.EffectValue * DiceManager.Instance.SignitureAmount;
    }
    private void HealHPRatioAction(ArtifactDetailData data)
    {
        BattleCharacterInBattle[] characters = BattleManager.Instance.PartyData.Characters;
        for (int i = 0; i < characters.Length; i++)
        {
            if(characters[i].IsDead) continue;

            float healAmount = characters[i].MaxHP * data.EffectValue;
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
        BattleManager.Instance.PartyData.CharacterGetBarrier(data.EffectValue);
    }
    private void CharacterReviveAction(ArtifactDetailData data)
    {
        BattlePartyData partyData = BattleManager.Instance.PartyData;

        partyData.Characters[partyData.CurrentDeadIndex].Revive();
        Debug.Log("부활 아티펙트 활성");
    }
    private void AdditionalAttackCountAction(ArtifactDetailData data)
    {
        BattleManager.Instance.PartyData.Characters[0].GetAttackCount((int)data.EffectValue);
    }
    #endregion

    public Action<IBuff> GetCallBackLocationAction(ArtifactDetailData data)
    {
        switch(data.CallBackLocation)
        {
            case ArtifactCallBackLocation.TurnEnter:
                return BattleManager.Instance.ArtifactBuffs.AddbuffsCallbackTurnEnter;
            case ArtifactCallBackLocation.CharacterAttack:
                return BattleManager.Instance.ArtifactBuffs.AddbuffsCallbackCharacterAttack;
            case ArtifactCallBackLocation.CharacterHit:
                return BattleManager.Instance.ArtifactBuffs.AddbuffsCallbackCharacterHit;
            case ArtifactCallBackLocation.CharacterDie:
                return BattleManager.Instance.ArtifactBuffs.AddbuffsCallbackCharacterDie;
            case ArtifactCallBackLocation.SpendCost:
                return BattleManager.Instance.ArtifactBuffs.AddbuffsCallbackSpendCost;
            default:
                return null;
        }
    }
}

public class ArtifactDetailData
{
    public ArtifactConditionTypeEnum ConditionType;
    public float ConditionValue;

    public ArtifactEffectTypeEnum EffectType;
    public float EffectValue;

    public ArtifactCallBackLocation CallBackLocation;

    private void Init(ArtifactConditionTypeEnum conType, float conValue, ArtifactEffectTypeEnum effType, float effValue, ArtifactCallBackLocation point = 0)
    {
        ConditionType = conType; ConditionValue = conValue;
        EffectType = effType; EffectValue = effValue;
            CallBackLocation = point;
    }

    public ArtifactDetailData(ArtifactEffectData data)
    {
        switch (data.Type)
        {
            case ArtifactEffectData.EffectType.AdditionalElementDamage:
                Init(ArtifactConditionTypeEnum.PlayerTurnStartNum, 1, ArtifactEffectTypeEnum.AdditionalElementDamage, data.Value, ArtifactCallBackLocation.TurnEnter);
                break;
            case ArtifactEffectData.EffectType.AdditionalDamage:
                Init(ArtifactConditionTypeEnum.PlayerTurnStartNum, 1, ArtifactEffectTypeEnum.AdditionalDamage, data.Value, ArtifactCallBackLocation.TurnEnter);
                break;
            case ArtifactEffectData.EffectType.AdditionalDiceRoll:
                Init(ArtifactConditionTypeEnum.PlayerTurnStartNum, 1, ArtifactEffectTypeEnum.AdditionalRoll, data.Value, ArtifactCallBackLocation.TurnEnter);
                break;
            case ArtifactEffectData.EffectType.AdditionalDamageToBoss:
                Init(ArtifactConditionTypeEnum.IsBoss, 0, ArtifactEffectTypeEnum.AdditionalDamage, data.Value, ArtifactCallBackLocation.TurnEnter);
                break;
            case ArtifactEffectData.EffectType.AdditionalMaxCost:
                Init(ArtifactConditionTypeEnum.PlayerTurnStartNum, 1, ArtifactEffectTypeEnum.AdditionalMaxCost, data.Value, ArtifactCallBackLocation.TurnEnter);
                break;
            case ArtifactEffectData.EffectType.AdditionalManaStone:
                Init(ArtifactConditionTypeEnum.PlayerTurnStartNum, 1, ArtifactEffectTypeEnum.AdditionalStone, data.Value, ArtifactCallBackLocation.TurnEnter);
                break;
            case ArtifactEffectData.EffectType.AdditionalDamageIfHaveSignitureDice:
                Init(ArtifactConditionTypeEnum.DiceSignitureCount, 0, ArtifactEffectTypeEnum.AdditionalStatusWithSignitureCount, data.Value, ArtifactCallBackLocation.CharacterAttack);
                break;
            case ArtifactEffectData.EffectType.AdditionalAttackCount:
                Init(ArtifactConditionTypeEnum.Chace, 50, ArtifactEffectTypeEnum.AdditionalAttackCount, data.Value, ArtifactCallBackLocation.CharacterAttack);
                break;
            case ArtifactEffectData.EffectType.HealingWhenStartBattle:
                Init(ArtifactConditionTypeEnum.PlayerTurnStartNum, 1, ArtifactEffectTypeEnum.HealHPRatio, data.Value, ArtifactCallBackLocation.TurnEnter);
                break;
            case ArtifactEffectData.EffectType.DebuffToEnemyAtFirstTurn:
                Init(ArtifactConditionTypeEnum.PlayerTurnStartNum, 1, ArtifactEffectTypeEnum.EnemyDebuff, data.Value, ArtifactCallBackLocation.TurnEnter);
                break;
            case ArtifactEffectData.EffectType.RemoveDebuffPerTurn:
                Init(ArtifactConditionTypeEnum.PlayerTurnEndNum, 1, ArtifactEffectTypeEnum.RemoveDebuff, data.Value, ArtifactCallBackLocation.TurnEnter);
                break;            
            case ArtifactEffectData.EffectType.CostRegenerationEveryTurn:
                Init(ArtifactConditionTypeEnum.PlayerTurnStartNum, 1, ArtifactEffectTypeEnum.GetCost, data.Value, ArtifactCallBackLocation.TurnEnter);
                break;            
            

            case ArtifactEffectData.EffectType.CostRegenerationWhenUse10Cost:
                Init(ArtifactConditionTypeEnum.CostSpendAmount, 10, ArtifactEffectTypeEnum.GetCost, data.Value, ArtifactCallBackLocation.SpendCost);
                break;
            case ArtifactEffectData.EffectType.ReviveWhenDie:
                Init(ArtifactConditionTypeEnum.Chace, 60, ArtifactEffectTypeEnum.CharacterRevive, data.Value, ArtifactCallBackLocation.CharacterDie);
                break;
            case ArtifactEffectData.EffectType.GenerateBarrier:
                Init(ArtifactConditionTypeEnum.None, 1, ArtifactEffectTypeEnum.GetBarrier, data.Value, ArtifactCallBackLocation.CharacterHit);
                break;
        }
    }
}