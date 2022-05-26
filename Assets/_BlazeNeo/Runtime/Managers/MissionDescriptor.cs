using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WizardsCode.BlazeNeoFPS
{
    /// <summary>
    /// Describes the conditions that must be met for a mission to be considered succesful.
    /// </summary>
    [CreateAssetMenu(fileName = "New Mission Descriptor", menuName = "Wizards Code/BlazeNeo/Mission Descriptor")]
    public class MissionDescriptor : ScriptableObject
    {
        [SerializeField, Tooltip("The time allowed for the mission." +
            " The player must complete all objectives and be in the extraction zone when this time elapses." +
            " If they are in the extraction zone but have not completed on or more missions objectives they survive" +
            " but do not complete the mission. If they fail to be in the objective at the point of extraction then" +
            " they die permanently.")]
        public int m_MaxMissionTimeInMinutes = 10;
        
        [SerializeField, Tooltip("How many enemies does the player need to kill in order to win the game.")]
        internal int m_ScoreNeededForTheWin = 1000;

        [SerializeField, Tooltip("How many lives is the player given before they are considered to have lost the game.")]
        internal int m_LivesAvailable = 3;

        [SerializeField, Tooltip("The name of the scene to load upon winning.")]
        internal string m_ExtractionMissionCompleteSceneName = "ExtractionWithMissionComplete";

        [SerializeField, Tooltip("The name of the scene to load upon winning.")]
        internal string m_ExtractionMissionIncompleteSceneName = "ExtractionWithMissionIncomplete";

        [SerializeField, Tooltip("The name of the scene to load upon losing.")]
        internal string m_LosingSceneName = "Lose";

        [SerializeField, Tooltip("A NPC terget who must be killed for this mission to be considered complete.")]
        internal KillTarget m_KillTarget;

        public bool IsComplete { get; internal set; }

    }
}
