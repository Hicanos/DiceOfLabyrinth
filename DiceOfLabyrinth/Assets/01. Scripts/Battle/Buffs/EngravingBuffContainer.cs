using System.Collections.Generic;

public class EngravingBuffContainer
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
    public float[] AdditionalStatus = new float[3];
    
    public EngravingAdditionalStatus()
    {
        AdditionalStatus[0] = 1;
    }
}
