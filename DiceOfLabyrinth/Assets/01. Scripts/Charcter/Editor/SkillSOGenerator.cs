using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class SkillSOGenerator : EditorWindow
{
    private string jsonPath = "Assets/Resources/Json/SkillData.json";
    private string soOutputPath = "Assets/01. Scripts/Charcter/SO/Skills/";

    [MenuItem("Tools/SkillSO Generator")]
    public static void ShowWindow()
    {
        GetWindow<SkillSOGenerator>("SkillSO Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("SkillSO 자동 생성기", EditorStyles.boldLabel);
        jsonPath = EditorGUILayout.TextField("JSON 경로", jsonPath);
        soOutputPath = EditorGUILayout.TextField("SO 저장 경로", soOutputPath);

        if (GUILayout.Button("생성"))
        {
            GenerateSkillSOs();
        }
    }

    private void GenerateSkillSOs()
    {
        if (!File.Exists(jsonPath))
        {
            Debug.LogError("SkillData.json 파일을 찾을 수 없습니다: " + jsonPath);
            return;
        }

        string json = File.ReadAllText(jsonPath);
        SkillDataListWrapper wrapper = JsonConvert.DeserializeObject<SkillDataListWrapper>(json);

        if (wrapper == null || wrapper.Items == null)
        {
            Debug.LogError("SkillData.json 파싱 실패 또는 Items가 비어있음");
            return;
        }

        if (!Directory.Exists(soOutputPath))
            Directory.CreateDirectory(soOutputPath);

        foreach (var data in wrapper.Items)
        {
            SkillSO so;
            if (data.SkillType == 0) // Active
                so = ScriptableObject.CreateInstance<ActiveSO>();
            else // Passive
                so = ScriptableObject.CreateInstance<PassiveSO>();

            so.SkillID = data.SkillID;
            so.SkillNameKr = data.NameKr;
            so.SkillNameEn = data.NameEn;
            so.SkillType = (DesignEnums.SkillType)data.SkillType;
            so.SkillRule = data.SkillRule;
            so.SkillTarget = data.Target;
            so.SkillDescription = data.SkillDescription;

            // 아이콘 경로가 있으면 할당
            if (!string.IsNullOrEmpty(data.SkillIcon))
                so.SkillIcon = AssetDatabase.LoadAssetAtPath<Sprite>(data.SkillIcon);

            // ActiveSO/PassiveSO 개별 필드 할당
            if (so is ActiveSO active)
            {
                active.SkillCost = data.SkillCost;
                active.BuffIDs = new[] { data.BuffID, data.BuffID_2 };
                active.BuffAmounts = new[] { (DesignEnums.BuffAmount)data.BuffAmount, (DesignEnums.BuffAmount)data.BuffAmount2 };
                active.BuffProbability = data.BuffProbability;
                active.BuffTurn = data.BuffTurn;
                active.SkillValue = data.SkillValue;
                active.PlusValue = data.PlusValue;
                active.CoolTime = data.CoolTime;
            }
            else if (so is PassiveSO passive)
            {
                passive.BuffIDs = new[] { data.BuffID, data.BuffID_2 };
                passive.BuffAmounts = new[] { (DesignEnums.BuffAmount)data.BuffAmount, (DesignEnums.BuffAmount)data.BuffAmount2 };
                passive.BuffTurn = data.BuffTurn;
                passive.SkillValue = data.SkillValue;
                passive.PlusValue = data.PlusValue;
                passive.CoolTime = data.CoolTime;
            }

            string assetPath = $"{soOutputPath}{so.SkillID}_SO.asset";
            if (File.Exists(assetPath))
                AssetDatabase.DeleteAsset(assetPath);

            so.hideFlags = HideFlags.None;
            AssetDatabase.CreateAsset(so, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("SkillSO 생성 완료!");
    }

    [System.Serializable]
    private class SkillDataListWrapper
    {
        public List<SkillDataForSO> Items;
    }

    [System.Serializable]
    private class SkillDataForSO
    {
        public int Key;
        public string SkillID;
        public string NameKr;
        public string NameEn;
        public int SkillType;
        public int SkillCost;
        public DesignEnums.SkillTarget Target;
        public DesignEnums.SkillRule SkillRule;
        public string SkillDescription;
        public string BuffID;
        public string BuffID_2;
        public int BuffAmount;
        public int BuffAmount2;
        public int BuffProbability;
        public int BuffTurn;
        public float SkillValue;
        public float PlusValue;
        public int CoolTime;
        public string SkillIcon;
    }
}
