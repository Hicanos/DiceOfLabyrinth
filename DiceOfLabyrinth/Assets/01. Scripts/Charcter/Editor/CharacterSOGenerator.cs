using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine.TextCore.Text;

/// <summary>
/// JSON 파일을 읽어 CharacterSO를 자동으로 생성하는 에디터 윈도우
/// 생성된 SO를 Addressable로 자동 등록
/// </summary>
public class CharacterSOGenerator : EditorWindow
{
    // JSON 파일 경로와 SO 저장 경로
    private string jsonPath = "Assets/Resources/Json/CharData.json";
    private string soOutputPath = "Assets/01. Scripts/Charcter/SO/Generated/";


    /// <summary>
    /// Unity 에디터 메뉴에 CharacterSO Generator를 추가
    /// </summary>
    [MenuItem("Tools/CharacterSO Generator")]
    public static void ShowWindow()
    {
        GetWindow<CharacterSOGenerator>("CharacterSO Generator");
    }

    /// <summary>
    /// 에디터 윈도우의 GUI를 정의
    /// </summary>
    private void OnGUI()
    {
        GUILayout.Label("CharacterSO 자동 생성기", EditorStyles.boldLabel);
        //JSON 파일 경로 입력 필드
        jsonPath = EditorGUILayout.TextField("JSON 경로", jsonPath);

        //SO 저장 경로 입력 필드
        soOutputPath = EditorGUILayout.TextField("SO 저장 경로", soOutputPath);

        // "생성" 버튼 => SO 생성 함수 호출
        if (GUILayout.Button("생성"))
        {
            GenerateCharacterSOs();
        }
    }

    /// <summary>
    /// JSON 파일을 읽어 CharacterSO를 생성하고 Addressable로 등록
    /// </summary>
    private void GenerateCharacterSOs()
    {
        // JSON 파일이 존재하는지 확인
        if (!File.Exists(jsonPath))
        {
            Debug.LogError("JSON 파일을 찾을 수 없습니다: " + jsonPath);
            return;
        }

        // JSON 파일 읽기 후 역직렬화
        string json = File.ReadAllText(jsonPath);
        CharDataListWrapper wrapper = JsonConvert.DeserializeObject<CharDataListWrapper>(json);

        // 역직렬화 실패 또는 데이터 없음
        if (wrapper == null || wrapper.Items == null)
        {
            Debug.LogError("JSON 파싱 실패 또는 Items가 비어있음");
            return;
        }

        // SO 저장 경로가 존재하지 않으면 생성
        if (!Directory.Exists(soOutputPath))
            Directory.CreateDirectory(soOutputPath);

        // 각 캐릭터 데이터에 대해 SO 생성 및 Addressable 등록 - 인스턴스 생성 및 데이터 할당
        foreach (var data in wrapper.Items)
        {
            CharacterSO so = ScriptableObject.CreateInstance<CharacterSO>();
            so.key = data.key;
            so.charID = data.CharID;
            so.nameKr = data.NameKr;
            so.nameEn = data.NameEn;
            so.classType = data.ClassType;
            so.baseATK = data.BaseATK;
            so.plusATK = data.PlusATK;
            so.baseDEF = data.BaseDEF;
            so.plusDEF = data.PlusDEF;
            so.baseHP = data.BaseHP;
            so.plusHP = data.PlusHP;
            so.critChance = data.CritChance;
            so.critDamage = data.CritDamage;
            so.elementType = data.ElementType;
            so.signitureNum = data.SignitureNum;
            so.diceID = data.DiceID;
            so.description = data.Description;
            so.dialog1 = data.dialog1;
            so.dialog2 = data.dialog2;

            // 프리팹 할당
            if (!string.IsNullOrEmpty(data.LobbyPrefabPath))
            {
                var LobbyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(data.LobbyPrefabPath);
                so.charLobbyPrefab = LobbyPrefab;
            }

            if (!string.IsNullOrEmpty(data.BattlePrefabPath))
            {
                var BattlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(data.BattlePrefabPath);
                so.charBattlePrefab = BattlePrefab;
            }

            // SO 파일 경로 설정
            string assetPath = $"{soOutputPath}{so.nameEn}_SO.asset";

            // 기존 SO 파일이 있으면 삭제
            if (File.Exists(assetPath))
                AssetDatabase.DeleteAsset(assetPath);

            AssetDatabase.CreateAsset(so, assetPath);

            // Addressable 등록
            var Asettings = AddressableAssetSettingsDefaultObject.Settings;
            if (Asettings != null)
            {
                // "Character SO Group" 그룹 찾기 또는 생성
                var groupName = "Character SO Group";
                var group = Asettings.FindGroup(groupName);
                if (group == null)
                {
                    group = Asettings.CreateGroup(groupName, false, false, false, null, typeof(BundledAssetGroupSchema));
                }

                string guid = AssetDatabase.AssetPathToGUID(assetPath);
                var entry = Asettings.CreateOrMoveEntry(guid, group);
                entry.address = so.nameEn; // address로 nameEn 사용
            }
        }

        // 에셋 저장 및 데이터 베이스 갱신
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Addressable 설정 업데이트
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings != null)
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, null, true);

        Debug.Log("CharacterSO 생성 및 Addressable 등록 완료!");
    }

    /// <summary>
    /// 역직렬화를 위한 래퍼(캐릭터 데이터 리스트 보관)
    /// </summary>

    [System.Serializable]
    private class CharDataListWrapper
    {
        public List<CharDataForSO> Items;
    }

    /// <summary>
    /// 
    /// JSON 데이터 구조와 매칭되는 캐릭터 데이터 클래스
    /// </summary>
    [System.Serializable]
    private class CharDataForSO
    {
        public int key;
        public string CharID;
        public string NameKr;
        public string NameEn;
        public DesignEnums.ClassTypes ClassType;
        public int BaseATK;
        public int PlusATK;
        public int BaseDEF;
        public int PlusDEF;
        public int BaseHP;
        public int PlusHP;
        public int CritChance;
        public int CritDamage;
        public DesignEnums.ElementTypes ElementType;
        public int SignitureNum;
        public int DiceID;
        public string Description;
        public string dialog1;
        public string dialog2;
        public string LobbyPrefabPath;
        public string BattlePrefabPath; // 프리팹 경로 추가
    }
}