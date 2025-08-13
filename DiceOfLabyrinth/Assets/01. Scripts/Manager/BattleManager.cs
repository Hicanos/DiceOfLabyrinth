using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
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

    public BattleSpawner BattleSpawner;
    public BattleUIValueChanger UIValueChanger;
    public BattleUIHP BattleUIHP;
    public BattleTutorial BattleTutorial;

    public BattleEnemy Enemy;
    public BattlePartyData PartyData;

    public BattleCharacterAttack CharacterAttack;
    public BattleEnemyAttack EnemyAttack;
    public EnemyPatternContainer EnemyPatternContainer;    
    
    public BattleStateMachine StateMachine;
    public DetailedTurnState CurrentDetailedState;
    public IBattleTurnState I_EnterBattleState;
    public IBattleTurnState I_PlayerTurnState;
    public IBattleTurnState I_EnemyTurnState;
    public IBattleTurnState I_FinishBattleState;
    public BattleStatePlayerTurn BattlePlayerTurnState;

    public ISkillMaker EngravingBuffMaker = new EngravingBuffMaker();
    public EngravingBuffContainer EngravingBuffs = new EngravingBuffContainer();
    public EngravingAdditionalStatus EngravingAdditionalStatus;

    public ISkillMaker ArtifactBuffMaker = new ArtifactBuffMaker();
    public ArtifactBuffContainer ArtifactBuffs = new ArtifactBuffContainer();
    public ArtifactAdditionalStatus ArtifactAdditionalStatus;

    [Header("Values")]
    public  int     BattleTurn;
    public  int     CostSpendedInTurn;
    public  bool    IsBattle;
    public  bool    InBattleStage;
    public  bool    IsStageClear;
    public  bool    IsBoss;
    public  bool    IsWon;
    private  readonly int maxCost = 12;
    private int     currentCost;
    public  float   WaitSecondEndBattle;
    public int MaxCost => maxCost + (int)ArtifactAdditionalStatus.AdditionalMaxCost;
    private int manastoneAmount;

    void Start()
    {
        StateMachine = new BattleStateMachine();

        I_EnterBattleState  = new BattleStateEnterBattle();
        I_PlayerTurnState   = new BattleStatePlayerTurn();
        I_EnemyTurnState    = new BattleStateEnemyTurn();
        I_FinishBattleState = new BattleStateFinishBattle();

        BattlePlayerTurnState = (BattleStatePlayerTurn)I_PlayerTurnState;

        UIManager.Instance.BattleUI.Setting();
        DiceManager.Instance.DiceHolding.SettingForHolding();
    }

    private void OnDestroy()
    {
        if (InputManager.Instance == null) return;

        InputManager.Instance.BattleInputEnd();
    }

    void Update()
    {
        if (IsBattle)
        {
            StateMachine.BattleUpdate();
        }
    }

    public void StartBattle(BattleStartData data) //전투 시작시
    {
        GetStartData(data);
                
        BattleTutorial.LoadData();
        

        ArtifactAdditionalStatus = new ArtifactAdditionalStatus();
        EngravingAdditionalStatus = new EngravingAdditionalStatus();
        
        StateMachine.ChangeState(I_EnterBattleState);
    }

    private void GetStartData(BattleStartData data) //start시 호출되도록
    {
        Enemy = new BattleEnemy(data.selectedEnemy);

        PartyData = new BattlePartyData(data.battleCharacters, data.artifacts, data.engravings);

        manastoneAmount = data.manaStone;
    }

    private void CheckDataChanged()
    {

    }

    public void EnterBattleSettings()
    {
        EngravingBuffMaker.MakeSkill();
        ArtifactBuffMaker.MakeSkill();
        BattleSpawner.SpawnCharacters();
        BattleSpawner.SpawnEnemy();

        BattleTurn = 0;
        IsWon = false;
        IsBattle = true;
        InBattleStage = true;
        IsStageClear = false;
    }

    public void FinishBattleSetting()
    {
        ArtifactBuffs.RemoveAllBuffs();
        EngravingBuffs.RemoveAllBuffs();
        ArtifactAdditionalStatus.ResetStatus();
        EngravingAdditionalStatus.ResetStatus();

        //BattleSpawner.CharacterDeActive();
        //BattleSpawner.DeactiveCharacterHP(PartyData);

        Destroy(Enemy.EnemyPrefab);

        IsBattle = false;
    }

    public void ExitStageSetting()
    {
        Debug.Log("익시트 스테이지");
        IsBattle = false;
        InputManager.Instance.BattleInputEnd();
        BattleSpawner.DestroyCharacters();
        BattleSpawner.DestroyDices();

        PartyData = null;
        InBattleStage = false;
    }        

    public void EndBattle(bool isWon = true)
    {
        StartCoroutine(EndBattleCoroutine(isWon));
    }

    IEnumerator EndBattleCoroutine(bool isWon = true)
    {        
        BattleResultData data;
        IsWon = isWon;

        yield return new WaitForSeconds(WaitSecondEndBattle);
        StateMachine.ChangeState(I_FinishBattleState);

        manastoneAmount = (int)((manastoneAmount + ArtifactAdditionalStatus.AdditionalStone) * EngravingAdditionalStatus.AdditionalStone);
        //결과창 실행
        if (isWon)
        {
            data = new BattleResultData(true, PartyData.GetEndBattleCharacter(), manastoneAmount);

            ExitStageSetting();
            //if (IsStageClear)
            //{
            //    ExitStageSetting();
            //}
            
            StageManager.Instance.OnBattleResult(data);            
        }
        else
        {
            data = new BattleResultData(false, PartyData.GetEndBattleCharacter(), manastoneAmount);
            ExitStageSetting();
            StageManager.Instance.OnBattleResult(data);
        }
    }    

    //public void ChangeState()
    //{
    //    IBattleTurnState state;

    //    StateMachine.ChangeState(state);
    //}

    public void EndPlayerTurn()
    {
        StateMachine.ChangeState(I_EnemyTurnState);
    }

    public void EndEnemyTurn()
    {
        StateMachine.ChangeState(I_PlayerTurnState);
    }

    /// <summary>
    /// 스킬 사용에 필요한 코스트를 매개변수만큼 얻는 메서드입니다.
    /// </summary>
    /// <param name="iNum"></param>
    public void GetCost(int iNum)
    {
        int cost = currentCost;

        cost = Mathf.Clamp(cost + iNum, 0, MaxCost);
        currentCost = cost;
        string st = $"{currentCost}/{MaxCost}";
        UIValueChanger.ChangeUIText(BattleTextUIEnum.Cost, st);
    }

    public void SpendCost(int iNum)
    {
        int cost = currentCost;
        if (cost < iNum)
        {
            Debug.Log("코스트가 부족합니다.");
            return;
        }
        cost = Mathf.Clamp(cost - iNum, 0, MaxCost);
        currentCost = cost;
        string st = $"{currentCost}/{MaxCost}";
        UIValueChanger.ChangeUIText(BattleTextUIEnum.Cost, st);
        ArtifactBuffs.ActionSpendCost();
    }    
}

