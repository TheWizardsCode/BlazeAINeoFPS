using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NeoFPS;
using UnityEngine.Serialization;

namespace WizardsCode.BlazeNeoFPS
{
    /// <summary>
    /// Manages the health of a destructible object and will destroy the object when it "dies"
    /// </summary>
    [RequireComponent(typeof(BasicDamageHandler))]
    [RequireComponent(typeof(BasicHealthManager))]
    [RequireComponent(typeof(SimpleSurface))]
    [RequireComponent(typeof(Collider))]
    public class DestructibleController : MonoBehaviour
    {
        [SerializeField, Tooltip("The particle effect to replace the object with when it is destroyed. These effects will be adjusted to match the obnject being destroyed.")]
        [FormerlySerializedAs("m_DestructionParticles")]
        GameObject[] m_ScaledDestructionParticles;
        [SerializeField, Tooltip("Density of particles to spawn. The higher this value the more particles will spawn."), Range(0.5f, 100)]
        float m_ParticalDensity = 25;
        [SerializeField, Tooltip("Explosive force. The higher this value the more the particles will move away from the center of the destructable object."), Range(0f, 10f)]
        float m_ExplosiveForce = 3;
        [SerializeField, Tooltip("The particle effect to replace the object with when it is destroyed. These effects will be adjusted to match the obnject being destroyed.")]
        GameObject[] m_UnscaledDestructionParticles;

        [Header("Fall Damage")]
        [SerializeField, Tooltip("A multiplier for fall damage. 0 means no damage will be taken.")]
        float m_ImpactDamageMultiplier = 1;

        private BasicHealthManager m_HealthManager;

        private void OnEnable()
        {
            m_HealthManager = GetComponent<BasicHealthManager>();
            m_HealthManager.onIsAliveChanged += OnIsAliveChanged;
        }

        private void OnDisable()
        {
            m_HealthManager.onIsAliveChanged -= OnIsAliveChanged;
        }

        protected void OnIsAliveChanged(bool isAlive)
        {
            if (!isAlive)
            {
                if (m_ScaledDestructionParticles != null)
                {
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
                    //Debug.Log($"Bounds multiplier {boundsMultiplier}");

                    //OPTIMIZATION: Use a pool system for the destruction particles
                    for (int d = 0; d < m_ScaledDestructionParticles.Length; d++)
                    {
                        GameObject go = Instantiate(m_ScaledDestructionParticles[d]);
                        go.transform.SetParent(transform);
                        go.transform.localPosition = Vector3.zero;

                        ParticleSystem[] ps = go.GetComponentsInChildren<ParticleSystem>();

                        ParticleSystem.ShapeModule shape;
                        ParticleSystem.EmissionModule emission;
                        ParticleSystem.Burst burst;
                        ParticleSystem.ForceOverLifetimeModule force;
                        for (int i = 0; i < ps.Length; i++)
                        {
                            shape = ps[i].shape;
                            if (shape.shapeType == ParticleSystemShapeType.MeshRenderer)
                            {
                                shape.meshRenderer = meshRenderer;
                                ps[i].GetComponent<ParticleSystemRenderer>().material = meshRenderer.material;
                            }

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
                            }
                            else
                            {
                                force.enabled = false;
                            }
                        }

                        Destroy(go, 10);
                    }
                }

                if (m_UnscaledDestructionParticles != null)
                {
                    //OPTIMIZATION: Use a pool system for the destruction particles
                    for (int d = 0; d < m_UnscaledDestructionParticles.Length; d++)
                    {
                        GameObject go = Instantiate(m_UnscaledDestructionParticles[d]);
                        go.transform.SetParent(transform);
                        go.transform.localPosition = Vector3.zero;

                    }
                }

                Destroy(gameObject, 15);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (m_ImpactDamageMultiplier == 0) return;

            ContactPoint contact = collision.contacts[0];
            Vector3 normal = contact.normal;
            Vector3 relativeVelocity = collision.relativeVelocity;
            float damage = Vector3.Dot(normal, relativeVelocity) * m_ImpactDamageMultiplier;
            m_HealthManager.AddDamage(damage);
        }
    }
}
