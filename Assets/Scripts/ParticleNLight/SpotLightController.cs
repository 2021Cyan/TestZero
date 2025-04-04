using UnityEngine;
using System.Collections;

public class SpotLightController : MonoBehaviour
{
    [SerializeField] private float _time = 3f;
    [SerializeField] private float _rotationAngle = 30f;
    [SerializeField] private bool _rotateLeft = true;

    private Transform _lightHead;

    private void Awake()
    {
        _lightHead = transform.GetChild(0);
        StartCoroutine("RotateLightHead");
    }

    IEnumerator RotateLightHead()
    {
        while (true)
        {
            int direction = _rotateLeft ? 1 : -1;
            
            // First rotation
            yield return RotateByAngle(direction * _rotationAngle);
            // Return to origin
            yield return RotateByAngle(-direction * _rotationAngle);

            // Second rotation (opposite direction)
            yield return RotateByAngle(-direction * _rotationAngle);
            // Return to origin
            yield return RotateByAngle(direction * _rotationAngle);
        }
    }

    IEnumerator RotateByAngle(float angle)
    {
        float startAngle = _lightHead.localRotation.eulerAngles.z;
        if (startAngle > 180f) startAngle -= 360f; // Normalize angle
        float targetAngle = startAngle + angle;
        float elapsedTime = 0f;

        while (elapsedTime < _time)
        {
            float currentAngle = Mathf.Lerp(startAngle, targetAngle, elapsedTime / _time);
            _lightHead.localRotation = Quaternion.Euler(0f, 0f, currentAngle);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _lightHead.localRotation = Quaternion.Euler(0f, 0f, targetAngle);
    }
}
