using UnityEngine;

public class BattleUIStagmaDisplayer : MonoBehaviour
{
    private void OnEnable()
    {
        DiceManager.Instance.DiceHolding.isCantFix = true;

        BattleManager.Instance.battleCanvas.planeDistance = 17.5f;
        //BattleManager.Instance.UIValueChanger();
    }

    public void OnClickClose()
    {
        BattleManager.Instance.battleCanvas.planeDistance = 100;
        gameObject.SetActive(false);

        DiceManager.Instance.DiceHolding.isCantFix = false;
    }
}
