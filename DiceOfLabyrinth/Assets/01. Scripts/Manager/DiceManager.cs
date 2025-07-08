using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PredictedDice;
using PredictedDice.Demo;

public class DiceManager : MonoBehaviour
{
    #region 싱글톤 구현
    private static DiceManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        //else
        //{
        //    Destroy(this.gameObject);
        //}
        _inputActions = new InputSystem_Actions();
    }

    public static DiceManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
    #endregion

    [SerializeField] GameObject diceContainer;
    [SerializeField] GameObject fakeDiceContainer;
    public DiceHolding DiceHolding;
    public GameObject[] dices;
    public GameObject[] fakeDices;
    public GameObject ground;
    public GameObject DiceBoard;

    [SerializeField] RollMultipleDiceSynced roll;
    public Camera diceCamera;
    public DiceBattle DiceBattle = new DiceBattle();
    DiceMy[] dicesDatas;

    private int[] diceResult;
    public int[] DiceResult => diceResult;

    private int[] diceResultCount;
    public int[] DiceResultCount => diceResultCount;

    int[] defaultDiceResultCount;

    private List<int> fixedDiceList;
    public List<int> FixedDiceList => fixedDiceList;

    private List<int> tempFixedDiceList;
    public List<int> TempFixedDiceList => tempFixedDiceList;

    const int maxDiceNum = 6;
    const int diceCount = 5;

    int signitureAmount;
    public int rollCount = 0;
    public readonly int maxRollCount = 3;
    public bool isSkipped = false;
    public bool isRolling = false;

    private Vector3[] dicePos; //굴린 후 정렬 위치
    public Vector3[] DicePos => dicePos;

    private Vector3[] fixedPos; //주사위 고정시 이동 위치
    public Vector3[] FixedPos => fixedPos;

    Vector3[] rotationVectors; //굴린 후 정렬시 적용할 회전값
    Vector3[] defaultPos; //주사위 굴리는 기본 위치 화면 오른쪽
    public DiceRankingEnum DiceRank;
    public InputSystem_Actions _inputActions;
    public IEnumerator diceRollCoroutine;

    void Start()
    {
        diceResult = new int[5];
        diceResultCount = new int[6];
        defaultDiceResultCount = new int[6] { 0, 0, 0, 0, 0, 0 };

        dices = new GameObject[diceCount];
        fakeDices = new GameObject[diceCount];
        dicesDatas = new DiceMy[diceCount];
        fixedDiceList = new List<int>();
        tempFixedDiceList = new List<int>();

        fixedPos = new Vector3[] { new Vector3(-3, 5, 10.5f), new Vector3(-3, 5, 8.8f), new Vector3(-3, 5, 6.43f), new Vector3(-1.55f, 5, 10.5f), new Vector3(-1.55f, 5, 8.8f) };
        defaultPos = new Vector3[] { new Vector3(2.29f, 0, 2.07f), new Vector3(2.82f, 3.33f, 1.35f), new Vector3(4.57f, 0, 2.11f), new Vector3(6.05f, 2.96f, 1.35f), new Vector3(7.1f, -0.2f, 1.94f) };
    }

    public void DiceSettingForBattle()
    {
        for (int i = 0; i < diceContainer.transform.childCount; i++)
        {
            dices[i] = diceContainer.transform.GetChild(i).gameObject;
            dicesDatas[i] = dices[i].GetComponent<DiceMy>();
        }
        for (int i = 0; i < fakeDiceContainer.transform.childCount; i++)
        {
            fakeDices[i] = fakeDiceContainer.transform.GetChild(i).gameObject;
            fakeDices[i].SetActive(false);
        }
    }

    public void RollDice()
    {
        SettingForRoll();
        diceRollCoroutine = SortingAfterRoll();

        StopCoroutine(diceRollCoroutine);

        GetRandomDiceNum();
        roll.SetDiceOutcome(diceResult);
        roll.RollAll();

        StartCoroutine(diceRollCoroutine);
    }

    private void SettingForRoll()
    {
        signitureAmount = 0;
        isSkipped = false;

        ground.SetActive(true);
        DiceBoard.SetActive(true);
        for (int i = 0; i < fakeDices.Length; i++)
        {
            if (fixedDiceList.Contains<int>(i) || tempFixedDiceList.Contains<int>(i)) continue;
            fakeDices[i].SetActive(false);
        }
        diceCamera.cullingMask |= 1 << LayerMask.NameToLayer("Dice");

        foreach (GameObject diceGO in dices)
        {
            Dice dice = diceGO.GetComponent<Dice>();
            dice.Locomotion.isEnd = false;
        }

        foreach (int i in tempFixedDiceList)
        {
            fixedDiceList.Add(i);
        }
        tempFixedDiceList.Clear();

        isRolling = true;
    }

    private void GetRandomDiceNum()
    {
        diceResultCount = defaultDiceResultCount.ToArray();

        for (int i = 0; i < diceResult.Length; i++)
        {
            if (tempFixedDiceList.Contains<int>(i) || fixedDiceList.Contains<int>(i))
            {
                diceResultCount[diceResult[i] - 1]++;
                continue;
            }
            diceResult[i] = UnityEngine.Random.Range(1, maxDiceNum);
            diceResultCount[diceResult[i] - 1]++;
            
            if (dicesDatas[i].diceSO.C_No == diceResult[i])
            {
                signitureAmount++;
            }
        }
    }

    IEnumerator SortingAfterRoll()
    {
        rollCount++;
        List<Dice> diceList = new List<Dice>();
        int rollEndCount = 0;

        for (int i = 0; i < dices.Length; i++) //현재 굴러가는 주사위의 Dice 컴포넌트를 리스트로
        {
            if (fixedDiceList.Contains<int>(i)) continue;

            diceList.Add(dices[i].GetComponent<Dice>());
        }
        yield return null;

        while (true)
        {
            for (int i = 0; i < diceList.Count; i++) //모든 주사위가 멈췄는지 체크
            {
                if (diceList[i].Locomotion.isEnd)
                {
                    rollEndCount++;
                }
            }

            if (rollEndCount == diceList.Count)
            {
                BattleManager.Instance.GetCost(signitureAmount);
                isRolling = false;
                
                if(BattleManager.Instance.currentPlayerState == PlayerTurnState.Roll || BattleManager.Instance.currentPlayerState == PlayerTurnState.Enter)
                {
                    BattleManager.Instance.battlePlayerTurnState.ChangePlayerTurnState(PlayerTurnState.RollEnd);
                }

                Debug.Log($"남은 리롤 횟수 : {maxRollCount - rollCount}");
                if (isSkipped == false)
                {
                    SortingFakeDice();
                }
                else
                {
                    GoDefaultPositionDice();
                }
                break;
            }                        
            rollEndCount = 0;
            yield return null;
        }
    }

    /// <summary>
    /// 한 턴이 끝났을때 주사위 관련 데이터를 리셋합니다.
    /// </summary>
    public void ResetSetting()
    {
        int childCount = BattleManager.Instance.fixedDiceArea.transform.childCount;
        for (int i = 0;i < childCount; i++)
        {
            BattleManager.Instance.fixedDiceArea.transform.GetChild(i).gameObject.SetActive(false);
        }

        rollCount = 0;

        foreach (int index in fixedDiceList)
        {
            roll.diceAndOutcomeArray[index].dice = dices[index].GetComponent<Dice>();
        }

        foreach (int index in tempFixedDiceList)
        {
            roll.diceAndOutcomeArray[index].dice = dices[index].GetComponent<Dice>();
        }

        fixedDiceList.Clear();
        tempFixedDiceList.Clear();

        GoDefaultPositionDice();
        GoDefaultPositionFakeDice();

        for (int i = 0; i < fakeDices.Length; i++)
        {     
            fakeDices[i].SetActive(false);
        }
    }    

    /// <summary>
    /// 주사위를 굴리기전 대기 위치로 이동시킵니다.
    /// </summary>
    private void GoDefaultPositionDice()
    {
        for (int i = 0; i < dices.Length; i++)
        {            
            dices[i].transform.localPosition = defaultPos[i];
        }
    }

    /// <summary>
    /// 표시용 주사위를 표시하는 메서드입니다.
    /// </summary>
    public void SortingFakeDice()
    {
        GoDefaultPositionDice();
        for (int i = 0; i < fakeDices.Length; i++)
        {
            fakeDices[i].SetActive(true);
        }
        diceCamera.cullingMask = diceCamera.cullingMask & ~(1 << LayerMask.NameToLayer("Dice"));
        ResetRotation();
    }

    /// <summary>
    /// 표시용 주사위의 회전값을 조정합니다.
    /// </summary>
    private void ResetRotation()
    {
        int i = 0;
        Quaternion quaternion;
        foreach (GameObject dice in fakeDices)
        {
            int iNum = diceResult[i] - 1;
            quaternion = Quaternion.Euler(rotationVectors[iNum].x, rotationVectors[iNum].y, rotationVectors[iNum].z);
            dice.transform.rotation = quaternion;
            i++;
        }
    }

    /// <summary>
    /// 표시용 주사위의 위치를 조정합니다.
    /// </summary>
    private void GoDefaultPositionFakeDice()
    {
        for (int i = 0; i < fakeDices.Length; i++)
        {
            if (fixedDiceList.Contains<int>(i) || tempFixedDiceList.Contains<int>(i)) continue;
            fakeDices[i].transform.localPosition = dicePos[i];
        }
    }

    public void HideFakeDice()
    {
        for (int i = 0; i < fakeDices.Length; i++)
        {
            fakeDices[i].SetActive(false);
        }
    }

    public void LoadDiceData()
    {
        LoadDiceDataScript loadScript = new LoadDiceDataScript();

        loadScript.LoadDiceJson();

        dicePos = loadScript.GetPoses().ToArray();
        DiceBattle.damageWightTable = loadScript.GetWeighting().ToArray();
        rotationVectors = loadScript.GetVectorCodes().ToArray();
    }

    public void StopSimulation()
    {
        foreach (GameObject diceGO in dices)
        {
            Dice dice = diceGO.GetComponent<Dice>();
 
            dice.StopSimulation();
            StopCoroutine(SortingAfterRoll());            
            
            BattleManager.Instance.battlePlayerTurnState.ChangePlayerTurnState(PlayerTurnState.RollEnd);
        }
        BattleManager.Instance.GetCost(signitureAmount);
    }
}