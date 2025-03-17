using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CameraScript : MonoBehaviour
{
    // Transform of the player
    public Transform player;
    // How smooth the camera will move 
    public float smoothSpeed = 0.125f;
    // Offset of the camera from the player
    public Vector3 offset;

    // Cam
    private CinemachineCamera vcam;
    private CinemachineBasicMultiChannelPerlin noise;

    // Screen shake variables
    private bool isShaking = false;
    private float shakeDuration = 0f;
    private float shakePower = 0f;
    private float dampingSpeed = 1.5f;
    private Vector3 shakeOffset;

    private void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();
        // noise = vcam.GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;
    }
    private void Update()
    {
        // cam = GetComponent<Camera>();
        // cam.orthographicSize = camSize; 
    }


    public void StartShake(float duration, float magnitude)
    {
        
        if (!isShaking)
        {
            noise.AmplitudeGain = magnitude;
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
            noise.PivotOffset = new Vector3(x, y, 0);
            // shakeOffset = new Vector3(x, y, 0); 
            elapsed += Time.deltaTime * dampingSpeed;
            yield return null;
        }
        isShaking = false;
        noise.AmplitudeGain = 0;
        noise.PivotOffset = Vector3.zero;
    }
}
