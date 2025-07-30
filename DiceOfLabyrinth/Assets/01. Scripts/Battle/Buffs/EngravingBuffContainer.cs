using System.Collections.Generic;

public class EngravingBuffContainer
{
    private List<IBuff> buffs = new List<IBuff>();
    private List<IBuff> buffsCallbackCharacterAttack = new List<IBuff>();
    private List<IBuff> buffsCallbackTurnEnter = new List<IBuff>();
    private List<IBuff> buffsCallbackTurnEnd = new List<IBuff>();

    public void Action()
    {
        foreach (var buff in buffs)
        {
            buff.Action();
        }
    }
    public void ActionCharacterAttack()
    {
        foreach (var buff in buffsCallbackCharacterAttack)
        {
            buff.Action();
        }
    }
    public void ActionTurnEnter()
    {
        foreach (var buff in buffsCallbackTurnEnter)
        {
            buff.Action();
        }
    }
    public void ActionTurnEnd()
    {
        foreach (var buff in buffsCallbackTurnEnd)
        {
            buff.Action();
        }
    }

    public void ReduceDuration()
    {
        foreach(var buff in buffs)
        {
            buff.ReduceDuration();
        }
        foreach (var buff in buffsCallbackCharacterAttack)
        {
            buff.ReduceDuration();
        }
        foreach (var buff in buffsCallbackTurnEnter)
        {
            buff.ReduceDuration();
        }
        foreach (var buff in buffsCallbackTurnEnd)
        {
            buff.ReduceDuration();
        }
    }

    public void AddEngravingBuffs(IBuff buff)
    {
        buffs.Add(buff);
    }
    public void AddBuffsCallbackCharacterAttack(IBuff buff)
    {
        buffsCallbackCharacterAttack.Add(buff);
    }
    public void AddBuffsCallbackTurnEnd(IBuff buff)
    {
        buffsCallbackTurnEnd.Add(buff);
    }
    public void AddBuffsCallbackTurnEnter(IBuff buff)
    {
        buffsCallbackTurnEnter.Add(buff);
    }

    public void RemoveEngravingBuffs(IBuff buff)
    {
        buffs.Remove(buff);
    }
    public void RemoveBuffsCallbackCharacterAttack(IBuff buff)
    {
        buffsCallbackCharacterAttack.Remove(buff);
    }
    public void RemoveBuffsCallbackTurnEnd(IBuff buff)
    {
        buffsCallbackTurnEnd.Remove(buff);
    }
    public void RemoveBuffsCallbackTurnEnter(IBuff buff)
    {
        buffsCallbackTurnEnter.Remove(buff);
    }

    public void RemoveAllBuffs()
    {
        buffs.Clear();
        buffsCallbackCharacterAttack.Clear();
        buffsCallbackTurnEnd.Clear();
        buffsCallbackTurnEnter.Clear();
    }
}

public class EngravingAdditionalStatus
{
    public float AdditionalDamage;
    public float AdditionalRoll;
    public float AdditionalStone;

    public EngravingAdditionalStatus()
    {
        AdditionalDamage = 1;
        AdditionalRoll = 0;
        AdditionalStone = 1;
    }
    public void ResetStatus()
    {
        AdditionalDamage = 1;
        AdditionalRoll = 0;
        AdditionalStone = 1;
    }
}
