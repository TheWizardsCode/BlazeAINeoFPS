using UnityEngine;

namespace WizardsCode.Common
{
    /// <summary>
    /// A mission target is an object that is marked as either a required or optional mission target.
    /// If marketed as required these must be neutralized (killed, collected, destroyed) in order to
    /// complete the mission. If marked as not-required these may be used as optional targets in some
    /// missions, for example, some missions may require that a certain number of targets are nuetralized.
    /// </summary>
    public class WizardsCodeMissionTarget : MonoBehaviour, IWizardsCodeHealthManager
    {
        [SerializeField, Tooltip("Is it required that this target be neutralized for the mission to be complete?")]
        bool m_isRequiredForMissionCompletion = true;

        public bool IsRequired { get { return m_isRequiredForMissionCompletion; } }

        public bool isAlive { get; set; }
    }
}
