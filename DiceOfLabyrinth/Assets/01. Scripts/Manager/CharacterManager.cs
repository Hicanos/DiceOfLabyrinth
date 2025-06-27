using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// 모든 캐릭터 데이터와 유저가 획득한 캐릭터를 관리하는 매니저 (MonoBehaviour 미상속)
/// </summary>
public class CharacterManager
{
    // 싱글톤 인스턴스
    private static CharacterManager instance;
    public static CharacterManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CharacterManager();
                instance.Initialize();
            }
            return instance;
        }
    }

    // 모든 캐릭터 데이터 (Key: charID, Value: CharacterSO)
    public Dictionary<string, CharacterSO> AllCharacters { get; private set; } = new Dictionary<string, CharacterSO>();

    // 유저가 획득한 캐릭터 ID 목록
    private HashSet<string> acquiredCharacterIDs = new HashSet<string>();

    // 획득한 캐릭터 리스트 반환
    public List<CharacterSO> AcquiredCharacters =>
        acquiredCharacterIDs
            .Where(charID => AllCharacters.ContainsKey(charID))
            .Select(charID => AllCharacters[charID])
            .ToList();

    // Addressable 로드 완료 여부
    public bool IsLoaded { get; private set; } = false;

    // 초기화(최초 Instance 접근 시 1회만 호출)
    private void Initialize()
    {
        LoadAcquiredCharacters();
        LoadAllCharactersAsync();
    }

    // Addressable로 등록된 캐릭터 SO들을 비동기로 로드
    public void LoadAllCharactersAsync(System.Action onLoaded = null)
    {
        Addressables.LoadAssetsAsync<CharacterSO>("CharacterSO", null).Completed += handle =>
        {
            foreach (var so in handle.Result)
            {
                so.hideFlags = HideFlags.None;
            }
            AllCharacters = handle.Result.ToDictionary(c => c.charID, c => c);
            IsLoaded = true;
            onLoaded?.Invoke();
        };
    }

    // 유저가 획득한 캐릭터 목록 로드
    private void LoadAcquiredCharacters()
    {
        var saved = PlayerPrefs.GetString("AcquiredCharacters", "");
        if (!string.IsNullOrEmpty(saved))
        {
            var names = saved.Split(',').Where(s => !string.IsNullOrWhiteSpace(s));
            acquiredCharacterIDs = new HashSet<string>(names);
        }
    }

    // 캐릭터 획득
    public void AcquireCharacter(string charID)
    {
        if (AllCharacters.ContainsKey(charID) && !acquiredCharacterIDs.Contains(charID))
        {
            acquiredCharacterIDs.Add(charID);
            SaveAcquiredCharacters();
        }
    }

    // 보유(획득)한 캐릭터 삭제
    public void RemoveAcquiredCharacter(string charID)
    {
        if (acquiredCharacterIDs.Contains(charID))
        {
            acquiredCharacterIDs.Remove(charID);
            SaveAcquiredCharacters();
        }
    }

    // 획득한 캐릭터 저장
    private void SaveAcquiredCharacters()
    {
        var saveStr = string.Join(",", acquiredCharacterIDs);
        PlayerPrefs.SetString("AcquiredCharacters", saveStr);
        PlayerPrefs.Save();
    }

    // 특정 캐릭터를 획득했는지 확인
    public bool IsCharacterAcquired(string charID)
    {
        return acquiredCharacterIDs.Contains(charID);
    }
}
