using NeoFPS;
using NeoFPS.ModularFirearms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WizardsCode.UnnofficialNeoFPSExtension
{
    /// <summary>
    /// This is about the simplest shooter AI you can have for Neo FPS. See the Cover demo in Blaze AI for an even simpler example that is not integrated to NeoFPS.
    /// 
    /// Add this to your AI character and ensure that your attack animation triggers the `ShootFrame` event at the correct time. The demo animation in Blaze AI is already setup correctly.
    /// </summary>
    public class BlazeNeoShoot : MonoBehaviour
    {
        [SerializeField, Tooltip("The point of the weapon at which the muzzle flash and other effects will be spawned.")]
        Transform m_Muzzle;
        [SerializeField, Tooltip("Muzzle effects to instantiate when the gun is fired.")]
        GameObject m_MuzzleFX;
        [SerializeField, Tooltip("The audio source that will play any gunshot sounds.")]
        AudioSource m_SoundFX;
        [SerializeField, Tooltip("The minimum and maximum amount of damage done.")]
        Vector2 m_Damage = new Vector2(8,11);
        [SerializeField, Tooltip("Event first each time the weapon is shot. This is usually triggered by the 'ShotFrame' Event in the animation.")]
        public UnityEvent m_OnShoot;

        BasicHealthManager healthManager;

        BlazeAI blaze;
        private AudioSource[] audioSources;

        void Start()
        {
            blaze = GetComponent<BlazeAI>();
            audioSources = m_SoundFX.GetComponents<AudioSource>();
        }

        void Update()
        {
            GameObject target;
            if (blaze.attackingCover != null)
            {
                target = blaze.attackingCover;
            }
            else
            {
                target = blaze.enemyToAttack;
            }
        }

        public void ShotFrame()
        {
            if (!healthManager || blaze.enemyToAttack != healthManager.gameObject)
            {
                blaze.enemyToAttack.TryGetComponent<BasicHealthManager>(out healthManager);
            }

            if (healthManager != null)
            {
                healthManager.AddDamage(Random.Range(m_Damage.x, m_Damage.y), true);
                
                if (m_MuzzleFX != null && m_Muzzle != null)
                {
                    Instantiate(m_MuzzleFX, m_Muzzle.transform.position, m_Muzzle.transform.rotation);
                }

                PlayAudio();
            }

            if (m_OnShoot != null)
            {
                m_OnShoot.Invoke(); ;
            }
        }

        public void PlayAudio()
        {
            if (m_SoundFX == null) return;

            if (audioSources.Length >=0)
            {
                audioSources[Random.Range(0, audioSources.Length)].Play();
            }
        }

        public void Aiming()
        {
        }
    }
}