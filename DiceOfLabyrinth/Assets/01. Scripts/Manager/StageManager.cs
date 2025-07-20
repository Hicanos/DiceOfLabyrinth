using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class StageSaveData
{
    public enum CurrentFormationType // 포메이션 타입
    {
        FormationA,
        FormationB,
        FormationC,
        FormationD,
    }
    public enum CurrentPhaseState
    {
        None,
        TeamSelect,
        StartReward,
        SelectChoice,
        Standby,
        Battle,
        NormalReward,
        EliteArtifactReward,
        EliteStagmaReward,
        BossReward,
        Shop,
        EquipmedArtifact
    }
    [Header("Stage Save Data")]
    public int currentChapterIndex; // 현재 챕터 인덱스
    public int currentStageIndex; // 현재 스테이지 인덱스
    public int currentPhaseIndex; // 현재 페이즈 인덱스
    public CurrentFormationType currentFormationType;
    public int normalStageCompleteCount; // 현재 스테이지의 노멀 룸 완료 개수, 챕터가 완료되면 초기화됩니다.
    public int eliteStageCompleteCount; // 현재 스테이지의 엘리트 룸 완료 개수, 챕터가 완료되면 초기화됩니다.
    public CurrentPhaseState currentPhaseState; // 현재 페이즈 상태

    [Header("Stage Resources")]
    public int manaStone; // 스테이지 내에서만 쓰이는 재화, 스테이지를 벗어나면 초기화됩니다.
    public List<ArtifactData> artifacts = new List<ArtifactData>(12);// 아티팩트 목록, 스테이지 내에서만 쓰이는 재화, 스테이지를 벗어나면 초기화됩니다.
    public List<StagmaData> stagmas = new List<StagmaData>(3); // 최대 3개 제한, 스태그마 목록, 스테이지 내에서만 쓰이는 재화, 스테이지를 벗어나면 초기화됩니다.
    public List<ArtifactData> equipedArtifacts = new List<ArtifactData>(4); // 현재 장착된 아티팩트 목록

    [Header("Stage Characters")]
    public List<CharacterSO> entryCharacters = new List<CharacterSO>(5); // 플레이어 캐릭터 목록, 플레이어 보유 캐릭터 중 5명을 선택하여 스테이지에 진입합니다.
    public CharacterSO leaderCharacter; // 리더 캐릭터, 스테이지에 진입할 때 선택한 캐릭터 중 하나를 리더로 설정합니다.
    public List<BattleCharacter> battleCharacters = new List<BattleCharacter>(5); // 전투에 참여하는 캐릭터 목록, 탐험 버튼을 누를 때 엔트리 캐릭터 목록의 캐릭터들이 할당됩니다.
    [Header("Selected Enemy")]
    public EnemyData selectedEnemy; // 선택된 적, 스테이지에 진입할 때 선택한 적을 저장합니다. 현재는 스테이지에 진입할 때마다 초기화됩니다.
    [Header("Stage Rewards")]
    public int savedExpReward; // 스테이지에서 획득한 경험치 보상, 스테이지 종료시 정산합니다.
    public int savedGoldReward; // 스테이지에서 획득한 골드 보상, 스테이지 종료시 정산합니다.
    public int savedJewelReward; // 스테이지에서 획득한 보석 보상, 스테이지 종료시 정산합니다.


    public List<ChapterStates> chapterStates = new List<ChapterStates>();

    public void ResetToDefault(int chapterIndex) //챕터 단위 초기화
    {
        currentChapterIndex = chapterIndex;
        currentStageIndex = 0;
        while(equipedArtifacts.Count < 4) // 아티팩트 장착 슬롯 크기를 4로 고정
            equipedArtifacts.Add(null);
        while(equipedArtifacts.Count > 4)
            equipedArtifacts.RemoveAt(equipedArtifacts.Count - 1); // 아티팩트 장착 슬롯 크기를 4로 고정
        for (int i = 0; i < 4; i++) // 아티팩트 장착 슬롯 초기화
            equipedArtifacts[i] = null;
        savedExpReward = 0;
        savedGoldReward = 0;
        savedJewelReward = 0;
        // 챕터 단위로만 초기화할 데이터가 있다면 여기에 추가

        ResetStageProgress(); // 스테이지 관련 데이터 일괄 초기화
    }
    public void ResetStageProgress() // 스테이지 진행중에만 사용하는 데이터 초기화
    {
        currentPhaseIndex = 0;
        currentFormationType = CurrentFormationType.FormationA;
        currentPhaseState = CurrentPhaseState.None;
        manaStone = 0;
        while (artifacts.Count < 12) // 아티팩트 목록 크기를 12로 고정
            artifacts.Add(null);
        while (artifacts.Count > 12)
            artifacts.RemoveAt(artifacts.Count - 1); // 아티팩트 목록 크기를 12로 고정
        while (stagmas.Count < 3) // 스태그마 목록 크기를 3으로 고정
            stagmas.Add(null);
        while (stagmas.Count > 3)
            stagmas.RemoveAt(stagmas.Count - 1); // 스태그마 목록 크기를 3으로 고정
        while (entryCharacters.Count < 5) // 엔트리 캐릭터 목록 크기를 5로 고정
            entryCharacters.Add(null);
        while (entryCharacters.Count > 5)
            entryCharacters.RemoveAt(entryCharacters.Count - 1); // 엔트리 캐릭터 목록 크기를 5로 고정
        while (battleCharacters.Count < 5) // 전투 캐릭터 목록 크기를 5로 고정
            battleCharacters.Add(null);
        while (battleCharacters.Count > 5)
            battleCharacters.RemoveAt(battleCharacters.Count - 1); // 전투 캐릭터 목록 크기를 5로 고정
        for (int i = 0; i < 12; i++)
            artifacts[i] = null;
        for (int i = 0; i < 3; i++)
            stagmas[i] = null;
        for (int i = 0; i < entryCharacters.Count; i++)
            entryCharacters[i] = null;
        leaderCharacter = null;
        for (int i = 0; i < battleCharacters.Count; i++)
            battleCharacters[i] = null;
        selectedEnemy = null;
        normalStageCompleteCount = 0;
        eliteStageCompleteCount = 0;
    }
}

