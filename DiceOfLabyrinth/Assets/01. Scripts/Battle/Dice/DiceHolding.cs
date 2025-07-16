using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PredictedDice;
using PredictedDice.Demo;

public class DiceHolding : MonoBehaviour
{
    DiceManager diceManager;
    BattleManager battleManager;

    [SerializeField] Camera diceCamera;
    [SerializeField] RollMultipleDiceSynced rollMultipleDice;
    [SerializeField] Button DiceRollButton;
    private List<int> fixedDiceList;

    public GameObject[] areas = new GameObject[5];
    private IEnumerator enumerator;

    private int index2 = 0;
    public  bool isCantFix = false;

    const int fakeDicePositionY = 10;
    const int diceCount = 5;

    private void Start()
    {
        battleManager = BattleManager.Instance;
        diceManager = DiceManager.Instance;
    }
    public void SettingForHolding()
    {
        UIManager.Instance.BattleUI.SettingForHolding();
    }

    public void GetFixedList()
    {
        fixedDiceList = DiceManager.Instance.FixedDiceList;
    }

    public void DiceInput(Vector2 screenPos)
    {
        SelectDice(screenPos);
        SkipRolling(screenPos);
    }

    private void SelectDice(Vector2 vec)
    {
        Debug.Log("1");
        if(isCantFix) return;
        Debug.Log("2");
        if (battleManager.isBattle == false) return;
        Debug.Log("3");
        DiceMy dice;

        Ray ray = diceCamera.ScreenPointToRay(vec);
        Debug.Log("4");
        if (Physics.Raycast(ray, out var hit, 100f))
        {
            Debug.Log("5");
            if (hit.collider.TryGetComponent(out dice))
            {
                Debug.Log("6");
                dice = hit.collider.gameObject.GetComponent<DiceMy>();
                dice.SetIndex();
                Debug.Log("실릭트");
                DiceFixed(dice);
            }
        }
    }

    private void DiceFixed(DiceMy dice)
    {
        isCantFix = true;
        int index = dice.MyIndex;
        List<Vector3> fixedPos = new List<Vector3>();
        bool isAdd;
        int fixedCount = fixedDiceList.Count;


        if (fixedCount == 0)
        {
            index2 = 0;
        }

        if (fixedDiceList == null || fixedDiceList.Contains<int>(index) == false)
        {
            isAdd = true;

            fixedDiceList.Add(index);
            rollMultipleDice.diceAndOutcomeArray[index].dice = null;

            enumerator = WaitForPosition(isAdd);
            StartCoroutine(enumerator);
        }
        else if (fixedDiceList.Contains<int>(index) == true)
        {
            isAdd = false;

            fixedDiceList.Remove(index);
            rollMultipleDice.diceAndOutcomeArray[index].dice = diceManager.dices[index].GetComponent<Dice>();
            diceManager.fakeDices[index].transform.localPosition = diceManager.DicePos[index];

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
        Canvas canvas = GetBattleCanvas();
        DiceRollButton = UIManager.Instance.BattleUI.Buttons[2].GetComponent<Button>(); //수정필요

        if (isAdd)
        {
            if (index2 < diceCount)
            {
                areas[index2].SetActive(true);
                index2++;
            }
        }
        else
        {
            areas[index2 - 1].SetActive(false);
            index2--;
        }

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
            result.y = fakeDicePositionY;
            results.Add(result);
        }

        foreach (int i in fixedDiceList)
        {
            diceManager.fakeDices[i].transform.position = results[count];
            count++;
        }

        if (fixedDiceList.Count == diceManager.dices.Length)
        {
            DiceRollButton.interactable = false;
        }
        else if (diceManager.RollRemain != 0)
        {
            DiceRollButton.interactable = true;
        }
        this.index2 = index2;
        isCantFix = false;
    }

    private void SkipRolling(Vector2 vec)
    {
        if (battleManager.isBattle == false || diceManager.isRolling == false) return;
        StopCoroutine(diceManager.diceRollCoroutine);        
        Ray ray = diceCamera.ScreenPointToRay(vec);

        if (Physics.Raycast(ray, out var hit, 100f))
        {
            if (hit.collider.gameObject.tag == "DiceBoard")
            {
                diceManager.StopSimulation();
                Debug.Log("스킵 다이스");
                //diceManager.isSkipped = true;
                diceManager.SortingFakeDice();
            }
        }
        diceManager.isRolling = false;
    }

    public void FixAllDIce()
    {
        StartCoroutine(AllDiceFixed());
    }

    public void ReleaseDice()
    {
        StartCoroutine(DiceRelease());
    }

    IEnumerator AllDiceFixed()
    {
        int count = 0;
        Vector3 result;
        Vector3[] results = new Vector3[diceCount];
        RectTransform targetRects;
        Canvas canvas = GetBattleCanvas();
        int fixedCount = fixedDiceList.Count;

        List<int> noFixed = new List<int>() { 0, 1, 2, 3, 4 };

        for (int i = 0; i < diceCount; i++)
        {
            if (fixedDiceList.Contains<int>(i))
            {
                count++;
                noFixed.Remove(i);
            }
        }

        for (int i = count; i < diceCount; i++)
        {
            areas[i].SetActive(true);
        }

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < diceCount; i++)
        {
            targetRects = areas[i].GetComponent<RectTransform>();
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, targetRects.position);
            RectTransformUtility.ScreenPointToWorldPointInRectangle(targetRects, screenPos, canvas.worldCamera, out result);
            result.y = 10;
            results[i] = result;
        }

        int count2 = 0;
        int index;

        for (int i = 0; i < fixedCount; i++)
        {
            index = fixedDiceList[i];
            diceManager.fakeDices[index].transform.position = results[count2];
            count2++;
        }
        for (int i = 0; i < noFixed.Count; i++)
        {
            index = noFixed[i];
            diceManager.fakeDices[index].transform.position = results[count2];
            count2++;
        }
    }

    IEnumerator DiceRelease()
    {
        Vector3 result;
        Vector3[] results;
        RectTransform targetRects;
        Canvas canvas = GetBattleCanvas();
        int fixedCount = fixedDiceList.Count;

        for (int i = 4; i > fixedCount - 1; i--)
        {
            areas[i].SetActive(false);
        }

        yield return new WaitForEndOfFrame();

        results = new Vector3[fixedCount];
        int count = 0;

        for (int i = 0; i < fixedCount; i++)
        {
            targetRects = areas[i].GetComponent<RectTransform>();
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, targetRects.position);
            RectTransformUtility.ScreenPointToWorldPointInRectangle(targetRects, screenPos, canvas.worldCamera, out result);
            result.y = 10;
            results[i] = result;
        }

        for (int i = 0; i < 5; i++)
        {
            if (fixedDiceList.Contains<int>(i))
            {
                diceManager.fakeDices[i].transform.position = results[count];
                count++;
            }
            else
            {
                diceManager.fakeDices[i].transform.localPosition = diceManager.DicePos[i];
            }
        }
    }

    private Canvas GetBattleCanvas()
    {
        return UIManager.Instance.BattleUI.battleCanvas;
    }
}
