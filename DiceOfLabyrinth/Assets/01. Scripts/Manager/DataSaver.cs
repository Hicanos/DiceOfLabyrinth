using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

/// <summary>
/// 모든 게임데이터를 저장하는 역할의 매니저
/// json 파일로 저장함, 불러오는 건 Loader가 담당
/// </summary>
public class DataSaver
{
    // 싱글톤 패턴
    public static DataSaver Instance { get; private set; }
    static DataSaver()
    {
        Instance = new DataSaver();
        Debug.Log("DataSaver 인스턴스 생성됨");
    }

    [Serializable]
    public class UserData
    {
        //유저의 레벨, 재화 정보 보관
    }

    [Serializable]
    public class CharacterData
    {
        // 플레이어가 보유한 캐릭터 정보와 그 캐릭터의 레벨 정보(스킬, 주사위 포함)
        // 고유 식별자 : CharacterSO에서 캐릭터 정보를 가져올 수 있음
        public string CharacterID; // 캐릭터 ID
        public int Level; // 캐릭터 레벨
        public int CurrentExp; // 현재 경험치

        // 캐릭터의 능력치
        public int ATK; // 공격력
        public int DEF; // 방어력
        public int HP; // 체력
        public float CritChance; // 치명타 확률
        public float CritDamage; // 치명타 피해량
        // 캐릭터의 스킬 정보 - SkillData 리스트로 저장
        // public List<SkillData> Skills = new List<SkillData>(); // 각 캐릭터가 보유한 스킬 정보

        // 생성자
        public CharacterData() { }
        public CharacterData(string characterID, int level, int atk, int def, int hp, float critChance, float critDamage)
        {
            CharacterID = characterID;
            Level = level;
            ATK = atk;
            DEF = def;
            HP = hp;
            CritChance = critChance;
            CritDamage = critDamage;
        }
    }

    [Serializable]
    public class SkillData
    {
        // 캐릭터의 스킬 정보 (스킬 레벨, 효과 등)
        public string SkillID; // 스킬 ID
        public int Level; // 스킬 레벨
        public int Cooldown; // 쿨타임
        public int Power; // 스킬 파워
    }

    [Serializable]
    public class StageData
    {
        // StageSaveData의 모든 필드 복사 (ScriptableObject는 이름/ID만 저장)
        public int currentChapterIndex;
        public int currentStageIndex;
        public int currentPhaseIndex;
        public int normalStageCompleteCount;
        public int eliteStageCompleteCount;
        public int manaStone;
        public int savedExpReward;
        public int savedGoldReward;
        public int savedJewelReward;

        public int currentFormationType;
        public int currentPhaseState;

        // ScriptableObject는 이름/ID만 저장
        public List<string> artifactNames = new List<string>(12);
        public List<string> engravingNames = new List<string>(3);
        public List<string> equipedArtifactNames = new List<string>(4);
        public List<string> entryCharacterIDs = new List<string>(5);
        public string leaderCharacterID;
        public List<BattleCharacterData> battleCharacters = new List<BattleCharacterData>(5);
        public string selectedEnemyID;
        public List<ChapterStates> chapterStates = new List<ChapterStates>();

        // 역직렬화용 기본 생성자
        public StageData() { }

        // 변환 생성자(직렬화/역직렬화에는 사용하지 않음, 오직 코드 내 변환용)
        public StageData(StageSaveData saveData)
        {
            if (saveData == null) return;

            currentChapterIndex = saveData.currentChapterIndex;
            currentStageIndex = saveData.currentStageIndex;
            currentPhaseIndex = saveData.currentPhaseIndex;
            normalStageCompleteCount = saveData.normalStageCompleteCount;
            eliteStageCompleteCount = saveData.eliteStageCompleteCount;
            manaStone = saveData.manaStone;
            savedExpReward = saveData.savedExpReward;
            savedGoldReward = saveData.savedGoldReward;
            savedJewelReward = saveData.savedJewelReward;
            currentFormationType = (int)saveData.currentFormationType;
            currentPhaseState = (int)saveData.currentPhaseState;

            artifactNames = saveData.artifacts?.Select(a => a != null ? a.name : null).ToList() ?? new List<string>(new string[12]);
            engravingNames = saveData.engravings?.Select(e => e != null ? e.name : null).ToList() ?? new List<string>(new string[3]);
            equipedArtifactNames = saveData.equipedArtifacts?.Select(a => a != null ? a.name : null).ToList() ?? new List<string>(new string[4]);
            entryCharacterIDs = saveData.entryCharacters?.Select(c => c != null ? c.name : null).ToList() ?? new List<string>(new string[5]);
            leaderCharacterID = saveData.leaderCharacter != null ? saveData.leaderCharacter.name : null;
            battleCharacters = saveData.battleCharacters?.Select(bc => new BattleCharacterData(bc)).ToList() ?? new List<BattleCharacterData>();
            selectedEnemyID = saveData.selectedEnemy != null ? saveData.selectedEnemy.name : null;
            chapterStates = saveData.chapterStates != null ? new List<ChapterStates>(saveData.chapterStates) : new List<ChapterStates>();
        }

