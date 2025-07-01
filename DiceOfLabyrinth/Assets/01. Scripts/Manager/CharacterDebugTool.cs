using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// 에디터에서 캐릭터 획득 및 획득 정보 확인을 위한 디버그 툴
/// </summary>
public class CharacterDebugTool : EditorWindow
{
    // 모든 캐릭터 목록 스크롤 위치
    private Vector2 scrollAll;
    // 획득한 캐릭터 목록 스크롤 위치
    private Vector2 scrollAcquired;
    // 직접 입력으로 획득할 캐릭터 ID
    private string acquireCharID = "";

    // 경험치 부여용 입력값
    private int addExpValue = 100;

    // LobbyCharacter 프리팹을 에디터에서 할당
    private GameObject lobbyCharacterPrefab;

    // 현재 씬에 생성된 LobbyCharacter 인스턴스 관리
    private List<LobbyCharacter> spawnedLobbyCharacters = new List<LobbyCharacter>();

    // 에디터 메뉴에 툴 등록
    [MenuItem("Tools/Character Debug Tool")]
    public static void ShowWindow()
    {
        GetWindow<CharacterDebugTool>("Character Debug Tool");
    }

    private void OnEnable()
    {        
        RefreshSpawnedLobbyCharacters();
    }

    private void OnFocus()
    {
        RefreshSpawnedLobbyCharacters();
    }

    private void OnHierarchyChange()
    {
        RefreshSpawnedLobbyCharacters();
    }

    private void RefreshSpawnedLobbyCharacters()
    {
        spawnedLobbyCharacters = GameObject.FindObjectsOfType<LobbyCharacter>().ToList();
    }

