using UnityEngine;

public class RobotHookShot : MonoBehaviour
{
    [SerializeField] private new HookShotRenderer renderer;
    [SerializeField] private InputHandler input;
    [SerializeField] private SpringJoint _robotSpringJoint;

    private void OnEnable()
    {
        input.HookShotPressed += StartHookShot;
        input.HookShotReleased += EndHookShot;
    }

    private void OnDisable()
    {
        input.HookShotPressed -= StartHookShot;
        input.HookShotReleased -= EndHookShot;
    }
    
    private void StartHookShot()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // QueryTriggerInteraction.Ignore => Ignore colliders that have 'IsTrigger' enabled => otherwise the raycast
        // hits the sphere collider of the planets for the gravity
        if (Physics.Raycast(mouseRay, out hit, Mathf.Infinity, LayerMask.GetMask("Planets"), QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Planets"))
            {
                renderer.DrawLineTo(hit.point);
                
                _robotSpringJoint.spring = 15f;
                _robotSpringJoint.damper = 8f;
                _robotSpringJoint.connectedAnchor = hit.point;
            }
        }
    }

    private void EndHookShot()
    {
        renderer.Hide();

        _robotSpringJoint.spring = 0f;
        _robotSpringJoint.damper = 0f;
        _robotSpringJoint.connectedAnchor = new Vector3(0f, 0f, 0f);
    }
}