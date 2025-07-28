using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class ArtifactManager
{
    private static readonly Dictionary<string, ArtifactData> artifactDict = new Dictionary<string, ArtifactData>();
    private static readonly List<ArtifactData> allArtifacts = new List<ArtifactData>();
    private static readonly List<AsyncOperationHandle<ArtifactData>> handles = new List<AsyncOperationHandle<ArtifactData>>();


    // Addressable 키 리스트는 외부에서 전달받음 (예: GameManager에서 Addressables.ResourceLocators로 수집)
    public static async Task LoadAllArtifactsAsync(List<string> addressableKeys)
    {
        artifactDict.Clear();
        allArtifacts.Clear();
        handles.Clear();
        foreach (var key in addressableKeys)
        {
            if (string.IsNullOrEmpty(key)) continue;
            var handle = Addressables.LoadAssetAsync<ArtifactData>(key);
            handles.Add(handle);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var so = handle.Result;
                allArtifacts.Add(so);
                if (!string.IsNullOrEmpty(so.ArtifactName))
                    artifactDict[so.ArtifactName] = so;
            }
        }
    }

    public static ArtifactData GetByName(string artifactName)
    {
        if (string.IsNullOrEmpty(artifactName)) return null;
        artifactDict.TryGetValue(artifactName, out var so);
        return so;
    }

    public static List<ArtifactData> GetAllArtifacts()
    {
        return allArtifacts;
    }

    public static void ReleaseAllArtifacts()
    {
        foreach (var handle in handles)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }
        allArtifacts.Clear();
        artifactDict.Clear();
        handles.Clear();
    }
}