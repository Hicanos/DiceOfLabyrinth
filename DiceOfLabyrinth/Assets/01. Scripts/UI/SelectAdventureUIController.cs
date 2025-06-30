using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectAdventureUIController : MonoBehaviour
{
    public ChapterData chapterData;
    [Header("Panels")]
    [SerializeField] private GameObject selectChapterPanel;
    [SerializeField] private GameObject selectDungeonPanel;

    [Header("Difficulty Toggles")]
    [SerializeField] private GameObject NormalDifficultyUnselect;
    [SerializeField] private GameObject NormalDifficultySelect;
    [SerializeField] private GameObject HardDifficultyUnselect;
    [SerializeField] private GameObject HardDifficultySelect;

    [Header("SelectedChapter")]
    [SerializeField] private TMP_Text selectedChapterNameText; // 선택된 챕터 이름 텍스트
    [SerializeField] private TMP_Text selectedChapterDescriptionText; // 선택된 챕터 설명 텍스트

    public bool isDifficulty = false; // 챕터 난이도 선택 여부

    private void Start()
    {
        selectChapterPanel.SetActive(true);
        selectDungeonPanel.SetActive(false);
    }

    public void OpenSelectDungeonPanel(int normalChapterIndex)
    {
        int chapterIndex = normalChapterIndex; // 스테이지 인덱스는 Normal 챕터의 인덱스와 동일합니다.
        if (isDifficulty)
        {
            chapterIndex++; // 하드 챕터의 인덱스는 Normal 챕터 인덱스 + 1입니다.
        }
        Debug.Log($"Selected Chapter Index: {chapterIndex}");
        if (chapterIndex < 0 || chapterIndex >= chapterData.chapterIndex.Count)
        {
            Debug.Log($"Invalid chapter index: {chapterIndex}. 인덱스에 해당하는 챕터 데이터가 없습니다.");
            return;
        }
        else if (StageManager.Instance.stageSaveData.chapterAndStageStates[chapterIndex].isUnLocked)
        {
            Debug.Log($"Chapter: {chapterIndex} is locked.");
            // 챕터가 잠겨있을 때 잠김 상태를 알려주는 UI를 표시하는 로직을 추가할 수 있습니다.
            return;
        }
        else if (StageManager.Instance.stageSaveData.chapterAndStageStates[chapterIndex].isCompleted)
        {
            Debug.Log("Chapter is already completed.");
            // 이미 완료된 챕터를 선택했을 때 완료 상태를 알려주는 UI를 표시하는 로직을 추가할 수 있습니다.
            return;
        }
        //else if({유저데이터에 추가될 스태미나} < chapterData.chapterIndex[chapterIndex].ChapterCost)
        //{
        //    Debug.Log("Not enough stamina to enter the chapter.");
        //    // 스태미나가 부족할 때 알림 UI를 표시하는 로직을 추가할 수 있습니다.
        //}
        else if (chapterIndex != StageManager.Instance.stageSaveData.currentChapterIndex) // 현재 챕터와 선택한 챕터가 다를 때만 초기화합니다.
        {
            // 현재 챕터의 변경을 묻는 UI를 추가할 수 있습니다.
            Debug.Log($"진행중인 챕터가 있는데 바꿀거냐 묻는 UI를 추가할 수 있습니다.");
            //StageManager.Instance.ResetStageData(chapterIndex); // 스테이지 데이터 초기화는 해당 UI를 띄우고 나서 진행합니다.
            return;
        }
        // 새로운 챕터가 아니면, 데이터 초기화를 할 필요가 없습니다.
        // 만약 기존 챕터여도 새 시작을 원한다면, 묻는 UI를 추가할 수 있습니다.
        Debug.Log("테스트 버전에선 새 시작을 가정하여 챕터 데이터를 초기화합니다.");
        StageManager.Instance.stageSaveData.ResetToDefault(chapterIndex); // 선택한 챕터의 기본 상태로 초기화합니다.
        selectChapterPanel.SetActive(true);
        selectDungeonPanel.SetActive(true);
    }   
    public void CloseSelectDungeonPanel()
    {
        selectDungeonPanel.SetActive(false);
    }

    public void OnClickDifficulty(bool DifficultyToggle)
    {
        isDifficulty = DifficultyToggle; // 토글 상태에 따라 난이도 설정
        DifficultyToggleRefresh();
    }


    public void OnClickedDungeon(int stageIndex)
    {
        Debug.Log("On Click Check");
        if (stageIndex < 0 || stageIndex >= chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex.Count)
        {
            Debug.Log($"Invalid stage index: {stageIndex}. 인덱스에 해당하는 스테이지 데이터가 없습니다.");
            return;
        }
        if (!StageManager.Instance.stageSaveData.chapterAndStageStates[StageManager.Instance.stageSaveData.currentChapterIndex].stageStates[stageIndex].isUnLocked)
        {
            Debug.Log($"Stage Index: {stageIndex} is locked.");
            // 스테이지가 잠겨있을 때 잠김 상태를 알려주는 UI를 표시하는 로직을 추가할 수 있습니다.
            return;
        }
        else if (StageManager.Instance.stageSaveData.chapterAndStageStates[StageManager.Instance.stageSaveData.currentChapterIndex].stageStates[stageIndex].isCompleted)
        {
            Debug.Log($"Stage Index: {stageIndex} is already completed.");
            // 이미 완료된 스테이지를 선택했을 때 완료 상태를 알려주는 UI를 표시하는 로직을 추가할 수 있습니다.
            return;
        }
        Debug.Log($"Stage {stageIndex} selected. Loading battle scene.");
        // 배틀 씬으로 이동할거냐 묻는 UI를 추가할 수 있습니다.
        SceneManagerEx.Instance.LoadScene("BattleScene"); // 배틀 씬으로 이동
        StageManager.Instance.RestoreStageState(); // 현재 스테이지 상태를 복원합니다.
    }

    public void DifficultyToggleRefresh()
    {
        if (isDifficulty)
        {
            NormalDifficultyUnselect.SetActive(true);
            NormalDifficultySelect.SetActive(false);
            HardDifficultyUnselect.SetActive(false);
            HardDifficultySelect.SetActive(true);
        }
        else
        {
            NormalDifficultyUnselect.SetActive(false);
            NormalDifficultySelect.SetActive(true);
            HardDifficultyUnselect.SetActive(true);
            HardDifficultySelect.SetActive(false);
        }
    }
}