using UnityEditor;
using UnityEngine;
using System.Linq;

/// <summary>
/// 에디터에서 캐릭터 획득 및 획득 정보 확인을 위한 디버그 툴
/// </summary>
public class CharacterDebugTool : EditorWindow
{
    // 씬에 존재하는 CharacterManager 인스턴스 참조
    private CharacterManager characterManager;
    // 모든 캐릭터 목록 스크롤 위치
    private Vector2 scrollAll;
    // 획득한 캐릭터 목록 스크롤 위치
    private Vector2 scrollAcquired;
    // 직접 입력으로 획득할 캐릭터 ID
    private string acquireCharID = "";

    // 에디터 메뉴에 툴 등록
    [MenuItem("Tools/Character Debug Tool")]
    public static void ShowWindow()
    {
        GetWindow<CharacterDebugTool>("Character Debug Tool");
    }

    /// <summary>
    /// 에디터 윈도우의 GUI를 그리는 메서드
    /// </summary>
    private void OnGUI()
    {
        // CharacterManager 인스턴스가 없으면 씬에서 찾기
        if (characterManager == null)
        {
            characterManager = FindFirstObjectByType<CharacterManager>();

            // 없을 경우 안내 메시지 출력
            if (characterManager == null)
            {
                EditorGUILayout.HelpBox("씬에 CharacterManager 오브젝트가 없습니다.", MessageType.Error);
                return;
            }
        }

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
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("획득한 캐릭터 목록", EditorStyles.boldLabel);

        // 획득한 캐릭터 목록 표시
        scrollAcquired = EditorGUILayout.BeginScrollView(scrollAcquired, GUILayout.Height(150));
        foreach (var so in characterManager.AcquiredCharacters)
        {
            EditorGUILayout.LabelField($"ID: {so.charID}, 이름: {so.nameKr} ({so.nameEn})");
        }
        EditorGUILayout.EndScrollView();
    }
}