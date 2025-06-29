
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
    [SerializeField] private Sprite backgroundImage;
    [SerializeField] private int expReward;
    [SerializeField] private int goldReward;
    [SerializeField] private int jewelReward;
    [SerializeField] private BossPhaseData bossPhase;
    [SerializeField] private List<NormalPhaseData> normalPhases;
    [SerializeField] private List<ElitePhaseData> elitePhases;
    [SerializeField] private List<ChooseOptions> choose;
    [SerializeField] private List<StagmaData> stagmaList;
    [SerializeField] private List<ArtifactData> artifactList;

    // 읽기 전용 프로퍼티들
    public string StageName => stageName;
    public string Description => description;
    public Sprite BackgroundImage => backgroundImage;
    public int ExpReward => expReward;
    public int GoldReward => goldReward;
    public int JewelReward => jewelReward;
    public List<NormalPhaseData> NormalPhases => normalPhases;
    public List<ElitePhaseData> ElitePhases => elitePhases;
    public List<ChooseOptions> Choose => choose;

    public List<StagmaData> StagmaList => stagmaList;
    public List<ArtifactData> ArtifactList => artifactList;

}

[System.Serializable]
public class NormalPhaseData
{
    // Phase 정보 필드들
    [SerializeField] private string phaseName;
    [SerializeField] private List<EnemySpawnData> enemies;
    [SerializeField] private List<PhaseRewardData> phaseRewardObjects;
    [SerializeField] private int manaStoneReward;

    // 읽기 전용 프로퍼티들
    public string PhaseName => phaseName;
    public List<EnemySpawnData> Enemies => enemies;
    public List<PhaseRewardData> PhaseRewardObjects => phaseRewardObjects;

    public int ManaStoneReward => manaStoneReward;
}

[System.Serializable]
public class ElitePhaseData
{
    // 보스 페이즈 정보 필드들
    [SerializeField] private string phaseName;
    [SerializeField] private List<EnemySpawnData> enemies;
    [SerializeField] private List<PhaseRewardData> phaseRewardObjects;
    [SerializeField] private int manaStoneReward;

    // 읽기 전용 프로퍼티들
    public string PhaseName => phaseName;
    public List<EnemySpawnData> Enemies => enemies;
    public List<PhaseRewardData> PhaseRewardObjects => phaseRewardObjects;
    public int ManaStoneReward => manaStoneReward;
}
[System.Serializable]
public class BossPhaseData
{
    // 보스 페이즈 정보 필드들
    [SerializeField] private string bossName;
    [SerializeField] private string description;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Vector3 spawnPosition;
    // 읽기 전용 프로퍼티들
    public string BossName => bossName;
    public string Description => description;
    public GameObject BossPrefab => bossPrefab;
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
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Vector3 spawnPosition;

    // 읽기 전용 프로퍼티들
    public GameObject EnemyPrefab => enemyPrefab;
    public Vector3 SpawnPosition => spawnPosition;
}

[System.Serializable]
public class PhaseRewardData
{
    // Phase 보상 정보 필드들
    [SerializeField] private string rewardName;
    [SerializeField] private string description;
    [SerializeField] private int rewardAmount;

    // 읽기 전용 프로퍼티들
    public string RewardName => rewardName;
    public string Description => description;
    public int RewardAmount => rewardAmount;
}

[System.Serializable]
public class ChooseOptions
{
    // 선택지 정보 필드들
    [SerializeField] private string chooseName;
    [SerializeField] private string description;
    [SerializeField] private Sprite iconImage;

    // 읽기 전용 프로퍼티들
    public string ChooseName => chooseName;
    public string Description => description;
    public Sprite IconImage => iconImage;
}