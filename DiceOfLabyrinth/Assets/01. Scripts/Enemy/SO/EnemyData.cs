using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/Enemy/EnemyData", order = 1)]
[System.Serializable]
public class EnemyData: ScriptableObject
{
    public enum EnemyType
    {
        Normal,
        Elite,
        Guardian,
        Lord,
    }

    public enum EnemySpecies
    {
        Goblin,
        Gnoll,
        Orc,
        Ogre,
        Treant,
        Spirit,
        EtherCreature,
    }

    public enum EnemyAttribute
    {
        Fire,
        Water,
        Earth,
        Grass,
        Electric,
    }
    [SerializeField] private string enemyName;
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private EnemySpecies enemySpecies;
    [SerializeField] private EnemyAttribute enemyAttribute;
    [SerializeField] private int baseMaxHp;
    [SerializeField] private int baseAtk;
    [SerializeField] private int baseDef;
    [SerializeField] private int hpPerLevel;
    [SerializeField] private int atkPerLevel;
    [SerializeField] private int defPerLevel;
    [SerializeField] private string description;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private List<int> activeSkills; // 액티브 스킬 인덱스 리스트
    [SerializeField] private List<int> passiveSkills; // 패시브 스킬 인덱스 리스트
    [SerializeField] private Quaternion enemySpawnRotation; // 적 스폰 회전값

    public string EnemyName => enemyName;
    public int EnemyLevel => StageManager.Instance.stageSaveData.currentStageIndex + 1;
    public EnemyType Type => enemyType;
    public EnemySpecies Species => enemySpecies;
    public EnemyAttribute Attribute => enemyAttribute;
    public string Description => description;
    public GameObject EnemyPrefab => enemyPrefab;
    public List<int> ActiveSkills => activeSkills;
    public List<int> PassiveSkills => passiveSkills;
    public Quaternion EnemySpawnRotation => enemySpawnRotation;

    // 기초값
    public int BaseMaxHp => baseMaxHp;
    public int BaseAtk => baseAtk;
    public int BaseDef => baseDef;
    public int HpPerLevel => hpPerLevel;
    public int AtkPerLevel => atkPerLevel;
    public int DefPerLevel => defPerLevel;

    // 실제 능력치 (레벨 반영)
    public int MaxHp => baseMaxHp + hpPerLevel * (EnemyLevel - 1);
    public int Atk => baseAtk + atkPerLevel * (EnemyLevel - 1);
    public int Def => baseDef + defPerLevel * (EnemyLevel - 1);
}