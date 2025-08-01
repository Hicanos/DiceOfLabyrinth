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

    public void SelectDice(Vector2 vec)
    {
        if(isCantFix) return;
        if (battleManager.IsBattle == false) return;
        DiceMy dice;

        Ray ray = diceCamera.ScreenPointToRay(vec);
        if (Physics.Raycast(ray, out var hit, 100f))
        {
            if (hit.collider.TryGetComponent(out dice))
            {
                dice = hit.collider.gameObject.GetComponent<DiceMy>();
                dice.SetIndex();
                //Debug.Log("실릭트");
                DiceFixed(dice);
            }
        }
    }

    //public void TestGetFixedDiceIndex(int index)
    //{
    //    fixedDiceList.Add(index);
    //}
    //public void TestGetReleasedDiceIndex(int index)
    //{
    //    fixedDiceList.Remove(index);
    //}

    private void DiceFixed(DiceMy dice)
    {
        isCantFix = true;

        int index = dice.MyIndex;
        List<Vector3> fixedPos = new List<Vector3>();

        if (fixedDiceList == null || fixedDiceList.Contains<int>(index) == false)
        {
            fixedDiceList.Add(index);
            rollMultipleDice.diceAndOutcomeArray[index].dice = null;

            DiceFix(index);
        }
        else if (fixedDiceList.Contains<int>(index) == true)
        {
            fixedDiceList.Remove(index);
            rollMultipleDice.diceAndOutcomeArray[index].dice = diceManager.Dices[index].GetComponent<Dice>();
            diceManager.FakeDices[index].transform.localPosition = diceManager.DicePos[index];

            if (diceManager.RollRemain != 0)
            {
                DiceRollButton.interactable = true;
            }
        }
        isCantFix = false;
    }
    public void DiceFix(int index)
    {
        Canvas canvas = GetBattleCanvas();
        RectTransform targetRect = areas[index].GetComponent<RectTransform>();
        if(DiceRollButton == null)
        {
            DiceRollButton = UIManager.Instance.BattleUI.Roll.GetComponent<Button>();
        }
        Vector3 result;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, targetRect.position);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(targetRect, screenPos, canvas.worldCamera, out result);
        result.y = fakeDicePositionY;

        if (fixedDiceList.Count == diceManager.Dices.Length)
        {
            DiceRollButton.interactable = false;
        }        

        diceManager.FakeDices[index].transform.position = result;
    }
    IEnumerator WaitForPosition(bool isAdd)
    {
        int count = 0;
        int index2 = this.index2;

        List<RectTransform> targetRects = new List<RectTransform>();
        List<Vector3> results = new List<Vector3>();
        Vector3 result;
        Canvas canvas = GetBattleCanvas();
        DiceRollButton = UIManager.Instance.BattleUI.Buttons[1].GetComponent<Button>(); //수정필요

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
            diceManager.FakeDices[i].transform.position = results[count];
            count++;
        }

        if (fixedDiceList.Count == diceManager.Dices.Length)
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

    public void SkipRolling(Vector2 vec)
    {
        if (battleManager.IsBattle == false || diceManager.IsRolling == false) return;
        StopCoroutine(diceManager.DiceRollCoroutine);        
        Ray ray = diceCamera.ScreenPointToRay(vec);

        if (Physics.Raycast(ray, out var hit, 100f))
        {
            if (hit.collider.gameObject.tag == "DiceBoard")
            {
                diceManager.StopSimulation();
                diceManager.IsRolling = false;
                //Debug.Log("스킵 다이스");
                //diceManager.isSkipped = true;
                diceManager.SortingFakeDice();
            }
        }
    }

    public void FixAllDIce()
    {
        NewAllDiceFixed();
    }

    public void ReleaseDice()
    {
        NewReleaseDice();
    }

    private void NewAllDiceFixed()
    {
        Canvas canvas = GetBattleCanvas();
        RectTransform[] targetRects = new RectTransform[5];

        for (int i = 0; i < 5; i++)
        {
            targetRects[i] = areas[i].GetComponent<RectTransform>();
        }
      
        if (DiceRollButton == null)
        {
            DiceRollButton = UIManager.Instance.BattleUI.Roll.GetComponent<Button>();
        }

        Vector3 result;
        for (int i = 0; i < 5; i++)
        {
            if(fixedDiceList.Contains<int>(i)) continue;
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, targetRects[i].position);
            RectTransformUtility.ScreenPointToWorldPointInRectangle(targetRects[i], screenPos, canvas.worldCamera, out result);
            result.y = fakeDicePositionY;

            diceManager.FakeDices[i].transform.position = result;
            fixedDiceList.Add(i);
        }
        
        DiceRollButton.interactable = false;
    }

    private void NewReleaseDice()
    {
        for(int i = 0; i < 5; i++)
        {
            diceManager.FakeDices[i].transform.localPosition = diceManager.DicePos[i];
        }
        
        fixedDiceList.RemoveRange(0, fixedDiceList.Count);

        if (diceManager.RollRemain > 0)
        {
            DiceRollButton.interactable = true;
        }
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
            diceManager.FakeDices[index].transform.position = results[count2];
            count2++;
        }
        for (int i = 0; i < noFixed.Count; i++)
        {
            index = noFixed[i];
            diceManager.FakeDices[index].transform.position = results[count2];
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
                diceManager.FakeDices[i].transform.position = results[count];
                count++;
            }
            else
            {
                diceManager.FakeDices[i].transform.localPosition = diceManager.DicePos[i];
            }
        }
    }

    private Canvas GetBattleCanvas()
    {
        return UIManager.Instance.BattleUI.battleCanvas;
    }
}
