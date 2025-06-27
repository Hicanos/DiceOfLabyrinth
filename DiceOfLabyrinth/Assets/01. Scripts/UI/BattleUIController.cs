using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour
{
    [Header("Stage Panel")]
    //[SerializeField] private Button backButton;
    [SerializeField] private Button nextButton;

    [Header("Battle Panel")]
    [SerializeField] private Button rollingButton;

    [Header("Victory Panel")]
    [SerializeField] private Button victoryNextButton;

    [Header("Defeat Panel")]
    [SerializeField] private Button defeatNextButton;

    [Header("Select Artifact Panel")]
    [SerializeField] private Button selectArtifact_01_Button;
    [SerializeField] private Button selectArtifact_02_Button;
    [SerializeField] private Button selectArtifact_03_Button;
    [SerializeField] private Button getArtifactButton;
    [SerializeField] private TMP_Text artifactDescriptionText;

    [Header("Select Event Panel")]
    [SerializeField] private Button event_01_Button;
    [SerializeField] private Button event_02_Button;

    [Header("Panels")]
    [SerializeField] private GameObject stagePanel;
    [SerializeField] private GameObject battlePanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private GameObject selectArtifactPanel;
    [SerializeField] private GameObject selectEventPanel;

    private void Start()
    {
        // 초기 설정
        stagePanel.SetActive(true);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectArtifactPanel.SetActive(false);
        selectEventPanel.SetActive(false);

        // Stage Panel 내부의 버튼 할당
        //backButton.onClick.AddListener(SceneManagerEx.Instance.OnBackClicked);
        nextButton.onClick.AddListener(OpenBattlePanel);

        // Battle Panel 내부의 버튼 할당
        rollingButton.onClick.AddListener(RollingDice);

        // Victory Panel 내부의 버튼 할당
        victoryNextButton.onClick.AddListener(BackToStage);

        // Defeat Panel 내부의 버튼 할당
        defeatNextButton.onClick.AddListener(BackToLobby);

        // Select Artifact Panel 내부의 버튼 할당
        selectArtifact_01_Button.onClick.AddListener(SelectArtifact);
        selectArtifact_02_Button.onClick.AddListener(SelectArtifact);
        selectArtifact_03_Button.onClick.AddListener(SelectArtifact);
        getArtifactButton.onClick.AddListener(GetArtifact);

        // Select Event 내부의 버튼 할당
        event_01_Button.onClick.AddListener(SelectEvent);
        event_02_Button.onClick.AddListener(SelectEvent);
    }

    private void OpenBattlePanel() // #1 nextButton 과 연결
    {
        stagePanel.SetActive(false);
        battlePanel.SetActive(true);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectArtifactPanel.SetActive(false);
        selectEventPanel.SetActive(false);
    }

    private void OpenSelectArtifactPanel() // 나중에 쓰도록 만들어 놓음
    {
        selectArtifactPanel.SetActive(true);
    }

    private void CloseSelectArtifactPanel() // 나중에 쓰도록 만들어 놓음
    {
        selectArtifactPanel.SetActive(false);
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

    private void SelectArtifact() // #5 selectArtifact_0@_Button 과 연결
    {
        // 아티팩트 선택 UI에서 아티팩트 버튼을 눌렀을 때

        // 선택한 아티팩트의 아웃라인만 켜지도록 (선택한건 키고 나머진 끄고)
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