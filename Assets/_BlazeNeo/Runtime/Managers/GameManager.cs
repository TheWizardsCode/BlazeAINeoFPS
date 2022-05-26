using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using WizardsCode.Managers;
using NeoFPS;
using UnityEngine.Playables;
using NeoFPS.SinglePlayer;
using TMPro;

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
        [Header("Mission")]
        [SerializeField, Tooltip("The game mode for this mission.")]
        private FpsSoloGameMinimal m_GameMode;
        [SerializeField, Tooltip("The mission currently being played.")]
        internal MissionDescriptor m_Mission;
        
        [Header("Timer")]
        [SerializeField]
        AudioClip m_AudioClip5Minutes;
        [SerializeField]
        AudioClip m_AudioClip2Minutes;
        [SerializeField]
        AudioClip m_AudioClip1Minute;
        [SerializeField]
        AudioClip m_AudioClip30Seconds;
        [SerializeField]
        AudioClip m_AudioClip10Seconds;
        [SerializeField]
        AudioClip m_AudioClip7Seconds;
        [SerializeField]
        AudioClip m_AudioClip5Seconds;
        [SerializeField]
        AudioClip m_AudioClip4Seconds;
        [SerializeField]
        AudioClip m_AudioClip3Seconds;
        [SerializeField]
        AudioClip m_AudioClip2Seconds;
        [SerializeField]
        AudioClip m_AudioClip1Second;

        [Header("UI")]
        [SerializeField, Tooltip("The UI element to turn on when we want to show the briefing.")]
        internal RectTransform m_BriefingPanel;

        [Header("Game Events")]
        [SerializeField, Tooltip("An event that is fired whenever the score changes. The parameter is the new score.")]
        public UnityAction<int> onScoreChanged;
        [SerializeField, Tooltip("An event that is fired whenever the extraction point timer changes by 1 second. The parameter is the number of seconds currently remaining.")]
        public UnityAction<int> onTimerChanged;

        ExtractionPointController m_ExtractionPoint;
        private int currentTimeDisplayed;
        private IHealthManager playerHealthManager;
        float m_TimeRemainingUntilExtraction;
        AudioSource m_AudioSource;

        public int livesLost { get; private set; }   
        public int score { get; private set; }

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            m_ExtractionPoint = FindObjectOfType<ExtractionPointController>();

            FindObjectOfType<SoloPlayerCharacterEventWatcher>().AttachSubscriber(this);

            m_AudioSource = GetComponent<AudioSource>();

            InitializeMission();
        }

        private void InitializeMission()
        {
            m_TimeRemainingUntilExtraction = m_Mission.m_MaxMissionTimeInMinutes * 60;

            if (m_Mission.m_KillTarget != null)
            {
                Instantiate(m_Mission.m_KillTarget);
            }
        }

        private void Start()
        {
            currentTimeDisplayed = (int)m_TimeRemainingUntilExtraction;
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
                return m_TimeRemainingUntilExtraction <= 0 || livesLost >= m_Mission.m_LivesAvailable;
            }
        }

        #region Lifecycle
        void Update()
        {
            if (!FpsGameMode.inGame)
            {
                return;
            }

            if (!IsBriefingComplete)
            {
                m_BriefingPanel.GetComponentInChildren<TextMeshProUGUI>().text = m_Mission.m_Description;
                m_BriefingPanel.gameObject.SetActive(true);

                if (Time.timeSinceLevelLoad > 30 || Input.GetKeyDown(KeyCode.Space))
                {
                    m_GameMode.Respawn(m_GameMode.player);
                    IsBriefingComplete = true;
                    m_BriefingPanel.gameObject.SetActive(false);
                }
            }
             
            if (HasLost)
            {
                SceneManager.LoadScene(m_Mission.m_LosingSceneName);
                return;
            }

            m_TimeRemainingUntilExtraction -= Time.deltaTime;

            if (m_TimeRemainingUntilExtraction <= 0)
            {
                if (m_ExtractionPoint.IsPlayerInExtractionZone)
                {
                    if (m_Mission.AreTargetsNeutralized && score >= m_Mission.m_ScoreNeededForTheWin)
                    {
                        SceneManager.LoadScene(m_Mission.m_ExtractionMissionCompleteSceneName);
                    } else
                    {
                        SceneManager.LoadScene(m_Mission.m_ExtractionMissionIncompleteSceneName);
                    }
                }
                else
                {
                    if (HasLost)
                    {
                        // TODO: this loading screen says the same thing regadless of mission failure status
                        SceneManager.LoadScene(m_Mission.m_LosingSceneName);
                    }
                }
            }
            else if (currentTimeDisplayed > (int)m_TimeRemainingUntilExtraction)
            {
                currentTimeDisplayed = (int)m_TimeRemainingUntilExtraction;
                if (onTimerChanged != null)
                {
                    onTimerChanged(currentTimeDisplayed + 1);
                    TimeAnnouncements(currentTimeDisplayed + 1);
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
            get { return m_TimeRemainingUntilExtraction; }
        }

        public bool IsBriefingComplete { get; private set; }

        /// <summary>
        /// Make announcements relating to the time to extraction.
        /// </summary>
        void TimeAnnouncements(int time)
        {
            if (time == 300)
            {
                m_AudioSource.clip = m_AudioClip5Minutes;
                m_AudioSource.Play();
                return;
            } else if (time == 120)
            {
                m_AudioSource.clip = m_AudioClip2Minutes;
                m_AudioSource.Play();
                return;
            } else  if (time == 60)
            {
                m_AudioSource.clip = m_AudioClip1Minute;
                m_AudioSource.Play();
                return;
            } else if (time == 30)
            {
                m_AudioSource.clip = m_AudioClip30Seconds;
                m_AudioSource.Play();
                return;
            } else if (time == 10)
            {
                m_AudioSource.clip = m_AudioClip10Seconds;
                m_AudioSource.Play();
                return;
            }
            else if (time == 7)
            {
                m_AudioSource.clip = m_AudioClip7Seconds;
                m_AudioSource.Play();
                return;
            }
            else if (time == 5)
            {
                m_AudioSource.clip = m_AudioClip5Seconds;
                m_AudioSource.Play();
                return;
            }
            else if (time == 4)
            {
                m_AudioSource.clip = m_AudioClip4Seconds;
                m_AudioSource.Play();
                return;
            }
            else if (time == 3)
            {
                m_AudioSource.clip = m_AudioClip3Seconds;
                m_AudioSource.Play();
                return;
            }
            else if (time == 2)
            {
                m_AudioSource.clip = m_AudioClip2Seconds;
                m_AudioSource.Play();
                return;
            }
            else if (time == 1)
            {
                m_AudioSource.clip = m_AudioClip1Second;
                m_AudioSource.Play();
                return;
            }
        }
#endregion
    }
}
