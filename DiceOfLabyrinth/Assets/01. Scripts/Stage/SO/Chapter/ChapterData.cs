using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChapterData", menuName = "ScriptableObjects/Stages/ChapterData", order = 1)]
public class ChapterData : ScriptableObject
{ 
    public List<ChapterInfo> chapterIndex;
}
[System.Serializable]
public class ChapterInfo
{
    [SerializeField] private string chapterName;
    [SerializeField] private string description;
    [SerializeField] private Sprite image;
    public StageData stageData;

    public string ChapterName => chapterName;
    public string Description => description;
    public Sprite Image => image;
}
