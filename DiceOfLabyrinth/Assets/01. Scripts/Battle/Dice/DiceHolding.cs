using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PredictedDice;
using PredictedDice.Demo;

public class DiceHolding : MonoBehaviour
{
    [SerializeField] Camera diceCamera;
    [SerializeField] RollMultipleDiceSynced rollMultipleDice;
    List<int> fixedDiceList;
    List<int> tempFixedDiceList;
    void Update()
    {
        SelectDice();
        SkipRolling();
    }

    private void SelectDice()
    {
        if (BattleManager.Instance.isBattle == false) return;

        DiceMy dice;
        if (Input.touchCount > 0)
        {
            RaycastHit hit;

            Ray ray = diceCamera.ScreenPointToRay(Input.touches[0].position);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.TryGetComponent<DiceMy>(out dice))
                {
                    fixedDiceList = DiceManager.Instance.FixedDiceList;
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
                if (hit.collider.gameObject.tag == "FakeDice")
                {
                    fixedDiceList = DiceManager.Instance.FixedDiceList;
                    dice = hit.collider.gameObject.GetComponent<DiceMy>();
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

    private void DiceFixed(DiceMy dice)
    {
        int index = dice.MyIndex;
        tempFixedDiceList = DiceManager.Instance.TempFixedDiceList;

        if (tempFixedDiceList == null || tempFixedDiceList.Contains<int>(index) == false)
        {
            tempFixedDiceList.Add(index);
            rollMultipleDice.diceAndOutcomeArray[index].dice = null;
            DiceManager.Instance.fakeDices[index].transform.localPosition = DiceManager.Instance.FixedPos[index];
            if (fixedDiceList.Count + tempFixedDiceList.Count == DiceManager.Instance.dices.Length)
            {
                BattleManager.Instance.DiceRollButton.interactable = false;
            }
        }
        else if (tempFixedDiceList.Contains<int>(index) == true)
        {
            tempFixedDiceList.Remove(index);
            rollMultipleDice.diceAndOutcomeArray[index].dice = DiceManager.Instance.dices[index].GetComponent<Dice>();
            DiceManager.Instance.fakeDices[index].transform.localPosition = DiceManager.Instance.DicePos[index];
            BattleManager.Instance.DiceRollButton.interactable = true;
        }
    }

    private void SkipRolling()
    {
        if (BattleManager.Instance.isBattle == false || DiceManager.Instance.isRolling == false) return;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            Ray ray = diceCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if(hit.collider.gameObject.tag == "DiceBoard")
                {
                    DiceManager.Instance.isSkipped = true;
                    DiceManager.Instance.SortingFakeDice();
                }
            }
        }
    }
}
