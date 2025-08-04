using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static SoundManager;
using UnityEngine.UI;

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
        Debug.Log("WriteVec");
        posVec = vec;
    }

    public void GetInputPress(InputAction.CallbackContext context)
    {
        if (isInputActive == false) return;
        if (context.phase == InputActionPhase.Started)
        {
            Debug.Log("Press Start");

            BattleManager.Instance.BattleSpawner.SkipCharacterSpwan();
            DiceManager.Instance.DiceHolding.SkipRolling(posVec);
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            Debug.Log("Press Exit");

            DiceManager.Instance.DiceHolding.SelectDice(posVec);
        }
    }
    public void OnUIClick(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Vector2 pointerPos = Pointer.current != null ? Pointer.current.position.ReadValue() : Vector2.zero;
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = pointerPos
            };
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            foreach (var result in results)
            {
                if (result.gameObject.GetComponent<Button>() != null)
                {
                    SoundManager.Instance.PlaySFX(SoundType.UIClick, 0.5f);
                    break;
                }
            }
        }
    }
}
