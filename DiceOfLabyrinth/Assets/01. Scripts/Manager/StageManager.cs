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

    public string currentChoiceState; // 현재 선택지 상태, "". "Standby", "NormalReward", "EliteArtifactReward", "EliteStagmaReward", "BossReward" 중 하나


    public int manaStone; // 스테이지 내에서만 쓰이는 재화, 스테이지를 벗어나면 초기화됩니다.
    public List<ArtifactData> artifacts = new List<ArtifactData>();// 아티팩트 목록, 스테이지 내에서만 쓰이는 재화, 스테이지를 벗어나면 초기화됩니다.
    public List<StagmaData> stagma = new List<StagmaData>(3); // 최대 3개 제한, 스태그마 목록, 스테이지 내에서만 쓰이는 재화, 스테이지를 벗어나면 초기화됩니다.
    public CharacterSO[] entryCharacters = new CharacterSO[5]; // 플레이어 캐릭터 목록, 플레이어 보유 캐릭터 중 5명을 선택하여 스테이지에 진입합니다.
    public CharacterSO leaderCharacter; // 리더 캐릭터, 스테이지에 진입할 때 선택한 캐릭터 중 하나를 리더로 설정합니다.


    public int savedExpReward; // 스테이지에서 획득한 경험치 보상, 스테이지 종료시 정산합니다.
    public int savedGoldReward; // 스테이지에서 획득한 골드 보상, 스테이지 종료시 정산합니다.
    public int savedJewelReward; // 스테이지에서 획득한 보석 보상, 스테이지 종료시 정산합니다.

    public List<ChapterAndStageStates> chapterAndStageStates = new List<ChapterAndStageStates>();
 
    public void ResetToDefault(int chapterIndex)
    {
        currentChapterIndex = chapterIndex;
        currentStageIndex = 0;
        currentPhaseIndex = 0;
        currentFormationType = CurrentFormationType.FormationA;
        currentChoiceState = "Standby"; // 초기 상태는 Standby로 설정
        manaStone = 0;
        artifacts.Clear();
        stagma.Clear();
        for (int i = 0; i < entryCharacters.Length; i++)
            entryCharacters[i] = null;
        leaderCharacter = null;
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
    public ChapterManager chapterManager;
    public ChapterData chapterData; // ChapterData 스크립터블 오브젝트, 에디터에서 할당해야 합니다.
    public StageSaveData stageSaveData; // 스테이지 저장 데이터, 스테이지 시작 시 초기화됩니다.
    public BattleUIController battleUIController; // 배틀 UI 컨트롤러, 스테이지 시작 시 초기화됩니다.

    public static StageManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 이 오브젝트를 씬 전환 시 파괴되지 않도록 설정
            if(chapterManager == null)
            {
                chapterManager = GetComponent<ChapterManager>();
                if (chapterManager == null)
                {
                    Debug.LogError("ChapterManager not found in the scene. Please ensure it is present.");
                }
            }
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
        else if (stageSaveData.currentChapterIndex < 0 || stageSaveData.currentChapterIndex >= chapterData.chapterIndex.Count)
        {
            Debug.LogError("현재의 챕터 인덱스가 유효하지 않습니다. 챕터 데이터를 확인해주세요.");
            return;
        }
        else if (stageSaveData.currentStageIndex < 0 || stageSaveData.currentStageIndex > 4) // 스테이지 인덱스가 0~4 범위를 벗어나는 경우
        {
            Debug.LogError("현재의 스테이지 인덱스가 유효하지 않습니다. 스테이지 데이터를 확인해주세요.");
            return;
        }
        else if (stageSaveData.currentPhaseIndex < 0 || stageSaveData.currentPhaseIndex > 4) // 페이즈 인덱스가 0~4 범위를 벗어나는 경우
        {
            Debug.LogError("현재의 페이즈 인덱스가 유효하지 않습니다. 페이즈 데이터를 확인해주세요.");
            return;
        }
        //else if (stageSaveData.leaderCharacter == null || stageSaveData.entryCharacters.Any(x => x == null)) // 리더 캐릭터나 엔트리 캐릭터가 설정되지 않은 경우
        //{
        //    BattleUIController.Instance.OpenSelectCharacterPanel(); // 캐릭터 선택 UI를 엽니다.
        //    return;
        //}
        else if (stageSaveData.currentChoiceState != "") // 현재 선택지 상태가 비어있지 않은 경우
        {
            switch (stageSaveData.currentChoiceState)
            {
                case "Standby":
                    battleUIController.OpenSelectStagmaPanel("Standby"); // 스탠바이 상태에 해당하는 스태그마 선택 UI를 엽니다.
                    break;
                case "NormalReward":
                    battleUIController.OpenSelectStagmaPanel("NormalReward"); // 노멀 리워드 상태에 해당하는 스태그마 선택 UI를 엽니다.
                    break;
                case "EliteArtifactReward":
                    battleUIController.OpenSelectArtifactPanel("EliteArtifactReward"); // 엘리트 아티팩트 리워드 상태에 해당하는 아티팩트 선택 UI를 엽니다.
                    break;
                case "EliteStagmaReward":
                    battleUIController.OpenSelectStagmaPanel("EliteStagmaReward"); // 엘리트 스태그마 리워드 상태에 해당하는 스태그마 선택 UI를 엽니다.
                    break;
                case "BossReward":
                    battleUIController.OpenSelectStagmaPanel("BossReward"); // 보스 리워드 상태에 해당하는 스태그마 선택 UI를 엽니다.
                    break;
                default:
                    Debug.LogError($"Unknown choice state: {stageSaveData.currentChoiceState}");
                    return;
            }
            return; // 선택지 상태가 처리되었으므로 이후 로직을 실행하지 않습니다.
        }
        else if (stageSaveData.currentPhaseIndex >=0 || stageSaveData.currentPhaseIndex <= 4) // 페이즈 인덱스가 유효한 경우
        {
            battleUIController.OpenStagePanel(stageSaveData.currentPhaseIndex); // 스테이지 패널을 엽니다.
            return;
        }
    }

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
        }
        else
        {
            chapterManager.CompleteChapter(stageSaveData.currentChapterIndex); // 마지막 스테이지 클리어 시 챕터 완료 처리
        }
    }

    public void SelectCharacterPhase()
    {

    }

    public void SelectCharacter(CharacterSO character, int index)
    {
        // 플레이어가 선택한 캐릭터를 엔트리 캐릭터 목록에 추가합니다.
        if (character == null)
        {
            Debug.LogError("Selected character is null. Please select a valid character.");
            return;
        }
        stageSaveData.entryCharacters[index] = character; // 선택한 캐릭터를 엔트리 캐릭터 목록에 설정
        if(stageSaveData.leaderCharacter == null)
        {
            stageSaveData.leaderCharacter = character; // 리더 캐릭터가 아직 설정되지 않았다면 선택한 캐릭터를 리더로 설정
        }
    }
    public void SelectLeaderCharacter(CharacterSO character)
    {
        // 플레이어가 선택한 캐릭터를 리더 캐릭터로 설정합니다.
        if (character == null)
        {
            Debug.LogError("Selected leader character is null. Please select a valid character.");
            return;
        }
        stageSaveData.leaderCharacter = character; // 선택한 캐릭터를 리더로 설정
    }

    public void RemoveCharacter(ChapterData chapterData, int index)
    {
        // 플레이어가 선택한 캐릭터를 엔트리 캐릭터 목록에서 제거합니다.
        if (index < 0 || index >= stageSaveData.entryCharacters.Length)
        {
            Debug.LogError($"Invalid character index: {index}. Please provide a valid index.");
            return;
        }
        stageSaveData.entryCharacters[index] = null; // 해당 인덱스의 캐릭터를 null로 설정하여 제거
        if(stageSaveData.leaderCharacter == stageSaveData.entryCharacters[index])
        {
            foreach (var character in stageSaveData.entryCharacters)
            {
                if (character != null) // 다른 캐릭터가 있다면 그 캐릭터를 리더로 설정
                {
                    stageSaveData.leaderCharacter = character;
                    return;
                }
            }
        }
    }


    public void AddStagma(StagmaData stagmaName)
    {
        // 스태그마 추가 로직을 구현합니다.
        if (stageSaveData.stagma.Count < 3) // 최대 3개까지 소지 가능
        {
            if (!stageSaveData.stagma.Contains(stagmaName))
            {
                stageSaveData.stagma.Add(stagmaName);
                Debug.Log($"Stagma {stagmaName} added.");
                // 스태그마 추가 UI 업데이트 메서드를 호출할 예정입니다.
            }
            else
            {
                Debug.LogWarning($"Stagma {stagmaName} is already in the list.");
            }
        }
        else
        {
            Debug.LogWarning("Maximum number of Stagmas reached. Cannot add more.");
        }
    }

    public void AddArtifacts(ArtifactData artifactName)
    {
        // 아티팩트 추가 로직을 구현합니다.
        if (!stageSaveData.artifacts.Contains(artifactName))
        {
            stageSaveData.artifacts.Add(artifactName);
            Debug.Log($"Artifact {artifactName} added.");
            // 아티팩트 추가 UI 업데이트 메서드를 호출할 예정입니다.
        }
        else
        {
            Debug.LogWarning($"Artifact {artifactName} is already in the list.");
        }
    }

    public void EquipArtifacts(ArtifactData artifactName)
    {
        // 아티팩트 장착 로직을 구현합니다.
        if (!stageSaveData.artifacts.Contains(artifactName))
        {
            Debug.LogWarning($"Artifact {artifactName} is not in the artifacts list.");
            return;
        }
        else
        {
            if (!chapterManager.equipedArtifacts.Contains(artifactName) && chapterManager.equipedArtifacts.Count < 4) // 최대 4개까지 장착 가능
            {
                chapterManager.equipedArtifacts.Add(artifactName);
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



    public void BattlePhaseNormalRoom(int phaseIndex)
    {
        // 전투 페이즈 시작 로직을 구현합니다.
        if (phaseIndex < 4)
        {
            stageSaveData.currentPhaseIndex = phaseIndex;
            var normalPhases = chapterData.chapterIndex[stageSaveData.currentChapterIndex].stageData.stageIndex[stageSaveData.currentStageIndex].NormalPhases;
            if (normalPhases == null || normalPhases.Count == 0)
            {
                Debug.LogError("NormalPhase 리스트가 비어 있습니다.");
                return;
            }
            int randomIndex = Random.Range(0, normalPhases.Count);
            NormalPhaseData phaseData = normalPhases[randomIndex];
            foreach (var enemyInfo in phaseData.Enemies)
            {
                Vector2 spawnPosition = enemyInfo.SpawnPosition;
                var enemyPrefab = enemyInfo.EnemyPrefab;

                GameObject enemyObj = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                IEnemy enemy = enemyObj.GetComponent<IEnemy>();
                if (enemy != null)
                {
                    enemy.Init();
                }
                else
                {
                    Debug.LogError($"{enemyPrefab.name}에 IEnemy가 구현되어 있지 않습니다.");
                }
            }
        }
        else if (phaseIndex == 4)
        {
            Debug.Log("4페이즈 인덱스는 보스룸을 위한 인덱스입니다. 잘못된 페이즈 인덱스입니다.");
        }
        else
        {
            Debug.LogError("Invalid phase index.");
        }

    }

    public void BattlePhaseEliteRoom(int phaseIndex)
    {
        if(phaseIndex < 4)
        {
            stageSaveData.currentPhaseIndex = phaseIndex;
            var elitePhases = chapterData.chapterIndex[stageSaveData.currentChapterIndex].stageData.stageIndex[stageSaveData.currentStageIndex].ElitePhases;
            if (elitePhases == null || elitePhases.Count == 0)
            {
                Debug.LogError("ElitePhase 리스트가 비어 있습니다.");
                return;
            }
            int randomIndex = Random.Range(0, elitePhases.Count);
            ElitePhaseData phaseData = elitePhases[randomIndex];
            foreach (var enemyInfo in phaseData.Enemies)
            {
                Vector3 spawnPosition = enemyInfo.SpawnPosition;
                var enemyPrefab = enemyInfo.EnemyPrefab;
                GameObject enemyObj = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                IEnemy enemy = enemyObj.GetComponent<IEnemy>();
                if (enemy != null)
                {
                    enemy.Init();
                }
                else
                {
                    Debug.LogError($"{enemyPrefab.name}에 IEnemy가 구현되어 있지 않습니다.");
                }
            }
        }
        else if (phaseIndex == 4)
        {
            Debug.Log("4페이즈 인덱스는 보스룸을 위한 인덱스입니다. 잘못된 페이즈 인덱스입니다.");
        }
        else
        {
            Debug.LogError("Invalid phase index.");
        }
    }

    private void ShopPhase()
    {
        //상점페이즈 로직, 예를 들어, 플레이어가 아이템을 구매하거나 판매할 수 있는 UI를 표시합니다.
    }
    public void PhaseSuccess(string roomType)
    {
            switch (roomType)
            {
            case "EliteRoom":
                // 엘리트 방 성공 로직
                stageSaveData.currentPhaseIndex++;
                if (stageSaveData.currentPhaseIndex < 4) // 4페이즈까지 진행 가능
                {
                    RewardPhase(stageSaveData.currentPhaseIndex);
                    BattlePhaseEliteRoom(stageSaveData.currentPhaseIndex);
                }
                break;
            case "ShopRoom":
                // 상점 방 성공 로직
                ShopPhase();
                break;

            case "NormalRoom":
                // 일반 방 성공 로직
                stageSaveData.currentPhaseIndex++;
                if (stageSaveData.currentPhaseIndex < 4) // 4페이즈까지 진행 가능
                {
                    RewardPhase(stageSaveData.currentPhaseIndex);
                    BattlePhaseNormalRoom(stageSaveData.currentPhaseIndex);
                }
                break;
            }
    }

    private void RewardPhase(int currentPhaseIndex)
    {
        
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
