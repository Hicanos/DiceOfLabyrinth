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
    [SerializeField]private ArtifactType artifactType;
    [SerializeField]private int additionalAtk;
    [SerializeField]private int additionalDef;
    [SerializeField]private ArtifactRaritySprite raritySprite;


    [SerializeField] private List<SetEffectData> setEffectData = new List<SetEffectData>();

    public string ArtifactName => artifactName;
    public string FlavorText => flavorText;
    public string Description => description;
    public Sprite Icon => icon;
    public ArtifactType Type => artifactType;
    public int AdditionalAtk => additionalAtk;
    public int AdditionalDef => additionalDef;
    public List<SetEffectData> SetEffectData => setEffectData;
    public Sprite RaritySprite 
    {
        get {
            if (raritySprite == null)
                return null;
            switch (artifactType) {
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