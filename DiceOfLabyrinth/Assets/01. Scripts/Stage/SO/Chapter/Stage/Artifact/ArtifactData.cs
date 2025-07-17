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
    public string artifactName;
    public string description;
    public Sprite icon;
    public ArtifactType artifactType;
    public int additionalAtk;
    public int additionalDef;
}