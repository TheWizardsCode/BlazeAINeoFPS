using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WizardsCode.BlazeNeoFPS
{
    public class ExtractionPointController : MonoBehaviour
    {
        [SerializeField, Tooltip("The radius that must be clear of enemies for the extraction to occur.")]
        private int m_ClearRadius = 100;

        Collider[] m_EnemiesPresent = new Collider[1];

        /// <summary>
        /// Return true if the player can be extracted right now.
        /// </summary>
        public bool CanExtract
        {
            get
            {
                return IsPlayerInExtractionZone && IsRadiusClear;
            }
        }

        /// <summary>
        /// Is the player within the extraction zone right now.
        /// </summary>
        public bool IsPlayerInExtractionZone
        {
            get; private set;
        }

        /// <summary>
        /// Are there any enemies within the required clear radius at this time?
        /// </summary>
        private bool IsRadiusClear { 
            get
            {
                return Physics.OverlapSphereNonAlloc(transform.position, m_ClearRadius, m_EnemiesPresent) > 0;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                IsPlayerInExtractionZone = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                IsPlayerInExtractionZone = false;
            }
        }
    }
}
