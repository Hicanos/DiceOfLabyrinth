using PredictedDice.Demo;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

enum DiceRankingEnum
{
    Top,
    Triple,
    Quadruple,
    Queen,
    SmallStraight,
    LargeStaright,
    FullHouse
}
public class DiceManager : MonoBehaviour
{
    #region 싱글톤 구현
    private static DiceManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
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
    GameObject[] dices;
    [SerializeField] RollMultipleDiceSynced roll;
    int[] diceResult;
    int[] diceResultCount;
    int[] defaultDiceResultCount;
    List<int> fixedDiceList;
    const int maxDiceNum = 6;

    int rollCount = 0;
    const int maxRollCount = 3;

    DiceRankingEnum diceRank;
    float[] damageWighting;

    void Start()
    {
        diceResult = new int[5];
        diceResultCount = new int[6];
        defaultDiceResultCount = new int[6] { 0, 0, 0, 0, 0, 0 };
        damageWighting = new float[7] { 1, 3, 6.5f, 10, 4, 6, 7.5f };
        dices = new GameObject[diceContainer.transform.childCount];
        fixedDiceList = new List<int>();
        for (int i = 0; i < diceContainer.transform.childCount; i++)
        {
            dices[i] = diceContainer.transform.GetChild(i).gameObject;
        }
    }

    private void Update()
    {
        SelectDice();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetRandomDiceNum(fixedDiceList);
        }
    }

    public void GetRandomDiceNum(List<int> fixedDiceList)
    {
        if (rollCount == maxRollCount) return;

        diceResultCount = defaultDiceResultCount.ToArray();

        for (int i = 0; i < diceResult.Length; i++)
        {
            if (fixedDiceList == null)
            {

            }
            else if (fixedDiceList.Contains<int>(i))
            {
                diceResultCount[diceResult[i] - 1]++;
                continue;
            }
            diceResult[i] = Random.Range(1, maxDiceNum);
            diceResultCount[diceResult[i] - 1]++;
        }
        DiceRankingJudgement(diceResultCount);
        //Debug.Log($"{diceResult[0]}, {diceResult[1]}, {diceResult[2]}, {diceResult[3]}, {diceResult[4]}");
        //Debug.Log($"{diceResultCount[0]}, {diceResultCount[1]}, {diceResultCount[2]}, {diceResultCount[3]}, {diceResultCount[4]}, {diceResultCount[5]}");
        rollCount++;
        Debug.Log($"남은 리롤 횟수 : {maxRollCount -  rollCount}");
        roll.SetWhiteDiceOutcome(diceResult[0]);
        roll.SetBlueDiceOutcome(diceResult[1]);
        roll.SetRedDiceOutcome(diceResult[2]);
        roll.SetGreenDiceOutcome(diceResult[3]);
        roll.SetPurpleDiceOutcome(diceResult[4]);

        roll.RollAll();
        Debug.Log(diceRank);
    }

    public void DiceFixed(int index)
    {
        if (fixedDiceList == null || fixedDiceList.Contains<int>(index) == false)
        {
            fixedDiceList.Add(index);
        }
        else if (fixedDiceList.Contains<int>(index) == true)
        {
            fixedDiceList.Remove(index);
        }
    }

    private void SelectDice()
    {
        //전투상태에서만 작동하도록 추가 조건 달기
        if (Input.touchCount > 0)
        {
            Camera camera = Camera.main;
            DiceMy dice;
            RaycastHit hit;

            Ray ray = camera.ScreenPointToRay(Input.touches[0].position);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.TryGetComponent<DiceMy>(out dice))
                {
                    DiceFixed(dice.MyIndex);
                    Debug.Log("주사위 감지");
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Camera camera = Camera.main;
            DiceMy dice;
            RaycastHit hit;

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.TryGetComponent<DiceMy>(out dice))
                {
                    DiceFixed(dice.MyIndex);
                    Debug.Log("주사위 감지");
                }
            }
        }
    }

    private void DiceRankingJudgement(int[] diceResultCount)
    {
        bool isSS = true;
        int SSCount = 0;
        bool isLS = true;
        int LSCount = 0;
        bool isPair = false;
        bool isTriple = false;
        diceRank = DiceRankingEnum.Top;

        for (int i = 0; i < diceResultCount.Length; i++)
        {
            if (diceResultCount[i] == 1)
            {
                LSCount++;
                SSCount++;
            }
            else if (diceResultCount[i] == 2)
            {
                if (isPair == true)
                {
                    isSS = false;
                    SSCount = 0;
                }
                isPair = true;
                SSCount++;
                if (isTriple == true)
                {
                    diceRank = DiceRankingEnum.FullHouse;
                    return;
                }
            }
            else if (diceResultCount[i] == 3)
            {
                if (isPair == true)
                {
                    diceRank = DiceRankingEnum.FullHouse;
                    return;
                }
                isSS = false;
                SSCount = 0;
                isTriple = true;
                diceRank = DiceRankingEnum.Triple;
            }
            else if (diceResultCount[i] == 4)
            {
                isSS = false;
                SSCount = 0;
                diceRank = DiceRankingEnum.Quadruple;
                return;
            }
            else if (diceResultCount[i] == 5)
            {
                isSS = false;
                SSCount = 0;
                diceRank = DiceRankingEnum.Queen;
                return;
            }
            else
            {
                if ((i == 2 || i == 3) == true)
                {
                    isSS = false;
                    SSCount = 0;
                }

                if ((i == 0 || i == diceResultCount.Length - 1) == false)
                {
                    isLS = false;
                }
            }
        }

        if (LSCount == 5 && isLS == true)
        {
            diceRank = DiceRankingEnum.LargeStaright;
            return;
        }
        if (SSCount == 4 && isSS == true)
        {
            diceRank = DiceRankingEnum.SmallStraight;
            return;
        }
    }

    private void DamageWeighting(DiceRankingEnum diceRanking)
    {
        float damage = damageWighting[(int)diceRanking];
    }
}
