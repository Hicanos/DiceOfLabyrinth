using UnityEngine;
using UnityEngine.InputSystem;

public class BattleInput : MonoBehaviour
{
    public void GetInput(InputAction.CallbackContext context)
    {
        //if (!context.started) return;
        Debug.Log("인풋");
        BattleManager.Instance.battleSpawner.SkipCharacterSpwan();

        Vector2 screenPos = context.ReadValue<Vector2>();
        DiceManager.Instance.DiceHolding.DiceInput(screenPos);
    }

    public void DebugBattleDefeat(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        Debug.Log("디버그");

        DiceManager.Instance.diceRollCoroutine = DiceManager.Instance.SortingAfterRoll();
        BattleManager battleManager = BattleManager.Instance;
        BattleCharacter battleCharacter;

        for (int i = 0; i < 5; i++)
        {
            battleCharacter = battleManager.BattleGroup.BattleCharacters[i];

            battleCharacter.TakeDamage(1000);
            battleManager.UIValueChanger.ChangeCharacterHpRatio((HPEnumCharacter)i);
            if (battleCharacter.IsDied) battleManager.BattleGroup.CharacterDead(i);
        }
    }
}
