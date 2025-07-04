using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
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
    public MessagePopup messagePopup; // 체크 패널, 스테이지가 잠겨있을 때 팝업을 띄우기 위해 사용합니다.

    [Header("Select Item Panel")]
    [SerializeField] private TMP_Text itemTitleText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private int selectIndex = 0; // 선택된 아이템 인덱스, 스태그마와 아티팩트 선택을 위한 인덱스
    [SerializeField] private StagmaData[] stagmaChoices = new StagmaData[3];
    [SerializeField] private ArtifactData[] artifactChoices = new ArtifactData[3];
    [SerializeField] private StagmaData selectedStagma;
    [SerializeField] private ArtifactData selectedArtifact;

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
    [SerializeField] Image[] characterButtons = new Image[7];
    
    [Header("Select Choice Panel")]
    [SerializeField] private TMP_Text[] selectChoiceText = new TMP_Text[2]; // 선택지 이벤트 패널 제목
    [SerializeField] private Image[] selectChoiceIcon = new Image[2]; // 선택지 이벤트 패널 아이콘

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
            StageManager.Instance.stageSaveData.currentPhaseIndex = -1; // 초기 페이즈 인덱스 설정
            OpenTeamFormationPanel(); // 팀 구성 패널 열기
        }
        else
        {
            messagePopup.Open("이 스테이지는 아직 잠겨 있습니다. 다른 스테이지를 완료한 후 다시 시도해 주세요."); // 스테이지가 잠겨있을 때 경고 메시지 표시
        }
    }
    public void OpenTeamFormationPanel()
    {
        RefreshTeamFormationButton(); // 팀 구성 버튼 상태 갱신
        RefreshSpawnedCharacters((int)StageManager.Instance.stageSaveData.currentFormationType); // 현재 스폰된 캐릭터들을 갱신
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

        int ownedCount = CharacterManager.Instance.OwnedCharacters.Count;
        int buttonCount = characterButtons.Length;
        int loopCount = Mathf.Min(ownedCount, buttonCount);
        for (int i = 0; i < loopCount; i++)
        {
            var characterSO = CharacterManager.Instance.OwnedCharacters[i].CharacterData;
            if (characterSO == null)
            {
                Debug.LogWarning($"[TeamFormation] CharacterSO at index {i} is null.");
            }

            if (characterSO.icon == null)
            {
                messagePopup.Open($"캐릭터 아이콘이 설정되지 않았습니다. 캐릭터 ID: {characterSO.nameKr}");
            }
            characterButtons[i].sprite = characterSO.icon; // 캐릭터 아이콘 설정
        }

    }

    public void OnClickCharacterButton(int characterIndex) // 캐릭터 버튼 클릭 시 호출되는 함수
    {
        if (CharacterManager.Instance == null)
        {
            messagePopup.Open("CharacterManager 인스턴스가 존재하지 않습니다.");
            return;
        }
        if (CharacterManager.Instance.OwnedCharacters == null)
        {
            messagePopup.Open("OwnedCharacters 리스트가 초기화되지 않았습니다.");
            return;
        }
        if (characterIndex < 0 || characterIndex >= CharacterManager.Instance.OwnedCharacters.Count)
        {
            messagePopup.Open("잘못된 캐릭터 인덱스입니다. 다시 시도해 주세요.");
            return;
        }
        if (CharacterManager.Instance.OwnedCharacters[characterIndex] == null)
        {
            messagePopup.Open("해당 인덱스의 캐릭터가 존재하지 않습니다.");
            return;
        }
        if (CharacterManager.Instance.OwnedCharacters[characterIndex].CharacterData == null)
        {
            messagePopup.Open("해당 캐릭터의 데이터가 존재하지 않습니다.");
            return;
        }
        if (StageManager.Instance == null)
        {
            messagePopup.Open("StageManager 인스턴스가 존재하지 않습니다.");
            return;
        }
        if (StageManager.Instance.stageSaveData == null)
        {
            messagePopup.Open("stageSaveData가 초기화되지 않았습니다.");
            return;
        }
        if (StageManager.Instance.stageSaveData.entryCharacters == null)
        {
            messagePopup.Open("entryCharacters 리스트가 초기화되지 않았습니다.");
            return;
        }
        // 리스트 크기를 5로 고정
        while (StageManager.Instance.stageSaveData.entryCharacters.Count < 5)
            StageManager.Instance.stageSaveData.entryCharacters.Add(null);

        while (StageManager.Instance.stageSaveData.entryCharacters.Count > 5)
            StageManager.Instance.stageSaveData.entryCharacters.RemoveAt(StageManager.Instance.stageSaveData.entryCharacters.Count - 1);

        CharacterSO selectedCharacter = CharacterManager.Instance.OwnedCharacters[characterIndex].CharacterData;

        // 이미 선택된 캐릭터면 해제(토글)
        bool wasSelected = false;
        for (int i = 0; i < 5; i++)
        {
            if (StageManager.Instance.stageSaveData.entryCharacters[i] == selectedCharacter)
            {
                StageManager.Instance.stageSaveData.entryCharacters[i] = null;
                wasSelected = true;
            }
        }

        // 선택 해제였다면 추가하지 않음
        if (!wasSelected)
        {
            // 첫 번째 null 슬롯에 할당
            for (int i = 0; i < 5; i++)
            {
                if (StageManager.Instance.stageSaveData.entryCharacters[i] == null)
                {
                    StageManager.Instance.stageSaveData.entryCharacters[i] = selectedCharacter;
                    if (StageManager.Instance.stageSaveData.leaderCharacter == null) // 리더 캐릭터가 설정되지 않았다면 첫 번째 선택된 캐릭터를 리더로 설정
                    {
                        StageManager.Instance.stageSaveData.leaderCharacter = selectedCharacter;
                        messagePopup.Open($"[{selectedCharacter.nameKr}] 캐릭터가 리더로 설정되었습니다.");
                    }
                    else
                    {
                        messagePopup.Open($"[{selectedCharacter.nameKr}] 캐릭터가 팀에 추가되었습니다.");
                    }
                    break;
                }
            }
        }
        // 선택된 캐릭터를 월드에 스폰하는 리프레시 함수 호출
        RefreshSpawnedCharacters((int)StageManager.Instance.stageSaveData.currentFormationType); // 스폰된 캐릭터 갱신
    }

    public void OnClickTeamFormationButton(int formationIndex) // 팀 구성 버튼 클릭 시 호출되는 함수
    {
        selectedTeamFormation = (SelectedTeamFormation)formationIndex; // 선택된 팀 구성 설정
        StageManager.Instance.stageSaveData.currentFormationType = StageSaveData.CurrentFormationType.FormationA + formationIndex; // 현재 팀 구성 타입 설정
        RefreshTeamFormationButton();
    }


    public void OnClickSelectLeaderButton()
    {
        var entry = StageManager.Instance.stageSaveData.entryCharacters;
        if (entry.All(c => c == null))
        {
            messagePopup.Open("팀에 캐릭터가 없습니다. 팀을 구성해 주세요.");
            return;
        }

        // 리더가 null이면 0번 인덱스(첫 번째 null이 아닌 캐릭터)를 리더로 지정
        if (StageManager.Instance.stageSaveData.leaderCharacter == null)
        {
            for (int i = 0; i < entry.Count; i++)
            {
                if (entry[i] != null)
                {
                    StageManager.Instance.stageSaveData.leaderCharacter = entry[i];
                    messagePopup.Open($"[{entry[i].nameKr}] 캐릭터가 리더로 설정되었습니다.");
                    return;
                }
            }
        }

        // 리더가 entryCharacters에 있으면 다음 인덱스의 캐릭터(비어있지 않은 캐릭터)를 리더로, 마지막 인덱스면 0번부터 다시 탐색
        int currentLeaderIndex = entry.FindIndex(c => c == StageManager.Instance.stageSaveData.leaderCharacter);
        int count = entry.Count;
        for (int offset = 1; offset <= count; offset++)
        {
            int nextIndex = (currentLeaderIndex + offset) % count;
            if (entry[nextIndex] != null)
            {
                StageManager.Instance.stageSaveData.leaderCharacter = entry[nextIndex];
                messagePopup.Open($"[{entry[nextIndex].nameKr}] 캐릭터가 리더로 설정되었습니다.");
                break;
            }
        }
    }

    public void OnClickExploreButton()
    {
        // 배틀 캐릭터 크기를 5로 고정
        while (StageManager.Instance.stageSaveData.battleCharacters.Count < 5)
            StageManager.Instance.stageSaveData.battleCharacters.Add(null);
        while (StageManager.Instance.stageSaveData.battleCharacters.Count > 5)
            StageManager.Instance.stageSaveData.battleCharacters.RemoveAt(StageManager.Instance.stageSaveData.battleCharacters.Count - 1);

        // 팀에 5명 모두 채워져 있는지 확인
        if (StageManager.Instance.stageSaveData.entryCharacters.Count(c => c != null) == 5)
        {
            for (int i = 0; i < 5; i++)
            {
                var so = StageManager.Instance.stageSaveData.entryCharacters[i];
                StageManager.Instance.stageSaveData.battleCharacters[i] = CharacterManager.Instance.RegisterBattleCharacterData(so.charID);
            }
            // 엔트리 캐릭터는 비우고 리프레시
            for (int i = 0; i < StageManager.Instance.stageSaveData.entryCharacters.Count; i++)
            {
                StageManager.Instance.stageSaveData.entryCharacters[i] = null;
            }
            DeleteSpawnedCharacters(); // 월드에 스폰된 캐릭터 제거
            OpenSelectStagmaPanel("StartReward"); // 시작 시 각인 선택 패널 열기
        }
        else
        {
            messagePopup.Open("팀에 캐릭터가 5명 모두 있어야 탐색을 시작할 수 있습니다.");
        }
    }

    private void RefreshSpawnedCharacters(int formationIndex) // 월드에 스폰된 캐릭터를 갱신하는 함수
    {
        DeleteSpawnedCharacters(); // 기존에 스폰된 캐릭터 제거
        for (int i = 0; i < StageManager.Instance.stageSaveData.entryCharacters.Count; i++)
        {
            if (StageManager.Instance.stageSaveData.entryCharacters[i] != null)
            {
                // 캐릭터를 월드에 스폰하는 로직, 스테이지 데이터에 스폰 포지션이 있으며 스폰 포지션과 같은 인덱스의 엔트리 캐릭터를 스폰
                GameObject battleCharacterObject = StageManager.Instance.stageSaveData.entryCharacters[i].charBattlePrefab;
                Vector3 spawnPoint = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.PlayerFormations[formationIndex].PlayerPositions[i].Position;
                GameObject spawnedCharacter = Instantiate(battleCharacterObject, spawnPoint, Quaternion.identity); // 스폰 포인트에 캐릭터 스폰
            }
        }
    }
    private void DeleteSpawnedCharacters() // 월드에 스폰된 캐릭터를 제거하는 함수
    {
        var characters = FindObjectsByType<SpawnedCharacter>(FindObjectsInactive.Include, FindObjectsSortMode.None); // 현재 씬에 있는 모든 SpawnedCharacter를 찾아서
        foreach (var character in characters)
        {
            Destroy(character.gameObject); // 제거
        }
    }

    private void RefreshTeamFormationButton() // 팀 구성 버튼 상태를 갱신하는 함수
    {
        for (int i = 0; i < teamFormationIcons.Length; i++)
        {
            teamFormationIcons[i].color = (SelectedTeamFormation)i == selectedTeamFormation ? Color.yellow : Color.white; // 선택된 팀 구성은 노란색, 나머지는 흰색으로 표시
        }
        RefreshSpawnedCharacters((int)StageManager.Instance.stageSaveData.currentFormationType); // 현재 스폰된 캐릭터들을 갱신
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
        selectedArtifact = null; // 선택된 아티팩트 초기화
        selectedStagma = null; // 선택된 스태그마 초기화
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

        List<StagmaData> availableStagmas = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].StagmaList; // 현재 스테이지의 스태그마 목록을 가져옴
        itemTitleText.text = "각인 선택"; // 스태그마 선택 UI 제목 설정
        itemDescriptionText.text = ""; // 초기화
        HashSet<StagmaData> picked = new HashSet<StagmaData>();
        for (int i = 0; i < 3; i++)
        {
            StagmaData candidate;
            do
            {
                int rand = Random.Range(0, availableStagmas.Count);
                candidate = availableStagmas[rand];
            } while (picked.Contains(candidate));
            stagmaChoices[i] = candidate;
            picked.Add(candidate);

            var iconImage = itemChoiceIcon[i].GetComponent<Image>();
            iconImage.sprite = candidate.icon;
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
        selectedArtifact = null; // 선택된 아티팩트 초기화
        selectedStagma = null; // 선택된 스태그마 초기화
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
                messagePopup.Open("잘못된 페이즈 상태입니다. 다시 시도해 주세요.");
                return;
        }
        StageManager.Instance.stageSaveData.currentPhaseState = phaseState; // 현재 페이즈 상태 저장
        itemTitleText.text = "아티팩트 선택"; // 아티팩트 선택 UI 제목 설정
        itemDescriptionText.text = ""; // 초기화

        List<ArtifactData> allArtifacts = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex]
    .stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].ArtifactList;

        List<ArtifactData> availableArtifacts = null;
        switch (phaseState)
        {
            case "NormalReward":
                // 커먼, 언커먼만
                availableArtifacts = allArtifacts
                    .Where(a => a.artifactType == ArtifactData.ArtifactType.Common || a.artifactType == ArtifactData.ArtifactType.Uncommon)
                    .ToList();
                break;
            case "EliteArtifactReward":
                // 언커먼, 레어만
                availableArtifacts = allArtifacts
                    .Where(a => a.artifactType == ArtifactData.ArtifactType.Uncommon || a.artifactType == ArtifactData.ArtifactType.Rare)
                    .ToList();
                break;
            case "BossReward":
                // 레어, 유니크, 레전더리만
                availableArtifacts = allArtifacts
                    .Where(a => a.artifactType == ArtifactData.ArtifactType.Rare
                             || a.artifactType == ArtifactData.ArtifactType.Unique
                             || a.artifactType == ArtifactData.ArtifactType.Legendary)
                    .ToList();
                break;
            default:
                availableArtifacts = allArtifacts.ToList();
                break;
        }
        HashSet<ArtifactData> picked = new HashSet<ArtifactData>();
        for (int i = 0; i < 3; i++)
        {
            ArtifactData candidate;
            do
            {
                int rand = Random.Range(0, availableArtifacts.Count);
                candidate = availableArtifacts[rand];
            } while (picked.Contains(candidate));
            artifactChoices[i] = candidate;
            picked.Add(candidate);

            var iconImage = itemChoiceIcon[i].GetComponent<Image>();
            iconImage.sprite = candidate.icon;
        }
        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(true);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(true);
        selectEventPanel.SetActive(false);
        OnClickSelectItemNumber(0); // 첫 번째 아이템을 선택한 것으로 초기화
    }

    public void OnClickSelectItemNumber(int selectIndex) // 아티팩트 패널과 스태그마 패널 둘 다 다루니 아이템 패널이라고 함
    {
        this.selectIndex = selectIndex; // 선택된 아이템 인덱스 설정
        if (selectIndex < 0 || selectIndex >= 3)
        {
            messagePopup.Open("잘못된 선택입니다. 다시 시도해 주세요.");
            return;
        }
        // 모든 아이콘의 부모 Outline을 비활성화
        for (int i = 0; i < itemChoiceIcon.Length; i++)
        {
            var parent = itemChoiceIcon[i].transform.parent;
            if (parent != null)
            {
                var outline = parent.GetComponent<Outline>();
                if (outline != null)
                    outline.enabled = false;
            }
        }

        // 선택된 아이콘의 부모 Outline만 활성화
        var selectedParent = itemChoiceIcon[selectIndex].transform.parent;
        if (selectedParent != null)
        {
            var selectedOutline = selectedParent.GetComponent<Outline>();
            if (selectedOutline != null)
                selectedOutline.enabled = true;
        }
        switch (StageManager.Instance.stageSaveData.currentPhaseState)
        {
            case "StartReward":
            case "EliteStagmaReward":
                selectedStagma = stagmaChoices[selectIndex];
                itemTitleText.text = selectedStagma.name; // 선택된 스태그마 이름 설정
                itemDescriptionText.text = selectedStagma.description; // 선택된 스태그마 설명 설정
                break;
            case "NormalReward":
            case "EliteArtifactReward":
            case "BossReward":
                selectedArtifact = artifactChoices[selectIndex];
                itemTitleText.text = selectedArtifact.name; // 선택된 아티팩트 이름 설정
                itemDescriptionText.text = selectedArtifact.description; // 선택된 아티팩트 설명 설정
                break;
            default:
                messagePopup.Open("잘못된 페이즈 상태입니다. 다시 시도해 주세요.");
                return;
        }
    }

    public void OnClickSelectItem() // 아티팩트 패널과 스태그마 패널 둘 다 다루니 아이템 패널이라고 함
    {
        string phaseState = StageManager.Instance.stageSaveData.currentPhaseState; // 현재 페이즈 상태를 가져옴
        selectItemPanel.SetActive(false);
        StageManager.Instance.stageSaveData.currentPhaseState = ""; // 선택지 페이즈 상태 초기화
        switch (phaseState)
        {
            case "StartReward":
            case "EliteStagmaReward":
                if (selectedStagma != null)
                    StageManager.Instance.AddStagma(selectedStagma);
                break;
            case "NormalReward":
            case "EliteArtifactReward":
            case "BossReward":
                if (selectedArtifact != null)
                    StageManager.Instance.AddArtifacts(selectedArtifact);
                break;
        }
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

    public void OnClickStageNextButton() // 스테이지 패널에서 다음 버튼 클릭 시 호출되는 함수
    {
        StageManager.Instance.stageSaveData.currentPhaseIndex++; // 다음 페이즈로 이동
        OpenSelectChoicePanel(); // 선택지 이벤트 패널 열기
    }

    public void OpenSelectChoicePanel() // 선택지 이벤트 패널을 여는 함수
    {
        StageManager.Instance.stageSaveData.currentPhaseState = "SelectChoice"; // 현재 페이즈 상태를 "SelectChoice"로 설정
        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(false);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectEventPanel.SetActive(true); // 선택지 이벤트 패널 활성화
    }

    public void OnClickSelectChoice(int phaseStage) // 나중에 쓰도록 만들어 놓음
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