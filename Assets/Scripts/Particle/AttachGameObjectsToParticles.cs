using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(ParticleSystem))]
public class AttachGameObjectsToParticles : MonoBehaviour
{
    private ParticleSystem m_ParticleSystem;
    private GameObject m_LightInstance;
    private ParticleSystem.Particle[] m_Particles;
    private LightPool lightPool;
    private Light2D light2D;

    void Start()
    {
        lightPool = LightPool.Instance;
        m_ParticleSystem = GetComponent<ParticleSystem>();
        m_Particles = new ParticleSystem.Particle[m_ParticleSystem.main.maxParticles];

        var main = m_ParticleSystem.main;
        main.stopAction = ParticleSystemStopAction.Callback;

        // Get a single light instance from the pool
        m_LightInstance = lightPool.GetLight();
        // m_LightInstance.transform.SetParent(m_ParticleSystem.transform);
        light2D = m_LightInstance.GetComponent<Light2D>();
        // m_LightInstance.SetActive(false); // Initially disable
    }

    void LateUpdate()
    {
        int particleCount = m_ParticleSystem.GetParticles(m_Particles);
        if (particleCount == 0)
        {
            return;
        }

        // Calculate the average position of all particles
        Vector3 averagePosition = Vector3.zero;
        float totalLifetime = 0f;
        float totalRemainingLifetime = 0f;

        
        for (int i = 0; i < particleCount; i++)
        {
            averagePosition += m_Particles[i].position;
            totalLifetime += m_Particles[i].startLifetime;
            totalRemainingLifetime += m_Particles[i].remainingLifetime;
        }
        averagePosition /= particleCount;
        m_LightInstance.transform.SetParent(m_ParticleSystem.transform);
        
        bool worldSpace = (m_ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World);
        if (worldSpace)
            m_LightInstance.transform.position = averagePosition;
        else
            m_LightInstance.transform.localPosition = averagePosition;

        // Adjust intensity based on remaining lifetime percentage
        float lifetimeRatio = totalRemainingLifetime / totalLifetime;
        light2D.intensity = Mathf.Lerp(0f, 1f, lifetimeRatio); // Adjust the max intensity as needed

        // Activate light
        m_LightInstance.SetActive(true);
    }

    private void OnParticleSystemStopped()
    {
        lightPool.ReturnLight(m_LightInstance);
        // m_LightInstance = null;
    }
}
