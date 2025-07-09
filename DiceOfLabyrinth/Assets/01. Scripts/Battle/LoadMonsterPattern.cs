using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class LoadMonsterPattern
{
    MonsterPattern pattern = new MonsterPattern();
    //private const string filePath = "Assets/Resources/Json/tempMonsterPattern.json";

    string index;
    int patternLength;
    int patternCount;

    //public void Load()
    //{
    //    string jsonString = File.ReadAllText(filePath);
    //    JObject root = JObject.Parse(jsonString);

    //    pattern.pattern = (JObject)root["Pattern"];
    //    pattern.skill = (JObject)root["Skill"];
    //}

    public void PrepareSkill()
    {
        //if (patternLength == 0)
        //{
        //    index = SelectPattern();
        //}

        ////string skillNum = pattern.pattern["Table"][index][patternCount].ToString();
        ////string skillName = pattern.skill[skillNum]["Name"].ToString();
        ////string skillDescription = pattern.skill[skillNum]["Description"].ToString();

        //BattleManager.Instance.Enemy.Data

        //BattleManager.Instance.UIValueChanger.ChangeUIText(BattleTextUIEnum.MonsterSkillName, $"{skillName} 준비중");
        //BattleManager.Instance.UIValueChanger.ChangeUIText(BattleTextUIEnum.MonsterSkillDescription, skillDescription);        

        //patternCount++;
        //if (patternCount == patternLength)
        //{
        //    patternLength = 0;
        //    patternCount = 0;
        //}
    }    

    private string SelectPattern()
    {
        Debug.Log("새 패턴 받아옴");
        JToken patternIndex = pattern.pattern["Index"];

        string index = BattleManager.Instance.Enemy.Data.Pattern.ToString();

        patternLength = pattern.pattern["Table"][index].Count();

        return index;
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
