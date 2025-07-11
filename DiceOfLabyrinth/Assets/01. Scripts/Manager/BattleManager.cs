using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;


public class BattleManager : MonoBehaviour
{
    #region 싱글톤 구현
    private static BattleManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }        
    }

    public static BattleManager Instance
    {
        get
        {
            if(instance == null)
            {
                Debug.LogWarning("BattleManager의 Instance가 null");
                return null;
            }
            return instance;
        }
    }
    #endregion

    public List<BattleCharacter> battleCharacters;
    public StageSaveData.CurrentFormationType currentFormationType;

    [SerializeField] private BattleEnemy enemy;
    public BattleEnemy Enemy => enemy;
    GameObject enemyPrefab;

    public List<ArtifactData> artifacts;
    public List<StagmaData> stagmas;    

    public InputAction inputAction;

    [SerializeField] Transform enemyContainer;
    public BattleUIValueChanger UIValueChanger;
    public BattleCoroutine battleCoroutine;

    public EnemyPatternContainer EnemyPatternContainer;
    public BattleStateMachine stateMachine;
    public PlayerTurnState currentPlayerState;
    public IBattleTurnState playerTurnState;
    public IBattleTurnState enemyTurnState;
    public BattlePlayerTurnState battlePlayerTurnState;

    [Header("UIs")]
    public List<AbstractBattleButton> BattleButtons = new List<AbstractBattleButton>();
    public Image[] CharacterHPBars;
    public Image[] EnemyHPBars;

    //[SerializeField] Image turnDisplay;
    //public GameObject Stages;    
    [NonSerialized] public GameObject fixedDiceArea;
    [NonSerialized] public GameObject stagmaDisplayer;
    [NonSerialized] public Canvas     battleCanvas;

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

        GetUIs();
        DiceManager.Instance.DiceHolding.SettingForHolding();
    }
    
    void Update()
    {
        stateMachine.BattleUpdate();
    }

    private void GetUIs()
    {
        CharacterHPBars = new Image[5];
        EnemyHPBars = new Image[1];
        BattleUI battleUI = UIManager.Instance.BattleUI;
        battleUI.Setting();

        fixedDiceArea = battleUI.fixedDiceArea;
        battleCanvas = battleUI.battleCanvas;
        stagmaDisplayer = battleUI.stagmaDisplayer;
        for(int i = 0; i < battleUI.Buttons.Length; i++)
        {
            BattleButtons.Add(battleUI.Buttons[i]);
        }

        CharacterHPBars = battleUI.CharacterHPBars;
        EnemyHPBars = battleUI.EnemyHPBars;
    }

    public void BattleStartCoroutine(BattleStartData data) //전투 시작시 호출해야할 메서드
    {
        GetStartData(data);
        SpawnEnemy();
        DiceManager.Instance.DiceSettingForBattle();

        if(StageManager.Instance.stageSaveData.currentPhaseIndex == 1)
        {
            battleCoroutine.CharacterSpawn();
        }
        else
        {
            battleCoroutine.CharacterActive();
        }        
    }

    private void GetStartData(BattleStartData data)
    {
        enemy = new BattleEnemy(data.selectedEnemy);
        battleCharacters = data.battleCharacters;
        artifacts = data.artifacts;
        stagmas = data.stagmas;
        //currentFormationType = data.
    }

    public void BattleStart()
    {
        if (StageManager.Instance.stageSaveData.currentPhaseIndex == 1)
        {
            battleCanvas.worldCamera = DiceManager.Instance.diceCamera;
            DiceManager.Instance.LoadDiceData();
        }
        BattleTurn = 0;
        EnemyHPBars[(int)HPEnumEnemy.enemy].gameObject.SetActive(true);
        UIValueChanger.ChangeEnemyHpRatio(HPEnumEnemy.enemy);

        //turnDisplay.gameObject.SetActive(true);
        isBattle = true;
        isWon = false;

        playerTurnState.Enter();        
    }

    public void BattleEnd()
    {
        BattleResultData data;
        isBattle = false;
        EnemyHPBars[(int)HPEnumEnemy.enemy].gameObject.SetActive(false);
        //turnDisplay.gameObject.SetActive(false);
        DiceManager.Instance.ResetSetting();
        //for (int i = 0; i < battleCharacters.Count; i++)
        //{
        //    HPBar[i].gameObject.SetActive(false);
        //}
        //turnDisplay.gameObject.SetActive(false);
        battleCoroutine.CharacterDeActive();
        Destroy(enemyPrefab);
        //결과창 실행
        if (isWon)
        {
            data = new BattleResultData(true, battleCharacters);
            if (StageManager.Instance.stageSaveData.currentPhaseIndex == 5)
            {
                StageManager.Instance.battleUIController.OpenVictoryPanel();                
            StageManager.Instance.OnBattleResult(data);
            }
            StageManager.Instance.RoomClear(enemy.Data);
        }
        else
        {
            data = new BattleResultData(false, battleCharacters);
            StageManager.Instance.battleUIController.OpenDefeatPanel();
        }
    }

    private void SpawnEnemy()
    {
        GameObject enemyGO = enemy.Data.EnemyPrefab;

        enemyPrefab = Instantiate(enemyGO, new Vector3(5.85f, -0.02f, -1.06f), Quaternion.identity, enemyContainer);
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
    private bool isDead;
    public EnemyData Data => data;
    public int CurrentHP => currentHP;
    public int CurrentAtk => currentAtk;
    public int CurrentDef => currentDef;
    public int MaxHP => currentMaxHP;
    public bool IsDead => isDead;

    public SOEnemySkill currentSkill;

    public BattleEnemy(EnemyData data)
    {
        this.data = data;
        currentMaxHP = data.MaxHp;
        currentHP = currentMaxHP;
        currentAtk = data.Atk;
        currentDef = data.Def;
        isDead = false;
    }

    public void Heal(int amount)
    {        
        currentHP = Mathf.Clamp(currentHP + amount, 0, currentMaxHP);
    }

    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Clamp(currentHP - damage, 0, currentMaxHP);

        if(currentHP == 0)
        {
            isDead = true;
        }
    }
}
