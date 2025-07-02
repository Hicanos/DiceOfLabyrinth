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
    public CheckPanel checkPanel; // 체크 패널, 스테이지가 잠겨있을 때 팝업을 띄우기 위해 사용합니다.

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
    [SerializeField] private GameObject[] itemChoiceIcon = new GameObject[3]; // 아이템 선택 아이콘을 위한 배열

    [Header("Select Dungeon")]
    [SerializeField] private TMP_Text selectedChapterText; // 스테이지 선택 패널 제목
    [SerializeField] private Image chapterIcon; // 스테이지 선택 패널 아이콘
    [SerializeField] private TMP_Text chapterDescriptionText; // 스테이지 선택 패널 설명

    [Header("Team Formation")]
    [SerializeField] private SelectedTeamFormation selectedTeamFormation; // 선택된 팀 구성
    [SerializeField] private Image[] teamFormationIcons = new Image[4]; // 팀 구성 아이콘 배열
    [SerializeField] GameObject[] characterButtons = new GameObject[7];
    
    [Header("selected items")]
    public StagmaData[] stagmaChoices = new StagmaData[3]; // 스태그마 선택을 위한 배열
    public ArtifactData[] artifactChoices = new ArtifactData[3]; // 아티팩트 선택을 위한 배열

    private void Start()
    {
        // 초기 설정
        //OpenStagePanel(StageManager.Instance.stageSaveData.currentPhaseIndex); // 현재 페이즈 인덱스에 해당하는 스테이지 패널을 엽니다.
        
        //StageManager.Instance.RestoreStageState();
    }

    public void OpenSelectDungeonPanel() // 스테이지 선택 패널을 여는 함수
    {
        StageManager.Instance.stageSaveData.currentPhaseState = ""; // 현재 페이즈 상태를 ""으로 설정, 던전 선택이 최초이므로
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
            checkPanel.Open("이 스테이지는 아직 잠겨 있습니다. 다른 스테이지를 완료한 후 다시 시도해 주세요."); // 스테이지가 잠겨있을 때 경고 메시지 표시
        }
    }
    public void OpenTeamFormationPanel()
    {
        RefreshTeamFormationButton(); // 팀 구성 버튼 상태 갱신
        RefreshSpawnedCharacters(); // 현재 스폰된 캐릭터들을 갱신
        //Debug.Log($"[TeamFormation] AcquiredCharacters Count: {CharacterManager.Instance.AcquiredCharacters.Count}");
        StageManager.Instance.stageSaveData.currentPhaseState = "TeamSelect"; // 팀 선택 패널
        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(true);
        stagePanel.SetActive(false);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectEventPanel.SetActive(false);
        // characterButtons의 개수를 보유 캐릭터 수 만큼으로 설정하는 로직은 나중에 구현할 예정 현재는 7개로 사용

        foreach(var button in characterButtons)
        {
            for (int i = button.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(button.transform.GetChild(i).gameObject);
            }
        }
        int acquiredCount = CharacterManager.Instance.AcquiredCharacters.Count;
        int buttonCount = characterButtons.Length;
        int loopCount = Mathf.Min(acquiredCount, buttonCount);
        for (int i = 0; i < loopCount; i++)
        {
            var characterSO = CharacterManager.Instance.AcquiredCharacters[i];
            if (characterSO == null)
            {
                Debug.LogWarning($"[TeamFormation] CharacterSO at index {i} is null.");
            }

            if (characterSO.charLobbyPrefab == null)
            {
                Debug.LogWarning($"[TeamFormation] charLobbyPrefab is null for charID: {characterSO.charID} (index {i})");
            }

            var prefab = Instantiate(characterSO.charLobbyPrefab, characterButtons[i].transform);
            prefab.transform.localPosition = Vector3.zero;
            prefab.transform.localRotation = Quaternion.identity;
            prefab.transform.localScale = Vector3.one;
        }

    }

    public void OnClickCharacterButton(int characterIndex) // 캐릭터 버튼 클릭 시 호출되는 함수
    {
        if (characterIndex < 0 || characterIndex >= CharacterManager.Instance.AcquiredCharacters.Count)
        {
            Debug.Log($"Invalid character index: {characterIndex}. Total acquired characters: {CharacterManager.Instance.AcquiredCharacters.Count}");
            checkPanel.Open("잘못된 캐릭터 인덱스입니다. 다시 시도해 주세요.");
            return;
        }
        var selectedCharacter = CharacterManager.Instance.AcquiredCharacters[characterIndex].charBattlePrefab.GetComponent<BattleCharacter>(); ; // 선택된 캐릭터 SO
        //조건문으로 엔트리캐릭터 배열 안에 선택된 캐릭터가 이미 들어있는지 확인
        if (StageManager.Instance.stageSaveData.entryCharacters.Contains(selectedCharacter)) // 이미 선택된 캐릭터인지 확인해서 제거
        {
            StageManager.Instance.stageSaveData.entryCharacters.Remove(selectedCharacter); // 선택된 캐릭터 제거
        }
        else if (StageManager.Instance.stageSaveData.entryCharacters.Count < 5) // 선택된 캐릭터가 아직 추가되지 않은 경우, 최대 5명까지 추가 가능
        {
            for (int i = 0; i < StageManager.Instance.stageSaveData.entryCharacters.Count; i++) // 선택된 캐릭터를 스테이지 저장 데이터에 설정
            {
                if (StageManager.Instance.stageSaveData.entryCharacters[i] == null) // 빈 슬롯에 캐릭터를 설정
                {
                    StageManager.Instance.stageSaveData.entryCharacters[i] = selectedCharacter;
                    break;
                }
            }
        }

        // 선택된 캐릭터를 월드에 스폰하는 리프레시 함수 호출
        RefreshSpawnedCharacters(); // 스폰된 캐릭터 갱신
    }

    public void OnClickTeamFormationButton(int formationIndex) // 팀 구성 버튼 클릭 시 호출되는 함수
    {
        selectedTeamFormation = (SelectedTeamFormation)formationIndex; // 선택된 팀 구성 설정
        StageManager.Instance.stageSaveData.currentFormationType = StageSaveData.CurrentFormationType.FormationA + formationIndex; // 현재 팀 구성 타입 설정
        RefreshTeamFormationButton();
    }

    private void RefreshSpawnedCharacters() // 스폰된 캐릭터를 갱신하는 함수, 구조 고쳐야 함
    {
        // 기존에 월드에 스폰된 캐릭터들을 제거하는 로직
        var characters = FindObjectsByType<BattleCharacter>(FindObjectsInactive.Include, FindObjectsSortMode.None); // 현재 씬에 있는 모든 BattleCharacter를 찾아서
        foreach (var character in characters)
        {
            Destroy(character.gameObject); // 제거
        }
        for (int i = 0; i < StageManager.Instance.stageSaveData.entryCharacters.Count; i++)
        {
            if (StageManager.Instance.stageSaveData.entryCharacters[i] != null)
            {
                // 캐릭터를 월드에 스폰하는 로직, 스테이지 데이터에 스폰 포지션이 있으며 스폰 포지션과 같은 인덱스의 엔트리 캐릭터를 스폰
                GameObject battleCharacterObject = StageManager.Instance.stageSaveData.entryCharacters[i].CharacterData.charBattlePrefab;
                Vector3 spawnPoint = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.PlayerFormations[(int)StageManager.Instance.stageSaveData.currentFormationType].PlayerPositions[i].Position;
                GameObject spawnedCharacter = Instantiate(battleCharacterObject, spawnPoint, Quaternion.identity); // 스폰 포인트에 캐릭터 스폰
            }
        }
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
        StageManager.Instance.stageSaveData.currentPhaseState = "Standby"; // 초기 페이즈 상태 설정
        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(true);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectEventPanel.SetActive(false);
    }

    private void OpenBattlePanel(int phaseIndex) // 스테이지 선택 후 배틀 패널을 여는 함수
    {
        StageManager.Instance.stageSaveData.currentPhaseState = "Battle"; // 현재 페이즈 상태를 "Battle"로 설정
        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(false);
        battlePanel.SetActive(true);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectEventPanel.SetActive(false);
    }

    public void OpenSelectStagmaPanel(string phaseState) // "Standby", "NormalReward", "EliteArtifactReward", "EliteStagmaReward" 등과 연결
    {
        StageManager.Instance.stageSaveData.currentPhaseState = phaseState; // 현재 페이즈 상태를 설정
        stagmaChoices = new StagmaData[3]; // 스태그마 선택 배열 초기화
        artifactChoices = new ArtifactData[3]; // 아티팩트 선택 배열 초기화
        //예외 상태 스트링 값을 처리하는 스위치
        switch (phaseState)
        {
            case "StartReward":
            case "EliteStagmaReward":
                // 스태그마 선택 UI를 열어야 하는 경우만 break(아래 코드 실행)
                break;
            default:
                Debug.LogError($"잘못된 phase state: {phaseState}");
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

        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(true);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(true);
        selectEventPanel.SetActive(false);
    }

    public void OpenSelectArtifactPanel(string phaseState) // "NormalReward", "EliteArtifactReward", "BossReward" 와 연결
    {
        StageManager.Instance.stageSaveData.currentPhaseState = phaseState; // 현재 페이즈 상태를 설정

        stagmaChoices = new StagmaData[3]; // 스태그마 선택 배열 초기화
        artifactChoices = new ArtifactData[3]; // 아티팩트 선택 배열 초기화
        // 예외 상태 스트링 값을 처리하는 스위치
        switch (phaseState)
        {
            case "BossReward":
            case "NormalReward":
            case "EliteArtifactReward":
                // 아티팩트 선택 UI를 열어야 하는 경우만 break(아래 코드 실행)
                break;
            default:
                Debug.LogError($"잘못된 phase state: {phaseState}");
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
        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(true);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(true);
        selectEventPanel.SetActive(false);
    }

    public void OnClickSelectItem(int selectIndex) // 아티팩트 패널과 스태그마 패널 둘 다 다루니 아이템 패널이라고 함
    {
        string phaseState = StageManager.Instance.stageSaveData.currentPhaseState; // 현재 페이즈 상태를 가져옴
        selectItemPanel.SetActive(false);
        StageManager.Instance.stageSaveData.currentPhaseState = ""; // 선택지 페이즈 상태 초기화
        StageManager.Instance.stageSaveData.stagmas.Add(stagmaChoices[selectIndex]); // 선택한 스태그마를 스테이지 저장 데이터에 추가
        StageManager.Instance.stageSaveData.artifacts.Add(artifactChoices[selectIndex]); // 선택한 스태그마를 스테이지 저장 데이터에 추가
        stagmaChoices = new StagmaData[3]; // 스태그마 선택 배열 초기화
        artifactChoices = new ArtifactData[3]; // 아티팩트 선택 배열 초기화
        switch (phaseState)
        {
            case "StartReward":
                // StartReward 페이즈 이후에는 다른 선택지 없이 스테이지1을 시작할 예정
                OpenStagePanel(StageManager.Instance.stageSaveData.currentPhaseIndex); // 스테이지 패널을 열도록 함
                break;
            case "NormalReward":
                OpenStagePanel(StageManager.Instance.stageSaveData.currentPhaseIndex); // 노멀 리워드 페이즈에서는 배틀 패널을 열도록 함
                break;
            case "EliteArtifactReward":
                OpenSelectStagmaPanel(phaseState); // 엘리트 아티팩트 리워드 페이즈에서는 스태그마 선택 패널을 열도록 함
                break;
            case "EliteStagmaReward":
                OpenStagePanel(StageManager.Instance.stageSaveData.currentPhaseIndex); // 엘리트 스태그마 리워드 페이즈에서는 배틀 패널을 열도록 함
                break;
            case "BossReward":
                OpenSelectEquipmedArtifactPanel(); // 보스 리워드 페이즈에서는 아티팩트 선택 패널을 열도록 함
                break;
            default:
                Debug.LogError($"잘못된 phase state: {phaseState}");
                break;
        }
    }

    public void OpenSelectEventPanel() // 선택지 이벤트 패널을 여는 함수
    {
        StageManager.Instance.stageSaveData.currentPhaseState = "SelectChoice"; // 현재 페이즈 상태를 "SelectEvent"로 설정
        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(false);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectEventPanel.SetActive(true); // 선택지 이벤트 패널 활성화
    }

    public void OnClickSelectEvent(int phaseStage) // 나중에 쓰도록 만들어 놓음
    {
        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(true);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectEventPanel.SetActive(false);
        // selectEquipmedArtifactPanel.SetActive(false); // 귀속 아티팩트 선택 패널 비활성화
    }

    public void OpenSelectEquipmedArtifactPanel() // 아티팩트 선택 패널을 여는 함수
    {
        StageManager.Instance.stageSaveData.currentPhaseState = "EquipmedArtifact"; // 현재 페이즈 상태를 "EquipmentArtifact"로 설정
        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(false);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectEventPanel.SetActive(false);
        // selectEquipmedArtifactPanel.SetActive(true); // 아티팩트 선택 패널 활성화
    }
}