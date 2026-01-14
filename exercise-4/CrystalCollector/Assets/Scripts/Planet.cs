using UnityEngine;

public class Planet : MonoBehaviour {
    [SerializeField] private float gravityStrength = 9.81f;

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rigidbody;
        
        if (other.TryGetComponent<Rigidbody>(out rigidbody)) {
            Vector3 direction = (transform.position - other.transform.position).normalized;
            rigidbody.AddForce(direction * gravityStrength, ForceMode.Acceleration);
            
            RobotBody robotBody;
            
            if (other.TryGetComponent<RobotBody>(out robotBody)) {
                robotBody.CurrentGravityDirection = direction;
            }
        }
    }
}