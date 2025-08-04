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
    public GameObject[] Dices;
    public GameObject[] FakeDices;

    public Camera DiceCamera;
    public DiceBattle DiceBattle = new DiceBattle();
    DiceMy[] dicesDatas;

    public RollMultipleDiceSynced RollDiceSynced;

    //public GameObject Ground;
    public GameObject DiceBoard;

    private int signitureAmount;
    private List<int> signitureIndex = new List<int>();
    private int sumOfDiceNum;

    public int SignitureAmount => signitureAmount;
    public List<int> SignitureIndex => signitureIndex;
    public int SumOfDiceNum => sumOfDiceNum;

    public IEnumerator DiceRollCoroutine;

    private int[] diceResult;
    private int[] diceResultCount;
    private int[] defaultDiceResultCount;
    private List<int> fixedDiceList;

    public int[] DiceResult => diceResult;
    public int[] DiceResultCount => diceResultCount;
    public List<int> FixedDiceList => fixedDiceList;

    const int maxDiceNum = 6;
    const int diceCount = 5;

    private int[,] faceProbability = new int[5, 6];
    private int[] signitureArr = new int[5];

    private int rollCount = 0;
    private readonly int maxRollCount = 3;
    public int RollRemain => maxRollCount + (int)BattleManager.Instance.EngravingAdditionalStatus.AdditionalRoll + (int)BattleManager.Instance.ArtifactAdditionalStatus.AdditionalRoll - rollCount;

    //public bool isSkipped = false;
    public bool IsRolling = false;

    private Vector3[] rolldiceDefaultPosition;
    public Vector3[] RolldiceDefaultPosition => rolldiceDefaultPosition;

    private Vector3[] dicePos; //굴린 후 정렬 위치
    public Vector3[] DicePos => dicePos;

    Vector3[] rotationVectors; //굴린 후 정렬시 적용할 회전값

    public DiceRankingEnum DiceRankBefore;
    public DiceRankingEnum DiceRank;

    void Start()
    {
        diceResult = new int[5];
        diceResultCount = new int[6];
        defaultDiceResultCount = new int[6] { 0, 0, 0, 0, 0, 0 };

        Dices = new GameObject[diceCount];
        FakeDices = new GameObject[diceCount];
        dicesDatas = new DiceMy[diceCount];
        fixedDiceList = new List<int>();
    }

    public void DiceSettingForBattle()
    {
        for (int i = 0; i < diceCount; i++)
        {
            CharDiceData diceData = BattleManager.Instance.PartyData.Characters[i].character.CharacterData.charDiceData;

            Dices[i] = diceContainer.transform.GetChild(i).gameObject;
            dicesDatas[i] = Dices[i].GetComponent<DiceMy>();
            signitureArr[i] = diceData.CignatureNo;
            
            faceProbability[i, 0] = diceData.FaceProbability1;
            faceProbability[i, 1] = faceProbability[i, 0] + diceData.FaceProbability2;
            faceProbability[i, 2] = faceProbability[i, 1] + diceData.FaceProbability3;
            faceProbability[i, 3] = faceProbability[i, 2] + diceData.FaceProbability4;
            faceProbability[i, 4] = faceProbability[i, 3] + diceData.FaceProbability5;
            faceProbability[i, 5] = faceProbability[i, 4] + diceData.FaceProbability6;

            FakeDices[i] = fakeDiceContainer.transform.GetChild(i).gameObject;
            FakeDices[i].SetActive(false);
        }
        GoDefaultPositionDice();
        GoDefaultPositionFakeDice();
    }

    public void RollDice()
    {        
        SettingForRoll();
        DiceRollCoroutine = SortingAfterRoll();

        StopCoroutine(DiceRollCoroutine);

        GetRandomDiceNum();
        RollDiceSynced.SetDiceOutcome(diceResult);
        RollDiceSynced.RollAll();

        StartCoroutine(DiceRollCoroutine);
    }

    private void SettingForRoll()
    {
        signitureAmount = 0;
        signitureIndex.Clear();
        sumOfDiceNum = 0;
        DiceHolding.isCantFix = true;

        //Ground.SetActive(true);
        DiceBoard.SetActive(true);
        for (int i = 0; i < FakeDices.Length; i++)
        {
            if (fixedDiceList.Contains<int>(i)) continue;
            FakeDices[i].SetActive(false);
        }
        DiceCamera.cullingMask |= 1 << LayerMask.NameToLayer("Dice");

        foreach (GameObject diceGO in Dices)
        {
            Dice dice = diceGO.GetComponent<Dice>();
            dice.Locomotion.isEnd = false;
        }

        IsRolling = true;
    }

    private void GetRandomDiceNum()
    {
        diceResultCount = defaultDiceResultCount.ToArray();
        int randNum;
        int resultNum;

        for (int i = 0; i < diceResult.Length; i++)
        {
            sumOfDiceNum += diceResult[i];

            if (fixedDiceList.Contains<int>(i))
            {
                diceResultCount[diceResult[i] - 1]++;
                continue;
            }

            randNum = Random.Range(1, 101);

            if (randNum <= faceProbability[i, 0]) resultNum = 1;
            else if (randNum <= faceProbability[i, 1]) resultNum = 2;
            else if (randNum <= faceProbability[i, 2]) resultNum = 3;
            else if (randNum <= faceProbability[i, 3]) resultNum = 4;
            else if (randNum <= faceProbability[i, 4]) resultNum = 5;
            else resultNum = 6;

            diceResult[i] = resultNum;
            diceResultCount[diceResult[i] - 1]++;

            if (signitureArr[i] == diceResult[i])
            {
                signitureAmount++;
                signitureIndex.Add(i);
            }
        }
    }

    IEnumerator SortingAfterRoll()
    {
        rollCount++;
        List<Dice> diceList = new List<Dice>();
        int rollEndCount = 0;

        for (int i = 0; i < diceCount; i++) //현재 굴러가는 주사위의 Dice 컴포넌트를 리스트로
        {
            if (fixedDiceList.Contains<int>(i)) continue;

            diceList.Add(Dices[i].GetComponent<Dice>());
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
                IsRolling = false;


                BattleManager.Instance.BattlePlayerTurnState.ChangeDetailedTurnState(DetailedTurnState.RollEnd);
                SortingFakeDice();

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
        StopCoroutine(DiceRollCoroutine);

        int childCount = DiceHolding.areas.Length;
        for (int i = 0; i < childCount; i++)
        {
            DiceHolding.areas[i].gameObject.SetActive(false);
        }

        rollCount = 0;

        foreach (int index in fixedDiceList)
        {
            RollDiceSynced.diceAndOutcomeArray[index].dice = Dices[index].GetComponent<Dice>();
        }

        fixedDiceList.Clear();

        GoDefaultPositionDice();
        GoDefaultPositionFakeDice();

        for (int i = 0; i < diceCount; i++)
        {
            FakeDices[i].SetActive(false);
        }
    }

    /// <summary>
    /// 주사위를 굴리기전 대기 위치로 이동시킵니다.
    /// </summary>
    private void GoDefaultPositionDice()
    {
        for (int i = 0; i < Dices.Length; i++)
        {
            Dices[i].transform.localPosition = rolldiceDefaultPosition[i];
        }
    }

    /// <summary>
    /// 표시용 주사위를 표시하는 메서드입니다.
    /// </summary>
    public void SortingFakeDice()
    {
        DiceHolding.isCantFix = false;
        GoDefaultPositionDice();
        for (int i = 0; i < FakeDices.Length; i++)
        {
            FakeDices[i].SetActive(true);
        }
        DiceCamera.cullingMask = DiceCamera.cullingMask & ~(1 << LayerMask.NameToLayer("Dice"));
        ResetRotation();
    }

    /// <summary>
    /// 표시용 주사위의 회전값을 조정합니다.
    /// </summary>
    private void ResetRotation()
    {
        int i = 0;
        Quaternion quaternion;
        foreach (GameObject dice in FakeDices)
        {
            int iNum = diceResult[i] - 1;
         
            quaternion = Quaternion.Euler(rotationVectors[iNum].x, rotationVectors[iNum].y + 90, rotationVectors[iNum].z);
            dice.transform.rotation = quaternion;
            i++;
        }
    }

    /// <summary>
    /// 표시용 주사위의 위치를 조정합니다.
    /// </summary>
    private void GoDefaultPositionFakeDice()
    {
        for (int i = 0; i < FakeDices.Length; i++)
        {
            if (fixedDiceList.Contains<int>(i)) continue;
            FakeDices[i].transform.localPosition = dicePos[i];
        }
    }

    public void HideFakeDice()
    {
        for (int i = 0; i < FakeDices.Length; i++)
        {
            FakeDices[i].SetActive(false);
        }
    }

    public void LoadDiceData()
    {
        LoadDiceDataScript loadScript = new LoadDiceDataScript();

        loadScript.LoadDiceJson();

        dicePos = loadScript.GetPoses().ToArray();
        DiceBattle.DamageWeightTable = loadScript.GetWeighting().ToArray();
        rotationVectors = loadScript.GetVectorCodes().ToArray();
        rolldiceDefaultPosition = loadScript.GetDiceDefaultPosition();
    }

    public void StopSimulation()
    {
        foreach (GameObject diceGO in Dices)
        {
            Dice dice = diceGO.GetComponent<Dice>();

            dice.StopSimulation();
            StopCoroutine(SortingAfterRoll());            
        }
        BattleManager.Instance.BattlePlayerTurnState.ChangeDetailedTurnState(DetailedTurnState.RollEnd);
        BattleManager.Instance.GetCost(signitureAmount);
    }
}