        // 역변환 메서드
        public StageSaveData ToStageSaveData()
        {
            var saveData = new StageSaveData();
            saveData.currentChapterIndex = currentChapterIndex;
            saveData.currentStageIndex = currentStageIndex;
            saveData.currentPhaseIndex = currentPhaseIndex;
            saveData.normalStageCompleteCount = normalStageCompleteCount;
            saveData.eliteStageCompleteCount = eliteStageCompleteCount;
            saveData.manaStone = manaStone;
            saveData.savedExpReward = savedExpReward;
            saveData.savedGoldReward = savedGoldReward;
            saveData.savedJewelReward = savedJewelReward;
            saveData.currentFormationType = (StageSaveData.CurrentFormationType)currentFormationType;
            saveData.currentPhaseState = (StageSaveData.CurrentPhaseState)currentPhaseState;

            saveData.artifacts = artifactNames.Select(name => ArtifactDataLoader.GetByName(name)).ToList();
            saveData.engravings = engravingNames.Select(name => EngravingDataLoader.GetByName(name)).ToList();
            saveData.equipedArtifacts = equipedArtifactNames.Select(name => ArtifactDataLoader.GetByName(name)).ToList();
            saveData.entryCharacters = entryCharacterIDs.Select(id => CharacterSOLoader.GetByName(id)).ToList();
            saveData.leaderCharacter = CharacterSOLoader.GetByName(leaderCharacterID);
            // BattleCharacter 복원은 별도 로직 필요
            // saveData.battleCharacters = battleCharacters.Select(bc => bc.ToBattleCharacter()).ToList();
            saveData.selectedEnemy = EnemyDataLoader.GetByName(selectedEnemyID);
            saveData.chapterStates = new List<ChapterStates>(chapterStates);
            return saveData;
        }
    }

    [Serializable]
    public class BattleCharacterData
    {
        public string charID;
        public int level;
        public int currentHP; // 현재 체력
        public int currentATK; // 현재 공격력
        public int currentDEF; // 현재 방어력
        public float currentCritChance; // 현재 치명타 확률
        public float currentCritDamage; // 현재 치명타 피해량
        public float currentPenetration; // 현재 관통력
        public int regularHP;
        public int regularATK;
        public int regularDEF;
        public float regularCritChance;
        public float regularCritDamage;
        public float regularPenetration;

        // 역직렬화용 기본 생성자
        public BattleCharacterData() { }

        // 변환 생성자(직렬화/역직렬화에는 사용하지 않음, 오직 코드 내 변환용)
        public BattleCharacterData(BattleCharacter bc)
        {
            if (bc == null) return;
            charID = bc.CharID;
            level = bc.Level;
            currentHP = bc.CurrentHP;
            currentATK = bc.CurrentATK;
            currentDEF = bc.CurrentDEF;
            currentCritChance = bc.CurrentCritChance;
            currentCritDamage = bc.CurrentCritDamage;
            currentPenetration = bc.CurrentPenetration;

            regularHP = bc.RegularHP;
            regularATK = bc.RegularATK;
            regularDEF = bc.RegularDEF;
            regularCritChance = bc.RegularCritChance;
            regularCritDamage = bc.RegularCritDamage;
            regularPenetration = bc.Penetration;
        }

        // BattleCharacterData를 BattleCharacter에 복원
        public void LoadBattleCharacter(BattleCharacter bc, BattleCharacterData data)
        {
            bc.DataSetting(data);
        }

    }

    [Serializable]
    public class ItemData
    {
        // 플레이어가 획득한 아이템 정보 (스킬 강화 아이템 등)
        // ItemManager의 Dictionary<string, int> ownedItems를 기반으로 저장
        public string ItemID; // 아이템 ID
        public int Quantity; // 아이템 개수

        // 생성자
        public ItemData() { }
        public ItemData(string itemID, int quantity)
        {
            ItemID = itemID;
            Quantity = quantity;
        }
    }

