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

public class EngravingBuffMaker : ISkillMaker, ISkillLocationMaker
{        
    public void MakeSkill()
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

                buff = new EngravingBuff(GetCondition((int)condition.ConditionType), condition, GetEffect((int)condition.EffectType));

                addBuffAction = GetEffectLocation((int)condition.EffectLocation);
                addBuffAction?.Invoke(buff);
            }
        }
    }

    public Func<float, bool> GetCondition(int enumIndex)
    {
        Func<float,bool> conditionFunc;

        switch (enumIndex)
        {
            case (int)ConditionTypeEnum.CorrectDiceRank:
                return conditionFunc = new Func<float, bool>(CorrectDiceRankCondition);
            case (int)ConditionTypeEnum.SameDiceRankAsPreviousTurn:
                return conditionFunc = new Func<float, bool>(SameDiceRankAsPreviousTurnCondition);
            case (int)ConditionTypeEnum.AttackInTurn:
                return conditionFunc = new Func<float, bool>(AttackInTurnCondition);
            case (int)ConditionTypeEnum.KillInTurn:
                return conditionFunc = new Func<float, bool>(KillInTurnCondition);
            default:
                return conditionFunc = new Func<float, bool>(DefaultCondition);
        }
    }

    #region 조건 판별 메서드
    private bool CorrectDiceRankCondition(float value)
    {
        if ((int)DiceManager.Instance.DiceRank == (int)value) return true;
        else { return false; }
    }

    private bool SameDiceRankAsPreviousTurnCondition(float value)
    {
        BattleManager battleManager = BattleManager.Instance;
        DiceManager diceManager = DiceManager.Instance;

        if (battleManager.BattleTurn > 1 && diceManager.DiceRankBefore == diceManager.DiceRank)
        {
            return true;
        }
        return false;
    }

    private bool AttackInTurnCondition(float value)
    {
        if(BattleManager.Instance.BattleTurn == value) return true;
        else return false;
    }

    private bool KillInTurnCondition(float value)
    {
        if (BattleManager.Instance.IsWon && BattleManager.Instance.BattleTurn == value) return true;
        else return false;
    }

    private bool DefaultCondition(float value)
    {
        return true;
    }
    #endregion

    public Action<float> GetEffect(int enumIndex)
    {
        switch ((EffectTypeEnum)enumIndex)
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

    private void AdditionalDamageAction(float value)
    {
        BattleManager.Instance.EngravingAdditionalStatus.AdditionalDamage += value;
    }
    private void AdditionalRollAction(float value)
    {
        BattleManager.Instance.EngravingAdditionalStatus.AdditionalRoll += value;
    }
    private void AdditionalStone(float value)
    {
        BattleManager.Instance.EngravingAdditionalStatus.AdditionalStone += value;
    }

    public Action<IBuff> GetEffectLocation(int enumIndex)
    {
        switch ((EffectLocationEnum)enumIndex)
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