using NeoFPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WizardsCode.AI
{
    public class AIProjectileController : MonoBehaviour
    {
        [SerializeField, Tooltip("The spped the projectile moves at.")]
        float m_Speed = 100;
        [SerializeField, Tooltip("The layermask specifying which layers will cause a collision")]
        LayerMask m_CollisionLayerMask;
        [SerializeField, Tooltip("The prefab to instantiate when the projectile makes contact with something.")]
        public GameObject m_CollisionPrefab;
        [SerializeField, Tooltip("The mesh of the projectile. This will be turned off when a collision is detected.")]
        public MeshRenderer m_ProjectileMesh;
        [SerializeField, Tooltip("If ther projectile has a particle system that provides a trail or similar add it here so that it can be turned off when it stops moving, e.g. on collision.")]
        public ParticleSystem m_DisableOnHit;
        [SerializeField, Tooltip("The audio source to play while the projectile is in flight.")]
        AudioSource m_InFlightAudioSource;
        
        private bool targetHit;

        private bool IsCollisionLayer(GameObject obj, LayerMask layerMask)
        {
            return ((layerMask.value & (1 << obj.layer)) > 0);
        }
        private void Update()
        {
            if (targetHit) return;
            transform.position += transform.forward * (m_Speed * Time.deltaTime);
            if (transform.position.y < 1.5f)
            {
                transform.position += transform.up * Time.deltaTime;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (enabled && IsCollisionLayer(other.gameObject, m_CollisionLayerMask))
            {
                Explode();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // --- return if not enabled because OnCollision is still called if compoenent is disabled ---
            if (enabled || IsCollisionLayer(collision.gameObject, m_CollisionLayerMask))
            {
                Explode();
            }
        }

        private void Explode()
        {
            //OPTIMIZATION: Use a pool
            GameObject newExplosion = Instantiate(m_CollisionPrefab, transform.position, m_CollisionPrefab.transform.rotation, null);

            m_ProjectileMesh.enabled = false;
            targetHit = true;
            m_InFlightAudioSource.Stop();
            foreach (Collider col in GetComponents<Collider>())
            {
                col.enabled = false;
            }
            m_DisableOnHit.Stop();

            GetComponent<IDamageHandler>().AddDamage(100);

            Destroy(gameObject, 5f);
        }
    }
}