using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            //게임 데이터 로드
            LoadGame();
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 제거
        }
    }

    // 게임 저장, 로드, 게임 종료, 전면 초기화 등

    private void SaveGame()
    {
        // 예: 현재 게임 상태를 각각 JSON으로 직렬화하고 파일에 저장
        // 1. 현재 유저(플레이어)의 재화 정보, 레벨정보 저장
        // 2. 현재 플레이어가 획득한 캐릭터 정보(강화 여부포함) 저장
        // 3. 현재 플레이어가 클리어한 스테이지 정보 저장
        // 4. 현재 플레이어가 획득한 아이템 정보 저장
        // DataSaver에서 저장로직 가져올 것


        // 아래에서 일괄처리
        DataSaver.Instance.Save();

    }

    private void Start()
    {
        // 불러온 모든 캐릭터의 Json데이터를 실제 LobbyCharacter에 할당
        // UI에 존재한s LobbyCharacter에 Awake에서 불러온 데이터를 집어넣음.
    }

    public void LoadGame()
    {
        // 게임 로드 로직 구현
        DataSaver.Instance.Load();
        Debug.Log("게임 로드됨");
    }

    public void ExitGame()
    {
        // 게임 종료 로직 구현
        // 현재까지 진행된 게임 상태를 저장하고, 게임을 종료함.
        SaveGame();
        Debug.Log("게임 종료됨");
        Application.Quit();
    }

    public void ResetGame()
    {
        // 게임 초기화 로직 구현
        Debug.Log("게임 초기화됨");
        // 예: 모든 데이터 초기화, 씬 재시작 등
        // 모든 매니저를 초기 상태로 되돌림( 클리어한 스테이지, 획득한 캐릭터, 강화된 캐릭터 등)
        // 저장된 데이터도 삭제함
        
    }

    // 애플리케이션 종료(강제 종료) 시 자동으로 게임 저장
    private void OnApplicationQuit()
    {
        SaveGame();
    }

}
