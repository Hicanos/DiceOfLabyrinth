using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
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
    public InputActionReference pointerPositionAction; // Input System을 사용하기 위한 Input Action Asset

    [Header("Select Item Panel")]
    [SerializeField] private TMP_Text itemTitleText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private int selectIndex = 0; // 선택된 아이템 인덱스, 스태그마와 아티팩트 선택을 위한 인덱스
    [SerializeField] private EngravingData[] engravingChoices = new EngravingData[3];
    [SerializeField] private ArtifactData[] artifactChoices = new ArtifactData[3];
    [SerializeField] private EngravingData selectedEngraving;
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
    [SerializeField] private GameObject selectArtifactPanel;

    [Header("Popup")]
    [SerializeField] private GameObject shopPopup;

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
    [SerializeField] private GameObject[] characterPlatforms = new GameObject[5]; // 캐릭터를 위한 플랫폼 배열, 5개로 고정

    [Header("Select Choice Panel")]
    [SerializeField] private TMP_Text[] selectChoiceText = new TMP_Text[2]; // 선택지 이벤트 패널 제목
    [SerializeField] private Image[] selectChoiceIcon = new Image[2]; // 선택지 이벤트 패널 아이콘
    [SerializeField] private ChoiceOptions[] ChoiceOptions = new ChoiceOptions[2]; // 선택지 이벤트 패널 선택지 옵션, 선택지는 2개까지만 뜸

    [Header("Platforms")]
    [SerializeField] private GameObject platformPrefab; // 플랫폼 프리팹
    [SerializeField] private Color platformDefaultColor; // 플랫폼 기본 색상
    [SerializeField] private Color platformSelectedColor; // 플랫폼 선택 시 색상
    private int selectedPlatformIndex = -1; // 선택된 플랫폼 인덱스
