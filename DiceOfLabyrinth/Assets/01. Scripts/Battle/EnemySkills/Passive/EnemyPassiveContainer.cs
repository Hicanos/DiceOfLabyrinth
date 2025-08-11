using System.Collections.Generic;

public class EnemyPassiveContainer
{
    public List<IBuff> PassiveEnemyHit = new List<IBuff>();
    public List<IBuff> PassiveEnemyAttack = new List<IBuff>();
    public List<IBuff> PassiveBattleStart = new List<IBuff>();

    public void ActionPassiveEnemyHit()
    {
        foreach (IBuff buff in PassiveEnemyHit)
        { buff.Action(); }
    }
    public void ActionPassiveEnemyAttack()
    {
        foreach (IBuff buff in PassiveEnemyAttack)
        { buff.Action(); }
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
