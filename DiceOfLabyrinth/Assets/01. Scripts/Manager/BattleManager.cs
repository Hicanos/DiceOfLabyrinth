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
    public BattleCharacter[] entryCharacters;
    public IEnemy enemy;

    public LoadMonsterPattern LoadMonsterPattern;
    public MonsterPattern MonsterPattern;
    public BattleStateMachine stateMachine;
    public IBattleTurnState playerTurnState;
    public IBattleTurnState enemyTurnState;
    public PlayerTurnState currentPlayerState;

    public TextMeshProUGUI costTest;
    public TextMeshProUGUI monsterSkillName;
    public Button DiceRollButton;
    public Button EndTurnButton;
    public Image patternDisplay;
    public AbstractBattleButton[] BattleButtons;   

    public readonly int MaxCost = 12;
    public int CurrnetCost = 0;
    public int BattleTurn = 0;
    public bool isBattle;

    public BattleCoroutine battleCoroutine;
    public Vector3[] tempFormation = new Vector3[] { new Vector3(-13.8f, 0, 1.16f), new Vector3(-10.5f, 0, -3.17f), new Vector3(-7.4f, 0, -7.24f), new Vector3(-7.25f, 0, -0.18f), new Vector3(-3.98f, 0, -4.46f) };
    void Start()
    {
        playerTurnState = new BattlePlayerTurnState();
        
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

    public void BattleStartCoroutine()
    {
        DiceManager.Instance.DiceSettingForBattle();
        battleCoroutine.StartPrepareBattle();
    }

    public void BattleStart()
    {
        //entryCharacters = StageManager.Instance.stageSaveData.entryCharacters;
        playerTurnState.Enter();
        patternDisplay.gameObject.SetActive(true);
        DiceManager.Instance.LoadDiceData();
        
        isBattle = true;
    }

    public void BattleEnd()
    {
        isBattle = false;

        currentPlayerState = PlayerTurnState.BattleEnd;
        OnOffButton();
    }

    private void GetMonster()
    {
        //enemy = 
    }

    public void CharacterAttack(float diceWeighting)
    {
        Debug.Log("공격!");
        for (int i = 0; i < entryCharacters.Length; i++)
        {
            float characterAtk = entryCharacters[i].CurrentATK;
            float monsterDef = enemy.EnemyData.Def;
            float damage = (characterAtk - monsterDef) * diceWeighting;

            //enemy.currentHp -= damage;
            //DealDamage(IDamagerable target , int damage);
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

    public void OnOffButton()
    {
        foreach(AbstractBattleButton button in BattleButtons)
        {
            button.OnOffButton(currentPlayerState);
        }
    }

    public void GetButton()
    {
        foreach (AbstractBattleButton button in BattleButtons)
        {
            button.GetButtonComponent();
        }
    }
}
