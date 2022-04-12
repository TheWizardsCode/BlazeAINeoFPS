using EmeraldAI;
using NeoFPS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace WizardsCode.EmeraldAIExtension
{
    /// <summary>
    /// The core Squad manager, starting with spawning/despawning on player proximity then managing the squad activity when active.
    /// </summary>
    [RequireComponent(typeof(SoloPlayerCharacterEventWatcher))]
    public class SquadBehaviours : MonoBehaviour, IPlayerCharacterSubscriber
    {
        [Header("Squad Activation")]
        [SerializeField, Tooltip("The number of members of this squad.")]
        int m_NumberOfMembers = 5;
        [SerializeField, Tooltip("The prefab to use when spawning members of this squad.")]
        EmeraldAISystem m_MemberPrefab;
        [SerializeField, Tooltip("The radius within which to spawn the members of this squad.")]
        float m_SpawnRadius = 15;
        [SerializeField, Tooltip("When a player is within this range activate the squad.")]
        float m_ActivationRange = 150;
        [SerializeField, Tooltip("When there are no players within this range deactivate the squad.")]
        float m_DeactivationRange = 250;
        [SerializeField, Tooltip("Is the squad active on startup? If they are they will be deactivated as soon as there are no players within range.")]
        bool m_IsActive = true;


        [Header("Squad Behaviours")]
        [SerializeField, Tooltip("How often, in seconds, the squad will review their coordinated efforts.")]
        float m_SquadOrdersFreqency = 3;

        private SoloPlayerCharacterEventWatcher m_Watcher;
        private ICharacter m_Player;

        List<EmeraldAISystem> m_SquadMembers = new List<EmeraldAISystem>();
        EmeraldAISystem m_SquadLeaderAISystem;
        EmeraldAIEventsManager m_SquadLeaderEventsManager;
        float m_TimeOfNextSquadOrdersReview;

        private EmeraldAISystem m_advancingSquadMember;
        private float m_OriginalAttackDistance;
        private bool spottedBarkPlayed = false;

        void Awake()
        {
            m_Watcher = GetComponent<SoloPlayerCharacterEventWatcher>();
        }

        private void Start()
        {
            Vector3 pos;
            EmeraldAISystem ai;
            for (int i = 0; i < m_NumberOfMembers; i++)
            {
                ai = Instantiate<EmeraldAISystem>(m_MemberPrefab);
                Vector2 offset = Random.insideUnitCircle * m_SpawnRadius;
                pos = transform.position;
                pos.x += offset.x;
                pos.z += offset.y;
                pos.y = Terrain.activeTerrain.SampleHeight(pos);
                ai.transform.position = pos;
                ai.gameObject.SetActive(m_IsActive);
                ai.GetComponent<EmeraldAIEventsManager>().SetDestinationPosition(pos);

                m_SquadMembers.Add(ai);
            }

            m_SquadLeaderAISystem = m_SquadMembers[0].GetComponent<EmeraldAISystem>();
            m_SquadLeaderEventsManager = m_SquadLeaderAISystem.GetComponent<EmeraldAIEventsManager>();
        }

        protected void OnEnable()
        {
            m_Watcher.AttachSubscriber(this);
        }

        private void OnDisable()
        {
            m_Watcher.ReleaseSubscriber(this);
        }

        public void OnPlayerCharacterChanged(ICharacter character)
        {
            m_Player = character;
        }

        void Update()
        {
            if (m_Player == null) return;

            if (!m_IsActive)
            {
                if (Vector3.Distance(m_Player.transform.position, transform.position) <= m_ActivationRange)
                {
                    SpawnSquad();
                } else
                {
                    return;
                }
            } else if (Vector3.Distance(m_Player.transform.position, transform.position) >= m_DeactivationRange)
            {
                DespawnSquad();
                return;
            }

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

        void SpawnSquad()
        {
            m_IsActive = true;
            for (int i = 0; i < m_SquadMembers.Count; i++)
            {
                m_SquadMembers[i].gameObject.SetActive(true);
            }
        }

        void DespawnSquad()
        {
            m_IsActive = false;
            for (int i = 0; i < m_SquadMembers.Count; i++)
            {
                m_SquadMembers[i].gameObject.SetActive(false);
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