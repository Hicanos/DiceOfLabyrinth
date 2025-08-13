using System;

public interface ISkillMaker
{
    public void MakeSkill();

    public Func<float, bool> GetCondition(int enumIndex);

    public Action<float> GetEffect(int enumIndex);
}

public interface ISkillLocationMaker
{
    public Action<IBuff> GetEffectLocation(int enumIndex);
}
