using NeoFPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WizardsCode.UnnofficialNeoFPSExtension
{
    /// <summary>
    /// This is a Neo FPS health manager for Blaze AI. It will connect
    /// the two systems together and notify the AI when it has died.
    /// </summary>
    public class BlazeNeoHealthManager : BasicHealthManager
    {
        BlazeAI blaze;

        protected override void Awake()
        {
            blaze = GetComponent<BlazeAI>();
        }

        protected override void OnIsAliveChanged()
        {
            if (!isAlive)
            {
                blaze.Death();
            }
            base.OnIsAliveChanged();
        }
    }
}