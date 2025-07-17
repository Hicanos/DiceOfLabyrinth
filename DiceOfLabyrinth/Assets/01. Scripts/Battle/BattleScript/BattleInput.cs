using UnityEngine;
using UnityEngine.InputSystem;

public class BattleInput : MonoBehaviour
{
    bool isInputActive = false;
    Vector2 posVec;

    public void InputStart()
    {
        isInputActive = true;
    }

    public void InputEnd()
    {
        isInputActive = false;
    }

    public void GetInput(InputAction.CallbackContext context)
    {
        if (isInputActive == false) return;
        Debug.Log("인풋");
                  
        WriteVec(context.ReadValue<Vector2>());
    }

    public void GetButton(InputAction.CallbackContext context)
    {        
        if(context.phase == InputActionPhase.Started)
        {
            Debug.Log("start");

            BattleManager.Instance.BattleSpawner.SkipCharacterSpwan();
            DiceManager.Instance.DiceHolding.SkipRolling(posVec);
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            Debug.Log("Exit");
            
            DiceManager.Instance.DiceHolding.SelectDice(posVec);
        }
    }

    public void WriteVec(Vector2 vec)
    {
        posVec = vec;
    }

    public void DebugBattleDefeat(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        //Debug.Log("디버그");

        DiceManager.Instance.DiceRollCoroutine = DiceManager.Instance.SortingAfterRoll();
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
