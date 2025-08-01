using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
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

    public BattleSpawner BattleSpawner;
    public BattleUIValueChanger UIValueChanger;
    public BattleUIHP BattleUIHP;
    public BattleTutorial BattleTutorial;

    public BattleEnemy Enemy;
    public BattleCharGroup BattleGroup;

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

    public EngravingBuffMaker EngravingBuffMaker = new EngravingBuffMaker();
    public EngravingBuffContainer EngravingBuffs = new EngravingBuffContainer();
    public EngravingAdditionalStatus EngravingAdditionalStatus;

    public ArtifactBuffMaker ArtifactBuffMaker = new ArtifactBuffMaker();
    public ArtifactBuffContainer ArtifactBuffs = new ArtifactBuffContainer();
    public ArtifactAdditionalStatus ArtifactAdditionalStatus;

    [Header("Values")]
    public bool     IsTutorialOver;
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

    public void EnterBattleSettings()
    {
        EngravingBuffMaker.MakeEngravingBuff();
        ArtifactBuffMaker.MakeArtifactBuff();
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

        BattleSpawner.CharacterDeActive();
        BattleSpawner.DeactiveCharacterHP(BattleGroup);

        Destroy(Enemy.EnemyPrefab);

        IsBattle = false;
    }

    public void ExitStageSetting()
    {
        Debug.Log("익시트 스테이지");
        IsBattle = false;
        InputManager.Instance.BattleInputEnd();
        BattleGroup = null;
        InBattleStage = false;

        BattleSpawner.DestroyDices();
    }

    private void GetStartData(BattleStartData data) //start시 호출되도록
    {
        Enemy = new BattleEnemy(data.selectedEnemy);

        if (BattleGroup == null)
        {
            BattleGroup = new BattleCharGroup(data.battleCharacters, data.artifacts, data.engravings);
        }
        manastoneAmount = data.manaStone;
    }

    private void CheckDataChanged()
    {

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
            data = new BattleResultData(true, BattleGroup.BattleCharacters, manastoneAmount);

            if (IsStageClear)
            {
                ExitStageSetting();
            }            
            
            StageManager.Instance.OnBattleResult(data);            
        }
        else
        {
            data = new BattleResultData(false, BattleGroup.BattleCharacters, manastoneAmount);
            ExitStageSetting();
            StageManager.Instance.OnBattleResult(data);
        }
    }    

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
        string st = $"{currentCost} / {MaxCost}";
        UIValueChanger.ChangeUIText(BattleTextUIEnum.Cost, st);
    }

    public void SpendCost(int iNum)
    {
        int cost = currentCost;

        cost = Mathf.Clamp(cost - iNum, 0, MaxCost);
        currentCost = cost;
        string st = $"{currentCost} / {MaxCost}";
        UIValueChanger.ChangeUIText(BattleTextUIEnum.Cost, st);
        ArtifactBuffs.ActionSpendCost();
    }    
}

public class BattleCharGroup
{
    const int numFive = 5;
    BattleManager battleManager;

    private List<BattleCharacter>  battleCharacters;
    private List<ArtifactData> artifacts;
    private List<EngravingData> engravings;
    //private EngravingEffect[]    engravingEffects;
    public List<BattleCharacter> BattleCharacters => battleCharacters;

    public List<ArtifactData> Artifacts => artifacts;
    public List<EngravingData> Engravings => engravings;  

    public GameObject[] CharacterPrefabs = new GameObject[numFive];
    public StageSaveData.CurrentFormationType CurrentFormationType;

    private int[] barrierAmounts = new int[numFive];
    public int[] BarrierAmounts => barrierAmounts;

    [NonSerialized] public GameObject[]      CharacterHPBars  = new GameObject[numFive];
    [NonSerialized] public RectTransform[]   CharacterHPs     = new RectTransform[numFive];
    [NonSerialized] public RectTransform[]   CharacterBarriers= new RectTransform[numFive];
    [NonSerialized] public RectTransform[]   CharacterBlank = new RectTransform[numFive];
    [NonSerialized] public TextMeshProUGUI[] CharacterHPTexts = new TextMeshProUGUI[numFive];
    [NonSerialized] public HorizontalLayoutGroup[] LayoutGroups = new HorizontalLayoutGroup[numFive];

