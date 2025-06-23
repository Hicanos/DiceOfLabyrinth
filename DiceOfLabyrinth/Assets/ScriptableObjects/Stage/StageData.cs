using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "StageData", menuName = "ScriptableObjects/StageData", order = 1)]
public class StageData : ScriptableObject
{
    [SerializeField] private List<StageInfo> stageIndex;

    public List<StageInfo> StageIndex => stageIndex;
}

[System.Serializable]
public class StageInfo
{
    // 스테이지 정보 필드들
    [SerializeField] private string stageName;
    [SerializeField] private string description;
    [SerializeField] private Sprite backgroundImage;
    [SerializeField] private int stageIndex;
    [SerializeField] private int stageCost;
    [SerializeField] private int expReward;
    [SerializeField] private int goldReward;
    [SerializeField] private int jewelReward;
    [SerializeField] private bool isCompleted;
    [SerializeField] private bool isLocked = true;
    [SerializeField] private PhaseData[] phases = new PhaseData[5];
    [SerializeField] private List<ChooseOptions> choose;

    // 읽기 전용 프로퍼티들
    public string StageName => stageName;
    public string Description => description;
    public Sprite BackgroundImage => backgroundImage;
    public int StageIndex => stageIndex;
    public int StageCost => stageCost;
    public int ExpReward => expReward;
    public int GoldReward => goldReward;
    public int JewelReward => jewelReward;
    public bool IsCompleted => isCompleted;
    public bool IsLocked => isLocked;
    public PhaseData[] Phases => phases;
    public List<ChooseOptions> Choose => choose;
}

[System.Serializable]
public class PhaseData
{
    // Phase 정보 필드들
    [SerializeField] private string phaseName;
    [SerializeField] private List<EnemySpawnData> enemies;
    [SerializeField] private List<PhaseRewardData> phaseRewardObjects;
    [SerializeField] private int gemReward;

    // 읽기 전용 프로퍼티들
    public string PhaseName => phaseName;
    public List<EnemySpawnData> Enemies => enemies;
    public List<PhaseRewardData> PhaseRewardObjects => phaseRewardObjects;

    public int GemReward => gemReward;
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
    [SerializeField] private int rewardAmount;

    // 읽기 전용 프로퍼티들
    public string RewardName => rewardName;
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