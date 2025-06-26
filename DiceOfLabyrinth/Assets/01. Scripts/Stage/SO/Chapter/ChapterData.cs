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
    [SerializeField] private int chapterCost;
    [SerializeField] private int directCompleteCost;
    public bool isCompleted = false; // 챕터 완료 상태, 기본값은 false로 설정
    public bool isLocked = true; // 챕터 잠금 상태, 기본값은 true로 설정

    public StageData stageData;

    public string ChapterName => chapterName;
    public string Description => description;
    public Sprite Image => image;
    public int ChapterCost => chapterCost;
    public int DirectCompleteCost => directCompleteCost;
}
