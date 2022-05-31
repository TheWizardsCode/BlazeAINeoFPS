using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WizardsCode.BlazeNeoFPS.UI
{
    public class UIProgressController : MonoBehaviour
    {
        [SerializeField, Tooltip("The text component to display the score.")]
        Text m_ScoreText;
        [SerializeField, Tooltip("The text component to display the time remaining until extract.")]
        Text m_TimerText;

        int targetScore;

        void OnEnable()
        {
            GameManager.Instance.onScoreChanged += OnScoreChanged;
            targetScore = GameManager.Instance.mission.m_ScoreNeededForTheWin;
            OnScoreChanged(0);

            GameManager.Instance.onTimerChanged += OnTimerChanged;
            OnTimerChanged((int)GameManager.Instance.TimeToExtraction);
        }

        private void OnDisable()
        {
            GameManager.Instance.onScoreChanged -= OnScoreChanged;
            GameManager.Instance.onTimerChanged -= OnTimerChanged;
        }

        private void OnScoreChanged(int score)
        {
            m_ScoreText.text = $"{score} / {targetScore}";
        }

        private void OnTimerChanged(int totalSeconds)
        {
            int seconds = totalSeconds % 60;
            int minutes = totalSeconds / 60;

            m_TimerText.text = $"{minutes}:{String.Format("{0:00}", seconds)}";
        }
    }
}