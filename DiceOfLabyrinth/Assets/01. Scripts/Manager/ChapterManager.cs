using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    public ChapterData chapterData;
    public StageManager stageManager;

    public List<ArtifactData> equipedArtifacts = new List<ArtifactData>(); // 현재 장착된 아티팩트 목록

    public static ChapterManager Instance { get; private set; }
    public List<ChapterAndStageStates> ChapterAndStageStates
    {
        get
        {
            return StageManager.Instance != null && StageManager.Instance.stageSaveData != null
                ? StageManager.Instance.stageSaveData.chapterAndStageStates
                : null;
        }
    }


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
    public void CompleteChapter(int chapterIndex)
    {
        if (chapterIndex < 0 || chapterIndex >= chapterData.chapterIndex.Count)
        {
            Debug.LogError($"Invalid chapter index: {chapterIndex}. Please provide a valid index.");
            return;
        }
        if (StageManager.Instance.stageSaveData.chapterAndStageStates[chapterIndex] != null)
        {
            StageManager.Instance.stageSaveData.chapterAndStageStates[chapterIndex].isCompleted = true; // 챕터 완료 상태를 true로 설정
            if (chapterIndex % 2 == 0) // 짝수 인덱스는 Normal 챕터이므로 Hard 챕터와 다음 Normal 챕터를 잠금 해제합니다.
            {
                StageManager.Instance.stageSaveData.chapterAndStageStates[chapterIndex + 1].isUnLocked = true; // hard 챕터 잠금 해제
                StageManager.Instance.stageSaveData.chapterAndStageStates[chapterIndex + 2].isUnLocked = true; // 다음 Normal 챕터 잠금 해제
            }
            else // 홀수 인덱스는 Hard 챕터이므로 아무런 챕터도 잠금 해제하지 않습니다.
            {
                // 만일을 위해 빈칸으로 남겨둡니다.
            }
            StageManager.Instance.ResetStageData(chapterIndex); // 챕터 완료 후 스테이지 데이터를 초기화합니다.
        }
        else
        {
            Debug.LogError($"Chapter {chapterIndex} not found.");
        }
    }
}
