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
    [Header("Stage Save Data")]
    public int currentChapterIndex; // 현재 챕터 인덱스
    public int currentStageIndex; // 현재 스테이지 인덱스
    public int currentPhaseIndex; // 현재 페이즈 인덱스
    public CurrentFormationType currentFormationType;
    public int normalStageCompleteCount; // 현재 스테이지의 노멀 룸 완료 개수, 챕터가 완료되면 초기화됩니다.
    public int eliteStageCompleteCount; // 현재 스테이지의 엘리트 룸 완료 개수, 챕터가 완료되면 초기화됩니다.

    public string currentPhaseState; //""은 던전선택에서 사용되는 상태입니다.
    // "". "StartReward", "NormalReward", "EliteArtifactReward", "EliteStagmaReward", "BossReward", "Shop" , "TeamSelect",  "Standby", "Battle" 중 하나

    [Header("Stage Resources")]
    public int manaStone; // 스테이지 내에서만 쓰이는 재화, 스테이지를 벗어나면 초기화됩니다.
    public List<ArtifactData> artifacts = new List<ArtifactData>(18);// 아티팩트 목록, 스테이지 내에서만 쓰이는 재화, 스테이지를 벗어나면 초기화됩니다.
    public List<StagmaData> stagmas = new List<StagmaData>(3); // 최대 3개 제한, 스태그마 목록, 스테이지 내에서만 쓰이는 재화, 스테이지를 벗어나면 초기화됩니다.
    public List<ArtifactData> equipedArtifacts = new List<ArtifactData>(4); // 현재 장착된 아티팩트 목록

    [Header("Stage Characters")]
    public List<CharacterSO> entryCharacters = new List<CharacterSO>(5); // 플레이어 캐릭터 목록, 플레이어 보유 캐릭터 중 5명을 선택하여 스테이지에 진입합니다.
    public CharacterSO leaderCharacter; // 리더 캐릭터, 스테이지에 진입할 때 선택한 캐릭터 중 하나를 리더로 설정합니다.
    public List<BattleCharacter> battleCharacters = new List<BattleCharacter>(5); // 전투에 참여하는 캐릭터 목록, 탐험 버튼을 누를 때 엔트리 캐릭터 목록의 캐릭터들이 할당됩니다.
    [Header("Selected Enemy")]
    public int currentEnemyHP; // 현재 적의 HP, 스테이지에 진입할 때 선택한 적의 HP를 저장합니다.
    public EnemyData selectedEnemy; // 선택된 적, 스테이지에 진입할 때 선택한 적을 저장합니다. 현재는 스테이지에 진입할 때마다 초기화됩니다.
    [Header("Stage Rewards")]
    public int savedExpReward; // 스테이지에서 획득한 경험치 보상, 스테이지 종료시 정산합니다.
    public int savedGoldReward; // 스테이지에서 획득한 골드 보상, 스테이지 종료시 정산합니다.
    public int savedJewelReward; // 스테이지에서 획득한 보석 보상, 스테이지 종료시 정산합니다.

    public List<ChapterAndStageStates> chapterAndStageStates = new List<ChapterAndStageStates>();
 
    public void ResetToDefault(int chapterIndex)// 셀렉트된 챕터의 인덱스를 받아 현재 챕터를 세팅하고 초기화합니다. 셀렉트 된 챕터가 없는 상태는 -1로 설정합니다.
    {
        currentChapterIndex = chapterIndex;
        currentStageIndex = -1; // 초기화 시 스테이지 인덱스는 -1로 설정, 셀렉트 된 스테이지가 없는 상태를 의미합니다.
        currentPhaseIndex = 0;
        currentFormationType = CurrentFormationType.FormationA;
        currentPhaseState = ""; // 초기 상태는 ""로 설정
        manaStone = 0; 
        for (int i = 0; i < 18; i++)
            artifacts[i] = null; // 아티팩트 목록 초기화
        for (int i = 0; i < 3; i++)
            stagmas[i] = null; // 스태그마 목록 초기화
        for (int i = 0; i < entryCharacters.Count; i++)
            entryCharacters[i] = null;
        leaderCharacter = null;
        for (int i = 0; i < battleCharacters.Count; i++)
            battleCharacters[i] = null; // 전투 캐릭터 목록 초기화
        savedExpReward = 0;
        savedGoldReward = 0;
        savedJewelReward = 0;

        for (int i = 0; i < chapterAndStageStates[chapterIndex].stageStates.Count; i++)
        {
            chapterAndStageStates[chapterIndex].stageStates[i].isCompleted = false; // 모든 스테이지는 미완료 상태로 초기화
            chapterAndStageStates[chapterIndex].stageStates[i].isUnLocked = (i == 0); // 첫 번째 스테이지만 잠금 해제, 나머지는 잠금 상태로 초기화
        }
        
    }
}

