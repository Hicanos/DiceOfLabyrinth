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
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text artifactSetEffectText;// 아티팩트 세트 효과 설명을 위한 텍스트, 각인 선택시엔 비활성화
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private GameObject artifactSetEffectToolTip; // 아티팩트 세트 효과 설명을 위한 GameObject, 각인 선택시엔 비활성화
    [SerializeField] private GameObject artifactSetEffectDescription;
    [SerializeField] private int selectIndex = 0; // 선택된 아이템 인덱스, 각인과 아티팩트 선택을 위한 인덱스
    [SerializeField] private EngravingData[] engravingChoices = new EngravingData[3];
    [SerializeField] private ArtifactData[] artifactChoices = new ArtifactData[3];
    [SerializeField] private EngravingData selectedEngraving;
    [SerializeField] private ArtifactData selectedArtifact;

    [Header("Panels")]
    [SerializeField] private GameObject selectFloorPanel;
    [SerializeField] private GameObject teamFormationPenel;
    [SerializeField] private GameObject stagePanel;
    [SerializeField] private GameObject battlePanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private GameObject selectItemPanel;
    [SerializeField] private GameObject selectChoicePanel;
    [SerializeField] private GameObject blessingPanel;
    [SerializeField] private GameObject cursePanel;

    [Header("Pause Panel")]
    [SerializeField] private PausePanel pausePanel;

    [Header("Popup")]
    [SerializeField] private GameObject shopPopup;
    [SerializeField] private GameObject recoveryPopup; // 회복 팝업
    [SerializeField] private TMP_Text manaStoneText; // 마나스톤 갯수를 표시할 텍스트

    [Header("Item Choice Icons")]
    [SerializeField] private GameObject[] itemChoiceIcon = new GameObject[3]; // 아이템 선택 아이콘을 위한 배열
    [SerializeField] private GameObject[] itemChoiceFrame = new GameObject[3]; // 아이템 선택 프레임을 위한 배열

    [Header("Select Floor")]
    [SerializeField] private TMP_Text selectedFloorText; // 스테이지 선택 층수
    [SerializeField] private Image floorBossIcon; // 스테이지 선택 보스 아이콘
    [SerializeField] private Image floorBossElementIcon; // 스테이지 선택 아이콘 엘리먼트 타입
    [SerializeField] private TMP_Text floorBossNameText; // 스테이지 선택 보스 이름

    [Header("Team Formation")]
    [SerializeField] private SelectedTeamFormation selectedTeamFormation; // 선택된 팀 구성
    [SerializeField] private Image[] teamFormationIcons = new Image[4]; // 팀 구성 아이콘 배열
    [SerializeField] List<CharacterViewer> characterButtons = new List<CharacterViewer>(); // 캐릭터 버튼 배열, 최대 7개로 고정
    public GameObject[] characterPlatforms = new GameObject[5]; // 캐릭터를 위한 플랫폼 배열, 5개로 고정
    public GameObject[] CharacterPlatforms
    {
        get { return PlatformManager.Instance.platforms; }
    } // PlatformManager에서 가져온 플랫폼 배열

    [Header("Select Choice Panel")]
    [SerializeField] private TMP_Text[] selectChoiceText = new TMP_Text[2]; // 선택지 이벤트 패널 제목
    [SerializeField] private Image[] selectChoiceIcon = new Image[2]; // 선택지 이벤트 패널 아이콘
    [SerializeField] private ChoiceOptions[] ChoiceOptions = new ChoiceOptions[2]; // 선택지 이벤트 패널 선택지 옵션, 선택지는 2개까지만 뜸

    [Header("Platforms")]
    [SerializeField] private Color platformDefaultColor; // 플랫폼 기본 색상
    [SerializeField] private Color platformSelectedColor; // 플랫폼 선택 시 색상
    private int selectedPlatformIndex = -1; // 선택된 플랫폼 인덱스

    [Header("BgSprites")]
    [SerializeField] private Image worldMap;

#if UNITY_EDITOR // 에디터에서만 디버그 키 입력을 처리합니다.
    private void Update()
    {
        if (Keyboard.current == null) return; // Input System이 없으면 무시

        //if (StageManager.Instance != null &&
        //    StageManager.Instance.stageSaveData != null &&
        //    StageManager.Instance.stageSaveData.currentPhaseState == StageSaveData.CurrentPhaseState.Battle)
        //{
        //    if (Keyboard.current.f9Key.wasPressedThisFrame)
        //    {

        //        BattleManager.Instance.BattleSpawner.CharacterDeActive();
        //        Destroy(BattleManager.Instance.Enemy.EnemyPrefab);
        //        var data = new BattleResultData(true, BattleManager.Instance.BattleGroup.BattleCharacters);
        //        messagePopup.Open("디버그: 즉시 배틀 승리 처리");
        //        StageManager.Instance.OnBattleResult(data);
        //    }
        //    if (Keyboard.current.f10Key.wasPressedThisFrame)
        //    {

        //        BattleManager.Instance.BattleSpawner.CharacterDeActive();
        //        Destroy(BattleManager.Instance.Enemy.EnemyPrefab);
        //        messagePopup.Open("디버그: 즉시 패배 처리");
        //        var defeatData = new BattleResultData(false, BattleManager.Instance.BattleGroup.BattleCharacters);
        //        StageManager.Instance.OnBattleResult(defeatData);
        //    }
        //}
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
        RefreshManaStoneViewer(); // 마나스톤 갯수 초기화
    }

    public void OnClickPauseButton()
    {
        if (pausePanel != null)
        {
            pausePanel.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }       
    }
    public void OnClickPerformed(InputAction.CallbackContext context)
    {
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            return;
        }
