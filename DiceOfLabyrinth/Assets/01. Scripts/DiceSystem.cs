using UnityEngine;
using System.Collections.Generic;

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
public class DiceSystem : MonoBehaviour
{
    [SerializeField] GameObject diceContainer;
    GameObject[] dices;

    int[] diceResult;
    int[] diceResultCount;
    List<int> diceList;
    const int maxDiceNum = 6;

    int rerollCount = 0;
    const int maxRerollCount = 2;

    DiceRankingEnum diceRank;
    float[] damageWighting;

    void Start()
    {
        diceResult = new int[6];
        diceResultCount = new int[6] { 0, 0, 0, 0, 0, 0 };
        diceList = new List<int>() { 0, 1, 2, 3, 4, 5 };
        damageWighting = new float[7] { 1, 3, 6.5f, 10, 4, 6, 7.5f };

        for (int i = 0; i < diceContainer.transform.childCount; i++)
        {
            dices[i] = diceContainer.transform.GetChild(i).gameObject;
        }
    }

    void Update()
    {

    }

    public void RollDice()
    {
        for (int i = 0; i < diceResult.Length; i++)
        {
            diceResult[i] = Random.Range(1, maxDiceNum);
            diceResultCount[diceResult[i] - 1]++;
        }
        DiceRankingJudgement(diceResultCount);
    }

    private void DiceRankingJudgement(int[] diceResultCount)
    {
        bool isSS = true;
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
            }
            else if (diceResultCount[i] == 2)
            {
                if (isPair == true)
                {
                    isSS = false;
                }
                isPair = true;

                if (isTriple)
                {
                    isSS = false;
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

                isTriple = true;
                diceRank = DiceRankingEnum.Triple;
            }
            else if (diceResultCount[i] == 4)
            {
                isSS = false;

                diceRank = DiceRankingEnum.Quadruple;
                return;
            }
            else if (diceResultCount[i] == 5)
            {
                isSS = false;

                diceRank = DiceRankingEnum.Queen;
                return;
            }
            else
            {
                isSS = false;
                if ((i == 0 || i == diceResultCount.Length - 1) == false)
                {
                    isLS = false;
                }
            }
        }

        if (isSS)
        {
            diceRank = DiceRankingEnum.SmallStraight;
            return;
        }
        if (LSCount == 5 && isLS == true)
        {
            diceRank = DiceRankingEnum.LargeStaright;
            return;
        }
    }

    private void DamageWeighting(DiceRankingEnum diceRanking)
    {
        float damage = damageWighting[(int)diceRanking];
    }
}
