using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WizardsCode.Common
{
    /// <summary>
    /// Marks an object as a source of a score.
    /// </summary>
    public class ScoreSource : MonoBehaviour
    {
        AbstractGameManager manager;

        private void Start()
        {
            manager = GameObject.FindObjectOfType<AbstractGameManager>();
            if (manager == null)
            {
                Debug.LogWarning("No game manager found, destroying ScoreSource as it requires one.");  
                Destroy(this);
            }
        }

        public void AddScore(int score)
        {
            manager.AddScore(score);
        }
    }
}