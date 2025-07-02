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

    // 보유한 캐릭터 데이터 (LobbyCharacter 리스트)
    public List<LobbyCharacter> OwnedCharacters { get; private set; } = new List<LobbyCharacter>();

    // BattleCharacter 관리용 컬렉션
    public Dictionary<string, BattleCharacter> BattleCharacters { get; private set; } = new Dictionary<string, BattleCharacter>();


    // Addressable 로드 완료 여부
    public bool IsLoaded { get; private set; } = false;

    // 초기화(최초 Instance 접근 시 1회만 호출)
    private void Initialize()
    {
        LoadAllCharactersAsync();
    }

    // Addressable로 등록된 캐릭터 SO들을 비동기로 로드
    public void LoadAllCharactersAsync(System.Action onLoaded = null)
    {
        Addressables.LoadAssetsAsync<CharacterSO>("CharacterSO", null).Completed += handle =>
        {
            AllCharacters = handle.Result.ToDictionary(c => c.charID, c => c);
            IsLoaded = true;
            LoadOwnedCharactersFromData();
            onLoaded?.Invoke();
        };
    }

    /// <summary>
    /// 저장 데이터에서 보유 캐릭터 리스트를 LobbyCharacter로 복원
    /// </summary>
    public void LoadOwnedCharactersFromData()
    {
        OwnedCharacters.Clear();
        foreach (var charData in DataSaver.Instance.SaveData.characters)
        {
            if (AllCharacters.TryGetValue(charData.CharacterID, out var so))
            {
                var lobbyChar = new LobbyCharacter();
                lobbyChar.Initialize(so, charData.Level);
                lobbyChar.CurrentExp = charData.CurrentExp;
                lobbyChar.RegularATK = charData.ATK;
                lobbyChar.RegularDEF = charData.DEF;
                lobbyChar.RegularHP = charData.HP;
                lobbyChar.CritChance = charData.CritChance;
                lobbyChar.CritDamage = charData.CritDamage;
                OwnedCharacters.Add(lobbyChar);
            }
        }
    }

    /// <summary>
    /// 캐릭터 획득 (중복 방지)
    /// </summary>
    public void AcquireCharacter(string charID)
    {
        if (OwnedCharacters.Any(c => c.CharacterData.charID == charID))
            return;

        if (AllCharacters.TryGetValue(charID, out var so))
        {
            var lobbyChar = new LobbyCharacter();
            lobbyChar.Initialize(so, 1);
            OwnedCharacters.Add(lobbyChar);
            DataSaver.Instance.SaveAllCharacters(OwnedCharacters);
        }
    }

    /// <summary>
    /// 보유 캐릭터 삭제
    /// </summary>
    public void RemoveCharacter(string charID)
    {
        var lobbyChar = OwnedCharacters.FirstOrDefault(c => c.CharacterData.charID == charID);
        if (lobbyChar != null)
        {
            OwnedCharacters.Remove(lobbyChar);
            DataSaver.Instance.SaveAllCharacters(OwnedCharacters);
        }
    }

    /// <summary>
    /// 전체 보유 캐릭터 저장
    /// </summary>
    public void SaveAll()
    {
        DataSaver.Instance.SaveAllCharacters(OwnedCharacters);
    }

    /// <summary>
    /// LobbyCharacter를 CharacterSO로 검색
    /// </summary>
    public LobbyCharacter GetLobbyCharacterBySO(CharacterSO so)
    {
        return OwnedCharacters.FirstOrDefault(lc => lc.CharacterData == so);
    }

    /// <summary>
    /// LobbyCharacter를 charID로 검색
    /// </summary>
    public LobbyCharacter GetLobbyCharacterByID(string charID)
    {
        return OwnedCharacters.FirstOrDefault(lc => lc.CharacterData != null && lc.CharacterData.charID == charID);
    }

    /// <summary>
    /// BattleCharacter 데이터만 등록 (프리팹 생성 X)
    /// 이미 등록되어 있으면 지우고 새로 생성
    /// </summary>
    public BattleCharacter RegisterBattleCharacterData(string charID)
    {
        // 이미 등록된 BattleCharacter가 있으면 지우고 새로 생성
        if (BattleCharacters.TryGetValue(charID, out var battleChar))
        {
            BattleCharacters.Remove(charID);
        }

        // 캐릭터 SO가 존재하는지 확인
        if (!AllCharacters.TryGetValue(charID, out var so))
            throw new System.Exception($"존재하지 않는 캐릭터 ID: {charID}");
        
        // BattleCharacter를 so를 기반으로 생성 (내부에서 LobbyCharacter까지 읽어옴)
        var battleData = new BattleCharacter(so);
        BattleCharacters[charID] = battleData;
        return battleData;
    }

    /// <summary>
    /// BattleCharacter 데이터 제거 (프리팹 오브젝트는 따로 관리)
    /// </summary>
    public void UnregisterBattleCharacterData(string charID)
    {
        if (BattleCharacters.ContainsKey(charID))
            BattleCharacters.Remove(charID);
    }

    /// <summary>
    /// BattleCharacter 데이터 조회
    /// </summary>
    public BattleCharacter GetBattleCharacterData(string charID)
    {
        BattleCharacters.TryGetValue(charID, out var battleChar);
        return battleChar;
    }

}
