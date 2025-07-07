using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PredictedDice;
using PredictedDice.Demo;

public class DiceHolding : MonoBehaviour
{
    [SerializeField] Camera diceCamera;
    [SerializeField] RollMultipleDiceSynced rollMultipleDice;
    [SerializeField] Button DiceRollButton;
    List<int> fixedDiceList;
    List<int> tempFixedDiceList;

    GameObject[] areas = new GameObject[5];
    IEnumerator enumerator;
    int index2 = 0;
    private void Start()
    {
        DiceRollButton = BattleManager.Instance.BattleButtons[(int)PlayerTurnState.Roll].GetComponent<Button>();

        for(int i = 0; i < areas.Length; i++)
        {
            areas[i] = BattleManager.Instance.fixedDiceArea.transform.GetChild(i).gameObject;
        }
    }

    public void OnInput(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        Vector2 screenPos = context.ReadValue<Vector2>();

        SelectDice(screenPos);
        SkipRolling(screenPos);
    }

    private void SelectDice(Vector2 vec)
    {
        if (BattleManager.Instance.isBattle == false) return;

        DiceMy dice;

        Ray ray = diceCamera.ScreenPointToRay(vec);

        if (Physics.Raycast(ray, out var hit, 100f))
        {
            if (hit.collider.TryGetComponent(out dice))
            {
                fixedDiceList = DiceManager.Instance.FixedDiceList;
                dice = hit.collider.gameObject.GetComponent<DiceMy>();
                dice.SetIndex();
                if (fixedDiceList != null && fixedDiceList.Contains<int>(dice.MyIndex))
                {
                    return;
                }
                DiceFixed(dice);
            }
        }

        //if (Input.touchCount > 0)
        //{
        //    RaycastHit hit;

        //    Ray ray = diceCamera.ScreenPointToRay(Input.touches[0].position);

        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        if (hit.collider.gameObject.TryGetComponent<DiceMy>(out dice))
        //        {
        //            fixedDiceList = DiceManager.Instance.FixedDiceList;
        //            dice.SetIndex();
        //            if (fixedDiceList != null && fixedDiceList.Contains<int>(dice.MyIndex))
        //            {
        //                Debug.Log("이미 고정된 주사위입니다.");
        //                return;
        //            }
        //            DiceFixed(dice);
        //            Debug.Log("주사위 감지");
        //        }
        //    }
        //}

        //if (Input.GetMouseButtonDown(0))
        //{
        //    RaycastHit hit;

        //    Ray ray = diceCamera.ScreenPointToRay(Input.mousePosition);

        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        if (hit.collider.gameObject.tag == "FakeDice")
        //        {
        //            fixedDiceList = DiceManager.Instance.FixedDiceList;
        //            dice = hit.collider.gameObject.GetComponent<DiceMy>();
        //            dice.SetIndex();
        //            if (fixedDiceList != null && fixedDiceList.Contains<int>(dice.MyIndex))
        //            {
        //                return;
        //            }
        //            DiceFixed(dice);                    
        //        }
        //    }
        //}
    }

    private void DiceFixed(DiceMy dice)
    {
        if (fixedDiceList == null && tempFixedDiceList == null)
        {
            Debug.Log("리셋");
            index2 = 0;
        }

        int index = dice.MyIndex;
        tempFixedDiceList = DiceManager.Instance.TempFixedDiceList;
        List<Vector3> fixedPos = new List<Vector3>();
        bool isAdd;

        if (tempFixedDiceList == null || tempFixedDiceList.Contains<int>(index) == false)
        {
            isAdd = true;

            tempFixedDiceList.Add(index);
            rollMultipleDice.diceAndOutcomeArray[index].dice = null;

            enumerator = WaitForPosition(isAdd);
            StartCoroutine(enumerator);
        }
        else if (tempFixedDiceList.Contains<int>(index) == true)
        {
            isAdd = false;

            tempFixedDiceList.Remove(index);
            rollMultipleDice.diceAndOutcomeArray[index].dice = DiceManager.Instance.dices[index].GetComponent<Dice>();
            DiceManager.Instance.fakeDices[index].transform.localPosition = DiceManager.Instance.DicePos[index];

            enumerator = WaitForPosition(isAdd);
            StartCoroutine(enumerator);
        }
    }

    IEnumerator WaitForPosition(bool isAdd)
    {
        int count = 0;
        int index2 = this.index2;

        List<RectTransform> targetRects = new List<RectTransform>();
        List<Vector3> results = new List<Vector3>();
        Vector3 result;
        Canvas canvas = BattleManager.Instance.battleCanvas;


        if (isAdd)
        {
            if (index2 < 5)
            {
                areas[index2].SetActive(true);
                index2++;
            }
        }
        else
        {
            areas[index2-1].SetActive(false);
            index2--;
        }
        Debug.Log(index2);
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < index2; i++)
        {
            targetRects.Add(areas[i].GetComponent<RectTransform>());
        }

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < index2; i++)
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, targetRects[i].position);
            RectTransformUtility.ScreenPointToWorldPointInRectangle(targetRects[i], screenPos, canvas.worldCamera, out result);
            result.y = 10;
            results.Add(result);
        }

        foreach (int i in fixedDiceList)
        {
            DiceManager.Instance.fakeDices[i].transform.position = results[count];
            count++;
        }
        foreach (int i in tempFixedDiceList)
        {
            DiceManager.Instance.fakeDices[i].transform.position = results[count];
            count++;
        }

        if (fixedDiceList.Count + tempFixedDiceList.Count == DiceManager.Instance.dices.Length)
        {
            DiceRollButton.interactable = false;
        }
        else
        {
            DiceRollButton.interactable = true;
        }
        this.index2 = index2;
    }

    private void SkipRolling(Vector2 vec)
    {
        if (BattleManager.Instance.isBattle == false || DiceManager.Instance.isRolling == false) return;

        Ray ray = diceCamera.ScreenPointToRay(vec);

        if (Physics.Raycast(ray, out var hit, 100f))
        {
            if (hit.collider.gameObject.tag == "DiceBoard")
            {
                DiceManager.Instance.StopSimulation();
                DiceManager.Instance.isSkipped = true;
                DiceManager.Instance.SortingFakeDice();
            }
        }
    }
}
