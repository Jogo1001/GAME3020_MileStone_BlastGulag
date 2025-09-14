using UnityEngine;

public class BlastGulagCamera : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    [Header("Camera Settings")]
    public float smoothSpeed = 0.2f;  // how smoothly the camera follows
    public float minZoom = 5f;
    public float maxZoom = 10f;
    public float zoomLimiter = 50f;   // tweak this for your level size

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (player1 == null || player2 == null) return;

        // Move camera to midpoint between players
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint;
        newPosition.z = transform.position.z; // keep original z
        transform.position = Vector3.Lerp(transform.position, newPosition, smoothSpeed);

        // Zoom based on distance between players
        float distance = (player1.position - player2.position).magnitude;
        float newZoom = Mathf.Lerp(maxZoom, minZoom, distance / zoomLimiter);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
    }

    Vector3 GetCenterPoint()
    {
        return (player1.position + player2.position) / 2f;
    }
}
