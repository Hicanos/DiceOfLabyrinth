using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SelectAdventureUIController : MonoBehaviour
{
    public ChapterData chapterData;
    public MessagePopup messagePopup; // 체크 패널, 챕터가 잠겨있을 때 팝업을 띄우기 위해 사용합니다.

    private int selectedChapterIndex = -1; // 선택된 챕터 인덱스

    [Header("Panels")]
    [SerializeField] private GameObject selectChapterPanel;
    [SerializeField] private GameObject costCalculationPanel;
    //[SerializeField] private GameObject selectDungeonPanel; //배틀씬으로 패널을 옮겼으므로 주석 처리
    [SerializeField] private GameObject scarceStaminaPanel;
    //[SerializeField] private GameObject teamFormationPanel; // 배틀씬으로 패널을 옮겼으므로 주석 처리

    [Header("Difficulty Toggles")]
    [SerializeField] private GameObject NormalDifficultyUnselect;
    [SerializeField] private GameObject NormalDifficultySelect;
    [SerializeField] private GameObject HardDifficultyUnselect;
    [SerializeField] private GameObject HardDifficultySelect;

    [Header("SelectedChapter")]
    [SerializeField] private List<TMP_Text> selectedChapterNameText = new List<TMP_Text>(); // 선택된 챕터 이름 텍스트, 여러 개의 챕터 이름을 표시할 수 있도록 리스트로 변경
    //[SerializeField] private TMP_Text selectedChapterDescriptionText; // 선택된 챕터 설명 텍스트, 현재 기획에선 설명이 필요하지 않으므로 주석 처리

    [Header("Direct Complete Multiplier")]
    [SerializeField] private TMP_Text directCompleteMultiplierText; // 직접 완료 배수 텍스트
    private int directCompleteMultiplier; // 직접 완료 배수 값

    [Header("스태미나 부족 패널")]
    [SerializeField] private TMP_Text afterCostText; // 지불 후 스태미너를 보여주는 텍스트
    [SerializeField] private TMP_Text staminaRegenerationText; // 스태미너 재생 시간 텍스트



    public bool isDifficulty = false; // 챕터 난이도 선택 여부

    private void Start()
    {
        selectChapterPanel.SetActive(true);
        costCalculationPanel.SetActive(false);
        scarceStaminaPanel.SetActive(false);
        DifficultyToggleRefresh(); // 초기 난이도 토글 상태 설정

    }
    public void OnClickChapterButton(int normalChapterIndex)
    {
        int chapterIndex = normalChapterIndex; // 스테이지 인덱스는 Normal 챕터의 인덱스와 동일합니다.
        if (isDifficulty)
        {
            chapterIndex++; // 하드 챕터의 인덱스는 Normal 챕터 인덱스 + 1입니다.
        }
        selectedChapterIndex = chapterIndex; // 선택된 챕터 인덱스 설정
        Debug.Log($"Selected Chapter Index: {chapterIndex}");
        if (chapterIndex < -1 || chapterIndex >= chapterData.chapterIndex.Count)
        {
            messagePopup.Open("선택한 챕터가 유효하지 않습니다. 다시 시도해 주세요.");
            return;
        }
        else if (CharacterManager.Instance.OwnedCharacters.Count < 5) // 캐릭터가 5개 미만일 때
        {
            messagePopup.Open("캐릭터를 5개 이상 보유해야 챕터를 선택할 수 있습니다.");
            return;
        }
        else if (!StageManager.Instance.stageSaveData.chapterAndStageStates[chapterIndex].isUnLocked)
        {
            messagePopup.Open("이 챕터는 아직 잠겨 있습니다. 다른 챕터를 완료한 후 다시 시도해 주세요.");

            return;
        }
        else if (StageManager.Instance.stageSaveData.currentChapterIndex == -1) // -1은 진행중인 챕터가 없음을 의미합니다.
        {
            if (StageManager.Instance.stageSaveData.chapterAndStageStates[chapterIndex].isCompleted)
            {
                messagePopup.Open(
                $"해당 챕터()는 이미 완료되었습니다. 다시 시작하시겠습니까?.",
                 () => OpenCostCalculationPanel(chapterIndex), // 확인(Yes) 버튼 클릭 시, 입장 코스트를 묻는 패널을 엽니다.
                 () => messagePopup.Close() // 취소(No) 버튼 클릭 시
            );
                return;
            }
            OpenCostCalculationPanel(chapterIndex); // 입장 코스트를 묻는 패널을 엽니다.
        }
        else if (chapterIndex != StageManager.Instance.stageSaveData.currentChapterIndex) // 현재 챕터와 선택한 챕터가 다를 때엔 이전 챕터의 종료를 먼저 하라는 팝업을 띄워야 합니다.
        {
            messagePopup.Open($"진행 중인 챕터 {StageManager.Instance.stageSaveData.currentChapterIndex + 1}가 있습니다. 먼저 해당 챕터를 종료한 후 다시 시도해 주세요.");
            return;
        }
        else // 진행 중이던 챕터를 다시 선택한 경우
        {
            selectChapterPanel.SetActive(false);
            costCalculationPanel.SetActive(false);
            scarceStaminaPanel.SetActive(false);
            {
                // 코스트 지불 없이 바로 배틀 씬으로 이동할 수 있도록 처리합니다.
                Debug.Log($"진행 중이던 챕터를 다시 선택했습니다. 코스트 계산 패널을 열지 않습니다.");
                // 진행중이던 챕터를 다시 시작했다는 팝업을 띄우는 로직을 추가할 수 있습니다.
                messagePopup.Open($"진행 중이던 챕터를 다시 선택했습니다. 배틀 씬으로 이동하시겠습니까?",
                () => StageManager.Instance.RestoreStageState(), // 확인(Yes) 버튼 클릭 시
                () => messagePopup.Close() // 취소(No) 버튼 클릭 시
                );
                return;
            }
        }
    }

    public void OnClickCostPanelBackButton()
    {
        selectChapterPanel.SetActive(true);
        costCalculationPanel.SetActive(false);
        //selectDungeonPanel.SetActive(false);
        scarceStaminaPanel.SetActive(false);
        //teamFormationPanel.SetActive(false);
    }

    private void OpenCostCalculationPanel(int chapterIndex)
    {
        UpdateSelectedChapterUI(chapterIndex); // 선택된 챕터의 UI 업데이트
        selectChapterPanel.SetActive(true);
        costCalculationPanel.SetActive(true);
        scarceStaminaPanel.SetActive(false);
    }

    public void OnClickCostCalculationPanelStartButton()
    {
        int chapterIndex = selectedChapterIndex; // 선택된 챕터 인덱스 가져오기
        if (chapterIndex < 0 || chapterIndex >= chapterData.chapterIndex.Count)
        {
            messagePopup.Open("선택한 챕터가 유효하지 않습니다. 다시 시도해 주세요.");
            return;
        }
        var selectedChapter = chapterData.chapterIndex[chapterIndex];
        if (!UserDataManager.Instance.UseStamina(selectedChapter.ChapterCost)) // 코스트를 지불하지 못하면,
        {
            OpenScaresStaminaPanel();
            //return;
        }
        else
        {
            selectedChapterIndex = -1; // 선택된 챕터 인덱스 초기화
            StageManager.Instance.stageSaveData.currentChapterIndex = chapterIndex; // 현재 챕터 인덱스 설정
            StageManager.Instance.stageSaveData.ResetToDefault(chapterIndex); // 스테이지 상태 초기화
            Debug.Log($"Starting battle for chapter: {selectedChapter.ChapterName} (Index: {chapterIndex})");
            //SceneManagerEx.Instance.LoadScene("BattleScene"); // 배틀 씬으로 이동
            StageManager.Instance.RestoreStageState(); // 현재 스테이지 상태 복원
        }
    }

    private void OpenScaresStaminaPanel()
    {
        selectChapterPanel.SetActive(true);
        costCalculationPanel.SetActive(true);
        scarceStaminaPanel.SetActive(true);
        UpdateStaminaUI(); // 스태미나 UI 업데이트
    }

    public void OnClickRechargeStaminaButton() // 스태미나 충전 버튼
    {
        Debug.Log("Recharge Stamina Button Clicked");
        if (!UserDataManager.Instance.UseJewel(50)) // 스태미나 충전 비용이 50 쥬얼이므로, 쥬얼이 부족할 경우
        {
            messagePopup.Open("스태미나를 충전하려면 50 쥬얼이 필요합니다.");
        }
        else
        { 
            UserDataManager.Instance.AddStamina(50); // 스태미나 50 증가
        }
        OpenCostCalculationPanel(selectedChapterIndex); // 선택된 챕터 인덱스를 다시 설정
    }
    public void OnClickScaresStaminaPanelBackButton()
    {
        OpenCostCalculationPanel(selectedChapterIndex); // 선택된 챕터 인덱스를 다시 설정
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

    private void UpdateStaminaUI()
    {
        afterCostText.text = $"{UserDataManager.Instance.userdata.stamina}/50 -> {UserDataManager.Instance.userdata.stamina + 50}/50"; // 스태미나 충전 후의 상태를 보여줍니다.
        //int regenerationTime = // 스태미나 재생 시간 계산 로직은 아직 구현되지 않았습니다.
    }
}