using UnityEngine;

[CreateAssetMenu(fileName = "NewSetEffectData", menuName = "SetEffect/SetEffectData")]
public class SetEffectData : ScriptableObject
{
    [SerializeField] private Sprite icon;
    [SerializeField] private string effectName;
    [TextArea]
    [SerializeField] private string description;

    public Sprite Icon => icon;
    public string EffectName => effectName;
    public string Description => description;
}