[System.Serializable]
public class ChapterAndStageStates
{
    public bool isCompleted;
    public bool isUnLocked;
    public List<StageState> stageStates = new List<StageState>(); // 각 스테이지의 상태를 저장하는 리스트
}

[System.Serializable]
public class StageState
{
    public bool isCompleted;
    public bool isUnLocked;
}
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
        if(SceneManager.GetActiveScene().name != "BattleScene")
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
        else if (stageSaveData.currentStageIndex == -1 || stageSaveData.currentPhaseState == "") // 던전 선택 상태
        {
            battleUIController.OpenSelectDungeonPanel(); // 스테이지 선택 UI를 엽니다.
            return;
        }
        else if (stageSaveData.currentPhaseIndex >= 0 || stageSaveData.currentPhaseIndex <= 4) // 현재 선택지 상태가 비어있지 않은 경우
                                                                                               // "StartReward", "NormalReward", "SelectChoice", "EliteArtifactReward", "EliteStagmaReward", "BossReward", "Shop" , "TeamSelect",  "Standby", "Battle" 중 하나
        {
            switch (stageSaveData.currentPhaseState)
            {
                case "TeamSelect":
                    battleUIController.OpenTeamFormationPanel(); // 팀 선택 UI를 엽니다.
                    return;
                case "StartReward":
                    battleUIController.OpenSelectStagmaPanel("StartReward"); // 스탠바이 상태에 해당하는 스태그마 선택 UI를 엽니다.
                    return;
                case "NormalReward":
                    battleUIController.OpenSelectStagmaPanel("NormalReward"); // 노멀 리워드 상태에 해당하는 스태그마 선택 UI를 엽니다.
                    return;
                case "EliteArtifactReward":
                    battleUIController.OpenSelectArtifactPanel("EliteArtifactReward"); // 엘리트 아티팩트 리워드 상태에 해당하는 아티팩트 선택 UI를 엽니다.
                    return;
                case "EliteStagmaReward":
                    battleUIController.OpenSelectStagmaPanel("EliteStagmaReward"); // 엘리트 스태그마 리워드 상태에 해당하는 스태그마 선택 UI를 엽니다.
                    return;
                case "BossReward":
                    battleUIController.OpenSelectStagmaPanel("BossReward"); // 보스 리워드 상태에 해당하는 스태그마 선택 UI를 엽니다.
                    return;
                case "Standby":
                    battleUIController.OpenStagePanel(stageSaveData.currentPhaseIndex); // 스탠바이 상태에 해당하는 UI를 엽니다.
                    return;
                case "Battle":
                    battleUIController.OpenStagePanel(stageSaveData.currentPhaseIndex); // 배틀 중이었어도 복구시엔 스테이지 패널을 엽니다.
                    return;
                case "Shop":
                    // 상점 상태에 해당하는 UI를 엽니다.
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
                messagePopup.Open($"스태그마 {stagmaName.stagmaName}은(는) 이미 목록에 있습니다.");
                return;
            }
        }

        // 빈 슬롯(null) 찾아서 추가
        for (int i = 0; i < 3; i++)
        {
            if (stageSaveData.stagmas[i] == null)
            {
                stageSaveData.stagmas[i] = stagmaName;
                messagePopup.Open($"스태그마 {stagmaName.stagmaName}이(가) 추가되었습니다.");
                return;
            }
        }

        // 모두 차 있으면 안내
        messagePopup.Open("최대 3개의 각인을 소지할 수 있습니다. 더 이상 추가할 수 없습니다.");
    }

    public void AddArtifacts(ArtifactData artifactName)
    {
        // 리스트 크기를 18로 고정
        while (stageSaveData.artifacts.Count < 18)
            stageSaveData.artifacts.Add(null);
        while (stageSaveData.artifacts.Count > 18)
            stageSaveData.artifacts.RemoveAt(stageSaveData.artifacts.Count - 1);

        // 이미 보유 중인지 체크
        for (int i = 0; i < 18; i++)
        {
            if (stageSaveData.artifacts[i] == artifactName)
            {
                messagePopup.Open($"아티팩트 {artifactName.artifactName}은(는) 이미 목록에 있습니다.");
                return;
            }
        }

        // 빈 슬롯(null) 찾아서 추가
        for (int i = 0; i < 18; i++)
        {
            if (stageSaveData.artifacts[i] == null)
            {
                stageSaveData.artifacts[i] = artifactName;
                Debug.Log($"Artifact {artifactName} added.");
                messagePopup.Open($"아티팩트 {artifactName.artifactName}이(가) 추가되었습니다.");
                return;
            }
        }

        // 모두 차 있으면 안내
        messagePopup.Open("최대 18개의 아티팩트를 소지할 수 있습니다. 더 이상 추가할 수 없습니다.");
    }

    public void EquipArtifacts(ArtifactData artifactName)
    {
        // 아티팩트 장착 로직을 구현합니다.
        if (!stageSaveData.artifacts.Contains(artifactName))
        {
            messagePopup.Open($"아티팩트 {artifactName.artifactName}이(가) 소지품에 없습니다.");
            return;
        }
        else
        {
            if (!stageSaveData.equipedArtifacts.Contains(artifactName) && stageSaveData.equipedArtifacts.Count < 4) // 최대 4개까지 장착 가능
            {
                stageSaveData.equipedArtifacts.Add(artifactName);
                stageSaveData.artifacts.Remove(artifactName); // 장착 후 소지품 목록에서 제거
            }
            else
            {
                Debug.LogWarning($"Artifact {artifactName} is already equipped or maximum equipped artifacts reached.");
                return;
            }
            Debug.Log($"Artifact {artifactName} equipped.");
            // 아티팩트 장착 UI 업데이트 메서드를 호출할 예정입니다.
        }
    }
    //private void RewardPhase(int currentPhaseIndex)
    //{

    //}
    public void StageComplete(int stageIndex)
    {
        // 스테이지 종료 로직을 구현합니다.

        Debug.Log($"Stage {stageIndex} cleared!");
        // 클리어된 스테이지 정보를 저장하는 로직을 추가할 예정입니다.
        StageManager.Instance.stageSaveData.chapterAndStageStates[StageManager.Instance.stageSaveData.currentChapterIndex].stageStates[stageIndex].isCompleted = true; // 스테이지 완료 상태 업데이트
        if (stageIndex < chapterData.chapterIndex[stageSaveData.currentChapterIndex].stageData.stageIndex.Count - 1) // 다음 스테이지가 있다면
        {
            StageManager.Instance.stageSaveData.chapterAndStageStates[StageManager.Instance.stageSaveData.currentChapterIndex].stageStates[stageIndex + 1].isUnLocked = true; // 다음 스테이지 잠금 해제
            stageSaveData.savedExpReward += chapterData.chapterIndex[stageSaveData.currentChapterIndex].stageData.stageIndex[stageIndex].ExpReward; // 경험치 보상 저장
            stageSaveData.savedGoldReward += chapterData.chapterIndex[stageSaveData.currentChapterIndex].stageData.stageIndex[stageIndex].GoldReward; // 골드 보상 저장
            stageSaveData.savedJewelReward += chapterData.chapterIndex[stageSaveData.currentChapterIndex].stageData.stageIndex[stageIndex].JewelReward; // 보석 보상 저장
            stageSaveData.currentStageIndex = -1; // 스테이지 인덱스 초기화
        }
        else
        {
            CompleteChapter(stageSaveData.currentChapterIndex); // 마지막 스테이지 클리어 시 챕터 완료 처리
        }
    }

    public void CompleteChapter(int chapterIndex)
    {
        if (chapterIndex < 0 || chapterIndex >= chapterData.chapterIndex.Count)
        {
            Debug.LogError($"Invalid chapter index: {chapterIndex}. Please provide a valid index.");
            return;
        }
        UserDataManager.Instance.AddExp(StageManager.Instance.stageSaveData.savedExpReward);
        UserDataManager.Instance.AddGold(StageManager.Instance.stageSaveData.savedGoldReward);
        UserDataManager.Instance.AddJewel(StageManager.Instance.stageSaveData.savedJewelReward);

        var states = StageManager.Instance.stageSaveData.chapterAndStageStates;
        states[chapterIndex].isCompleted = true;

        int groupIndex = chapterIndex / 10;
        List<int> normalChapters = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            int normalIdx = groupIndex * 10 + i * 2;
            if (normalIdx < states.Count)
                normalChapters.Add(normalIdx);
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
            // 다음 노말 챕터 5개 해금
            foreach (var normalIdx in normalChapters)
            {
                int nextNormalIdx = normalIdx + 10;
                if (nextNormalIdx < states.Count)
                    states[nextNormalIdx].isUnLocked = true;
            }
        }

        StageManager.Instance.stageSaveData.ResetToDefault(-1); // 챕터 완료 후 스테이지 데이터 초기화, -1은 현재 챕터가 셀렉트되지 않았음을 의미합니다.
    }

    public void DefeatChapter(int chapterIndex)
    {
        if (chapterIndex < 0 || chapterIndex >= chapterData.chapterIndex.Count)
        {
            Debug.LogError($"Invalid chapter index: {chapterIndex}. Please provide a valid index.");
            return;
        }
        var states = StageManager.Instance.stageSaveData.chapterAndStageStates;
        states[chapterIndex].isCompleted = false; // 챕터 완료 상태를 해제
        states[chapterIndex].isUnLocked = true; // 챕터 잠금 해제 상태 유지
        StageManager.Instance.stageSaveData.ResetToDefault(-1); // 챕터 패배 후 스테이지 데이터 초기화, -1은 현재 챕터가 셀렉트되지 않았음을 의미합니다.
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
        if (stageSaveData == null || stageSaveData.chapterAndStageStates == null)
        {
            Debug.LogError("stageSaveData 또는 chapterAndStageStates가 null입니다.");
            return;
        }
        // 챕터 개수만큼 상태 리스트 확장
        while (stageSaveData.chapterAndStageStates.Count < chapterData.chapterIndex.Count)
            stageSaveData.chapterAndStageStates.Add(new ChapterAndStageStates());

        for (int c = 0; c < chapterData.chapterIndex.Count; c++)
        {
            var chapterInfo = chapterData.chapterIndex[c];
            var chapterState = stageSaveData.chapterAndStageStates[c];

            chapterState.isUnLocked = chapterState.isUnLocked || chapterInfo.DefaultIsUnLocked;
            chapterState.isCompleted = chapterState.isCompleted || chapterInfo.DefaultIsCompleted;

            while (chapterState.stageStates.Count < chapterInfo.stageData.stageIndex.Count)
                chapterState.stageStates.Add(new StageState());

            for (int s = 0; s < chapterInfo.stageData.stageIndex.Count; s++)
            {
                var stageState = chapterState.stageStates[s];
                stageState.isUnLocked = (s == 0);    // 첫 번째 스테이지만 언락
                stageState.isCompleted = false;      // 모두 미완료
            }
        }
    }
}
