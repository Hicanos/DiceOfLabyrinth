using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactData", menuName = "ScriptableObjects/Stages/ArtifactData")]
public class ArtifactData : ScriptableObject
{
    public string artifactName;
    public string description;
    public Sprite icon;
}