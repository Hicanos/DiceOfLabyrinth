using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    #region 싱글톤 구현
    private static InputManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static InputManager Instance
    {
        get
        {
            if( instance == null)
            {
                Debug.Log("InputManager Instance is Null");
                return null;
            }
            return instance;
        }
    }
    #endregion

    bool isInputActive;
    Vector2 posVec;

    public void BattleInputStart()
    {
        isInputActive = true;
    }

    public void BattleInputEnd()
    {
        isInputActive = false;
    }

    public void WriteInputVector(InputAction.CallbackContext context)
    {
        if (isInputActive == false) return;
        //Debug.Log("인풋");

        WriteVec(context.ReadValue<Vector2>());
    }

    private void WriteVec(Vector2 vec)
    {
        posVec = vec;
    }

    public void GetInputPress(InputAction.CallbackContext context)
    {
        if (isInputActive == false) return;
        if (context.phase == InputActionPhase.Started)
        {
            //Debug.Log("start");

            BattleManager.Instance.BattleSpawner.SkipCharacterSpwan();
            DiceManager.Instance.DiceHolding.SkipRolling(posVec);
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            //Debug.Log("Exit");

            DiceManager.Instance.DiceHolding.SelectDice(posVec);
        }
    }    
}
