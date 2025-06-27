using PredictedDice;
using PredictedDice.Demo;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
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
    [SerializeField] GameObject fakeDiceContainer;
    [SerializeField] RollMultipleDiceSynced roll;
    [SerializeField] Camera diceCamera;
    public GameObject[] dices;
    public GameObject[] fakeDices;
    public GameObject ground;

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

    Vector3[] dicePos; //굴린 후 정렬 위치
    Vector3[] fixedPos; //주사위 고정시 이동 위치
    Vector3[] rotationVectors; //굴린 후 정렬시 적용할 회전값 현재 적용X
    Vector3[] defaultPos; //주사위 굴리는 기본 위치 화면 아래쪽
    void Start()
    {
        diceResult = new int[5];
        diceResultCount = new int[6];
        defaultDiceResultCount = new int[6] { 0, 0, 0, 0, 0, 0 };

        dices = new GameObject[diceContainer.transform.childCount];
        fakeDices = new GameObject[fakeDiceContainer.transform.childCount];
        fixedDiceList = new List<int>();
        tempFixedDiceList = new List<int>();

        for (int i = 0; i < diceContainer.transform.childCount; i++)
        {
            dices[i] = diceContainer.transform.GetChild(i).gameObject;
        }
        for (int i = 0; i < fakeDiceContainer.transform.childCount; i++)
        {
            fakeDices[i] = fakeDiceContainer.transform.GetChild(i).gameObject;
            fakeDices[i].SetActive(false);
        }

        fixedPos = new Vector3[] { new Vector3(-3, 5, 10.5f), new Vector3(-3, 5, 8.8f), new Vector3(-3, 5, 6.43f), new Vector3(-1.55f, 5, 10.5f), new Vector3(-1.55f, 5, 8.8f) };
        defaultPos = new Vector3[] { new Vector3(1.09999847f, 0, 2.07000017f), new Vector3(2.81999993f, 3.32999992f, 1.35000002f), new Vector3(4.57000017f, 0, 2.1099999f), new Vector3(6.09000015f, 2.96000004f, 1.35000002f), new Vector3(7.76000023f, -0.200000003f, 1.94000006f) };
    }

    private void Update()
    {
        SelectDice();
    }

    public void RollDice()
    {
        SettingForRoll();

        StopCoroutine(SortingAfterRoll());

        GetRandomDiceNum();
        roll.SetDiceOutcome(diceResult);
        roll.RollAll();

        StartCoroutine(SortingAfterRoll());
    }

    private void SettingForRoll()
    {
        ground.SetActive(true);
        for (int i = 0; i < fakeDices.Length; i++)
        {
            dices[i].SetActive(true);
            if (fixedDiceList.Contains<int>(i) || tempFixedDiceList.Contains<int>(i)) continue;
            fakeDices[i].SetActive(false);
        }

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
        Debug.Log($"{diceResult[0]},{diceResult[1]},{diceResult[2]},{diceResult[3]},{diceResult[4]}");
    }

    IEnumerator SortingAfterRoll()
    {
        rollCount++;
        BattleManager.Instance.DiceRollButton.interactable = false;
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
                if (rollCount == maxRollCount)
                {
                    BattleManager.Instance.DiceRollButton.interactable = false;
                }
                else
                {
                    BattleManager.Instance.DiceRollButton.interactable = true;
                }
                Debug.Log($"남은 리롤 횟수 : {maxRollCount - rollCount}");
                SortingFakeDice();
                break;
            }
            rollEndCount = 0;
            yield return null;
        }
    }

    private void SortingFakeDice()
    {        
        GoDefaultPositionDice();
        for (int i = 0; i < fakeDices.Length; i++)
        {
            dices[i].SetActive(false);
            fakeDices[i].SetActive(true);
        }
        ResetRotation();
    }

    private void SelectDice()
    {
        //전투상태에서만 작동하도록 추가 조건 달기
        if (rollCount == maxRollCount) return;
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
                    if (fixedDiceList != null && fixedDiceList.Contains<int>(dice.MyIndex))
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

    public void DiceFixed(DiceMy dice)
    {
        int index = dice.MyIndex;

        if (tempFixedDiceList == null || tempFixedDiceList.Contains<int>(index) == false)
        {
            tempFixedDiceList.Add(index);
            roll.diceAndOutcomeArray[index].dice = null;
            fakeDices[index].transform.localPosition = fixedPos[index];
            if(fixedDiceList.Count + tempFixedDiceList.Count == dices.Length)
            {
                BattleManager.Instance.DiceRollButton.interactable = false;
            }
        }
        else if (tempFixedDiceList.Contains<int>(index) == true)
        {
            tempFixedDiceList.Remove(index);
            roll.diceAndOutcomeArray[index].dice = dices[index].GetComponent<Dice>();
            fakeDices[index].transform.localPosition = dicePos[index];
            BattleManager.Instance.DiceRollButton.interactable = true;
        }
    }

    public void ResetSetting() // 한 턴이 끝났을때 주사위 관련 데이터를 리셋
    {
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
            dices[i].SetActive(true);            
            fakeDices[i].SetActive(false);
        }
    }

    private void GoDefaultPositionDice()
    {
        for (int i = 0; i < dices.Length; i++)
        {
            if (fixedDiceList.Contains<int>(i) || tempFixedDiceList.Contains<int>(i)) continue;
            dices[i].transform.localPosition = defaultPos[i];
        }
    }

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

    private void ResetRotation() //표시용 주사위 회전값 조정
    {
        int i = 0;
        quaternion quaternion;
        foreach (GameObject dice in fakeDices)
        {
            int iNum = diceResult[i] - 1;
            quaternion = Quaternion.Euler(rotationVectors[iNum].x, rotationVectors[iNum].y, rotationVectors[iNum].z);            
            dice.transform.rotation = quaternion;
            i++;
        }
    }

    public void LoadDiceData()
    {
        LoadDiceDataScript loadScript = new LoadDiceDataScript();

        loadScript.LoadDiceJson();

        dicePos = loadScript.GetPoses().ToArray();
        damageWighting = loadScript.GetWeighting().ToArray();
        rotationVectors = loadScript.GetVectorCodes().ToArray();
    }

    #region 배틀에 전달할 값
    public float GetDiceWeighting()
    {
        DiceRankingJudgement();
        Debug.Log(diceRank);
        return DamageWeighting();
    }

    private float DamageWeighting() //주사위 눈금 *족보별계수
    {
        return diceResult.Sum() * damageWighting[(int)diceRank];
    }

    public int GetSignitureAmount()
    {
        int iNum = 0;
        int i = 0;
        foreach (GameObject diceGO in dices)
        {
            DiceMy dice = diceGO.GetComponent<DiceMy>();
            if (diceGO.GetComponent<DiceMy>().diceSO.C_No == diceResult[i])
            {

                iNum++;
            }
            i++;
        }
        return iNum;
    }

    private void DiceRankingJudgement()
    {
        int count = 0;
        int maxCount = 0;
        bool isPair = false;
        bool isTriple = false;
        diceRank = DiceRankingEnum.Top;

        for (int i = 0; i < diceResultCount.Length; i++)
        {
            if (diceResultCount[i] == 1)
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
    #endregion
}
