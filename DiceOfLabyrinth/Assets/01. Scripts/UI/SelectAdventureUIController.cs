using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectAdventureUIController : MonoBehaviour
{
    public ChapterData chapterData;

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
            Debug.LogError($"Invalid chapter index: {chapterIndex}. Please provide a valid index.");
            return;
        }
        if (chapterData.chapterIndex[chapterIndex].isLocked)
        {
            Debug.Log("Chapter is locked.");
            // 챕터가 잠겨있을 때 잠김 상태를 알려주는 UI를 표시하는 로직을 추가할 수 있습니다.
            return;
        }
        if (chapterData.chapterIndex[chapterIndex].isCompleted)
        {
            Debug.Log("Chapter is already completed.");
            // 이미 완료된 챕터를 선택했을 때 완료 상태를 알려주는 UI를 표시하는 로직을 추가할 수 있습니다.
            return;
        }
        selectDungeonPanel.SetActive(true);
        ChapterManager.Instance.ResetChapterData(chapterIndex); // 챕터 데이터를 초기화합니다.
    }   
    private void CloseSelectDungeonPanel()
    {
        selectDungeonPanel.SetActive(false);
    }

}