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

    public BattleCharacter[] battleCharacters; //임시
    public GameObject[] characterPrefabs;
    GameObject enemyGO;
    [field:SerializeField] public TestEnemy TestEnemy;
    //[field:SerializeField] EnemyData enemyData;

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
    public TextMeshProUGUI turnText;
    public Button DiceRollButton;
    public AbstractBattleButton[] BattleButtons;
    [SerializeField] Image turnDisplay;
    [SerializeField] Image enemyHPBar;
    public RectTransform enemyHPImage;

    public readonly int MaxCost = 12;
    public int CurrnetCost = 0;
    public int BattleTurn = 0;
    public bool isBattle;

    public BattleCoroutine battleCoroutine;
    IEnumerator moveAttackCoroutine;
    void Start()
    {
        playerTurnState = new BattlePlayerTurnState();
        battlePlayerTurnState = (BattlePlayerTurnState)playerTurnState;

        enemyTurnState = new BattleEnemyTurnState();

        stateMachine = new BattleStateMachine(playerTurnState);

        LoadMonsterPattern = new LoadMonsterPattern();
        MonsterPattern = new MonsterPattern();

        battleCharacters = new BattleCharacter[5];
        characterPrefabs = new GameObject[5];
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
        battleCoroutine.StartPrepareBattle();
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

        Instantiate(enemyGO, new Vector3(5.85f, -0.02f, -1.06f), Quaternion.identity, enemyContainer);
    }

    /// <summary>
    /// 캐릭터가 공격할 때 실행하는 코루틴을 실행하는 메서드입니다.
    /// </summary>    
    public void CharacterAttack(float diceWeighting)
    {
        moveAttackCoroutine = CharacterAttackCoroutine(diceWeighting);
        StartCoroutine(moveAttackCoroutine);
    }

    private void StopCharacterAttack()
    {
        StopCoroutine(moveAttackCoroutine);
    }

    IEnumerator CharacterAttackCoroutine(float diceWeighting)
    {
        int monsterDef = TestEnemy.EnemyData.Def;
        float pastTime, destTime = 0.5f;
        Vector3 attackPosition = new Vector3(3.25f, 0, 0);

        for (int i = 0; i < battleCharacters.Length; i++)
        {
            int characterAtk = battleCharacters[i].CurrentATK;

            int damage = (characterAtk - monsterDef) * (int)diceWeighting;
            damage = Mathf.Clamp(damage, 0, damage);            

            Vector3 firstPosition = characterPrefabs[i].transform.position;

            pastTime = 0;
            while (pastTime < destTime)
            {
                characterPrefabs[i].transform.position = Vector3.Lerp(firstPosition, attackPosition, pastTime / destTime);

                pastTime += Time.deltaTime;
                yield return null;
            }
            DealDamage(TestEnemy, damage/10);
            pastTime = 0;
            while (pastTime < destTime)
            {
                characterPrefabs[i].transform.position = Vector3.Lerp(attackPosition, firstPosition, pastTime / destTime);

                pastTime += Time.deltaTime;
                yield return null;
            }

            if (TestEnemy.IsDead == true)
            {
                //쓰러지는 애니메이션 있으면 좋을듯
                battlePlayerTurnState.ChangePlayerTurnState(PlayerTurnState.BattleEnd);
                Debug.Log("몬스터 쓰러짐");
                BattleEnd();
                StopCharacterAttack();
                //결과창
            }            
            yield return null;
        }
        stateMachine.ChangeState(enemyTurnState);
    }

    public void DealDamage(IDamagable target, int damage)
    {
        target.TakeDamage(damage);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="iNum"></param>
    public void GetCost(int iNum)
    {
        int cost = CurrnetCost;

        cost = Mathf.Clamp(cost + iNum, 0, MaxCost);

        CurrnetCost = cost;
        costTest.text = cost.ToString();
    }
}