    /// <summary>
    /// userData, chractarData, stageData, itemData를 포함하는 게임 저장 데이터 클래스
    /// </summary>
    [Serializable]
    public class GameSaveData
    {
        public UserData userData = new UserData();
        public List<CharacterData> characters = new List<CharacterData>();
        public List<ItemData> items = new List<ItemData>();
        public StageData stageData;
    }

    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "save.json");
    public GameSaveData SaveData = new GameSaveData();

    /// <summary>
    /// 캐릭터 정보 동기화
    /// </summary>
    public void SyncCharacterData()
    {
        if (CharacterManager.Instance != null)
        {
            var lobbyCharacters = CharacterManager.Instance.OwnedCharacters;
            SaveData.characters = lobbyCharacters.Select(lobbyChar => new CharacterData
            {
                CharacterID = lobbyChar.CharacterData.charID,
                Level = lobbyChar.Level,
                CurrentExp = lobbyChar.CurrentExp,
                ATK = lobbyChar.RegularATK,
                DEF = lobbyChar.RegularDEF,
                HP = lobbyChar.RegularHP,
                CritChance = lobbyChar.CritChance,
                CritDamage = lobbyChar.CritDamage
            }).ToList();
        }
    }

    /// <summary>
    /// 아이템 정보 동기화
    /// </summary>
    public void SyncItemData()
    {
        if (ItemManager.Instance != null)
        {
            var items = ItemManager.Instance.OwnedItems;
            SaveData.items = items.Select(item => new ItemData(item.Key, item.Value)).ToList();
        }
    }

    /// <summary>
    /// 게임 데이터 저장(json 파일)
    /// </summary>
    public void Save()
    {
        try
        {
            SyncCharacterData();
            SyncItemData();

            // StageSaveData → StageData 변환 및 저장
            if (StageManager.Instance != null && StageManager.Instance.stageSaveData != null)
                SaveData.stageData = new StageData(StageManager.Instance.stageSaveData);

            string json = JsonConvert.SerializeObject(SaveData, Formatting.Indented);
            File.WriteAllText(SavePath, json);
#if UNITY_EDITOR
            Debug.Log($"게임 데이터 저장됨: {SavePath}");
#endif
        }
        catch (Exception ex)
        {
            Debug.LogError($"게임 데이터 저장 실패: {ex.Message}");
        }
    }

    /// <summary>
    /// 개별 캐릭터 저장(추가/갱신)
    /// </summary>
    public void SaveCharacter(LobbyCharacter lobbyChar)
    {
        var charData = new CharacterData
        {
            CharacterID = lobbyChar.CharacterData.charID,
            Level = lobbyChar.Level,
            CurrentExp = lobbyChar.CurrentExp,
            ATK = lobbyChar.RegularATK,
            DEF = lobbyChar.RegularDEF,
            HP = lobbyChar.RegularHP,
            CritChance = lobbyChar.CritChance,
            CritDamage = lobbyChar.CritDamage
        };

        int idx = SaveData.characters.FindIndex(c => c.CharacterID == charData.CharacterID);
        if (idx >= 0)
            SaveData.characters[idx] = charData;
        else
            SaveData.characters.Add(charData);

        Save();
    }

    /// <summary>
    /// 전체 캐릭터 저장
    /// </summary>
    public void SaveAllCharacters(List<LobbyCharacter> lobbyCharacters)
    {
        SaveData.characters = lobbyCharacters.Select(lobbyChar => new CharacterData
        {
            CharacterID = lobbyChar.CharacterData.charID,
            Level = lobbyChar.Level,
            CurrentExp = lobbyChar.CurrentExp,
            ATK = lobbyChar.RegularATK,
            DEF = lobbyChar.RegularDEF,
            HP = lobbyChar.RegularHP,
            CritChance = lobbyChar.CritChance,
            CritDamage = lobbyChar.CritDamage
        }).ToList();

        Save();
    }

    public void SaveItems(Dictionary<string, int> items)
    {
        SaveData.items = items.Select(item => new ItemData(item.Key, item.Value)).ToList();
        Save();
    }

    /// <summary>
    /// 저장된 게임데이터 로드
    /// </summary>
    public void Load()
    {
        string json = "";
        try
        {
            if (File.Exists(SavePath))
            {
                json = File.ReadAllText(SavePath);
                SaveData = JsonConvert.DeserializeObject<GameSaveData>(json);

                EnsureStageDataIntegrity();

                Debug.Log($"stageData null? {SaveData.stageData == null}");
                Debug.Log($"artifactNames null? {SaveData.stageData.artifactNames == null}");
                Debug.Log($"engravingNames null? {SaveData.stageData.engravingNames == null}");
                Debug.Log($"chapterStates null? {SaveData.stageData.chapterStates == null}");

                if (SaveData.stageData != null && StageManager.Instance != null)
                    StageManager.Instance.stageSaveData = SaveData.stageData.ToStageSaveData();
                
#if UNITY_EDITOR
                Debug.Log($"게임 데이터 로드됨: {SavePath}");
#endif
            }
            else
            {
                SaveData = new GameSaveData();
                Save();
#if UNITY_EDITOR
                Debug.Log("저장 파일이 없어 새 데이터로 초기화");
#endif
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"게임 데이터 불러오기 실패: {ex.Message}\n{ex.StackTrace}\njson: {json}");
            SaveData = new GameSaveData();
            Save();
        }

        CharacterManager.Instance.LoadAllCharactersAsync();
        ItemManager.Instance.LoadAllItemSOs();
    }

    private void EnsureStageDataIntegrity()
    {
        if (SaveData.stageData == null)
            SaveData.stageData = new StageData(new StageSaveData());
        if (SaveData.stageData.artifactNames == null || SaveData.stageData.artifactNames.Count != 12)
            SaveData.stageData.artifactNames = new List<string>(new string[12]);
        if (SaveData.stageData.engravingNames == null || SaveData.stageData.engravingNames.Count != 3)
            SaveData.stageData.engravingNames = new List<string>(new string[3]);
        if (SaveData.stageData.equipedArtifactNames == null || SaveData.stageData.equipedArtifactNames.Count != 4)
            SaveData.stageData.equipedArtifactNames = new List<string>(new string[4]);
        if (SaveData.stageData.entryCharacterIDs == null || SaveData.stageData.entryCharacterIDs.Count != 5)
            SaveData.stageData.entryCharacterIDs = new List<string>(new string[5]);
        if (SaveData.stageData.battleCharacters == null)
            SaveData.stageData.battleCharacters = new List<BattleCharacterData>();
        if (SaveData.stageData.chapterStates == null)
            SaveData.stageData.chapterStates = new List<ChapterStates>();
    }
}

