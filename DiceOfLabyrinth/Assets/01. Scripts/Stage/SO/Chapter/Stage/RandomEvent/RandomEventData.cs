using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomEventData", menuName = "ScriptableObjects/Stages/RandomEventData", order = 1)]
public class RandomEventData : ScriptableObject
{
    [SerializeField] private string eventName;
    [SerializeField] private RandomEventType eventType; // 이벤트 타입
    [SerializeField] private EventEffect eventEffect; // 이벤트 효과
    [SerializeField, TextArea] private string description;
    [SerializeField] private List<float> value;
    public string EventName => eventName;
    public RandomEventType EventType => eventType;
    public EventEffect EventEffect => eventEffect;
    public string Description => description;
    public List<float> Value => value;
}

public enum EventEffect
{
    AdditionalAttack,// 추가 공격력
    AdditionalDefense,// 추가 방어력
    CostRegeneration,// 비용 회복
    DebuffImmunity,// 디버프 면역
    RandomDebuff,// 랜덤한 디버프에 걸림
    EnemyLevelChange,// 적 레벨 변경
    RandomArtifact,// 지니고 있던 아티팩트를 랜덤하게 변경
    RandomStat,// 능력치 랜덤 변경(변수 두개)
}
public enum RandomEventType
{
    Blessing, // 축복
    Curse, // 저주
    RiskAndReturn, // 위험과 보상
}