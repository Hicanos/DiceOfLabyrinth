using UnityEngine;

public class BattleUIStagmaDisplayer : MonoBehaviour
{
    private void OnEnable()
    {
        DiceManager.Instance.DiceHolding.isCantFix = true;
    }

    public void OnClickClose()
    {
        gameObject.SetActive(false);

        DiceManager.Instance.DiceHolding.isCantFix = false;
    }
}
