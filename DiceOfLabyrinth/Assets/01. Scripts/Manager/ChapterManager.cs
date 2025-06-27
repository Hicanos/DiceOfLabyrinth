using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    public ChapterData chapterData;
    public StageManager stageManager;

    public List<ArtifactData> equipedArtifacts = new List<ArtifactData>(); // ���� ������ ��Ƽ��Ʈ ���

    public static ChapterManager Instance { get; private set; }


    private void Awake()
    {
        // �̱��� ������ �����Ͽ� ChapterManager�� �ν��Ͻ��� �ϳ��� �����ϵ��� �մϴ�.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ��� ����
            if (stageManager == null)
            {
                stageManager = GetComponent<StageManager>();
                if (stageManager == null)
                {
                    Debug.LogError("StageManager not found in the scene. Please ensure it is present.");
                }
            }

        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϸ� �ߺ� ���� ����
        }
    }

    public void LoadChapter(int chapterIndex)
    {
        //else if({�÷��̾��� ���¹̳� ���� ���� �� �������} <  chapterData.chapterIndex[chapterIndex].ChapterCost)
        //{
        //     Debug.LogError($"Not enough stamina to start chapter {chapterIndex}. Please recharge your stamina.");
        //     �÷��̾��� ���¹̳��� ������ �� ���¹̳� ���� ���¸� �˷��ִ� UI�� ǥ���ϴ� ������ �߰��� �� �ֽ��ϴ�.
        //     return;
        //}
        // ResetChapterData(chapterIndex); // é�� �����͸� �ʱ�ȭ�մϴ�.
        // é�� UI�� SetActive�� Ȱ��ȭ�ϴ� ������ �߰��� �� �ֽ��ϴ�.
    }
    public void ResetChapterData(int chapterIndex)
    {
        stageManager.currentChapterIndex = chapterIndex;
        stageManager.currentStageIndex = 0; // é�� ���� �� ù ��° ���������� �ʱ�ȭ
        stageManager.currentPhaseIndex = 0; // é�� ���� �� ù ��° ������� �ʱ�ȭ
        stageManager.gem = 0; // é�� ���� �� ��ȭ �ʱ�ȭ
        stageManager.artifacts.Clear(); // é�� ���� �� ��Ƽ��Ʈ ��� �ʱ�ȭ
        equipedArtifacts.Clear(); // é�� ���� �� ������ ��Ƽ��Ʈ ��� �ʱ�ȭ
        stageManager.stagma.Clear(); // é�� ���� �� ���±׸� ��� �ʱ�ȭ
        // ��� �������� ���/Ŭ���� ���� �ʱ�ȭ
        var stages = chapterData.chapterIndex[chapterIndex].stageData.stageIndex;
        for (int i = 0; i < stages.Count; i++)
        {
            stages[i].IsLocked = (i != 0);      // ù ��°�� false, �������� true
            stages[i].IsCompleted = false;      // ��� �̿Ϸ�� �ʱ�ȭ
        }
    }
    public void CompleteChapter(int chapterIndex)
    {
        if (chapterIndex < 0 || chapterIndex >= chapterData.chapterIndex.Count)
        {
            Debug.LogError($"Invalid chapter index: {chapterIndex}. Please provide a valid index.");
            return;
        }
        if (chapterData.chapterIndex[chapterIndex] != null)
        {
            chapterData.chapterIndex[chapterIndex].isCompleted = true; // é�� �Ϸ� ���¸� true�� ����
            chapterData.chapterIndex[chapterIndex +1].isLocked = false; // ���� é�� ��� ����
            Debug.Log($"Chapter {chapterData.chapterIndex[chapterIndex].ChapterName} completed!");
        }
        else
        {
            Debug.LogError($"Chapter {chapterIndex} not found.");
        }
    }
}
