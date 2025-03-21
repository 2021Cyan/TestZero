// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Rendering.Universal;

// [RequireComponent(typeof(ParticleSystem))]
// public class AttachGameObjectsToParticles : MonoBehaviour
// {
//     private ParticleSystem m_ParticleSystem;
//     private List<GameObject> m_Instances = new List<GameObject>();
//     private ParticleSystem.Particle[] m_Particles;
//     private LightPool lightPool;

//     // Start is called before the first frame update
//     void Start()
//     {
//         lightPool = LightPool.Instance;
//         m_ParticleSystem = GetComponent<ParticleSystem>();
//         m_Particles = new ParticleSystem.Particle[m_ParticleSystem.main.maxParticles];
//         var main = m_ParticleSystem.main;
//         main.stopAction = ParticleSystemStopAction.Callback;
//     }

//     // Update is called once per frame
//     void LateUpdate()
//     {
//         int particleCount = m_ParticleSystem.GetParticles(m_Particles);
//         // Calculate the number of instances needed for every third particle
//         int thirdCount = particleCount / 3;

//         // Ensure we have enough instances for every third particle
//         while (m_Instances.Count < thirdCount)
//         {
//             GameObject instance = lightPool.GetLight();
//             instance.transform.SetParent(m_ParticleSystem.transform);
//             m_Instances.Add(instance);
//         }

//         bool worldSpace = (m_ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World);

//         // Process only every third particle
//         for (int i = 0; i < m_Instances.Count; i++)
//         {
//             int particleIndex = i * 3; // Use every third particle

//             if (particleIndex < particleCount) // Ensure the particle index is within bounds
//             {
//                 if (worldSpace)
//                     m_Instances[i].transform.position = m_Particles[particleIndex].position;
//                 else
//                     m_Instances[i].transform.localPosition = m_Particles[particleIndex].position;

//                 Light2D light2D = m_Instances[i].GetComponent<Light2D>();
//                 light2D.color = m_Particles[particleIndex].GetCurrentColor(m_ParticleSystem);
//                 light2D.intensity = m_Particles[particleIndex].GetCurrentSize(m_ParticleSystem);
//                 m_Instances[i].SetActive(true);
//             }
//             else
//             {
//                 // Return excess instances to the pool
//                 lightPool.ReturnLight(m_Instances[i]);
//                 m_Instances.RemoveAt(i);
//                 i--; // Adjust the index after removal
//             }
//         }
//     }

//     private void OnParticleSystemStopped()
//     {
//         foreach (GameObject instance in m_Instances)
//         {
//             lightPool.ReturnLight(instance);
//         }
//         m_Instances.Clear();
//     }
// }