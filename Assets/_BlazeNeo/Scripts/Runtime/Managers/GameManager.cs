using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using WizardsCode.Managers;
using NeoFPS;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WizardsCode.BlazeNeoFPS
{
    /// <summary>
    /// Manages the game state and monitors for win/lose conditions.
    /// </summary>
    public class GameManager : SingletonBehaviour<GameManager>, IPlayerCharacterSubscriber
    {
        [Header("Timer")]
        [SerializeField, Tooltip("Time until extraction in seconds. The player needs to have killed enough enemies and be in the extraction zone when this timer hits 0.")]
        float m_TimeUntilExtraction = 600;

        [Header("Score")]
        [SerializeField, Tooltip("How many enemies does the player need to kill in order to win the game.")]
        internal int m_ScoreNeededForTheWin = 1000;
        [SerializeField, Tooltip("How many lives is the player given before they are considered to have lost the game.")]
        internal int m_LivesAvailable = 3;

        [SerializeField, Tooltip("The name of the scene to load upon winning.")]
        private string m_WinningSceneName = "Win";

        [SerializeField, Tooltip("The name of the scene to load upon losing.")]
        private string m_LosingSceneName = "Lose";

        [Header("Game Events")]
        [SerializeField, Tooltip("An event that is fired whenever the score changes. The parameter is the new score.")]
        public UnityAction<int> onScoreChanged;
        [SerializeField, Tooltip("An event that is fired whenever the extraction point timer changes by 1 second. The parameter is the number of seconds currently remaining.")]
        public UnityAction<int> onTimerChanged;

        ExtractionPointController m_ExtractionPoint;
        private int currentTimeDisplayed;
        private IHealthManager playerHealthManager;

        public int livesLost { get; private set; }   
        public int score { get; private set; }

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            m_ExtractionPoint = FindObjectOfType<ExtractionPointController>();

            FindObjectOfType<SoloPlayerCharacterEventWatcher>().AttachSubscriber(this);
        }

        private void Start()
        {
            currentTimeDisplayed = (int)m_TimeUntilExtraction;
        }

        internal void AddScore(int score)
        {
            this.score += score;
            if (onScoreChanged != null)
                onScoreChanged(this.score);
        }

        /// <summary>
        /// Test to see if the player has lost the game.
        /// </summary>
        private bool HasLost
        {
            get
            {
                return livesLost >= m_LivesAvailable;
            }
        }

        #region Lifecycle
        void Update()
        {
            if (!FpsGameMode.inGame)
            {
                return;
            }

            if (HasLost)
            {
                SceneManager.LoadScene(m_LosingSceneName);
                return;
            }

            m_TimeUntilExtraction -= Time.deltaTime;

            if (m_TimeUntilExtraction <= 0)
            {
                if (m_ExtractionPoint.IsPlayerInExtractionZone)
                {
                    SceneManager.LoadScene(m_WinningSceneName);
                }
                else
                {
                    if (HasLost)
                    {
                        // TODO: this loading screen says no lives lost, should be missed extraction
                        SceneManager.LoadScene(m_LosingSceneName);
                    }
                }
            }
            else if (currentTimeDisplayed > (int)m_TimeUntilExtraction)
            {
                currentTimeDisplayed = (int)m_TimeUntilExtraction;
                if (onTimerChanged != null)
                {
                    onTimerChanged(currentTimeDisplayed + 1);
                }
            }
        }

        internal void EndGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
#endregion

#region Player Events
        /// <summary>
        /// Call this whenever the player is killed.
        /// </summary>
        public void OnPlayerDeath()
        {
            livesLost++;
        }
#endregion

#region Enemy Events
        /// <summary>
        /// Call this whenever an enemy is killed.
        /// </summary>
        public void OnEnemyDeath()
        {
            score++;
        }

        public void OnPlayerCharacterChanged(ICharacter character)
        {
            if (playerHealthManager != null)
            {
                playerHealthManager.onIsAliveChanged -= OnIsAliveChaged;
            }

            if (character == null)
            {
                return;
            }
            playerHealthManager = character.GetComponent<IHealthManager>();
            playerHealthManager.onIsAliveChanged += OnIsAliveChaged;
        }

        private void OnIsAliveChaged(bool alive)
        {
            if (!alive)
            {
                livesLost++;
            }
        }
        #endregion

        #region ExtractionManager
        public float TimeToExtraction
        {
            get { return m_TimeUntilExtraction; }
        }
        #endregion
    }
}
