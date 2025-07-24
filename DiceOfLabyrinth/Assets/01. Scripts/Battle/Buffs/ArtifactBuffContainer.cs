using System.Collections.Generic;

public class ArtifactBuffContainer
{
    List<IBuff> buffs = new List<IBuff>();

    public void Action()
    {
        foreach (var buff in buffs)
        {
            buff.Action();
        }
    }

    public void ReduceDuration()
    {
        foreach (var buff in buffs)
        {
            buff.ReduceDuration();
        }
    }

    public void AddArtifactBuff(IBuff buff)
    {
        buffs.Add(buff);
    }

    public void RemoveArtifactBuff(IBuff buff)
    {
        buffs.Remove(buff);
    }
}

public class ArtifactAdditionalStatus
{
    public float AdditionalDamage;
    public float AdditionalElementDamage;
    public float Stone;
    public float HealHPRatio;
}