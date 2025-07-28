
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "StageData", menuName = "ScriptableObjects/Stages/StageData", order = 1)]
public class StageData : ScriptableObject
{
    public List<StageInfo> stageIndex;
    [SerializeField] private List<PlayerFormations> playerFormations;
    public List<StageInfo> StageIndex => stageIndex;
    public List<PlayerFormations> PlayerFormations => playerFormations;
}

[System.Serializable]
public class StageInfo
{
    // 스테이지 정보 필드들
    [SerializeField] private string stageName;
    [SerializeField] private string description;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private int rewardValue;
    [SerializeField] private List<EnemyData> enemies;
    [SerializeField] private List<ChoiceOptions> choiceOptions;
    [SerializeField] private List<EngravingData> engravingList;
    [SerializeField] private List<ArtifactData> artifactList;

    // 읽기 전용 프로퍼티들
    public string StageName => stageName;
    public string Description => description;
    public Image BackgroundImage => backgroundImage;
    public int ExpReward => rewardValue * 10; // 15배로 보정
    public int GoldReward => rewardValue * 15; // 10배로 보정
    public int PotionReward => rewardValue; // 포션 보상은 1배로 설정
    public List<EnemyData> Enemies => enemies;
    public List<ChoiceOptions> ChoiceOptions => choiceOptions;

    public List<EngravingData> EngravingList => engravingList;
    public List<ArtifactData> ArtifactList => artifactList;

}

[System.Serializable]
public class NormalPhaseData
{
    // Phase 정보 필드들
    [SerializeField] private string phaseName; 
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private Vector3 spawnPosition;

    // 읽기 전용 프로퍼티들
    public string PhaseName => phaseName;
    public EnemyData EnemyData => enemyData;
    public Vector3 SpawnPosition => spawnPosition;

}

[System.Serializable]
public class ElitePhaseData
{
    // Phase 정보 필드들
    [SerializeField] private string phaseName;
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private Vector3 spawnPosition;

    // 읽기 전용 프로퍼티들
    public string PhaseName => phaseName;
    public EnemyData EnemyData => enemyData;
    public Vector3 SpawnPosition => spawnPosition;
}
[System.Serializable]
public class BossPhaseData
{
    // 보스 페이즈 정보 필드들
    [SerializeField] private string bossName;
    [SerializeField] private string description;
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private Vector3 spawnPosition;
    // 읽기 전용 프로퍼티들
    public string BossName => bossName;
    public string Description => description;
    public EnemyData EnemyData => enemyData;
    public Vector3 SpawnPosition => spawnPosition;
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
public class EnemySpawnData
{
    // 적 스폰 정보 필드들
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private Vector3 spawnPosition;

    // 읽기 전용 프로퍼티들
    public EnemyData EnemyData => enemyData;
    public Vector3 SpawnPosition => spawnPosition;
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