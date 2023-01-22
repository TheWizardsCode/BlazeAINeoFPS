using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BlazeAISpace;

namespace WizardsCode.FPS
{
    /// <summary>
    /// This component has an awake function that will randomize some of the AI
    /// parameters and then destroy the component. This is used to vary the AI
    /// a little.
    /// </summary>
    public class RandomizeBlazeAI : MonoBehaviour
    {
        [Header("Model")]
        [SerializeField, Tooltip("The % variation allowed in the AI size. This is a minimum and maximum variation allowed.")]
        Vector2 m_SizeVariation = new Vector2(0.9f, 1.1f);

        [Header("Attack")]
        [SerializeField, Tooltip("The % variation allowed in the safe distance and attack range. This is a minimum and maximum variation allowed.")]
        Vector2 m_AttackRange = new Vector2(0.9f, 1.1f);

        [Header("Cover")]
        /* Blaze v1 feature that seems to have been removed
        [SerializeField, Tooltip("Should the cover position (center of cover or edge) be randomized?")]
        bool m_RandomizeCoverPosition = true;
        */
        [SerializeField, Tooltip("The % variation allowed in the cover search range. This is a minimum and maximum variation allowed.")]
        Vector2 m_CoverRange = new Vector2(0.9f, 1.1f);
        [SerializeField, Tooltip("The % variation allowed in the cover sensitivity. This is a minimum and maximum variation allowed.")]
        Vector2 m_CoverSensitivity = new Vector2(0.9f, 1.1f);

        private BlazeAI m_ai;

        private void Awake()
        {
            m_ai = GetComponentInChildren<BlazeAI>();

            RandomizeModel();
            RandomizeAttack();
            RandomizeCover();
            //TODO Randomize call to others, call radius, coll others time, recieve call from others

            Destroy(this);
        }

        private void RandomizeCover()
        {
            /* Blaze v1 feature that seems to have been removed
            if (m_RandomizeCoverPosition)
            {
                if (Random.value > 0.5)
                {
                    m_ai.attackState.coverShooterOptions.moveToCoverCenter = true;
                    m_ai.coverShooterBehaviour.
                } 
                else
                {
                    m_ai.attackState.coverShooterOptions.moveToCoverCenter = false;
                }
            }
            */

            GoingToCoverBehaviour cover = m_ai.GetComponent<GoingToCoverBehaviour>();
            cover.searchDistance *= Random.Range(m_CoverRange.x, m_CoverRange.y);    
            cover.hideSensitivity *= Random.Range(m_CoverRange.x, m_CoverRange.y);
        }

        private void RandomizeAttack()
        {
            float variation = Random.Range(m_AttackRange.x, m_AttackRange.y);
            //TODO: this needs to handle cover and non-cover attack modes, currently only cover is handled
            CoverShooterBehaviour shooter = m_ai.GetComponent<CoverShooterBehaviour>();
            shooter.distanceFromEnemy *= variation;
            shooter.attackDistance *= variation;
        }

        private void RandomizeModel()
        {
            Vector3 size = transform.localScale;
            float variation = Random.Range(m_SizeVariation.x, m_SizeVariation.y);
            size *= variation;
            transform.localScale = size;
        }
    }
}
