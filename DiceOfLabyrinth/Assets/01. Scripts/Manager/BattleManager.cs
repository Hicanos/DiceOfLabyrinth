using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    #region 싱글톤 구현
    private static BattleManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static BattleManager Instance
    {
        get
        {
            if(instance == null)
            {
                return null;
            }
            return instance;
        }
    }
    #endregion
    //public CharacterSO[] entryCharacters;
    public GameObject[] entryCharacters; //임시
    GameObject enemyGO;
    public TestEnemy TestEnemy => enemyGO.GetComponent<TestEnemy>();

    [SerializeField] Transform enemyContainer;

    public LoadMonsterPattern LoadMonsterPattern;
    public MonsterPattern MonsterPattern;
    public BattleStateMachine stateMachine;
    public PlayerTurnState currentPlayerState;
    public IBattleTurnState playerTurnState;
    public IBattleTurnState enemyTurnState;
    public BattlePlayerTurnState battlePlayerTurnState;

    public TextMeshProUGUI costTest;
    public TextMeshProUGUI monsterSkillName;
    public TextMeshProUGUI monsterSkillDescription;
    public Button DiceRollButton;
    public AbstractBattleButton[] BattleButtons;

    public readonly int MaxCost = 12;
    public int CurrnetCost = 0;
    public int BattleTurn = 0;
    public bool isBattle;

    public BattleCoroutine battleCoroutine;
    void Start()
    {
        playerTurnState = new BattlePlayerTurnState();
        battlePlayerTurnState = (BattlePlayerTurnState)playerTurnState;

        enemyTurnState = new BattleEnemyTurnState();

        stateMachine = new BattleStateMachine(playerTurnState);

        LoadMonsterPattern = new LoadMonsterPattern();
        MonsterPattern = new MonsterPattern();

        BattleStartCoroutine(); //테스트용
    }

    
    void Update()
    {
        stateMachine.BattleUpdate();
    }

    public void BattleStartCoroutine() //전투 시작시 호출해야할 메서드
    {
        GetMonster();
        DiceManager.Instance.DiceSettingForBattle();
        battleCoroutine.StartPrepareBattle();
    }

    public void BattleStart()
    {
        //entryCharacters = StageManager.Instance.stageSaveData.entryCharacters;
        playerTurnState.Enter();
        DiceManager.Instance.LoadDiceData();
        
        isBattle = true;
    }

    public void BattleEnd()
    {
        isBattle = false;

        battlePlayerTurnState.ChangePlayerTurnState(PlayerTurnState.BattleEnd);
    }

    private void GetMonster()
    {
        enemyGO = StageManager.Instance.stageData.stageIndex[0].NormalPhases[0].Enemies[0].EnemyPrefab;
        Instantiate(enemyGO, new Vector3(8.77f, 0, -1.06f), Quaternion.identity, enemyContainer);
    }

    public void CharacterAttack(float diceWeighting)
    {
        for (int i = 0; i < entryCharacters.Length; i++)
        {
            //float characterAtk = entryCharacters[i].CurrentATK;
            int characterAtk = 110;

            IDamagable damagerableEnemy = enemyGO.GetComponent<IDamagable>();
            TestEnemy enemy = (TestEnemy)damagerableEnemy;

            int monsterDef = enemy.EnemyData.Def;

            int damage = (characterAtk - monsterDef) * (int)diceWeighting;
            Debug.Log($"{characterAtk}-{monsterDef}*{(int)diceWeighting} = {damage}");
            //enemy.currentHp -= damage;
            DealDamage(damagerableEnemy, damage);
        }
    }

    public void DealDamage(IDamagable target, int damage)
    {
        target.TakeDamage(damage);
    }

    public void GetCost(int iNum)
    {
        int cost = CurrnetCost;

        cost = Mathf.Clamp(cost + iNum, 0, MaxCost);

        CurrnetCost = cost;
        costTest.text = cost.ToString();
    }
}
