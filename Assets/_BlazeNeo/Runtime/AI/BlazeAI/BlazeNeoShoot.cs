using NeoFPS;
using NeoFPS.ModularFirearms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WizardsCode.BlazeNeoFPS;

namespace WizardsCode.UnnofficialNeoFPSExtension
{
    /// <summary>
    /// This is about the simplest shooter AI you can have for Neo FPS. See the Cover demo in Blaze AI for an even simpler example that is not integrated to NeoFPS.
    /// 
    /// Add this to your AI character and ensure that your attack animation triggers the `ShootFrame` event at the correct time. The demo animation in Blaze AI is already setup correctly.
    /// </summary>
    public class BlazeNeoShoot : MonoBehaviour, IDamageSource
    {
        [SerializeField, Tooltip("The point of the weapon at which the muzzle flash and other effects will be spawned.")]
        Transform m_Muzzle;
        [SerializeField, Tooltip("Muzzle effects to instantiate when the gun is fired.")]
        GameObject m_MuzzleFX;
        [SerializeField, Tooltip("The audio source that will play any gunshot sounds.")]
        AudioSource m_SoundFX;
        [SerializeField, Tooltip("A set of SFX clips, one of which will be fired when the weapon is fired.")]
        AudioClip[] m_AudioClips;
        [SerializeField, Tooltip("The minimum and maximum amount of damage done as standard. Note that this will often be adjusted based on the current difficulty level.")]
        Vector2 m_Damage = new Vector2(8,11);
        [SerializeField, Tooltip("Event first each time the weapon is shot. This is usually triggered by the 'ShotFrame' Event in the animation.")]
        public UnityEvent m_OnShoot;

        BasicHealthManager healthManager;

        BlazeAI blaze;

        public DamageFilter outDamageFilter
        {
            get { return DamageFilter.AllDamageAllTeams; }
            set { }
        }

        Vector2 m_AdjustedDamage = Vector2.one;
        float m_DamageMultiplier = 1;
        public float damageMultiplier
        {
            get { return m_DamageMultiplier; }
            internal set { 
                m_DamageMultiplier = value;
                m_AdjustedDamage.x = m_Damage.x * m_DamageMultiplier;
                m_AdjustedDamage.y = m_Damage.y * m_DamageMultiplier;
            }
        }

        public float damage
        {
            get { return Random.Range(m_AdjustedDamage.x, m_AdjustedDamage.y); }
        }

        public IController controller
        {
            get { return null; }
        }

        public Transform damageSourceTransform
        {
            get { return transform; }
        }

        public string description
        {
            get { return name; }
        }

        private void OnEnable()
        {
            damageMultiplier = GameManager.Instance.mission.damageMultiplier;
        }

        void Start()
        {
            blaze = GetComponent<BlazeAI>();
        }

        public void ShotFrame()
        {
            if (blaze.enemyCover != null)
            {
                if (!healthManager || blaze.enemyCover != healthManager.gameObject)
                {
                    blaze.enemyCover.TryGetComponent<BasicHealthManager>(out healthManager);
                }
            }
            else if (blaze.enemyToAttack != null)
            {
                if (!healthManager || blaze.enemyToAttack != healthManager.gameObject)
                {
                    blaze.enemyToAttack.TryGetComponent<BasicHealthManager>(out healthManager);
                }
            }

            if (healthManager != null)
            {
                healthManager.AddDamage(damage, true, this);
                
                if (m_MuzzleFX != null && m_Muzzle != null)
                {
                    // TODO: use a pool for Muzzle Flash
                    GameObject go = Instantiate(m_MuzzleFX, m_Muzzle.transform.position, m_Muzzle.transform.rotation);
                    Destroy(go, 5);
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
            if (m_SoundFX == null || m_AudioClips.Length == 0) return;

            m_SoundFX.clip = m_AudioClips[Random.Range(0, m_AudioClips.Length)];
            m_SoundFX.Play();
        }

        public void Aiming()
        {
        }
    }
}