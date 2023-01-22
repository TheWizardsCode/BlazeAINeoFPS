using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WizardsCode.Common.UI
{
    public class UIProgressController : MonoBehaviour
    {
        [SerializeField, Tooltip("The text component to display the score.")]
        TMP_Text m_ScoreText;
        [SerializeField, Tooltip("The text component to display the time remaining until extract.")]
        TMP_Text m_TimerText;

        int targetScore;

        void OnEnable()
        {
            AbstractGameManager.Instance.onScoreChanged += OnScoreChanged;
            targetScore = AbstractGameManager.Instance.mission.m_ScoreNeededForTheWin;
            OnScoreChanged(0);

            AbstractGameManager.Instance.onTimerChanged += OnTimerChanged;
            OnTimerChanged((int)AbstractGameManager.Instance.TimeToExtraction);
        }

        private void OnDisable()
        {
            AbstractGameManager.Instance.onScoreChanged -= OnScoreChanged;
            AbstractGameManager.Instance.onTimerChanged -= OnTimerChanged;
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