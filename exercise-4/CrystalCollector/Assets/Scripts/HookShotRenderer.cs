using UnityEngine;

public class HookShotRenderer : MonoBehaviour
{
    [SerializeField] private GameObject connectionPointPrefab;
    
    private LineRenderer _lineRenderer;
    private GameObject _connectionPointObject;
    
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void DrawLineTo(Vector3 point)
    {
        gameObject.SetActive(true);
        _connectionPointObject = Instantiate(connectionPointPrefab, point, Quaternion.identity);
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, point);
        enabled = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        Destroy(_connectionPointObject);
        enabled = false;
    }

    private void Update()
    {
        transform.rotation = Quaternion.identity;
        _lineRenderer.SetPosition(0, transform.position);
    }
}