using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class EnemyManager
{
    private static readonly Dictionary<string, EnemyData> enemyDict = new Dictionary<string, EnemyData>();
    private static readonly List<EnemyData> allEnemies = new List<EnemyData>();
    private static readonly List<AsyncOperationHandle<EnemyData>> handles = new List<AsyncOperationHandle<EnemyData>>();

    // Addressable 키 리스트는 외부에서 전달받음
    public static async Task LoadAllEnemiesAsync(List<string> addressableKeys)
    {
        enemyDict.Clear();
        allEnemies.Clear();
        handles.Clear();
        foreach (var key in addressableKeys)
        {
            if (string.IsNullOrEmpty(key)) continue;
            var handle = Addressables.LoadAssetAsync<EnemyData>(key);
            handles.Add(handle);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var so = handle.Result;
                allEnemies.Add(so);
                if (!string.IsNullOrEmpty(so.EnemyName))
                    enemyDict[so.EnemyName] = so;
            }
        }
    }

    public static EnemyData GetByName(string enemyName)
    {
        if (string.IsNullOrEmpty(enemyName)) return null;
        enemyDict.TryGetValue(enemyName, out var so);
        return so;
    }

    public static List<EnemyData> GetAllEnemies()
    {
        return allEnemies;
    }

    public static void ReleaseAllEnemies()
    {
        foreach (var handle in handles)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }
        allEnemies.Clear();
        enemyDict.Clear();
        handles.Clear();
    }
}