// 아래는 ScriptableObject를 이름으로 찾아주는 로더 예시(실제 구현 필요)
public static class ArtifactDataLoader
{
    public static ArtifactData GetByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        // 동기 Addressables.LoadAsset은 지원하지 않으므로, 미리 캐싱하거나 비동기 사용 권장
        return AddressableCache<ArtifactData>.GetOrLoad(name);
    }

    public static async Task<ArtifactData> GetByNameAsync(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        var handle = Addressables.LoadAssetAsync<ArtifactData>(name);
        await handle.Task;
        return handle.Status == AsyncOperationStatus.Succeeded ? handle.Result : null;
    }
}

public static class EngravingDataLoader
{
    public static EngravingData GetByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        return AddressableCache<EngravingData>.GetOrLoad(name);
    }

    public static async Task<EngravingData> GetByNameAsync(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        var handle = Addressables.LoadAssetAsync<EngravingData>(name);
        await handle.Task;
        return handle.Status == AsyncOperationStatus.Succeeded ? handle.Result : null;
    }
}

public static class CharacterSOLoader
{
    public static CharacterSO GetByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        return AddressableCache<CharacterSO>.GetOrLoad(name);
    }

    public static async Task<CharacterSO> GetByNameAsync(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        var handle = Addressables.LoadAssetAsync<CharacterSO>(name);
        await handle.Task;
        return handle.Status == AsyncOperationStatus.Succeeded ? handle.Result : null;
    }
}

public static class EnemyDataLoader
{
    public static EnemyData GetByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        return AddressableCache<EnemyData>.GetOrLoad(name);
    }

    public static async Task<EnemyData> GetByNameAsync(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        var handle = Addressables.LoadAssetAsync<EnemyData>(name);
        await handle.Task;
        return handle.Status == AsyncOperationStatus.Succeeded ? handle.Result : null;
    }
}

// Addressable 캐싱 유틸리티 (동기 접근을 위한 임시 캐시)
public static class AddressableCache<T> where T : UnityEngine.Object
{
    private static Dictionary<string, T> cache = new Dictionary<string, T>();

    public static T GetOrLoad(string key)
    {
        if (string.IsNullOrEmpty(key)) return null;
        if (cache.TryGetValue(key, out var obj) && obj != null)
            return obj;

        // 동기 로드 불가: 실제 게임에서는 미리 로드하거나, 비동기 로드 후 캐싱 필요
#if UNITY_EDITOR
        Debug.LogWarning($"Addressable '{key}'을(를) 동기로 로드할 수 없습니다. 미리 캐싱하거나 비동기 로드를 사용하세요.");
#endif
        return null;
    }

    public static void Cache(string key, T obj)
    {
        if (!string.IsNullOrEmpty(key) && obj != null)
            cache[key] = obj;
    }
}
