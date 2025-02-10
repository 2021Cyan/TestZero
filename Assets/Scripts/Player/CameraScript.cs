using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    // Transform of the player
    public Transform player;
    // How smooth the camera will move 
    public float smoothSpeed = 0.125f;
    // Offset of the camera from the player
    public Vector3 offset;

    // Cam size
    public float camSize = 10f;
    private Camera cam;

    // Screen shake variables
    private bool isShaking = false;
    private float shakeDuration = 0f;
    private float shakePower = 0f;
    private float dampingSpeed = 1.5f;
    private Vector3 shakeOffset;

    private void Update()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = camSize; 
    }

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 desiredPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = new Vector3(smoothedPosition.x + shakeOffset.x, smoothedPosition.y + shakeOffset.y, transform.position.z);
        }
    }

    public void StartShake(float duration, float magnitude)
    {
        if (!isShaking)
        {
            shakeDuration = duration;
            shakePower = magnitude;
            StartCoroutine(Shake());
        }
    }

    private IEnumerator Shake()
    {
        isShaking = true;
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float strength = Mathf.Lerp(shakePower, 0, elapsed / shakeDuration);
            float x = Random.Range(-0.2f, 0.2f) * strength;
            float y = Random.Range(-0.2f, 0.2f) * strength;
            shakeOffset = new Vector3(x, y, 0); 
            elapsed += Time.deltaTime * dampingSpeed;
            yield return null;
        }
        isShaking = false;
        shakeOffset = Vector3.zero;
    }
}
