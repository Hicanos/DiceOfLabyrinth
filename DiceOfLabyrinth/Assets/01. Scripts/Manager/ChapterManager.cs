using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
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

        /// 저장된 보상들을 정산하는 로직은 유저데이터가 만들어지면 추가할 예정입니다.
        // {유저데이터의 경험치} += StageManager.Instance.stageSaveData.savedExpReward;
        // {유저데이터의 골드} += StageManager.Instance.stageSaveData.savedGoldReward;
        // {유저데이터의 쥬얼} += StageManager.Instance.stageSaveData.savedJewelReward;
        ///
        var states = StageManager.Instance.stageSaveData.chapterAndStageStates;
        states[chapterIndex].isCompleted = true;

        int groupIndex = chapterIndex / 10;
        List<int> normalChapters = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            int normalIdx = groupIndex * 10 + i * 2;
            if (normalIdx < states.Count)
                normalChapters.Add(normalIdx);
        }

        // 노말 챕터 5개가 모두 클리어됐는지 확인
        bool allNormalCompleted = normalChapters.All(idx => states[idx].isCompleted);

        if (allNormalCompleted)
        {
            // 하드 챕터 5개 해금
            foreach (var normalIdx in normalChapters)
            {
                int hardIdx = normalIdx + 1;
                if (hardIdx < states.Count)
                    states[hardIdx].isUnLocked = true;
            }
            // 다음 노말 챕터 5개 해금
            foreach (var normalIdx in normalChapters)
            {
                int nextNormalIdx = normalIdx + 10;
                if (nextNormalIdx < states.Count)
                    states[nextNormalIdx].isUnLocked = true;
            }
        }

        StageManager.Instance.stageSaveData.ResetToDefault(-1); // 챕터 완료 후 스테이지 데이터 초기화, -1은 현재 챕터가 셀렉트되지 않았음을 의미합니다.
    }

    public void DefeatChapter(int chapterIndex)
    {
        if (chapterIndex < 0 || chapterIndex >= chapterData.chapterIndex.Count)
        {
            Debug.LogError($"Invalid chapter index: {chapterIndex}. Please provide a valid index.");
            return;
        }
        var states = StageManager.Instance.stageSaveData.chapterAndStageStates;
        states[chapterIndex].isCompleted = false; // 챕터 완료 상태를 해제
        states[chapterIndex].isUnLocked = true; // 챕터 잠금 해제 상태 유지
        StageManager.Instance.stageSaveData.ResetToDefault(-1); // 챕터 패배 후 스테이지 데이터 초기화, -1은 현재 챕터가 셀렉트되지 않았음을 의미합니다.
    }
}
