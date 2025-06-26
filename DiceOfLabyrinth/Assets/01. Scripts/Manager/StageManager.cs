using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class StageManager : MonoBehaviour
{
    public ChapterManager chapterManager;
    public ChapterData chapterData; // ChapterData 스크립터블 오브젝트, 에디터에서 할당해야 합니다.

    public int currentChapterIndex; // 현재 챕터 인덱스
    public int currentStageIndex; // 현재 스테이지 인덱스
    public int currentPhaseIndex; // 현재 페이즈 인덱스

    public int gem; // 스테이지 내에서만 쓰이는 재화, 스테이지를 벗어나면 초기화됩니다.
    public List<ArtifactData> artifacts = new List<ArtifactData>();// 아티팩트 목록, 스테이지 내에서만 쓰이는 재화, 스테이지를 벗어나면 초기화됩니다.
    public List<StagmaData> stagma = new List<StagmaData>(3); // 최대 3개 제한, 스태그마 목록, 스테이지 내에서만 쓰이는 재화, 스테이지를 벗어나면 초기화됩니다.
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


    public void StartStage(int chapterIndex, int stageIndex)
    {
        // 스테이지 시작 로직을 구현합니다. 아래의 If 문은 UI를 다루는 cs가 만들어지면 수정할 예정입니다.
        if (chapterData.chapterIndex[chapterIndex].stageData.stageIndex[stageIndex].IsCompleted)
        {
            Debug.Log($"Stage {stageIndex} is already completed.");
            // 이미 완료된 스테이지를 재도전 할지 여부를 묻는 UI를 표시할 예정입니다.
        }
        else if (chapterData.chapterIndex[chapterIndex].stageData.stageIndex[stageIndex].IsLocked)
        {
            Debug.Log($"Stage {stageIndex} is locked. Please complete previous stages.");
            // 잠금된 스테이지를 시작할 수 없다는 UI 메시지를 표시할 예정입니다.
            return;
        }
        // 플레이어 데이터에 입장 코스트가 만들어지면 코스트 비교를 추가할 예정입니다.
        //else if (stageData.StageIndex[stageIndex].StageCost > 플레이어의 입장 코스트)
        //{
        //    // 자원이 부족하다는 UI 메시지를 표시할 예정입니다.
        //    return;
        //}
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
    }

    public void EndStage(int chapterIndex, int stageIndex, bool isSuccess)
    {
        // 스테이지 종료 로직을 구현합니다.
        // isSuccess에 따라 클리어 여부를 처리하고, Json 파일에 데이터를 저장하는 메서드를 호출할 예정입니다.
        if (isSuccess)
        {
            Debug.Log($"Stage {stageIndex} cleared!");
            // 클리어된 스테이지 정보를 저장하는 로직을 추가할 예정입니다.
            chapterData.chapterIndex[chapterIndex].stageData.stageIndex[stageIndex].IsCompleted = true; // 스테이지 완료 상태 업데이트
            chapterData.chapterIndex[chapterIndex].stageData.stageIndex[stageIndex+1].IsLocked = false; // 다음 스테이지 잠금 해제
            //보상 로직 추가 예정입니다. 예: 경험치, 골드, 보석 등, 플레이어 데이터가 만들어지면 += 할 예정입니다./
            //

        }
        else
        {
            Debug.Log($"Stage {stageIndex} failed.");
            // 실패 시 처리 로직을 추가할 예정입니다.
        }
        SceneManager.LoadScene("MainMenu"); // 메인 메뉴로 돌아가기, // SceneManagerEX.cs가 만들어지면 수정할 예정입니다.
    }

    public void StandbyPhase()
    {
        //전투 페이즈 이전에 능력치 세팅 로직을 구현합니다.
        //BattleUIController.cs에서 능력치 세팅 UI 메서드를 호출할 예정입니다.
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
