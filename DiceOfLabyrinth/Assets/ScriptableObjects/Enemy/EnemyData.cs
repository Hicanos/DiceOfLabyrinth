using UnityEngine;

[System.Serializable]
public class EnemyData
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
        Gnole,
        Orc,
        Ogre,
    }

    public enum EnemyAttribute
    {
        Fire,
        Water,
        Earth,
        Wind,
        Light,
        Dark,
    }
    public enum EnemyPattern
    {
        PT_101,
        PT_102,
        PT_103,
        PT_104,
        PT_105,
        PT_106,
        PT_107,
        PT_108,
        PT_109,
        PT_110,
    }

    [SerializeField] private string enemyName;
    [SerializeField] private int enemyLevel;
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private EnemySpecies enemySpecies;
    [SerializeField] private EnemyAttribute enemyAttribute;
    [SerializeField] private EnemyPattern enemyPattern;
    [SerializeField] private int baseMaxHp;
    [SerializeField] private int baseAtk;
    [SerializeField] private int baseDef;
    [SerializeField] private int hpPerLevel;
    [SerializeField] private int atkPerLevel;
    [SerializeField] private int defPerLevel;
    [SerializeField] private string description;

    public string EnemyName => enemyName;
    public int EnemyLevel => enemyLevel;
    public EnemyType Type => enemyType;
    public EnemySpecies Species => enemySpecies;
    public EnemyAttribute Attribute => enemyAttribute;
    public EnemyPattern Pattern => enemyPattern;
    public string Description => description;

    // 기초값
    public int BaseMaxHp => baseMaxHp;
    public int BaseAtk => baseAtk;
    public int BaseDef => baseDef;
    public int HpPerLevel => hpPerLevel;
    public int AtkPerLevel => atkPerLevel;
    public int DefPerLevel => defPerLevel;

    // 실제 능력치 (레벨 반영)
    public int MaxHp => baseMaxHp + hpPerLevel * (enemyLevel - 1);
    public int Atk => baseAtk + atkPerLevel * (enemyLevel - 1);
    public int Def => baseDef + defPerLevel * (enemyLevel - 1);
}