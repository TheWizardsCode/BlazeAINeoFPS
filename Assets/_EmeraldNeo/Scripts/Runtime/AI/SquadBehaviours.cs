using EmeraldAI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace WizardsCode.EmeraldAIExtension
{
    public class SquadBehaviours : MonoBehaviour
    {
        [SerializeField, Tooltip("How often, in seconds, the squad will review their coordinated efforts.")]
        float m_SquadOrdersFreqency = 3;
        [SerializeField, Tooltip("The AI that are members of this squad. The first AI in the list is considered the Squad Commander.")]
        List<EmeraldAISystem> m_SquadMembers = new List<EmeraldAISystem>();

        EmeraldAISystem m_SquadLeaderAISystem;
        EmeraldAIEventsManager m_SquadLeaderEventsManager;
        float m_TimeOfNextSquadOrdersReview;

        private EmeraldAISystem m_advancingSquadMember;
        private float m_OriginalAttackDistance;
        private bool spottedBarkPlayed = false;

        void Awake()
        {
            m_SquadLeaderAISystem = m_SquadMembers[0].GetComponent<EmeraldAISystem>();
            m_SquadLeaderEventsManager = m_SquadLeaderAISystem.GetComponent<EmeraldAIEventsManager>();

        }

        void Update()
        {
            if (Time.timeSinceLevelLoad < m_TimeOfNextSquadOrdersReview) return;

            m_TimeOfNextSquadOrdersReview = Time.timeSinceLevelLoad + m_SquadOrdersFreqency;

            float distance = Mathf.Min(m_SquadLeaderEventsManager.GetDistanceFromTarget(), m_SquadLeaderAISystem.AttackDistance * 0.5f);

            for (int i = 0; i < m_SquadMembers.Count; i++)
            {
                // If AI has been destroyed remove them from the squad
                if (!m_SquadMembers[i])
                {
                    // If this was the last squad member destroy the squad behaviour, the remaining squad member will act individually
                    if (m_SquadMembers.Count == 1)
                    {
                        Destroy(gameObject);
                        return;
                    }

                    m_SquadMembers.RemoveAt(i);
                    // If they were the commander nominate a new commander
                    if (i == 0)
                    {
                        m_SquadLeaderAISystem = m_SquadMembers[0];
                        // OPTIMIZATION: Cache the EmeraldAIEventsManager
                        m_SquadLeaderEventsManager = m_SquadLeaderAISystem.GetComponent<EmeraldAIEventsManager>();
                        distance = Mathf.Min(m_SquadLeaderEventsManager.GetDistanceFromTarget(), m_SquadLeaderAISystem.AttackDistance * 0.5f);
                    }
                }

                // Ensure all Squad members have the same target
                // OPTIMIZATION: Cache the EmeraldAIEventsManager
                EmeraldAIEventsManager currentSquadMemberEventManager = m_SquadMembers[i].GetComponent<EmeraldAIEventsManager>();
                if (m_SquadMembers[i].HasTarget)
                {
                    RadioInformation(currentSquadMemberEventManager);
                }

                if (m_SquadLeaderEventsManager.GetCombatTarget() == null) return;

                float optimalAttackDistance = (m_SquadMembers[i].AttackDistance - m_SquadMembers[i].BackupDistance) / 2;

                if (m_advancingSquadMember != null
                    && currentSquadMemberEventManager.GetDistanceFromTarget() > m_advancingSquadMember.GetComponent<EmeraldAIEventsManager>().GetDistanceFromTarget())
                {
                    m_advancingSquadMember.AttackDistance = m_OriginalAttackDistance;
                    m_advancingSquadMember = m_SquadMembers[i];
                    m_OriginalAttackDistance = m_advancingSquadMember.AttackDistance;
                    m_advancingSquadMember.AttackDistance = distance * 0.8f;
                    continue;
                } else if (currentSquadMemberEventManager.GetDistanceFromTarget() > distance)
                {
                    if (m_advancingSquadMember)
                    {
                        m_advancingSquadMember.AttackDistance = m_OriginalAttackDistance;
                    }
                    m_advancingSquadMember = m_SquadMembers[i];
                    m_OriginalAttackDistance = m_advancingSquadMember.AttackDistance;
                    m_advancingSquadMember.AttackDistance = distance * 0.8f;
                }
            }
        }


        /// <summary>
        /// Spread information around the squad using the radio.
        /// </summary>
        private void RadioInformation(EmeraldAIEventsManager otherMember)
        {
            if (otherMember == m_SquadLeaderEventsManager)
            {
                return;
            }

            if (m_SquadLeaderEventsManager.GetCombatTarget() == null
                && otherMember.GetCombatTarget() != null) 
            {

                m_SquadLeaderEventsManager.OverrideCombatTarget(otherMember.GetCombatTarget());
                return;
            }

            if (otherMember.GetCombatTarget() == null
                && m_SquadLeaderEventsManager.GetCombatTarget() != null)
            {
                otherMember.OverrideCombatTarget(m_SquadLeaderEventsManager.GetCombatTarget());
            }
        }
    }
}