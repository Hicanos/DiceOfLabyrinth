using System;

public class EnemyPassive : IBuff
{
    private SOEnemyPassive enemyPassiveSO;
    EnemyPassiveEffectData effectData;
    private Func<float, bool> conditionFunc;
    private Action<float> effectAction;
    private int useCount;

    public EnemyPassive(SOEnemyPassive passiveSO, EnemyPassiveEffectData effectData, Func<float, bool> conditionFunc, Action<float> effectAction, int useCount)
    {
        enemyPassiveSO = passiveSO;
        this.effectData = effectData;
        this.conditionFunc = conditionFunc;
        this.effectAction = effectAction;

        this.useCount = useCount;
    }

    public void Action()
    {        
        if (conditionFunc(effectData.ConditionValue) == false) return;
        if (useCount == 0) return;

        effectAction(effectData.EffectValue);

        UIManager.Instance.BattleUI.BattleUILog.WriteBattleLog(BattleManager.Instance.Enemy.Data.EnemyName, enemyPassiveSO.Name, effectData.Description);
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