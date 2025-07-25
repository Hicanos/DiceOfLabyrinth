using System.Collections.Generic;

public class ArtifactBuffContainer
{
    List<IBuff> buffs = new List<IBuff>();
    List<IBuff> buffsUpdate = new List<IBuff>();

    public void Action()
    {
        foreach (var buff in buffs)
        {
            buff.Action();
        }
    }

    public void ActionUpdate()
    {
        foreach (var buff in buffsUpdate)
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
    public void AddArtifactBuffUpdate(IBuff buff)
    {
        buffsUpdate.Add(buff);
    }

    public void RemoveArtifactBuff(IBuff buff)
    {
        buffs.Remove(buff);
    }
    public void RemoveArtifactBuffUpdate(IBuff buff)
    {
        buffsUpdate.Remove(buff);
    }
}

public class ArtifactAdditionalStatus
{
    //AdditionalDamage;
    //AdditionalElementDamage;
    //AdditionalRoll;
    //AdditionalSIgniture;
    //AdditionalMaxCost;
    //AdditionalStone;
    //AdditionalAttack;

    public float[] AdditionalStatus = new float[7];
}