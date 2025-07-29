using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSetEffectData", menuName = "SetEffect/SetEffectData")]
public class SetEffectData : ScriptableObject
{
    [SerializeField] private Sprite icon;
    [SerializeField] private string effectName;
    [TextArea]
    [SerializeField] private string description;
    [SerializeField] private List<SetEffectTypeData> setEffects;

    public Sprite Icon => icon;
    public string EffectName => effectName;
    public string Description => description;
    public List<SetEffectTypeData> SetEffects => setEffects;

    public float GetEffectValue(SetEffectTypeData.SetEffectType effectType, int count)
    {
        float value = 0f;
        foreach (var setEffect in setEffects)
        {
            if (setEffect.EffectType == effectType)
            {
                int maxCount = 0;
                foreach (var effectCount in setEffect.SetEffectCountData)
                {
                    if (effectCount.Count <= count && effectCount.Count >= maxCount)
                    {
                        maxCount = effectCount.Count;
                        value = effectCount.EffectValue;
                    }
                }
            }
        }
        return value; // 가장 큰 count 이하의 effectValue 반환
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
        HealingWhenStartBattle, // 전투 시작 시 회복
        AdditionalMaxCost, // 추가 최대 비용
        SignitureCostGain, // 턴당 추가 비용 획득
        AdditionalDamageToBoss, // 보스에게 추가 피해
        GainManaStone, // 마나 스톤 획득
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