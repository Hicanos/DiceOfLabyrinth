using System.Collections.Generic;
using UnityEngine;

public class EngravingEffect
{    
    EngravingData data;

    List<DamageCondition> damageConditions;
    DamageConditionRank[] damageConditionsRank;
    BonusAbilityRoll[] bonusAbilityRoll;

    public EngravingEffect(EngravingData data)
    {
        if (data == null) return;
        this.data = data;
        damageConditions = data.DamageConditions;
        damageConditionsRank = data.DamageConditionRanks;
        bonusAbilityRoll = data.BonusAbilityRolls;
    }

    public void GetEngravingEffectInAttack()
    {
        BattleManager battleManager = BattleManager.Instance;
        DiceManager diceManager = DiceManager.Instance;

        if (damageConditions != null)
        {
            for (int i = 0; i < damageConditions.Count; i++)
            {
                switch (damageConditions[i].Type)
                {
                    case DamageCondition.ConditionType.SameAsPreviousTurn:
                        if (battleManager.BattleTurn > 1 && diceManager.DiceRankBefore == diceManager.DiceRank)
                        {
                            battleManager.EngravingAdditionalValue.AdditionalDamage += damageConditions[i].AdditionalValue;
                            Debug.Log("SameAsPreviousTurn Engraving Active");
                        }
                        break;
                    case DamageCondition.ConditionType.FirstAttack:
                        if (battleManager.BattleTurn == 1)
                        {
                            battleManager.EngravingAdditionalValue.AdditionalDamage += damageConditions[i].AdditionalValue;
                            Debug.Log("FirstAttack Engraving Active");
                        }
                        break;
                }
            }
        }

        if (damageConditionsRank != null)
        {
            for (int i = 0; i < damageConditionsRank.Length; i++)
            {
                if (diceManager.DiceRank == damageConditionsRank[i].ConditionRank)
                {
                    battleManager.EngravingAdditionalValue.AdditionalDamage += damageConditionsRank[i].AdditionalValue;
                    Debug.Log($"{diceManager.DiceRank} Engraving Active");
                }
            }
        }
    }

    public void GetEngravingEffectInTurnEnter()
    {
        if (bonusAbilityRoll == null) return;

        for (int i = 0; i < bonusAbilityRoll.Length; i++)
        {
            switch (bonusAbilityRoll[i].Ability)
            {
                case BonusAbilityRoll.AbilityEnum.BonusReroll:
                    Debug.Log("BonusReroll Engraving Active");
                    BattleManager.Instance.EngravingAdditionalValue.AdditionalRollCount += (int)bonusAbilityRoll[i].Value;
                    break;
            }
        }
    }

    public void GetEngravingEffectInBattleEnd()
    {
        if (bonusAbilityRoll == null) return;

        for (int i = 0; i < bonusAbilityRoll.Length; i++)
        {
            switch (bonusAbilityRoll[i].Ability)
            {
                case BonusAbilityRoll.AbilityEnum.FirstTurnKillReward:
                    if (BattleManager.Instance.BattleTurn == 1)
                    {
                        Debug.Log("FirstTurnKillReward Engraving Active");
                    }
                    break;
            }
        }
    }    
}
