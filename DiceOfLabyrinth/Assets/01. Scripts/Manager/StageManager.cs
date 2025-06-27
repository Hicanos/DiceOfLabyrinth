using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using System.Xml;

public class StageManager : MonoBehaviour
{
    public ChapterManager chapterManager;
    public ChapterData chapterData; // ChapterData 스크립터블 오브젝트, 에디터에서 할당해야 합니다.
    
    public enum DifficultyLevel // 스테이지 난이도 레벨, 필요시 추가할 수 있습니다.
    {
        Normal,
        Hard
    }
    public enum FormationType // 포메이션 타입
    {
        Formation1,
        Formation2,
        Formation3,
        Formation4,
    }
    public int currentChapterIndex; // 현재 챕터 인덱스
    public int currentStageIndex; // 현재 스테이지 인덱스
    public int currentPhaseIndex; // 현재 페이즈 인덱스
    public DifficultyLevel difficultyLevel;

    
    public int gem; // 스테이지 내에서만 쓰이는 재화, 스테이지를 벗어나면 초기화됩니다.
    public List<ArtifactData> artifacts = new List<ArtifactData>();// 아티팩트 목록, 스테이지 내에서만 쓰이는 재화, 스테이지를 벗어나면 초기화됩니다.
    public List<StagmaData> stagma = new List<StagmaData>(3); // 최대 3개 제한, 스태그마 목록, 스테이지 내에서만 쓰이는 재화, 스테이지를 벗어나면 초기화됩니다.
    public CharacterSO[] entryCharacters = new CharacterSO[5]; // 플레이어 캐릭터 목록, 플레이어 보유 캐릭터 중 5명을 선택하여 스테이지에 진입합니다.
    public CharacterSO leaderCharacter; // 리더 캐릭터, 스테이지에 진입할 때 선택한 캐릭터 중 하나를 리더로 설정합니다.


    public int savedExpReward; // 스테이지에서 획득한 경험치 보상, 스테이지 종료시 정산합니다.
    public int savedGoldReward; // 스테이지에서 획득한 골드 보상, 스테이지 종료시 정산합니다.
    public int savedJewelReward; // 스테이지에서 획득한 보석 보상, 스테이지 종료시 정산합니다.
    public static StageManager Instance { get; private set; }

