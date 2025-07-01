using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SelectAdventureUIController : MonoBehaviour
{
    public UserData userData; // 유저 데이터
    public ChapterData chapterData;

    private int selectedChapterIndex = -1; // 선택된 챕터 인덱스

    [Header("Panels")]
    [SerializeField] private GameObject selectChapterPanel;
    [SerializeField] private GameObject costCalculationPanel;
    //[SerializeField] private GameObject selectDungeonPanel; //배틀씬으로 패널을 옮겼으므로 주석 처리
    [SerializeField] private GameObject scaresStaminaPanel;
    //[SerializeField] private GameObject teamFormationPanel; // 배틀씬으로 패널을 옮겼으므로 주석 처리

    [Header("Difficulty Toggles")]
    [SerializeField] private GameObject NormalDifficultyUnselect;
    [SerializeField] private GameObject NormalDifficultySelect;
    [SerializeField] private GameObject HardDifficultyUnselect;
    [SerializeField] private GameObject HardDifficultySelect;

    [Header("SelectedChapter")]
    [SerializeField] private List<TMP_Text> selectedChapterNameText = new List<TMP_Text>(); // 선택된 챕터 이름 텍스트, 여러 개의 챕터 이름을 표시할 수 있도록 리스트로 변경
    //[SerializeField] private TMP_Text selectedChapterDescriptionText; // 선택된 챕터 설명 텍스트, 현재 기획에선 설명이 필요하지 않으므로 주석 처리

    [Header("Scares and Stamina Panel")]
    [SerializeField] private TMP_Text directCompleteMultiplierText; // 직접 완료 배수 텍스트
    private int directCompleteMultiplier;

    public bool isDifficulty = false; // 챕터 난이도 선택 여부

    private void Start()
    {
        selectChapterPanel.SetActive(true);
        costCalculationPanel.SetActive(false);
        scaresStaminaPanel.SetActive(false);
        DifficultyToggleRefresh(); // 초기 난이도 토글 상태 설정

    }
    public void OnClickChapterButton(int normalChapterIndex)
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
        else if (!StageManager.Instance.stageSaveData.chapterAndStageStates[chapterIndex].isUnLocked)
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
        else if (StageManager.Instance.stageSaveData.currentChapterIndex == -1) // -1은 진행중인 챕터가 없음을 의미합니다.
        {
            Debug.Log($"새챕터 새로 시작: {chapterIndex}");
            OpenCostCalculationPanel(chapterIndex); // 입장 코스트를 묻는 패널을 엽니다.
        }
        else if (chapterIndex != StageManager.Instance.stageSaveData.currentChapterIndex) // 현재 챕터와 선택한 챕터가 다를 때엔 이전 챕터의 종료를 먼저 하라는 팝업을 띄워야 합니다.
        {
            Debug.Log($"현재 챕터 {StageManager.Instance.stageSaveData.currentChapterIndex}와 선택한 챕터 {chapterIndex}가 다릅니다. 해당 챕터를 정산부터 해주라는 경고 패널을 열 예정입니다.");
            return; // 현재 챕터와 선택한 챕터가 다를 때는 경고 패널을 열어야 합니다. 아직 구현이 안 되어 있으므로 일단 리턴합니다.
        }
        else // 현재 챕터와 선택한 챕터가 같을 때
        {
            // 코스트 지불 없이 바로 배틀 씬으로 이동할 수 있도록 처리합니다.
            Debug.Log($"진행 중이던 챕터 {chapterIndex}를 다시 선택했습니다. 코스트 계산 패널을 열지 않습니다.");
            SceneManagerEx.Instance.LoadScene("BattleScene"); // 배틀 씬으로 이동
            StageManager.Instance.RestoreStageState(); // 현재 스테이지 상태를 복원합니다.
            return;
        }
    }

    public void OnClickCostPanelBackButton()
    {
        selectChapterPanel.SetActive(true);
        costCalculationPanel.SetActive(false);
        //selectDungeonPanel.SetActive(false);
        scaresStaminaPanel.SetActive(false);
        //teamFormationPanel.SetActive(false);
    }

    private void OpenCostCalculationPanel(int chapterIndex)
    {
        UpdateSelectedChapterUI(chapterIndex); // 선택된 챕터의 UI 업데이트
        selectedChapterIndex = chapterIndex; // 선택된 챕터 인덱스 저장
        selectChapterPanel.SetActive(true);
        costCalculationPanel.SetActive(true);
        //selectDungeonPanel.SetActive(false);
        scaresStaminaPanel.SetActive(false);
        //teamFormationPanel.SetActive(false);
    }

    public void OnClickCostCalculationPanelStartButton()
    {
        int chapterIndex = selectedChapterIndex; // 선택된 챕터 인덱스 가져오기
        if (chapterIndex < 0 || chapterIndex >= chapterData.chapterIndex.Count)
        {
            Debug.LogError($"Invalid chapter index: {chapterIndex}. Cannot start battle.");
            return;
        }
        var selectedChapter = chapterData.chapterIndex[chapterIndex];
        if (userData.stamina < selectedChapter.ChapterCost)
        {
            Debug.LogError($"Not enough stamina to start chapter {chapterIndex}. Required: {selectedChapter.ChapterCost}, Available: {userData.stamina}");
            OpenScaresStaminaPanel();
            return;
        }
        else
        {
            selectedChapterIndex = -1; // 선택된 챕터 인덱스 초기화
            userData.stamina -= selectedChapter.ChapterCost; // 스테이지 시작 시 스태미너 차감
            StageManager.Instance.stageSaveData.currentChapterIndex = chapterIndex; // 현재 챕터 인덱스 설정
            StageManager.Instance.stageSaveData.chapterAndStageStates[chapterIndex].isUnLocked = true; // 챕터 잠금 해제 상태 설정
            StageManager.Instance.stageSaveData.chapterAndStageStates[chapterIndex].isCompleted = false; // 챕터 완료 상태 초기화
            SceneManagerEx.Instance.LoadScene("BattleScene"); // 배틀 씬으로 이동
            StageManager.Instance.RestoreStageState(); // 현재 스테이지 상태 복원
        }
    }

    private void OpenScaresStaminaPanel()
    {
        selectChapterPanel.SetActive(true);
        costCalculationPanel.SetActive(true);
        scaresStaminaPanel.SetActive(true);
    }

    public void OnClickDifficulty(bool DifficultyToggle)
    {
        isDifficulty = DifficultyToggle; // 토글 상태에 따라 난이도 설정
        DifficultyToggleRefresh();
    }


    private void DifficultyToggleRefresh()
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

    private void UpdateSelectedChapterUI(int chapterIndex)
    {
        if (chapterIndex < 0 || chapterIndex >= chapterData.chapterIndex.Count)
        {
            Debug.LogError($"Invalid chapter index: {chapterIndex}. Cannot update UI.");
            return;
        }
        var selectedChapter = chapterData.chapterIndex[chapterIndex];
        foreach (var text in selectedChapterNameText)
        {
            text.text = selectedChapter.ChapterName;
        }
        //selectedChapterDescriptionText.text = selectedChapter.Description; // 현재 기획에선 설명이 필요하지 않으므로 주석 처리
    }
}