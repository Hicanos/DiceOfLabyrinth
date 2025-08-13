using System;
using System.Collections.Generic;
using UnityEngine;

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

public class ArtifactBuffMaker : ISkillMaker, ISkillLocationMaker
{
    public void MakeSkill()
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

                IBuff buff = new ArtifactBuffUpdate(detailData, GetCondition((int)detailData.ConditionType), GetEffect((int)detailData.EffectType));

                AddBuffAction = GetEffectLocation((int)detailData.CallBackLocation);
                AddBuffAction?.Invoke(buff);
            }
        }
    }

    public Func<float, bool> GetCondition(int enumIndex)
    {
        switch (enumIndex)
        {
            case (int)ArtifactConditionTypeEnum.PlayerTurnStartNum:
                return new Func<float, bool>(PlayerTurnStartNumCondition);
            case (int)ArtifactConditionTypeEnum.PlayerTurnEndNum:
                return new Func<float, bool>(PlayerTurnEndNumCondition);
            case (int)ArtifactConditionTypeEnum.DiceSignitureCount:
                return new Func<float, bool>(DiceSignitureCountCondition);
            case (int)ArtifactConditionTypeEnum.Chace:
                return new Func<float, bool>(ChanceCondition);
            case (int)ArtifactConditionTypeEnum.IsBoss:
                return new Func<float, bool>(IsBossCondition);
            case (int)ArtifactConditionTypeEnum.CostSpendAmount:
                return new Func<float, bool>(CostSpendAmountCondition);
            default:
                return new Func<float, bool>(DefaultCondition);
        }
    }

    #region 조건 판별 메서드
    private bool DefaultCondition(float value)
    {
        return true;
    }

    private bool PlayerTurnStartNumCondition(float value)
    {
        if (BattleManager.Instance.CurrentDetailedState == DetailedTurnState.Enter && BattleManager.Instance.BattleTurn == value) return true;
        else return false;
    }

    private bool PlayerTurnEndNumCondition(float value)
    {
        if (BattleManager.Instance.CurrentDetailedState == DetailedTurnState.EndTurn) return true;
        else return false;
    }

    private bool DiceSignitureCountCondition(float value)
    {
        if (DiceManager.Instance.SignitureAmount > 0) return true;
        else return false;
    }

    private bool ChanceCondition(float value)
    {
        int iNum = UnityEngine.Random.Range(1, 101);
        if (iNum <= value) return true;
        else return false;
    }

    private bool IsBossCondition(float value)
    {
        if (BattleManager.Instance.IsBoss) return true;
        else return false;
    }
    private bool CostSpendAmountCondition(float value)
    {
        if (BattleManager.Instance.CostSpendedInTurn >= value) return true;
        else return false;
    }
    #endregion

    public Action<float> GetEffect(int enumIndex)
    {
        switch ((ArtifactEffectTypeEnum)enumIndex)
        {
            case ArtifactEffectTypeEnum.AdditionalDamage:
                return new Action<float>(AdditionalDamageAction);
            case ArtifactEffectTypeEnum.AdditionalElementDamage:
                return new Action<float>(AdditionalElementDamageAction);
            case ArtifactEffectTypeEnum.AdditionalRoll:
                return new Action<float>(AdditionalRollAction);            
            case ArtifactEffectTypeEnum.AdditionalMaxCost:
                return new Action<float>(AdditionalMaxCostAction);
            case ArtifactEffectTypeEnum.AdditionalStone:
                return new Action<float>(AdditionalStoneAction);
            case ArtifactEffectTypeEnum.AdditionalAttack:
                return new Action<float>(AdditionalAttackAction);
            case ArtifactEffectTypeEnum.AdditionalSIgniture:
                return new Action<float>(AdditionalSignitureAction);
            case ArtifactEffectTypeEnum.AdditionalStatusWithSignitureCount:
                return new Action<float>(AdditionalStatusWithSignitureCountAction);
            case ArtifactEffectTypeEnum.HealHPRatio:
                return new Action<float>(HealHPRatioAction);
            case ArtifactEffectTypeEnum.GetCost:
                return new Action<float>(GetCostAction);
            case ArtifactEffectTypeEnum.EnemyDebuff:
                return new Action<float>(EnemyDebuffAction);
            case ArtifactEffectTypeEnum.RemoveDebuff:
                return new Action<float>(RemoveDebuffAction);
            case ArtifactEffectTypeEnum.GetBarrier:
                return new Action<float>(GetBarrierAction);
            case ArtifactEffectTypeEnum.CharacterRevive:
                return new Action<float>(CharacterReviveAction);
            case ArtifactEffectTypeEnum.AdditionalAttackCount:
                return new Action<float>(AdditionalAttackCountAction);
            default:
                return new Action<float>(DefaultAction);
        }
    }

    #region 액션 메서드
    private void DefaultAction(float value)
    {
        Debug.Log("Can't find ArtifactEffectType");
    }
    private void AdditionalDamageAction(float value)
    {
        BattleManager.Instance.ArtifactAdditionalStatus.AdditionalDamage += value;
    }
    private void AdditionalElementDamageAction(float value)
    {
        BattleManager.Instance.ArtifactAdditionalStatus.AdditionalElementDamage += value;
    }
    private void AdditionalRollAction(float value)
    {
        BattleManager.Instance.ArtifactAdditionalStatus.AdditionalRoll += value;
    }
    private void AdditionalMaxCostAction(float value)
    {
        BattleManager.Instance.ArtifactAdditionalStatus.AdditionalMaxCost += value;
    }
    private void AdditionalStoneAction(float value)
    {
        BattleManager.Instance.ArtifactAdditionalStatus.AdditionalStone += value;
    }
    private void AdditionalAttackAction(float value)
    {
        BattleManager.Instance.ArtifactAdditionalStatus.AdditionalAttack += value;
    }
    private void AdditionalSignitureAction(float value)
    {
        BattleManager.Instance.ArtifactAdditionalStatus.AdditionalSIgniture += value;
    }    
    private void AdditionalStatusWithSignitureCountAction(float value)
    {
        BattleManager.Instance.ArtifactAdditionalStatus.AdditionalSIgniture += value * DiceManager.Instance.SignitureAmount;
    }
    private void HealHPRatioAction(float value)
    {
        BattleCharacterInBattle[] characters = BattleManager.Instance.PartyData.Characters;
        for (int i = 0; i < characters.Length; i++)
        {
            if(characters[i].IsDead) continue;

            float healAmount = characters[i].MaxHP * value;
            characters[i].Heal((int)healAmount);
        }        
    }
    private void GetCostAction(float value)
    {
        BattleManager.Instance.GetCost((int)value);
    }
    private void EnemyDebuffAction(float value)
    {
        Debug.Log("디버프 아티펙트 활성");
    }
    private void RemoveDebuffAction(float value)
    {
        Debug.Log("디버프 제거 아티펙트 활성");
    }
    private void GetBarrierAction(float value)
    {
        BattleManager.Instance.PartyData.CharacterGetBarrier(value);
    }
    private void CharacterReviveAction(float value)
    {
        BattlePartyData partyData = BattleManager.Instance.PartyData;
        BattleCharacterInBattle character = partyData.Characters[partyData.CurrentDeadIndex];
        Debug.Log("부활 아티펙트 활성");

        character.Revive();
        float amount = character.MaxHP * value;
        character.Heal((int)amount);
    }
    private void AdditionalAttackCountAction(float value)
    {
        BattleManager.Instance.PartyData.Characters[0].GetAttackCount((int)value);
    }
    #endregion

    public Action<IBuff> GetEffectLocation(int enumIndex)
    {
        switch((ArtifactCallBackLocation)enumIndex)
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
                Init(ArtifactConditionTypeEnum.None, 1, ArtifactEffectTypeEnum.CharacterRevive, data.Value, ArtifactCallBackLocation.CharacterDie);
                break;
            case ArtifactEffectData.EffectType.GenerateBarrier:
                Init(ArtifactConditionTypeEnum.None, 1, ArtifactEffectTypeEnum.GetBarrier, data.Value, ArtifactCallBackLocation.CharacterHit);
                break;
        }
    }
}