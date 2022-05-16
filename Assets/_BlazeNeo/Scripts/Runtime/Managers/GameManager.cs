using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace WizardsCode.NeoEmeraldFPS
{
    /// <summary>
    /// Manages the game state and monitors for win/lose conditions.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField, Tooltip("How many enemies does the player need to kill in order to win the game.")]
        internal int m_ScoreNeededForTheWin = 1000;
        [SerializeField, Tooltip("The name of the scene to load upon winning.")]
        private string m_WinningSceneName = "Win";

        [SerializeField, Tooltip("The name of the scene to load upon losing.")]
        private string m_LosingSceneName = "Lose";

        [SerializeField, Tooltip("An event that is fired whenever the score changes. The parameter is the new score.")]
        public UnityAction<int> onScoreChanged;

        int m_LivesLost = 0;
        int m_Score = 0;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        internal void AddScore(int score)
        {
            m_Score += score;
            if (onScoreChanged != null) 
                onScoreChanged(m_Score);
        }

        /// <summary>
        /// Test to see if the player has lost the game.
        /// </summary>
        private bool HasLost
        {
            get
            {
                return m_LivesLost > 3;
            }
        }

        /// <summary>
        /// Test to see if the player has won the game.
        /// </summary>
        private bool HasWon
        {
            get
            {
                return m_Score >= m_ScoreNeededForTheWin;
            }
        }

        void Update()
        {
            if (HasLost)
            {
                SceneManager.LoadScene(m_LosingSceneName);
            } else if (HasWon)
            {
                SceneManager.LoadScene(m_WinningSceneName);
            }
        }

        /// <summary>
        /// Call this whenever the player is killed.
        /// </summary>
        public void OnPlayerDeath()
        {
            m_LivesLost++;
        }


        /// <summary>
        /// Call this whenever an enemy is killed.
        /// </summary>
        public void OnEnemyDeath()
        {
            m_Score++;
        }
    }
}
