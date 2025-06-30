using UnityEngine;
using System.Linq;

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

public class DiceBattle
{
    public float[] damageWightTable;

    public int GetSignitureAmount()
    {
        int iNum = 0;
        int i = 0;
        foreach (GameObject diceGO in DiceManager.Instance.dices)
        {
            DiceMy dice = diceGO.GetComponent<DiceMy>();
            if (diceGO.GetComponent<DiceMy>().diceSO.C_No == DiceManager.Instance.DiceResult[i])
            {

                iNum++;
            }
            i++;
        }
        return iNum;
    }

    public float GetDiceWeighting()
    {        
        Debug.Log(DiceRankingJudgement(DiceManager.Instance.DiceResultCount));
        return DamageWeighting(DiceRankingJudgement(DiceManager.Instance.DiceResultCount));
    }

    private float DamageWeighting(DiceRankingEnum diceRank) //족보별계수
    {
        return damageWightTable[(int)diceRank];
    }

    private DiceRankingEnum DiceRankingJudgement(int[] diceResultCount)
    {
        int count = 0;
        int maxCount = 0;
        bool isPair = false;
        bool isTriple = false;
        DiceRankingEnum diceRank = DiceRankingEnum.Top;

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
                    return diceRank;
                }
            }
            else if (diceResultCount[i] == 3)
            {
                count++;
                if (isPair == true)
                {
                    diceRank = DiceRankingEnum.FullHouse;
                    return diceRank;
                }
                isTriple = true;
                diceRank = DiceRankingEnum.Triple;
            }
            else if (diceResultCount[i] == 4)
            {
                count++;
                diceRank = DiceRankingEnum.Quadruple;
                return diceRank;
            }
            else if (diceResultCount[i] == 5)
            {
                count++;
                diceRank = DiceRankingEnum.Queen;
                return diceRank;
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
            return diceRank;
        }
        if (maxCount == 4)
        {
            diceRank = DiceRankingEnum.SmallStraight;
            return diceRank;
        }

        return diceRank;
    }
}
