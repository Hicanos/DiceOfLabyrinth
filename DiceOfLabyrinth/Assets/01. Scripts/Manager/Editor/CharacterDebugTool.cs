using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// 에디터에서 캐릭터 획득 및 획득 정보 확인을 위한 디버그 툴
/// </summary>
public class CharacterDebugTool : EditorWindow
{
    private Vector2 scrollAll;
    private Vector2 scrollAcquired;
    private string acquireCharID = "";
    private int addExpValue = 100;

    // 에디터 메뉴에 툴 등록
    [MenuItem("Tools/Character Debug Tool")]
    public static void ShowWindow()
    {
        GetWindow<CharacterDebugTool>("Character Debug Tool");
    }

    private void OnGUI()
    {
        var characterManager = CharacterManager.Instance;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("모든 캐릭터 목록", EditorStyles.boldLabel);

        // 모든 캐릭터 목록 표시 및 획득 버튼
        scrollAll = EditorGUILayout.BeginScrollView(scrollAll, GUILayout.Height(150));
        if (!characterManager.IsLoaded)
        {
            EditorGUILayout.LabelField("캐릭터 데이터 로딩 중...");
            EditorGUILayout.EndScrollView();
            return;
        }
        foreach (var pair in characterManager.AllCharacters)
        {
            var so = pair.Value;
            EditorGUILayout.LabelField($"ID: {so.charID}, 이름: {so.nameKr} ({so.nameEn})");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("획득", GUILayout.Width(50)))
            {
                characterManager.AcquireCharacter(so.charID);
            }
            // BattleCharacter 프리팹 즉시 인스턴스 버튼
            if (GUILayout.Button("전투 프리팹 생성", GUILayout.Width(100)))
            {
                InstantiateBattleCharacterWithLobbyData(so);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("직접 캐릭터 ID 입력하여 획득", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        acquireCharID = EditorGUILayout.TextField(acquireCharID, GUILayout.Width(200));
        if (GUILayout.Button("획득", GUILayout.Width(50)))
        {
            characterManager.AcquireCharacter(acquireCharID);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("획득한 캐릭터 목록", EditorStyles.boldLabel);

        // DiceDataLoader는 한 번만 생성해서 재사용
        DiceDataLoader diceLoader = new DiceDataLoader();

        string removeCharID = null;
        scrollAcquired = EditorGUILayout.BeginScrollView(scrollAcquired, GUILayout.Height(400));
        foreach (var lobbyChar in characterManager.OwnedCharacters)
        {
            // CharacterSO의 diceID로 CharDiceData를 찾아서 시그니처 넘버를 가져옴
            int cignatureNo = -1;
            string diceID = lobbyChar.CharacterData != null ? lobbyChar.CharacterData.diceID : null;
            if (!string.IsNullOrEmpty(diceID))
            {
                var diceData = diceLoader.ItemsList.FirstOrDefault(d => d.DiceID == diceID);
                if (diceData != null)
                    cignatureNo = diceData.CignatureNo;
            }

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
                $"ID: {lobbyChar.CharacterData.charID}, 이름: {lobbyChar.CharacterData.nameKr} ({lobbyChar.CharacterData.nameEn}), 시그니처 넘버: {cignatureNo}",
                GUILayout.Width(400));
            if (GUILayout.Button("삭제", GUILayout.Width(50)))
            {
                removeCharID = lobbyChar.CharacterData.charID;
            }
            EditorGUILayout.EndHorizontal();

            // Regular 데이터(로비 캐릭터의 현재 능력치)
            EditorGUILayout.LabelField($"[Regular] 레벨: {lobbyChar.Level}   경험치: {lobbyChar.CurrentExp}");
            EditorGUILayout.LabelField($"[Regular] ATK: {lobbyChar.RegularATK}  DEF: {lobbyChar.RegularDEF}  HP: {lobbyChar.RegularHP}  크리확률: {lobbyChar.CritChance}  크리뎀: {lobbyChar.CritDamage}");

            // initial 데이터(전투 시작 시 BattleCharacter의 초기값, 여기서는 Regular 값과 동일하게 가정)
            EditorGUILayout.LabelField($"[initial] (전투 시작 시 복사되는 초기값)");
            EditorGUILayout.LabelField($"[initial] ATK: {lobbyChar.RegularATK}  DEF: {lobbyChar.RegularDEF}  HP: {lobbyChar.RegularHP}  크리확률: {lobbyChar.CritChance}  크리뎀: {lobbyChar.CritDamage}");

            EditorGUILayout.BeginHorizontal();
            addExpValue = EditorGUILayout.IntField("경험치 부여", addExpValue, GUILayout.Width(150));
            if (GUILayout.Button("경험치 추가", GUILayout.Width(80)))
            {
                lobbyChar.AddExp(addExpValue);
                characterManager.SaveAll();
            }
            // 개별 저장/로드 버튼 추가
            if (GUILayout.Button("저장", GUILayout.Width(50)))
            {
                DataSaver.Instance.SaveCharacter(lobbyChar);
            }
            if (GUILayout.Button("로드", GUILayout.Width(50)))
            {
                LoadLobbyCharacterData(lobbyChar);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();

        if (!string.IsNullOrEmpty(removeCharID))
        {
            characterManager.RemoveCharacter(removeCharID);
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

    /// <summary>
    /// CharacterSO에 등록된 BattleCharacter 프리팹을 인스턴스화하고,
    /// 동일 charID의 BattleCharacter가 이미 있으면 이름 기반으로 찾아 삭제 후 새로 생성
    /// LobbyCharacter 데이터로 BattleCharacter를 자동 세팅
    /// </summary>
    private void InstantiateBattleCharacterWithLobbyData(CharacterSO so)
    {
        if (so.charBattlePrefab == null)
        {
            Debug.LogError($"CharacterSO({so.charID})에 charBattlePrefab이 할당되어 있지 않습니다.");
            return;
        }

        // 이름 기반으로 기존 BattleCharacter 오브젝트 탐색 및 삭제
        string battleCharName = $"BattleCharacter_{so.charID}";
        var existing = GameObject.Find(battleCharName);
        if (existing != null)
        {
#if UNITY_EDITOR
            Debug.Log($"기존 BattleCharacter({so.charID}) 오브젝트를 삭제합니다: {existing.name}");
#endif
            GameObject.DestroyImmediate(existing);
        }

        // LobbyCharacter 데이터 찾기
        var lobbyChar = CharacterManager.Instance.OwnedCharacters
            .FirstOrDefault(lc => lc.CharacterData != null && lc.CharacterData.charID == so.charID);

        if (lobbyChar == null)
        {
            Debug.LogError($"해당 캐릭터({so.charID})의 LobbyCharacter 데이터가 없습니다. 먼저 획득해야 합니다.");
            return;
        }

        // BattleCharacter 프리팹 인스턴스화 및 LobbyCharacter 데이터로 자동 세팅
        var go = GameObject.Instantiate(so.charBattlePrefab);
        go.name = battleCharName;
        var battleChar = go.GetComponent<BattleCharacter>();
        if (battleChar != null)
        {
            battleChar.SetCharacterSO(so); // 자동으로 LobbyCharacter를 찾아 세팅
            Debug.Log($"BattleCharacter({so.charID})가 CharacterSO로 초기화되어 생성되었습니다.");
        }
        else
        {
            Debug.LogError("BattleCharacter 컴포넌트를 찾을 수 없습니다.");
        }
    }
}