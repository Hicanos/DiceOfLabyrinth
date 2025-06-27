using UnityEngine;

[CreateAssetMenu(fileName = "StagmaData", menuName = "ScriptableObjects/Stages/StagmaData")]
public class StagmaData : ScriptableObject
{
    public string stagmaName;
    public string description;
    public Sprite icon;
    //주사위 족보에 따른 추가 효과를 위한 필드들
    public float queenAdditionalDamage; // 퀸 족보일 때 추가 데미지 배수
    public float fullHouseAdditionalDamage; // 풀하우스 족보일 때 추가 데미지 배수
    public float quadrupleAdditionalDamage; // 4개 주사위가 같은 족보일 때 추가 데미지 배수
    public float tripleAdditionalDamage; // 3개 주사위가 같은 족보일 때 추가 데미지 배수
    public float twoPairAdditionalDamage; // 2쌍이 같은 족보일 때 추가 데미지 배수
    public float onePairAdditionalDamage; // 1쌍이 같은 족보일 때 추가 데미지 배수
    public float smallStraightAdditionalDamage; // 스몰 스트레이트 족보일 때 추가 데미지 배수
    public float largeStraightAdditionalDamage; // 라지 스트레이트 족보일 때 추가 데미지 배수
    public float noPairAdditionalDamage; // 족보가 없는 경우 추가 데미지 배수
}
