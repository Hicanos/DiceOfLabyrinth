using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class CharData
{
    /// <summary>
    /// Key
    /// </summary>
    public int key;

    /// <summary>
    /// Key
    /// </summary>
    public string CharID;

    /// <summary>
    /// Name
    /// </summary>
    public string NameKr;

    /// <summary>
    /// Name_En
    /// </summary>
    public string NameEn;

    /// <summary>
    /// ClassType
    /// </summary>
    public DesignEnums.ClassTypes ClassType;

    /// <summary>
    /// Char_Stat_ATK
    /// </summary>
    public int BaseATK;

    /// <summary>
    /// ATK_Bonus
    /// </summary>
    public int PlusATK;

    /// <summary>
    /// Char_Stat_DEF
    /// </summary>
    public int BaseDEF;

    /// <summary>
    /// DEF_Bonus
    /// </summary>
    public int PlusDEF;

    /// <summary>
    /// Char_Stat_HP
    /// </summary>
    public int BaseHP;

    /// <summary>
    /// HP_Bonus
    /// </summary>
    public int PlusHP;

    /// <summary>
    /// Char_Stat_CriC
    /// </summary>
    public int CritChance;

    /// <summary>
    /// Char_Stat_CriD
    /// </summary>
    public int CritDamage;

    /// <summary>
    /// Char_E_Type
    /// </summary>
    public DesignEnums.ElementTypes ElementType;

    /// <summary>
    /// C_No
    /// </summary>
    public int SignitureNum;

    /// <summary>
    /// Char_Dice
    /// </summary>
    public int DiceID;

}
public class CharDataLoader
{
    public List<CharData> ItemsList { get; private set; }
    public Dictionary<int, CharData> ItemsDict { get; private set; }

    public CharDataLoader(string path = "JSON/CharData")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, CharData>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<CharData> Items;
    }

    public CharData GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public CharData GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
