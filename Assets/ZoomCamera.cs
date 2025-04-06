using UnityEngine;

public class CameraZoomAndMove : MonoBehaviour
{
    public Transform targetPosition;
    public float startOrthoSize = 10f;
    public float endOrthoSize = 5f;
    public float transitionDuration = 2f;
    public float lingerTime = 2f; // How long to wait after zoom/move ends

    private float zoomSpeed;
    private float moveSpeed;
    private float lingerTimer = 0f;
    private bool doneMoving = false;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = startOrthoSize;

        zoomSpeed = Mathf.Abs(startOrthoSize - endOrthoSize) / transitionDuration;
        float moveDistance = Vector3.Distance(transform.position, targetPosition.position);
        moveSpeed = moveDistance / transitionDuration;
    }

    void Update()
    {
        bool zoomDone = Mathf.Abs(cam.orthographicSize - endOrthoSize) < 0.01f;
        bool moveDone = Vector3.Distance(transform.position, targetPosition.position) < 0.01f;

        if (!zoomDone)
        {
            cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, endOrthoSize, zoomSpeed * Time.deltaTime);
        }

        if (!moveDone)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, moveSpeed * Time.deltaTime);
        }

        // Start linger countdown once both are done
        if (zoomDone && moveDone)
        {
            if (!doneMoving)
            {
                doneMoving = true;
                lingerTimer = lingerTime;
            }

            lingerTimer -= Time.deltaTime;
            if (lingerTimer <= 0f)
            {
                enabled = false; // Finally stop the script
            }
        }
    }
}
