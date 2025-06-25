using PredictedDice;
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
    [SerializeField] RollMultipleDiceSynced roll;
    [SerializeField] Camera diceCamera;
    GameObject[] dices;

    int[] diceResult;
    int[] diceResultCount;
    int[] defaultDiceResultCount;
    List<int> fixedDiceList;
    List<int> tempFixedDiceList;
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

        dices = new GameObject[diceContainer.transform.childCount];

        fixedDiceList = new List<int>();
        tempFixedDiceList = new List<int>();

        damageWighting = new float[7] { 1, 3, 6.5f, 10, 4, 6, 7.5f }; //추후 값을 받아올수 있도록 수정

        for (int i = 0; i < diceContainer.transform.childCount; i++)
        {
            dices[i] = diceContainer.transform.GetChild(i).gameObject;
        }
    }

    private void Update()
    {
        SelectDice();
    }

    public void RollDice()
    {
        GetRandomDiceNum();

        roll.SetDiceOutcome(diceResult);

        roll.RollAll();
                
        foreach(int i in tempFixedDiceList)
        {
            fixedDiceList.Add(i);
        }
        tempFixedDiceList.Clear();
    }

    private void GetRandomDiceNum()
    {
        diceResultCount = defaultDiceResultCount.ToArray();

        for (int i = 0; i < diceResult.Length; i++)
        {
            if (tempFixedDiceList == null && fixedDiceList == null)
            {

            }
            else if (tempFixedDiceList.Contains<int>(i) || fixedDiceList.Contains<int>(i))
            {
                diceResultCount[diceResult[i] - 1]++;
                continue;
            }
            diceResult[i] = UnityEngine.Random.Range(1, maxDiceNum);
            diceResultCount[diceResult[i] - 1]++;
        }

        rollCount++;
        if (rollCount == maxRollCount)
        {
            BattleManager.Instance.DiceRollButton.interactable = false;
        }
        Debug.Log($"남은 리롤 횟수 : {maxRollCount - rollCount}");
    }

    public void DiceFixed(DiceMy dice)
    {
        int index = dice.MyIndex;

        if (tempFixedDiceList == null || tempFixedDiceList.Contains<int>(index) == false)
        {
            tempFixedDiceList.Add(index);
            roll.diceAndOutcomeArray[index].dice = null;
        }
        else if (tempFixedDiceList.Contains<int>(index) == true)
        {
            tempFixedDiceList.Remove(index);
            roll.diceAndOutcomeArray[index].dice = dices[index].GetComponent<Dice>();
        }
    }

    private void SelectDice()
    {
        //전투상태에서만 작동하도록 추가 조건 달기
        if (rollCount == 0) return;
        DiceMy dice;
        if (Input.touchCount > 0)
        {
            RaycastHit hit;

            Ray ray = diceCamera.ScreenPointToRay(Input.touches[0].position);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.TryGetComponent<DiceMy>(out dice))
                {
                    dice.SetIndex();
                    if(fixedDiceList != null && fixedDiceList.Contains<int>(dice.MyIndex))
                    {
                        Debug.Log("이미 고정된 주사위입니다.");
                        return;
                    }
                    DiceFixed(dice);
                    Debug.Log("주사위 감지");
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            Ray ray = diceCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.TryGetComponent<DiceMy>(out dice))
                {
                    dice.SetIndex();
                    if (fixedDiceList != null && fixedDiceList.Contains<int>(dice.MyIndex))
                    {
                        Debug.Log("이미 고정된 주사위입니다.");
                        return;
                    }
                    DiceFixed(dice);
                    Debug.Log("주사위 감지" + dice.name + dice.MyIndex);
                }
            }
        }
    }

    private void DiceRankingJudgement(int[] diceResultCount)
    {
        int count = 0;
        int maxCount = 0;
        bool isPair = false;
        bool isTriple = false;
        diceRank = DiceRankingEnum.Top;

        for (int i = 0; i < diceResultCount.Length; i++)
        {
            if(diceResultCount[i] == 1)
            {
                count++;
            }
            else if (diceResultCount[i] == 2)
            {
                count++;
                isPair = true;

                if (isTriple == true)
                {
                    diceRank = DiceRankingEnum.FullHouse;
                    return;
                }
            }
            else if (diceResultCount[i] == 3)
            {
                count++;
                if (isPair == true)
                {
                    diceRank = DiceRankingEnum.FullHouse;
                    return;
                }
                isTriple = true;
                diceRank = DiceRankingEnum.Triple;
            }
            else if (diceResultCount[i] == 4)
            {
                count++;
                diceRank = DiceRankingEnum.Quadruple;
                return;
            }
            else if (diceResultCount[i] == 5)
            {
                count++;
                diceRank = DiceRankingEnum.Queen;
                return;
            }
            else
            {
                if (count > maxCount)
                {
                    maxCount = count;
                }
                count = 0;
            }
        }
        
        maxCount = maxCount == 0 ? count : maxCount;        

        if (maxCount == 5)
        {
            diceRank = DiceRankingEnum.LargeStaright;
            return;
        }
        if (maxCount == 4)
        {
            diceRank = DiceRankingEnum.SmallStraight;
            return;
        }
    }

    private float DamageWeighting()
    {
        return damageWighting[(int)diceRank];
    }


    public float GetDiceWeighting()
    {
        DiceRankingJudgement(diceResultCount);
        Debug.Log(diceRank);
        return DamageWeighting();
    }

    public int GetSignitureAmount()
    {
        int iNum = 0;
        foreach (GameObject diceGO in dices)
        {
            DiceMy dice = diceGO.GetComponent<DiceMy>();
            if (diceResult.Contains<int>(dice.diceSO.C_No))
            {
                iNum++;
            }
        }
        return iNum;
    }

    public void ResetSetting()
    {
        rollCount = 0;

        foreach(int index in fixedDiceList)
        {
            roll.diceAndOutcomeArray[index].dice = dices[index].GetComponent<Dice>();
        }

        fixedDiceList.Clear();
        tempFixedDiceList.Clear();

        foreach (GameObject dice in dices)
        {
            dice.transform.rotation = Quaternion.identity;
        }
    }
}
