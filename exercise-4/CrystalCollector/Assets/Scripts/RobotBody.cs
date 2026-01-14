using System;
using UnityEngine;

public class RobotBody : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputHandler input;
    [SerializeField] private Transform headTransform;
    
    [Header("Settings")] // Adjust to your liking
    [SerializeField] private float rollSpeed;
    [SerializeField] private float jumpForce;

    public Vector3 CurrentGravityDirection {get; set;} // Update this every physics frame
        
    private float _moveInput; // this can be 1, -1 or 0
    private Rigidbody _rigidbody;
    private bool _jumpNextPhysicsFrame;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        input.MoveInputChanged += OnMoveInputChanged;
        input.JumpPressed += OnJumpInput;
    }
    
    private void OnDisable()
    {
        input.MoveInputChanged -= OnMoveInputChanged;
        input.JumpPressed -= OnJumpInput;
    }

    private void OnMoveInputChanged(float newInputValue) => _moveInput = newInputValue;
    private void OnJumpInput() => _jumpNextPhysicsFrame = true;
    
    private void FixedUpdate() {
        Vector3 rollAxis = headTransform.right; 
        _rigidbody.AddTorque(rollAxis * rollSpeed * _moveInput);
    
        if (_jumpNextPhysicsFrame)
        {
            _rigidbody.AddForce(-CurrentGravityDirection * jumpForce, ForceMode.Impulse);
            _jumpNextPhysicsFrame = false;
        }
    }
}