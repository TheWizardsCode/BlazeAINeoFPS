using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WizardsCode.Optimization;
using NeoFPS;
using System;

namespace WizardsCode.Optimization
{
    public class NeoProximityRegistration : ProximityRegistration
    {
        BasicHealthManager health;

        private void IsAliveChanged(bool alive)
        {
            if (!alive)
            {
                Unregister();
                Destroy(this);
            }
        }

        public override void Register()
        {
            base.Register();

            health = GetComponentInChildren<BasicHealthManager>();
            if (health != null)
            {
                health.onIsAliveChanged += IsAliveChanged;
            }
        }

        public override void Unregister()
        {
            base.Unregister();
            if (health != null)
            {
                health.onIsAliveChanged -= IsAliveChanged;
            }
        }

    }
}
