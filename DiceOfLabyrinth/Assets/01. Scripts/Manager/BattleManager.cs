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
    public AbstractBattleButton[] BattleButtons;   

    public readonly int MaxCost = 12;
    public int CurrnetCost = 0;
    public int BattleTurn = 0;
    public bool isBattle;

    void Start()
    {
        playerTurnState = new BattlePlayerTurnState();
        
        enemyTurnState = new BattleEnemyTurnState();

        stateMachine = new BattleStateMachine(playerTurnState);

        LoadMonsterPattern = new LoadMonsterPattern();
        MonsterPattern = new MonsterPattern();

        BattleStart(); //테스트용        
    }

    
    void Update()
    {
        stateMachine.BattleUpdate();
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
