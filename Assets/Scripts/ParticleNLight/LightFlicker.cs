using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;


[RequireComponent(typeof(Light2D))]
public class LightFlicker : MonoBehaviour
{
    [SerializeField] private float time = 1f;
    [SerializeField] private float minIntensity = 0f;
    [SerializeField] private float maxIntensity = 1f;
    [SerializeField] private Light2D light2D;

    void Start()
    {
        light2D = GetComponent<Light2D>();
        StartCoroutine("Flicker");
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            float startIntensity = light2D.intensity;
            float targetIntensity = startIntensity == minIntensity ? 0 : maxIntensity;
            float elapsedTime = 0;

            while (elapsedTime < time)
            {
                light2D.intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / time);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            light2D.intensity = targetIntensity;
        }
    }
}
