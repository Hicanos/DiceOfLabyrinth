using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour
{
    public enum SelectedTeamFormation
    {
        FormationA,
        FormationB,
        FormationC,
        FormationD
    }

    public ChapterData chapterData;

    [Header("Select Item Panel")]
    [SerializeField] private TMP_Text itemTitleText;
    [SerializeField] private TMP_Text itemDescriptionText;

    [Header("Panels")]
    [SerializeField] private GameObject selectDungeonPanel;
    [SerializeField] private GameObject teamFormationPenel;
    [SerializeField] private GameObject stagePanel;
    [SerializeField] private GameObject battlePanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private GameObject selectItemPanel;
    [SerializeField] private GameObject selectEventPanel;
    [SerializeField] private GameObject[] itemChoiceIcon = new GameObject[3]; // 스태그마 선택 아이콘을 위한 배열

    [Header("Select Dungeon")]
    [SerializeField] private TMP_Text selectedChapterText; // 스테이지 선택 패널 제목
    [SerializeField] private Image chapterIcon; // 스테이지 선택 패널 아이콘
    [SerializeField] private TMP_Text chapterDescriptionText; // 스테이지 선택 패널 설명

    [Header("Team Formation")]
    [SerializeField] private SelectedTeamFormation selectedTeamFormation; // 선택된 팀 구성
    [SerializeField] private Image[] teamFormationIcons = new Image[4]; // 팀 구성 아이콘 배열
    [SerializeField] private List<Image> AcquiredCharacterIcons = new List<Image>(7); // 획득한 캐릭터 아이콘 리스트

    [Header("selected items")]
    public StagmaData[] stagmaChoices = new StagmaData[3]; // 스태그마 선택을 위한 배열
    public ArtifactData[] artifactChoices = new ArtifactData[3]; // 아티팩트 선택을 위한 배열

    private void Start()
    {
        // 초기 설정
        OpenStagePanel(StageManager.Instance.stageSaveData.currentPhaseIndex); // 현재 페이즈 인덱스에 해당하는 스테이지 패널을 엽니다.
        
        //StageManager.Instance.RestoreStageState();
    }

    public void OpenSelectDungeonPanel() // 스테이지 선택 패널을 여는 함수
    {
        selectDungeonPanel.SetActive(true);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(false);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectEventPanel.SetActive(false);
        // 선택된 스테이지 정보 업데이트
        selectedChapterText.text = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].ChapterName;
        chapterIcon.sprite = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].Image;
        chapterDescriptionText.text = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].Description;
    }

    public void OnClickDungeonButton(int stageIndex) // 스테이지 선택 버튼 클릭 시 호출되는 함수
    {
        if (StageManager.Instance.stageSaveData.chapterAndStageStates[StageManager.Instance.stageSaveData.currentChapterIndex].stageStates[stageIndex].isUnLocked) // 스테이지가 잠금 해제되어 있는지 확인
        {
            StageManager.Instance.stageSaveData.currentStageIndex = stageIndex; // 현재 스테이지 인덱스 설정
            StageManager.Instance.stageSaveData.currentPhaseIndex = 0; // 초기 페이즈 인덱스 설정
            OpenTeamFormationPanel(); // 팀 구성 패널 열기
        }
        else
        {
            Debug.Log($"Chapter {stageIndex} is locked. Please unlock it first.");
            // 스테이지가 잠겨있을 때 잠김 상태를 알려주는 UI를 표시하는 로직을 추가할 수 있습니다.
        }
    }
    public void OpenTeamFormationPanel()
    {
        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(true);
        stagePanel.SetActive(false);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectEventPanel.SetActive(false);
        RefreshTeamFormationButton(); // 팀 구성 버튼 상태 갱신
    }

    public void OnClickTeamFormationButton(int formationIndex) // 팀 구성 버튼 클릭 시 호출되는 함수
    {
        selectedTeamFormation = (SelectedTeamFormation)formationIndex; // 선택된 팀 구성 설정
        StageManager.Instance.stageSaveData.currentFormationType = StageSaveData.CurrentFormationType.FormationA + formationIndex; // 현재 팀 구성 타입 설정
        RefreshTeamFormationButton();
    }

    private void RefreshTeamFormationButton() // 팀 구성 버튼 상태를 갱신하는 함수
    {
        for (int i = 0; i < teamFormationIcons.Length; i++)
        {
            teamFormationIcons[i].color = (SelectedTeamFormation)i == selectedTeamFormation ? Color.yellow : Color.white; // 선택된 팀 구성은 노란색, 나머지는 흰색으로 표시
        }
    }
    public void OpenStagePanel(int phaseIndex) // 스테이지 패널을 여는 함수
    {
        stagePanel.SetActive(true);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectEventPanel.SetActive(false);
    }

    private void OpenBattlePanel(int phaseIndex) // 스테이지 선택 후 배틀 패널을 여는 함수
    {
        stagePanel.SetActive(false);
        battlePanel.SetActive(true);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectEventPanel.SetActive(false);
    }

    public void OpenSelectStagmaPanel(string phaseState) // "Standby", "NormalReward", "EliteArtifactReward", "EliteStagmaReward" 등과 연결
                                                         // 현재 기획에선 Stanby 와 EliteReward 페이즈에서 스태그마 선택을 할 수 있도록 되어있음
    {
        // phaseState에 따라 아이템 선택 후 다음 페이즈로 넘어가는 방향을 결정
        //예외 상태 스트링 값을 처리하는 스위치
        switch (phaseState)
        {
            case "Standby":
            case "EliteStagmaReward":
                // 스태그마 선택 UI를 열어야 하는 경우만 break(아래 코드 실행)
                break;
            case "NormalReward":
                Debug.Log("노멀 리워드에는 각인이 없습니다.");
                return;
            case "EliteArtifactReward":
                Debug.Log("아티팩트 리워드가 아니라 각인 리워드입니다.");
                return;
            case "BossReward":
                Debug.Log("보스 리워드에는 각인이 없습니다.");
                return;
            default:
                Debug.LogError($"Unknown phase state: {phaseState}");
                return;
        }
        StageManager.Instance.stageSaveData.currentPhaseState = phaseState; // 현재 페이즈 상태 저장
        List<StagmaData> availableStagmas = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].StagmaList; // 현재 스테이지의 스태그마 목록을 가져옴
        itemTitleText.text = "각인 선택"; // 스태그마 선택 UI 제목 설정
        itemDescriptionText.text = ""; // 초기화
        List<StagmaData> selectedStagmas = new List<StagmaData>();
        while (selectedStagmas.Count < 3)
        {
            int rand = Random.Range(0, availableStagmas.Count);
            StagmaData candidate = availableStagmas[rand];
            if (!selectedStagmas.Contains(candidate))
                selectedStagmas.Add(candidate);
        }

        // 배열에 저장 및 UI 반영
        for (int i = 0; i < 3; i++)
        {
            stagmaChoices[i] = selectedStagmas[i];
            var iconImage = itemChoiceIcon[i].GetComponent<Image>();
            iconImage.sprite = stagmaChoices[i].icon;
        }

        stagePanel.SetActive(true);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(true);
        selectEventPanel.SetActive(false);
    }

    public void OpenSelectArtifactPanel(string phaseState) // "NormalReward", "EliteArtifactReward", "BossReward" 와 연결
                                                           // 현재 기획에선 "BossReward" 페이즈에서 아티팩트 선택을 할 수 있도록 되어있음
    {
        // phaseState에 따라 아티팩트 선택 후 다음 페이즈로 넘어가는 방향을 결정
        // 예외 상태 스트링 값을 처리하는 스위치
        switch (phaseState)
        {
            case "BossReward":
            case "NormalReward":
            case "EliteArtifactReward":
                // 아티팩트 선택 UI를 열어야 하는 경우만 break(아래 코드 실행)
                break;
            case "EliteStagmaReward":
                Debug.Log("엘리트 스태그마 리워드에는 아티팩트 선택이 없습니다.");
                return;
            case "Standby":
                Debug.Log("스탠바이 페이즈에는 아티팩트 선택이 없습니다.");
                return;
            default:
                Debug.LogError($"Unknown phase state: {phaseState}");
                return;
        }
        StageManager.Instance.stageSaveData.currentPhaseState = phaseState; // 현재 페이즈 상태 저장
        itemTitleText.text = "아티팩트 선택"; // 아티팩트 선택 UI 제목 설정
        itemDescriptionText.text = ""; // 초기화
            
        List<ArtifactData> availableArtifacts = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].ArtifactList; // 현재 스테이지의 아티팩트 목록을 가져옴
        List<ArtifactData> selectedArtifacts = new List<ArtifactData>();
        while (selectedArtifacts.Count < 3)
        {
            int rand = Random.Range(0, availableArtifacts.Count);
            ArtifactData candidate = availableArtifacts[rand];
            if (!selectedArtifacts.Contains(candidate))
                selectedArtifacts.Add(candidate);
        }

        // 배열에 저장 및 UI 반영
        for (int i = 0; i < 3; i++)
        {
            artifactChoices[i] = selectedArtifacts[i];
            var iconImage = itemChoiceIcon[i].GetComponent<Image>();
            iconImage.sprite = stagmaChoices[i].icon;
        }
        stagePanel.SetActive(true);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(true);
        selectEventPanel.SetActive(false);

    }

    private void CloseSelectItemPanel() // 아티팩트 패널과 스태그마 패널 둘 다 다루니 아이템 패널이라고 함
    {
        string phaseState = StageManager.Instance.stageSaveData.currentPhaseState; // 현재 페이즈 상태를 가져옴
        selectItemPanel.SetActive(false);
        StageManager.Instance.stageSaveData.currentPhaseState = ""; // 선택지 페이즈 상태 초기화
        switch (phaseState)
        {
            case "Standby":
                // Standby 페이즈 이후에는 다른 선택지 없이 스테이지1을 시작할 예정
                OpenBattlePanel(StageManager.Instance.stageSaveData.currentPhaseIndex); // 배틀 패널을 열도록 함
                break;
            case "NormalReward":
                //다음 전투페이즈로 넘어가는 로직을 추가할 수 있음
                break;
            case "EliteArtifactReward":
                OpenSelectStagmaPanel(phaseState); // 엘리트 아티팩트 리워드 페이즈에서는 스태그마 선택 패널을 열도록 함
                break;
            case "EliteStagmaReward":
                // 다음 전투페이즈로 넘어가는 로직을 추가할 수 있음
                break;
            case "BossReward":
            default:
                Debug.LogError($"Unknown phase state: {phaseState}");
                break;
        }
    }


    private void OpenSelectEventPanel() // 나중에 쓰도록 만들어 놓음
    {
        selectEventPanel.SetActive(true);
    }

    private void CloseSelectEventPanel() // 나중에 쓰도록 만들어 놓음
    {
        selectEventPanel.SetActive(false);
    }
}