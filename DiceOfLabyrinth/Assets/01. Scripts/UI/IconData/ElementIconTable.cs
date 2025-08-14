using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ElementIconTable", menuName = "GameData/ElementIconTable")]
public class ElementIconTable : ScriptableObject
{
    [System.Serializable]
    public struct ElementIconPair
    {
        public DesignEnums.ElementTypes elementType;
        public Sprite icon;
    }

    public List<ElementIconPair> elementIcons;

    public Sprite GetIcon(DesignEnums.ElementTypes type)
    {
        var pair = elementIcons.Find(e => e.elementType == type);
        return pair.icon;
    }
}