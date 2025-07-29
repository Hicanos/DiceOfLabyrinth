using System.Collections.Generic;

public class ArtifactBuffContainer
{
    List<IBuff> buffs = new List<IBuff>();
    List<IBuff> buffsUpdate = new List<IBuff>();
    List<IBuff> buffsCallbackSpendCost = new List<IBuff>();
    List<IBuff> buffsCallbackCharacterHit = new List<IBuff>();
    List<IBuff> buffsCallbackCharacterDie = new List<IBuff>();
    List<IBuff> buffsCallbackTurnEnter = new List<IBuff>();
    List<IBuff> buffsCallbackCharacterAttack = new List<IBuff>();

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
    public void ActionSpendCost()
    {
        foreach (var buff in buffsCallbackSpendCost)
        {
            buff.Action();
        }
    }
    public void ActionCharacterHit()
    {
        foreach (var buff in buffsCallbackCharacterHit)
        {
            buff.Action();
        }
    }
    public void ActionCharacterDie()
    {
        foreach (var buff in buffsCallbackCharacterDie)
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
    public void AddbuffsCallbackCharacterAttack(IBuff buff)
    {
        buffsCallbackCharacterAttack.Add(buff);
    }
    public void AddbuffsCallbackTurnEnter(IBuff buff)
    {
        buffsCallbackTurnEnter.Add(buff);
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
    public void RemovebuffsCallbackCharacterAttack(IBuff buff)
    {
        buffsCallbackCharacterAttack.Remove(buff);
    }
    public void RemovebuffsCallbackTurnEnter(IBuff buff)
    {
        buffsCallbackTurnEnter.Remove(buff);
    }

    public void RemoveAllBuffs()
    {
        buffs.Clear();
        buffsUpdate.Clear();
        buffsCallbackSpendCost.Clear();
        buffsCallbackCharacterHit.Clear();
        buffsCallbackCharacterDie.Clear();
        buffsCallbackCharacterAttack.Clear();
        buffsCallbackTurnEnter.Clear();
    }
}

public class ArtifactAdditionalStatus
{
    public float AdditionalDamage;
    public float AdditionalElementDamage;
    public float AdditionalRoll;
    public float AdditionalSIgniture;
    public float AdditionalMaxCost;
    public float AdditionalStone;
    public float AdditionalAttack;

    public void ResetStatus()
    {
        AdditionalDamage = 0;
        AdditionalElementDamage = 0;
        AdditionalRoll = 0;
        AdditionalSIgniture = 0;
        AdditionalMaxCost = 0;
        AdditionalStone = 0;
        AdditionalAttack = 0;
    }
}