    //public static StageManager SafeInstance // null에 대비한 방어용 프로퍼티, 필요시 주석 해제하여 사용하세요. 이걸 사용할거면 어드레서블 등으로 챕터 데이터를 불러와야 합니다.
    //{
    //    get
    //    {
    //        if (Instance == null)
    //        {
    //            var found = FindFirstObjectByType<StageManager>();
    //            if (found != null)
    //            {
    //                Instance = found;
    //            }
    //            else
    //            {
    //                var go = new GameObject("StageManager");
    //                Instance = go.AddComponent<StageManager>();
    //            }
    //        }
    //        return Instance;
    //    }
    //}

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
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 중복 생성 방지
        }

        if (chapterData == null)
        {
            Debug.LogError("ChapterData is not assigned in StageManager. Please assign it in the inspector.");
        }

        // Json 파일에서 클리어된 스테이지 데이터를 로드하는 메서드가 만들어지면 여기서 호출할 예정입니다.
    }


    public void LoadStage(int chapterIndex, int stageIndex)
    {
        // 스테이지 시작 로직을 구현합니다. 아래의 If 문은 UI를 다루는 cs가 만들어지면 수정할 예정입니다.
        if (chapterData.chapterIndex[chapterIndex].stageData.stageIndex[stageIndex].IsCompleted)
        {
            Debug.Log($"Stage {stageIndex} is already completed.");
            // 이미 완료된 스테이지는 도전할 수 없습니다.
            return;
        }
        else if (chapterData.chapterIndex[chapterIndex].stageData.stageIndex[stageIndex].IsLocked)
        {
            Debug.Log($"Stage {stageIndex} is locked. Please complete previous stages.");
            // 잠금된 스테이지를 시작할 수 없다는 UI를 표시할 예정입니다.
            return;
        }
        SceneManager.LoadScene("BattleScene");//SceneManagerEX.cs가 만들어지면 수정할 예정입니다.
        StandbyPhase();
    }

    public void ResetStageData(int chapterIndex, int stageIndex)
    {
        currentChapterIndex = chapterIndex; // 현재 챕터 인덱스 설정
        currentStageIndex = stageIndex; // 현재 스테이지 인덱스 설정
        currentPhaseIndex = 0; // 현재 페이즈 인덱스 초기화
        gem = 0; // 스테이지 시작 시 재화 초기화
        artifacts.Clear(); // 스테이지 시작 시 아티팩트 목록 초기화
        stagma.Clear(); // 스테이지 시작 시 스태그마 목록 초기화
        savedExpReward = 0; // 경험치 보상 초기화
        savedGoldReward = 0; // 골드 보상 초기화
        savedJewelReward = 0; // 보석 보상 초기화
        difficultyLevel = DifficultyLevel.Normal; // 난이도 초기화, 필요시 수정 가능
    }

    public void StageComplete(int chapterIndex, int stageIndex)
    {
        // 스테이지 종료 로직을 구현합니다.
       
            Debug.Log($"Stage {stageIndex} cleared!");
            // 클리어된 스테이지 정보를 저장하는 로직을 추가할 예정입니다.
            chapterData.chapterIndex[chapterIndex].stageData.stageIndex[stageIndex].IsCompleted = true; // 스테이지 완료 상태 업데이트
        if (stageIndex < chapterData.chapterIndex[chapterIndex].stageData.stageIndex.Count - 1) // 다음 스테이지가 있다면
        {
            chapterData.chapterIndex[chapterIndex].stageData.stageIndex[stageIndex + 1].IsLocked = false; // 다음 스테이지 잠금 해제
            savedExpReward += chapterData.chapterIndex[chapterIndex].stageData.stageIndex[stageIndex].ExpReward; // 경험치 보상 저장

        }
        else
        {
            chapterManager.CompleteChapter(chapterIndex); // 마지막 스테이지 클리어 시 챕터 완료 처리
        }
    }

    public void SelectCharacter(CharacterSO character, int index)
    {
        // 플레이어가 선택한 캐릭터를 엔트리 캐릭터 목록에 추가합니다.
        if (character == null)
        {
            Debug.LogError("Selected character is null. Please select a valid character.");
            return;
        }
        entryCharacters[index] = character; // 선택한 캐릭터를 엔트리 캐릭터 목록에 설정
        if(leaderCharacter == null)
        {
            leaderCharacter = character; // 리더 캐릭터가 아직 설정되지 않았다면 선택한 캐릭터를 리더로 설정
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
        leaderCharacter = character; // 선택한 캐릭터를 리더로 설정
    }

    public void RemoveCharacter(ChapterData chapterData, int index)
    {
        // 플레이어가 선택한 캐릭터를 엔트리 캐릭터 목록에서 제거합니다.
        if (index < 0 || index >= entryCharacters.Length)
        {
            Debug.LogError($"Invalid character index: {index}. Please provide a valid index.");
            return;
        }
        entryCharacters[index] = null; // 해당 인덱스의 캐릭터를 null로 설정하여 제거
        if(leaderCharacter == entryCharacters[index])
        {
            foreach (var character in entryCharacters)
            {
                if (character != null) // 다른 캐릭터가 있다면 그 캐릭터를 리더로 설정
                {
                    leaderCharacter = character;
                    return;
                }
            }
        }
    }

    public void StandbyPhase()
    {
        
        //전투 페이즈 이전에 능력치 세팅 로직을 구현합니다.
        // 각인 선택 UI를 출력할 예정입니다.
        //BattleUIController.cs에서 능력치 세팅 UI 메서드를 호출할 예정입니다.
    }

    public void AddStagma(StagmaData stagmaName)
    {
        // 스태그마 추가 로직을 구현합니다.
        if (stagma.Count < 3) // 최대 3개까지 소지 가능
        {
            if (!stagma.Contains(stagmaName))
            {
                stagma.Add(stagmaName);
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
        if (!artifacts.Contains(artifactName))
        {
            artifacts.Add(artifactName);
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
        if (!artifacts.Contains(artifactName))
        {
            Debug.LogWarning($"Artifact {artifactName} is not in the artifacts list.");
            return;
        }
        else
        {
            if (!chapterManager.equipedArtifacts.Contains(artifactName) && chapterManager.equipedArtifacts.Count < 4) // 최대 4개까지 장착 가능
            {
                chapterManager.equipedArtifacts.Add(artifactName);
                artifacts.Remove(artifactName); // 장착 후 소지품 목록에서 제거
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
            currentPhaseIndex = phaseIndex;
            var normalPhases = chapterData.chapterIndex[currentChapterIndex].stageData.stageIndex[currentStageIndex].NormalPhases;
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
            currentPhaseIndex = phaseIndex;
            var elitePhases = chapterData.chapterIndex[currentChapterIndex].stageData.stageIndex[currentStageIndex].ElitePhases;
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
                currentPhaseIndex++;
                if (currentPhaseIndex < 4) // 4페이즈까지 진행 가능
                {
                    RewardPhase(currentPhaseIndex);
                    BattlePhaseEliteRoom(currentPhaseIndex);
                }
                break;
            case "ShopRoom":
                // 상점 방 성공 로직
                ShopPhase();
                break;

            case "NormalRoom":
                // 일반 방 성공 로직
                currentPhaseIndex++;
                if (currentPhaseIndex < 4) // 4페이즈까지 진행 가능
                {
                    RewardPhase(currentPhaseIndex);
                    BattlePhaseNormalRoom(currentPhaseIndex);
                }
                break;
            }
    }

    private void RewardPhase(int currentPhaseIndex)
    {
        
    }
}
