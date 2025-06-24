using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 테스트 캐릭터를 만들기 위한 Scriptable Object
/// </summary>

enum Grade
{
    N,
    R,
    SR,
    SSR,
    UR    
}

enum CharacterType
{
    Tanker,
    Dealer,
    Supporter
}

enum ElementType
{
    Fire,
    Water,
    Wind,
    Light,
    Dark
}

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Character/Create New Character")]
public class CharacterSO : ScriptableObject
{
    // 캐릭터 ID(string), 캐릭터 타입, 캐릭터 이름, 캐릭터 최대 레벨, 체력, 공격력, 방어력,
    // 스폰 시 Object, 정보 표시용 Object(UI), 설명, 스킬(Dictionary. SkillData에서 들고옴),
    // 시그니쳐 넘버
    // 크리티컬 확률, 크리티컬 대미지(150%)

    [SerializeField] private string characterID;
    [SerializeField] private Grade grade;
    [SerializeField] private CharacterType characterType;
    [SerializeField] private ElementType elementType;
    [SerializeField] private string characterName;
    [SerializeField] private int maxLevel;
    [SerializeField] private int hp;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int signatureNumber; // 시그니쳐 넘버
    [SerializeField] private float criticalChance; // 크리티컬 확률
    [SerializeField] private float criticalDamage; // 크리티컬 대미지 (150% = 1.5f)
    [SerializeField] private GameObject spawnObject;
    [SerializeField] private GameObject infoObject;
    [SerializeField] private string description;

    // Skill 목록을 Dictionary로 저장해서 Key값(스킬 ID)으로 접근
    [SerializeField] private Dictionary<string, SkillSO> skills;

}
