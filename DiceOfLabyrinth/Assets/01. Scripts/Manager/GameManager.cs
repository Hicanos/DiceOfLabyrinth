using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 전체의 데이터 로드, 복구, 릴리즈를 총괄하는 매니저
/// - 모든 SO/데이터를 비동기로 로드
/// - 데이터 복구는 SO 로드 완료 후에만 진행
/// - 게임 종료/초기화/씬 전환 시 Addressables 릴리즈
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Addressable 키 리스트 (Inspector에서 할당하거나, 프로젝트에서 관리)
    public List<string> ArtifactAddressableKeys;
    public List<string> EngravingAddressableKeys;
    public List<string> EnemyAddressableKeys;
    public List<string> ItemAddressableKeys;

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
    /// 게임 시작 시점에 모든 SO/데이터를 비동기로 로드 후 복구
    /// </summary>
    private IEnumerator Start()
    {
        // StageManager.Instance가 생성될 때까지 대기
        while (StageManager.Instance == null)
            yield return null;

        // 비동기 데이터 로드 및 복구
        yield return LoadAllGameDataCoroutine();
    }

    /// <summary>
    /// 모든 SO/데이터를 비동기로 로드 후 DataSaver 복구
    /// </summary>
    private IEnumerator LoadAllGameDataCoroutine()
    {
        // 1. Addressables SO 비동기 로드 (순서 중요)
        var artifactTask = ArtifactManager.LoadAllArtifactsAsync(ArtifactAddressableKeys);
        var engravingTask = EngravingManager.LoadAllEngravingsAsync(EngravingAddressableKeys);
        var enemyTask = EnemyManager.LoadAllEnemiesAsync(EnemyAddressableKeys);
        var characterTask = CharacterManager.Instance.LoadAllCharactersAsync();
        var itemTask = ItemManager.Instance.LoadAllItemSOs();

        // 모든 Task가 끝날 때까지 대기
        while (!artifactTask.IsCompleted || !engravingTask.IsCompleted || !enemyTask.IsCompleted ||
               !characterTask.IsCompleted || !itemTask.IsCompleted)
        {
            yield return null;
        }

        // SO가 모두 로드된 후에만 DataSaver 복구
        RestoreGameData();
        Debug.Log("게임 데이터 및 SO 로드/복구 완료");
    }

    private void RestoreGameData()
    {
        DataSaver.Instance.Load();
        CharacterManager.Instance.LoadOwnedCharactersFromData();
        ItemManager.Instance.LoadOwnedItemsFromData();
        if (StageManager.Instance != null && DataSaver.Instance.SaveData.stageData != null)
        {
            StageManager.Instance.stageSaveData = DataSaver.Instance.SaveData.stageData.ToStageSaveData();
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
    /// 게임 종료 (저장 후 Addressables 릴리즈)
    /// </summary>
    public void ExitGame()
    {
        SaveGame();
        ReleaseAllAddressables();
        Debug.Log("게임 종료됨");
        Application.Quit();
    }

    /// <summary>
    /// 게임 초기화 (저장 후 Addressables 릴리즈)
    /// </summary>
    public void ResetGame()
    {
        SaveGame();
        ReleaseAllAddressables();
        Debug.Log("게임 초기화됨");
        // 추가: 데이터 파일 삭제, 씬 재시작 등
    }

    /// <summary>
    /// 애플리케이션 종료 시 자동 저장 및 릴리즈
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveGame();
        ReleaseAllAddressables();
    }

    /// <summary>
    /// 모든 Addressables SO/핸들 릴리즈 (메모리 해제)
    /// </summary>
    private void ReleaseAllAddressables()
    {
        ArtifactManager.ReleaseAllArtifacts();
        EngravingManager.ReleaseAllEngravings();
        EnemyManager.ReleaseAllEnemies();
        CharacterManager.Instance.ReleaseAllCharacters();
        ItemManager.Instance.ReleaseAllItems();
    }
}
