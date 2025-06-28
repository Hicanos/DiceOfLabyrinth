using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour
{

    public ChapterData chapterData;

    private string currentPhaseState; // 현재 페이즈 상태를 저장하는 변수 (예: "Standby", "NormalReward", "EliteReward" 등)

    [Header("Select Item Panel")]

    [SerializeField] private TMP_Text itemTitleText;
    [SerializeField] private TMP_Text itemDescriptionText;

    [Header("Panels")]
    [SerializeField] private GameObject stagePanel;
    [SerializeField] private GameObject battlePanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private GameObject selectItemPanel;
    [SerializeField] private GameObject selectEventPanel;
    [SerializeField] private GameObject[] itemChoiceIcon = new GameObject[3]; // 스태그마 선택 아이콘을 위한 배열
    
    public StagmaData[] stagmaChoices = new StagmaData[3]; // 스태그마 선택을 위한 배열
    public ArtifactData[] artifactChoices = new ArtifactData[3]; // 아티팩트 선택을 위한 배열

    private void Start()
    {
        // 초기 설정
        stagePanel.SetActive(true);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectEventPanel.SetActive(false);
        currentPhaseState = ""; // 초기 페이즈 상태를 빈 문자열로 설정

        StageManager.Instance.StandbyPhase();
    }

    private void OpenBattlePanel() // #1 nextButton 과 연결
    {
        stagePanel.SetActive(false);
        battlePanel.SetActive(true);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectEventPanel.SetActive(false);
    }

    public void OpenSelectStagmaPanel(string phaseState) // "Standby", "NormalReward", "EliteReward" 등과 연결
                                                         // 현재 기획에선 Stanby 와 EliteReward 페이즈에서 스태그마 선택을 할 수 있도록 되어있음
    {
        // phaseState에 따라 아이템 선택 후 다음 페이즈로 넘어가는 방향을 결정
        currentPhaseState = phaseState; // 현재 페이즈 상태 저장
        List<StagmaData> availableStagmas = chapterData.chapterIndex[StageManager.Instance.currentChapterIndex].stageData.stageIndex[StageManager.Instance.currentStageIndex].StagmaList; // 현재 스테이지의 스태그마 목록을 가져옴
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

        selectItemPanel.SetActive(true);
    }

    private void CloseSelectStagmaPanel() // 나중에 쓰도록 만들어 놓음
    {
        selectItemPanel.SetActive(false);
    }

    public void OpenSelectArtifactPanel(string phaseState) // "Standby", "NormalReward", "EliteReward" 등과 연결
                                                           // 현재 기획에선 NormalReward 와 EliteReward 페이즈에서 아티팩트 선택을 할 수 있도록 되어있음
    {
        // phaseState에 따라 아티팩트 선택 후 다음 페이즈로 넘어가는 방향을 결정
        currentPhaseState = phaseState; // 현재 페이즈 상태 저장
        itemTitleText.text = "아티팩트 선택"; // 아티팩트 선택 UI 제목 설정
        itemDescriptionText.text = ""; // 초기화
            
        List<ArtifactData> availableArtifacts = chapterData.chapterIndex[StageManager.Instance.currentChapterIndex].stageData.stageIndex[StageManager.Instance.currentStageIndex].ArtifactList; // 현재 스테이지의 아티팩트 목록을 가져옴
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
        selectItemPanel.SetActive(true);

    }

    private void OpenSelectEventPanel() // 나중에 쓰도록 만들어 놓음
    {
        selectEventPanel.SetActive(true);
    }

    private void CloseSelectEventPanel() // 나중에 쓰도록 만들어 놓음
    {
        selectEventPanel.SetActive(false);
    }

    private void RollingDice() // #2 rollingButton 과 연결
    {
        // 주사위를 굴렸을 때
    }

    private void BackToStage() // #3 victoryNextButton 과 연결
    {
        // 승리 UI에서 다음 버튼을 눌렀을 때
        // EX.승리한 데이터를 저장 후 스테이지UI에 정보를 넘김 (보상 등)

        // UI 다 닫고 스테이지UI만 뜨게
    }

    private void BackToLobby() // #4 defeatNextButton 과 연결
    {
        // 패배 UI에서 다음 버튼을 눌렀을 때
        // EX.패배한 데이터를 저장 후 스테이지UI에 정보를 넘김 (보상 등)

        // 로비 씬으로 이동
    }

    private void SelectItem() // #5 selectItem_0@_Button 과 연결
    {
        // 아이템 선택 UI에서 아이템 버튼을 눌렀을 때

        // 선택한 아이템의 아웃라인만 켜지도록 (선택한건 키고 나머진 끄고)
        // 그리고 선택한 아티팩의 설명이 나옴 (artifactDescriptionText 이용)
    }

    private void GetArtifact() // #6 getArtifactButton 과 연결
    {
        // 아티팩트 선택 UI에서 받기 버튼을 눌렀을 때

        // 선택한 아티팩트를 획득함

        // 아티팩트 선택 UI 닫기
    }

    private void SelectEvent() // #7 event_0@_Button 과 연결
    {
        // 이벤트 선택 UI에서 이벤트 버튼을 눌렀을 때

        // 선택한 이벤트를 진행함 (진행 버튼에 해당 이벤트의 정보를 넘김)
        // 그렇게 되면 진행을 눌렀을 때 그 이벤트에 맞게 무언가 실행 되도록 (배틀 UI로 넘어갈수도 있고 이벤트 UI로 넘어갈수도 있고)
        // (이벤트 UI는 아직 전달받은 게 없어서 안만듬)
    }
}