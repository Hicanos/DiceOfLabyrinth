using System;
using System.Collections.Generic;

public class EnemyPassiveMaker
{
    public void MakePassive(SOEnemyPassive passiveSO)
    {
        EnemyPassive enemyPassiveData;
        EnemyPassiveEffectData[] datas = passiveSO.Effects;

        for(int i = 0; i < datas.Length; i++)
        {
            enemyPassiveData = new EnemyPassive(passiveSO, GetConditionFunc(datas[i]), GetEffectFunc(datas[i]), passiveSO.Effects[i].UseCount);
            BattleManager battleManager = BattleManager.Instance;
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

    private Func<int, bool> GetConditionFunc(EnemyPassiveEffectData data)
    {
        switch (data.ConditionType)
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
    private bool HPRatioConditionFunc(int value)
    {
        BattleEnemy enemy = BattleManager.Instance.Enemy;
        int hp = enemy.CurrentHP;
        int compareHP = enemy.MaxHP * value / 100;

        if (hp <= compareHP)
        {
            return true;
        }
        return false;
    }
    private bool UseSkillIndexConditionFunc(int value)
    {
        if(BattleManager.Instance.Enemy.currentSkill_Index == value)
        {
            return true;
        }
        return false;
    }
    private bool DefaultConditionFunc(int value)
    {
        return true;
    }
    #endregion

    private Action<int> GetEffectFunc(EnemyPassiveEffectData data)
    {
        switch (data.EffectType)
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
    private void AdditionalAtkAction(int value)
    {
        BattleEnemy enemy = BattleManager.Instance.Enemy;

        int atk = enemy.CurrentAtk * value / 100;

        enemy.AdditionalAtk = atk;
    }
    private void AdditionalDefAction(int value)
    {
        BattleEnemy enemy = BattleManager.Instance.Enemy;

        int def = enemy.CurrentDef * value / 100;

        enemy.AdditionalDef = def;
    }
    private void AttackTargetBackAction(int value)
    {

    }
    private void AttackTargetHighestAtkAction(int value)
    {

    }
    private void RestoreHPAction(int value)
    {
        BattleEnemy enemy = BattleManager.Instance.Enemy;
        int amount = enemy.MaxHP * value / 100;

        if (value > 0)
        {
            enemy.Heal(amount);
        }
        else
        {
            enemy.TakeDamage(amount);
        }
    }
    private void GetBarrierAction(int value)
    {
        BattleManager.Instance.Enemy.GetBarrier(value);
    }
    private void LifeStealAction(int value)
    {
        BattleManager battleManager = BattleManager.Instance;
        int amount = battleManager.PartyData.CurrentHitDamage * value / 100;

        battleManager.Enemy.Heal(amount);
    }
    #endregion
}

public class EnemyPassive : IBuff
{
    private SOEnemyPassive enemyPassiveSO;
    private Func<int, bool> conditionFunc;
    private Action<int> effectAction;
    private int useCount;

    public EnemyPassive(SOEnemyPassive passiveSO, Func<int, bool> conditionFunc, Action<int> effectAction, int useCount)
    {
        enemyPassiveSO = passiveSO;
        this.conditionFunc = conditionFunc;
        this.effectAction = effectAction;

        this.useCount = useCount;
    }

    public void Action()
    {
        for(int i = 0; i < enemyPassiveSO.Effects.Length; i++)
        {
            if (conditionFunc(enemyPassiveSO.Effects[i].ConditionValue) == false) return;
            if (useCount == 0) return;

            effectAction(enemyPassiveSO.Effects[i].EffectValue);
        }

        UIManager.Instance.BattleUI.BattleUILog.WriteBattleLog(BattleManager.Instance.Enemy, enemyPassiveSO);
        ReduceDuration();
    }

    public void ReduceDuration()
    {
        useCount--;
        if (useCount < 0)
        {
            useCount = 0;
        }
        
    }
}

public class EnemyPassiveContainer
{
    public List<IBuff> PassiveEnemyHit = new List<IBuff>();
    public List<IBuff> PassiveEnemyAttack = new List<IBuff>();
    public List<IBuff> PassiveBattleStart = new List<IBuff>();

    public void ActionPassiveEnemyHit()
    {
        foreach(IBuff buff in PassiveEnemyHit)
        { buff.Action(); }
    }
    public void ActionPassiveEnemyAttack()
    {
        foreach (IBuff buff in PassiveEnemyAttack)
        {  buff.Action(); }
    }
    public void ActionPassiveBattleStart()
    {
        foreach (IBuff buff in PassiveBattleStart)
        { buff.Action(); }
    }

    public void AddPassiveEnemyHit(IBuff buff)
    { PassiveEnemyHit.Add(buff); }
    public void AddPassiveEnemyAttack(IBuff buff)
    { PassiveEnemyAttack.Add(buff); }
    public void AddPassiveBattleStart(IBuff buff)
    { PassiveBattleStart.Add(buff); }
}
