#if NEOFPS
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WizardsCode.Optimization;
using NeoFPS;

namespace WizardsCode.Optimization
{
    public class NeoProximityActivationManager : ProximityActivationManager, IPlayerCharacterSubscriber
    {
        public void OnPlayerCharacterChanged(ICharacter character)
        {
            if (character != null)
            {
                m_ProximityTarget = character.transform;
            }
        }

        private void Awake()
        {
            FindObjectOfType<SoloPlayerCharacterEventWatcher>().AttachSubscriber(this);
        }
    }
}
#endif