using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    //public GameObject[] entryCharacters; //임시
    public BattleCharacter[] battleCharacters; //임시
    GameObject enemyGO;
    public TestEnemy TestEnemy => enemyGO.GetComponent<TestEnemy>();

    [SerializeField] Transform enemyContainer;
    public Transform characterContainer;

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

        battleCharacters = new BattleCharacter[5];
    }
    
    void Update()
    {
        stateMachine.BattleUpdate();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            
            battleCharacters[0] = CharacterManager.Instance.RegisterBattleCharacterData("Char_0");
            battleCharacters[1] = CharacterManager.Instance.RegisterBattleCharacterData("Char_1");
            battleCharacters[2] = CharacterManager.Instance.RegisterBattleCharacterData("Char_2");
            battleCharacters[3] = CharacterManager.Instance.RegisterBattleCharacterData("Char_3");
            battleCharacters[4] = CharacterManager.Instance.RegisterBattleCharacterData("Char_4");
            BattleStartCoroutine();
        }
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

    }

    private void GetMonster()
    {
        int chapterIndex = StageManager.Instance.stageSaveData.currentChapterIndex;
        chapterIndex = 0; //임시
        enemyGO = StageManager.Instance.chapterData.chapterIndex[0].stageData.stageIndex[chapterIndex].NormalPhases[0].Enemies[0].EnemyPrefab;
        Instantiate(enemyGO, new Vector3(5.85f, -0.02f, -1.06f), Quaternion.identity, enemyContainer);
    }

    public void CharacterAttack(float diceWeighting)
    {
        for (int i = 0; i < battleCharacters.Length; i++)
        {                        
            int characterAtk = battleCharacters[i].CurrentATK;

            IDamagable damagerableEnemy = enemyGO.GetComponent<IDamagable>();
            TestEnemy enemy = (TestEnemy)damagerableEnemy;

            int monsterDef = enemy.EnemyData.Def;

            int damage = (characterAtk - monsterDef) * (int)diceWeighting;
            damage = Mathf.Clamp(damage, 0, damage);
            Debug.Log($"{characterAtk}-{monsterDef}*{(int)diceWeighting} = {damage}");

            DealDamage(damagerableEnemy, damage);
            if(enemy.IsDead == true)
            {
                //쓰러지는 애니메이션 있으면 좋을듯
                battlePlayerTurnState.ChangePlayerTurnState(PlayerTurnState.BattleEnd);
                Debug.Log("몬스터 쓰러짐");
                //결과창
            }
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
