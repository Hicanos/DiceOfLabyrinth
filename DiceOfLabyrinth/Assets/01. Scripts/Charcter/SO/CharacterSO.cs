using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 테스트 캐릭터를 만들기 위한 Scriptable Object
/// </summary>

[CreateAssetMenu(fileName = "NewCharacterSO", menuName = "Character/Create CharacterSO")]
public class CharacterSO : ScriptableObject
{
    public int key;
    public string charID;
    public string nameKr;
    public string nameEn;
    public DesignEnums.ClassTypes classType;
    public int baseATK;
    public int plusATK;
    public int baseDEF;
    public int plusDEF;
    public int baseHP;
    public int plusHP;
    public int critChance;
    public int critDamage;
    public DesignEnums.ElementTypes elementType;
    public int signitureNum;
    public int diceID;
    public GameObject charLobbyPrefab; // 로비에서 사용되는 캐릭터 프리팹
    public GameObject charBattlePrefab; // 배틀에서 사용되는 캐릭터 프리팹
}