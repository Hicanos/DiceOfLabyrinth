using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

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
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject selectArtifactPanel;

    [Header("Item Choice Icons")]
    [SerializeField] private GameObject[] itemChoiceIcon = new GameObject[3]; // 아이템 선택 아이콘을 위한 배열

    //[Header("Select Dungeon")]
    //[SerializeField] private TMP_Text selectedChapterText; // 스테이지 선택 패널 제목
    //[SerializeField] private Image chapterIcon; // 스테이지 선택 패널 아이콘
    //[SerializeField] private TMP_Text chapterDescriptionText; // 스테이지 선택 패널 설명

    [Header("Team Formation")]
    [SerializeField] private SelectedTeamFormation selectedTeamFormation; // 선택된 팀 구성
    [SerializeField] private Image[] teamFormationIcons = new Image[4]; // 팀 구성 아이콘 배열
    [SerializeField] Image[] characterButtons = new Image[7];
    
    [Header("Select Choice Panel")]
    [SerializeField] private TMP_Text[] selectChoiceText = new TMP_Text[2]; // 선택지 이벤트 패널 제목
    [SerializeField] private Image[] selectChoiceIcon = new Image[2]; // 선택지 이벤트 패널 아이콘
    [SerializeField] private ChoiceOptions[] ChoiceOptions = new ChoiceOptions[2]; // 선택지 이벤트 패널 선택지 옵션, 선택지는 2개까지만 뜸

#if UNITY_EDITOR // 에디터에서만 디버그 키 입력을 처리합니다.
    private void Update()
    {
        //// 배틀 상태일 때만 동작
        //if (StageManager.Instance != null &&
        //    StageManager.Instance.stageSaveData != null &&
        //    StageManager.Instance.stageSaveData.currentPhaseState == StageSaveData.CurrentPhaseState.Battle)
        //{
        //    // F9: 즉시 배틀 승리
        //    if (Input.GetKeyDown(KeyCode.F9)&& StageManager.Instance.stageSaveData.currentPhaseState == StageSaveData.CurrentPhaseState.Battle)
        //    {
        //        messagePopup.Open("디버그: 즉시 배틀 승리 처리"); // 메시지 팝업 표시
        //        StageManager.Instance.RoomClear(StageManager.Instance.stageSaveData.selectedEnemy); // 배틀 승리 처리
        //    }
        //    // F10: 즉시 전투 패배
        //    if (Input.GetKeyDown(KeyCode.F10) && StageManager.Instance.stageSaveData.currentPhaseState == StageSaveData.CurrentPhaseState.Battle)
        //    {
        //        messagePopup.Open("디버그: 즉시 패배 처리"); // 메시지 팝업 표시
        //        StageManager.Instance.StageDefeat(StageManager.Instance.stageSaveData.currentChapterIndex); // 배틀 패배 처리
        //    }
        //}
        //if (StageManager.Instance != null && StageManager.Instance.stageSaveData != null)
        //{
        //    // F11: 즉시 챕터 종료 처리 (컴플리트 아님)
        //    if (Input.GetKeyDown(KeyCode.F11) && StageManager.Instance.stageSaveData.currentChapterIndex != -1)
        //    {
        //        Debug.Log("디버그: 즉시 챕터 종료 처리");
        //        messagePopup.Open("디버그: 즉시 챕터 종료 처리");
        //        StageManager.Instance.EndChapterEarly(StageManager.Instance.stageSaveData.currentChapterIndex);
        //    }
        //    // F12: 즉시 챕터 완료 처리 (컴플리트 처리)
        //    if (Input.GetKeyDown(KeyCode.F12) && StageManager.Instance.stageSaveData.currentChapterIndex != -1)
        //    {
        //        Debug.Log("디버그: 즉시 챕터 완료 처리");
        //        messagePopup.Open("디버그: 즉시 챕터 완료 처리");
        //        StageManager.Instance.CompleteChapter(StageManager.Instance.stageSaveData.currentChapterIndex);
        //    }
        //}
        if (Keyboard.current == null) return; // Input System이 없으면 무시

        if (StageManager.Instance != null &&
            StageManager.Instance.stageSaveData != null &&
            StageManager.Instance.stageSaveData.currentPhaseState == StageSaveData.CurrentPhaseState.Battle)
        {
            if (Keyboard.current.f9Key.wasPressedThisFrame)
            {
                DiceManager.Instance.ResetSetting();

                BattleManager.Instance.battleSpawner.CharacterDeActive();
                Destroy(BattleManager.Instance.Enemy.EnemyPrefab);
                var data = new BattleResultData(true, BattleManager.Instance.BattleGroup.BattleCharacters);
                messagePopup.Open("디버그: 즉시 배틀 승리 처리");
                StageManager.Instance.OnBattleResult(data);
            }
            if (Keyboard.current.f10Key.wasPressedThisFrame)
            {
                DiceManager.Instance.ResetSetting();

                BattleManager.Instance.battleSpawner.CharacterDeActive();
                Destroy(BattleManager.Instance.Enemy.EnemyPrefab);
                messagePopup.Open("디버그: 즉시 패배 처리");
                var defeatData = new BattleResultData(false, BattleManager.Instance.BattleGroup.BattleCharacters);
                StageManager.Instance.OnBattleResult(defeatData);
            }
        }
        if (StageManager.Instance != null && StageManager.Instance.stageSaveData != null)
        {
            if (Keyboard.current.f11Key.wasPressedThisFrame && StageManager.Instance.stageSaveData.currentChapterIndex != -1)
            {
                Debug.Log("디버그: 즉시 챕터 종료 처리");
                messagePopup.Open("디버그: 즉시 챕터 종료 처리");
                StageManager.Instance.EndChapterEarly(StageManager.Instance.stageSaveData.currentChapterIndex);
            }
            if (Keyboard.current.f12Key.wasPressedThisFrame && StageManager.Instance.stageSaveData.currentChapterIndex != -1)
            {
                Debug.Log("디버그: 즉시 챕터 완료 처리");
                messagePopup.Open("디버그: 즉시 챕터 완료 처리");
                StageManager.Instance.CompleteChapter(StageManager.Instance.stageSaveData.currentChapterIndex);
            }
        }
    }
