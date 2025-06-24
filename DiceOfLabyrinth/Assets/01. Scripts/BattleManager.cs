using UnityEngine;

public class BattleManager : MonoBehaviour
{
    #region ½Ì±ÛÅæ ±¸Çö
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

    BattleStateMachine stateMachine;
    IBattleTurnState playerTurnState;
    IBattleTurnState enemyTurnState;

    void Start()
    {
        playerTurnState = new BattlePlayerTurnState();
        enemyTurnState = new BattleEnemyTurnState();

        stateMachine = new BattleStateMachine(playerTurnState);
    }

    
    void Update()
    {
        stateMachine.BattleUpdate();
    }
}
