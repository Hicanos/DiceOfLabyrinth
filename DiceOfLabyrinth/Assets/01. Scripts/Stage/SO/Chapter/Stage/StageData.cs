
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Linq;

[CreateAssetMenu(fileName = "StageData", menuName = "ScriptableObjects/Stages/StageData", order = 1)]
public class StageData : ScriptableObject
{
    public List<StageInfo> stageIndex;
    [SerializeField] private List<PlayerFormations> playerFormations;
    public List<StageInfo> StageIndex => stageIndex;
    public List<PlayerFormations> PlayerFormations => playerFormations;
    public int DirectCompleteExpReward =>
        stageIndex.Sum(stage => stage.ExpReward); // 모든 스테이지의 경험치 보상을 합산하여 반환
    public int DirectCompleteGoldReward =>
        stageIndex.Sum(stage => stage.GoldReward); // 모든 스테이지의 골드 보상을 합산하여 반환
    public int DirectCompletePotionReward =>
        stageIndex.Sum(stage => stage.PotionReward); // 모든 스테이지의 포션 보상을 합산하여 반환
}

[System.Serializable]
public class StageInfo
{
    // 스테이지 정보 필드들
    [SerializeField] private string stageName;
    [SerializeField] private string description;
    [SerializeField] private Sprite teamSelect;
    [SerializeField] private Sprite worldMapBackground;
    [SerializeField] private Sprite room12Background;
    [SerializeField] private Sprite room34Background;
    [SerializeField] private Sprite bossRoomBackground;
    [SerializeField] private int rewardValue;
    [SerializeField] private List<EnemyData> enemies;
    [SerializeField] private List<ChoiceOptions> choiceOptions;
    [SerializeField] private List<EngravingData> engravingList;
    [SerializeField] private List<ArtifactData> artifactList;
    [SerializeField] private List<RandomEventData> randomEvents;

    // 읽기 전용 프로퍼티들
    public string StageName => stageName;
    public string Description => description;
    public Sprite TeamSelect => teamSelect;
    public Sprite WorldMapBackground => worldMapBackground;
    public Sprite Room12Background => room12Background;
    public Sprite Room34Background => room34Background;
    public Sprite BossRoomBackground => bossRoomBackground;
    public int ExpReward => rewardValue * 10; // 15배로 보정
    public int GoldReward => rewardValue * 15; // 10배로 보정
    public int PotionReward => rewardValue; // 포션 보상은 1배로 설정
    public List<EnemyData> Enemies => enemies;
    public List<ChoiceOptions> ChoiceOptions => choiceOptions;

    public List<EngravingData> EngravingList => engravingList;
    public List<ArtifactData> ArtifactList => artifactList;
    public List<RandomEventData> RandomEvents => randomEvents;

}


[System.Serializable]
public class PlayerFormations
{
    [SerializeField] private string formationName;
    [SerializeField] private List<PlayerPositions> playerPositions = new List<PlayerPositions>(5);

    public string FormationName => formationName;
    public List<PlayerPositions> PlayerPositions => playerPositions;
}

[System.Serializable]
public class PlayerPositions
{
    [SerializeField] private Vector3 position;
    public Vector3 Position => position;
}
[System.Serializable]
public class ChoiceOptions
{
    // 선택지 정보 필드들
    [SerializeField] private string choiceText;
    [SerializeField] private string description;
    [SerializeField] private Sprite choiceIcon;

    // 읽기 전용 프로퍼티들
    public string ChoiceText => choiceText;
    public string Description => description;
    public Sprite ChoiceIcon => choiceIcon;
}