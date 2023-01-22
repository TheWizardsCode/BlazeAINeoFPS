using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using WizardsCode.Managers;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WizardsCode.Common
{
    /// <summary>
    /// Manages the game state and monitors for win/lose conditions.
    /// </summary>
    public abstract class AbstractGameManager : SingletonBehaviour<AbstractGameManager>
    {
        protected enum GameState { MainMenu = 100, InGameMenu = 150, InitializingGame = 200, InBriefing = 250, StartingGame = 275, InGame = 300, LossReport = 400, ExtractionReport = 500 }

        [Header("Mission")]
        [SerializeField, Tooltip("The mission currently being played.")]
        MissionDescriptor m_MissionDescriptor;
        
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
        [SerializeField, Tooltip("The name of the main menu scene used between games.")]
        internal string m_MainMenuScene = "MainMenu";
        [SerializeField, Tooltip("The UI Cancas group to display when the player loses a life.")]
        private CanvasGroup m_DeathCanvasGroup = null;
        [SerializeField, Tooltip("The UI element to turn on when we want to show the briefing.")]
        internal RectTransform m_BriefingPanel;

        [Header("Game Events")]
        [SerializeField, Tooltip("An event that is fired whenever the score changes. The parameter is the new score.")]
        public UnityAction<int> onScoreChanged;
        [SerializeField, Tooltip("An event that is fired whenever the extraction point timer changes by 1 second. The parameter is the number of seconds currently remaining.")]
        public UnityAction<int> onTimerChanged;

        InterdimensionalPortalController m_ExtractionPoint;
        private int currentTimeDisplayed;
        private WizardsCodeMissionTarget[] m_missiontargets;
        float m_TimeRemainingUntilExtraction;
        AudioSource m_AudioSource;
        protected GameState m_GameState = GameState.MainMenu;

        public MissionDescriptor mission {
            get { return m_MissionDescriptor; }
        }

        public CanvasGroup deathCanvasGroup
        {
            get { return m_DeathCanvasGroup; }
        }

        private bool IsMissionComplete
        {
            get { return AreTargetsNeutralized && score >= m_MissionDescriptor.m_ScoreNeededForTheWin; }
        }

        public int livesLost { get; private set; }   
        public int score { get; private set; }

        public bool AreTargetsNeutralized
        {
            get
            {
                int optional = 0;

                for (int i = 0; i < m_missiontargets.Length; i++)
                {
                    if (m_missiontargets[i].IsRequired)
                    {
                        if (m_missiontargets[i].isAlive) return false;
                    } else
                    {
                        if (!m_missiontargets[i].isAlive) optional++;
                    }
                }

                return optional >= m_MissionDescriptor.minOptionalTargetsForCompletion;
            }
        }

        protected virtual void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }

        /// <summary>
        /// Setup the players loadout for this mission.
        /// </summary>
        protected abstract void InitializeLoadout();

        protected virtual void InitializeMission()
        {
            m_ExtractionPoint = FindObjectOfType<InterdimensionalPortalController>();

            m_AudioSource = GetComponent<AudioSource>();

            m_TimeRemainingUntilExtraction = m_MissionDescriptor.m_MaxMissionTimeInMinutes * 60;
            currentTimeDisplayed = (int)m_TimeRemainingUntilExtraction;

            //OPTIMIZATION: have these set at design time
            m_missiontargets = FindObjectsOfType<WizardsCodeMissionTarget>(true);

            m_GameState = GameState.InBriefing;
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
                return m_TimeRemainingUntilExtraction <= 0 || livesLost >= m_MissionDescriptor.m_LivesAvailable;
            }
        }

#region Lifecycle
        void Update()
        {
            switch (m_GameState)
            {
                case GameState.MainMenu:
                    break;
                case GameState.InGameMenu:
                    break;
                case GameState.InitializingGame:
                    InitializeMission();
                    break;
                case GameState.InBriefing:
                    UpdateInBriefing();
                    break;
                case GameState.StartingGame:
                    StartGame();
                    break;
                case GameState.InGame:
                    UpdateInGame();
                    break;
                case GameState.LossReport:
                    if (SceneTimeout())
                    {
                        m_GameState = GameState.MainMenu;
                        SceneManager.LoadScene(m_MainMenuScene);
                    }
                    break;
                case GameState.ExtractionReport:
                    if (SceneTimeout())
                    {
                        m_GameState = GameState.MainMenu;
                        SceneManager.LoadScene(m_MainMenuScene);
                    }
                    break;
            }
        }

        protected virtual void StartGame()
        {
            m_GameState = GameState.InGame;
            IsBriefingComplete = true;
            m_BriefingPanel.gameObject.SetActive(false);
        }

        /// <summary>
        /// Return true if either there is a timeout after scene load or the user indicates they want to continue.
        /// </summary>
        /// <returns></returns>
        private static bool SceneTimeout(int timeout = 30)
        {
            return Time.timeSinceLevelLoad > timeout || InputAdvance();
        }

        /// <summary>
        /// Returns true if the player has hit one of the inputs that cause the current game state to advance.
        /// </summary>
        /// <returns>true if player is ready to advance</returns>
        private static bool InputAdvance()
        {
            return Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
        }

        private void UpdateInGame()
        {
            if (HasLost)
            {
                m_GameState = GameState.LossReport;
                SceneManager.LoadScene(m_MissionDescriptor.m_LosingSceneName);
                return;
            }

            m_TimeRemainingUntilExtraction -= Time.deltaTime;

            if (!HasLost && m_ExtractionPoint.CanExtract && IsMissionComplete)
            {
                SceneManager.LoadScene(m_MissionDescriptor.m_ExtractionMissionCompleteSceneName);
            }
            
            if (m_TimeRemainingUntilExtraction <= 0)
            {
                if (m_ExtractionPoint.IsPlayerInExtractionZone)
                {
                    m_GameState = GameState.ExtractionReport;
                    if (IsMissionComplete)
                    {
                        SceneManager.LoadScene(m_MissionDescriptor.m_ExtractionMissionCompleteSceneName);
                    }
                    else
                    {
                        SceneManager.LoadScene(m_MissionDescriptor.m_ExtractionMissionIncompleteSceneName);
                    }
                }
                else
                {
                    if (HasLost)
                    {
                        // TODO: this loading screen says the same thing regadless of mission failure status
                        m_GameState = GameState.LossReport;
                        SceneManager.LoadScene(m_MissionDescriptor.m_LosingSceneName);
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

        private void UpdateInBriefing()
        {
            if (!IsBriefingComplete)
            {
                m_BriefingPanel.GetComponentInChildren<TextMeshProUGUI>().text = m_MissionDescriptor.m_Description;
                m_BriefingPanel.gameObject.SetActive(true);

                if (Time.timeSinceLevelLoad > 30 || InputAdvance())
                {
                    m_GameState = GameState.StartingGame;
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


        #region Enemy Events
        /// <summary>
        /// Call this whenever an enemy is killed.
        /// </summary>
        public void OnEnemyDeath()
        {
            score++;
        }

        protected virtual void OnIsAliveChaged(bool alive)
        {
            if (alive)
            {
                m_DeathCanvasGroup.alpha = 0f;
            }
            else
            {
                livesLost++;
            }
            gameObject.SetActive(!alive);
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
