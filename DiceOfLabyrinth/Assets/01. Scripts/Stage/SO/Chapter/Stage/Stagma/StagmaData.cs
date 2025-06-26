using UnityEngine;

[CreateAssetMenu(fileName = "StagmaData", menuName = "ScriptableObjects/Stages/StagmaData")]
public class StagmaData : ScriptableObject
{
    public string stagmaName;
    public string description;
    public Sprite icon;
    public float additionalDamage;
}
