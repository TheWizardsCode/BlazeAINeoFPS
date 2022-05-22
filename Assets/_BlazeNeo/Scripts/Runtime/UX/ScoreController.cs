using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WizardsCode.BlazeNeoFPS.UI
{
    public class ScoreController : MonoBehaviour
    {
        [SerializeField, Tooltip("The text component to display the score.")]
        Text m_ScoreText;

        int targetScore;

        void Start()
        {
            GameManager manager = GameObject.FindObjectOfType<GameManager>();
            if (manager)
            {
                manager.onScoreChanged += OnScoreChanged;
                targetScore = manager.m_ScoreNeededForTheWin;
            } else
            {
                Debug.LogWarning("Unable to find GameManager to manage the score. Disabling the Score HUD component.");
                Destroy(gameObject);
            }

            OnScoreChanged(0);
        }

        private void OnScoreChanged(int score)
        {
            m_ScoreText.text = $"{score} / {targetScore}";
        }
    }
}