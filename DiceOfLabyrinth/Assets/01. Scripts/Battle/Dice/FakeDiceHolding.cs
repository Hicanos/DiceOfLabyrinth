using UnityEngine;
using System.Collections.Generic;

public class FakeDiceHolding : MonoBehaviour
{
    private const int diceMaxNum = 6;
    private const int diceCount = 5;

    [SerializeField] AbstractDiceArea[] fakeDiceAreas;
    [SerializeField] AbstractDiceArea[] fixedFakeDiceAreas;

    private int[] diceResult;
    private bool[] isFixedList;

    public void SpawnFakeDices(List<BattleCharacter> characters)
    {
        for (int i = 0; i < diceCount; i++)
        {
            fakeDiceAreas[i].SetDiceImages(characters);
            fixedFakeDiceAreas[i].SetDiceImages(characters);
        }
    }

    //public void ActiveFakeDiceResult(int[] diceResult)
    //{
    //    this.diceResult = diceResult;

    //    for (int i = 0; i < diceCount; i++)
    //    {
    //        fakeDiceAreas[i].CurrentActiveIndex = diceResult[i];
    //        fixedFakeDiceAreas[i].CurrentActiveIndex = diceResult[i];

    //        if (isFixedList[i] == true) continue;

    //        fakeDiceAreas[i].ActiveDice();
    //    }
    //}

    //public void DeactiveFakeDiceResult()
    //{
    //    for (int i = 0; i < diceCount; i++)
    //    {
    //        if (isFixedList[i] == true) continue;

    //        fakeDiceAreas[i].DeactiveDice();
    //    }
    //}    

    //public void OnClickFixDIce(int areaIndex)
    //{
    //    if (isFixedList[areaIndex] == true) return;

    //    DiceManager.Instance.DiceHolding.TestGetFixedDiceIndex(areaIndex);
    //    isFixedList[areaIndex] = true;

    //    fakeDiceAreas[areaIndex].DeactiveDice();
    //    fixedFakeDiceAreas[areaIndex].ActiveDice();
    //}

    //public void OnClickReleaseDice(int areaIndex)
    //{
    //    if (isFixedList[areaIndex] == false) return;

    //    DiceManager.Instance.DiceHolding.TestGetReleasedDiceIndex(areaIndex);
    //    isFixedList[areaIndex] = false;

    //    fixedFakeDiceAreas[areaIndex].DeactiveDice();
    //    fakeDiceAreas[areaIndex].ActiveDice();
    //}

    //public void OnClickFixAllDIce()
    //{
    //    DiceManager diceManager = DiceManager.Instance;
    //    for(int i = 0; i < diceCount;i++)
    //    {
    //        diceManager.DiceHolding.TestGetFixedDiceIndex(i);

    //        OnClickFixDIce(i);
    //    }
    //}

    //public void OnClickReleaseDice()
    //{
    //    DiceManager diceManager = DiceManager.Instance;
    //    for (int i = 0; i < diceCount; i++)
    //    {
    //        diceManager.DiceHolding.TestGetReleasedDiceIndex(i);

    //        OnClickReleaseDice(i);
    //    }
    //}
}