#else
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
#endif
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
            }
        }
    }
    public void RefreshManaStoneViewer()
    {
        if (manaStoneText != null)
        {
            manaStoneText.text = StageManager.Instance.stageSaveData.manaStone.ToString(); // 현재 마나스톤 갯수를 표시
        }
        else
        {
            Debug.LogWarning("ManaStoneText가 설정되지 않았습니다.");
        }
    }
    private void OnPlatformClicked(int platformIndex)
    {
        selectedPlatformIndex = platformIndex; // 선택된 플랫폼 인덱스 저장
        RefreshPlatformColors(platformIndex);
    }

    public void OpenSelectFloorPanel() // 스테이지 선택 패널을 여는 함수
    {
        StageManager.Instance.stageSaveData.currentPhaseState = StageSaveData.CurrentPhaseState.None;
        selectFloorPanel.SetActive(true);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(false);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        blessingPanel.SetActive(false);
        cursePanel.SetActive(false);
        shopPopup.SetActive(false);
        recoveryPopup.SetActive(false);
        if (InventoryPopup.Instance != null)
            InventoryPopup.Instance.OnClickCloseButton(); // 인벤토리 팝업 닫기
        selectChoicePanel.SetActive(false);
        foreach (var characterPlatform in characterPlatforms)
        {
            if (characterPlatform != null)
                characterPlatform.SetActive(false);
        }
        SoundManager.Instance.PlayBGM(SoundManager.SoundType.BGM_Dungeon); // 배틀 배경음악 재생
        // 선택된 스테이지 정보 업데이트
        selectedFloorText.text = $"{StageManager.Instance.stageSaveData.currentStageIndex + 1}층"; // 현재 페이즈 인덱스에 1을 더하여 층수 표시
        // 보스는 페이즈 인덱스 3이하는 가디언, 4는 로드
        if (StageManager.Instance.stageSaveData.currentPhaseIndex < 3)
        {
            EnemyData guardianEnemy = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].Enemies.Find(x => x.Type == EnemyData.EnemyType.Guardian);
            floorBossIcon.sprite = guardianEnemy.EnemyIcon; // 가디언 아이콘 설정
            floorBossElementIcon.sprite = guardianEnemy.GetElementIcon();
            floorBossNameText.text = guardianEnemy.EnemyName; // 가디언 이름 설정
        }
        else if (StageManager.Instance.stageSaveData.currentPhaseIndex >= 4)
        {
            EnemyData lordEnemy = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].Enemies.Find(x => x.Type == EnemyData.EnemyType.Lord);
            floorBossIcon.sprite = lordEnemy.EnemyIcon; // 로드 아이콘 설정
            floorBossElementIcon.sprite = lordEnemy.GetElementIcon();
            floorBossNameText.text = lordEnemy.EnemyName; // 로드 이름 설정
        }
    }

    public void OnClickFloorButton(int stageIndex) // 스테이지 선택 버튼 클릭 시 호출되는 함수
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
            OpenTeamFormationPanel(); // 팀 구성 패널 열기
        }
    }
    public void OpenTeamFormationPanel()
    {
        RefreshTeamFormationButton(); // 팀 구성 버튼 상태 갱신
        PlatformManager.Instance.SetPlatformAndCharacters(); // 플랫폼과 캐릭터를 설정
        //Debug.Log($"[TeamFormation] AcquiredCharacters Count: {CharacterManager.Instance.AcquiredCharacters.Count}");
        StageManager.Instance.stageSaveData.currentPhaseState = StageSaveData.CurrentPhaseState.TeamSelect; // 현재 페이즈 상태를 팀 구성으로 설정
        selectFloorPanel.SetActive(false);
        teamFormationPenel.SetActive(true);
        stagePanel.SetActive(false);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        blessingPanel.SetActive(false);
        cursePanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectChoicePanel.SetActive(false);
        SoundManager.Instance.PlayBGM(SoundManager.SoundType.BGM_Dungeon); // 배틀 배경음악 재생
        foreach (var characterPlatform in characterPlatforms)
        {
            if (characterPlatform != null)
                characterPlatform.SetActive(true);
        }
         shopPopup.SetActive(false);
        recoveryPopup.SetActive(false);
        if (InventoryPopup.Instance != null)
            InventoryPopup.Instance.OnClickCloseButton(); // 인벤토리 팝업 닫기
        BackgroundController.Instance.SetBackground(chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.
            stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].TeamSelect);

        int ownedCount = CharacterManager.Instance.OwnedCharacters.Count;
        Transform buttonParent = characterButtons[0].transform.parent;

        // 부족하면 복제
        while (characterButtons.Count < ownedCount)
        {
            var go = Instantiate(characterButtons[0].gameObject, buttonParent);
            var viewer = go.GetComponent<CharacterViewer>();
            characterButtons.Add(viewer);
        }

        // 넘치면 비활성화
        for (int i = 0; i < characterButtons.Count; i++)
        {
            if (i < ownedCount)
            {
                characterButtons[i].gameObject.SetActive(true);
                var characterSO = CharacterManager.Instance.OwnedCharacters[i].CharacterData;
                characterButtons[i].SetCharacterData(characterSO);
            }
            else
            {
                characterButtons[i].gameObject.SetActive(false);
            }
        }
        OnClickTeamFormationButton((int)StageManager.Instance.stageSaveData.currentFormationType); // 현재 팀 구성 타입에 맞는 버튼 상태 갱신
    }

    public void OnClickCharacterButton(CharacterSO characterData) // 캐릭터 버튼 클릭 시 호출되는 함수
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
        if (characterData == null)
        {
            messagePopup.Open("잘못된 캐릭터 데이터입니다. 다시 시도해 주세요.");
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

        // 이미 선택된 캐릭터면 해제(토글)
        bool wasSelected = false;
        for (int i = 0; i < 5; i++)
        {
            if (StageManager.Instance.stageSaveData.entryCharacters[i] == characterData)
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
                    StageManager.Instance.stageSaveData.entryCharacters[i] = characterData;
                    if (StageManager.Instance.stageSaveData.leaderCharacter == null) // 리더 캐릭터가 설정되지 않았다면 첫 번째 선택된 캐릭터를 리더로 설정
                    {
                        StageManager.Instance.stageSaveData.leaderCharacter = characterData;
                        messagePopup.Open($"[{characterData.nameKr}] 캐릭터가 리더로 설정되었습니다.");
                    }
                    break;
                }
            }
        }
        // 선택된 캐릭터를 월드에 스폰하는 리프레시 함수 호출
        PlatformManager.Instance.SetPlatformAndCharacters(); // 스폰된 캐릭터 갱신
    }

    public void OnClickTeamFormationButton(int formationIndex) // 팀 구성 버튼 클릭 시 호출되는 함수
    {
        selectedTeamFormation = (SelectedTeamFormation)formationIndex; // 선택된 팀 구성 설정
        StageManager.Instance.stageSaveData.currentFormationType = StageSaveData.CurrentFormationType.FormationA + formationIndex; // 현재 팀 구성 타입 설정
        PlatformManager.Instance.SetPlatformAndCharacters(); // 플랫폼과 캐릭터를 설정
        RefreshTeamFormationButton();
    }

    public void OnClickFilterButton()
    {
        messagePopup.Open("미구현된 기능입니다.\n" +
            "추후 업데이트될 예정입니다.");
    }

    public void OnClickSelectLeaderButton()
    {
        // 플랫폼 인덱스를 기반으로 리더를 선정하게 변경
        if (selectedPlatformIndex < 0 || selectedPlatformIndex >= characterPlatforms.Length)
        {
            messagePopup.Open("리더를 선택할 수 없습니다. 플랫폼을 먼저 선택해 주세요.");
            return;
        }
        var selectedCharacter = StageManager.Instance.stageSaveData.entryCharacters[selectedPlatformIndex];
        if (selectedCharacter == null)
        {
            messagePopup.Open("선택한 플랫폼에 캐릭터가 없습니다.");
            return;
        }
        StageManager.Instance.stageSaveData.leaderCharacter = selectedCharacter; // 선택한 캐릭터를 리더로 설정

        // 리더 마크 갱신은 PlatformManager에서 일괄 처리
        PlatformManager.Instance.SetPlatformAndCharacters();

        messagePopup.Open($"[{selectedCharacter.nameKr}] 캐릭터가 리더로 설정되었습니다.");
    }


    private void RefreshPlatformColors(int selectedIndex)
    {
        for (int i = 0; i < characterPlatforms.Length; i++)
        {
            var platformRenderer = characterPlatforms[i].GetComponent<Renderer>();
            if (platformRenderer != null)
            {
                platformRenderer.material.color = (i == selectedIndex) ? platformSelectedColor : platformDefaultColor;
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
            PlatformManager.Instance.DeactivateAllCharacters(); // 모든 캐릭터 비활성화
            switch (StageManager.Instance.stageSaveData.currentPhaseState)
            {
                case StageSaveData.CurrentPhaseState.TeamSelect:
                    StageManager.Instance.stageSaveData.currentPhaseIndex = 0; // 첫 번째 페이즈로 시작
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

    private void RefreshTeamFormationButton() // 팀 구성 버튼 상태를 갱신하는 함수
    {
        for (int i = 0; i < teamFormationIcons.Length; i++)
        {
            teamFormationIcons[i].color = (SelectedTeamFormation)i == selectedTeamFormation ? Color.yellow : Color.white; // 선택된 팀 구성은 노란색, 나머지는 흰색으로 표시
        }
        for (int i = 0; i < characterPlatforms.Length; i++)
        {
            PlatformManager.Instance.SetPlatformAndCharacters(); // 플랫폼과 캐릭터를 설정
        }
    }
    public void OpenStagePanel(int phaseIndex) // 스테이지 패널을 여는 함수
    {
        StageManager.Instance.stageSaveData.currentPhaseState = StageSaveData.CurrentPhaseState.Standby; // 현재 페이즈 상태를 대기 상태로 설정
        // 스테이지 데이터에서 현재 스테이지에 맞는 월드맵 배경을 설정
        if (
    worldMap != null &&
    chapterData != null &&
    chapterData.chapterIndex != null &&
    StageManager.Instance != null &&
    StageManager.Instance.stageSaveData != null &&
    StageManager.Instance.stageSaveData.currentChapterIndex >= 0 &&
    StageManager.Instance.stageSaveData.currentChapterIndex < chapterData.chapterIndex.Count &&
    chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex] != null &&
    chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData != null &&
    chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex != null &&
    StageManager.Instance.stageSaveData.currentStageIndex >= 0 &&
    StageManager.Instance.stageSaveData.currentStageIndex < chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex.Count &&
    chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex] != null &&
    chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].WorldMapBackground != null
)
        {
            worldMap.sprite = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex]
                .stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].WorldMapBackground;
        }
        selectFloorPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(true);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectChoicePanel.SetActive(false);
        blessingPanel.SetActive(false);
        cursePanel.SetActive(false);
        shopPopup.SetActive(false);
        recoveryPopup.SetActive(false);
        if (InventoryPopup.Instance != null)
            InventoryPopup.Instance.OnClickCloseButton(); // 인벤토리 팝업 닫기
        foreach (var characterPlatform in characterPlatforms)
        {
            if (characterPlatform != null)
                characterPlatform.SetActive(false);
        }
        SoundManager.Instance.PlayBGM(SoundManager.SoundType.BGM_Dungeon); // 배틀 배경음악 재생

        StagePanel stagePanelScript = GetComponentInChildren<StagePanel>();
        if (stagePanelScript != null)
        {
            stagePanelScript.UpdateFlags(StageManager.Instance.stageSaveData.currentPhaseIndex);
        }
    }

    public void OpenBattlePanel()
    {
        StageManager.Instance.stageSaveData.currentPhaseState = StageSaveData.CurrentPhaseState.Battle; // 현재 페이즈 상태를 배틀 상태로 설정
        
        selectFloorPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(false);
        battlePanel.SetActive(true);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectChoicePanel.SetActive(false);
        blessingPanel.SetActive(false);
        cursePanel.SetActive(false);
        shopPopup.SetActive(false);
        recoveryPopup.SetActive(false);
        if (InventoryPopup.Instance != null)
            InventoryPopup.Instance.OnClickCloseButton(); // 인벤토리 팝업 닫기
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
        selectedEngraving = null; // 선택된 각인 초기화
        engravingChoices = new EngravingData[3]; // 각인 선택 배열 초기화
        artifactChoices = new ArtifactData[3]; // 아티팩트 선택 배열 초기화
        //예외 상태 스트링 값을 처리하는 스위치
        switch (phaseState)
        {
            case StageSaveData.CurrentPhaseState.StartReward:
            case StageSaveData.CurrentPhaseState.EliteEngravingReward:
                // 각인 선택 UI를 열어야 하는 경우만 break(아래 코드 실행)
                break;
            default:
                Debug.LogError($"잘못된 phase state: {phaseState}");
                return;
        }

        List<EngravingData> allEngravings = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].EngravingList; // 현재 스테이지의 각인 목록을 가져옴
        var owned = StageManager.Instance.stageSaveData.engravings.Where(s => s != null).ToList();
        var availableEngravings = allEngravings.Except(owned).ToList();
        itemTitleText.text = "각인 선택"; // 각인 선택 UI 제목 설정
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

        selectFloorPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        // 스테이지 데이터에서 현재 스테이지에 맞는 월드맵 배경을 설정
        if (
            worldMap != null &&
            chapterData != null &&
            chapterData.chapterIndex != null &&
            StageManager.Instance != null &&
            StageManager.Instance.stageSaveData != null &&
            StageManager.Instance.stageSaveData.currentChapterIndex >= 0 &&
            StageManager.Instance.stageSaveData.currentChapterIndex < chapterData.chapterIndex.Count &&
            chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex] != null &&
            chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData != null &&
            chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex != null &&
            StageManager.Instance.stageSaveData.currentStageIndex >= 0 &&
            StageManager.Instance.stageSaveData.currentStageIndex < chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex.Count &&
            chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex] != null &&
            chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].WorldMapBackground != null
)
        {
            worldMap.sprite = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex]
                .stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].WorldMapBackground;
        }
        stagePanel.SetActive(true);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(true);
        selectChoicePanel.SetActive(false);
        blessingPanel.SetActive(false);
        cursePanel.SetActive(false);
        shopPopup.SetActive(false);
        recoveryPopup.SetActive(false);
        if (InventoryPopup.Instance != null)
            InventoryPopup.Instance.OnClickCloseButton(); // 인벤토리 팝업 닫기
        foreach (var characterPlatform in characterPlatforms)
        {
            if (characterPlatform != null)
                characterPlatform.SetActive(false);
        }
        //각인 선택 패널에선 아티팩트 선택용 UI를 비활성화
        artifactSetEffectText.text = ""; // 아티팩트 세트 효과 텍스트 초기화
        artifactSetEffectToolTip.SetActive(false);
        artifactSetEffectDescription.SetActive(false);
        SoundManager.Instance.PlayBGM(SoundManager.SoundType.BGM_Dungeon); // 배틀 배경음악 재생
        OnClickSelectItemNumber(0); // 첫 번째 아이템을 선택한 것으로 초기화
    }

    public void OpenSelectArtifactPanel(StageSaveData.CurrentPhaseState phaseState) // "NormalReward", "EliteArtifactReward","EliteEngravingReward", "BossReward" 와 연결
    {
        StageManager.Instance.stageSaveData.currentPhaseState = phaseState; // 현재 페이즈 상태를 설정
        selectedArtifact = null; // 선택된 아티팩트 초기화
        selectedEngraving = null; // 선택된 각인 초기화
        engravingChoices = new EngravingData[3]; // 각인 선택 배열 초기화
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
        owned.AddRange(StageManager.Instance.stageSaveData.equipedArtifacts); // 장착된 아티팩트도 소유 목록에 추가
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
            itemChoiceFrame[i].GetComponent<Image>().sprite = candidate.RaritySprite; // 아티팩트의 희귀도에 맞는 프레임 이미지 설정
        }
        selectFloorPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        // 스테이지 데이터에서 현재 스테이지에 맞는 월드맵 배경을 설정
        if (
            worldMap != null &&
            chapterData != null &&
            chapterData.chapterIndex != null &&
            StageManager.Instance != null &&
            StageManager.Instance.stageSaveData != null &&
            StageManager.Instance.stageSaveData.currentChapterIndex >= 0 &&
            StageManager.Instance.stageSaveData.currentChapterIndex < chapterData.chapterIndex.Count &&
            chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex] != null &&
            chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData != null &&
            chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex != null &&
            StageManager.Instance.stageSaveData.currentStageIndex >= 0 &&
            StageManager.Instance.stageSaveData.currentStageIndex < chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex.Count &&
            chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex] != null &&
            chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].WorldMapBackground != null
)
        {
            worldMap.sprite = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex]
                .stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].WorldMapBackground;
        }
        stagePanel.SetActive(true);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(true);
        selectChoicePanel.SetActive(false);
        shopPopup.SetActive(false);
        blessingPanel.SetActive(false);
        cursePanel.SetActive(false);
        recoveryPopup.SetActive(false);
        if (InventoryPopup.Instance != null)
            InventoryPopup.Instance.OnClickCloseButton(); // 인벤토리 팝업 닫기
        foreach (var characterPlatform in characterPlatforms)
        {
            if (characterPlatform != null)
                characterPlatform.SetActive(false); // 캐릭터 플랫폼 비활성화
        }
        // 아티팩트 선택 패널에선 아티팩트용 UI를 활성화
        artifactSetEffectText.text = ""; // 아티팩트 세트 효과 텍스트 초기화
        artifactSetEffectToolTip.SetActive(true); // 아티팩트 세트 효과 설명 활성화
        artifactSetEffectDescription.SetActive(true); // 아티팩트 세트 효과 설명 활성화
        SoundManager.Instance.PlayBGM(SoundManager.SoundType.BGM_Dungeon); // 배틀 배경음악 재생
        OnClickSelectItemNumber(0); // 첫 번째 아이템을 선택한 것으로 초기화
    }

    public void OnClickSelectItemNumber(int selectIndex) // 아티팩트 패널과 각인 패널 둘 다 다루니 아이템 패널이라고 함
    {
        this.selectIndex = selectIndex; // 선택된 아이템 인덱스 설정
        if (selectIndex < 0 || selectIndex >= 3)
        {
            messagePopup.Open("잘못된 선택입니다. 다시 시도해 주세요.");
            return;
        }
        
        for (int i = 0; i < itemChoiceFrame.Length; i++)
        {
            if (itemChoiceFrame[i].GetComponent<CanvasGroup>() == null)
            {
                itemChoiceFrame[i].gameObject.AddComponent<CanvasGroup>(); // CanvasGroup이 없으면 추가
            }
            var canvasGroup = itemChoiceFrame[i].GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = (i == selectIndex) ? 1f : 0.5f; // 선택된 아이콘은 1, 나머지는 0.5로 설정
            }
        }
        switch (StageManager.Instance.stageSaveData.currentPhaseState)
        {
            case StageSaveData.CurrentPhaseState.StartReward:
            case StageSaveData.CurrentPhaseState.EliteEngravingReward:
                selectedEngraving = engravingChoices[selectIndex];
                itemNameText.text = selectedEngraving.EngravingName; // 선택된 각인 이름 설정
                artifactSetEffectText.text = ""; // 아티팩트 세트 효과 텍스트 초기화
                itemDescriptionText.text = selectedEngraving.Description; // 선택된 각인 설명 설정
                break;
            case StageSaveData.CurrentPhaseState.NormalReward:
            case StageSaveData.CurrentPhaseState.EliteArtifactReward:
            case StageSaveData.CurrentPhaseState.BossReward:
                selectedArtifact = artifactChoices[selectIndex];
                itemNameText.text = selectedArtifact.ArtifactName; // 선택된 아티팩트 이름 설정
                // 중복 제거를 위해 HashSet 사용
                var effectNames = new HashSet<string>();
                artifactSetEffectText.text = "";
                foreach (var effect in selectedArtifact.SetEffectData)
                {
                    if (effect != null && effectNames.Add(effect.EffectName))
                    {
                        artifactSetEffectText.text += $"#{effect.EffectName} ";
                    }
                }
                itemDescriptionText.text = selectedArtifact.Description; // 선택된 아티팩트 설명 설정
                break;
            default:
                messagePopup.Open("잘못된 페이즈 상태입니다. 다시 시도해 주세요.");
                return;
        }
    }

    public void OnClickSelectItem() // 아티팩트 패널과 각인 패널 둘 다 다루니 아이템 패널이라고 함
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
                OpenSelectEngravingPanel(StageSaveData.CurrentPhaseState.EliteEngravingReward); // 엘리트 아티팩트 리워드 페이즈에서는 각인 선택 패널을 열도록 함
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
        if(StageManager.Instance.stageSaveData.currentPhaseState == StageSaveData.CurrentPhaseState.Standby) // 현재 페이즈 상태가 대기 상태일 때
        {
            if (StageManager.Instance.stageSaveData.currentPhaseIndex < 4) // 페이즈0-3 까지는 선택지 패널을 열고 그 후 배틀 룸 입장
            {
                OpenSelectChoicePanel(); // 선택지 이벤트 패널 열기
            }
            else if (StageManager.Instance.stageSaveData.currentPhaseIndex == 4) // 페이즈 4는 선택지 대신 상점을 염
            {
                OpenShopPopup(); // 상점 패널 열기
            }
            else if (StageManager.Instance.stageSaveData.currentPhaseIndex == 5) // 페이즈 5는 보스 룸
            {
                messagePopup.Open("보스가 등장했습니다! 입장하시겠습니까?",
                () => StageManager.Instance.selectBossEnemy(),
                () => messagePopup.Close());
            }
            else // 페이즈 인덱스가 범위를 벗어난 경우
            {
                messagePopup.Open("잘못된 페이즈 인덱스입니다. 다시 시도해 주세요.");
            }
        }
        else if (StageManager.Instance.stageSaveData.currentPhaseState == StageSaveData.CurrentPhaseState.EquipmentArtifact)
        {
            InventoryPopup.Instance.OnClickInventoryButton();
        }
    }

    public void OpenSelectChoicePanel() // 선택지 선택지 패널을 여는 함수
    {
        StageManager.Instance.stageSaveData.currentPhaseState = StageSaveData.CurrentPhaseState.SelectChoice; // 현재 페이즈 상태를 선택지로 설정
        var stageSelectChoices = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex]
            .stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].ChoiceOptions; // 현재 스테이지의 선택지 목록을 가져옴
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

        selectFloorPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        // 스테이지 데이터에서 현재 스테이지에 맞는 월드맵 배경을 설정
        if (
    worldMap != null &&
    chapterData != null &&
    chapterData.chapterIndex != null &&
    StageManager.Instance != null &&
    StageManager.Instance.stageSaveData != null &&
    StageManager.Instance.stageSaveData.currentChapterIndex >= 0 &&
    StageManager.Instance.stageSaveData.currentChapterIndex < chapterData.chapterIndex.Count &&
    chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex] != null &&
    chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData != null &&
    chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex != null &&
    StageManager.Instance.stageSaveData.currentStageIndex >= 0 &&
    StageManager.Instance.stageSaveData.currentStageIndex < chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex.Count &&
    chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex] != null &&
    chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].WorldMapBackground != null
)
        {
            worldMap.sprite = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex]
                .stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].WorldMapBackground;
        }
        stagePanel.SetActive(true);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        blessingPanel.SetActive(false);
        cursePanel.SetActive(false);
        shopPopup.SetActive(false);
        recoveryPopup.SetActive(false);
        if (InventoryPopup.Instance != null)
            InventoryPopup.Instance.OnClickCloseButton(); // 인벤토리 팝업 닫기
        selectChoicePanel.SetActive(true); // 선택지 이벤트 패널 활성화
        foreach (var characterPlatform in characterPlatforms)
        {
            if (characterPlatform != null)
                characterPlatform.SetActive(false);
        }
        SoundManager.Instance.PlayBGM(SoundManager.SoundType.BGM_Dungeon); // 배틀 배경음악 재생
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
                //messagePopup.Open("이벤트는 아직 구현이 안되었습니다");
                OpenEventPanel(); // 이벤트 패널 열기
                return;
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

    public void OpenEventPanel() // 이벤트 패널을 여는 함수 이벤트 패널의 종류는 축복과 저주 중 하나이므로 그것을 결정 후 그에 맞는 패널을 염
    {
        //페이즈 스테이트가 축복또는 저주가 아닐 경우 랜덤으로 둘 중 하나 결정
        if(StageManager.Instance.stageSaveData.currentPhaseState != StageSaveData.CurrentPhaseState.BlessingEvent &&
           StageManager.Instance.stageSaveData.currentPhaseState != StageSaveData.CurrentPhaseState.CurseEvent)
        {
            StageManager.Instance.stageSaveData.currentPhaseState = Random.Range(0, 2) == 0 ? 
                StageSaveData.CurrentPhaseState.BlessingEvent : StageSaveData.CurrentPhaseState.CurseEvent;
            StageManager.Instance.stageSaveData.UpOrDown = 0; // 업 또는 다운 초기화
        }
        blessingPanel.SetActive(StageManager.Instance.stageSaveData.currentPhaseState == StageSaveData.CurrentPhaseState.BlessingEvent); // 축복 패널 활성화
        cursePanel.SetActive(StageManager.Instance.stageSaveData.currentPhaseState == StageSaveData.CurrentPhaseState.CurseEvent); // 저주 패널 활성화
        selectFloorPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(false);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectChoicePanel.SetActive(false);
        shopPopup.SetActive(false);
        recoveryPopup.SetActive(false);
        if (InventoryPopup.Instance != null)
            InventoryPopup.Instance.OnClickCloseButton(); // 인벤토리 팝업 닫기
        foreach (var characterPlatform in characterPlatforms)
            {
            if (characterPlatform != null)
                characterPlatform.SetActive(false); // 캐릭터 플랫폼 비활성화
        }
    }

    public void OpenVictoryPanel() // 승리 패널을 여는 함수
    {
        DanceTime.Dance(true);
        StageManager.Instance.stageSaveData.currentPhaseState = StageSaveData.CurrentPhaseState.Battle; // 현재 페이즈 상태를 승리로 설정
        selectFloorPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(false);
        battlePanel.SetActive(true);
        victoryPanel.SetActive(true); // 승리 패널 활성화
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectChoicePanel.SetActive(false);
        blessingPanel.SetActive(false);
        cursePanel.SetActive(false);
        shopPopup.SetActive(false);
        recoveryPopup.SetActive(false);
        if (InventoryPopup.Instance != null)
            InventoryPopup.Instance.OnClickCloseButton(); // 인벤토리 팝업 닫기
        foreach (var characterPlatform in characterPlatforms)
        {
            if (characterPlatform != null)
                characterPlatform.SetActive(false);
        }
        SoundManager.Instance.PlayOneShotBGM(SoundManager.SoundType.BGM_Victory); // 승리 효과음 재생
    }

    public void OpenDefeatPanel() // 패배 패널을 여는 함수
    {
        DanceTime.Dance(false);
        StageManager.Instance.stageSaveData.currentPhaseState = StageSaveData.CurrentPhaseState.Battle; // 현재 페이즈 상태를 패배로 설정
        selectFloorPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        stagePanel.SetActive(false);
        battlePanel.SetActive(true);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(true); // 패배 패널 활성화
        selectItemPanel.SetActive(false);
        selectChoicePanel.SetActive(false);
        blessingPanel.SetActive(false);
        cursePanel.SetActive(false);
        shopPopup.SetActive(false);
        recoveryPopup.SetActive(false);
        if (InventoryPopup.Instance != null)
            InventoryPopup.Instance.OnClickCloseButton(); // 인벤토리 팝업 닫기
        foreach (var characterPlatform in characterPlatforms)
        {
            if (characterPlatform != null)
                characterPlatform.SetActive(false);
        }
        SoundManager.Instance.PlayOneShotBGM(SoundManager.SoundType.BGM_Defeat); // 패배 효과음 재생
    }

    public void OnClickVictoryNextButton() // 승리 패널에서 다음 버튼 클릭 시 호출되는 함수
    {
        DanceTime.StopDance();
        StageManager.Instance.RoomClear(StageManager.Instance.stageSaveData.selectedEnemy); // 현재 룸 클리어 처리
    }

    public void OnClickDefeatNextButton() // 패배 패널에서 다음 버튼 클릭 시 호출되는 함수
    {
        DanceTime.StopDance();
        StageManager.Instance.EndChapterEarly(StageManager.Instance.stageSaveData.currentChapterIndex); // 현재 챕터 패배 처리
    }

    public void OpenShopPopup()
    {
        StageManager.Instance.stageSaveData.currentPhaseState = StageSaveData.CurrentPhaseState.Shop; // 현재 페이즈 상태를 상점으로 설정
        selectFloorPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        // 스테이지 데이터에서 현재 스테이지에 맞는 월드맵 배경을 설정
        if (
    worldMap != null &&
    chapterData != null &&
    chapterData.chapterIndex != null &&
    StageManager.Instance != null &&
    StageManager.Instance.stageSaveData != null &&
    StageManager.Instance.stageSaveData.currentChapterIndex >= 0 &&
    StageManager.Instance.stageSaveData.currentChapterIndex < chapterData.chapterIndex.Count &&
    chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex] != null &&
    chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData != null &&
    chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex != null &&
    StageManager.Instance.stageSaveData.currentStageIndex >= 0 &&
    StageManager.Instance.stageSaveData.currentStageIndex < chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex.Count &&
    chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex] != null &&
    chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].WorldMapBackground != null
)
        {
            worldMap.sprite = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex]
                .stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].WorldMapBackground;
        }
        stagePanel.SetActive(true);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectChoicePanel.SetActive(false);
        blessingPanel.SetActive(false);
        cursePanel.SetActive(false);
        shopPopup.SetActive(true);
        recoveryPopup.SetActive(false);
        if (InventoryPopup.Instance != null)
            InventoryPopup.Instance.OnClickCloseButton(); // 인벤토리 팝업 닫기
        foreach (var characterPlatform in characterPlatforms)
        {
            if (characterPlatform != null)
                characterPlatform.SetActive(false);
        }
        SoundManager.Instance.PlayBGM(SoundManager.SoundType.BGM_Dungeon); // 배틀 배경음악 재생
    }

    public void OpenSelectEquipedArtifactPanel() // 아티팩트 장착 선택 패널을 여는 함수
    {
        StageManager.Instance.stageSaveData.currentPhaseState = StageSaveData.CurrentPhaseState.EquipmentArtifact; // 현재 페이즈 상태를 "EquipmentArtifact"로 설정
        selectFloorPanel.SetActive(false);
        teamFormationPenel.SetActive(false);
        // 스테이지 데이터에서 현재 스테이지에 맞는 월드맵 배경을 설정
        if (
    worldMap != null &&
    chapterData != null &&
    chapterData.chapterIndex != null &&
    StageManager.Instance != null &&
    StageManager.Instance.stageSaveData != null &&
    StageManager.Instance.stageSaveData.currentChapterIndex >= 0 &&
    StageManager.Instance.stageSaveData.currentChapterIndex < chapterData.chapterIndex.Count &&
    chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex] != null &&
    chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData != null &&
    chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex != null &&
    StageManager.Instance.stageSaveData.currentStageIndex >= 0 &&
    StageManager.Instance.stageSaveData.currentStageIndex < chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex.Count &&
    chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex] != null &&
    chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex].stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].WorldMapBackground != null
)
        {
            worldMap.sprite = chapterData.chapterIndex[StageManager.Instance.stageSaveData.currentChapterIndex]
                .stageData.stageIndex[StageManager.Instance.stageSaveData.currentStageIndex].WorldMapBackground;
        }
        stagePanel.SetActive(true);
        battlePanel.SetActive(false);
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        selectItemPanel.SetActive(false);
        selectChoicePanel.SetActive(false);
        blessingPanel.SetActive(false);
        cursePanel.SetActive(false);
        shopPopup.SetActive(false);
        recoveryPopup.SetActive(false);

        foreach (var characterPlatform in characterPlatforms)
            {
                if (characterPlatform != null)
                    characterPlatform.SetActive(false);
            }
        SoundManager.Instance.PlayBGM(SoundManager.SoundType.BGM_Dungeon); // 배틀 배경음악 재생
    }
}