using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    public ChapterData chapterData;
    public StageManager stageManager;

    public List<ArtifactData> equipedArtifacts = new List<ArtifactData>(); // 현재 장착된 아티팩트 목록

    public static ChapterManager Instance { get; private set; }


    private void Awake()
    {
        // 싱글턴 패턴을 적용하여 ChapterManager의 인스턴스가 하나만 존재하도록 합니다.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
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
            Destroy(gameObject); // 이미 인스턴스가 존재하면 중복 생성 방지
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
        // 챕터 UI를 SetActive로 활성화하고 챕터 인덱스 값을 UI에 전달하는 로직을 추가합니다.
    }
    private void StartNewChapter(int chapterIndex)
    {
        stageManager.currentChapterIndex = chapterIndex;
        stageManager.currentStageIndex = 0; // 챕터 시작 시 첫 번째 스테이지로 초기화
        stageManager.currentPhaseIndex = 0; // 챕터 시작 시 첫 번째 페이즈로 초기화
        stageManager.gem = 0; // 챕터 시작 시 재화 초기화
        stageManager.artifacts.Clear(); // 챕터 시작 시 아티팩트 목록 초기화
        equipedArtifacts.Clear(); // 챕터 시작 시 장착된 아티팩트 목록 초기화
        stageManager.stagma.Clear(); // 챕터 시작 시 스태그마 목록 초기화
        // 챕터 데이터에서 해당 챕터 정보를 가져와서 필요한 초기화 작업 수행
        ChapterInfo chapterInfo = chapterData.chapterIndex[chapterIndex];
        if (chapterInfo != null)
        {
            Debug.Log($"Starting Chapter: {chapterInfo.ChapterName}");
            // 추가적인 초기화 작업이 필요하다면 여기에 작성
        }
    }
}