#endif

    public void OpenSelectDungeonPanel() // 스테이지 선택 패널을 여는 함수
    {
        StageManager.Instance.stageSaveData.currentPhaseState = StageSaveData.CurrentPhaseState.None;
        selectDungeonPanel.SetActive(true);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(false);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        //shopPanel.SetActive(false); // 상점 패널은 현재 사용하지 않으므로 주석 처리
        // selectArtifactPanel.SetActive(false); // 아티팩트 선택 패널은 현재 사용하지 않으므로 주석 처리

        selectEventPanel.SetActive(false);
        // 선택된 스테이지 정보 업데이트
        //selectedChapterText.text = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].ChapterName;
        //chapterIcon.sprite = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].Image;
        //chapterDescriptionText.text = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].Description;
    }

    public void OnClickDungeonButton(int stageIndex) // 스테이지 선택 버튼 클릭 시 호출되는 함수
    {
        if (stageIndex < StageManager.Instance.stageSaveData.currentStageIndex) // 스테이지가 이미 클리어되었을 때
        {
            messagePopup.Open("이 던전은 이미 클리어 했습니다. 다음 스테이지를 선택해 주세요.");
        }
        else if (stageIndex > StageManager.Instance.stageSaveData.currentStageIndex) // 스테이지가 잠겨있을 때
        {
            messagePopup.Open("이 던전은 아직 잠겨 있습니다. 다른 스테이지를 완료한 후 다시 시도해 주세요."); // 스테이지가 잠겨있을 때 경고 메시지 표시
        }
        else
        {
            StageManager.Instance.stageSaveData.currentStageIndex = stageIndex; // 현재 스테이지 인덱스 설정
            StageManager.Instance.stageSaveData.currentPhaseIndex = -1; // 초기 페이즈 인덱스 설정
            OpenTeamFormationPanel(); // 팀 구성 패널 열기
        }
    }
    public void OpenTeamFormationPanel()
    {
        RefreshTeamFormationButton(); // 팀 구성 버튼 상태 갱신
        RefreshSpawnedCharacters((int)StageManager.Instance.stageSaveData.currentFormationType); // 현재 스폰된 캐릭터들을 갱신
        //Debug.Log($"[TeamFormation] AcquiredCharacters Count: {CharacterManager.Instance.AcquiredCharacters.Count}");
        StageManager.Instance.stageSaveData.currentPhaseState = StageSaveData.CurrentPhaseState.TeamSelect; // 현재 페이즈 상태를 팀 구성으로 설정
        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(true);
        stagePanel.SetActive(false);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectEventPanel.SetActive(false);
        // shopPanel.SetActive(false); // 상점 패널은 현재 사용하지 않으므로 주석 처리
        // selectArtifactPanel.SetActive(false); // 아티팩트 선택 패널은 현재 사용하지 않으므로 주석 처리


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
            switch (StageManager.Instance.stageSaveData.currentPhaseIndex)
            {
                case -1: // 페이즈가 설정되지 않은 경우
                    StageManager.Instance.stageSaveData.currentPhaseIndex = 0; // 첫 번째 페이즈로 설정
                    OpenSelectStagmaPanel(StageSaveData.CurrentPhaseState.StartReward); // 시작 시 각인 선택 패널 열기
                    break;
                default:
                    messagePopup.Open($"잘못된 페이즈 인덱스: {StageManager.Instance.stageSaveData.currentPhaseIndex}"); // 잘못된 페이즈 인덱스 경고
                    break;
            }
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
        StageManager.Instance.stageSaveData.currentPhaseState = StageSaveData.CurrentPhaseState.Standby; // 현재 페이즈 상태를 대기 상태로 설정
        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(true);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectEventPanel.SetActive(false);
        // shopPanel.SetActive(false); // 상점 패널은 현재 사용하지 않으므로 주석 처리
        // selectArtifactPanel.SetActive(false); // 아티팩트 선택 패널은 현재 사용하지 않으므로 주석 처리
    }

    public void OpenBattlePanel()
    {
        StageManager.Instance.stageSaveData.currentPhaseState = StageSaveData.CurrentPhaseState.Battle; // 현재 페이즈 상태를 배틀 상태로 설정
        StageManager.Instance.stageSaveData.currentPhaseIndex++; // 다음 페이즈로 이동

        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(false);
        battlePanel.SetActive(true);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectEventPanel.SetActive(false);
        // shopPanel.SetActive(false); // 상점 패널은 현재 사용하지 않으므로 주석 처리
        // selectArtifactPanel.SetActive(false); // 아티팩트 선택 패널은 현재 사용하지 않으므로 주석 처리
    }

    public void OpenSelectStagmaPanel(StageSaveData.CurrentPhaseState phaseState) // "Standby", "NormalReward", "EliteArtifactReward", "EliteStagmaReward" 등과 연결
    {
        StageManager.Instance.stageSaveData.currentPhaseState = phaseState; // 현재 페이즈 상태를 설정
        selectedArtifact = null; // 선택된 아티팩트 초기화
        selectedStagma = null; // 선택된 스태그마 초기화
        stagmaChoices = new StagmaData[3]; // 스태그마 선택 배열 초기화
        artifactChoices = new ArtifactData[3]; // 아티팩트 선택 배열 초기화
        //예외 상태 스트링 값을 처리하는 스위치
        switch (phaseState)
        {
            case StageSaveData.CurrentPhaseState.StartReward:
            case StageSaveData.CurrentPhaseState.EliteStagmaReward:
                // 스태그마 선택 UI를 열어야 하는 경우만 break(아래 코드 실행)
                break;
            default:
                Debug.LogError($"잘못된 phase state: {phaseState}");
                return;
        }

        List<StagmaData> allStagmas = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].StagmaList; // 현재 스테이지의 스태그마 목록을 가져옴
        var owned = StageManager.Instance.stageSaveData.stagmas.Where(s => s != null).ToList();
        var availableStagmas = allStagmas.Except(owned).ToList();
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
        // shopPanel.SetActive(false); // 상점 패널은 현재 사용하지 않으므로 주석 처리
        // selectArtifactPanel.SetActive(false); // 아티팩트 선택 패널은 현재 사용하지 않으므로 주석 처리
        OnClickSelectItemNumber(0); // 첫 번째 아이템을 선택한 것으로 초기화
    }

    public void OpenSelectArtifactPanel(StageSaveData.CurrentPhaseState phaseState) // "NormalReward", "EliteArtifactReward","EliteStagmaReward", "BossReward" 와 연결
    {
        StageManager.Instance.stageSaveData.currentPhaseState = phaseState; // 현재 페이즈 상태를 설정
        selectedArtifact = null; // 선택된 아티팩트 초기화
        selectedStagma = null; // 선택된 스태그마 초기화
        stagmaChoices = new StagmaData[3]; // 스태그마 선택 배열 초기화
        artifactChoices = new ArtifactData[3]; // 아티팩트 선택 배열 초기화
        // 예외 상태 스트링 값을 처리하는 스위치
        switch (phaseState)
        {
            case StageSaveData.CurrentPhaseState.NormalReward:
            case StageSaveData.CurrentPhaseState.EliteArtifactReward:
            case StageSaveData.CurrentPhaseState.BossReward:
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
        var owned = StageManager.Instance.stageSaveData.artifacts.Where(a => a != null).ToList(); // 현재 소유한 아티팩트 목록
        var available = allArtifacts.Except(owned).ToList();

        List<ArtifactData> availableArtifacts = null;
        switch (phaseState)
        {
            case StageSaveData.CurrentPhaseState.NormalReward:
                // 커먼, 언커먼만
                availableArtifacts = available
                    .Where(a => a.artifactType == ArtifactData.ArtifactType.Common || a.artifactType == ArtifactData.ArtifactType.Uncommon)
                    .ToList();
                break;
            case StageSaveData.CurrentPhaseState.EliteArtifactReward:
                // 언커먼, 레어만
                availableArtifacts = available
                    .Where(a => a.artifactType == ArtifactData.ArtifactType.Uncommon || a.artifactType == ArtifactData.ArtifactType.Rare)
                    .ToList();
                break;
            case StageSaveData.CurrentPhaseState.BossReward:
                // 레어, 유니크, 레전더리만
                availableArtifacts = available
                    .Where(a => a.artifactType == ArtifactData.ArtifactType.Rare
                             || a.artifactType == ArtifactData.ArtifactType.Unique
                             || a.artifactType == ArtifactData.ArtifactType.Legendary)
                    .ToList();
                break;
            default:
                availableArtifacts = available.ToList();
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
        // shopPanel.SetActive(false); // 상점 패널은 현재 사용하지 않으므로 주석 처리
        // selectArtifactPanel.SetActive(false); // 아티팩트 선택 패널은 현재 사용하지 않으므로 주석 처리
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
            case StageSaveData.CurrentPhaseState.StartReward:
            case StageSaveData.CurrentPhaseState.EliteStagmaReward:
                selectedStagma = stagmaChoices[selectIndex];
                itemTitleText.text = selectedStagma.name; // 선택된 스태그마 이름 설정
                itemDescriptionText.text = selectedStagma.description; // 선택된 스태그마 설명 설정
                break;
            case StageSaveData.CurrentPhaseState.NormalReward:
            case StageSaveData.CurrentPhaseState.EliteArtifactReward:
            case StageSaveData.CurrentPhaseState.BossReward:
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
        var phaseState = StageManager.Instance.stageSaveData.currentPhaseState; // 현재 페이즈 상태를 가져옴
        selectItemPanel.SetActive(false);
        switch (phaseState)
        {
            case StageSaveData.CurrentPhaseState.StartReward:
            case StageSaveData.CurrentPhaseState.EliteStagmaReward:
                if (selectedStagma != null)
                    StageManager.Instance.AddStagma(selectedStagma);
                break;
            case StageSaveData.CurrentPhaseState.NormalReward:
            case StageSaveData.CurrentPhaseState.EliteArtifactReward:
            case StageSaveData.CurrentPhaseState.BossReward:
                if (selectedArtifact != null)
                    StageManager.Instance.AddArtifacts(selectedArtifact);
                break;
        }
        switch (phaseState)
        {
            case StageSaveData.CurrentPhaseState.StartReward:
            case StageSaveData.CurrentPhaseState.NormalReward:
            case StageSaveData.CurrentPhaseState.EliteStagmaReward:
                OpenStagePanel(StageManager.Instance.stageSaveData.currentPhaseIndex); // 스타트,노멀,엘리트 각인 페이즈 이후에는 다른 선택지 없이 스탠바이를 시작할 예정
            
                break;
            case StageSaveData.CurrentPhaseState.EliteArtifactReward:
                OpenSelectStagmaPanel(StageSaveData.CurrentPhaseState.EliteStagmaReward); // 엘리트 아티팩트 리워드 페이즈에서는 스태그마 선택 패널을 열도록 함
                break;
            case StageSaveData.CurrentPhaseState.BossReward:
                OpenSelectEquipedArtifactPanel(); // 보스 리워드 페이즈에서는 아티팩트 장착 패널을 열도록 함
                break;
            default:
                messagePopup.Open("잘못된 페이즈 상태입니다. 리워드 페이즈가 아닙니다.");
                break;
        }
    }

    public void OnClickStageNextButton() // 스테이지 패널에서 다음 버튼 클릭 시 호출되는 함수
    {
        if (StageManager.Instance.stageSaveData.currentPhaseIndex < 4) // 4룸 까지가 일반 또는 노말 스테이지
        {
            OpenSelectChoicePanel(); // 선택지 이벤트 패널 열기
        }
        else if (StageManager.Instance.stageSaveData.currentPhaseIndex == 4) // 5룸은 보스 룸
        {
            StageManager.Instance.stageSaveData.currentPhaseState = StageSaveData.CurrentPhaseState.BossReward; // 현재 페이즈 상태를 보스 리워드로 설정
            messagePopup.Open("보스가 등장했습니다! 입장할래?",
            () => StageManager.Instance.selectBossEnemy(),
            () => messagePopup.Close());
        }
        else // 페이즈 인덱스가 범위를 벗어난 경우
        {
            messagePopup.Open("잘못된 페이즈 인덱스입니다. 다시 시도해 주세요.");
        }
    }

    public void OpenSelectChoicePanel() // 선택지 이벤트 패널을 여는 함수
    {
        var stageSelectChoices = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex]
            .stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].ChoiceOptions; // 현재 스테이지의 선택지 목록을 가져옴\
                                                                                                        // 노말/엘리트 클리어 카운트 2 이상이면 해당 선택지 제외
        int normalCount = StageManager.Instance.stageSaveData.normalStageCompleteCount;
        int eliteCount = StageManager.Instance.stageSaveData.eliteStageCompleteCount;
        var filteredChoices = stageSelectChoices
            .Where(opt =>
                !(opt.ChoiceText == "노말" && normalCount >= 2) &&
                !(opt.ChoiceText == "엘리트" && eliteCount >= 2)
            ).ToList();

        StageManager.Instance.stageSaveData.currentPhaseState = StageSaveData.CurrentPhaseState.SelectChoice;

        // 필터링된 선택지 중 랜덤 2개 선택
        List<ChoiceOptions> tempChoices = new List<ChoiceOptions>(filteredChoices);
        ChoiceOptions[] twoSelectChoices = new ChoiceOptions[2];
        for (int i = 0; i < 2; i++)
        {
            int randIndex = Random.Range(0, tempChoices.Count);
            twoSelectChoices[i] = tempChoices[randIndex];
            tempChoices.RemoveAt(randIndex);
        }
        for (int i = 0; i < 2; i++)
        {
            selectChoiceText[i].text = twoSelectChoices[i].ChoiceText;
            selectChoiceIcon[i].sprite = twoSelectChoices[i].ChoiceIcon;
            ChoiceOptions[i] = twoSelectChoices[i];
        }

        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(true);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        // shopPanel.SetActive(false); // 상점 패널은 현재 사용하지 않으므로 주석 처리
        // selectArtifactPanel.SetActive(false); // 아티팩트 선택 패널은 현재 사용하지 않으므로 주석 처리
        selectEventPanel.SetActive(true); // 선택지 이벤트 패널 활성화
    }

    public void OnClickSelectChoice(int selectIndex)
    {
        if (selectIndex < 0 || selectIndex >= ChoiceOptions.Length)
        {
            messagePopup.Open("잘못된 선택입니다. 다시 시도해 주세요.");
            return;
        }
        // 선택된 선택지 옵션을 적용
        var selectedChoice = ChoiceOptions[selectIndex];
        switch (selectedChoice.ChoiceText)
        {
            case "이벤트":
                messagePopup.Open("이벤트는 아직 구현이 안되었습니다");
                return; // 이벤트는 아직 구현되지 않았으므로 경고 메시지 표시
            case "노말":
                messagePopup.Open("선택지 노말이 선택되었습니다.");
                StageManager.Instance.selectNormalEnemy();
                break;
            case "엘리트":
                messagePopup.Open("선택지 엘리트가 선택되었습니다.");
                StageManager.Instance.selectEliteEnemy(); // 엘리트 적 선택
                break;
            default:
                messagePopup.Open("알 수 없는 선택입니다. 다시 시도해 주세요.");
            return;
        }
        for (int i = 0; i < ChoiceOptions.Length; i++)
            ChoiceOptions[i] = null; // 선택된 선택지 옵션을 null로 설정하여 중복 선택 방지
    }

    public void OpenVictoryPanel() // 승리 패널을 여는 함수
    {
        StageManager.Instance.stageSaveData.currentPhaseState = StageSaveData.CurrentPhaseState.Battle; // 현재 페이즈 상태를 승리로 설정
        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(false);
        battlePanel.SetActive(true);
        victoryPanel.SetActive(true); // 승리 패널 활성화
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectEventPanel.SetActive(false);
        // shopPanel.SetActive(false); // 상점 패널은 현재 사용하지 않으므로 주석 처리
        // selectArtifactPanel.SetActive(false); // 아티팩트 선택 패널은 현재 사용하지 않으므로 주석 처리
    }

    public void OpenDefeatPanel() // 패배 패널을 여는 함수
    {
        StageManager.Instance.stageSaveData.currentPhaseState = StageSaveData.CurrentPhaseState.Battle; // 현재 페이즈 상태를 패배로 설정
        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(false);
        battlePanel.SetActive(true);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(true); // 패배 패널 활성화
        selectItemPanel.SetActive(false);
        selectEventPanel.SetActive(false);
        // shopPanel.SetActive(false); // 상점 패널은 현재 사용하지 않으므로 주석 처리
        // selectArtifactPanel.SetActive(false); // 아티팩트 선택 패널은 현재 사용하지 않으므로 주석 처리
    }

    public void OnClickVictoryNextButton() // 승리 패널에서 다음 버튼 클릭 시 호출되는 함수
    {
        OpenSelectDungeonPanel();
    }

    public void OnClickDefeatNextButton() // 패배 패널에서 다음 버튼 클릭 시 호출되는 함수
    {
        StageManager.Instance.EndChapterEarly(StageManager.Instance.stageSaveData.currentChapterIndex); // 현재 챕터 패배 처리
    }

    public void OpenShopPanel()
    {
        StageManager.Instance.stageSaveData.currentPhaseState = StageSaveData.CurrentPhaseState.Shop; // 현재 페이즈 상태를 상점으로 설정
        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(false);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectEventPanel.SetActive(false);
        // shopPanel.SetActive(true); // 상점 패널 활성화
        // selectArtifactPanel.SetActive(false); // 아티팩트 선택 패널은 현재 사용하지 않으므로 주석 처리
        messagePopup.Open("상점은 아직 구현되지 않았으므로 바로 보스 스테이지로 넘어갑니다.",
        () => OpenStagePanel(StageManager.Instance.stageSaveData.currentPhaseIndex),
        () => messagePopup.Close());
    }
    public void OpenSelectEquipedArtifactPanel() // 아티팩트 장착 선택 패널을 여는 함수
    {
        StageManager.Instance.stageSaveData.currentPhaseState = StageSaveData.CurrentPhaseState.EquipmedArtifact; // 현재 페이즈 상태를 "EquipmedArtifact"로 설정
        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(false);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectEventPanel.SetActive(false);
        // shopPanel.SetActive(false); // 상점 패널은 현재 사용하지 않으므로 주석 처리
        // selectEquipmedArtifactPanel.SetActive(true); // 아티팩트 선택 패널 활성화
        messagePopup.Open("장착할 아티팩트 선택은 아직 구현되지 않았으므로 바로 스테이지 클리어로 넘어갑니다.",
        () => StageManager.Instance.StageComplete(StageManager.Instance.stageSaveData.currentStageIndex), // 스테이지 클리어 처리
        () => messagePopup.Close());
    }
}