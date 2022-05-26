using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WizardsCode.BlazeNeoFPS
{
    public class ExtractionPointController : MonoBehaviour
    {
        public bool IsPlayerInExtractionZone
        {
            get; private set;
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
