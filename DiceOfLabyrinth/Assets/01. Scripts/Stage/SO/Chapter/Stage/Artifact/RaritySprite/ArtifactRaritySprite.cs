using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactRaritySprite", menuName = "ScriptableObjects/Stages/ArtifactRaritySprite", order = 1)]
public class ArtifactRaritySprite : ScriptableObject
{
    public Sprite commonSprite;
    public Sprite uncommonSprite;
    public Sprite rareSprite;
    public Sprite uniqueSprite;
    public Sprite legendarySprite;
}
