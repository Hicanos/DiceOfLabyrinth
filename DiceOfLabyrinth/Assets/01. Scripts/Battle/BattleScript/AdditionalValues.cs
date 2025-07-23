public class AdditionalValues
{
    public int AdditionalMaxCost;
    public float AdditionalElementDamage;
    public float AdditionalDamage;
    public float GetAdditionalDamageToBoss;
    public float AdditionalDamageToBoss => BattleManager.Instance.IsBoss ? GetAdditionalDamageToBoss : 0;
    public int AdditionalRollCount;
    public float TotalAdditionalDamage => AdditionalDamage + AdditionalDamageToBoss;
    private int DefultAdditionalDamage;

    public void Init(int additionalDamage)
    {
        AdditionalDamage = additionalDamage;
        DefultAdditionalDamage = additionalDamage;
    }

    public void Reset()
    {
        AdditionalMaxCost = 0;
        AdditionalElementDamage = 0;
        AdditionalDamage = DefultAdditionalDamage;
        GetAdditionalDamageToBoss = 0;
        AdditionalRollCount = 0;
        AdditionalMaxCost = 0;
        DefultAdditionalDamage = 0;
    }
}
