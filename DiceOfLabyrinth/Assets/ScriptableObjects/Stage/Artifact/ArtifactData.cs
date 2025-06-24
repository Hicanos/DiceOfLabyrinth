using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactData", menuName = "ScriptableObjects/ArtifactData")]
public class ArtifactData : ScriptableObject
{
    public string artifactName;
    public string description;
    public Sprite icon;
}