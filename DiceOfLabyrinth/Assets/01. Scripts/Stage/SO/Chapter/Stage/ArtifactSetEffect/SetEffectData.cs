using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSetEffectData", menuName = "SetEffect/SetEffectData")]
public class SetEffectData : ScriptableObject
{
    [SerializeField] private Sprite icon;
    [SerializeField] private string effectName;
    [TextArea]
    [SerializeField] private string description;
    [SerializeField] private List<SetEffectTypeData> setEffectCounts;

    public Sprite Icon => icon;
    public string EffectName => effectName;
    public string Description => description;
    public List<SetEffectTypeData> SetEffectCounts => setEffectCounts;
    
    public float GetEffectValue(SetEffectTypeData.SetEffectType effectType, int count)
    {
        foreach (var setEffect in setEffectCounts)
        {
            if (setEffect.EffectType == effectType)
            {
                foreach (var effectCount in setEffect.SetEffectCountData)
                {
                    if (effectCount.Count == count)
                    {
                        return effectCount.EffectValue;
                    }
                }
            }
        }
        return 0f; // 기본값
    }
}

[System.Serializable]
public struct SetEffectTypeData
{
    public enum SetEffectType
    {
        AdditionalElementDamage,// 추가 원소 피해
        DamageReduction,// 피해 감소
        FirstAttackDamage,// 첫 공격 피해
        HealingPerTurn, // 턴당 회복
        AdditionalMaxCost, // 추가 최대 비용
        AdditionalCostGainPerTurn, // 턴당 추가 비용 획득
        AdditionalDamageToBoss, // 보스에게 추가 피해
        AdditionalDamageAfterFewTurns, // 몇 턴 후 추가 피해
        IgnoreDefense, // 방어력 무시
        CriticalChance, // 치명타 확률
        CriticalDamage, // 치명타 피해
    }
    [SerializeField] private SetEffectType setEffectType;
    [SerializeField] private List<SetEffectCountData> setEffectCountData;

    public SetEffectType EffectType => setEffectType;
    public List<SetEffectCountData> SetEffectCountData => setEffectCountData;
}
[System.Serializable]
public struct SetEffectCountData
{
    [SerializeField] private int count;
    [SerializeField] private float effectValue;
    public int Count => count;
    public float EffectValue => effectValue;
}