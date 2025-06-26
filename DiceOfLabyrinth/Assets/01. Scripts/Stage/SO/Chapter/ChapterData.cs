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
    public bool isCompleted = false; // é�� �Ϸ� ����, �⺻���� false�� ����
    public bool isLocked = true; // é�� ��� ����, �⺻���� true�� ����

    public StageData stageData;

    public string ChapterName => chapterName;
    public string Description => description;
    public Sprite Image => image;
    public int ChapterCost => chapterCost;
    public int DirectCompleteCost => directCompleteCost;
}
