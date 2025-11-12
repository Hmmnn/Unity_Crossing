using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class InputListener : MonoBehaviour
{
    [Header("Input Values")]
    public int forward;
    public int right;
    
    public Vector2 move;
    public bool attack;

#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
    {
        Vector2 inputValue = value.Get<Vector2>();
        move = inputValue;

        if (inputValue.x > 0)
            right = 1;
        else if (inputValue.x < 0)
            right = -1;

        if (inputValue.y > 0)
            forward = 1;
        else if (inputValue.y < 0) 
            forward = -1;
    }

    public void OnAttack(InputValue value)
    {
        attack = value.isPressed;
    }
#endif

    public void ResetMoveValues()
    {
        forward = 0;
        right = 0;
    }

    public void ResetAttackValue()
    {
        attack = false;
    }
}
