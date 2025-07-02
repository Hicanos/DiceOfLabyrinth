using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

public class LoadMonsterPattern
{
    MonsterPattern pattern = new MonsterPattern();
    private const string filePath = "Assets/Resources/Json/tempMonsterPattern.json";

    string index;
    int patternLength;
    int patternCount;

    public void Load()
    {
        string jsonString = File.ReadAllText(filePath);
        JObject root = JObject.Parse(jsonString);

        pattern.pattern = (JObject)root["Pattern"];
        pattern.skill = (JObject)root["Skill"];
    }

    public void PrepareSkill()
    {
        if (patternLength == 0)
        {
            SelectPattern();
        }

        string skillNum = pattern.pattern["Table"][index][patternCount].ToString();
        string skillName = pattern.skill[skillNum]["Name"].ToString();
        string skillDescription = pattern.skill[skillNum]["Description"].ToString();
        BattleManager.Instance.monsterSkillName.text = $"{skillName} 준비중";
        BattleManager.Instance.monsterSkillDescription.text = skillDescription;
        patternCount++;
        if (patternCount == patternLength)
        {
            patternLength = 0;
            patternCount = 0;
        }
    }    

    private void SelectPattern()
    {
        Debug.Log("새 패턴 받아옴");
        JToken patternIndex = pattern.pattern["Index"];

        int iNum = UnityEngine.Random.Range(0, patternIndex.Count());
        index = patternIndex[iNum].ToString();

        patternLength = pattern.pattern["Table"][index].Count();
    }
}

public class MonsterPattern
{
    public JObject pattern;
    public JObject skill;
}

public class MonsterSkillData
{
    string name;
    string description;


}
