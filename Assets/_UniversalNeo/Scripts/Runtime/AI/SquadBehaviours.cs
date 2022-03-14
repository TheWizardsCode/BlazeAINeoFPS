using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalAI;

namespace WizardsCode.UniversalAIExtension
{
    public class SquadBehaviours : MonoBehaviour
    {
        [SerializeField, Tooltip("How often, in seconds, the squad will review their coordinated efforts.")]
        float m_SquadOrdersFreqency = 3;
        [SerializeField, Tooltip("The AI that are a member of this squad. The first AI in the list is considered the Squad Commander.")]
        UniversalAISystem[] m_SquadMembers;

        UniversalAISystem m_AISystem;
        UniversalAICommandManager m_CommandManager;
        float m_TimeOfNextSquadOrdersReview;

        private UniversalAISystem m_advancingSquadMember;
        private float m_OriginalAttackDistance;

        void Awake()
        {
            m_AISystem = m_SquadMembers[0].GetComponent<UniversalAISystem>();
            m_CommandManager = m_AISystem.UniversalAICommandManager;
        }

        void Update()
        {
            if (Time.timeSinceLevelLoad < m_TimeOfNextSquadOrdersReview) return;
            m_TimeOfNextSquadOrdersReview = Time.timeSinceLevelLoad + m_SquadOrdersFreqency;

            float distance = m_CommandManager.GetTargetDistance();

            for (int i = 0; i < m_SquadMembers.Length; i++)
            {                    
                if (m_SquadMembers[i].UniversalAICommandManager.GetTarget() == null 
                    && m_CommandManager.GetTarget() != null)
                {
                    m_SquadMembers[i].UniversalAICommandManager.SetTarget(m_CommandManager.GetTarget());
                    continue;
                } else
                {
                    if (i > 0 && m_CommandManager.GetTarget() == null)
                    {
                        m_CommandManager.SetTarget(m_SquadMembers[i].UniversalAICommandManager.GetTarget());
                        continue;
                    }
                }

                if (m_advancingSquadMember != null 
                    && m_SquadMembers[i].UniversalAICommandManager.GetTargetDistance() > m_advancingSquadMember.UniversalAICommandManager.GetTargetDistance())
                {
                    m_advancingSquadMember.Settings.Attack.AttackDistance = m_OriginalAttackDistance;
                    m_advancingSquadMember = m_SquadMembers[i];
                    m_OriginalAttackDistance = m_advancingSquadMember.Settings.Attack.AttackDistance;
                    m_advancingSquadMember.Settings.Attack.AttackDistance = distance * 0.8f;
                    continue;
                } else if (m_SquadMembers[i].UniversalAICommandManager.GetTargetDistance() > distance)
                {
                    if (m_advancingSquadMember)
                    {
                        m_advancingSquadMember.Settings.Attack.AttackDistance = m_OriginalAttackDistance;
                    }
                    m_advancingSquadMember = m_SquadMembers[i];
                    m_OriginalAttackDistance = m_advancingSquadMember.Settings.Attack.AttackDistance;
                    m_advancingSquadMember.Settings.Attack.AttackDistance = distance * 0.8f;
                }
            }



            if (m_advancingSquadMember != null)
            {
                Debug.Log($"{m_advancingSquadMember} is advancing from a distance of {m_advancingSquadMember.UniversalAICommandManager.GetTargetDistance()} to a distance of up to {m_advancingSquadMember.Settings.Attack.AttackDistance}.");
            }
        }
    }
}