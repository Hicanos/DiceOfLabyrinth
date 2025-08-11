using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions; // 정규식 사용을 위한 네임스페이스

[CreateAssetMenu(fileName = "randomEventData", menuName = "ScriptableObjects/Stages/randomEventData", order = 1)]
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
    public static string GetEventDescription(RandomEventData randomEvent)
    {
        if (randomEvent == null) return "";

        string result = randomEvent.Description;

        // 1) 일반 값 {value_i}
        string valuePattern = @"\{value_(\d+)\}";
        result = Regex.Replace(result, valuePattern, match =>
        {
            int index = int.Parse(match.Groups[1].Value);
            if (index < 0 || index >= randomEvent.Value.Count)
                return match.Value;

            return randomEvent.Value[index].ToString();
        });

        // 2) 퍼센트 값 {percentValue_i}
        string percentPattern = @"\{percentValue_(\d+)\}";
        result = Regex.Replace(result, percentPattern, match =>
        {
            int index = int.Parse(match.Groups[1].Value);
            if (index < 0 || index >= randomEvent.Value.Count)
                return match.Value;

            return (randomEvent.Value[index] * 100f).ToString("F1") + "%";
        });
        return result;
    }
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