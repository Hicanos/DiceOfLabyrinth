using System;
using System.Collections.Generic;

public enum ConditionTypeEnum
{
    None,
    CorrectDiceRank,
    SameDiceRankAsPreviousTurn,
    AttackInTurn,
    KillInTurn
}

public enum EffectTypeEnum
{
    AdditionalDamage,
    AdditionalRoll,
    AdditionalStone
}

public enum EffectLocationEnum
{
    CharacterAttack,
    TurnEnter,
    TurnEnd
}

public class EngravingBuffMaker
{        
    public void MakeEngravingBuff()
    {
        List<EngravingData> engravings = BattleManager.Instance.PartyData.Engravings;
        Action<IBuff> addBuffAction;
        DamageCondition condition;
        IBuff buff;

        for (int i = 0; i < engravings.Count; i++)
        {
            if (engravings[i] == null) return;

            for (int j = 0; j < engravings[i].DamageConditions.Count; j++)
            {
                condition = engravings[i].DamageConditions[j];

                buff = new EngravingBuff(GetConditionType(condition), condition, GetEffectAction(condition));

                addBuffAction = GetEffectLocation(condition);
                addBuffAction?.Invoke(buff);
            }
        }
    }

    public Func<DamageCondition, bool> GetConditionType(DamageCondition condition)
    {
        Func<DamageCondition,bool> conditionFunc;

        switch (condition.ConditionType)
        {
            case ConditionTypeEnum.CorrectDiceRank:
                return conditionFunc = new Func<DamageCondition, bool>(CorrectDiceRankCondition);
            case ConditionTypeEnum.SameDiceRankAsPreviousTurn:
                return conditionFunc = new Func<DamageCondition, bool>(SameDiceRankAsPreviousTurnCondition);
            case ConditionTypeEnum.AttackInTurn:
                return conditionFunc = new Func<DamageCondition, bool>(AttackInTurnCondition);
            case ConditionTypeEnum.KillInTurn:
                return conditionFunc = new Func<DamageCondition, bool>(KillInTurnCondition);
            default:
                return conditionFunc = new Func<DamageCondition, bool>(DefaultCondition);
        }
    }

    #region 조건 판별 메서드
    private bool CorrectDiceRankCondition(DamageCondition condition)
    {
        if (DiceManager.Instance.DiceRank == condition.ConditionRank) return true;
        else { return false; }
    }

    private bool SameDiceRankAsPreviousTurnCondition(DamageCondition condition)
    {
        BattleManager battleManager = BattleManager.Instance;
        DiceManager diceManager = DiceManager.Instance;

        if (battleManager.BattleTurn > 1 && diceManager.DiceRankBefore == diceManager.DiceRank)
        {
            return true;
        }
        return false;
    }

    private bool AttackInTurnCondition(DamageCondition condition)
    {
        if(BattleManager.Instance.BattleTurn == condition.ConditionValue) return true;
        else return false;
    }

    private bool KillInTurnCondition(DamageCondition condition)
    {
        if (BattleManager.Instance.IsWon && BattleManager.Instance.BattleTurn == condition.ConditionValue) return true;
        else return false;
    }

    private bool DefaultCondition(DamageCondition condition)
    {
        return true;
    }
    #endregion

    public Action<DamageCondition> GetEffectAction(DamageCondition condition)
    {
        switch (condition.EffectType)
        {
            case EffectTypeEnum.AdditionalDamage:
                return AdditionalDamageAction;
            case EffectTypeEnum.AdditionalRoll:
                return AdditionalRollAction;
            case EffectTypeEnum.AdditionalStone:
                return AdditionalStone;
            default:
                return null;
        }
    }

    private void AdditionalDamageAction(DamageCondition condition)
    {
        BattleManager.Instance.EngravingAdditionalStatus.AdditionalDamage += condition.EffectValue;
    }
    private void AdditionalRollAction(DamageCondition condition)
    {
        BattleManager.Instance.EngravingAdditionalStatus.AdditionalRoll += condition.EffectValue;
    }
    private void AdditionalStone(DamageCondition condition)
    {
        BattleManager.Instance.EngravingAdditionalStatus.AdditionalStone += condition.EffectValue;
    }

    private Action<IBuff> GetEffectLocation(DamageCondition condition)
    {
        switch (condition.EffectLocation)
        {
            case EffectLocationEnum.CharacterAttack:
                return BattleManager.Instance.EngravingBuffs.AddBuffsCallbackCharacterAttack;
            case EffectLocationEnum.TurnEnter:
                return BattleManager.Instance.EngravingBuffs.AddBuffsCallbackTurnEnter;
            case EffectLocationEnum.TurnEnd:
                return BattleManager.Instance.EngravingBuffs.AddBuffsCallbackTurnEnd;
            default:
                return null;
        }
    }
}