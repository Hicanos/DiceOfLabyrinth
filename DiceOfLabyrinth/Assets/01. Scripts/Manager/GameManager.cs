using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 전체의 데이터 로드, 복구, 릴리즈를 총괄하는 매니저
/// - 모든 SO/데이터를 StaticDataManager에서 관리
/// - 데이터 복구는 SO 로드 완료 후에만 진행
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 제거
        }
    }

    /// <summary>
    /// 게임 시작 시점에 모든 SO/데이터를 StaticDataManager에서 관리 후 복구
    /// </summary>
    private IEnumerator Start()
    {

        // StageManager.Instance가 생성될 때까지 대기
        while (StageManager.Instance == null)
            yield return null;

        // 캐릭터 SO 로드 대기
        yield return CharacterManager.Instance.LoadAllCharactersAsync();

        // 아이템 SO 로드 대기
        yield return ItemManager.Instance.LoadAllItemSOs();


        // SO 데이터 복구
        RestoreGameData();
        Debug.Log("게임 데이터 로드 및 SO 복구 완료");
    }

    private void RestoreGameData()
    {
        DataSaver.Instance.Load();
        CharacterManager.Instance.LoadOwnedCharactersFromData();
        ItemManager.Instance.LoadOwnedItemsFromData();
        if (StageManager.Instance != null && DataSaver.Instance.SaveData.stageData != null)
        {
            StageManager.Instance.stageSaveData = DataSaver.Instance.SaveData.stageData.ToStageSaveData();

            StageManager.Instance.InitializeStageStates(StageManager.Instance.chapterData);
        }
    }

    /// <summary>
    /// 게임 저장
    /// </summary>
    private void SaveGame()
    {
        DataSaver.Instance.Save();
    }

    /// <summary>
    /// 게임 종료
    /// </summary>
    public void ExitGame()
    {
        SaveGame();
        Debug.Log("게임 종료됨");
        Application.Quit();
    }

    /// <summary>
    /// 게임 초기화
    /// </summary>
    public void ResetGame()
    {
        SaveGame();
        Debug.Log("게임 초기화됨");
        // 추가: 데이터 파일 삭제, 씬 재시작 등
    }

    /// <summary>
    /// 애플리케이션 종료 시 자동 저장
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
