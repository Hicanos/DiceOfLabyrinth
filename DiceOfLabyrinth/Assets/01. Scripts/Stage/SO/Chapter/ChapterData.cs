using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChapterData", menuName = "ScriptableObjects/Stages/ChapterData", order = 1)]
public class ChapterData : ScriptableObject
{ 
    public List<ChapterInfo> chapterIndex; // 짝수 인덱스는 Normal, 홀수 인덱스는 Hard 챕터로 설정, 예 : 0번은 1챕터 Normal, 1번은 1챕터 Hard, 2번은 2챕터 Normal, 3번은 2챕터 Hard 등
}
[System.Serializable]
public class ChapterInfo
{
    public enum ChapterDifficulty
    {
        Normal,
        Hard,
    }
    [SerializeField] private string chapterName;
    [SerializeField] private ChapterDifficulty chapterDifficulty; // 챕터 난이도, Normal 또는 Hard로 설정 가능
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

    public ChapterDifficulty Difficulty => chapterDifficulty; // 챕터 난이도 프로퍼티
}
