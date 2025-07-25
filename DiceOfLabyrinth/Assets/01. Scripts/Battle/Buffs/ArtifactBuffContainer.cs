using System.Collections.Generic;

public class ArtifactBuffContainer
{
    List<IBuff> buffs = new List<IBuff>();
    List<IBuff> buffsUpdate = new List<IBuff>();
    List<IBuff> buffsCallbackSpendCost = new List<IBuff>();
    List<IBuff> buffsCallbackCharacterHit = new List<IBuff>();
    List<IBuff> buffsCallbackCharacterDie = new List<IBuff>();

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
    public void ActionCallbackSpendCost()
    {
        foreach (var buff in buffsCallbackSpendCost)
        {
            buff.Action();
        }
    }
    public void ActionCallbackCharacterHit()
    {
        foreach (var buff in buffsCallbackCharacterHit)
        {
            buff.Action();
        }
    }
    public void ActionCallbackCharacterDie()
    {
        foreach (var buff in buffsCallbackCharacterDie)
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
    public void AddbuffsCallbackSpendCost(IBuff buff)
    {
        buffsCallbackSpendCost.Add(buff);
    }
    public void AddbuffsCallbackCharacterHit(IBuff buff)
    {
        buffsCallbackCharacterHit.Add(buff);
    }
    public void AddbuffsCallbackCharacterDie(IBuff buff)
    {
        buffsCallbackCharacterDie.Add(buff);
    }

    public void RemoveArtifactBuff(IBuff buff)
    {
        buffs.Remove(buff);
    }
    public void RemoveArtifactBuffUpdate(IBuff buff)
    {
        buffsUpdate.Remove(buff);
    }
    public void RemovebuffsCallbackSpendCost(IBuff buff)
    {
        buffsUpdate.Remove(buff);
    }
    public void RemovebuffsCallbackCharacterHit(IBuff buff)
    {
        buffsUpdate.Remove(buff);
    }
    public void RemovebuffsCallbackCharacterDie(IBuff buff)
    {
        buffsUpdate.Remove(buff);
    }

    public void RemoveAllBuffs()
    {
        buffs.Clear();
        buffsUpdate.Clear();
        buffsCallbackSpendCost.Clear();
        buffsCallbackCharacterHit.Clear();
        buffsCallbackCharacterDie.Clear();
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

    public void ResetStatus()
    {
        for(int i = 0; i < AdditionalStatus.Length; i++)
        {
            AdditionalStatus[i] = 0f;
        }
    }
}