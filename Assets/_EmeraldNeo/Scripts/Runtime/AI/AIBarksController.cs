using EmeraldAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WizardsCode.NeoEmeraldFPS
{
    /// <summary>
    /// Cause the enemy to emit barks on a semi-regular basis.
    /// </summary>
    public class AIBarksController : MonoBehaviour
    {
        [SerializeField, Tooltip("Average delay between barks. This is used to prevent the AI being too noisy.")]
        float m_ApproximateTimeBetweenBarks = 2;
        [SerializeField, Tooltip("Audio clips a to play when an enemy is spotted.")]
        AudioClip[] m_EnemySpottedClips;

        AudioSource audioSource;
        static float timeOfNextBark = 0;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError($"{name} has an AiBarkController but no AudioSource. Disabling BarkController.");
                enabled = false;
            }
        }

        public void OnEnemySpotted()
        {
            if (Time.timeSinceLevelLoad < timeOfNextBark && m_EnemySpottedClips.Length > 0) return;

            timeOfNextBark = Time.timeSinceLevelLoad + (m_ApproximateTimeBetweenBarks * Random.Range(0.8f, 1.2f));

            StartCoroutine(PlayBark(m_EnemySpottedClips[Random.Range(0, m_EnemySpottedClips.Length)]));
        }

        IEnumerator PlayBark(AudioClip clip)
        {
            yield return new WaitForSeconds(Random.Range(m_ApproximateTimeBetweenBarks / 6, m_ApproximateTimeBetweenBarks / 4));

            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}