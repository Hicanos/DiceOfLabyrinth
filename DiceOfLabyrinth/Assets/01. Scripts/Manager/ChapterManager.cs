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

    public void StartChapter(int chapterIndex)
    {
        if (chapterIndex < 0 || chapterIndex >= chapterData.chapterIndex.Count)
        {
            Debug.LogError($"Invalid chapter index: {chapterIndex}. Please provide a valid index.");
            return;
        }
        else if (chapterData.chapterIndex[chapterIndex].isLocked)
        {
            Debug.LogError($"Chapter {chapterIndex} is locked. Please unlock it before starting.");
            return;
        }
        else if (stageManager.currentChapterIndex != chapterIndex)
        {
            StartNewChapter(chapterIndex);
        }
        // é�� UI�� SetActive�� Ȱ��ȭ�ϰ� é�� �ε��� ���� UI�� �����ϴ� ������ �߰��մϴ�.
    }
    private void StartNewChapter(int chapterIndex)
    {
        stageManager.currentChapterIndex = chapterIndex;
        stageManager.currentStageIndex = 0; // é�� ���� �� ù ��° ���������� �ʱ�ȭ
        stageManager.currentPhaseIndex = 0; // é�� ���� �� ù ��° ������� �ʱ�ȭ
        stageManager.gem = 0; // é�� ���� �� ��ȭ �ʱ�ȭ
        stageManager.artifacts.Clear(); // é�� ���� �� ��Ƽ��Ʈ ��� �ʱ�ȭ
        equipedArtifacts.Clear(); // é�� ���� �� ������ ��Ƽ��Ʈ ��� �ʱ�ȭ
        stageManager.stagma.Clear(); // é�� ���� �� ���±׸� ��� �ʱ�ȭ
        // é�� �����Ϳ��� �ش� é�� ������ �����ͼ� �ʿ��� �ʱ�ȭ �۾� ����
        ChapterInfo chapterInfo = chapterData.chapterIndex[chapterIndex];
        if (chapterInfo != null)
        {
            Debug.Log($"Starting Chapter: {chapterInfo.ChapterName}");
            // �߰����� �ʱ�ȭ �۾��� �ʿ��ϴٸ� ���⿡ �ۼ�
        }
    }
}
