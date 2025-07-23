using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ArtifactData", menuName = "ScriptableObjects/Stages/ArtifactData")]
public class ArtifactData : ScriptableObject
{
    public enum ArtifactType
    {
        Common,
        Uncommon,
        Rare,
        Unique,
        Legendary
    }
    [SerializeField]private string artifactName;
    [SerializeField] private string flavorText;
    [SerializeField]private string description;
    [SerializeField]private Sprite icon;
    [SerializeField]private ArtifactType artifactRarity;
    [SerializeField]private ArtifactRaritySprite raritySprite;

    [SerializeField] private List<ArtifactEffectData> artifactEffects = new List<ArtifactEffectData>();
    [SerializeField] private List<SetEffectData> setEffectData = new List<SetEffectData>();

    public string ArtifactName => artifactName;
    public string FlavorText => flavorText;
    public string Description => description;
    public Sprite Icon => icon;
    public int PurchasePrice
    {
        get {
            switch (artifactRarity) {
                case ArtifactType.Common:
                    return 110;
                case ArtifactType.Uncommon:
                    return 180;
                case ArtifactType.Rare:
                    return 260;
                case ArtifactType.Unique:
                    return 380;
                case ArtifactType.Legendary:
                    return 540;
                default:
                    return 9999;
            }
        }
    }
    public int SellPrice
    {         get {
            switch (artifactRarity) {
                case ArtifactType.Common:
                    return 66;
                case ArtifactType.Uncommon:
                    return 108;
                case ArtifactType.Rare:
                    return 156;
                case ArtifactType.Unique:
                    return 228;
                case ArtifactType.Legendary:
                    return 324;
                default:
                    return 0;
            }
        }
    }
    public ArtifactType ArtifactRarity => artifactRarity;
    public List<SetEffectData> SetEffectData => setEffectData;
    public Sprite RaritySprite 
    {
        get {
            if (raritySprite == null)
                return null;
            switch (artifactRarity) {
                case ArtifactType.Common:
                    return raritySprite.CommonSprite;
                case ArtifactType.Uncommon:
                    return raritySprite.UncommonSprite;
                case ArtifactType.Rare:
                    return raritySprite.RareSprite;
                case ArtifactType.Unique:
                    return raritySprite.UniqueSprite;
                case ArtifactType.Legendary:
                    return raritySprite.LegendarySprite;
                default:
                    return raritySprite.CommonSprite;
            }
        }
    }
}
[System.Serializable]
public class ArtifactEffectData
{
    public enum EffectType
    {
        AdditionalElementDamage, //속성 추가 피해
        AdditionalAttack, // 공격력 추가
        AdditionalDefense,// 방어력 추가
        AdditionalMaxHp, // 최대 체력 추가
        AdditionalDamage, // 추가 피해
        AdditionalDiceRoll, // 추가 주사위 굴림
        AdditionalAttackCount, // 추가 공격 횟수
        AdditionalDamageToBoss, // 보스에게 추가 피해
        AdditionalDamageIfHaveSignitureDice, // 시그니처 주사위가 있을 때 추가 피해
        HealingWhenStartBattle, // 전투 시작 시 회복
        DebuffToEnemyAtFirstTurn, // 첫 턴 적에게 디버프
        RemoveDebuffPerTurn, // 턴당 디버프 제거
        CostRegenerationWhenUse10Cost, // 10 코스트 사용 시 비용 재생
        CostRegenerationEveryTurn, // 턴마다 비용 재생
        ReviveWhenDie, // 죽을 때 부활
        AdditionalMaxCost,// 최대 코스트
        AdditionalManaStone, // 마석 추가 획득
        IgnoreDefense, // 방어력 관통
        CriticalChance, // 치명타 확률
        CriticalDamage, // 치명타 피해
        GenerateBarrier, // 보호막 생성
    }
    [SerializeField] private EffectType effectType;
    [SerializeField] private float effectValue;
    public EffectType Type => effectType;
    public float Value => effectValue;
}