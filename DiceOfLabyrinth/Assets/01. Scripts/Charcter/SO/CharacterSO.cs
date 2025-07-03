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
    public float critChance;
    public float critDamage;
    public int penetration;
    public float elementDMG;
    public DesignEnums.ElementTypes elementType;
    public string description;
    public string dialog1;
    public string dialog2;
    public string diceID;
    public GameObject charLobbyPrefab; // 로비에서 사용되는 캐릭터 프리팹
    public GameObject charBattlePrefab; // 배틀에서 사용되는 캐릭터 프리팹
    public CharDiceData charDiceData; // 캐릭터 전용 주사위 데이터
    public Sprite icon; // 캐릭터 아이콘
    public Sprite Upper; // 캐릭터 상체 이미지
    public Sprite Standing; // 캐릭터 스탠딩 이미지
}