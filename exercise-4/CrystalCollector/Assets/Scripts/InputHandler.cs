using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "PlayerInput")]
public class InputHandler : ScriptableObject, Controls.IDefaultActions
{
    public Vector2 MousePosition { get; private set; }
    
    public Action<float> MoveInputChanged;
    public Action<float> TurnInputChanged;
    public Action JumpPressed;
    public Action HookShotPressed;
    public Action HookShotReleased;
    
    private Controls _controls;
    
    private void OnEnable()
    {
        if (_controls == null)
        {
            _controls = new Controls();
            _controls.Default.SetCallbacks(this);
        }
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        
        MoveInputChanged?.Invoke(context.ReadValue<float>());
    }

    public void OnTurn(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        
        TurnInputChanged?.Invoke(context.ReadValue<float>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        
        JumpPressed?.Invoke();
    }

    public void OnHookShotPress(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (context.ReadValueAsButton())
            HookShotPressed?.Invoke();
        else
            HookShotReleased?.Invoke();
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        MousePosition = context.ReadValue<Vector2>();
    }
}
