using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class StageManager : MonoBehaviour
{
    public StageData stageData; // 스테이지 데이터를 담는 변수
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

        // Json 파일에서 클리어된 스테이지 데이터를 로드하는 메서드가 만들어지면 여기서 호출할 예정입니다.
    }

    public void StartStage(int stageIndex)
    {
        // 스테이지 시작 로직을 구현합니다.
        SceneManager.LoadScene("BattleScene");
        stageData.StageIndex[stageIndex].IsLocked = false; // 스테이지 잠금 해제
    }

    public void EndStage(int stageIndex, bool isSuccess)
    {
        // 스테이지 종료 로직을 구현합니다.
        // isSuccess에 따라 클리어 여부를 처리하고, Json 파일에 데이터를 저장하는 메서드를 호출할 예정입니다.
        if (isSuccess)
        {
            Debug.Log($"Stage {stageIndex} cleared!");
            // 클리어된 스테이지 정보를 저장하는 로직을 추가할 예정입니다.
        }
        else
        {
            Debug.Log($"Stage {stageIndex} failed.");
            // 실패 시 처리 로직을 추가할 예정입니다.
        }
        SceneManager.LoadScene("MainMenu"); // 메인 메뉴로 돌아가기
    }

}
