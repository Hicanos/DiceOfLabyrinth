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
    private string artifactName;
    private string description;
    private Sprite icon;
    private ArtifactType artifactType;
    private int additionalAtk;
    private int additionalDef;

    private List<SetEffectData> setEffectData = new List<SetEffectData>();

    public string ArtifactName => artifactName;
    public string Description => description;
    public Sprite Icon => icon;
    public ArtifactType Type => artifactType;
    public int AdditionalAtk => additionalAtk;
    public int AdditionalDef => additionalDef;
    public List<SetEffectData> SetEffectData => setEffectData;

}