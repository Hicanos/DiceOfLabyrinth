using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public BattleSpawner BattleSpawner;
    public BattleUIValueChanger UIValueChanger;
    //[SerializeField] BattleInput battleInput;

    public BattleEnemy Enemy;
    public BattleCharGroup BattleGroup;

    public BattleCharacterAttack CharacterAttack;
    public BattleEnemyAttack EnemyAttack;
    public EnemyPatternContainer EnemyPatternContainer;

    public GameObject CharacterHPPrefab;
    public GameObject EnemyHPPrefab;
    public BattleUIHP BattleUIHP;
    
    public BattleStateMachine StateMachine;
    public DetailedTurnState CurrentDetailedState;
    public IBattleTurnState I_PlayerTurnState;
    public IBattleTurnState I_EnemyTurnState;
    public BattlePlayerTurnState BattlePlayerTurnState;
    private IBattleTurnState currentBattleState;

    public EngravingBuffMaker EngravingBuffMaker = new EngravingBuffMaker();
    public EngravingBuffContainer EngravingBuffs = new EngravingBuffContainer();
    public EngravingAdditionalStatus EngravingAdditionalStatus = new EngravingAdditionalStatus();

    //public AdditionalValues ArtifactAdditionalValue = new AdditionalValues();


    [Header("Values")]
    private  readonly int maxCost = 12;
    public  int     BattleTurn   = 0;
    private int     currnetCost  = 0;
    public  bool    isBattle;
    public  bool    IsBoss;
    public  bool    IsWon;
    public int MaxCost => maxCost;

    void Start()
    {
        I_PlayerTurnState = new BattlePlayerTurnState();

        BattlePlayerTurnState = (BattlePlayerTurnState)I_PlayerTurnState;

        I_EnemyTurnState = new BattleEnemyTurnState();

        StateMachine = new BattleStateMachine(I_PlayerTurnState);
        currentBattleState = I_PlayerTurnState;        

        UIManager.Instance.BattleUI.Setting();
        DiceManager.Instance.DiceHolding.SettingForHolding();               
    }
    
    void Update()
    {
        StateMachine.BattleUpdate();
        BattleStateUpdate();
    }

    private void BattleStateUpdate()
    {
        if(currentBattleState != StateMachine.currentState)
        {
            currentBattleState = StateMachine.currentState;
            StateMachine.ChangeState(currentBattleState);
        }
    }

    public void StartBattle(BattleStartData data) //전투 시작시
    {
        IsWon = false;
        GetStartData(data);
        BattleStartValueSetting();
        EngravingBuffMaker.MakeEngravingBuff();
        InputManager.Instance.BattleInputStart();
        enterBattle.BattleStart();
    }

    private void GetStartData(BattleStartData data) //start시 호출되도록
    {
        Enemy = new BattleEnemy(data.selectedEnemy);

        if (BattleGroup == null)
        {
            BattleGroup = new BattleCharGroup(data.battleCharacters, data.artifacts, data.engravings);
        }        
    }

    public void EndBattle(bool isWon = true)
    {
        BattleResultData data;
        isBattle = false;
        IsWon = isWon;

        BattleSpawner.DeactiveCharacterHP(BattleGroup);
        DiceManager.Instance.ResetSetting();

        BattleSpawner.CharacterDeActive();
        Destroy(Enemy.EnemyPrefab);
        //결과창 실행
        if (isWon)
        {
            //for (int i = 0; i < BattleGroup.BattleEngravings.Length; i++)
            //{
            //    BattleGroup.BattleEngravings[i].GetEngravingEffectInBattleEnd();
            //}

            data = new BattleResultData(true, BattleGroup.BattleCharacters);
            
            StageManager.Instance.OnBattleResult(data);            
        }
        else
        {
            data = new BattleResultData(false, BattleGroup.BattleCharacters);

            StageManager.Instance.OnBattleResult(data);
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

    private void BattleStartValueSetting()
    {
        BattleTurn = 0;
        isBattle = true;
    }

    private void ExitBattleSetting()
    {
        BattleGroup = null;
        isBattle = false;
        DiceManager.Instance.DestroyDices();
        InputManager.Instance.BattleInputEnd();
    }
}

public class BattleCharGroup
{
    const int numFive = 5;

    private List<BattleCharacter>  battleCharacters;
    private List<ArtifactData> artifacts;
    private List<EngravingData> engravings;
    //private EngravingEffect[]    engravingEffects;
    public List<BattleCharacter> BattleCharacters => battleCharacters;

    public List<ArtifactData> Artifacts => artifacts;
    public List<EngravingData> Engravings => engravings;
    //public EngravingEffect[]   BattleEngravings => engravingEffects;    

    public GameObject[] CharacterPrefabs = new GameObject[numFive];
    public StageSaveData.CurrentFormationType CurrentFormationType;

    [NonSerialized] public GameObject[]      CharacterHPBars  = new GameObject[numFive];
    [NonSerialized] public RectTransform[]   CharacterHPs     = new RectTransform[numFive];
    [NonSerialized] public TextMeshProUGUI[] CharacterHPTexts = new TextMeshProUGUI[numFive];

    public List<int> FrontLine = new List<int>();
    public List<int> BackLine = new List<int>();
    private int frontLineNum;

    public List<int> DeadIndex = new List<int>();
    public int DeadCount;
    private bool isAllDead => DeadCount == numFive ? true : false;

    public BattleCharGroup(List<BattleCharacter> characters, List<ArtifactData> artifacts, List<EngravingData> engravings)
    {
        battleCharacters = characters; this.artifacts = artifacts; this.engravings = engravings;

        CurrentFormationType = StageManager.Instance.stageSaveData.currentFormationType;
        frontLineNum = (int)CurrentFormationType;

        for (int i = 0; i < frontLineNum + 1; i++)
        {
            FrontLine.Add(i);
        }
        for (int i = frontLineNum + 1; i < numFive; i++)
        {
            BackLine.Add(i);
        }
    }

    public void CharacterDead(int index)
    {
        DeadCount++;
        DeadIndex.Add(index);

        if (FrontLine.Contains<int>(index))
        {
            FrontLine.Remove(index);
        }
        else if(BackLine.Contains<int>(index))
        {
            BackLine.Remove(index);
        }

        if (isAllDead)
        {
            BattleManager.Instance.EndBattle(false);
        }
    }

    public void CharacterRevive(int index)
    {
        DeadCount--;

        if (index <= frontLineNum)
        {
            FrontLine.Add(index);
            FrontLine.Sort();
            battleCharacters[index].Revive();
        }
        else if (index > frontLineNum)
        {
            BackLine.Add(index);
            BackLine.Sort();
            battleCharacters[index].Revive();
        }
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
    public float DebuffAtk;
    public float DebuffDef;
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