using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WizardsCode.Common;

namespace WizardsCode.AI
{
    /// <summary>
    /// The BlazeAIManager is responsible for controlling the pace of the game by coordinating the AI behaviours.
    /// 
    /// It does this by looking at the players current health and loadout and adjusting things like AI accuracy,
    /// loot drops and more.
    /// 
    /// </summary>
    public class AIManager : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField, Tooltip("How frequently should the AI Manager tune the AI behaviours?")]
        float m_UpdateFrequency = 5;
        #endregion

        #region Properties
        /// <summary>
        /// A value between 0 and 1 indicating the value of the current loot drops. 
        /// 0 being the least valuable.
        /// </summary>
        public float lootValue { get; private set; }
        #endregion

        private void Start()
        {
            StartCoroutine(UpdateAILootDrops());
            //TODO: GAMEPLAY: Update the Alert delay and alert distance to control the intensity of the battle
        }

        /// <summary>
        /// Changes the liklihood of valuable loot being dropped when, for example, an AI is killed.
        /// </summary>
        /// <returns></returns>
        IEnumerator UpdateAILootDrops()
        {
            yield return new WaitForSeconds(Random.Range(0, m_UpdateFrequency));

            while (true)
            {
                float winProgress = (float)AbstractGameManager.Instance.score / AbstractGameManager.Instance.mission.m_ScoreNeededForTheWin;
                float livesLeft = (float)AbstractGameManager.Instance.livesLost / AbstractGameManager.Instance.mission.m_LivesAvailable;

                lootValue = 1 - ((winProgress + livesLeft) / 2);

                yield return new WaitForSeconds(m_UpdateFrequency);
            }
        }

    }
}
