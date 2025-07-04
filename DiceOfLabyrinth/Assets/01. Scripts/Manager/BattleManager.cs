using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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

    public BattleCharacter[] battleCharacters; //임시

    GameObject enemyGO;
    [field:SerializeField] public TestEnemy TestEnemy;
    //[field:SerializeField] EnemyData enemyData;

    [SerializeField] Transform enemyContainer;
    public BattleUIValueChanger UIValueChanger;
    public BattleCoroutine battleCoroutine;

    [Header("Battle State")]
    public LoadMonsterPattern LoadMonsterPattern;
    public MonsterPattern MonsterPattern;
    public BattleStateMachine stateMachine;
    public PlayerTurnState currentPlayerState;
    public IBattleTurnState playerTurnState;
    public IBattleTurnState enemyTurnState;
    public BattlePlayerTurnState battlePlayerTurnState;

    [Header("UI")]
    public AbstractBattleButton[] BattleButtons;        
    [SerializeField] Image turnDisplay;
    [SerializeField] Image enemyHPBar;    

    public  readonly int MaxCost = 12;
    public  int     BattleTurn = 0;
    private int     currnetCost = 0;
    public  bool    isBattle;
        
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

    /// <summary>
    /// 전투를 시작할때 실행시켜야할 메서드
    /// </summary>
    public void BattleStartCoroutine()
    {
        GetMonster();
        
        DiceManager.Instance.DiceSettingForBattle();
        battleCoroutine.CharacterSpwan();
    }

    public void BattleStart()
    {                
        enemyHPBar.gameObject.SetActive(true);
        turnDisplay.gameObject.SetActive(true);
        playerTurnState.Enter();
        DiceManager.Instance.LoadDiceData();
        
        isBattle = true;
    }

    /// <summary>
    /// 전투가 끝났을 때 실행시켜야할 메서드, 결과창을 띄웁니다.
    /// </summary>
    public void BattleEnd()
    {
        isBattle = false;
        enemyHPBar.gameObject.SetActive(false);
        turnDisplay.gameObject.SetActive(false);

        //결과창 실행
    }

    private void GetMonster()
    {
        int chapterIndex = StageManager.Instance.stageSaveData.currentChapterIndex;
        chapterIndex = 0; //임시
        enemyGO = StageManager.Instance.chapterData.chapterIndex[0].stageData.stageIndex[chapterIndex].NormalPhases[0].Enemies[0].EnemyPrefab;
        TestEnemy = enemyGO.GetComponent<TestEnemy>();
        TestEnemy.Init();
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