    public List<int> FrontLine = new List<int>();
    public List<int> BackLine = new List<int>();
    private int frontLineNum;

    public List<int> DeadIndex = new List<int>();
    public int DeadCount;
    private bool isAllDead => DeadCount == numFive ? true : false;
    private int currentHitIndex;
    private int currentHittedDamage;
    private int currentDeadIndex;
    public int CurrentDeadIndex => currentDeadIndex;

    public BattleCharGroup(List<BattleCharacter> characters, List<ArtifactData> artifacts, List<EngravingData> engravings)
    {
        battleManager = BattleManager.Instance;

        battleCharacters = characters; this.artifacts = artifacts; this.engravings = engravings;

        CurrentFormationType = StageManager.Instance.stageSaveData.currentFormationType;
        frontLineNum = (int)CurrentFormationType;

        for(int i = 0; i < numFive; i++)
        {
            if (battleCharacters[i].CurrentHP == 0)
            {
                //characters[i].IsDied == true;
                DeadIndex.Add(i);
                DeadCount++;
            }
            else
            {
                DeadIndex.Remove(i);
                DeadCount--;
            }
        }

        for (int i = 0; i < frontLineNum + 1; i++)
        {
            if (DeadIndex.Contains(i)) continue;
            FrontLine.Add(i);
        }
        for (int i = frontLineNum + 1; i < numFive; i++)
        {
            if (DeadIndex.Contains(i)) continue;
            BackLine.Add(i);
        }
    }

    public void CharacterHeal(int index, int amount)
    {
        BattleCharacters[index].Heal(amount);
    }

    public void CharacterGetBarrier(float effectRatio)
    {
        float amount = currentHittedDamage * effectRatio;

        barrierAmounts[currentHitIndex] = (int)amount;
        battleManager.UIValueChanger.ChangeCharacterHp((HPEnumCharacter)currentHitIndex);
    }

    public void CharacterHit(int index, int amount)
    {
        LayoutGroups[index].childControlWidth = false;

        if (barrierAmounts[index] >= amount)
        {
            barrierAmounts[index] = barrierAmounts[index] - amount;
        }
        else
        {
            amount = amount - barrierAmounts[index];
            CharacterHPHit(index, amount);
        }

        battleManager.UIValueChanger.ChangeCharacterHp((HPEnumCharacter)index);
        LayoutGroups[index].childControlWidth = true;
    }

    public void CharacterHPHit(int index, int damage)
    {
        currentHitIndex = index;
        currentHittedDamage = damage;
        BattleCharacters[index].TakeDamage(damage);
        battleManager.ArtifactBuffs.ActionCharacterHit();
    }

    public void CharacterDead(int index)
    {
        DeadCount++;
        currentDeadIndex = index;
        DeadIndex.Add(index);
        battleManager.ArtifactBuffs.ActionCharacterDie();

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
            battleManager.EndBattle(false);
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
    private int currentBarrier;
    private bool isDead;
    public float DebuffAtk;
    public float DebuffDef;
    public EnemyData Data => data;
    public int CurrentHP => currentHP;
    public int CurrentAtk => currentAtk;
    public int CurrentDef => currentDef;
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

    int currentHittedDamage;

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
        LayoutGroups.childControlWidth = false;

        if (currentBarrier >= damage)
        {
            currentBarrier = currentBarrier - damage;
        }
        else
        {
            damage = damage - currentBarrier;
            EnemyHPHit(damage);
        }

        BattleManager.Instance.UIValueChanger.ChangeEnemyHpUI(HPEnumEnemy.enemy);
        LayoutGroups.childControlWidth = true;
    }

    private void EnemyHPHit(int damage)
    {
        currentHittedDamage = damage;
        DamageHP(damage);
    }

    private void DamageHP(int damage)
    {
        currentHP = Mathf.Clamp(currentHP - damage, 0, currentMaxHP);

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
}