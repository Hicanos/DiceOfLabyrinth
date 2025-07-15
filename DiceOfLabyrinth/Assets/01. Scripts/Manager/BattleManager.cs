using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
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

    EnterBattle enterBattle = new EnterBattle();
    public BattleSpawner battleSpawner;
    public BattleUIValueChanger UIValueChanger;

    public BattleEnemy Enemy;
    public BattleCharGroup BattleGroup;

    public BattleCharacterAttack CharacterAttack;
    public BattleEnemyAttack EnemyAttack;
    public EnemyPatternContainer EnemyPatternContainer;

    public GameObject CharacterHPPrefab;
    public GameObject EnemyHPPrefab;

    public InputAction inputAction;
    
    public BattleStateMachine stateMachine;
    public PlayerTurnState currentPlayerState;
    public IBattleTurnState playerTurnState;
    public IBattleTurnState enemyTurnState;
    public BattlePlayerTurnState battlePlayerTurnState;

    [Header("Values")]
    public  readonly int MaxCost = 12;
    public  int     BattleTurn   = 0;
    private int     currnetCost  = 0;
    public  bool    isBattle;
    public  bool    isWon;

    void Start()
    {
        playerTurnState = new BattlePlayerTurnState();

        battlePlayerTurnState = (BattlePlayerTurnState)playerTurnState;

        enemyTurnState = new BattleEnemyTurnState();

        stateMachine = new BattleStateMachine(playerTurnState);

        UIManager.Instance.BattleUI.Setting();
        DiceManager.Instance.DiceHolding.SettingForHolding();
    }
    
    void Update()
    {
        stateMachine.BattleUpdate();
    }
    public void StartBattle(BattleStartData data) //전투 시작시
    {
        GetStartData(data);
        enterBattle.BattleStart();
    }

    private void GetStartData(BattleStartData data) //start시 호출되도록
    {
        Enemy = new BattleEnemy(data.selectedEnemy);
        BattleGroup = new BattleCharGroup(data.battleCharacters, data.artifacts, data.stagmas);
    }

    public void EndBattle()
    {
        BattleResultData data;
        isBattle = false;

        DiceManager.Instance.ResetSetting();

        battleSpawner.CharacterDeActive();
        Destroy(Enemy.EnemyPrefab);
        //결과창 실행
        if (isWon)
        {
            data = new BattleResultData(true, BattleGroup.BattleCharacters);
            if (StageManager.Instance.stageSaveData.currentPhaseIndex == 5)
            {
                StageManager.Instance.battleUIController.OpenVictoryPanel();                
            StageManager.Instance.OnBattleResult(data);
            }
            StageManager.Instance.RoomClear(Enemy.Data);
        }
        else
        {
            data = new BattleResultData(false, BattleGroup.BattleCharacters);
            StageManager.Instance.battleUIController.OpenDefeatPanel();
        }
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

    public void BattleStartValueSetting()
    {
        BattleTurn = 0;
        isBattle = true;
        isWon = false;
    }
}

public class BattleCharGroup
{
    private List<BattleCharacter>  battleCharacters;
    private List<ArtifactData>     artifacts;
    private List<StagmaData>       stagmas;

    public List<BattleCharacter> BattleCharacters => battleCharacters;
    public List<ArtifactData>    Artifacts => artifacts;
    public List<StagmaData>      Stagmas => stagmas;

    public GameObject[] CharacterPrefabs = new GameObject[5];
    public StageSaveData.CurrentFormationType CurrentFormationType;

    [NonSerialized] public GameObject[]      CharacterHPBars  = new GameObject[5];
    [NonSerialized] public RectTransform[]   CharacterHPs     = new RectTransform[5];
    [NonSerialized] public TextMeshProUGUI[] CharacterHPTexts = new TextMeshProUGUI[5];

    public BattleCharGroup(List<BattleCharacter> characters, List<ArtifactData> artifacts, List<StagmaData> stagmas)
    {
        battleCharacters = characters; this.artifacts = artifacts; this.stagmas = stagmas;
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

    public GameObject EnemyPrefab;
    public IEnemy iEnemy;

    public GameObject EnemyHPBar;
    public RectTransform EnemyHP;
    public TextMeshProUGUI EnemyHPText;

    public SOEnemySkill currentSkill;
    public int currentSkill_Index;
    public List<int> currentTargetIndex;

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
