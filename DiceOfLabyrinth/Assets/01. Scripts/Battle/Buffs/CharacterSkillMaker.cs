//확률
//코스트 소모
//시그니처(본인)
//시그니처(유일)
//시그니처(개수)
//시그니처(유무)
//주사위 눈금
//족보
//n번째 턴
//코스트 소모량

using System;

public class CharacterSkillMaker
{




    //public Func<bool> GetSkillCondition()
    //{
    //    Func<bool> condition;



    //    return condition;
    //}

    #region 조건 판별 메서드
    private bool Probability(float value)
    {
        int iNum = UnityEngine.Random.Range(0, 101);

        if (iNum <= value) return true;
        return false;
    }
    private bool CostSpendCount(float value)
    {
        if (BattleManager.Instance.CostSpendedInTurn >= (int)value) return true;
        return false;
    }
    private bool SignitureMy(float value, int myIndex)
    {
        if (DiceManager.Instance.SignitureIndex.Contains(myIndex)) return true;
        return false;
    }
    private bool SignitureOnlyOne(float value)
    {
        if(DiceManager.Instance.SignitureAmount == 0) return true;
        return false;
    }
    private bool SignitureCount(float value)
    {
        if(DiceManager.Instance.SignitureAmount >= (int)value) return true;
        return false;
    }
    private bool SignitureExist(float value)
    {
        if (DiceManager.Instance.SignitureAmount > 0) return true;
        return false;
    }
    private bool DiceNumSum(float value)
    {
        if(DiceManager.Instance.SumOfDiceNum >= (int)value) return true;
        return false;
    }
    private bool CorrectDiceRankCondition(DamageCondition condition)
    {
        if (DiceManager.Instance.DiceRank == condition.ConditionRank) return true;
        else { return false; }
    }
    #endregion
}
