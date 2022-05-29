using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NeoFPS;

namespace WizardsCode.BlazeNeoFPS
{
    /// <summary>
    /// Manages the health of a destructible object and will destroy the object when it "dies"
    /// </summary>
    [RequireComponent(typeof(BasicDamageHandler))]
    [RequireComponent(typeof(SimpleSurface))]
    [RequireComponent(typeof(Collider))]
    public class DestructibleController : BasicHealthManager
    {
        [SerializeField, Tooltip("The particle effect to replace the object with when it is destroyed.")]
        GameObject m_DestructionParticles;
        [SerializeField, Tooltip("Density of particles to spawn. The higher this value the more particles will spawn."), Range(0.5f, 100)]
        float m_ParticalDensity = 25;
        [SerializeField, Tooltip("Explosive force. The higher this value the more the particles will move away from the center of the destructable object."), Range(0f, 10f)]
        float m_ExplosiveForce = 3;
        

        protected override void OnIsAliveChanged()
        {
            if (!isAlive)
            {
                if (m_DestructionParticles != null)
                {
                    //OPTIMIZATION: Use a pool system for the destruction particles
                    GameObject go = Instantiate(m_DestructionParticles, transform);
                    //OPTIMIZATION: cache on start
                    ParticleSystem[] ps = go.GetComponentsInChildren<ParticleSystem>();
                    //OPTIMIZATION: cache on start
                    MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
                    meshRenderer.enabled = false;
                    
                    //OPTIMIZATION: cache on start
                    Collider[] colliders = GetComponentsInChildren<Collider>();
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        colliders[i].enabled = false;
                    }

                    int boundsMultiplier = 2 * (int)(meshRenderer.bounds.extents.x + meshRenderer.bounds.extents.y + meshRenderer.bounds.extents.z);
                    Debug.Log($"Bounds multiplier {boundsMultiplier}");

                    ParticleSystem.ShapeModule shape;
                    ParticleSystem.EmissionModule emission;
                    ParticleSystem.Burst burst;
                    ParticleSystem.ForceOverLifetimeModule force;
                    for (int i = 0; i < ps.Length; i++)
                    {
                        shape = ps[i].shape;
                        shape.meshRenderer = meshRenderer;

                        emission = ps[i].emission;
                        burst = emission.GetBurst(0);
                        burst.count = m_ParticalDensity * boundsMultiplier;
                        emission.SetBurst(0, burst);

                        force = ps[i].forceOverLifetime;
                        if (m_ExplosiveForce > 0)
                        {
                            force.enabled = true;
                            force.x = Random.Range(-m_ExplosiveForce, m_ExplosiveForce);
                            force.y = Random.Range(-m_ExplosiveForce, m_ExplosiveForce);
                            force.z = Random.Range(-m_ExplosiveForce, m_ExplosiveForce);
                        } else
                        {
                            force.enabled = false;
                        }

                        ps[i].GetComponent<ParticleSystemRenderer>().material = meshRenderer.material;
                    }

                    Destroy(gameObject, 10);
                }
            }
            base.OnIsAliveChanged();
        }
    }
}
