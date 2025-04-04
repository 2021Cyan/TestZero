using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;


[RequireComponent(typeof(Light2D))]
public class LightFlicker : MonoBehaviour
{
    [SerializeField] private float time = 1f;
    [SerializeField] private bool falloffStrengthControl = false;
    [SerializeField] private float falloffStrengthRange = 0.1f;
    [SerializeField] private Light2D light2D;


    private float initialFalloffStrength;
    private float initialIntensity;
    void Start()
    {
        light2D = GetComponent<Light2D>();
        initialFalloffStrength = light2D.falloffIntensity;
        initialIntensity = light2D.intensity;
        StartCoroutine("Flicker");
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            float startValue;
            float targetValue;

            startValue = light2D.intensity;
            targetValue = startValue == 0 ? initialIntensity : 0;
            if (falloffStrengthControl)
            {
                startValue = Mathf.Clamp(light2D.falloffIntensity, initialFalloffStrength - falloffStrengthRange, initialFalloffStrength + falloffStrengthRange);
                targetValue = Mathf.Approximately(startValue, initialFalloffStrength - falloffStrengthRange) ? initialFalloffStrength : (initialFalloffStrength - falloffStrengthRange);
            }
               
            float elapsedTime = 0;

            while (elapsedTime < time)
            {
                if (falloffStrengthControl)
                {
                    light2D.falloffIntensity = Mathf.Lerp(startValue, targetValue, elapsedTime / time);
                }
                else
                {
                    light2D.intensity = Mathf.Lerp(startValue, targetValue, elapsedTime / time);
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (falloffStrengthControl)
            {
                light2D.falloffIntensity = targetValue;
            }
            else
            {
                light2D.intensity = targetValue;
            }
        }
    }
}