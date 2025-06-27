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

    public void LoadChapter(int chapterIndex)
    {

    }
    public void ResetChapterData(int chapterIndex)
    {
        stageManager.currentChapterIndex = chapterIndex;
        stageManager.currentStageIndex = 0; // 챕터 시작 시 첫 번째 스테이지로 초기화
        stageManager.currentPhaseIndex = 0; // 챕터 시작 시 첫 번째 페이즈로 초기화
        stageManager.gem = 0; // 챕터 시작 시 재화 초기화
        stageManager.artifacts.Clear(); // 챕터 시작 시 아티팩트 목록 초기화
        equipedArtifacts.Clear(); // 챕터 시작 시 장착된 아티팩트 목록 초기화
        stageManager.stagma.Clear(); // 챕터 시작 시 스태그마 목록 초기화
        // 모든 스테이지 잠금/클리어 상태 초기화
        var stages = chapterData.chapterIndex[chapterIndex].stageData.stageIndex;
        for (int i = 0; i < stages.Count; i++)
        {
            stages[i].IsLocked = (i != 0);      // 첫 번째만 false, 나머지는 true
            stages[i].IsCompleted = false;      // 모두 미완료로 초기화
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
            chapterData.chapterIndex[chapterIndex].isCompleted = true; // 챕터 완료 상태를 true로 설정
            if (chapterIndex % 2 == 0) // 짝수 인덱스는 Normal 챕터이므로 Hard 챕터와 다음 Normal 챕터를 잠금 해제합니다.
            {
                chapterData.chapterIndex[chapterIndex + 1].isLocked = false; // hard 챕터 잠금 해제
                chapterData.chapterIndex[chapterIndex + 2].isLocked = false; // 다음 Normal 챕터 잠금 해제
            }
            else // 홀수 인덱스는 Hard 챕터이므로 아무런 챕터도 잠금 해제하지 않습니다.
            {
                // 만일을 위해 빈칸으로 남겨둡니다.
            }
            // 챕터 완료 후 스테이지 데이터 초기화
            ResetChapterData(chapterIndex);
            // 세이브 로직이 추가된다면 여기서 세이브 데이터를 저장하는 로직을 추가합니다.
        }
        else
        {
            Debug.LogError($"Chapter {chapterIndex} not found.");
        }
    }
}
