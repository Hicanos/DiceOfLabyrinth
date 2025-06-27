using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChapterData", menuName = "ScriptableObjects/Stages/ChapterData", order = 1)]
public class ChapterData : ScriptableObject
{ 
    public List<ChapterInfo> chapterIndex; // ¦�� �ε����� Normal, Ȧ�� �ε����� Hard é�ͷ� ����, �� : 0���� 1é�� Normal, 1���� 1é�� Hard, 2���� 2é�� Normal, 3���� 2é�� Hard ��
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
    [SerializeField] private ChapterDifficulty chapterDifficulty; // é�� ���̵�, Normal �Ǵ� Hard�� ���� ����
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

    public ChapterDifficulty Difficulty => chapterDifficulty; // é�� ���̵� ������Ƽ
}
