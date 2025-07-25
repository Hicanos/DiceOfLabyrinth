using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

/// <summary>
/// 모든 게임데이터를 저장하는 역할의 매니저
/// json 파일로 저장함, 불러오는 건 Loader가 담당
/// </summary>
public class DataSaver
{
    // 저장1: 현재 유저의 재화정보, 레벨정보
    // 저장2: 현재 플레이어가 획득한 캐릭터 정보(강화 여부포함)
    // 저장3: 현재 플레이어가 클리어한 스테이지 정보
    // 저장4: 현재 플레이어가 획득한 아이템 정보
    
    // 해당 클래스는 static으로 선언하여 사용
    // 싱글톤 패턴추가(static)

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

    //[Serializable]
    //public class StageData
    //{
    //    // StageSaveData의 모든 필드 복사
    //    public int currentChapterIndex;
    //    public int currentStageIndex;
    //    public int currentPhaseIndex;
    //    public int normalStageCompleteCount;
    //    public int eliteStageCompleteCount;
    //    public int manaStone;
    //    public int savedExpReward;
    //    public int savedGoldReward;
    //    public int savedJewelReward;

    //    // enum, 리스트 등도 복사
    //    public int currentFormationType;
    //    public int currentPhaseState;
    //    //public List<ArtifactData> artifacts = new List<ArtifactData>(12);
    //    //public List<EngravingData> engravings = new List<EngravingData>(3);
    //    //public List<ArtifactData> equipedArtifacts = new List<ArtifactData>(4);
    //    //public List<CharacterSO> entryCharacters = new List<CharacterSO>(5);
    //    //public CharacterSO leaderCharacter;
    //    //public List<BattleCharacter> battleCharacters = new List<BattleCharacter>(5);
    //    //public EnemyData selectedEnemy;
    //    //public List<ChapterStates> chapterStates = new List<ChapterStates>();

    //    // 변환 생성자
    //    public StageData(StageSaveData saveData)
    //    {
    //        currentChapterIndex = saveData.currentChapterIndex;
    //        currentStageIndex = saveData.currentStageIndex;
    //        currentPhaseIndex = saveData.currentPhaseIndex;
    //        normalStageCompleteCount = saveData.normalStageCompleteCount;
    //        eliteStageCompleteCount = saveData.eliteStageCompleteCount;
    //        manaStone = saveData.manaStone;
    //        savedExpReward = saveData.savedExpReward;
    //        savedGoldReward = saveData.savedGoldReward;
    //        savedJewelReward = saveData.savedJewelReward;
    //        currentFormationType = (int)saveData.currentFormationType;
    //        currentPhaseState = (int)saveData.currentPhaseState;
    //        //artifacts = new List<ArtifactData>(saveData.artifacts);
    //        //engravings = new List<EngravingData>(saveData.engravings);
    //        //equipedArtifacts = new List<ArtifactData>(saveData.equipedArtifacts);
    //        //entryCharacters = new List<CharacterSO>(saveData.entryCharacters);
    //        //leaderCharacter = saveData.leaderCharacter;
    //        //battleCharacters = new List<BattleCharacter>(saveData.battleCharacters);
    //        //selectedEnemy = saveData.selectedEnemy;
    //        //chapterStates = new List<ChapterStates>(saveData.chapterStates);
    //    }

    //    // 역변환 메서드
    //    public StageSaveData ToStageSaveData()
    //    {
    //        var saveData = new StageSaveData();
    //        saveData.currentChapterIndex = currentChapterIndex;
    //        saveData.currentStageIndex = currentStageIndex;
    //        saveData.currentPhaseIndex = currentPhaseIndex;
    //        saveData.normalStageCompleteCount = normalStageCompleteCount;
    //        saveData.eliteStageCompleteCount = eliteStageCompleteCount;
    //        saveData.manaStone = manaStone;
    //        saveData.savedExpReward = savedExpReward;
    //        saveData.savedGoldReward = savedGoldReward;
    //        saveData.savedJewelReward = savedJewelReward;
    //        //saveData.currentFormationType = (StageSaveData.CurrentFormationType)currentFormationType;
    //        //saveData.currentPhaseState = (StageSaveData.CurrentPhaseState)currentPhaseState;
    //        //saveData.artifacts = new List<ArtifactData>(artifacts);
    //        //saveData.engravings = new List<EngravingData>(engravings);
    //        //saveData.equipedArtifacts = new List<ArtifactData>(equipedArtifacts);
    //        //saveData.entryCharacters = new List<CharacterSO>(entryCharacters);
    //        //saveData.leaderCharacter = leaderCharacter;
    //        //saveData.battleCharacters = new List<BattleCharacter>(battleCharacters);
    //        //saveData.selectedEnemy = selectedEnemy;
    //        //saveData.chapterStates = new List<ChapterStates>(chapterStates);
    //        return saveData;
    //    }
    //}

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
        //public StageData stageData; // null로 두고, 저장 시점에만 생성
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
            //if (StageManager.Instance != null && StageManager.Instance.stageSaveData != null)
            //    SaveData.stageData = new StageData(StageManager.Instance.stageSaveData);

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
        try
        {
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                SaveData = JsonConvert.DeserializeObject<GameSaveData>(json);

                // StageData → StageSaveData 변환 및 복원
                //if (SaveData.stageData != null && StageManager.Instance != null)
                //    StageManager.Instance.stageSaveData = SaveData.stageData.ToStageSaveData();
#if UNITY_EDITOR
                Debug.Log($"게임 데이터 로드됨: {SavePath}");
#endif
            }
            else
            {
                SaveData = new GameSaveData();
                Save(); // 초기화 후 새로 저장
#if UNITY_EDITOR
                Debug.Log("저장 파일이 없어 새 데이터로 초기화");
#endif
            }
        }
        catch (Exception ex) //Seserialize 과정에서 예외 발생 시
        {
            Debug.LogError($"게임 데이터 불러오기 실패: {ex.Message}");
            SaveData = new GameSaveData();
            Save(); // 초기화 후 새로 저장
        }

        CharacterManager.Instance.LoadAllCharactersAsync();
        ItemManager.Instance.LoadAllItemSOs();
    }
}
