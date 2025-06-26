using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// 모든 캐릭터 데이터와 유저가 획득한 캐릭터를 관리하는 매니저
/// </summary>
public class CharacterManager : MonoBehaviour
{
    //싱글톤패턴

    public static CharacterManager Instance { get; private set; }
   


    // 모든 캐릭터 데이터 (Key: charID, Value: CharacterSO)
    public Dictionary<string, CharacterSO> AllCharacters { get; private set; } = new Dictionary<string, CharacterSO>();

    // 유저가 획득한 캐릭터 ID 목록
    private HashSet<string> acquiredCharacterIDs = new HashSet<string>();

    // 획득한 캐릭터 리스트 반환
    // acquiredList는 List<CharacterSO> 타입이며, 각 캐릭터의 상세 정보에 접근 가능
    // 아래와 같이 사용 가능
    //    var acquiredList = characterManager.AcquiredCharacters;
    //foreach (var character in acquiredList)
    //{
    //    Debug.Log($"ID: {character.charID}, 이름: {character.nameKr}, 공격력: {character.baseATK}");
    //}

    // 특정 캐릭터 중 하나만 찾을 경우 아래와 같이 사용

    /*
     string targetCharID = "원하는_charID";
    var acquired = characterManager.AcquiredCharacters
     .FirstOrDefault(c => c.charID == targetCharID);

    if (acquired != null)
    {
       Debug.Log($"획득한 캐릭터: {acquired.nameKr}");
    }
    else
    {
        Debug.LogWarning("획득한 캐릭터 중 해당 charID가 없습니다.");
    }
     */

    public List<CharacterSO> AcquiredCharacters => 
        acquiredCharacterIDs
            .Where(charID => AllCharacters.ContainsKey(charID))
            .Select(charID => AllCharacters[charID])
            .ToList();

    // Addressable로 등록된 캐릭터 SO들을 로드
    // 캐릭터는 Addressable에 등록되어있으며, Character SO Group에 위치함
    // CharacterSO의 위치 Assets/01. Scripts/Character/SO/Generated/{nameEn}_SO.asset
    // Resources 폴더를 사용하지 않고 Addressable을 사용하여 캐릭터 데이터를 관리
    // Resources.LoadAll을 사용하지 않고 Addressable을 통해 캐릭터 데이터를 로드

    // Addressable 로드 완료 여부
    public bool IsLoaded { get; private set; } = false;

    public void LoadAllCharactersAsync(System.Action onLoaded = null)
    {
        // "Character SO Group"에 등록된 모든 CharacterSO를 Addressable Label로 관리한다고 가정
        Addressables.LoadAssetsAsync<CharacterSO>("CharacterSO", null).Completed += handle =>
        {
            // hideFlags를 명시적으로 None으로 설정
            foreach (var so in handle.Result)
            {
                so.hideFlags = HideFlags.None;
            }
            AllCharacters = handle.Result.ToDictionary(c => c.charID, c => c);
            IsLoaded = true;
            onLoaded?.Invoke();
        };
    }

    void Awake()
    {
        LoadAcquiredCharacters();
        LoadAllCharactersAsync();
    }

    // 유저가 획득한 캐릭터 nameKr 목록 로드 (PlayerPrefs 등에서 불러오기)
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
