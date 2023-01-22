using NeoFPS;
using WizardsCode.Common;

namespace WizardsCode.UnnofficialNeoFPSExtension
{
    /// <summary>
    /// This is a Neo FPS health manager for Blaze AI. It will connect
    /// the two systems together and notify the AI when it has died.
    /// </summary>
    public class BlazeNeoHealthManager : BasicHealthManager, IWizardsCodeHealthManager
    {
        BlazeAI blaze;

        public new bool isAlive { 
            get { return base.isAlive; } 
            set {  base.isAlive = value; }
        }

        protected override void Awake()
        {
            blaze = GetComponentInChildren<BlazeAI>();
        }

        /// <summary>
        /// This is the event fired by NeoFPS whenever IsAlice is changed.
        /// </summary>
        protected override void OnIsAliveChanged()
        {
            if (!isAlive && blaze != null)
            {
                blaze.Death();
            }

            base.OnIsAliveChanged();
        }
    }
}