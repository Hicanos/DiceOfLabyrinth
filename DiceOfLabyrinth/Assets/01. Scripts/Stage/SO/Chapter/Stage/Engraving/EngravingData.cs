using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EngravingData", menuName = "ScriptableObjects/Stages/EngravingData")]
public class EngravingData : ScriptableObject
{
    [SerializeField] private string engravingName;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private Sprite bgSprite; // 배경 이미지 스프라이트
    [SerializeField] private List<DamageCondition> damageConditions;
    [SerializeField] private DamageConditionRank[] damageConditionRanks;
    [SerializeField] private BonusAbilityRoll[] bonusAbilityRolls;

    public string EngravingName => engravingName;
    public string Description => description;
    public Sprite Icon => icon;
    public Sprite BgSprite => bgSprite; // 배경 이미지 스프라이트
    public List<DamageCondition> DamageConditions => damageConditions;
    public DamageConditionRank[] DamageConditionRanks => damageConditionRanks;
    public BonusAbilityRoll[] BonusAbilityRolls => bonusAbilityRolls;
}

[System.Serializable]
public struct DamageCondition
{
    public enum ConditionType
    {
        //Queen,// 5개 주사위가 같은 족보
        //FullHouse,// 풀하우스 족보
        //Quadruple,// 4개 주사위가 같은 족보
        //Triple,// 3개 주사위가 같은 족보
        //TwoPair,// 2쌍이 같은 족보
        //OnePair,// 1쌍이 같은 족보
        //SmallStraight,// 스몰 스트레이트 족보
        //LargeStraight,// 라지 스트레이트 족보
        //BasicAttack,// 기본 공격 추가 데미지
        SameAsPreviousTurn, // 이전 턴과 동일한 족보일 때 추가 데미지
        FirstAttack, // 첫 공격 시 추가 데미지
        //FirstTurnKillReward, // 첫 턴에 처치 시 추가 manastone 획득량
        //BonusReroll, // 보너스 리롤(데미지가 아닌 리롤 횟수 증가)
        //NoPair// 족보가 없는 경우
    }
    [SerializeField] private ConditionType type;
    [SerializeField]private float additionalValue;// 추가 데미지 배수나 리롤 횟수 증가 등
    public ConditionType Type => type;
    public float AdditionalValue => additionalValue;
}

[System.Serializable]
public struct DamageConditionRank
{
    [SerializeField] private DiceRankingEnum conditionRank;
    [SerializeField] private float additionalValue;

    public DiceRankingEnum ConditionRank => conditionRank;
    public float AdditionalValue => additionalValue;
}

[System.Serializable]
public struct BonusAbilityRoll
{
    public enum AbilityEnum
    {
        BonusReroll,
        FirstTurnKillReward
    }

    [SerializeField] private AbilityEnum ability;
    [SerializeField] private float value;

    public AbilityEnum Ability => ability;
    public float Value => value;
}