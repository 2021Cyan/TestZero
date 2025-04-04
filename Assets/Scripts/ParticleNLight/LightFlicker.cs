using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;


[RequireComponent(typeof(Light2D))]
public class LightFlicker : MonoBehaviour
{
    public float time = 1f;
    private Light2D light2D;

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
            float targetIntensity = startIntensity == 1 ? 0 : 1;
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