public class BattleEnemy : IDamagable
{
    private EnemyData data;
    private int currentMaxHP;
    private int currentHP;
    private int currentAtk;
    private int currentDef;
    private int currentBarrier;
    private bool isDead;
    private bool isBarrierOn;
    public float DebuffAtk;
    public float DebuffDef;

    public int AdditionalAtk;
    public int AdditionalDef;

    public EnemyData Data => data;
    public int CurrentHP => currentHP;
    public int CurrentAtk => currentAtk + AdditionalAtk;
    public int CurrentDef => currentDef + AdditionalDef;
    public int MaxHP => currentMaxHP;
    public bool IsDead => isDead;
    public int CurrentBarrier => currentBarrier;

    public GameObject EnemyPrefab;
    public IEnemy iEnemy;

    [NonSerialized] public GameObject EnemyHPBars;
    [NonSerialized] public RectTransform EnemyHPs;
    [NonSerialized] public RectTransform EnemyBarriers;
    [NonSerialized] public RectTransform EnemyBlank;
    [NonSerialized] public TextMeshProUGUI EnemyHPTexts;
    [NonSerialized] public HorizontalLayoutGroup LayoutGroups;

    public SOEnemySkill currentSkill;
    public int currentSkill_Index;
    public List<int> currentTargetIndex;
    public EnemyPassiveContainer PassiveContainer;

    int currentHittedDamage;

    public BattleEnemy(EnemyData data)
    {
        this.data = data;
        currentMaxHP = data.MaxHp;
        currentHP = currentMaxHP;
        currentAtk = data.Atk;
        currentDef = data.Def;
        isDead = false;
        PassiveContainer = new EnemyPassiveContainer();
    }

    public void Heal(int amount)
    {        
        currentHP = Mathf.Clamp(currentHP + amount, 0, currentMaxHP);

        UpdateHPBar();
    }

    public void TakeDamage(int damage)
    {
        if (isBarrierOn)
        {
            TakeDamageBarrier(damage);
        }
        else
        {
            damage = damage - currentBarrier;
            TakeDamageHP(damage);
        }

        UpdateHPBar();
    }

    private void TakeDamageBarrier(int damage)
    {
        if (currentBarrier >= damage)
        {
            currentBarrier -= damage;
        }
        else
        {
            isBarrierOn = false;
            currentBarrier = 0;
            TakeDamageHP(damage - currentBarrier);
        }
    }

    private void TakeDamageHP(int damage)
    {
        Debug.Log("TakeDamageHP");
        currentHP = Mathf.Clamp(currentHP - damage, 0, currentMaxHP);

        PassiveContainer.ActionPassiveEnemyHit();

        if (currentHP == 0)
        {
            EnemyIsDead();
        }
    }

    private void EnemyIsDead()
    {
        isDead = true;

        if(Data.Type == EnemyData.EnemyType.Guardian || Data.Type == EnemyData.EnemyType.Lord)
        {
            BattleManager.Instance.IsStageClear = true;
        }

        BattleManager.Instance.BattlePlayerTurnState.ChangeDetailedTurnState(DetailedTurnState.EndTurn);
        BattleManager.Instance.EndBattle();
    }

    public void GetBarrier(float value)
    {
        isBarrierOn = true;
        float amount = MaxHP * value;

        currentBarrier += (int)amount;

        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        LayoutGroups.childControlWidth = false;
        BattleManager.Instance.UIValueChanger.ChangeEnemyHpUI(HPEnumEnemy.enemy);
        LayoutGroups.childControlWidth = true;
    }
}