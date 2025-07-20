using UnityEngine;

[CreateAssetMenu(fileName = "StagmaData", menuName = "ScriptableObjects/Stages/StagmaData")]
public class StagmaData : ScriptableObject
{
    //public string stagmaName;
    //public string description;
    //public Sprite icon;
    //public Sprite bgSprite; // 배경 이미지 스프라이트
    ////주사위 족보에 따른 추가 효과를 위한 필드들
    //public float queenAdditionalDamage; // 5개 주사위가 같은 족보일 때 추가 데미지 배수
    //public float fullHouseAdditionalDamage; // 풀하우스 족보일 때 추가 데미지 배수
    //public float quadrupleAdditionalDamage; // 4개 주사위가 같은 족보일 때 추가 데미지 배수
    //public float tripleAdditionalDamage; // 3개 주사위가 같은 족보일 때 추가 데미지 배수
    //public float twoPairAdditionalDamage; // 2쌍이 같은 족보일 때 추가 데미지 배수
    //public float onePairAdditionalDamage; // 1쌍이 같은 족보일 때 추가 데미지 배수
    //public float smallStraightAdditionalDamage; // 스몰 스트레이트 족보일 때 추가 데미지 배수
    //public float largeStraightAdditionalDamage; // 라지 스트레이트 족보일 때 추가 데미지 배수
    //public float noPairAdditionalDamage; // 족보가 없는 경우 추가 데미지 배수
    // 직렬화 필드 프라이빗 으로 선언한 뒤 읽기 전용 프로퍼티로 노출
    [SerializeField] private string stagmaName;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private Sprite bgSprite; // 배경 이미지 스프라이트
    [SerializeField] private float queenAdditionalDamage; // 5개 주사위가 같은 족보일 때 추가 데미지 배수
    [SerializeField] private float fullHouseAdditionalDamage; // 풀하우스 족보일 때 추가 데미지 배수
    [SerializeField] private float quadrupleAdditionalDamage; // 4개 주사위가 같은 족보일 때 추가 데미지 배수
    [SerializeField] private float tripleAdditionalDamage; // 3개 주사위가 같은 족보일 때 추가 데미지 배수
    [SerializeField] private float twoPairAdditionalDamage; // 2쌍이 같은 족보일 때 추가 데미지 배수
    [SerializeField] private float onePairAdditionalDamage; // 1쌍이 같은 족보일 때 추가 데미지 배수
    [SerializeField] private float smallStraightAdditionalDamage; // 스몰 스트레이트 족보일 때 추가 데미지 배수
    [SerializeField] private float largeStraightAdditionalDamage; // 라지 스트레이트 족보일 때 추가 데미지 배수
    [SerializeField] private float noPairAdditionalDamage; // 족보가 없는 경우 추가 데미지 배수
    public string StagmaName => stagmaName;
    public string Description => description;
    public Sprite Icon => icon;
    public Sprite BgSprite => bgSprite; // 배경 이미지 스프라이트
    public float QueenAdditionalDamage => queenAdditionalDamage; // 5개 주사위가 같은 족보일 때 추가 데미지 배수
    public float FullHouseAdditionalDamage => fullHouseAdditionalDamage; // 풀하우스 족보일 때 추가 데미지 배수
    public float QuadrupleAdditionalDamage => quadrupleAdditionalDamage; // 4개 주사위가 같은 족보일 때 추가 데미지 배수
    public float TripleAdditionalDamage => tripleAdditionalDamage; // 3개 주사위가 같은 족보일 때 추가 데미지 배수
    public float TwoPairAdditionalDamage => twoPairAdditionalDamage; // 2쌍이 같은 족보일 때 추가 데미지 배수
    public float OnePairAdditionalDamage => onePairAdditionalDamage; // 1쌍이 같은 족보일 때 추가 데미지 배수
    public float SmallStraightAdditionalDamage => smallStraightAdditionalDamage; // 스몰 스트레이트 족보일 때 추가 데미지 배수
    public float LargeStraightAdditionalDamage => largeStraightAdditionalDamage; // 라지 스트레이트 족보일 때 추가 데미지 배수
    public float NoPairAdditionalDamage => noPairAdditionalDamage; // 족보가 없는 경우 추가 데미지 배수

}