    /// <summary>
    /// 에디터 윈도우의 GUI를 그리는 메서드
    /// </summary>
    private void OnGUI()
    {
        // 싱글톤 인스턴스 사용
        var characterManager = CharacterManager.Instance;

        // 캐릭터 데이터가 아직 로드되지 않은 경우 안내
        if (!characterManager.IsLoaded)
        {
            EditorGUILayout.LabelField("캐릭터 데이터 로딩 중...");
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("모든 캐릭터 목록", EditorStyles.boldLabel);

        // 모든 캐릭터 목록 표시 및 획득 버튼
        scrollAll = EditorGUILayout.BeginScrollView(scrollAll, GUILayout.Height(150));
        foreach (var pair in characterManager.AllCharacters)
        {
            var so = pair.Value;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"ID: {so.charID}, 이름: {so.nameKr} ({so.nameEn})", GUILayout.Width(300));
            if (GUILayout.Button("획득", GUILayout.Width(50)))
            {
                characterManager.AcquireCharacter(so.charID);

                //// LobbyCharacter 인스턴스 생성 및 초기화
                //CreateAndInitLobbyCharacter(so, 1); // 기본 레벨 1로 생성, 필요시 저장 데이터 반영
                //RefreshSpawnedLobbyCharacters();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("직접 캐릭터 ID 입력하여 획득", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        // 캐릭터 ID 직접 입력 필드
        acquireCharID = EditorGUILayout.TextField(acquireCharID, GUILayout.Width(200));
        // 입력한 ID로 캐릭터 획득
        if (GUILayout.Button("획득", GUILayout.Width(50)))
        {
            characterManager.AcquireCharacter(acquireCharID);

            // SO 찾기
            if (characterManager.AllCharacters.TryGetValue(acquireCharID, out var so))
            {
                CreateAndInitLobbyCharacter(so, 1);
                RefreshSpawnedLobbyCharacters();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("획득한 캐릭터 목록", EditorStyles.boldLabel);

        // 획득한 캐릭터 목록 표시 및 삭제 버튼
        string removeCharID = null;
        scrollAcquired = EditorGUILayout.BeginScrollView(scrollAcquired, GUILayout.Height(300));
        foreach (var so in characterManager.AcquiredCharacters)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"ID: {so.charID}, 이름: {so.nameKr} ({so.nameEn})", GUILayout.Width(300));
            if (GUILayout.Button("삭제", GUILayout.Width(50)))
            {
                removeCharID = so.charID; // 삭제할 ID 저장
            }
            EditorGUILayout.EndHorizontal();

            // 캐릭터별 경험치/레벨/능력치 표시 및 조작
            var lobbyChar = FindLobbyCharacter(so.charID);
            if (lobbyChar != null)
            {
                EditorGUILayout.LabelField($"레벨: {lobbyChar.Level}   경험치: {lobbyChar.CurrentExp}");
                EditorGUILayout.LabelField($"ATK: {lobbyChar.RegularATK}  DEF: {lobbyChar.RegularDEF}  HP: {lobbyChar.RegularHP}  크리확률: {lobbyChar.CritChance}  크리뎀: {lobbyChar.CritDamage}");

                EditorGUILayout.BeginHorizontal();
                addExpValue = EditorGUILayout.IntField("경험치 부여", addExpValue, GUILayout.Width(150));
                if (GUILayout.Button("경험치 추가", GUILayout.Width(80)))
                {
                    lobbyChar.AddExp(addExpValue);
                }
                if (GUILayout.Button("저장", GUILayout.Width(50)))
                {
                    DataSaver.Instance.SaveCharacter(lobbyChar);
                }
                if (GUILayout.Button("로드", GUILayout.Width(50)))
                {
                    LoadLobbyCharacterData(lobbyChar);
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("LobbyCharacter 인스턴스를 찾을 수 없습니다. (씬에 프리팹이 존재해야 함)", MessageType.Warning);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();

        // 루프 밖에서 삭제 실행 (Begin/End 쌍 보장)
        if (!string.IsNullOrEmpty(removeCharID))
        {
            characterManager.RemoveAcquiredCharacter(removeCharID);
            // 삭제 시 LobbyCharacter 오브젝트도 삭제
            var lobbyChar = FindLobbyCharacter(removeCharID);
            if (lobbyChar != null)
            {
                GameObject.DestroyImmediate(lobbyChar.gameObject);
                //RefreshSpawnedLobbyCharacters();
            }
        }
    }

    /// <summary>
    /// 현재 씬에 존재하는 LobbyCharacter 중 해당 charID를 가진 인스턴스 반환
    /// </summary>
    private LobbyCharacter FindLobbyCharacter(string charID)
    {
        return spawnedLobbyCharacters.FirstOrDefault(lc => lc.CharacterData != null && lc.CharacterData.charID == charID);
    }

    /// <summary>
    /// LobbyCharacter 프리팹을 생성하고 초기화
    /// </summary>
    private void CreateAndInitLobbyCharacter(CharacterSO so, int level)
    {
        // SO에 할당된 프리팹 참조 사용
        var prefab = so.charLobbyPrefab;
        if (prefab == null)
        {
            Debug.LogError($"CharacterSO({so.charID})에 charLobbyPrefab이 할당되어 있지 않습니다.");
            return;
        }
        // 이미 존재하면 중복 생성 방지
        if (FindLobbyCharacter(so.charID) != null)
            return;

        var go = GameObject.Instantiate(prefab);
        go.name = $"LobbyCharacter_{so.charID}";
        var lobbyChar = go.GetComponent<LobbyCharacter>();
        if (lobbyChar != null)
        {
            lobbyChar.Initialize(so, level);

            // 저장된 데이터가 있으면 반영
            var saveData = DataSaver.Instance.SaveData.characters
                .FirstOrDefault(c => c.CharacterID == so.charID);
            if (saveData != null)
            {
                lobbyChar.Level = saveData.Level;
                lobbyChar.CurrentExp = saveData.CurrentExp;
                lobbyChar.RegularATK = saveData.ATK;
                lobbyChar.RegularDEF = saveData.DEF;
                lobbyChar.RegularHP = saveData.HP;
                lobbyChar.CritChance = saveData.CritChance;
                lobbyChar.CritDamage = saveData.CritDamage;
            }
        }
        else
        {
            Debug.LogError("LobbyCharacter 컴포넌트를 찾을 수 없습니다.");
        }
    }

    /// <summary>
    /// 저장된 데이터에서 해당 캐릭터의 정보를 로드하여 LobbyCharacter에 반영
    /// </summary>
    private void LoadLobbyCharacterData(LobbyCharacter lobbyChar)
    {
        var saveData = DataSaver.Instance.SaveData.characters
            .FirstOrDefault(c => c.CharacterID == lobbyChar.CharacterData.charID);
        if (saveData != null)
        {
            lobbyChar.Level = saveData.Level;
            lobbyChar.CurrentExp = saveData.CurrentExp;
            lobbyChar.RegularATK = saveData.ATK;
            lobbyChar.RegularDEF = saveData.DEF;
            lobbyChar.RegularHP = saveData.HP;
            lobbyChar.CritChance = saveData.CritChance;
            lobbyChar.CritDamage = saveData.CritDamage;
        }
    }
}