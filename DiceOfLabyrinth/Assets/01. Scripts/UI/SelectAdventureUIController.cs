using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectAdventureUIController : MonoBehaviour
{
    public ChapterData chapterData;
    public StageManager stageManager;

    [SerializeField] private GameObject selectChapterPanel;
    [SerializeField] private GameObject selectDungeonPanel;

    private void Start()
    {
        selectChapterPanel.SetActive(true);
        selectDungeonPanel.SetActive(false);

    }

    public void OpenSelectDungeonPanel(int chapterIndex)
    {
        if (chapterIndex < 0 || chapterIndex >= chapterData.chapterIndex.Count)
        {
            Debug.Log($"Invalid chapter index: {chapterIndex}. 인덱스에 해당하는 챕터 데이터가 없습니다.");
            return;
        }
        else if (chapterData.chapterIndex[chapterIndex].isLocked)
        {
            Debug.Log($"Chapter{chapterIndex + 1} is locked.");
            // 챕터가 잠겨있을 때 잠김 상태를 알려주는 UI를 표시하는 로직을 추가할 수 있습니다.
            return;
        }
        else if (chapterData.chapterIndex[chapterIndex + 1].isCompleted)
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
        else
        {
            Debug.Log($"챕터 데이터를 초기화합니다. 만약 경고 UI가 필요하다면 수정해주세요.");
            ChapterManager.Instance.ResetChapterData(chapterIndex); // 챕터 데이터를 초기화합니다.
        }
        // 새로운 챕터가 아니면, 데이터 초기화를 할 필요가 없습니다.
        // 만약 기존 챕터여도 새 시작을 원한다면, 묻는 UI를 추가할 수 있습니다.
        selectChapterPanel.SetActive(true);
        selectDungeonPanel.SetActive(true);
        ChapterManager.Instance.LoadChapter(chapterIndex); // 챕터 데이터를 로드합니다.
    }   
    public void CloseSelectDungeonPanel()
    {
        selectDungeonPanel.SetActive(false);
    }

    public void OnClickedDungeon(int stageIndex)
    {
        if (stageIndex < 0 || stageIndex >= chapterData.chapterIndex[stageManager.currentChapterIndex].stageData.stageIndex.Count)
        {
            Debug.Log($"Invalid stage index: {stageIndex}. 인덱스에 해당하는 스테이지 데이터가 없습니다.");
            return;
        }
        if (chapterData.chapterIndex[stageManager.currentChapterIndex].stageData.stageIndex[stageIndex].IsLocked)
        {
            Debug.Log($"Stage {stageIndex + 1} is locked.");
            // 스테이지가 잠겨있을 때 잠김 상태를 알려주는 UI를 표시하는 로직을 추가할 수 있습니다.
            return;
        }
        else if (chapterData.chapterIndex[stageManager.currentChapterIndex].stageData.stageIndex[stageIndex].IsCompleted)
        {
            Debug.Log($"Stage {stageIndex + 1} is already completed.");
            // 이미 완료된 스테이지를 선택했을 때 완료 상태를 알려주는 UI를 표시하는 로직을 추가할 수 있습니다.
            return;
        }
        Debug.Log($"Stage {stageIndex + 1} selected. Loading battle scene.");
        SceneManagerEx.Instance.LoadScene("BattleScene"); // 배틀 씬으로 이동
        stageManager.LoadStage(stageManager.currentChapterIndex, stageIndex); // 스테이지 데이터를 로드합니다.
    }
}