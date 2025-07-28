using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class EngravingManager
{
    private static readonly Dictionary<string, EngravingData> engravingDict = new Dictionary<string, EngravingData>();
    private static readonly List<EngravingData> allEngravings = new List<EngravingData>();
    private static readonly List<AsyncOperationHandle<EngravingData>> handles = new List<AsyncOperationHandle<EngravingData>>();

    // Addressable 키 리스트는 외부에서 전달받음
    public static async Task LoadAllEngravingsAsync(List<string> addressableKeys)
    {
        engravingDict.Clear();
        allEngravings.Clear();
        handles.Clear();
        foreach (var key in addressableKeys)
        {
            if (string.IsNullOrEmpty(key)) continue;
            var handle = Addressables.LoadAssetAsync<EngravingData>(key);
            handles.Add(handle);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var so = handle.Result;
                allEngravings.Add(so);
                if (!string.IsNullOrEmpty(so.EngravingName))
                    engravingDict[so.EngravingName] = so;
            }
        }
    }

    public static EngravingData GetByName(string engravingName)
    {
        if (string.IsNullOrEmpty(engravingName)) return null;
        engravingDict.TryGetValue(engravingName, out var so);
        return so;
    }

    public static List<EngravingData> GetAllEngravings()
    {
        return allEngravings;
    }

    public static void ReleaseAllEngravings()
    {
        foreach (var handle in handles)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }
        allEngravings.Clear();
        engravingDict.Clear();
        handles.Clear();
    }
}