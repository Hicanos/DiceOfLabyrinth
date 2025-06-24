using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class StageManager : MonoBehaviour
{
    public StageData stageData; // 스테이지 데이터를 담는 변수

    private int currentStageIndex; // 현재 스테이지 인덱스
    private int currentPhaseIndex; // 현재 페이즈 인덱스
    private int gem; // 스테이지 내에서만 쓰이는 재화, 스테이지를 벗어나면 초기화됩니다.
    private GameObject artifact; // 아티팩트 오브젝트, 스테이지 내에서만 쓰이는 오브젝트, 스테이지를 벗어나면 초기화됩니다.
    private GameObject stagma; // 스태그마 오브젝트, 스테이지 내에서만 쓰이는 오브젝트, 스테이지를 벗어나면 초기화됩니다.
    public static StageManager Instance { get; private set; }

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 이 오브젝트를 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 중복 생성 방지
        }

        // 스테이지 데이터가 할당되지 않은 경우 에러 메시지를 출력합니다.
        if (stageData == null)
        {
            Debug.LogError("StageData is not assigned in the inspector!");
        }

        // Json 파일에서 클리어된 스테이지 데이터를 로드하는 메서드가 만들어지면 여기서 호출할 예정입니다.
    }

    public void StartStage(int stageIndex)
    {
        // 스테이지 시작 로직을 구현합니다.
        if (stageData.StageIndex[stageIndex].IsCompleted)
        {
            Debug.Log($"Stage {stageIndex} is already completed.");
            // 이미 완료된 스테이지를 재도전 할지 여부를 묻는 UI를 표시할 예정입니다.
            //if() // 재도전을 거절하는 경우 리턴, UI 입력에 따른 불리언 메서드가 만들어지면 추가할 예정입니다.
            //{
            //    return;
            //}
        }
        else if (stageData.StageIndex[stageIndex].IsLocked)
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
        currentStageIndex = stageIndex; // 현재 스테이지 인덱스 설정
        currentPhaseIndex = 0; // 현재 페이즈 인덱스 초기화
        gem = 0;
        StandbyPhase();
    }

    public void EndStage(int stageIndex, bool isSuccess)
    {
        // 스테이지 종료 로직을 구현합니다.
        // isSuccess에 따라 클리어 여부를 처리하고, Json 파일에 데이터를 저장하는 메서드를 호출할 예정입니다.
        if (isSuccess)
        {
            Debug.Log($"Stage {stageIndex} cleared!");
            // 클리어된 스테이지 정보를 저장하는 로직을 추가할 예정입니다.
            stageData.StageIndex[stageIndex].IsCompleted = true; // 스테이지 완료 상태 업데이트
            stageData.StageIndex[stageIndex+1].IsLocked = false; // 다음 스테이지 잠금 해제
            //보상 로직 추가 예정입니다. 예: 경험치, 골드, 보석 등, 플레이어 데이터가 만들어지면 += 할 예정입니다.

        }
        else
        {
            Debug.Log($"Stage {stageIndex} failed.");
            // 실패 시 처리 로직을 추가할 예정입니다.
        }
        SceneManager.LoadScene("MainMenu"); // 메인 메뉴로 돌아가기, // SceneManagerEX.cs가 만들어지면 수정할 예정입니다.
    }

    private void StandbyPhase()
    {
        //전투 페이즈 이전에 능력치 세팅 로직을 구현합니다.
        BattlePhase(0); // 첫 번째 페이즈로 이동
    }

    public void BattlePhase(int phaseIndex)
    {
        // 전투 페이즈 시작 로직을 구현합니다.
        if (phaseIndex < stageData.StageIndex[currentStageIndex].Phases.Length)
        {
            currentPhaseIndex = phaseIndex;
            PhaseData phaseData = stageData.StageIndex[currentStageIndex].Phases[currentPhaseIndex];
            Debug.Log($"Starting Battle Phase {phaseIndex} with {phaseData.Enemies.Count} enemies.");
            // 적 스폰 로직을 추가할 예정입니다.
            stageData.StageIndex[currentStageIndex].Phases[currentPhaseIndex].
            // 예: SpawnEnemies(phaseData.Enemies);
        }
        else
        {
            Debug.LogError("Invalid phase index.");
        }

    }

    private void ShopPhase()
    {
        //상점페이즈 로직

        BattlePhase(4);
    }
    private void PhaseSuccess(bool isSuccess)
    {
        // 페이즈 성공 여부에 따라 다음 페이즈로 이동하거나 스테이지 종료 로직을 호출합니다.
        if (isSuccess)
        {
            Debug.Log($"Phase {currentPhaseIndex} cleared!");
            if (currentPhaseIndex < stageData.StageIndex[currentStageIndex].Phases.Length - 2)
            {
                BattlePhase(currentPhaseIndex + 1); // 다음 페이즈로 이동
            }
            else if (currentPhaseIndex == stageData.StageIndex[currentStageIndex].Phases.Length - 2)
            {
                ShopPhase(); //
            }
            else // 
            {
                EndStage(currentStageIndex, true); // 스테이지 클리어
            }
        }
        else
        {
            Debug.Log($"Phase {currentPhaseIndex} failed.");
            EndStage(currentStageIndex, false); // 스테이지 실패
        }
    }
}
