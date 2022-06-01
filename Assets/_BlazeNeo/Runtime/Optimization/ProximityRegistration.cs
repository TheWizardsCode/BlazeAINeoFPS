using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace WizardsCode.Optimization
{
    /// <summary>
    /// This component will cause the object to self register with the `ProximityActivationManager`
    /// in the scene when it awakes. This will allow the object to be disabled when it is a defined 
    /// distance from the target (as managed by the `ProximityActivationManager`.
    /// 
    /// Note that the object must be capable of being enabled/disabled without losing context within
    /// the game.
    /// </summary>
    public class ProximityRegistration : MonoBehaviour
    {
        [SerializeField, Tooltip("The distance at which the object is considered nearby and thus should be enabled.")]
        float m_NearDistance = 20;
        [SerializeField, Tooltip("The distance at which the object is considered far away and thus should be disabled.")]
        float m_FarDistance = 30;

        /// <summary>
        /// The square of the distance at which the object is considered nearby and thus should be enabled.
        /// </summary>
        public float NearDistanceSqr
        {
            get; private set;
        }

        /// <summary>
        /// The square of the distance at which the object is considered far away and thus should be disabled.
        /// </summary>
        public float FarDistanceSqr
        {
            get; private set;
        }

        /// <summary>
        /// Indicates whether this object has been disabled due to proximity to the target or not.
        /// </summary>
        public bool DisabledByProximity
        {
            get; private set;
        }

        /// <summary>
        /// Disable this object because of its proximity check.
        /// </summary>
        public void Disable()
        {
            DisabledByProximity = true;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Disable this object because of its proximity check.
        /// </summary>
        public void Enable()
        {
            DisabledByProximity = false;
            gameObject.SetActive(true);
        }
        private void Awake()
        {
            NearDistanceSqr = m_NearDistance * m_NearDistance;
            FarDistanceSqr = m_FarDistance * m_FarDistance;

            //OPTIMIZATION: make the ProximityActivationManager a singleton
            ProximityActivationManager manager = GameObject.FindObjectOfType<ProximityActivationManager>();
            if (manager) manager.Add(this);
        }
    }
}