[System.Serializable]
public class ChapterStates
{
    public bool isCompleted;
    public bool isUnLocked;
    //public List<StageState> stageStates = new List<StageState>(); // 각 스테이지의 상태를 저장하는 리스트
}

//[System.Serializable]
//public class StageState
//{
//    public bool isCompleted;
//    public bool isUnLocked;
//}
public class StageManager : MonoBehaviour
{
    public ChapterData chapterData; // ChapterData 스크립터블 오브젝트, 에디터에서 할당해야 합니다.
    public StageSaveData stageSaveData; // 스테이지 저장 데이터, 스테이지 시작 시 초기화됩니다.
    public BattleUIController battleUIController; // 배틀 UI 컨트롤러, 스테이지 시작 시 초기화됩니다.
    public MessagePopup messagePopup; // 체크 패널, 챕터가 잠겨있을 때 팝업을 띄우기 위해 사용합니다.

    public static StageManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 이 오브젝트를 씬 전환 시 파괴되지 않도록 설정
            InitializeStageStates(chapterData);
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 중복 생성 방지
        }

        if (chapterData == null)
        {
            Debug.LogError("ChapterData is not assigned in StageManager. Please assign it in the inspector.");
        }
    }

    public void RestoreStageState()//배틀씬에 입장시 세이브 데이터에 따라 진행도를 복원하고 UI 컨트롤러에 알려줍니다.
    {
        if (SceneManager.GetActiveScene().name != "BattleScene")
        {
            SceneManagerEx.Instance.LoadScene("BattleScene"); // 배틀 씬으로 이동
        }
        if (battleUIController == null)
        {
            battleUIController = FindAnyObjectByType<BattleUIController>();
            if (battleUIController == null)
            {
                Debug.LogWarning("BattleUIController를 BattleScene에서 찾을 수 없습니다.");
            }
        }

        if (stageSaveData == null)
        {
            Debug.LogError("StageSaveData가 할당되지 않았습니다. 스테이지 데이터를 초기화해주세요.");
            return;
        }
        else if (stageSaveData.currentChapterIndex < -1 || stageSaveData.currentChapterIndex >= chapterData.chapterIndex.Count)
        {
            Debug.LogError("현재의 챕터 인덱스가 유효하지 않습니다. 챕터 데이터를 확인해주세요.");
            return;
        }
        else if (stageSaveData.currentStageIndex < -1 || stageSaveData.currentStageIndex > 4) // 스테이지 인덱스가 0~4 범위를 벗어나는 경우, -1은 스테이지가 선택되지 않은 상태를 의미합니다.
        {
            Debug.LogError("현재의 스테이지 인덱스가 유효하지 않습니다. 스테이지 데이터를 확인해주세요.");
            return;
        }
        else if (stageSaveData.currentPhaseIndex < 0 || stageSaveData.currentPhaseIndex > 4) // 페이즈 인덱스가 0~4 범위를 벗어나는 경우
        {
            Debug.LogError("현재의 페이즈 인덱스가 유효하지 않습니다. 페이즈 데이터를 확인해주세요.");
            return;
        }
        else if (stageSaveData.currentStageIndex == -1 || stageSaveData.currentPhaseState == StageSaveData.CurrentPhaseState.None) // 던전 선택 상태
        {
            battleUIController.OpenSelectDungeonPanel(); // 스테이지 선택 UI를 엽니다.
            return;
        }
        else if (stageSaveData.currentPhaseIndex >= 0 || stageSaveData.currentPhaseIndex <= 4) // 현재 선택지 상태가 비어있지 않은 경우
                                                                                               // "StartReward", "NormalReward", "SelectChoice", "EliteArtifactReward", "EliteStagmaReward", "BossReward", "Shop" , "TeamSelect",  "Standby", "Battle" 중 하나
        {
            switch (stageSaveData.currentPhaseState)
            {
                case StageSaveData.CurrentPhaseState.TeamSelect:
                    battleUIController.OpenTeamFormationPanel(); // 팀 선택 UI를 엽니다.
                    return;
                case StageSaveData.CurrentPhaseState.StartReward:
                    battleUIController.OpenSelectStagmaPanel(StageSaveData.CurrentPhaseState.StartReward);
                    return;
                case StageSaveData.CurrentPhaseState.NormalReward:
                    battleUIController.OpenSelectArtifactPanel(StageSaveData.CurrentPhaseState.NormalReward); // 노멀 리워드 상태에 해당하는 아티팩트 선택 UI를 엽니다.
                    return;
                case StageSaveData.CurrentPhaseState.EliteArtifactReward:
                    battleUIController.OpenSelectStagmaPanel(StageSaveData.CurrentPhaseState.EliteArtifactReward); // 엘리트 아티팩트 리워드 상태에 해당하는 스태그마 선택 UI를 엽니다.
                    return;
                case StageSaveData.CurrentPhaseState.EliteStagmaReward:
                    battleUIController.OpenSelectArtifactPanel(StageSaveData.CurrentPhaseState.EliteStagmaReward); // 엘리트 스태그마 리워드 상태에 해당하는 아티팩트 선택 UI를 엽니다.
                    return;
                case StageSaveData.CurrentPhaseState.BossReward:
                    battleUIController.OpenSelectStagmaPanel(StageSaveData.CurrentPhaseState.BossReward); // 보스 리워드 상태에 해당하는 아티팩트 선택 UI를 엽니다.
                    return;
                case StageSaveData.CurrentPhaseState.Standby:
                    battleUIController.OpenStagePanel(stageSaveData.currentPhaseIndex); // 스탠바이 상태에 해당하는 UI를 엽니다.
                    return;
                case StageSaveData.CurrentPhaseState.Battle:
                    battleUIController.OpenStagePanel(stageSaveData.currentPhaseIndex); // 배틀 중이었어도 복구시엔 스테이지 패널을 엽니다.
                    return;
                case StageSaveData.CurrentPhaseState.Shop:
                    // 상점 상태에 해당하는 UI를 엽니다.
                    return;
                case StageSaveData.CurrentPhaseState.EquipmedArtifact:
                    battleUIController.OpenSelectEquipedArtifactPanel(); // 아티팩트 장착 상태에 해당하는 UI를 엽니다.
                    return;
                case StageSaveData.CurrentPhaseState.SelectChoice:
                    battleUIController.OpenSelectChoicePanel(); // 선택지 상태에 해당하는 UI를 엽니다.
                    return;

                default:
                    Debug.LogError($"Unknown choice state: {stageSaveData.currentPhaseState}");
                    return;
            }
        }
    }

    public void AddStagma(StagmaData stagmaName)
    {
        // 리스트 크기를 3으로 고정
        while (stageSaveData.stagmas.Count < 3)
            stageSaveData.stagmas.Add(null);
        while (stageSaveData.stagmas.Count > 3)
            stageSaveData.stagmas.RemoveAt(stageSaveData.stagmas.Count - 1);

        // 이미 보유 중인지 체크
        for (int i = 0; i < 3; i++)
        {
            if (stageSaveData.stagmas[i] == stagmaName)
            {
                messagePopup.Open($"각인 {stagmaName.StagmaName}은(는) 이미 목록에 있습니다.");
                return;
            }
        }

        // 빈 슬롯(null) 찾아서 추가
        for (int i = 0; i < 3; i++)
        {
            if (stageSaveData.stagmas[i] == null)
            {
                stageSaveData.stagmas[i] = stagmaName;
                messagePopup.Open($"각인 {stagmaName.StagmaName}이(가) 추가되었습니다.");
                return;
            }
        }

        // 모두 차 있으면 안내
        messagePopup.Open("최대 3개의 각인을 소지할 수 있습니다. 더 이상 추가할 수 없습니다.");
    }

    public void AddArtifacts(ArtifactData artifactName)
    {
        // 리스트 크기를 12로 고정
        while (stageSaveData.artifacts.Count < 12)
            stageSaveData.artifacts.Add(null);
        while (stageSaveData.artifacts.Count > 12)
            stageSaveData.artifacts.RemoveAt(stageSaveData.artifacts.Count - 1);

        // 이미 보유 중인지 체크
        for (int i = 0; i < 12; i++)
        {
            if (stageSaveData.artifacts[i] == artifactName)
            {
                messagePopup.Open($"아티팩트 {artifactName.ArtifactName}은(는) 이미 목록에 있습니다.");
                return;
            }
        }

        // 빈 슬롯(null) 찾아서 추가
        for (int i = 0; i < 12; i++)
        {
            if (stageSaveData.artifacts[i] == null)
            {
                stageSaveData.artifacts[i] = artifactName;
                Debug.Log($"Artifact {artifactName} added.");
                messagePopup.Open($"아티팩트 {artifactName.ArtifactName}이(가) 추가되었습니다.");
                return;
            }
        }

        // 모두 차 있으면 안내
        messagePopup.Open("최대 12개의 아티팩트를 소지할 수 있습니다. 더 이상 추가할 수 없습니다.");
    }

    public void EquipArtifacts(ArtifactData artifactName)
    {
        // 리스트 크기를 4로 고정
        while (stageSaveData.equipedArtifacts.Count < 4)
            stageSaveData.equipedArtifacts.Add(null);
        while (stageSaveData.equipedArtifacts.Count > 4)
            stageSaveData.equipedArtifacts.RemoveAt(stageSaveData.equipedArtifacts.Count - 1);

        // 이미 장착 중인지 체크
        for (int i = 0; i < 4; i++)
        {
            if (stageSaveData.equipedArtifacts[i] == artifactName)
            {
                messagePopup.Open($"아티팩트 {artifactName.ArtifactName}은(는) 이미 장착되어 있습니다.");
                return;
            }
        }

        // 소지품에 있는지 체크
        if (!stageSaveData.artifacts.Contains(artifactName))
        {
            messagePopup.Open($"아티팩트 {artifactName.ArtifactName}이(가) 소지품에 없습니다.");
            return;
        }

        // 빈 슬롯(null) 찾아서 장착
        for (int i = 0; i < 4; i++)
        {
            if (stageSaveData.equipedArtifacts[i] == null)
            {
                stageSaveData.equipedArtifacts[i] = artifactName;
                for (int j = 0; j < stageSaveData.artifacts.Count; j++)
                {
                    if (stageSaveData.artifacts[j] == artifactName)
                    {
                        stageSaveData.artifacts[j] = null;
                        break;
                    }
                }
                Debug.Log($"Artifact {artifactName} equipped in slot {i}.");
                // 아티팩트 장착 UI 업데이트 메서드를 호출할 예정입니다.
                return;
            }
        }

        // 모두 차 있으면 안내
        messagePopup.Open("최대 4개의 아티팩트를 장착할 수 있습니다. 더 이상 장착할 수 없습니다.");
    }
    public void RoomClear(EnemyData enemyData)// 스테이지 내의 룸 클리어 로직을 구현합니다.
    {
        // 룸 클리어 로직을 구현합니다.
        var type = enemyData.Type; // 적의 타입을 가져옵니다.
        switch (type)
        {
            case EnemyData.EnemyType.Normal:
                stageSaveData.normalStageCompleteCount++; // 노멀 룸 완료 개수 증가
                battleUIController.OpenSelectArtifactPanel(StageSaveData.CurrentPhaseState.NormalReward); // 노멀 룸 클리어 시 아티팩트 선택 UI를 엽니다.
                break;
            case EnemyData.EnemyType.Elite:
                stageSaveData.eliteStageCompleteCount++; // 엘리트 룸 완료 개수 증가
                battleUIController.OpenSelectArtifactPanel(StageSaveData.CurrentPhaseState.EliteArtifactReward); // 엘리트 룸 클리어 시 아티팩트 선택 UI를 엽니다.
                break;
            case EnemyData.EnemyType.Guardian:
                battleUIController.OpenSelectArtifactPanel(StageSaveData.CurrentPhaseState.BossReward); // 보스 룸 클리어 시 아티팩트 선택 UI를 엽니다.
                break;
            case EnemyData.EnemyType.Lord:
                CompleteChapter(stageSaveData.currentChapterIndex); // 챕터 완료 로직을 호출합니다.
                break;
            default:
                Debug.LogError($"Unknown enemy type: {type}");
                return;
        }
    }
    public void OnBattleResult(BattleResultData result)
    {
        // 배틀 종료 후 아군 상태(체력 등) 반영
        stageSaveData.battleCharacters = result.battleCharacters;

        if (result.isVictory)
        {
            battleUIController.OpenVictoryPanel(); // 승리 패널을 엽니다.
        }
        else
        {
            StageDefeat(stageSaveData.currentStageIndex);
        }
    }
    public void StageComplete(int stageIndex)
    {
        // 스테이지 종료 로직을 구현합니다.

        Debug.Log($"Stage {stageIndex} cleared!");
        //StageManager.Instance.stageSaveData.chapterStates[StageManager.Instance.stageSaveData.currentChapterIndex].stageStates[stageIndex].isCompleted = true; // 스테이지 완료 상태 업데이트
        if (stageIndex < chapterData.chapterIndex[stageSaveData.currentChapterIndex].stageData.stageIndex.Count - 1) // 다음 스테이지가 있다면
        {
            stageSaveData.savedExpReward += chapterData.chapterIndex[stageSaveData.currentChapterIndex].stageData.stageIndex[stageIndex].ExpReward; // 경험치 보상 저장
            stageSaveData.savedGoldReward += chapterData.chapterIndex[stageSaveData.currentChapterIndex].stageData.stageIndex[stageIndex].GoldReward; // 골드 보상 저장
            stageSaveData.savedJewelReward += chapterData.chapterIndex[stageSaveData.currentChapterIndex].stageData.stageIndex[stageIndex].JewelReward; // 보석 보상 저장
            stageSaveData.currentStageIndex = stageIndex + 1; // 다음 스테이지로 진행
            //StageManager.Instance.stageSaveData.chapterStates[StageManager.Instance.stageSaveData.currentChapterIndex].stageStates[stageIndex].isUnLocked = true; // 다음 스테이지 잠금 해제
            stageSaveData.ResetStageProgress(); // 스테이지 진행 상태 초기화
            battleUIController.OpenSelectDungeonPanel(); // 스테이지 선택 UI를 엽니다.
        }
        else
        {
            stageSaveData.savedExpReward += chapterData.chapterIndex[stageSaveData.currentChapterIndex].stageData.stageIndex[stageIndex].ExpReward; // 마지막 스테이지의 경험치 보상 저장
            stageSaveData.savedGoldReward += chapterData.chapterIndex[stageSaveData.currentChapterIndex].stageData.stageIndex[stageIndex].GoldReward; // 마지막 스테이지의 골드 보상 저장
            stageSaveData.savedJewelReward += chapterData.chapterIndex[stageSaveData.currentChapterIndex].stageData.stageIndex[stageIndex].JewelReward; // 마지막 스테이지의 보석 보상 저장
            CompleteChapter(stageSaveData.currentChapterIndex); // 챕터 완료 로직 호출
        }
    }
    public void StageDefeat(int stageIndex)
    {
        stageSaveData.savedExpReward += chapterData.chapterIndex[stageSaveData.currentChapterIndex].stageData.stageIndex[stageIndex].ExpReward; // 경험치 보상 저장
        stageSaveData.savedGoldReward += chapterData.chapterIndex[stageSaveData.currentChapterIndex].stageData.stageIndex[stageIndex].GoldReward; // 골드 보상 저장
        stageSaveData.savedJewelReward += chapterData.chapterIndex[stageSaveData.currentChapterIndex].stageData.stageIndex[stageIndex].JewelReward; // 보석 보상 저장
        battleUIController.OpenDefeatPanel(); // 스테이지 패배 UI를 엽니다.
    }

    public void CompleteChapter(int chapterIndex)
    {
        if (chapterIndex < 0 || chapterIndex >= chapterData.chapterIndex.Count)
        {
            Debug.LogError($"Invalid chapter index: {chapterIndex}. Please provide a valid index.");
            return;
        }
        messagePopup.Open("챕터 완료! 정산되는 재화:" +
            $"\n경험치: {StageManager.Instance.stageSaveData.savedExpReward}" +
            $"\n골드: {StageManager.Instance.stageSaveData.savedGoldReward}" +
            $"\n보석: {StageManager.Instance.stageSaveData.savedJewelReward}"); // 챕터 완료 안내
        UserDataManager.Instance.AddExp(StageManager.Instance.stageSaveData.savedExpReward);
        UserDataManager.Instance.AddGold(StageManager.Instance.stageSaveData.savedGoldReward);
        UserDataManager.Instance.AddJewel(StageManager.Instance.stageSaveData.savedJewelReward);

        var states = StageManager.Instance.stageSaveData.chapterStates;
        states[chapterIndex].isCompleted = true;
        Debug.Log($"Chapter complete boolean: {states[chapterIndex].isCompleted}");

        int groupIndex = chapterIndex / 10;
        List<int> normalChapters = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            int normalIdx = groupIndex * 10 + i * 2;
            if (normalIdx < states.Count)
                normalChapters.Add(normalIdx);
        }

        // 노말 챕터를 클리어했다면 다음 노말 챕터 언락
        if (chapterIndex % 2 == 0)
        {
            int nextNormalIdx = chapterIndex + 2;
            if (nextNormalIdx < states.Count)
                states[nextNormalIdx].isUnLocked = true;
        }

        // 노말 챕터 5개가 모두 클리어됐는지 확인
        bool allNormalCompleted = normalChapters.All(idx => states[idx].isCompleted);

        if (allNormalCompleted)
        {
            // 하드 챕터 5개 해금
            foreach (var normalIdx in normalChapters)
            {
                int hardIdx = normalIdx + 1;
                if (hardIdx < states.Count)
                    states[hardIdx].isUnLocked = true;
            }
        }
        SceneManagerEx.Instance.LoadScene("LobbyScene"); // 로비 씬으로 이동
        StageManager.Instance.stageSaveData.ResetToDefault(-1); // 챕터 완료 후 스테이지 데이터 초기화, -1은 현재 챕터가 셀렉트되지 않았음을 의미합니다.
    }

    public void EndChapterEarly(int chapterIndex)
    {
        if (chapterIndex < 0 || chapterIndex >= chapterData.chapterIndex.Count)
        {
            Debug.LogError($"Invalid chapter index: {chapterIndex}. Please provide a valid index.");
            return;
        }
        SceneManagerEx.Instance.LoadScene("LobbyScene"); // 로비 씬으로 이동
        var states = StageManager.Instance.stageSaveData.chapterStates;
        UserDataManager.Instance.AddExp(StageManager.Instance.stageSaveData.savedExpReward); // 경험치 보상 추가
        UserDataManager.Instance.AddGold(StageManager.Instance.stageSaveData.savedGoldReward); // 골드 보상 추가
        UserDataManager.Instance.AddJewel(StageManager.Instance.stageSaveData.savedJewelReward); // 보석 보상 추가
        messagePopup.Open("정산되는 재화:" +
            $"\n경험치: {StageManager.Instance.stageSaveData.savedExpReward}" +
            $"\n골드: {StageManager.Instance.stageSaveData.savedGoldReward}" +
            $"\n보석: {StageManager.Instance.stageSaveData.savedJewelReward}"); // 정산되는 재화 안내
        StageManager.Instance.stageSaveData.ResetToDefault(-1); // 챕터 종료 후 스테이지 데이터 초기화, -1은 현재 챕터가 셀렉트되지 않았음을 의미합니다.
    
    }
    public void InitializeStageStates(ChapterData chapterData)
    {
        if (chapterData == null)
        {
            Debug.LogError("ChapterData가 할당되지 않았습니다.");
            return;
        }
        if (chapterData.chapterIndex == null)
        {
            Debug.LogError("chapterData.chapterIndex가 null입니다.");
            return;
        }
        if (stageSaveData == null || stageSaveData.chapterStates == null)
        {
            Debug.LogError("stageSaveData 또는 chapterAndStageStates가 null입니다.");
            return;
        }

        // 챕터 개수 맞추기 (초과분 제거)
        while (stageSaveData.chapterStates.Count > chapterData.chapterIndex.Count)
            stageSaveData.chapterStates.RemoveAt(stageSaveData.chapterStates.Count - 1);

        // 챕터 개수만큼 상태 리스트 확장
        while (stageSaveData.chapterStates.Count < chapterData.chapterIndex.Count)
            stageSaveData.chapterStates.Add(new ChapterStates());

        for (int c = 0; c < chapterData.chapterIndex.Count; c++)
        {
            var chapterInfo = chapterData.chapterIndex[c];
            var chapterState = stageSaveData.chapterStates[c];

            chapterState.isUnLocked = chapterState.isUnLocked || chapterInfo.DefaultIsUnLocked;
            chapterState.isCompleted = chapterState.isCompleted || chapterInfo.DefaultIsCompleted;

            //// 스테이지 개수 맞추기 (초과분 제거)
            //while (chapterState.stageStates.Count > chapterInfo.stageData.stageIndex.Count)
            //    chapterState.stageStates.RemoveAt(chapterState.stageStates.Count - 1);

            //// 스테이지 개수만큼 상태 리스트 확장
            //while (chapterState.stageStates.Count < chapterInfo.stageData.stageIndex.Count)
            //    chapterState.stageStates.Add(new StageState());

            //for (int s = 0; s < chapterInfo.stageData.stageIndex.Count; s++)
            //{
            //    var stageState = chapterState.stageStates[s];
            //    stageState.isUnLocked = (s == 0);    // 첫 번째 스테이지만 언락
            //    stageState.isCompleted = false;      // 모두 미완료
            //}
        }
    }

    public void selectNormalEnemy()
    {
        var chapterIdx = stageSaveData.currentChapterIndex;
        var stageIdx = stageSaveData.currentStageIndex;
        if (chapterData == null || chapterData.chapterIndex == null ||
            chapterIdx < 0 || chapterIdx >= chapterData.chapterIndex.Count)
        {
            messagePopup.Open("챕터 데이터가 올바르지 않습니다.");
            return;
        }
        var chapter = chapterData.chapterIndex[chapterIdx];
        if (chapter.stageData == null || chapter.stageData.stageIndex == null ||
            stageIdx < 0 || stageIdx >= chapter.stageData.stageIndex.Count)
        {
            messagePopup.Open("스테이지 데이터가 올바르지 않습니다.");
            return;
        }
        var stage = chapter.stageData.stageIndex[stageIdx];
        // 모든 적 리스트에서 노말 타입만 필터링
        var normalEnemies = stage.Enemies?.Where(e => e != null && e.Type == EnemyData.EnemyType.Normal).ToList();
        if (normalEnemies == null || normalEnemies.Count == 0)
        {
            messagePopup.Open("노말 타입 적 데이터가 없습니다.");
            return;
        }
        int randomIndex = Random.Range(0, normalEnemies.Count);
        stageSaveData.selectedEnemy = normalEnemies[randomIndex];
        var battleStartData = new BattleStartData(stageSaveData);
        battleUIController.OpenBattlePanel();
        BattleManager.Instance.StartBattle(battleStartData);
    }

    public void selectEliteEnemy()
    {
        var chapterIdx = stageSaveData.currentChapterIndex;
        var stageIdx = stageSaveData.currentStageIndex;
        if (chapterData == null || chapterData.chapterIndex == null ||
            chapterIdx < 0 || chapterIdx >= chapterData.chapterIndex.Count)
        {
            messagePopup.Open("챕터 데이터가 올바르지 않습니다.");
            return;
        }
        var chapter = chapterData.chapterIndex[chapterIdx];
        if (chapter.stageData == null || chapter.stageData.stageIndex == null ||
            stageIdx < 0 || stageIdx >= chapter.stageData.stageIndex.Count)
        {
            messagePopup.Open("스테이지 데이터가 올바르지 않습니다.");
            return;
        }
        var stage = chapter.stageData.stageIndex[stageIdx];
        // 모든 적 리스트에서 엘리트 타입만 필터링
        var eliteEnemies = stage.Enemies?.Where(e => e != null && e.Type == EnemyData.EnemyType.Elite).ToList();
        if (eliteEnemies == null || eliteEnemies.Count == 0)
        {
            messagePopup.Open("엘리트 타입 적 데이터가 없습니다.");
            return;
        }
        int randomIndex = Random.Range(0, eliteEnemies.Count);
        stageSaveData.selectedEnemy = eliteEnemies[randomIndex];
        var battleStartData = new BattleStartData(stageSaveData);
        battleUIController.OpenBattlePanel();
        BattleManager.Instance.StartBattle(battleStartData);
    }

    public void selectBossEnemy()
    {
        var chapterIdx = stageSaveData.currentChapterIndex;
        var stageIdx = stageSaveData.currentStageIndex;
        if (chapterData == null || chapterData.chapterIndex == null ||
            chapterIdx < 0 || chapterIdx >= chapterData.chapterIndex.Count)
        {
            messagePopup.Open("챕터 데이터가 올바르지 않습니다.");
            return;
        }
        var chapter = chapterData.chapterIndex[chapterIdx];
        if (chapter.stageData == null || chapter.stageData.stageIndex == null ||
            stageIdx < 0 || stageIdx >= chapter.stageData.stageIndex.Count)
        {
            messagePopup.Open("스테이지 데이터가 올바르지 않습니다.");
            return;
        }
        var stage = chapter.stageData.stageIndex[stageIdx];
        var enemies = stage.Enemies?.Where(e => e != null).ToList();
        if (enemies == null || enemies.Count == 0)
        {
            messagePopup.Open("적 데이터가 없습니다.");
            return;
        }

        EnemyData selectedBoss = null;
        if (stageIdx == chapter.stageData.stageIndex.Count - 1)
        {
            // 마지막 스테이지: Lord 타입 보스
            var lordList = enemies.Where(e => e.Type == EnemyData.EnemyType.Lord).ToList();
            if (lordList.Count == 0)
            {
                messagePopup.Open("로드 타입 보스가 없습니다.");
                return;
            }
            selectedBoss = lordList[Random.Range(0, lordList.Count)];
        }
        else
        {
            // 그 외: Guardian 타입 보스
            var guardianList = enemies.Where(e => e.Type == EnemyData.EnemyType.Guardian).ToList();
            if (guardianList.Count == 0)
            {
                messagePopup.Open("가디언 타입 보스가 없습니다.");
                return;
            }
            selectedBoss = guardianList[Random.Range(0, guardianList.Count)];
        }

        stageSaveData.selectedEnemy = selectedBoss;
        var battleStartData = new BattleStartData(stageSaveData);
        battleUIController.OpenBattlePanel();
        BattleManager.Instance.StartBattle(battleStartData);
    }
}
