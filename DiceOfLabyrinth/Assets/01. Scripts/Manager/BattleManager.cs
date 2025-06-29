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

    public BattleStateMachine stateMachine;
    public IBattleTurnState playerTurnState;
    public IBattleTurnState enemyTurnState;

    public TextMeshProUGUI costTest;
    public Button DiceRollButton;
    public Button ConfirmButton; //공격 -> 턴 넘어감

    public readonly int MaxCost = 12;
    public int CurrnetCost = 0;
    public int BattleTurn = 0;
    public bool isBattle;

    void Start()
    {
        playerTurnState = new BattlePlayerTurnState();
        
        enemyTurnState = new BattleEnemyTurnState();

        stateMachine = new BattleStateMachine(playerTurnState);

        playerTurnState.Enter(); //테스트용
        DiceManager.Instance.LoadDiceData();
        isBattle = true;
    }

    
    void Update()
    {
        stateMachine.BattleUpdate();
    }

    public void BattleStart()
    {
        playerTurnState.Enter();
        DiceManager.Instance.LoadDiceData();
        isBattle = true;
    }

    public void BattleEnd()
    {
        isBattle = false;
    }
}
