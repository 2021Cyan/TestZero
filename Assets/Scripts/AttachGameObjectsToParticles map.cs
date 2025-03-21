// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Rendering.Universal;

// [RequireComponent(typeof(ParticleSystem))]
// public class AttachGameObjectsToParticles : MonoBehaviour
// {
//     private ParticleSystem m_ParticleSystem;
//     private Dictionary<int, GameObject> m_Instances = new Dictionary<int, GameObject>(); // Use a dictionary
//     private ParticleSystem.Particle[] m_Particles;
//     private LightPool lightPool;

//     void Start()
//     {
//         lightPool = LightPool.Instance;
//         m_ParticleSystem = GetComponent<ParticleSystem>();
//         m_Particles = new ParticleSystem.Particle[m_ParticleSystem.main.maxParticles];
//         var main = m_ParticleSystem.main;
//         main.stopAction = ParticleSystemStopAction.Callback;
//     }

//     void LateUpdate()
//     {
//         int particleCount = m_ParticleSystem.GetParticles(m_Particles);
//         int evenCount = particleCount / 2;

//         // Ensure we have enough instances for even indices
//         for (int i = 0; i < evenCount; i++)
//         {
//             int particleIndex = i * 2;

//             if (!m_Instances.ContainsKey(particleIndex))
//             {
//                 GameObject instance = lightPool.GetLight();
//                 instance.transform.SetParent(m_ParticleSystem.transform);
//                 m_Instances[particleIndex] = instance;
//             }
//         }

//         bool worldSpace = (m_ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World);

//         // Process only even indices
//         List<int> keysToRemove = new List<int>();

//         foreach (var kvp in m_Instances)
//         {
//             int particleIndex = kvp.Key;
//             GameObject instance = kvp.Value;

//             if (particleIndex < particleCount)
//             {
//                 if (worldSpace)
//                     instance.transform.position = m_Particles[particleIndex].position;
//                 else
//                     instance.transform.localPosition = m_Particles[particleIndex].position;

//                 Light2D light2D = instance.GetComponent<Light2D>();
//                 light2D.color = m_Particles[particleIndex].GetCurrentColor(m_ParticleSystem);
//                 light2D.intensity = m_Particles[particleIndex].GetCurrentSize(m_ParticleSystem);
//                 instance.SetActive(true);
//             }
//             else
//             {
//                 // Return excess instances to the pool
//                 lightPool.ReturnLight(instance);
//                 keysToRemove.Add(particleIndex);
//             }
//         }

//         foreach (int key in keysToRemove)
//         {
//             m_Instances.Remove(key);
//         }
//     }

//     private void OnParticleSystemStopped()
//     {
//         foreach (var instance in m_Instances.Values)
//         {
//             lightPool.ReturnLight(instance);
//         }
//         m_Instances.Clear();
//     }
// }
