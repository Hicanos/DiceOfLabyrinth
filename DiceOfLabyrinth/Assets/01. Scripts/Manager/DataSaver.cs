using UnityEngine;
using System.IO;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using NUnit.Framework;
using System.Collections.Generic;
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
        public int CritChance; // 치명타 확률
        public int CritDamage; // 치명타 피해량
    }

    [Serializable]
    public class StageData
    {
        // 플레이어가 클리어한 스테이지 정보
    }

    [Serializable]
    public class ItemData
    {
        // 플레이어가 획득한 아이템 정보 (스킬 강화 아이템 등)
    }

    /// <summary>
    /// userData, chractarData, stageData, itemData를 포함하는 게임 저장 데이터 클래스
    /// </summary>
    [Serializable]
    public class GameSaveData
    {
        public UserData userData = new UserData();
        public List<CharacterData> characters = new List<CharacterData>();
        // StageData, ItemData (List로 저장)
    }

    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "save.json");
    public GameSaveData SaveData = new GameSaveData();

    /// <summary>
    /// 게임 데이터 저장(json 파일)
    /// </summary>
    public void Save()
    {
        try
        {
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

    public void Load()
    {
        try
        {
            if(File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                SaveData = JsonConvert.DeserializeObject<GameSaveData>(json);
#if UNITY_EDITOR
                Debug.Log($"게임 데이터 로드됨: {SavePath}");
#endif
            }
            else
            {
                SaveData = new GameSaveData();
#if UNITY_EDITOR
                Debug.Log("저장 파일이 없어 새 데이터로 초기화");
#endif
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"게임 데이터 불러오기 실패: {ex.Message}");
            SaveData = new GameSaveData();
        }
    }
}