#if UNITY_EDITOR // 에디터에서만 디버그 키 입력을 처리합니다.
    private void Update()
    {
        if (Keyboard.current == null) return; // Input System이 없으면 무시

        if (StageManager.Instance != null &&
            StageManager.Instance.stageSaveData != null &&
            StageManager.Instance.stageSaveData.currentPhaseState == StageSaveData.CurrentPhaseState.Battle)
        {
            if (Keyboard.current.f9Key.wasPressedThisFrame)
            {

                BattleManager.Instance.BattleSpawner.CharacterDeActive();
                Destroy(BattleManager.Instance.Enemy.EnemyPrefab);
                var data = new BattleResultData(true, BattleManager.Instance.BattleGroup.BattleCharacters);
                messagePopup.Open("디버그: 즉시 배틀 승리 처리");
                StageManager.Instance.OnBattleResult(data);
            }
            if (Keyboard.current.f10Key.wasPressedThisFrame)
            {

                BattleManager.Instance.BattleSpawner.CharacterDeActive();
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
    private void OnEnable()
    {
        // 기존에 생성된 플랫폼 오브젝트가 있다면 먼저 파괴
        for (int i = 0; i < characterPlatforms.Length; i++)
        {
            if (characterPlatforms[i] != null)
            {
                Destroy(characterPlatforms[i]);
                characterPlatforms[i] = null;
            }
        }

        // 5개 생성 및 할당
        for (int i = 0; i < characterPlatforms.Length; i++)
        {
            // 원하는 위치로 변경 가능
            Vector3 spawnPos = Vector3.zero;
            var platform = Instantiate(platformPrefab, spawnPos, Quaternion.identity);
            platform.SetActive(false); // 생성 즉시 비활성화
            characterPlatforms[i] = platform;
            characterPlatforms[i].GetComponent<PlatformClickRelay>().platformIndex = i; // 플랫폼 인덱스 설정
        }
    }
    private void OnDisable()
    {
        for (int i = 0; i < characterPlatforms.Length; i++)
        {
            if (characterPlatforms[i] != null)
            {
                Destroy(characterPlatforms[i]);
                characterPlatforms[i] = null;
            }
        }
        selectedPlatformIndex = -1; // 초기 선택된 플랫폼 인덱스 설정
    }
    public void OnClickPerformed(InputAction.CallbackContext context)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        Vector2 pointerPos = pointerPositionAction.action.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(pointerPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var relay = hit.transform.GetComponent<PlatformClickRelay>();
            if (relay != null)
            {
                if (context.interaction is TapInteraction)
                {
                    for (int i = 0; i < characterPlatforms.Length; i++)
                    {
                        if (hit.transform.gameObject == characterPlatforms[i])
                        {
                            OnPlatformClicked(i);
                            Debug.Log($"Platform {i} clicked.");
                            break;
                        }
                    }
                }
                //else if (context.interaction is HoldInteraction)
                //{
                //    Debug.Log($"Platform {relay.platformIndex} is held.");
                //}
                //else
                //{
                //    Debug.LogWarning("Unknown interaction type detected.");
                //}
            }
        }
    }
    private void OnPlatformClicked(int platformIndex)
    {
        selectedPlatformIndex = platformIndex; // 선택된 플랫폼 인덱스 저장
        RefreshPlatformColors(platformIndex);
    }

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
        foreach (var characterPlatform in characterPlatforms)
        {
            if (characterPlatform != null)
                characterPlatform.SetActive(false);
        }
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
        foreach (var characterPlatform in characterPlatforms)
        {
            if (characterPlatform != null)
                characterPlatform.SetActive(true);
        }
         shopPopup.SetActive(false);
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


    private void RefreshPlatformColors(int selectedIndex)
    {
        for (int i = 0; i < characterPlatforms.Length; i++)
        {
            if (characterPlatforms[i] != null)
            {
                var platformRenderer = characterPlatforms[i].GetComponent<Renderer>();
                if (platformRenderer != null)
                {
                    platformRenderer.material.color = (i == selectedIndex) ? platformSelectedColor : platformDefaultColor; // 선택된 플랫폼은 선택 색상, 나머지는 기본 색상
                }
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
                    OpenSelectEngravingPanel(StageSaveData.CurrentPhaseState.StartReward); // 시작 시 각인 선택 패널 열기
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
            Vector3 spawnPoint = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.PlayerFormations[formationIndex].PlayerPositions[i].Position;
            if (StageManager.Instance.stageSaveData.entryCharacters[i] != null)
            {
                // 캐릭터를 월드에 스폰하는 로직, 스테이지 데이터에 스폰 포지션이 있으며 스폰 포지션과 같은 인덱스의 엔트리 캐릭터를 스폰
                GameObject battleCharacterObject = StageManager.Instance.stageSaveData.entryCharacters[i].charBattlePrefab;
                GameObject spawnedCharacter = Instantiate(battleCharacterObject, spawnPoint, Quaternion.identity); // 스폰 포인트에 캐릭터 스폰
            }

            GameObject characterPlatform = characterPlatforms[i]; // 캐릭터 플랫폼 가져오기
            characterPlatform.transform.position = spawnPoint; // 플랫폼 위치 설정
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
        shopPopup.SetActive(false);
        // selectArtifactPanel.SetActive(false); // 아티팩트 선택 패널은 현재 사용하지 않으므로 주석 처리
        foreach (var characterPlatform in characterPlatforms)
        {
            if (characterPlatform != null)
                characterPlatform.SetActive(false);
        }
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
        shopPopup.SetActive(false);
        // selectArtifactPanel.SetActive(false); // 아티팩트 선택 패널은 현재 사용하지 않으므로 주석 처리
        foreach (var characterPlatform in characterPlatforms)
        {
            if (characterPlatform != null)
                characterPlatform.SetActive(false);
        }
    }

    public void OpenSelectEngravingPanel(StageSaveData.CurrentPhaseState phaseState) // "Standby", "NormalReward", "EliteArtifactReward", "EliteEngravingReward" 등과 연결
    {
        StageManager.Instance.stageSaveData.currentPhaseState = phaseState; // 현재 페이즈 상태를 설정
        selectedArtifact = null; // 선택된 아티팩트 초기화
        selectedEngraving = null; // 선택된 스태그마 초기화
        engravingChoices = new EngravingData[3]; // 스태그마 선택 배열 초기화
        artifactChoices = new ArtifactData[3]; // 아티팩트 선택 배열 초기화
        //예외 상태 스트링 값을 처리하는 스위치
        switch (phaseState)
        {
            case StageSaveData.CurrentPhaseState.StartReward:
            case StageSaveData.CurrentPhaseState.EliteEngravingReward:
                // 스태그마 선택 UI를 열어야 하는 경우만 break(아래 코드 실행)
                break;
            default:
                Debug.LogError($"잘못된 phase state: {phaseState}");
                return;
        }

        List<EngravingData> allEngravings = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].EngravingList; // 현재 스테이지의 스태그마 목록을 가져옴
        var owned = StageManager.Instance.stageSaveData.engravings.Where(s => s != null).ToList();
        var availableEngravings = allEngravings.Except(owned).ToList();
        itemTitleText.text = "각인 선택"; // 스태그마 선택 UI 제목 설정
        itemDescriptionText.text = ""; // 초기화
        HashSet<EngravingData> picked = new HashSet<EngravingData>();
        for (int i = 0; i < 3; i++)
        {
            EngravingData candidate;
            do
            {
                int rand = Random.Range(0, availableEngravings.Count);
                candidate = availableEngravings[rand];
            } while (picked.Contains(candidate));
            engravingChoices[i] = candidate;
            picked.Add(candidate);

            var iconImage = itemChoiceIcon[i].GetComponent<Image>();
            iconImage.sprite = candidate.Icon;
        }

        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(true);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(true);
        selectEventPanel.SetActive(false);
        shopPopup.SetActive(false);
        // selectArtifactPanel.SetActive(false); // 아티팩트 선택 패널은 현재 사용하지 않으므로 주석 처리
        foreach (var characterPlatform in characterPlatforms)
        {
            if (characterPlatform != null)
                characterPlatform.SetActive(false);
        }
        OnClickSelectItemNumber(0); // 첫 번째 아이템을 선택한 것으로 초기화
    }

    public void OpenSelectArtifactPanel(StageSaveData.CurrentPhaseState phaseState) // "NormalReward", "EliteArtifactReward","EliteEngravingReward", "BossReward" 와 연결
    {
        StageManager.Instance.stageSaveData.currentPhaseState = phaseState; // 현재 페이즈 상태를 설정
        selectedArtifact = null; // 선택된 아티팩트 초기화
        selectedEngraving = null; // 선택된 스태그마 초기화
        engravingChoices = new EngravingData[3]; // 스태그마 선택 배열 초기화
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
                    .Where(a => a.ArtifactRarity == ArtifactData.ArtifactType.Common || a.ArtifactRarity == ArtifactData.ArtifactType.Uncommon)
                    .ToList();
                break;
            case StageSaveData.CurrentPhaseState.EliteArtifactReward:
                // 언커먼, 레어만
                availableArtifacts = available
                    .Where(a => a.ArtifactRarity == ArtifactData.ArtifactType.Uncommon || a.ArtifactRarity == ArtifactData.ArtifactType.Rare)
                    .ToList();
                break;
            case StageSaveData.CurrentPhaseState.BossReward:
                // 레어, 유니크, 레전더리만
                availableArtifacts = available
                    .Where(a => a.ArtifactRarity == ArtifactData.ArtifactType.Rare
                             || a.ArtifactRarity == ArtifactData.ArtifactType.Unique
                             || a.ArtifactRarity == ArtifactData.ArtifactType.Legendary)
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
            iconImage.sprite = candidate.Icon;
        }
        selectDungeonPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(true);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(true);
        selectEventPanel.SetActive(false);
        shopPopup.SetActive(false);
        // selectArtifactPanel.SetActive(false); // 아티팩트 선택 패널은 현재 사용하지 않으므로 주석 처리
        foreach (var characterPlatform in characterPlatforms)
        {
            characterPlatform.SetActive(false); // 캐릭터 플랫폼 비활성화
        }
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
            case StageSaveData.CurrentPhaseState.EliteEngravingReward:
                selectedEngraving = engravingChoices[selectIndex];
                itemTitleText.text = selectedEngraving.name; // 선택된 스태그마 이름 설정
                itemDescriptionText.text = selectedEngraving.Description; // 선택된 스태그마 설명 설정
                break;
            case StageSaveData.CurrentPhaseState.NormalReward:
            case StageSaveData.CurrentPhaseState.EliteArtifactReward:
            case StageSaveData.CurrentPhaseState.BossReward:
                selectedArtifact = artifactChoices[selectIndex];
                itemTitleText.text = selectedArtifact.name; // 선택된 아티팩트 이름 설정
                itemDescriptionText.text = selectedArtifact.Description; // 선택된 아티팩트 설명 설정
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
            case StageSaveData.CurrentPhaseState.EliteEngravingReward:
                if (selectedEngraving != null)
                    StageManager.Instance.AddEngraving(selectedEngraving);
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
            case StageSaveData.CurrentPhaseState.EliteEngravingReward:
                OpenStagePanel(StageManager.Instance.stageSaveData.currentPhaseIndex); // 스타트,노멀,엘리트 각인 페이즈 이후에는 다른 선택지 없이 스탠바이를 시작할 예정
            
                break;
            case StageSaveData.CurrentPhaseState.EliteArtifactReward:
                OpenSelectEngravingPanel(StageSaveData.CurrentPhaseState.EliteEngravingReward); // 엘리트 아티팩트 리워드 페이즈에서는 스태그마 선택 패널을 열도록 함
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
        if (StageManager.Instance.stageSaveData.currentPhaseIndex < 4) // 페이즈4 까지는 선택지 패널을 열고 그 후 배틀 룸 입장
        {
            OpenSelectChoicePanel(); // 선택지 이벤트 패널 열기
        }
        else if (StageManager.Instance.stageSaveData.currentPhaseIndex == 4) // 페이즈 5는 선택지 대신 상점을 염
        {
            OpenShopPopup(); // 상점 패널 열기
        }
        else if (StageManager.Instance.stageSaveData.currentPhaseIndex == 5) // 페이즈 6은 보스 룸
        {
            MessagePopup.Instance.Open("보스가 등장했습니다! 입장할래?",
            () => StageManager.Instance.selectBossEnemy(),
            () => MessagePopup.Instance.Close());
        }
        else // 페이즈 인덱스가 범위를 벗어난 경우
        {
            messagePopup.Open("잘못된 페이즈 인덱스입니다. 다시 시도해 주세요.");
        }
    }

    public void OpenSelectChoicePanel() // 선택지 이벤트 패널을 여는 함수
    {
        StageManager.Instance.stageSaveData.currentPhaseState = StageSaveData.CurrentPhaseState.SelectChoice; // 현재 페이즈 상태를 선택지 이벤트로 설정
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
        shopPopup.SetActive(false);
        // selectArtifactPanel.SetActive(false); // 아티팩트 선택 패널은 현재 사용하지 않으므로 주석 처리
        selectEventPanel.SetActive(true); // 선택지 이벤트 패널 활성화
        foreach (var characterPlatform in characterPlatforms)
        {
            if (characterPlatform != null)
                characterPlatform.SetActive(false);
        }
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
        shopPopup.SetActive(false);
        // selectArtifactPanel.SetActive(false); // 아티팩트 선택 패널은 현재 사용하지 않으므로 주석 처리
        foreach (var characterPlatform in characterPlatforms)
        {
            if (characterPlatform != null)
                characterPlatform.SetActive(false);
        }
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
        shopPopup.SetActive(false);
        // selectArtifactPanel.SetActive(false); // 아티팩트 선택 패널은 현재 사용하지 않으므로 주석 처리
        foreach (var characterPlatform in characterPlatforms)
        {
            if (characterPlatform != null)
                characterPlatform.SetActive(false);
        }
    }

    public void OnClickVictoryNextButton() // 승리 패널에서 다음 버튼 클릭 시 호출되는 함수
    {
        StageManager.Instance.RoomClear(StageManager.Instance.stageSaveData.selectedEnemy); // 현재 스테이지 클리어 처리
    }

    public void OnClickDefeatNextButton() // 패배 패널에서 다음 버튼 클릭 시 호출되는 함수
    {
        StageManager.Instance.EndChapterEarly(StageManager.Instance.stageSaveData.currentChapterIndex); // 현재 챕터 패배 처리
    }

    public void OpenShopPopup()
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
        shopPopup.SetActive(true);
        // selectArtifactPanel.SetActive(false); // 아티팩트 선택 패널은 현재 사용하지 않으므로 주석 처리
        foreach (var characterPlatform in characterPlatforms)
        {
            if (characterPlatform != null)
                characterPlatform.SetActive(false);
        }
        ShopPopup.Instance.StartShop();
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
        shopPopup.SetActive(false);
        // selectEquipmedArtifactPanel.SetActive(true); // 아티팩트 선택 패널 활성화
        foreach (var characterPlatform in characterPlatforms)
        {
            if (characterPlatform != null)
                characterPlatform.SetActive(false);
        }
        messagePopup.Open("장착할 아티팩트 선택은 아직 구현되지 않았으므로 바로 스테이지 클리어로 넘어갑니다.",
        () => StageManager.Instance.StageComplete(StageManager.Instance.stageSaveData.currentStageIndex), // 스테이지 클리어 처리
        () => messagePopup.Close());
    }
}