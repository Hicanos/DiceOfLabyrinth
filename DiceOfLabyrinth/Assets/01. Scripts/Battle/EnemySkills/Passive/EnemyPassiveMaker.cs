using System;
using System.Collections.Generic;

public class EnemyPassiveMaker : ISkillMaker
{
    public void MakeSkill()
    {
        BattleManager battleManager = BattleManager.Instance;

        List<int> passiveIndexes = battleManager.Enemy.Data.PassiveSkills;

        SOEnemyPassive passiveSO;
        EnemyPassive enemyPassiveData;
        EnemyPassiveEffectData[] datas;

        for(int i = 0; i < passiveIndexes.Count; i++)
        {
            passiveSO = battleManager.EnemyPatternContainer.GetPassiveSO(passiveIndexes[i]);
            datas = passiveSO.Effects;

            for(int j = 0; j < datas.Length; j++)
            {
                enemyPassiveData = new EnemyPassive(passiveSO, datas[j], GetCondition((int)datas[j].ConditionType), GetEffect((int)datas[j].EffectType), passiveSO.Effects[j].UseCount);
                
                switch (passiveSO.EffectLocation)
                {
                    case EnemyPassiveEffectLocationEnum.EnemyHit:
                        battleManager.Enemy.PassiveContainer.AddPassiveEnemyHit(enemyPassiveData);
                        break;
                    case EnemyPassiveEffectLocationEnum.EnemyAttack:
                        battleManager.Enemy.PassiveContainer.AddPassiveEnemyAttack(enemyPassiveData);
                        break;
                    case EnemyPassiveEffectLocationEnum.BattleStart:
                        battleManager.Enemy.PassiveContainer.AddPassiveBattleStart(enemyPassiveData);
                        break;
                }
            }            
        }
    }

    public Func<float, bool> GetCondition(int enumIndex)
    {
        switch ((EnemyPassiveConditionEnum)enumIndex)
        {
            case EnemyPassiveConditionEnum.HPRatio:
                return HPRatioConditionFunc;
            case EnemyPassiveConditionEnum.UseSkillIndex:
                return UseSkillIndexConditionFunc;
            default:
                return DefaultConditionFunc;
        }
    }

    #region 조건 판별 메서드들
    private bool HPRatioConditionFunc(float value)
    {
        BattleEnemy enemy = BattleManager.Instance.Enemy;
        int hp = enemy.CurrentHP;
        float compareHP = enemy.MaxHP * value;

        if (hp <= compareHP)
        {
            return true;
        }
        return false;
    }
    private bool UseSkillIndexConditionFunc(float value)
    {
        if(BattleManager.Instance.Enemy.currentSkill_Index == (int)value)
        {
            return true;
        }
        return false;
    }
    private bool DefaultConditionFunc(float value)
    {
        return true;
    }
    #endregion

    public Action<float> GetEffect(int enumIndex)
    {
        switch ((EnemyPassiveEffectEnum)enumIndex)
        {
            case EnemyPassiveEffectEnum.AdditionalAtk:
                return AdditionalAtkAction;
            case EnemyPassiveEffectEnum.AdditionalDef:
                return AdditionalDefAction;
            case EnemyPassiveEffectEnum.AttackTargetBack:
                return AttackTargetBackAction;
            case EnemyPassiveEffectEnum.AttackTargetHighestAtk:
                return AttackTargetHighestAtkAction;
            case EnemyPassiveEffectEnum.RestoreHP:
                return RestoreHPAction;
            case EnemyPassiveEffectEnum.GetBarrier:
                return GetBarrierAction;
            case EnemyPassiveEffectEnum.LifeSteal:
                return LifeStealAction;
            default:
                return null;
        }
    }

    #region 패시브 기능 메서드들
    private void AdditionalAtkAction(float value)
    {
        BattleEnemy enemy = BattleManager.Instance.Enemy;

        float atk = enemy.CurrentAtk * value;

        enemy.AdditionalAtk = (int)atk;
    }
    private void AdditionalDefAction(float value)
    {
        BattleEnemy enemy = BattleManager.Instance.Enemy;

        float def = enemy.CurrentDef * value;

        enemy.AdditionalDef = (int)def;
    }
    private void AttackTargetBackAction(float value)
    {

    }
    private void AttackTargetHighestAtkAction(float value)
    {

    }
    private void RestoreHPAction(float value)
    {
        BattleEnemy enemy = BattleManager.Instance.Enemy;
        float amount = enemy.MaxHP * value;

        if (value > 0)
        {
            enemy.Heal((int)amount);
        }
        else
        {
            enemy.TakeDamage((int)amount);
        }
    }
    private void GetBarrierAction(float value)
    {
        BattleManager.Instance.Enemy.GetBarrier(value);
    }
    private void LifeStealAction(float value)
    {
        BattleManager battleManager = BattleManager.Instance;
        float amount = battleManager.PartyData.CurrentHitDamage * value;

        battleManager.Enemy.Heal((int)amount);
    }
    #endregion
}
