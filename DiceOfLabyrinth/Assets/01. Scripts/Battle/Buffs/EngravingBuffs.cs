using System.Collections.Generic;

public class EngravingBuffs
{
    private List<IBuff> Buffs = new List<IBuff>();

    public void Action()
    {
        foreach (var buff in Buffs)
        {
            buff.Action();
        }
    }

    public void ReduceDuration()
    {
        foreach(var buff in Buffs)
        {
            buff.ReduceDuration();
        }
    }

    public void AddEngravingBuffs(IBuff buff)
    {
        Buffs.Add(buff);
    }

    public void RemoveEngravingBuffs(IBuff buff)
    {
        Buffs.Remove(buff);
    }
}

public class EngravingAdditionalStatus
{
    public float AdditionalDamage;
    public float AdditionalRoll;
    public float AdditionalStone;

    public float[] AdditionalStatus = new float[3];
}
