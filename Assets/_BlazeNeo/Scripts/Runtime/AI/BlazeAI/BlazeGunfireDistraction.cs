using NeoFPS.ModularFirearms;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WizardsCode.UnnofficialNeoFPSExtension
{
    /// <summary>
    /// Uses the Blaze AI distraction system to simulate hearing in the AI. Attach this to your NeoFPS modular weapons,
    /// set the range and other properties and you are done.
    /// </summary>
    public class BlazeGunfireDistraction : BlazeAIDistraction
    {
        BaseTriggerBehaviour trigger;

        private void OnEnable()
        {
            trigger = GetComponent<BaseTriggerBehaviour>();
            trigger.onShoot += OnShoot;
        }

        private void OnDisable()
        {
            if (trigger != null)
            {
                trigger.onShoot -= OnShoot;
            }
        }

        private void OnShoot()
        {
            TriggerDistraction();
        }
    }
}
