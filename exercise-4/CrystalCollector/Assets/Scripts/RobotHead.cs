using UnityEngine;

public class RobotHead : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RobotBody body;
    [SerializeField] private InputHandler input;

    [Header("Settings")] 
    [SerializeField] private float turnSpeed;

    private float _turnInput;

    private void OnEnable()
    {
        input.TurnInputChanged += OnTurnInputChanged;
    }
    
    private void OnDisable()
    {
        input.TurnInputChanged -= OnTurnInputChanged;
    }

    private void OnTurnInputChanged(float newTurnInput) => _turnInput = newTurnInput;

    private void FixedUpdate()
    {
        var inputYRotation = Quaternion.Euler(0, _turnInput * turnSpeed * Time.fixedDeltaTime, 0);
        
        transform.position = body.transform.position;
        transform.rotation = Quaternion.FromToRotation(-transform.up, body.CurrentGravityDirection) * transform.rotation * inputYRotation;
    }
}