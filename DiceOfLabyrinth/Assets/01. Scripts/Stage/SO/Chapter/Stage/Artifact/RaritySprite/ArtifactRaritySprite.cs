using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactRaritySprite", menuName = "ScriptableObjects/Stages/ArtifactRaritySprite", order = 1)]
public class ArtifactRaritySprite : ScriptableObject
{
    [SerializeField]private Sprite commonSprite;
    [SerializeField]private Sprite uncommonSprite;
    [SerializeField]private Sprite rareSprite;
    [SerializeField]private Sprite uniqueSprite;
    [SerializeField]private Sprite legendarySprite;
    public Sprite CommonSprite => commonSprite;
    public Sprite UncommonSprite => uncommonSprite;
    public Sprite RareSprite => rareSprite;
    public Sprite UniqueSprite => uniqueSprite;
    public Sprite LegendarySprite => legendarySprite;
}
