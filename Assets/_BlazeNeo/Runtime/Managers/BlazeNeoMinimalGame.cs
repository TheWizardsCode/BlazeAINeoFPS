using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NeoFPS.SinglePlayer;

namespace WizardsCode.BlazeNeoFPS
{
    public class BlazeNeoMinimalGame : FpsSoloGameMinimal
    {
        protected override void Awake()
        {
            base.Awake();
            inGame = false;
        }
    }
}
