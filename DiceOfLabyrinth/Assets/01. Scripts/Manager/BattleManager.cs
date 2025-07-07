using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

    public List<BattleCharacter> battleCharacters;

    private BattleEnemy enemy;
    public BattleEnemy Enemy => enemy;

    GameObject enemyGO;
    public TestEnemy TestEnemy;

    [SerializeField] Transform enemyContainer;
    public BattleUIValueChanger UIValueChanger;
    public BattleCoroutine battleCoroutine;

    [Header("Battle States")]
    public LoadMonsterPattern LoadMonsterPattern;
    public MonsterPattern MonsterPattern;
    public BattleStateMachine stateMachine;
    public PlayerTurnState currentPlayerState;
    public IBattleTurnState playerTurnState;
    public IBattleTurnState enemyTurnState;
    public BattlePlayerTurnState battlePlayerTurnState;

    [Header("UIs")]
    public AbstractBattleButton[] BattleButtons;        
    public Image[] HPBar;
    [SerializeField] Image turnDisplay;
    [SerializeField] GameObject victoryUI;
    [SerializeField] GameObject defeatUI;

    [Header("Values")]
    public  readonly int MaxCost = 12;
    public  int     BattleTurn = 0;
    private int     currnetCost = 0;
    public  bool    isBattle;
    public  bool    isWon;

    void Start()
    {
        playerTurnState = new BattlePlayerTurnState();
        battlePlayerTurnState = (BattlePlayerTurnState)playerTurnState;

        enemyTurnState = new BattleEnemyTurnState();

        stateMachine = new BattleStateMachine(playerTurnState);

        LoadMonsterPattern = new LoadMonsterPattern();
        MonsterPattern = new MonsterPattern();
    }
    
    void Update()
    {
        stateMachine.BattleUpdate();
        //if(Input.GetKeyDown(KeyCode.Space))
        //{            
        //    battleCharacters[0] = CharacterManager.Instance.RegisterBattleCharacterData("Char_0");
        //    battleCharacters[1] = CharacterManager.Instance.RegisterBattleCharacterData("Char_1");
        //    battleCharacters[2] = CharacterManager.Instance.RegisterBattleCharacterData("Char_2");
        //    battleCharacters[3] = CharacterManager.Instance.RegisterBattleCharacterData("Char_3");
        //    battleCharacters[4] = CharacterManager.Instance.RegisterBattleCharacterData("Char_4");
        //    BattleStartCoroutine();
        //}
    }

    public void BattleStartCoroutine(BattleStartData data) //전투 시작시 호출해야할 메서드
    {
        GetStartData(data);
        SpawnEnemy();
        DiceManager.Instance.DiceSettingForBattle();
        battleCoroutine.CharacterSpwan();
    }

    public void BattleStart()
    {
        HPBar[(int)HPEnum.enemy].gameObject.SetActive(true);
        turnDisplay.gameObject.SetActive(true);
        playerTurnState.Enter();
        DiceManager.Instance.LoadDiceData();
        
        isBattle = true;
    }

    public void BattleEnd()
    {
        isBattle = false;
        HPBar[(int)HPEnum.enemy].gameObject.SetActive(false);
        //for(int i = 0; i < battleCharacters.Length; i++)
        //{
        //    HPBar[i].gameObject.SetActive(false);
        //}
        //turnDisplay.gameObject.SetActive(false);

        //결과창 실행
        if(isWon)
        {
            victoryUI.SetActive(true);
        }
        else
        {
            defeatUI.SetActive(true);
        }
    }

    public void GetStartData(BattleStartData data)
    {
        enemy = new BattleEnemy(data.selectedEnemy);
        battleCharacters = data.battleCharacters;
    }

    private void SpawnEnemy()
    {
        enemyGO = enemy.Data.EnemyPrefab;

        Instantiate(enemyGO, new Vector3(5.85f, -0.02f, -1.06f), Quaternion.identity, enemyContainer);
    }

    /// <summary>
    /// 스킬 사용에 필요한 코스트를 매개변수만큼 얻는 메서드입니다.
    /// </summary>
    /// <param name="iNum"></param>
    public void GetCost(int iNum)
    {
        int cost = currnetCost;

        cost = Mathf.Clamp(cost + iNum, 0, MaxCost);

        currnetCost = cost;
        UIValueChanger.ChangeUIText(BattleTextUIEnum.Cost, cost.ToString());        
    }
}

public class BattleEnemy : IDamagable
{
    private EnemyData data;
    private int currentMaxHP;
    private int currentHP;
    private int currentAtk;
    private int currentDef;

    public EnemyData Data => data;
    public int CurrentHP => currentHP;
    public int CurrentAtk => currentAtk;
    public int CurrentDef => currentDef;

    public BattleEnemy(EnemyData data)
    {
        this.data = data;
        currentMaxHP = data.MaxHp;
        currentHP = currentMaxHP;
        currentAtk = data.Atk;
        currentDef = data.Def;
    }

    public void Heal(int amount)
    {        
        currentHP = Mathf.Clamp(currentHP + amount, 0, currentMaxHP);
    }

    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Clamp(currentHP - damage, 0, currentMaxHP);
    }
}
