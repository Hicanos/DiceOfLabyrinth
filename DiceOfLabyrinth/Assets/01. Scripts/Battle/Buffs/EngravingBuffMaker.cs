using System;
using System.Collections.Generic;
using System.Diagnostics;

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

public class EngravingBuffMaker
{        
    public void MakeEngravingBuff()
    {
        List<EngravingData> engravings = BattleManager.Instance.BattleGroup.Engravings;
        DamageCondition condition;
        IBuff buff;
        for (int i = 0; i < engravings.Count; i++)
        {
            if (engravings[i] == null) continue;

            for (int j = 0; j < engravings[i].DamageConditions.Count; j++)
            {
                condition = engravings[i].DamageConditions[j];
                UnityEngine.Debug.Log("각인 할당");
                buff = new EngravingBuff(GetConditionType(condition), condition, GetEffectType(condition));
                BattleManager.Instance.EngravingBuffs.AddEngravingBuffs(buff);
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

    public DetailedTurnState GetEffectType(DamageCondition condition)
    {
        switch (condition.EffectType)
        {
            case EffectTypeEnum.AdditionalDamage:
                return DetailedTurnState.Attack;
            case EffectTypeEnum.AdditionalStone:
                return DetailedTurnState.BattleEnd;
            default:
                return DetailedTurnState.Enter;
        }
    }    
}