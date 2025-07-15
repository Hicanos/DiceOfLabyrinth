using UnityEngine;
using UnityEngine.InputSystem;

public class BattleInput : MonoBehaviour
{
    public void GetInput(InputAction.CallbackContext context)
    {
        //if (!context.started) return;

        BattleManager.Instance.battleSpawner.SkipCharacterSpwan();

        Vector2 screenPos = context.ReadValue<Vector2>();
        DiceManager.Instance.DiceHolding.DiceInput(screenPos);
    }
}
